using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Engine.Serialization;

namespace Rydia.Engine
{


    public class Pixmap : Resource, ICustomSerializable
    {

        private List<PixelData> _PixelData = new();

        [DontSerialize]
        public PixelData MainLayer
        {
            get
            {
                return PixelData.Count > 0 ? PixelData[0] : null;
            }
            set
            {
                if (value == null)
                    value = new PixelData();
                if (PixelData.Count > 0)
                    PixelData[0] = value;
                else
                    PixelData.Add(value);
            }
        }

        [DontSerialize]
        public List<PixelData> PixelData
        {
            get
            {
                return this._PixelData;
            }
            set
            {
                if (value == null)
                    this._PixelData.Clear();
                else
                    this._PixelData = value;
            }
        }

        /// <summary>
        /// Pixmapのデータを読み込みます。
        /// </summary>
        /// <param name="reader">データを読み込むリーダー。</param>
        public void ReadData(IDataReader reader)
        {
            this._PixelData.Clear();

            int count = reader.ReadValue<int>("PixelDataCount");

            for (int i = 0; i < count; i++)
            {
                string prefix = $"PixelData_{i}";

                PixelData pixelData = new PixelData
                {
                    Width = reader.ReadValue<int>($"{prefix}_Width"),
                    Height = reader.ReadValue<int>($"{prefix}_Height"),
                    Data = reader.ReadValue<byte[]>($"{prefix}_Data")
                };

                this._PixelData.Add(pixelData);
            }
        }

        /// <summary>
        /// Pixmapのデータを書き込みます。
        /// </summary>
        /// <param name="writer">データを書き込むライター。</param>
        public void WriteData(IDataWriter writer)
        {
            writer.WriteValue("PixelDataCount", this._PixelData.Count);

            for (int i = 0; i < this._PixelData.Count; i++)
            {
                PixelData pixelData = this._PixelData[i];

                string prefix = $"PixelData_{i}";

                writer.WriteValue($"{prefix}_Width", pixelData.Width);
                writer.WriteValue($"{prefix}_Height", pixelData.Height);
                writer.WriteValue($"{prefix}_Data", pixelData.Data);
            }
        }
    }

}
