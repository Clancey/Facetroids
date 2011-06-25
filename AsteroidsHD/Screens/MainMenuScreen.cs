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
        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Get Facebook Images");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
			/*
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
            */
			if((string.IsNullOrEmpty(Settings.FbAuth) || Settings.FbAuthExpire <= DateTime.Now))
			{				
				var fvc = new FacebookAuthorizationViewController("172317429487848", new string[] {"publish_stream"}, FbDisplayType.Touch);
				fvc.AccessToken += delegate(string accessToken, DateTime expires) {
					Settings.FbAuth = accessToken;
					Settings.FbAuthExpire = expires;
					fvc.View.RemoveFromSuperview();
					
					DataAccess.GetFriends();
					//BackgroundUpdater.AddToFacebook();				
					//this.DismissModalViewControllerAnimated(true);
				};
				fvc.AuthorizationFailed += delegate(string message) {
					fvc.View.RemoveFromSuperview();					
					//this.DismissModalViewControllerAnimated(true);
				};
				fvc.Canceled += delegate {	
					Console.WriteLine("Canceled clicked");	
					fvc.View.RemoveFromSuperview();
					//this.DismissModalViewControllerAnimated(true);
					//this.NavigationController.PopViewControllerAnimated(false);
				};
				UIApplication.SharedApplication.KeyWindow.AddSubview(fvc.View);
			}
			else				
					DataAccess.GetFriends();
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
