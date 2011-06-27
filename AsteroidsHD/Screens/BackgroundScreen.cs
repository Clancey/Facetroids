#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#if IPHONE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endif
#endregion

namespace AsteroidsHD
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        //Texture2D backgroundTexture;
		Texture2D t2dBackground, t2dParallax;
		int iViewportWidth = 1280;
        int iViewportHeight = 720;
 
        int iBackgroundWidth = 1920;
        int iBackgroundHeight = 720;
 
        int iParallaxWidth = 1680;
        int iParallaxHeight = 480;


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

			
			t2dBackground = content.Load<Texture2D>("PrimaryBackground");
            iBackgroundWidth = t2dBackground.Width;
            iBackgroundHeight = t2dBackground.Height;
            t2dParallax = content.Load<Texture2D>("ParallaxStars");
            iParallaxWidth = t2dParallax.Width;
            iParallaxHeight = t2dParallax.Height;
			
			iViewportWidth = ScreenManager.Width + 5;
			iViewportHeight = ScreenManager.Height + 5;
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
			BackgroundOffset += 1;
            ParallaxOffset += 3;
            base.Update(gameTime, otherScreenHasFocus, false);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			
            Rectangle fullscreen = new Rectangle(0, 0, ScreenManager.Width + 5, ScreenManager.Height + 5);
            byte fade = TransitionAlpha;
			
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            //spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.Opaque);

            //spriteBatch.Draw(backgroundTexture, fullscreen,
              //               new Color(fade, fade, fade));
			Draw(spriteBatch);

            spriteBatch.End();
        }
		
		public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background panel, offset by the player's location
            spriteBatch.Draw(
                t2dBackground,
                new Rectangle(-1 * iBackgroundOffset, 
                              0, iBackgroundWidth, 
                              iViewportHeight), 
                Color.White);
            
            // If the right edge of the background panel will end 
            // within the bounds of the display, draw a second copy 
            // of the background at that location.
            if (iBackgroundOffset > iBackgroundWidth-iViewportWidth) { 
                spriteBatch.Draw(
                    t2dBackground,
                    new Rectangle(
                      (-1 * iBackgroundOffset) + iBackgroundWidth, 
                      0, 
                      iBackgroundWidth,
                      iViewportHeight), 
                    Color.White); }
 
            if (drawParallax)
            {
                // Draw the parallax star field
                spriteBatch.Draw(
                    t2dParallax,
                    new Rectangle(-1 * iParallaxOffset, 
                                  0, iParallaxWidth, 
                                  iViewportHeight), 
                    Color.SlateGray);
                // if the player is past the point where the star 
                // field will end on the active screen we need 
                // to draw a second copy of it to cover the 
                // remaining screen area.
                if (iParallaxOffset > iParallaxWidth-iViewportWidth) { 
                    spriteBatch.Draw(
                        t2dParallax, 
                        new Rectangle(
                          (-1 * iParallaxOffset) + iParallaxWidth, 
                          0,
                          iParallaxWidth,
                          iViewportHeight), 
                        Color.SlateGray); }
            }
        }
		int iBackgroundOffset;
        int iParallaxOffset;
		public int BackgroundOffset
        {
            get { return iBackgroundOffset; }
            set
            {
                iBackgroundOffset = value;
                if (iBackgroundOffset < 0)
                {
                    iBackgroundOffset += iBackgroundWidth;
                }
                if (iBackgroundOffset > iBackgroundWidth)
                {
                    iBackgroundOffset -= iBackgroundWidth;
                }
            }
        }
 
        public int ParallaxOffset
        {
            get { return iParallaxOffset; }
            set
            {
                iParallaxOffset = value;
                if (iParallaxOffset < 0)
                {
                    iParallaxOffset += iParallaxWidth;
                }
                if (iParallaxOffset > iParallaxWidth)
                {
                    iParallaxOffset -= iParallaxWidth;
                }
            }
        }
		bool drawParallax = true;
 
        public bool DrawParallax
        {
            get { return drawParallax; }
            set { drawParallax = value; }
        }
        #endregion
    }
}
