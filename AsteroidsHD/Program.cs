using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Facebook.Authorization;
using Microsoft.Xna.Framework.GamerServices;

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
			//var gc = new GamerServicesComponent(game);
			if(Util.CanUseGameCenter)
				Guide.ShowSignIn(1,false);
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
