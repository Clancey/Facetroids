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
	class BackgroundScreenOld : GameScreen
	{
		#region Fields
		public Vector2 Center{get;set;}
		public bool AutoMove {get;set;}
		ContentManager content;
		//Texture2D backgroundTexture;
		Texture2D t2dBackground, t2dParallax;
		int iViewportWidth = 1280;
		int iViewportHeight = 720;

		int iBackgroundWidth = 1920;
		int iBackgroundHeight = 720;

		int iParallaxWidth = 1680;
		int iParallaxHeight = 480;
		public Vector2 Velocity = new Vector2 (1, 1);
		private int color = 0;
		public void ChangeColor ()
		{
			
			Console.WriteLine ("changing background");
			string image = "PrimaryBackground";
			
			switch (color) {
			case 1:
				image += "-green";
				break;
			case 2:
				image += "-purple";
				break;
			case 3:
				image += "-teal";
				break;
			case 4:
				image += "-yellow";
				break;
			default:
				break;
			}
			
			//if(t2dBackground == null)
			//t2dBackground.Dispose();
			t2dBackground = content.Load<Texture2D> (image);
			//color ++;
			if (color > 4)
				color = 0;
		}

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public BackgroundScreenOld ()
		{
			TransitionOnTime = TimeSpan.FromSeconds (0.5);
			TransitionOffTime = TimeSpan.FromSeconds (0.5);
		}


		/// <summary>
		/// Loads graphics content for this screen. The background texture is quite
		/// big, so we use our own local ContentManager to load it. This allows us
		/// to unload before going from the menus into the game itself, wheras if we
		/// used the shared ContentManager provided by the Game class, the content
		/// would remain loaded forever.
		/// </summary>
		public override void LoadContent ()
		{
			if (content == null)
				content = new ContentManager (ScreenManager.Game.Services, "Content");
			
			ChangeColor ();
			
			iBackgroundWidth = t2dBackground.Width;
			iBackgroundHeight = t2dBackground.Height;
			t2dParallax = content.Load<Texture2D> ("ParallaxStars");
			iParallaxWidth = t2dParallax.Width;
			iParallaxHeight = t2dParallax.Height;
			
			iViewportWidth = ScreenManager.Width;
			iViewportHeight = ScreenManager.Height;
		}


		/// <summary>
		/// Unloads graphics content for this screen.
		/// </summary>
		public override void UnloadContent ()
		{
			content.Unload ();
			
			t2dBackground.Dispose ();
			t2dParallax.Dispose ();
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
		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			BackgroundOffsetX += (int)Velocity.X;
			//BackgroundOffsetY -= (int)Velocity.Y;
			ParallaxOffsetX += (int)(3 * Velocity.X);
			//ParallaxOffsetY -= (int)(3 *Velocity.Y);
			base.Update (gameTime, otherScreenHasFocus, false);
		}


		/// <summary>
		/// Draws the background screen.
		/// </summary>
		public override void Draw (GameTime gameTime)
		{
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			
			Rectangle fullscreen = new Rectangle (0, 0, ScreenManager.Width + 5, ScreenManager.Height + 5);
			byte fade = TransitionAlpha;
			
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, _view);
			//spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.Opaque);
			
			//spriteBatch.Draw(backgroundTexture, fullscreen,
			//               new Color(fade, fade, fade));
			Draw (spriteBatch);
			
			spriteBatch.End ();
		}
		const int offset = 0;
		public void Draw (SpriteBatch spriteBatch)
		{
			// Draw the background panel, offset by the player's location
			int curH = 0;
			while (curH < iViewportHeight) {
				spriteBatch.Draw (t2dBackground, new Rectangle (-1 * iBackgroundOffsetX, iBackgroundOffsetY + curH, iBackgroundWidth, iBackgroundHeight), Color.White);
				curH += iBackgroundHeight;
			}
			
			// If the right edge of the background panel will end 
			// within the bounds of the display, draw a second copy 
			// of the background at that location.
			curH = 0;
			while (curH < iViewportHeight) {
				if (iBackgroundOffsetX > iBackgroundWidth - iViewportWidth) {
					spriteBatch.Draw (t2dBackground, new Rectangle ((-1 * iBackgroundOffsetX) + iBackgroundWidth, iBackgroundOffsetY + curH, iBackgroundWidth, iBackgroundHeight), Color.White);
				}
				curH += iBackgroundHeight;
			}
			
			if (iBackgroundOffsetY > iBackgroundHeight - iViewportHeight - offset) {
				spriteBatch.Draw (t2dBackground, new Rectangle (-1 * iBackgroundOffsetX, (iBackgroundOffsetY) - iBackgroundHeight - offset, iBackgroundWidth, iBackgroundHeight + offset), Color.White);
			}
			
			
			
			
			if (iBackgroundOffsetY > iBackgroundHeight - iViewportHeight - offset && iBackgroundOffsetX > iBackgroundWidth - iViewportWidth) {
				spriteBatch.Draw (t2dBackground, new Rectangle ((-1 * iBackgroundOffsetX) + iBackgroundWidth, (iBackgroundOffsetY) - iBackgroundHeight - offset, iBackgroundWidth, iBackgroundHeight + offset), Color.White);
			}
			
			
			if (drawParallax) {
				// Draw the parallax star field
				spriteBatch.Draw (t2dParallax, new Rectangle (-1 * iParallaxOffsetX, iParallaxOffsetY, iParallaxWidth, iViewportHeight), Color.SlateGray);
				// if the player is past the point where the star 
				// field will end on the active screen we need 
				// to draw a second copy of it to cover the 
				// remaining screen area.
				if (iParallaxOffsetX > iParallaxWidth - iViewportWidth) {
					spriteBatch.Draw (t2dParallax, new Rectangle ((-1 * iParallaxOffsetX) + iParallaxWidth, iParallaxOffsetY, iParallaxWidth, iViewportHeight), Color.SlateGray);
				}
				/*
				if(iParallaxOffsetY > iParallaxHeight - iViewportHeight - 40 ){
                spriteBatch.Draw(
                    t2dParallax,
                    new Rectangle(
                      -1 * iParallaxOffsetX, 
                      (  iParallaxOffsetY) - iParallaxHeight, 
                      iParallaxWidth,
                      iViewportHeight + 40 ), 
                    Color.White); }
			
			
			
			
			if(iParallaxOffsetY > iParallaxHeight - iViewportHeight -40 && iParallaxOffsetX > iParallaxWidth-iViewportWidth){
                spriteBatch.Draw(
                    t2dParallax,
                    new Rectangle(
                      (-1 * iParallaxOffsetX) + iParallaxWidth, 
                      (  iParallaxOffsetY) - iParallaxHeight, 
                      iParallaxWidth,
                      iViewportHeight + 40), 
                    Color.White); }
				*/				
			}			
		}
		int iBackgroundOffsetX;
		int iBackgroundOffsetY;
		int iParallaxOffsetX;
		int iParallaxOffsetY;
		public int BackgroundOffsetX {
			get { return iBackgroundOffsetX; }
			set {
				iBackgroundOffsetX = value;
				if (iBackgroundOffsetX < 0) {
					iBackgroundOffsetX += iBackgroundWidth;
				}
				if (iBackgroundOffsetX > iBackgroundWidth) {
					iBackgroundOffsetX -= iBackgroundWidth;
				}
			}
		}
		public int BackgroundOffsetY {
			get { return iBackgroundOffsetY; }
			set {
				iBackgroundOffsetY = value;
				if (iBackgroundOffsetY < 0) {
					iBackgroundOffsetY += iBackgroundHeight;
				}
				if (iBackgroundOffsetY > iBackgroundHeight) {
					iBackgroundOffsetY -= iBackgroundHeight;
				}
			}
		}

		public int ParallaxOffsetX {
			get { return iParallaxOffsetX; }
			set {
				iParallaxOffsetX = value;
				if (iParallaxOffsetX < 0) {
					iParallaxOffsetX += iParallaxWidth;
				}
				if (iParallaxOffsetX > iParallaxWidth) {
					iParallaxOffsetX -= iParallaxWidth;
				}
			}
		}
		public int ParallaxOffsetY {
			get { return iParallaxOffsetY; }
			set {
				iParallaxOffsetY = value;
				if (iParallaxOffsetY < 0) {
					iParallaxOffsetY += iParallaxHeight;
				}
				if (iParallaxOffsetY > iParallaxHeight) {
					iParallaxOffsetY -= iParallaxHeight;
				}
			}
		}
		bool drawParallax = true;

		public bool DrawParallax {
			get { return drawParallax; }
			set { drawParallax = value; }
		}
		#endregion
	}
	
}

