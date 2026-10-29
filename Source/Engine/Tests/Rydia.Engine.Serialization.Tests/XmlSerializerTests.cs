using System.Xml;
using static Rydia.Engine.Serialization.XmlSerializer;
using System.Collections.Generic;
using System.Linq;
namespace Rydia.Engine.Serialization.Tests
{

    public class XmlSerializerTests
    {

        private XmlDataWriter writer;
        private XmlDataReader reader;

        private enum TestEnum
        {
            None,
            First,
            Second
        }

        [SetUp]
        public void Setup()
        {
            this.writer = new XmlDataWriter();
        }

        /// <summary>
        /// XML ドキュメントを保存して読み込み直し、
        /// 読み取りテスト用の XmlDataReader を生成します。
        /// </summary>
        private void CreateReader()
        {
            using var stream = new MemoryStream();

            this.writer.Document.Save(stream);
            stream.Position = 0;

            var document = new XmlDocument();
            document.Load(stream);

            this.reader = new XmlDataReader(document);
        }

        [Test(Description = "文字列を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_String()
        {
            this.writer.WriteValue("Name", "Player1");
            CreateReader();

            Assert.That(this.reader.ReadValue<string>("Name"), Is.EqualTo("Player1"));
        }

        [Test(Description = "真偽値を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_Boolean()
        {
            this.writer.WriteValue("Enabled", true);

            TestContext.WriteLine(this.writer.Document.OuterXml);

            CreateReader();

            bool actual = this.reader.ReadValue<bool>("Enabled");

            TestContext.WriteLine($"Actual: {actual}");

            Assert.That(actual, Is.True);
        }

        [Test(Description = "各整数型の値を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_IntegerTypes()
        {
            this.writer.WriteValue("Byte", (byte)255);
            this.writer.WriteValue("SByte", (sbyte)-100);
            this.writer.WriteValue("Short", (short)-1234);
            this.writer.WriteValue("UShort", (ushort)1234);
            this.writer.WriteValue("Int", -123456);
            this.writer.WriteValue("UInt", 123456u);
            this.writer.WriteValue("Long", -123456789L);
            this.writer.WriteValue("ULong", 123456789UL);

            CreateReader();

            Assert.Multiple(() =>
            {
                Assert.That(this.reader.ReadValue<byte>("Byte"), Is.EqualTo((byte)255));
                Assert.That(this.reader.ReadValue<sbyte>("SByte"), Is.EqualTo((sbyte)-100));
                Assert.That(this.reader.ReadValue<short>("Short"), Is.EqualTo((short)-1234));
                Assert.That(this.reader.ReadValue<ushort>("UShort"), Is.EqualTo((ushort)1234));
                Assert.That(this.reader.ReadValue<int>("Int"), Is.EqualTo(-123456));
                Assert.That(this.reader.ReadValue<uint>("UInt"), Is.EqualTo(123456u));
                Assert.That(this.reader.ReadValue<long>("Long"), Is.EqualTo(-123456789L));
                Assert.That(this.reader.ReadValue<ulong>("ULong"), Is.EqualTo(123456789UL));
            });
        }

        [Test(Description = "浮動小数点型および decimal 型の値を正しく読み取れることを確認します。")]
        public void WriteAndRead_FloatingPointTypes()
        {
            this.writer.WriteValue("Float", 12.5f);
            this.writer.WriteValue("Double", 123.456d);
            this.writer.WriteValue("Decimal", 987.654m);

            CreateReader();

            Assert.Multiple(() =>
            {
                Assert.That(this.reader.ReadValue<float>("Float"), Is.EqualTo(12.5f));
                Assert.That(this.reader.ReadValue<double>("Double"), Is.EqualTo(123.456d));
                Assert.That(this.reader.ReadValue<decimal>("Decimal"), Is.EqualTo(987.654m));
            });
        }

        [Test(Description = "文字を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_Char()
        {
            this.writer.WriteValue("Character", 'A');
            CreateReader();

            Assert.That(this.reader.ReadValue<char>("Character"), Is.EqualTo('A'));
        }

        [Test(Description = "列挙型の値を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_Enum()
        {
            this.writer.WriteValue("State", TestEnum.Second);
            CreateReader();

            Assert.That(
                this.reader.ReadValue<TestEnum>("State"),
                Is.EqualTo(TestEnum.Second));
        }

        [Test(Description = "Type オブジェクトを書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_Type()
        {
            this.writer.WriteValue("Type", typeof(string));
            CreateReader();

            Assert.That(
                this.reader.ReadValue<Type>("Type"),
                Is.EqualTo(typeof(string)));
        }

        [Test(Description = "null 値を書き込み、正しく読み取れることを確認します。")]
        public void WriteAndRead_Null()
        {
            this.writer.WriteValue("Value", null);
            CreateReader();

            Assert.That(this.reader.ReadValue("Value"), Is.Null);
        }

        [Test(Description = "out 引数を使用して値を読み取れることを確認します。")]
        public void WriteAndRead_OutParameter()
        {
            this.writer.WriteValue("Level", 10);
            CreateReader();

            this.reader.ReadValue<int>("Level", out int level);

            Assert.That(level, Is.EqualTo(10));
        }

        [Test(Description = "書き込んだキーの一覧を正しく取得できることを確認します。")]
        public void Keys_ReturnsWrittenKeys()
        {
            this.writer.WriteValue("Name", "Player1");
            this.writer.WriteValue("Level", 10);
            CreateReader();

            Assert.That(
                this.reader.Keys,
                Is.EquivalentTo(new[] { "Name", "Level" }));
        }

        [Test(Description = "同じ名前の値を再度書き込むと、既存の値が置き換えられることを確認します。")]
        public void WriteValue_SameName_ReplacesExistingValue()
        {
            this.writer.WriteValue("Level", 10);
            this.writer.WriteValue("Level", 20);
            CreateReader();

            Assert.That(this.reader.ReadValue<int>("Level"), Is.EqualTo(20));
            Assert.That(this.reader.Keys.Count(), Is.EqualTo(1));
        }

        [Test(Description = "存在しないキーを読み取ると、KeyNotFoundException が発生することを確認します。")]
        public void ReadValue_MissingKey_ThrowsKeyNotFoundException()
        {
            CreateReader();

            Assert.Throws<KeyNotFoundException>(
                () => this.reader.ReadValue("Missing"));
        }

        [Test(Description = "null の XmlDocument を渡すと、ArgumentNullException が発生することを確認します。")]
        public void XmlDataReader_NullDocument_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new XmlDataReader(null));
        }

    }
}
