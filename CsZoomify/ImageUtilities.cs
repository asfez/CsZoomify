#region
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
#endregion

namespace CsZoomify
{
    internal static class ImageUtilities
    {
        internal static void Crop(Image originalimg, Rectangle rectangle, Size newSize, string filename)
        {
            using (var thumbnail = new Bitmap(Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height)))
            {
                var objGraphics = Graphics.FromImage(thumbnail);
                objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                objGraphics.SmoothingMode = SmoothingMode.HighQuality;
                objGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                objGraphics.CompositingQuality = CompositingQuality.HighQuality;
                objGraphics.DrawImage(originalimg, new Rectangle(0, 0, newSize.Width, newSize.Height), rectangle, GraphicsUnit.Pixel);
                thumbnail.Save(filename, ImageFormat.Jpeg);
            }

        }
    }
}