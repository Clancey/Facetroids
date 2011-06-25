using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Facebook.Authorization;

namespace AsteroidsHD
{
	[Register ("AppDelegate")]
	class Program : UIApplicationDelegate 
	{
		private MainGame game;
		FacebookAuthorizationViewController fvc;
		public override void FinishedLaunching (UIApplication app)
		{
			// Fun begins..
			game = new MainGame();
			game.Run();
			//DataAccess.GetFriends();
			//FaceDetection.DetectFaces();
			UIApplication.SharedApplication.ApplicationSupportsShakeToEdit = true;
		}

		static void Main (string [] args)
		{
			UIApplication.Main (args,null,"AppDelegate");
		}
		
		public override void DidEnterBackground (UIApplication application)
		{
			game.ScreenManager.GlobalPause();
		}
	}
}
