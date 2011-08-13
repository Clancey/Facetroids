using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
namespace AsteroidsHD
{
	public class Slider : UIView
	{
		ScreenManager ScreenManager;
		public UISlider slider;
		public Slider (ScreenManager screenManager) : base (new System.Drawing.RectangleF(0,0,200,50))
		{
			ScreenManager = screenManager;
			Console.WriteLine("is window null?" + Guide.Window == null);
			slider = new UISlider(new System.Drawing.RectangleF(0,0,200,50));
			slider.MinValue = .1f; 
			slider.MaxValue = 1f;
			this.AddSubview(slider);
			//Guide.Window.AddSubview(this);
			Util.MainGame.Window.AddSubview(this);
			//UIApplication.SharedApplication.KeyWindow.AddSubview(this);
			
		}
		
		public void DrawMe(Rectangle rect)
		{	
			/*
			float degrees = 45;
			if (this.ScreenManager.GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeRight)
				degrees *= -1f;
			CGAffineTransform landscapeTransform = CGAffineTransform.MakeRotation(MathHelper.ToRadians(degrees));
			landscapeTransform.Translate( 80.0f, 100.0f);
			this.Transform = landscapeTransform;
			
			if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft)
				Transform = CGAffineTransform.MakeRotation(MathHelper.ToRadians(90f));
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
				Transform = CGAffineTransform.MakeRotation(MathHelper.ToRadians(-90f));
			else if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
				Transform = CGAffineTransform.MakeRotation(MathHelper.ToRadians(180f));
				*/
			this.Frame = new System.Drawing.RectangleF(rect.X,rect.Y,rect.Width,rect.Height);
			//slider.Frame =  new System.Drawing.RectangleF(0,0,rect.Width,rect.Height);
			//Game.View.BringSubviewToFront(this);
			
		}
		System.Drawing.RectangleF lastFrame;
		public override System.Drawing.RectangleF Frame {
			get {
				return base.Frame;
			}
			set {
				if(lastFrame == value)
					return;
				lastFrame = value;
				System.Drawing.PointF point = new System.Drawing.PointF(0,0);
				//if(Game.View.CurrentOrientation == DisplayOrientation.LandscapeLeft)
				//{
					point.X	 = value.Y - value.Height - 10;
					point.Y = value.X;
				/*}
				else if(Game.View.CurrentOrientation == DisplayOrientation.LandscapeRight)
				{
					Console.WriteLine(value);
					point.X	 =  value.Y;
					point.Y = Game.View.Frame.Height - value.X - 10;//Game.View.Frame.Height - value.X;
				}*/
				base.Frame = new System.Drawing.RectangleF(new System.Drawing.PointF(point.X,point.Y), value.Size);
			}
		}
		public void Unload()
		{
			this.RemoveFromSuperview();	
		}
	}
}

