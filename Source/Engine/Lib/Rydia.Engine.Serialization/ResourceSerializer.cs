using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// Resource クラスの派生型をシリアライズおよびデシリアライズするための
    /// 基底クラスを表します。
    /// </summary>
    /// <typeparam name="TResource">
    /// シリアライズ対象となる Resource クラスの派生型を指定します。
    /// </typeparam>
    public abstract class ResourceSerializer<TResource>
        where TResource : Resource
    {

        private const string TypeKey = "$type";

        /// <summary>
        /// データを書き込むためのライターを取得します。
        /// </summary>
        protected abstract IDataWriter Writer { get; }

        /// <summary>
        /// データを読み取るためのリーダーを取得します。
        /// </summary>
        protected abstract IDataReader Reader { get; }

        /// <summary>
        /// 指定された名前でデータを書き込みます。
        /// </summary>
        /// <param name="name">書き込むデータの名前。</param>
        /// <param name="value">書き込むデータ。</param>
        public void WriteValue(string name, object value)
        {
            Writer.WriteValue(name, value);
        }

        /// <summary>
        /// 指定された名前のデータを読み取ります。
        /// </summary>
        /// <param name="name">読み取るデータの名前。</param>
        /// <returns>読み取ったデータ。</returns>
        public object ReadValue(string name)
        {
            return Reader.ReadValue(name);
        }

        /// <summary>
        /// 指定された名前のデータを指定した型で読み取ります。
        /// </summary>
        /// <typeparam name="T">読み取るデータの型。</typeparam>
        /// <param name="name">読み取るデータの名前。</param>
        /// <returns>読み取ったデータ。</returns>
        public T ReadValue<T>(string name)
        {
            return Reader.ReadValue<T>(name);
        }

        /// <summary>
        /// 指定された名前のデータを読み取り、出力引数に格納します。
        /// </summary>
        /// <typeparam name="T">読み取るデータの型。</typeparam>
        /// <param name="name">読み取るデータの名前。</param>
        /// <param name="value">読み取ったデータ。</param>
        public void ReadValue<T>(string name, out T value)
        {
            Reader.ReadValue(name, out value);
        }

        /// <summary>
        /// 指定した Resource をストリームにシリアライズします。
        /// </summary>
        /// <param name="resource">シリアライズ対象の Resource。</param>
        /// <param name="stream">シリアライズ先のストリーム。</param>
        protected void Serialize(TResource resource)
        {
           
            Type type = resource.GetType();

            // デシリアライズ時に使用する型情報
            Writer.WriteValue(
                TypeKey,
                type.AssemblyQualifiedName
                    ?? throw new InvalidOperationException(
                        "型情報を取得できません。"));

            if (typeof(ICustomSerializable).IsAssignableFrom(type))
            {
                ((ICustomSerializable)resource).WriteData(Writer);
            }
            else
            {
                // プロパティを書き込む
                foreach (PropertyInfo property in GetSerializableProperties(type))
                {
                    Writer.WriteValue(
                        property.Name,
                        property.GetValue(resource));
                }

                // プロパティと重複しない公開フィールドを書き込む
                foreach (FieldInfo field in GetSerializableFields(type))
                {
                    Writer.WriteValue(
                        field.Name,
                        field.GetValue(resource));
                }
            }

        }

        /// <summary>
        /// ストリームから Resource をデシリアライズします。
        /// </summary>
        /// <param name="stream">デシリアライズ元のストリーム。</param>
        /// <returns>デシリアライズされた Resource。</returns>
        protected TResource Deserialize()
        {

            string typeName = Reader.ReadValue<string>(TypeKey);

            Type type = Type.GetType(typeName)
        ?? throw new InvalidOperationException(
            $"型 '{typeName}' を解決できません。");

            if (!typeof(TResource).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    $"型 '{type.FullName}' は '{typeof(TResource).FullName}' と互換性がありません。");
            }

            object obj = Activator.CreateInstance(type)
                ?? throw new InvalidOperationException(
                    $"型 '{type.FullName}' のインスタンスを生成できません。");

            if (obj is ICustomSerializable customSerializable)
            {
                customSerializable.ReadData(Reader);
            }
            else
            {
                foreach (PropertyInfo property in GetSerializableProperties(type))
                {
                    if (!Reader.Keys.Contains(property.Name))
                        continue;

                    object value = Reader.ReadValue(property.Name);

                    property.SetValue(
                        obj,
                        ConvertValue(value, property.PropertyType));
                }

                foreach (FieldInfo field in GetSerializableFields(type))
                {
                    if (!Reader.Keys.Contains(field.Name))
                        continue;

                    object value = Reader.ReadValue(field.Name);

                    field.SetValue(
                        obj,
                        ConvertValue(value, field.FieldType));
                }
            }

            

            return (TResource)obj;
        }

        /// <summary>
        /// シリアライズ対象のプロパティを取得します。
        /// </summary>
        private static PropertyInfo[] GetSerializableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                    p.GetIndexParameters().Length == 0 &&
                    p.GetMethod is { IsPublic: true, IsStatic: false } &&
                    p.SetMethod is { IsPublic: true, IsStatic: false })
                .ToArray();
        }

        /// <summary>
        /// シリアライズ対象のフィールドを取得します。
        /// </summary>
        private static FieldInfo[] GetSerializableFields(Type type)
        {
            string[] propertyNames = GetSerializableProperties(type)
                .Select(p => p.Name)
                .ToArray();

            return type.GetFields(
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic)
                .Where(f =>
                !f.IsStatic &&
                !f.IsInitOnly &&
                !propertyNames.Contains(f.Name) &&
                !Attribute.IsDefined(f, typeof(DontSerializeAttribute)))
                .ToArray();
        }

        /// <summary>
        /// 読み取った値を指定された型に変換します。
        /// </summary>
        private static object ConvertValue(object value, Type targetType)
        {
            if (value == null)
            {
                if (targetType.IsValueType &&
                    Nullable.GetUnderlyingType(targetType) == null)
                {
                    throw new InvalidOperationException(
                        $"型 '{targetType.FullName}' に null を設定できません。");
                }

                return null;
            }

            Type actualType =
                Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (actualType.IsInstanceOfType(value))
                return value;

            if (actualType.IsEnum)
            {
                return value is string text
                    ? Enum.Parse(actualType, text)
                    : Enum.ToObject(actualType, value);
            }

            return Convert.ChangeType(
                value,
                actualType,
                CultureInfo.InvariantCulture);
        }

    }

}
