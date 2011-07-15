using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
namespace AsteroidsHD
{
	public class Slider : UISlider
	{
		ScreenManager ScreenManager;
		public Slider (ScreenManager screenManager) : base (new System.Drawing.RectangleF(0,0,200,50))
		{
			this.MinValue = .1f;
			this.MaxValue = 1f;
			ScreenManager = screenManager;
			Console.WriteLine("is window null?" + Guide.Window == null);
			Guide.Window.AddSubview(this);
			//ScreenManager.Game.Window.AddSubview(this);
			//UIApplication.SharedApplication.KeyWindow.AddSubview(this);
			
		}
		
		public void DrawMe(Rectangle rect)
		{	
			/*
			float degrees = 90f;
			if (this.ScreenManager.GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeRight)
				degrees *= -1f;
			CGAffineTransform landscapeTransform = CGAffineTransform.MakeRotation(MathHelper.ToRadians(degrees));
			landscapeTransform.Translate( 80.0f, 100.0f);
			this.Transform = landscapeTransform;
			*/
			this.Frame = new System.Drawing.RectangleF(rect.X,rect.Y,rect.Width,rect.Height);
		}
		public void Unload()
		{
			this.RemoveFromSuperview();	
		}
	}
}

