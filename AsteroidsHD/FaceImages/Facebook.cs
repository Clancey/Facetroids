using System;
using MonoTouch.UIKit;
using System.IO;
using System.Linq;
namespace AsteroidsHD
{
	public static class Facebook
	{
		public static string[] GetImages()
		{
			string[] images = Directory.GetFiles(ImageStore.RoundedPicDir).Where(x=> x.ToLower().EndsWith(".png")).ToArray();
			if(images.Count() == 0)
				images = new string[] {"Content/asteroid-front.pdf"};
			
			Console.WriteLine(images[0]);
			return images;
		}
	}
}

