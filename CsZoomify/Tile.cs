using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using CsZoomifyTest;

namespace CsZoomify
{

    public class Tile
    {
        public ZoomifyImage ZoomifyImage { get; private set; }
        public Rectangle OriginalRectangle { get; set; }
        public Size Size { get; set; }

        public int ZoomLevel { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Group { get; set; }
        
        public Tile(ZoomifyImage image)
        {
            ZoomifyImage = image;
            Size = new Size(image.TileSize,image.TileSize);
        }
        
        internal void Write()
        {
            var target = new DirectoryInfo(ZoomifyImage.Directory.FullName + "\\TileGroup" + Group);
            if(!target.Exists) target.Create();
            ImageUtilities.Crop(ZoomifyImage.Image, OriginalRectangle,
                                new Size(
                                    Math.Min(Convert.ToInt32(Size.Width*ZoomifyImage.SizeCoeffByZoomLevel[ZoomLevel]),
                                             ZoomifyImage.Size.Width),
                                    Math.Min(Convert.ToInt32(Size.Height*ZoomifyImage.SizeCoeffByZoomLevel[ZoomLevel]),
                                             ZoomifyImage.Size.Height)),
                                String.Format(@"{0}\{1}-{2}-{3}.jpg", target.FullName.ToLower(), ZoomLevel, X, Y));


        }


    }
}
