using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using static Rydia.Engine.Serialization.XmlResourceSerializer<Rydia.Engine.Resource>;

namespace Rydia.Engine.Serialization.Tests
{

    public class XmlSerializerTests
    {

        private XmlDataWriter writer;
        private XmlDataReader reader;

        private string _TestDirectory;
        private string _TestFilePath;

        private enum TestEnum
        {
            None,
            First,
            Second
        }

        [SetUp]
        public void SetUp()
        {
            this.writer = new XmlDataWriter();

            this._TestFilePath = Path.Combine("AudioData.xml");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this._TestDirectory))
            {
                Directory.Delete(this._TestDirectory, true);
            }
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

        [Test(Description = "プロパティの値が正しく復元されること、および複数のリソースから同一のリソースインスタンスが参照されることを検証します。")]
        public void XmlSerializerTest()
        {
            var obj2 = new TestObject2
            {
                Health = 150,
                Level = 15,
                Status = new short[] { 6, 7, 8, 9, 10 }
            };
            var obj1 = new TestObject1
            {
                Health = 100,
                Level = 10,
                Status = new int[] { 1, 2, 3, 4, 5 },
                Obj = obj2
            };

            var obj3 = new TestObject1
            {
                Health = 200,
                Level = 20,
                Status = new int[] { 11, 12, 13, 14, 15 },
                Obj = obj2
            };

            obj1.Path = "TestObject2.xml";
            obj2.Path = "TestObject1.xml";
            obj3.Path = "TestObject3.xml";

            var serializer1 = new XmlResourceSerializer<TestObject1>();
            var serializer2 = new XmlResourceSerializer<TestObject2>();
            
            serializer1.Serialize(obj1, obj1.Path);
            serializer2.Serialize(obj2, obj2.Path);
            serializer1.Serialize(obj3, obj3.Path);

            var deserialized1 = serializer1.Deserialize(obj1.Path);
            var deserialized3 = serializer1.Deserialize(obj3.Path);

            Assert.That(deserialized1, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(deserialized1.Health, Is.EqualTo(obj1.Health));
                Assert.That(deserialized1.Level, Is.EqualTo(obj1.Level));
                Assert.That(deserialized1.Status, Is.EqualTo(obj1.Status));

                Assert.That(deserialized1.Obj.Res.Health, Is.EqualTo(obj1.Obj.Res.Health));
                Assert.That(deserialized1.Obj.Res.Level, Is.EqualTo(obj1.Obj.Res.Level));
                Assert.That(deserialized1.Obj.Res.Status, Is.EqualTo(obj1.Obj.Res.Status));

                Assert.That(deserialized3.Health, Is.EqualTo(obj3.Health));
                Assert.That(deserialized3.Level, Is.EqualTo(obj3.Level));
                Assert.That(deserialized3.Status, Is.EqualTo(obj3.Status));

                Assert.That(deserialized3.Obj.Res.Health, Is.EqualTo(obj3.Obj.Res.Health));
                Assert.That(deserialized3.Obj.Res.Level, Is.EqualTo(obj3.Obj.Res.Level));
                Assert.That(deserialized3.Obj.Res.Status, Is.EqualTo(obj3.Obj.Res.Status));

                // deserialized3.Obj.Res と deserialized1.Obj.Res が同一インスタンスかどうか
                Assert.That(deserialized3.Obj.Res, Is.SameAs(deserialized1.Obj.Res));
            });
        }

        [Test(Description = "AudioDataをXMLにシリアライズできることを確認します。")]
        public void AudioData_SerializeTest()
        {
            this._TestFilePath = "AudioData.xml";
            // Arrange
            var audioData = new AudioData
            {
                Channels = 2,
                SampleRate = 44100,
                PCMData = new short[]
                {
                0, 1000, -1000, 2000,
                -2000, 3000, -3000, 4000
                }
            };

            var serializer = new XmlResourceSerializer<AudioData>();

            // Act
            serializer.Serialize(audioData, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Does.Contain("Channels"));
            Assert.That(xml, Does.Contain("SampleRate"));
            Assert.That(xml, Does.Contain("PCMData"));
        }

        [Test(Description = "XMLからAudioDataをデシリアライズできることを確認します。")]
        public void AudioData_DeserializeTest()
        {
            this._TestFilePath = "AudioData.xml";
            // Arrange
            var source = new AudioData
            {
                Channels = 2,
                SampleRate = 44100,
                PCMData = new short[]
                {
                0, 1000, -1000, 2000,
                -2000, 3000, -3000, 4000
                }
            };

            var serializer = new XmlResourceSerializer<AudioData>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            AudioData result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Channels, Is.EqualTo(source.Channels));
            Assert.That(result.SampleRate, Is.EqualTo(source.SampleRate));

            Assert.That(result.PCMData, Is.Not.Null);
            Assert.That(result.PCMData, Is.EqualTo(source.PCMData));
        }

        [Test(Description = "PCMDataが空の場合でもシリアライズ・デシリアライズできることを確認します。")]
        public void SerializeDeserializeEmptyPCMDataTest()
        {
            this._TestFilePath = "AudioData.xml";
            // Arrange
            var source = new AudioData
            {
                Channels = 1,
                SampleRate = 22050,
                PCMData = Array.Empty<short>()
            };

            var serializer = new XmlResourceSerializer<AudioData>();

            // Act
            serializer.Serialize(source, this._TestFilePath);
            AudioData result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Channels, Is.EqualTo(1));
            Assert.That(result.SampleRate, Is.EqualTo(22050));
            Assert.That(result.PCMData, Is.Not.Null);
            Assert.That(result.PCMData, Is.Empty);
        }

        [Test(Description = "PixelDataをXMLにシリアライズできることを確認します。")]
        public void PixelData_SerializeTest()
        {
            this._TestFilePath = "PixelData.xml";
            // Arrange
            PixelData pixelData = CreatePixelData();

            var serializer = new XmlResourceSerializer<PixelData>();

            // Act
            serializer.Serialize(pixelData, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Does.Contain("Width"));
            Assert.That(xml, Does.Contain("Height"));
            Assert.That(xml, Does.Contain("Data"));
        }

        [Test(Description = "PixelDataをXMLからデシリアライズできることを確認します。")]
        public void PixelData_DeserializeTest()
        {
            this._TestFilePath = "PixelData.xml";
            // Arrange
            PixelData source = CreatePixelData();

            var serializer = new XmlResourceSerializer<PixelData>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            PixelData result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Width, Is.EqualTo(source.Width));
            Assert.That(result.Height, Is.EqualTo(source.Height));
            Assert.That(result.Data, Is.EqualTo(source.Data));
        }

        /// <summary>
        /// テスト用のPixelDataを生成します。
        /// </summary>
        /// <returns>テスト用のPixelData。</returns>
        private static PixelData CreatePixelData()
        {
            return new PixelData
            {
                Width = 2,
                Height = 2,
                Data =
                [
                    255, 0, 0, 255,
            0, 255, 0, 255,
            0, 0, 255, 255,
            255, 255, 255, 255
                ]
            };
        }

        [Test(Description = "VertexShaderをXMLにシリアライズできることを確認します。")]
        public void VertexShader_SerializeTest()
        {
            this._TestFilePath = "VertexShader.xml";
            // Arrange
            VertexShader vertexShader = CreateVertexShader();

            var serializer = new XmlResourceSerializer<VertexShader>();

            // Act
            serializer.Serialize(vertexShader, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Does.Contain("Source"));
            Assert.That(xml, Does.Contain("attribute"));
            Assert.That(xml, Does.Contain("gl_Position"));
        }

        [Test(Description = "VertexShaderをXMLからデシリアライズできることを確認します。")]
        public void VertexShader_DeserializeTest()
        {
            this._TestFilePath = "VertexShader.xml";
            // Arrange
            VertexShader source = CreateVertexShader();

            var serializer = new XmlResourceSerializer<VertexShader>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            VertexShader result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Source, Is.EqualTo(source.Source));
        }

        /// <summary>
        /// テスト用のVertexShaderを生成します。
        /// </summary>
        /// <returns>テスト用のVertexShader。</returns>
        private static VertexShader CreateVertexShader()
        {
            return new VertexShader
            {
                Source = """
            attribute vec3 aPosition;

            void main()
            {
                gl_Position = vec4(aPosition, 1.0);
            }
            """
            };
        }

        [Test(Description = "FragmentShaderをXMLにシリアライズできることを確認します。")]
        public void FragmentShader_SerializeTest()
        {
            this._TestFilePath = "FragmentShader.xml";
            // Arrange
            FragmentShader fragmentShader = CreateFragmentShader();

            var serializer = new XmlResourceSerializer<FragmentShader>();

            // Act
            serializer.Serialize(fragmentShader, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Does.Contain("Source"));
            Assert.That(xml, Does.Contain("precision"));
            Assert.That(xml, Does.Contain("gl_FragColor"));
        }

        [Test(Description = "FragmentShaderをXMLからデシリアライズできることを確認します。")]
        public void FragmentShader_DeserializeTest()
        {
            this._TestFilePath = "FragmentShader.xml";
            // Arrange
            FragmentShader source = CreateFragmentShader();

            var serializer = new XmlResourceSerializer<FragmentShader>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            FragmentShader result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Source, Is.EqualTo(source.Source));
        }

        /// <summary>
        /// テスト用のFragmentShaderを生成します。
        /// </summary>
        /// <returns>テスト用のFragmentShader。</returns>
        private static FragmentShader CreateFragmentShader()
        {
            return new FragmentShader
            {
                Source = """
            precision mediump float;

            void main()
            {
                gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
            }
            """
            };
        }

        [Test(Description = "PixmapをXMLにシリアライズできることを確認します。")]
        public void Pixmap_SerializeTest()
        {
            this._TestFilePath = "Pixmap.xml";

            // Arrange
            Pixmap pixmap = CreatePixmap();

            var serializer = new XmlResourceSerializer<Pixmap>();

            // Act
            serializer.Serialize(pixmap, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);

            // PixelDataの情報が保存されていること
            Assert.That(xml, Does.Contain("PixelDataCount"));
            Assert.That(xml, Does.Contain("PixelData_0_Width"));
            Assert.That(xml, Does.Contain("PixelData_0_Height"));
            Assert.That(xml, Does.Contain("PixelData_0_Data"));

            // MainLayerや通常のPixelDataプロパティが保存されていないこと
            Assert.That(xml, Does.Not.Contain("MainLayer"));
            Assert.That(xml, Does.Not.Contain("Name=\"PixelData\""));
        }

        [Test(Description = "PixmapをXMLからデシリアライズできることを確認します。")]
        public void Pixmap_DeserializeTest()
        {

            this._TestFilePath = "Pixmap.xml";
            // Arrange
            Pixmap source = CreatePixmap();

            var serializer = new XmlResourceSerializer<Pixmap>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            Pixmap result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PixelData, Is.Not.Null);
            Assert.That(result.PixelData.Count, Is.EqualTo(source.PixelData.Count));

            for (int i = 0; i < source.PixelData.Count; i++)
            {
                PixelData expected = source.PixelData[i];
                PixelData actual = result.PixelData[i];

                Assert.That(actual.Width, Is.EqualTo(expected.Width));
                Assert.That(actual.Height, Is.EqualTo(expected.Height));
                Assert.That(actual.Data, Is.EqualTo(expected.Data));
            }

            // MainLayerが先頭のPixelDataを参照していること
            Assert.That(result.MainLayer, Is.SameAs(result.PixelData[0]));
        }

        /// <summary>
        /// テスト用のPixmapを生成します。
        /// </summary>
        /// <returns>テスト用のPixmap。</returns>
        private static Pixmap CreatePixmap()
        {
            return new Pixmap
            {
                PixelData =
                [
                    new PixelData
                    {
                        Width = 2,
                        Height = 2,
                        Data =
                        [
                            255, 0, 0, 255,
                            0, 255, 0, 255,
                            0, 0, 255, 255,
                            255, 255, 255, 255
                        ]
                    },
                    new PixelData
                    {
                        Width = 1,
                        Height = 1,
                        Data =
                        [
                            128, 128, 128, 255
                        ]
                    }
                ]
            };
        }

    }
}
