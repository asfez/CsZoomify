#region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CsZoomify;

#endregion

namespace CsZoomifyTest
{
    public class ZoomifyImage : IDisposable
    {

        /// <summary>
        /// All the tiles definition. The tiles definition
        /// are created in the constructor, but the physical images are
        /// created after the call of the function Zoomify.
        /// </summary>
        public IEnumerable<Tile> Tiles { get; private set; }

        internal DirectoryInfo Directory { get; set; }
        
        internal Image Image { get; set; }

        /// <summary>
        /// The size of the original image
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        ///  The size of the tile, by default to 256
        /// </summary>
        public int TileSize { get; set; }

        /// <summary>
        /// The number of zoom levels calculated
        /// </summary>
        public int ZoomLevels { get; private set; }
        
        /// <summary>
        /// Allow to set a coefficient to save the tile bigger than the tilesize.
        /// It is useful when the viewer uses a fractional zoom to get a better quality
        /// when the tile are zoomed.
        /// </summary>
        public Double[] SizeCoeffByZoomLevel { get; private set; }

        public ZoomifyImage(string filename, int tileSize = 256)
        {
            if(!File.Exists(filename))
                throw new FileNotFoundException("The image does't exist",filename);
            TileSize = tileSize;
            Tiles = new List<Tile>();
            Image = Image.FromFile(filename);
            Size = Image.Size;

            var maxsize = Math.Max(Size.Width, Size.Height);
            var zoom = 1;
            var tmp = maxsize;
            while (tmp > TileSize)
            {
                tmp = tmp/2;
                zoom++;
            }
            ZoomLevels = zoom;
            SizeCoeffByZoomLevel = new double[ZoomLevels];
            for (int i = 0; i < SizeCoeffByZoomLevel.Length; i++)
            {
                SizeCoeffByZoomLevel[i] = 1;
            }
            CreateTileDefinitions();
        }


        /// <summary>
        /// Zoomifies the image in the specified target directory.
        /// </summary>
        /// <param name="targetDirectory">The target directory.</param>
        public void Zoomify(string targetDirectory)
        {
            Directory = new DirectoryInfo(targetDirectory);
            if(!Directory.Exists) Directory.Create();
            foreach (var tile in Tiles)
            {
                tile.Write();
            }

            WriteXmlProperties();
        }

        private void WriteXmlProperties()
        {
            File.WriteAllText(Directory.FullName + "\\ImageProperties.xml",
                              String.Format(
                                  "<IMAGE_PROPERTIES WIDTH=\"{0}\" HEIGHT=\"{1}\" NUMTILES=\"{2}\" NUMIMAGES=\"1\" VERSION=\"1.8\" TILESIZE=\"{3}\" />",
                                  Size.Width, Size.Height, Tiles.Count(), TileSize));
        }

        /// <summary>
        /// Zoomifies all the images in a directorythe directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="filepattern">The filepattern.</param>
        /// <param name="targetDirectory">The target directory.</param>
        /// <param name="configurator">The configurator action for custom ZoomImage config (SizeCoeff...).</param>
        /// <returns>A list of exceptions</returns>
        public static List<Exception> ZoomifyDirectory(string directory, string filepattern, string targetDirectory, Action<ZoomifyImage> configurator = null)
        {
            var dinfo = new DirectoryInfo(directory);
            if(!dinfo.Exists)
                throw new DirectoryNotFoundException(String.Format("The directory {0} doesn't exist",directory));
            var files = dinfo.GetFiles(filepattern);
            var res = new List<Exception>();
            foreach (var fileInfo in files)
            {
                try
                {
                    var img = new ZoomifyImage(fileInfo.FullName);
                    if (configurator != null)
                        configurator(img);
                    img.Zoomify(targetDirectory + "\\" + Path.GetFileNameWithoutExtension(fileInfo.FullName));
                }
                catch (Exception exception)
                {
                    res.Add(exception);
                }
            }
            return res;
        }

        private void CreateTileDefinitions()
        {
            for (int i = 0; i < ZoomLevels; i++)
            {
                var tilesize = TileSize *  Convert.ToInt32(Math.Pow(2, i));
                var cols = Convert.ToInt32(Math.Floor((Decimal)Size.Width / tilesize)) + (Size.Width % tilesize > 0 ? 1 : 0);
                var rows = Convert.ToInt32(Math.Floor((Decimal)Size.Height / tilesize)) + (Size.Height % tilesize > 0 ? 1 : 0);
                var tilecount = 0;
                for (int x = 0; x < cols; x++)
                {
                    for (int y = 0; y < rows; y++)
                    {

                        var w = Math.Min(tilesize + Size.Width - (x + 1) * tilesize, tilesize);
                        var h = Math.Min(tilesize + Size.Height - (y + 1) * tilesize, tilesize);
                        tilecount++;

                        var tile = new Tile(this)
                            {
                                OriginalRectangle = new Rectangle(x*tilesize, y*tilesize, w, h),
                                ZoomLevel = ZoomLevels - i - 1,
                                X = x,
                                Y = y,
                                Size = new Size(Convert.ToInt32(TileSize *  (Convert.ToDouble(w)/tilesize)),  Convert.ToInt32(TileSize *  (Convert.ToDouble(h)/tilesize))),
                                Group = Convert.ToInt32( Math.Floor((Decimal)tilecount / 256))
                            };
                        ((List<Tile>)Tiles).Add(tile);
                    }
                }
            }
        }

        public void Dispose()
        {
            if(Image != null) Image.Dispose();
        }
    }
}