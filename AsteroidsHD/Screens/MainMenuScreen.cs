#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
#if IPHONE
using Microsoft.Xna.Framework;
#else
using Microsoft.Xna.Framework;
using System;
using MonoTouch.Facebook.Authorization;
using MonoTouch.UIKit;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;
#endif
#endregion

namespace AsteroidsHD
{
	/// <summary>
	/// The main menu screen is the first thing displayed when the game starts up.
	/// </summary>
	class MainMenuScreen : MenuScreen
	{
		#region Initialization


		/// <summary>
		/// Constructor fills in the menu contents.
		/// </summary>
		public MainMenuScreen () : base("Facetroids")
		{
			// Create our menu entries.
			MenuEntry playGameMenuEntry = new MenuEntry ("Play Game");
			MenuEntry optionsMenuEntry = new MenuEntry ("Options");
			MenuEntry HighScores = new MenuEntry ("High Scores");
			MenuEntry getFacebookFaces = new MenuEntry ("Get Facebook Images");
			
			// Hook up menu event handlers.
			playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
			optionsMenuEntry.Selected += OptionsMenuEntrySelected;
			getFacebookFaces.Selected += OnGetFacebook;
			HighScores.Selected += HandleHighScoresSelected;
			
			// Add entries to the menu.
			MenuEntries.Add (playGameMenuEntry);
			MenuEntries.Add (optionsMenuEntry);
			MenuEntries.Add(HighScores);
			MenuEntries.Add (getFacebookFaces);
			
		}
		
		public override void LoadContent ()
		{
			base.LoadContent ();
						
			if(Util.CanUseGameCenter)
			{
				var center = new GamerServicesComponent(Util.MainGame);
				Guide.ShowSignIn(1,false);
				Util.SubmitScores();
			}
		}



		#endregion

		#region Handle Input


		/// <summary>
		/// Event handler for when the Play Game menu entry is selected.
		/// </summary>
		void PlayGameMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			if(Settings.GameType == GameType.Retro)
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameplayScreen ());
			else
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameScreen[]{Util.BackgroundScreen, new GameplayScreen ()});
		}


		/// <summary>
		/// Event handler for when the Options menu entry is selected.
		/// </summary>
		void OptionsMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			ScreenManager.AddScreen (new OptionsMenuScreen (), e.PlayerIndex);
		}

		void HandleHighScoresSelected (object sender, PlayerIndexEventArgs e)
		{
			//Guide.ShowAchievements();
			Guide.ShowLeaderboard();
		}

		/// <summary>
		/// When the user cancels the main menu, ask if they want to exit the sample.
		/// </summary>
		protected override void OnCancel (PlayerIndex playerIndex)
		{
			/*
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
            */			
			if ((string.IsNullOrEmpty (Settings.FbAuth) || Settings.FbAuthExpire <= DateTime.Now)) {
				var fvc = new FacebookAuthorizationViewController ("158978690838499", new string[] { "publish_stream" }, FbDisplayType.Touch);
				fvc.AccessToken += delegate(string accessToken, DateTime expires) {
					Settings.FbAuth = accessToken;
					Settings.FbAuthExpire = expires;
					fvc.View.RemoveFromSuperview ();
					TouchPanel.Reset();
					DataAccess.GetFriends ();
					//BackgroundUpdater.AddToFacebook();				
					//this.DismissModalViewControllerAnimated(true);
				};
					//this.DismissModalViewControllerAnimated(true);
				fvc.AuthorizationFailed += delegate(string message) { fvc.View.RemoveFromSuperview (); };
				fvc.Canceled += delegate {
					Console.WriteLine ("Canceled clicked");
					fvc.View.RemoveFromSuperview ();
					TouchPanel.Reset();
					//this.DismissModalViewControllerAnimated(true);
					//this.NavigationController.PopViewControllerAnimated(false);
				};
				fvc.View.Frame = new System.Drawing.RectangleF(System.Drawing.PointF.Empty,new System.Drawing.SizeF(fvc.View.Frame.Height,fvc.View.Frame.Width));
				Util.MainGame.View.AddSubview(fvc.View);
			} else
				DataAccess.GetFriends ();
		}

		
		
		#endregion
	}
}
