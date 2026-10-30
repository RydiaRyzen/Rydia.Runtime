using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization.Tests
{

    [TestFixture]
    public class FontCharSetSerializerTests
    {
        private string _TestDirectory;
        private string _TestFilePath;

        [SetUp]
        public void SetUp()
        {
            this._TestFilePath = Path.Combine("FontCharSet.xml");
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
        /// FontCharSetをXMLにシリアライズできることを確認します。
        /// </summary>
        [Test]
        public void SerializeTest()
        {
            // Arrange
            FontCharSet fontCharSet = CreateFontCharSet();

            var serializer = new XmlResourceSerializer<FontCharSet>();

            // Act
            serializer.Serialize(fontCharSet, this._TestFilePath);

            // Assert
            Assert.That(File.Exists(this._TestFilePath), Is.True);

            string xml = File.ReadAllText(this._TestFilePath);

            Assert.That(xml, Is.Not.Empty);
            Assert.That(xml, Does.Contain("CharCount"));
            Assert.That(xml, Does.Contain("Char_0_Character"));
            Assert.That(xml, Does.Contain("Char_0_Width"));
            Assert.That(xml, Does.Contain("Char_0_Height"));
            Assert.That(xml, Does.Contain("Char_0_Data"));
        }

        /// <summary>
        /// XMLからFontCharSetをデシリアライズできることを確認します。
        /// </summary>
        [Test]
        public void DeserializeTest()
        {
            // Arrange
            FontCharSet source = CreateFontCharSet();

            var serializer = new XmlResourceSerializer<FontCharSet>();

            serializer.Serialize(source, this._TestFilePath);

            // Act
            FontCharSet result = serializer.Deserialize(this._TestFilePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CharCount, Is.EqualTo(3));

            // A
            PixelData pixelA = result.GetPixelData('A');

            Assert.That(pixelA, Is.Not.Null);
            Assert.That(pixelA.Width, Is.EqualTo(2));
            Assert.That(pixelA.Height, Is.EqualTo(2));
            Assert.That(pixelA.Data, Is.EqualTo(new byte[]
            {
            255, 0, 0, 255,
            0, 255, 0, 255,
            0, 0, 255, 255,
            255, 255, 255, 255
            }));

            // B
            PixelData pixelB = result.GetPixelData('B');

            Assert.That(pixelB, Is.Not.Null);
            Assert.That(pixelB.Width, Is.EqualTo(1));
            Assert.That(pixelB.Height, Is.EqualTo(2));
            Assert.That(pixelB.Data, Is.EqualTo(new byte[]
            {
            255, 255, 0, 255,
            255, 0, 255, 255
            }));

            // C
            PixelData pixelC = result.GetPixelData('C');

            Assert.That(pixelC, Is.Not.Null);
            Assert.That(pixelC.Width, Is.EqualTo(1));
            Assert.That(pixelC.Height, Is.EqualTo(1));
            Assert.That(pixelC.Data, Is.EqualTo(new byte[]
            {
            0, 255, 255, 255
            }));
        }

        /// <summary>
        /// TryGetPixelDataで画像データを取得できることを確認します。
        /// </summary>
        [Test]
        public void TryGetPixelDataTest()
        {
            // Arrange
            FontCharSet source = CreateFontCharSet();

            var serializer = new XmlResourceSerializer<FontCharSet>();

            serializer.Serialize(source, this._TestFilePath);

            FontCharSet result = serializer.Deserialize(this._TestFilePath);

            // Act
            bool found = result.TryGetPixelData('A', out PixelData pixelData);
            bool notFound = result.TryGetPixelData('Z', out PixelData missingData);

            // Assert
            Assert.That(found, Is.True);
            Assert.That(pixelData, Is.Not.Null);

            Assert.That(notFound, Is.False);
            Assert.That(missingData, Is.Null);
        }

        /// <summary>
        /// 登録済みの文字を上書きできることを確認します。
        /// </summary>
        [Test]
        public void AddOverwriteTest()
        {
            // Arrange
            var fontCharSet = new FontCharSet();

            var first = new PixelData
            {
                Width = 1,
                Height = 1,
                Data = new byte[] { 255, 0, 0, 255 }
            };

            var second = new PixelData
            {
                Width = 1,
                Height = 1,
                Data = new byte[] { 0, 255, 0, 255 }
            };

            // Act
            fontCharSet.Add('A', first);
            fontCharSet.Add('A', second);

            // Assert
            Assert.That(fontCharSet.CharCount, Is.EqualTo(1));
            Assert.That(fontCharSet.GetPixelData('A'), Is.SameAs(second));
        }

        /// <summary>
        /// テスト用のFontCharSetを生成します。
        /// </summary>
        private static FontCharSet CreateFontCharSet()
        {
            var fontCharSet = new FontCharSet();

            fontCharSet.Add('A', new PixelData
            {
                Width = 2,
                Height = 2,
                Data = new byte[]
                {
                255, 0, 0, 255,
                0, 255, 0, 255,
                0, 0, 255, 255,
                255, 255, 255, 255
                }
            });

            fontCharSet.Add('B', new PixelData
            {
                Width = 1,
                Height = 2,
                Data = new byte[]
                {
                255, 255, 0, 255,
                255, 0, 255, 255
                }
            });

            fontCharSet.Add('C', new PixelData
            {
                Width = 1,
                Height = 1,
                Data = new byte[]
                {
                0, 255, 255, 255
                }
            });

            return fontCharSet;
        }
    }

}
