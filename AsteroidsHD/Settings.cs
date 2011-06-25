using System;
using MonoTouch.Foundation;
namespace AsteroidsHD
{
	public static class Settings
	{
		private static NSUserDefaults prefs =  NSUserDefaults.StandardUserDefaults ;

		public static string FbId
		{
			get {return prefs.StringForKey("FbId"); }
			set { prefs.SetString(value,"FbId");}
		}
		
		public static string FbAuth
		{
			get {return prefs.StringForKey("fbauth"); }
			set { prefs.SetString(value,"fbauth");}
		}
		
		public static DateTime FbAuthExpire
		{
			get {return DateTime.Parse(prefs.StringForKey("fbauthexp")); }
			set { prefs.SetString(value.ToString(),"fbauthexp");}
		}
		
		
	}
}

