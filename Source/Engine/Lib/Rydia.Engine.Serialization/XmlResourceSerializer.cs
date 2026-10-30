using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.AccessControl;
using System.Text;
using System.Xml;
using Rydia.Engine.Resources;

namespace Rydia.Engine.Serialization
{

    public sealed class XmlResourceSerializer<TResource>
    : ResourceSerializer<TResource>
    where TResource : Resource
    {

        /// <summary>
        /// XML 形式でデータを書き込むためのクラスです。
        /// </summary>
        public class XmlDataWriter : IDataWriter
        {
            private readonly XmlDocument document;
            private readonly XmlElement root;

            /// <summary>
            /// XML データを書き込むためのインスタンスを初期化します。
            /// </summary>
            public XmlDataWriter()
            {
                this.document = new XmlDocument();

                this.root = this.document.CreateElement("Data");
                this.document.AppendChild(this.root);
            }

            /// <summary>
            /// XML ドキュメントを取得します。
            /// </summary>
            public XmlDocument Document
            {
                get
                {
                    return this.document;
                }
            }

            /// <summary>
            /// 指定された名前でデータを書き込みます。
            /// </summary>
            /// <param name="name">書き込むデータの名前。</param>
            /// <param name="value">書き込むデータ。</param>
            public void WriteValue(string name, object value)
            {
                ArgumentException.ThrowIfNullOrEmpty(name);

                XmlElement element = this.document.CreateElement("Item");
                element.SetAttribute("Name", name);

                WriteElementValue(element, value);

                // 同名要素が存在する場合は置き換える
                foreach (XmlNode node in this.root.ChildNodes)
                {
                    if (node is XmlElement existing &&
                        existing.GetAttribute("Name") == name)
                    {
                        this.root.ReplaceChild(element, existing);
                        return;
                    }
                }

                this.root.AppendChild(element);
            }

            private void WriteElementValue(XmlElement element, object value)
            {
                if (value is null)
                {
                    element.SetAttribute("Type", "null");
                    return;
                }

                Type valueType = value is Type
                    ? typeof(Type)
                    : value.GetType();

                element.SetAttribute(
                    "Type",
                    valueType.AssemblyQualifiedName);

                if (value is IDictionary dictionary)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        XmlElement child = Document.CreateElement("Item");
                        child.SetAttribute("Name", entry.Key.ToString());

                        WriteElementValue(child, entry.Value);
                        element.AppendChild(child);
                    }

                    return;
                }
                if (value is Type typeValue)
                {
                    element.InnerText = typeValue.AssemblyQualifiedName ??
                        typeValue.FullName
                        ?? typeValue.Name;
                    return;
                }

                if (value is bool boolean)
                {
                    element.InnerText = XmlConvert.ToString(boolean);
                    return;
                }
                if (value.GetType().IsEnum)
                {
                    element.SetAttribute("Type", value.GetType().AssemblyQualifiedName);
                    element.InnerText = value.ToString();
                    return;
                }
                if (value is Array array)
                {
                    foreach (object item in array)
                    {
                        XmlElement child = this.document.CreateElement("Value");
                        WriteElementValue(child, item);
                        element.AppendChild(child);
                    }

                    return;
                }
                if (value is IFormattable formattable)
                {
                    element.InnerText = formattable.ToString(
                        null,
                        CultureInfo.InvariantCulture);

                    return;
                }
                if (value is IContentRef contentRef)
                {
                    element.SetAttribute(
                        "Type",
                        value.GetType().AssemblyQualifiedName);

                    element.InnerText = contentRef.Res?.Path ?? string.Empty;
                    return;
                }
                element.InnerText = value.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// XML 形式のデータを読み取るためのクラスです。
        /// </summary>
        public class XmlDataReader : IDataReader
        {
            private readonly XmlDocument document;
            private readonly XmlElement root;

            /// <summary>
            /// XML ドキュメントからデータを読み取るためのインスタンスを初期化します。
            /// </summary>
            /// <param name="document">読み取る XML ドキュメント。</param>
            public XmlDataReader(XmlDocument document)
            {
                ArgumentNullException.ThrowIfNull(document);

                this.document = document;

                this.root = document.DocumentElement
                    ?? throw new ArgumentException(
                        "XML ドキュメントにルート要素がありません。",
                        nameof(document));
            }

            /// <summary>
            /// 読み取り可能なデータのキー一覧を取得します。
            /// </summary>
            public IEnumerable<string> Keys
            {
                get
                {
                    return this.root.ChildNodes
                    .OfType<XmlElement>()
                    .Where(e => e.Name == "Item")
                    .Select(e => e.GetAttribute("Name"));
                }
            }

            /// <summary>
            /// 指定された名前のデータを読み取ります。
            /// </summary>
            /// <param name="name">読み取るデータの名前。</param>
            /// <returns>読み取ったデータ。</returns>
            public object ReadValue(string name)
            {
                return ReadElementValue(FindElement(name));
            }

            /// <summary>
            /// XML 要素から値を読み取ります。
            /// </summary>
            /// <param name="element">読み取る XML 要素。</param>
            /// <returns>読み取った値。</returns>
            private object ReadElementValue(XmlElement element)
            {
                string typeName = element.GetAttribute("Type");

                if (typeName == "null")
                    return null;

                Type type = Type.GetType(typeName)
                    ?? throw new InvalidOperationException(
                        $"型 '{typeName}' を解決できません。");

                string text = element.InnerText;

                if (type == typeof(string))
                    return text;

                if (type == typeof(Type))
                {
                    return Type.GetType(text)
                        ?? throw new InvalidOperationException(
                            $"型 '{text}' を解決できません。");
                }

                if (type.IsArray)
                {
                    Type elementType = type.GetElementType()
                        ?? throw new InvalidOperationException(
                            $"配列 '{type}' の要素型を取得できません。");

                    XmlElement[] values = element.ChildNodes
                        .OfType<XmlElement>()
                        .Where(e => e.Name == "Value")
                        .ToArray();

                    Array array = Array.CreateInstance(elementType, values.Length);

                    for (int i = 0; i < values.Length; i++)
                    {
                        object item = ReadElementValue(values[i]);

                        if (item != null && !elementType.IsInstanceOfType(item))
                        {
                            throw new InvalidOperationException(
                                $"配列 '{type}' の要素型と読み取った値の型が一致しません。");
                        }

                        array.SetValue(item, i);
                    }

                    return array;
                }

                if (type.IsEnum)
                    return Enum.Parse(type, element.InnerText);

                if (type == typeof(bool))
                    return XmlConvert.ToBoolean(text);

                if (type == typeof(byte))
                    return XmlConvert.ToByte(text);

                if (type == typeof(sbyte))
                    return XmlConvert.ToSByte(text);

                if (type == typeof(short))
                    return XmlConvert.ToInt16(text);

                if (type == typeof(ushort))
                    return XmlConvert.ToUInt16(text);

                if (type == typeof(int))
                    return XmlConvert.ToInt32(text);

                if (type == typeof(uint))
                    return XmlConvert.ToUInt32(text);

                if (type == typeof(long))
                    return XmlConvert.ToInt64(text);

                if (type == typeof(ulong))
                    return XmlConvert.ToUInt64(text);

                if (type == typeof(float))
                    return XmlConvert.ToSingle(text);

                if (type == typeof(double))
                    return XmlConvert.ToDouble(text);

                if (type == typeof(decimal))
                    return XmlConvert.ToDecimal(text);

                if (type == typeof(char))
                    return XmlConvert.ToChar(text);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ContentRef<>))
                {
                    string path = element.InnerText;
                    Type resourceType = type.GetGenericArguments()[0];
                    // path から ttype 型の Resource を取得
                    Resource resource = ResourceCache.Load(path, resourceType);

                    if (resource == null)
                    {
                        throw new InvalidOperationException(
                            $"リソースを取得できませんでした。Path: {path}, Type: {resourceType.FullName}");
                    }
                    // ContentRef<T> を生成
                    return Activator.CreateInstance(
                        type,
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.NonPublic,
                        binder: null,
                        args: new object[] { resource },
                        culture: CultureInfo.InvariantCulture
                        );
                }
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] arguments = type.GetGenericArguments();
                    Type keyType = arguments[0];
                    Type valueType = arguments[1];

                    IDictionary dictionary = (IDictionary)Activator.CreateInstance(type);

                    foreach (XmlElement item in element.ChildNodes
                        .OfType<XmlElement>()
                        .Where(e => e.Name == "Item"))
                    {
                        string keyText = item.GetAttribute("Name");

                        object key = keyType == typeof(char)
                            ? keyText[0]
                            : Convert.ChangeType(
                                keyText,
                                keyType,
                                CultureInfo.InvariantCulture);


                        object value = ReadElementValue(item);
                        dictionary.Add(key, value);
                    }

                    return dictionary;
                }

                return Convert.ChangeType(
                    text,
                    type,
                    CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// 指定された名前のデータを指定した型で読み取ります。
            /// </summary>
            /// <typeparam name="T">読み取るデータの型。</typeparam>
            /// <param name="name">読み取るデータの名前。</param>
            /// <returns>読み取ったデータ。</returns>
            public T ReadValue<T>(string name)
            {
                object value = ReadValue(name);

                if (value == null)
                    return default;

                if (value is T result)
                    return result;

                return (T)Convert.ChangeType(
                    value,
                    Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T),
                    CultureInfo.InvariantCulture);
            }

            /// <summary>
            /// 指定された名前のデータを読み取り、出力引数に格納します。
            /// </summary>
            /// <typeparam name="T">読み取るデータの型。</typeparam>
            /// <param name="name">読み取るデータの名前。</param>
            /// <param name="value">読み取ったデータ。</param>
            public void ReadValue<T>(string name, out T value)
            {
                value = ReadValue<T>(name);
            }

            private XmlElement FindElement(string name)
            {
                foreach (XmlNode node in this.root.ChildNodes)
                {
                    if (node is XmlElement element &&
                        element.Name == "Item" &&
                        element.GetAttribute("Name") == name)
                    {
                        return element;
                    }
                }

                throw new KeyNotFoundException(
                    $"キー '{name}' が XML データに存在しません。");
            }
        }
        private XmlDataWriter xmlDataWriter;
        private XmlDataReader xmlDataReader;

        protected override IDataWriter Writer
        {
            get
            {
                return this.xmlDataWriter;
            }
        }

        protected override IDataReader Reader
        {
            get
            {
                return this.xmlDataReader;
            }
        }

        public TResource Deserialize(string path)
        {
            using (var fr = File.OpenRead(path))
            {
                return Deserialize(fr);
            }
        }

        public TResource Deserialize(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);

            if (!stream.CanRead)
            {
                throw new ArgumentException(
                    "ストリームから読み込むことができません。",
                    nameof(stream));
            }
            var document = new XmlDocument();
            document.Load(stream);

            this.xmlDataReader = new XmlDataReader(document);
            return base.Deserialize();
        }

        public void Serialize(TResource resource, string path)
        {
            using (var fs = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                Serialize(resource, fs);
            }
        }

        public void Serialize(TResource resource, Stream stream)
        {
            ArgumentNullException.ThrowIfNull(resource);
            ArgumentNullException.ThrowIfNull(stream);


            if (!stream.CanWrite)
            {
                throw new ArgumentException(
                    "ストリームへの書き込みができません。",
                    nameof(stream));
            }
            this.xmlDataWriter = new XmlDataWriter();
            base.Serialize(resource);
            stream.Position = 0;
            this.xmlDataWriter.Document.Save(stream);
        }
    }

}
