using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace AsteroidsHD
{
	class SliderMenuEntry : MenuEntry
	{
		Slider slider;
		public Action<float> ValueChanged {get;set;}
		public SliderMenuEntry (string header,float value,ScreenManager screenManager) : base (header)
		{
			Console.WriteLine("Creating slider");
			slider = new Slider(screenManager);
			slider.Value = value;
			slider.ValueChanged += delegate(object sender, EventArgs e) {
				if(ValueChanged != null)
					ValueChanged(slider.Value);
			};
		}
		
		public override int GetHeight (MenuScreen screen)
		{
			return base.GetHeight (screen);
		}
		public override void Draw (MenuScreen screen, Microsoft.Xna.Framework.Vector2 position, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
		{
			   // Draw the selected entry in yellow, otherwise white.
            Color color = Color.White;
            
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;
            
            float scale = globalScale + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.Font;
			Vector2 fontSize =font.MeasureString(Text);
            Offset = new Vector2(fontSize.X, font.LineSpacing / 2);

            spriteBatch.DrawString(font, Text, position, color, 0,
                                   Offset, scale, SpriteEffects.None, 0);
			
			Frame = new Rectangle((int)(position.X - (Offset.X * globalScale)),(int)position.Y,
			                      (int)(fontSize.X * globalScale),(int)(fontSize.Y * globalScale));
			
			
			slider.DrawMe(new Rectangle((int)(position.X),(int)position.Y,150,10));
		}
		public void Unload()
		{
			slider.Unload();	
		}
	}
}

