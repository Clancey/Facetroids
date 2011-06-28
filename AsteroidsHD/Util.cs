using System;
using MonoTouch.UIKit;
using System.IO;
using MonoTouch.GameKit;
using Microsoft.Xna.Framework.GamerServices;
namespace AsteroidsHD
{
	public static class Util
	{
		public static bool IsIpad {
			get {return UIScreen.MainScreen.Bounds.Width > 500;	}
		}
		
		public static readonly string BaseDir = Directory.GetParent(Environment.GetFolderPath (Environment.SpecialFolder.Personal)).ToString();
			
		public static bool CanUseGameCenter
		{
			get{
				var iosVersion = double.Parse(UIDevice.CurrentDevice.SystemVersion);
				if(iosVersion < 4.1)
					return false;
				
				return iosVersion > 4.1;
					
			}
		}
	}
}

