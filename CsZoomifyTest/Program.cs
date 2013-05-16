using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CsZoomify;

namespace CsZoomifyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            if (args.Length < 2)
            {
                Console.WriteLine("usage : cszoomify.exe [imagefilame] [targetpath]");
                return;
            }

            var file = args[0];
            var target = args[1];

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
            Console.WriteLine("Done ! Press enter to close.");
            Console.ReadLine();
        }
    }
}
