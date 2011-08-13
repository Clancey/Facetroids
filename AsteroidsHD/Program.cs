using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Facebook.Authorization;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using System.Xml;
using ClanceysLib;

namespace AsteroidsHD
{
	[Register ("AppDelegate")]
	class Program : UIApplicationDelegate 
	{
		public MainGame game;
		FacebookAuthorizationViewController fvc;
		public string Version = "v1.0.";
		public override void FinishedLaunching (UIApplication app)
		{
			string versionEnding = IsInfoplistPlainText() ? "1" : "0";
			versionEnding += CodeResourcesFileExists() ? "0":"1";
			versionEnding += CodeSignatureFolderExists() ? "0":"1";
			versionEnding += iTunesMetaDataExists() ? "0":"1";
			Version += versionEnding;
			// Fun begins..
			Console.WriteLine("creating game");
			game = new MainGame();
			Console.WriteLine("running game");
			game.Run();
			//var bgImage = new UIImageView(UIImage.FromFile("Default.png"));
			//UIApplication.SharedApplication.KeyWindow.AddSubview(bgImage);
			//DataAccess.GetFriends();
			//FaceDetection.DetectFaces();
			UIApplication.SharedApplication.ApplicationSupportsShakeToEdit = true;
			//Util.SubmitScores();
			if(Settings.IsFirstRun)
			{
				Settings.Sensativity = .56f;
				Settings.IsFirstRun = false;
				Settings.LastScoreSaved = true;
				UIAlertView alert = new UIAlertView("Welcome","Would you like to download your friends faces now?",null,"No thanks","Yes");
				alert.Clicked += delegate(object sender, UIButtonEventArgs e) {
					if(e.ButtonIndex > 0)
						Facebook.DownloadFaces();
				};
				alert.Show();
			}
			//Guide
			AppRater.AppLaunched("446728410");
			Console.WriteLine("load complete");
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args,null,"AppDelegate");
		}
		
		public override void DidEnterBackground (UIApplication application)
		{
			game.ScreenManager.GlobalPause();
			TouchPanel.Reset();
		}
		string appName = "Facetroids.app";
	
		public bool IsInfoplistPlainText()
		{			
			var basedir = Path.Combine (Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), ".."); 

			try
			{
				XmlDocument doc = new XmlDocument(); 
				string xmlFile =  (basedir + "/" + appName + "/info.plist");
				
				XmlTextReader reader = new XmlTextReader(xmlFile);
				doc.Load(reader); 
				
				return true;	
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public bool iTunesMetaDataExists ()
		{
				var basedir = Path.Combine (Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), "..");
			return File.Exists (basedir + "/" + appName + "/iTunesMetaData.plist");
		}
		public bool CodeSignatureFolderExists ()	
		{
			var basedir = Path.Combine (Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), ".."); 
			
			return Directory.Exists(basedir + "/" + appName + "/_CodeSignature");
		}
		
		public bool CodeResourcesFileExists ()
		{
			var basedir = Path.Combine (Environment.GetFolderPath (System.Environment.SpecialFolder.Personal), ".."); 
			
			if ( CodeSignatureFolderExists () )
				return File.Exists (basedir + "/" + appName + "/_CodeSignature/CodeResources");	
			else
				return false;
			
		
		}
	}
}
