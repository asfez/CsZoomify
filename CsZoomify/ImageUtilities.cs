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

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        internal static void Crop(Image originalimg, Rectangle rectangle, Size newSize, string filename)
        {

            using (var thumbnail = new Bitmap(Convert.ToInt32(newSize.Width), Convert.ToInt32(newSize.Height)))
            {
                var objGraphics = Graphics.FromImage(thumbnail);
                objGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                objGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                objGraphics.CompositingQuality = CompositingQuality.HighQuality;



                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

                // Create an Encoder object based on the GUID 
                // for the Quality parameter category.
                Encoder myEncoder = Encoder.Quality;
                Encoder myEncoder2 = Encoder.Compression;

                // Create an EncoderParameters object. 
                // An EncoderParameters object has an array of EncoderParameter 
                // objects. In this case, there is only one 
                // EncoderParameter object in the array.
                var myEncoderParameters = new EncoderParameters(1);

                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                
                myEncoderParameters.Param[0] = myEncoderParameter;
               



                objGraphics.DrawImage(originalimg, new Rectangle(0, 0, newSize.Width, newSize.Height), rectangle, GraphicsUnit.Pixel);
                thumbnail.Save(filename, jgpEncoder, myEncoderParameters);
            }

        }
    }
}