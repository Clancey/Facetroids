using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace AsteroidsHD
{
	public class InstructionsScreen: GameScreen
	{
		public InstructionsScreen ():base()
		{
		}
		
		public override void Draw (Microsoft.Xna.Framework.GameTime gameTime)
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
		}
	}
}

