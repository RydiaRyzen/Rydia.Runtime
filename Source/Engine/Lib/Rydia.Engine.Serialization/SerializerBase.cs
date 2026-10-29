using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// オブジェクトのシリアライズおよびデシリアライズの
    /// 共通機能を提供するベースクラスです。
    /// </summary>
    public abstract class SerializerBase
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
        /// オブジェクトをシリアライズします。
        /// </summary>
        /// <param name="obj">シリアライズするオブジェクト。</param>
        public void Serialize(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            Type type = obj.GetType();

            // デシリアライズ時に使用する型情報
            Writer.WriteValue(
                TypeKey,
                type.AssemblyQualifiedName
                    ?? throw new InvalidOperationException(
                        "型情報を取得できません。"));

            // プロパティを書き込む
            foreach (PropertyInfo property in GetSerializableProperties(type))
            {
                Writer.WriteValue(
                    property.Name,
                    property.GetValue(obj));
            }

            // プロパティと重複しない公開フィールドを書き込む
            foreach (FieldInfo field in GetSerializableFields(type))
            {
                Writer.WriteValue(
                    field.Name,
                    field.GetValue(obj));
            }
        }

        /// <summary>
        /// データをデシリアライズしてオブジェクトを復元します。
        /// </summary>
        /// <returns>復元されたオブジェクト。</returns>
        public object Deserialize()
        {
            string typeName = Reader.ReadValue<string>(TypeKey);

            Type type = Type.GetType(typeName)
                ?? throw new InvalidOperationException(
                    $"型 '{typeName}' を解決できません。");

            object obj = Activator.CreateInstance(type)
                ?? throw new InvalidOperationException(
                    $"型 '{type.FullName}' のインスタンスを生成できません。");

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

            return obj;
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

            return type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(f =>
                    !f.IsStatic &&
                    !f.IsInitOnly &&
                    !propertyNames.Contains(f.Name))
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
