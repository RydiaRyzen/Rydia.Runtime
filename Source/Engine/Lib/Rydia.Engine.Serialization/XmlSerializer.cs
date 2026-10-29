using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// XML形式でオブジェクトをシリアライズおよび
    /// デシリアライズするクラスのベースクラスです。
    /// </summary>
    public class XmlSerializer : SerializerBase
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

                if (value is null)
                {
                    element.SetAttribute("Type", "null");
                    element.InnerText = string.Empty;
                }
                else if (value is bool boolean)
                {
                    element.SetAttribute("Type", value.GetType().AssemblyQualifiedName);
                    element.InnerText = XmlConvert.ToString(boolean);
                }
                else if (value is Type typeValue)
                {
                    element.SetAttribute(
                        "Type",
                        typeof(Type).AssemblyQualifiedName);

                    element.InnerText =
                        typeValue.AssemblyQualifiedName
                        ?? typeValue.FullName
                        ?? typeValue.Name;
                }
                else
                {
                    element.SetAttribute("Type", value.GetType().AssemblyQualifiedName);

                    if (value is Type type)
                    {
                        element.InnerText = type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
                    }
                    else if (value is IFormattable formattable)
                    {
                        element.InnerText = formattable.ToString(null, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        element.InnerText = value.ToString() ?? string.Empty;
                    }
                }

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
                XmlElement element = FindElement(name);

                string typeName = element.GetAttribute("Type");
                string text = element.InnerText;

                if (typeName == "null")
                    return null;

                Type type = Type.GetType(typeName)
                    ?? throw new InvalidOperationException(
                        $"型 '{typeName}' を解決できません。");

                if (type == typeof(string))
                    return text;

                if (type == typeof(Type))
                {
                    return Type.GetType(text)
                        ?? throw new InvalidOperationException(
                            $"型 '{text}' を解決できません。");
                }

                if (type.IsEnum)
                    return Enum.Parse(type, text);

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
                if (type == typeof(Type))
                {
                    return Type.GetType(text)
                        ?? throw new InvalidOperationException(
                            $"型 '{text}' を解決できませんでした。");
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

        private XmlDataReader dataReader;
        private XmlDataWriter dataWriter = new XmlDataWriter();

        private string _Path;

        protected override IDataWriter Writer
        {
            get
            {
                return this.dataWriter;
            }
        }

        protected override IDataReader Reader
        {
            get
            {
                return this.dataReader;
            }
        }

        public XmlSerializer(string path)
        {
            this._Path = path;
            var doc = new XmlDocument();
            doc.Load(File.OpenRead(path));
            this.dataReader = new XmlDataReader(doc);
        }

        public void Serialize(string path, object obj)
        {
            this.dataWriter = new XmlDataWriter();
            base.Serialize(obj);
            this.dataWriter.Document.Save(path);
        }

        public object Deserialize(string path)
        {
            var document = new XmlDocument();
            document.Load(path);
            return base.Deserialize();
        }

    }

}
