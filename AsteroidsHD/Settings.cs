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
		
		public static GameType GameType {
			get{ return (GameType)prefs.IntForKey("gameType");}
			set{prefs.SetInt((int)value,"gameType");}
		}
		
		public static int HighScore
		{
			get{return prefs.IntForKey("highScore");}
			set{prefs.SetInt(value,"highScore");}
		}
		
		public static bool UseAccel {
			get{return !prefs.BoolForKey("useAccel");}
			set{prefs.SetBool(!value,"useAccel");}
		}
		
		public static float Sensativity {
			get{return prefs.FloatForKey("sensitivity");}
			set{prefs.SetFloat(value,"sensitivity");}
		}
		public static bool IsFirstRun {			
			get{return !prefs.BoolForKey("isFirtRun");}
			set{prefs.SetBool(!value,"isFirtRun");}
		}		
		public static bool HasSeenTutorial {			
			get{return prefs.BoolForKey("HasSeenTutorial");}
			set{prefs.SetBool(value,"HasSeenTutorial");}
		}
		
		
		public static bool UseSound {			
			get{return !prefs.BoolForKey("useSound");}
			set{prefs.SetBool(!value,"useSound");}
		}
		
		public static int DbVersion
		{
			get{return 	prefs.IntForKey("dbVersion");}
			set{prefs.SetInt(value,"dbVersion");}
		}
		
		
		public static int Score 
		{
			get{return prefs.IntForKey("lastScore");}
			set{prefs.SetInt(value,"lastScore");}
		}
		public static int Level
		{
			get{return prefs.IntForKey("lastLevel");}
			set{prefs.SetInt(value,"lastLevel");}
		}
		public static bool LastScoreSaved
		{
			get{return prefs.BoolForKey("lastScoreSaved");}
			set{prefs.SetBool(value,"lastScoreSaved");}
		}
		
	}
}

