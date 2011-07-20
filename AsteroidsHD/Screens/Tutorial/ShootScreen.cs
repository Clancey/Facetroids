using System;
using MonoTouch.UIKit;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
namespace AsteroidsHD
{
	public class ShootScreen: GameScreen
	{
		UIImageView image;
		UILabel label;
		
		public ShootScreen ()
		{
			
		}
		public override void LoadContent ()
		{
			
			var imageExists = File.Exists("Content/Tutorial/shootScreen.png");
			image = new UIImageView(UIImage.FromFile("Content/Tutorial/shootScreen.png"));
			label = new UILabel();
			label.Text = "Tap the right side of the screen to shoot";
			label.AdjustsFontSizeToFitWidth = true;
			label.TextAlignment = UITextAlignment.Center;
			label.Frame = new System.Drawing.RectangleF(10,10,this.ScreenManager.Width,150);
			label.TextColor = UIColor.White;
			label.BackgroundColor = UIColor.Clear;
			image.Frame = new System.Drawing.RectangleF((this.ScreenManager.Width - 150)/2,170,image.Frame.Width,image.Frame.Height);
			Guide.Window.AddSubview(image);
			Guide.Window.AddSubview(label);
			
		}
		public override void UnloadContent ()
		{
			label.RemoveFromSuperview();
			image.RemoveFromSuperview();
			label = null;
			image = null;
		}
		
		public override void Draw (Microsoft.Xna.Framework.GameTime gameTime)
		{
			//SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			//spriteBatch.Begin();
			label.Draw(label.Frame);
			image.Draw(image.Frame);
			//base.Draw (gameTime);
		}
	}
}

