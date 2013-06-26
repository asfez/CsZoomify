using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsZoomify;

namespace CsZoomify
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length <3)
            {
                Console.WriteLine("usage : cszoomify.exe [dir|file] [imagefilame] [targetpath]");
                Console.ReadLine();
                return;
            }

            var action = args[0];
            var file = args[1];
            var target = args[2];

            switch (action.ToLower())
            {
                case "file" :
                    {
                        if (!File.Exists(file))
                        {
                            Console.WriteLine(String.Format("The file {0} doesn't exist !", 0));
                            return;
                        }

                        var img = new ZoomifyImage(file);
                        for (int i = 0; i < img.SizeCoeffByZoomLevel.Length; i++)
                        {
                            img.SizeCoeffByZoomLevel[i] = 1.5;
                        }
                        img.SizeCoeffByZoomLevel[0] = 3;
                        img.SizeCoeffByZoomLevel[img.SizeCoeffByZoomLevel.Length - 1] = 1;
                        img.Zoomify(target);
                        break;
                    }
                case "dir" :
                    {
                        if (!Directory.Exists(file))
                        {
                            Console.WriteLine(String.Format("The directory {0} doesn't exist !", 0));
                            return;
                        }


                        Action<ZoomifyImage>  config = img =>
                            {
                                for (int i = 0; i < img.SizeCoeffByZoomLevel.Length; i++)
                                {
                                    img.SizeCoeffByZoomLevel[i] = 1.5;
                                }
                                img.SizeCoeffByZoomLevel[0] = 3;
                                img.SizeCoeffByZoomLevel[img.SizeCoeffByZoomLevel.Length - 1] = 1;
                            };

                        ZoomifyImage.ZoomifyDirectory(file, "*.jpg",target, config);
                        break;
                    }
            }

            
            Console.WriteLine("Done ! Press enter to close.");
            Console.ReadLine();
        }
    }
}
