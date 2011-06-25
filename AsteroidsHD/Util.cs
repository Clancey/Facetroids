using System;
using MonoTouch.UIKit;
using System.IO;
namespace AsteroidsHD
{
	public static class Util
	{
		public static bool IsIpad {
			get {return UIScreen.MainScreen.Bounds.Width > 500;	}
		}
		
		public static readonly string BaseDir = Directory.GetParent(Environment.GetFolderPath (Environment.SpecialFolder.Personal)).ToString();
	}
}

