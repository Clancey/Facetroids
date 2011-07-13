#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
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
#endif

#endregion

namespace AsteroidsHD
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

        }
		SliderMenuEntry slider;
		MenuEntry useSound;
		public override void LoadContent ()
		{
			base.LoadContent();	
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
			useSound = new MenuEntry("");
			
			useSound.Selected += delegate {
				Settings.UseSound = !Settings.UseSound;	
				setTitles();
			};
			setTitles();
            slider = new SliderMenuEntry("Sensitivity",Settings.Sensativity,ScreenManager);
			
			slider.ValueChanged += (value)=>
			{
				Settings.Sensativity = value;
			};
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnGetFacebook;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
			
            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
			MenuEntries.Add(slider);
			MenuEntries.Add(useSound);
            MenuEntries.Add(quitGameMenuEntry);
		}
		
		private void setTitles()
		{
			useSound.Text = "Sound: " + (Settings.UseSound ? "On":"Off");
		}
		
		public override void UnloadContent ()
		{
			base.UnloadContent ();
			slider.Unload();
		}

        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            var confirmQuitMessageBox = new QuitMenu();

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }



        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            base.Draw(gameTime);
        }


        #endregion
    }
}
