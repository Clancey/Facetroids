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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
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
		public MainMenuScreen () : base("")
		{
			// Create our menu entries.
			MenuEntry playGameMenuEntry = new MenuEntry ("Play Game");
			MenuEntry optionsMenuEntry = new MenuEntry ("Options");
			MenuEntry HighScores = new MenuEntry ("High Scores");
			MenuEntry HowTowPlay = new MenuEntry ("How to Play");
			MenuEntry getFacebookFaces = new MenuEntry ("Get Facebook Images");
			
			// Hook up menu event handlers.
			playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
			optionsMenuEntry.Selected += OptionsMenuEntrySelected;
			getFacebookFaces.Selected += OnGetFacebook;
			HighScores.Selected += HandleHighScoresSelected;
			HowTowPlay.Selected += HandleHowTowPlaySelected;
			
			// Add entries to the menu.
			MenuEntries.Add (playGameMenuEntry);
			MenuEntries.Add (optionsMenuEntry);
			MenuEntries.Add (HighScores);
			MenuEntries.Add (HowTowPlay);
			MenuEntries.Add (getFacebookFaces);
			
		}

		Texture2D logo;
		ContentManager content;
		public override void LoadContent ()
		{
			base.LoadContent ();
			
			if (content == null)
				content = new ContentManager (ScreenManager.Game.Services, "Content");
			
			logo = content.Load<Texture2D>("facetroids" + (Util.IsIpad ? "-ipad" : ""));
			var center = new GamerServicesComponent (Util.MainGame);
			if (Util.CanUseGameCenter) {
				Guide.ShowSignIn (1, false);
				Util.SubmitScores ();
			}
		}



		#endregion

		#region Handle Input


		/// <summary>
		/// Event handler for when the Play Game menu entry is selected.
		/// </summary>
		void PlayGameMenuEntrySelected (object sender, PlayerIndexEventArgs e)
		{
			
			if (Settings.GameType == GameType.Retro)
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameplayScreen (!Settings.HasSeenTutorial));
			else
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameScreen[] { Util.BackgroundScreen, new GameplayScreen (!Settings.HasSeenTutorial) });
		}


		void HandleHowTowPlaySelected (object sender, PlayerIndexEventArgs e)
		{
			if (Settings.GameType == GameType.Retro)
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameplayScreen (true));
			else
				LoadingScreen.Load (ScreenManager, true, e.PlayerIndex, new GameScreen[] { Util.BackgroundScreen, new GameplayScreen (true) });
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
			Guide.ShowLeaderboard ();
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
			Facebook.DownloadFaces();
		}

		public override void Draw (GameTime gameTime)
		{
			base.Draw (gameTime);
			
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
			
			spriteBatch.Draw(logo,new Vector2((ScreenManager.Width - logo.Width) / 2,25),Color.White);
			var versionSize = ScreenManager.Font.MeasureString(Util.VersionString);
			var pos = new Vector2(10,ScreenManager.Height - versionSize.Y);
			spriteBatch.DrawString(ScreenManager.Font,Util.VersionString,pos,Color.White);
			spriteBatch.End();
			
		}
		
		
		#endregion
	}
}
