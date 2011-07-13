#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#if IPHONE
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#else
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using MonoTouch.UIKit;
#endif
#endregion

namespace AsteroidsHD
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;
		string AditionalHeader;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        /// 
        public MenuScreen(string menuTitle,string additionalHeader)
        {
            this.menuTitle = menuTitle;
			this.AditionalHeader = additionalHeader;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }
        public MenuScreen(string menuTitle): this(menuTitle,"")
        {
			
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
			
			var touches  = TouchPanel.GetState();
			if( touches.Count == 1)
			{
				var touch = touches[0];
				var position = GetOffsetPosition(touch.Position,false);
				
				if(touch.State == TouchLocationState.Pressed  || touch.State == TouchLocationState.Moved)
				{
					for (int i = 0; i < menuEntries.Count; i++)
					{
						MenuEntry menuEntry = menuEntries[i];
						if(menuEntry.Frame.Contains(position))
							selectedEntry = i;
					}	
				}
				else if (touch.State ==  TouchLocationState.Released	)
				{
					for (int i = 0; i < menuEntries.Count; i++)
					{
						MenuEntry menuEntry = menuEntries[i];
						if(menuEntry.Frame.Contains(position))
						{
							selectedEntry = i;
							OnSelectEntry(i, PlayerIndex.One);
						}
					}
				}
				
			}
			
			
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }
		
		internal Vector2 GetOffsetPosition(Vector2 position, bool useScale)
		{
			Vector2 translatedPosition = position / UIScreen.MainScreen.Scale;
			return translatedPosition;
		}

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnGetFacebook(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
			
			var center = ScreenManager.Width / 2;
            Vector2 position = new Vector2(center, 140);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;
			
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
			//spriteBatch.Begin();
            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);// + 10;
            }

            // Draw the menu title.            
            Vector2 headerPosition = new Vector2(ScreenManager.Width / 2, 40);
            Vector2 headerOrigin = font.MeasureString(AditionalHeader) / 2;
            Color headerColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            headerPosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, AditionalHeader, headerPosition, headerColor, 0,
                                   headerOrigin, titleScale, SpriteEffects.None, 0);
			
			
            Vector2 titlePosition = new Vector2(ScreenManager.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);

            titlePosition.Y -= transitionOffset * 100;


            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
			base.Draw(gameTime);
        }


        #endregion
    }
}
