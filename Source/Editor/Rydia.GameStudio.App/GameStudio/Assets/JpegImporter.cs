using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Engine;

namespace Rydia.GameStudio.Assets
{

    public class JpegImporter : IResourceImporter<PixelData>
    {

        public bool TryImport(byte[] data, out PixelData resource)
        {
            try
            {
                using (var ms = new MemoryStream(data))
                using (var image = SixLabors.ImageSharp.Image.Load(ms))
                using (var rgba = image.CloneAs<SixLabors.ImageSharp.PixelFormats.Rgba32>())
                {
                    int width = image.Width;
                    int height = image.Height;
                    byte[] pixelData = new byte[width * height * 4]; // RGBA の 4 バイト

                    // ピクセルデータを取得
                    rgba.CopyPixelDataTo(pixelData);

                    resource = new PixelData
                    {
                        Width = width,
                        Height = height,
                        Data = pixelData
                    };
                    return true;
                }
            }
            catch (Exception e)
            {
                resource = null;
                return false;
            }

        }

    }

}
