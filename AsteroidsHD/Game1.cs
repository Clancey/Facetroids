using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input.Touch;
using MonoTouch.UIKit;
using OpenTK.Graphics.ES11;

namespace AsteroidsHD
{
	
	public enum GameType
	{
		Retro,
		Modern,
		Facebook
	}
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Ship ship;

		int ScreenWidth, ScreenHeight;
		int level = 1;
		int score = 0;
		int lives;
		float scale;
		bool useAccel = true;
		GameType gameType = GameType.Facebook;
		
		int baseAsteroids = Util.IsIpad ? 5 : 3;
		
		KeyboardState oldState;

		Sprite bullet;
		List<Sprite> bullets = new List<Sprite> ();
		Texture2D gamePadTexture;
		List<Texture2D> asteroidTextures = new List<Texture2D> ();
		List<Sprite> asteroids = new List<Sprite> ();
		ObjModel AwesomeAsteroid;

		SpriteFont myFont;
		Texture2D banner;
		Texture2D asteroid;

		float distance = 0.0f;

		Random random = new Random ();

		bool GameOver = true;
		Texture2D astroidBacking;

		public Game1 ()
		{
			scale = UIScreen.MainScreen.Scale;
			graphics = new GraphicsDeviceManager (this);
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = true;
			ScreenHeight = (int)(UIScreen.MainScreen.Bounds.Width);
			ScreenWidth = (int)(UIScreen.MainScreen.Bounds.Height);
			graphics.PreferredBackBufferHeight = ScreenHeight;
			graphics.PreferredBackBufferWidth = ScreenWidth;
			View.Shake += delegate(object sender, EventArgs e) {
				Console.WriteLine("I was shaken");	
			};
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			base.Initialize ();			
			SetupGame ();
			//FaceDetection.DetectFaces();
		}


		private void SetupGame ()
		{
			//ScreenHeight = (int)UIScreen.MainScreen.Bounds.Width;
			//ScreenWidth = (int)UIScreen.MainScreen.Bounds.Height;
			SetupShip ();
			CreateAsteroids ();
		}

		private void SetupShip ()
		{
			ship.Position = new Vector2 (ScreenWidth / 2, ScreenHeight / 2);
			ship.Rotation = 0f;
			ship.Velocity = Vector2.Zero;
			ship.Create ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);
			AwesomeAsteroid = ObjModelLoader.LoadFromFile("Models/rock.obj");
			asteroid = Texture2D.FromFile(graphics.GraphicsDevice,"Models/rock.jpg",256,256);
			
			gamePadTexture = Content.Load<Texture2D> ("gamepad.png");
			var ship1t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship.pdf";
			var ship2t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship-thrust.pdf";
			var ship1 = Texture2D.FromFile(graphics.GraphicsDevice,ship1t,35,35);
			var ship2 = Texture2D.FromFile(graphics.GraphicsDevice,ship2t,35,35);
			ship = new Ship (ship1,ship2);
			//ship.Scale = .05f;
			bullet = new Sprite (Content.Load<Texture2D> ("shot-frame1"));
			//bullet.Scale = .05f;
			ReloadAsteroids();
			
			myFont = Content.Load<SpriteFont> ("Fonts/SpriteFont1");
			
			banner = Content.Load<Texture2D> ("BANNER");
	
			var gamePadH = ScreenHeight - 100;
			var gamePadLeft = ScreenWidth -80;
			//Console.WriteLine (gamePadH);
			// Set the virtual GamePad
			ButtonDefinition BButton = new ButtonDefinition ();
			BButton.Texture = gamePadTexture;
			BButton.Position = new Vector2 (gamePadLeft, gamePadH + 10);
			BButton.Type = Buttons.B;
			BButton.TextureRect = new Rectangle (72, 77, 36, 36);
			
			ButtonDefinition AButton = new ButtonDefinition ();
			AButton.Texture = gamePadTexture;
			AButton.Position = new Vector2 (gamePadLeft - 75, gamePadH + 10);
			AButton.Type = Buttons.A;
			AButton.TextureRect = new Rectangle (73, 114, 36, 36);
			
			GamePad.ButtonsDefinitions.Add (BButton);
			GamePad.ButtonsDefinitions.Add (AButton);
			
			ThumbStickDefinition thumbStick = new ThumbStickDefinition ();
			thumbStick.Position = new Vector2 (50, gamePadH);
			thumbStick.Texture = gamePadTexture;
			thumbStick.TextureRect = new Rectangle (2, 2, 68, 68);
			
			
			GamePad.LeftThumbStickDefinition = thumbStick;
			
		}
		
		public void ReloadAsteroids()
		{
			asteroidTextures.Clear();
			if(gameType == GameType.Modern)
			{
				asteroidTextures.Add (Texture2D.FromFile(graphics.GraphicsDevice,"Content/asteroid-front.pdf",60,60));
				return;
			}
			else if(gameType == GameType.Facebook)
			{
				foreach(var img  in Facebook.GetImages())
				{
					Console.WriteLine(img);
					asteroidTextures.Add (Texture2D.FromFile(graphics.GraphicsDevice,img,60,60));	
				}
				return;
			}
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (Texture2D.FromFile(graphics.GraphicsDevice,"Content/Retro/large" + i +".pdf",60,60));//60

			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (Texture2D.FromFile(graphics.GraphicsDevice,"Content/Retro/medium" + i +".pdf",60,60)); //45
			
			for (int i = 1; i < 4; i++)			
				asteroidTextures.Add (Texture2D.FromFile(graphics.GraphicsDevice,"Content/Retro/small" + i +".pdf",60,60));//25
			
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent ()
		{
			asteroidTextures.Clear();
		}
		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		GamePadState oldGamePadState;
		bool LeftSideTouchedOld;
		bool RightSideTouchedOld;
		bool tapDown = false;
		protected override void Update (GameTime gameTime)
		{
			//ScreenHeight = graphics.GraphicsDevice.DisplayMode.Height;
			//ScreenWidth = graphics.GraphicsDevice.DisplayMode.Width;
			//Console.WriteLine(ScreenWidth);
			KeyboardState newState = Keyboard.GetState ();
			
			TouchCollection touchState = TouchPanel.GetState ();
			
			if (GameOver) {
				asteroids.Clear ();
				ship.Kill ();
				var isTouching = touchState.Count > 0;
				if (!tapDown && isTouching) {
					level = 1;
					score = 0;
					lives = 3;
					SetupGame ();
					CreateAsteroids ();
					GameOver = false;
				} 
				tapDown = isTouching;	
				return;
				
			}
			
			
			if(useAccel)
				updateFromAccelerometer(gameTime,touchState);
			else
				updateFromGamePad(gameTime);
			
			
			
			
			UpdateShip ();
			UpdateAsteroids ();
			UpdateBullets ();
			AllDead ();
			
			base.Update (gameTime);
		}
		
		private void updateFromAccelerometer(GameTime gameTime,TouchCollection touchState)
		{
			
			var accelState = Accelerometer.GetState().Acceleration;	
			
			var position = new Vector2 (0, 0);
			//if(Math.Abs(accelState.Y) > .1f)
				position.X = accelState.Y * 4 * .5f;
			
			if(this.graphics.GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeRight)
				position.X *= -1;
			
			ship.Rotation -= 0.05f * position.X;
			
			var halfScreen = ScreenWidth / 2;
			bool leftSideTouched = false;
			bool rightSideTouched = false;
			foreach(var touch in touchState)
			{
				if(touch.State != TouchLocationState.Released || touch.State != TouchLocationState.Invalid) 
				{
					if(touch.Position.X < halfScreen)
						leftSideTouched = true;
					else
						rightSideTouched = true;
				}
					
			}
			
			if (!RightSideTouchedOld && rightSideTouched)
				FireBullet ();
			
			if (leftSideTouched)
				AccelerateShip ();
			else
				DecelerateShip ();
			
			LeftSideTouchedOld = leftSideTouched;
			RightSideTouchedOld = rightSideTouched;
			
		}
		
		private void updateFromGamePad(GameTime gameTime)
		{
			GamePadState gamepastatus = GamePad.GetState (PlayerIndex.One);
			var position = new Vector2 (0, 0);
			position.Y += (int)(gamepastatus.ThumbSticks.Left.Y * -4);
			position.X += (int)(gamepastatus.ThumbSticks.Left.X * 4);	
			
			ship.Rotation -= 0.05f * position.X;
		
			if (gamepastatus.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released)
				FireBullet ();
			
			if (position.Y < 0)
				AccelerateShip ();
			else
				DecelerateShip ();
			
			
			if (gamepastatus.Buttons.A == ButtonState.Pressed && oldGamePadState.Buttons.A == ButtonState.Released)
					HyperSpace ();
			
			
			//if (newState.IsKeyUp (Keys.RightControl) && oldState.IsKeyDown (Keys.RightControl)) {
			//	HyperSpace ();
			//}
			
			//oldState = newState;
			oldGamePadState = gamepastatus;
		}

		private void AllDead ()
		{
			bool allDead = true;
			
			foreach (Sprite s in asteroids) {
				if (s.Alive)
					allDead = false;
			}
			
			if (allDead) {
				SetupGame ();
				level++;
				asteroids.Clear ();
				CreateAsteroids ();
				bullets.Clear();
			}
			
		}

		private void HyperSpace ()
		{
			int positionX;
			int positionY;
			
			positionX = random.Next (ship.Width, ScreenWidth - ship.Width);
			positionY = random.Next (ship.Height, ScreenHeight - ship.Height);
			
			ship.Position = new Vector2 (positionX, positionY);
			
			ship.Velocity = Vector2.Zero;
		}

		private void AccelerateShip ()
		{
			ship.IsThrusting = true;
			ship.Velocity += new Vector2 ((float)(Math.Cos (ship.Rotation - MathHelper.PiOver2) * 0.05f), (float)((Math.Sin (ship.Rotation - MathHelper.PiOver2) * 0.05f)));
			
			if (ship.Velocity.X > 5.0f) {
				ship.Velocity = new Vector2 (5.0f, ship.Velocity.Y);
			}
			if (ship.Velocity.X < -5.0f) {
				ship.Velocity = new Vector2 (-5.0f, ship.Velocity.Y);
			}
			if (ship.Velocity.Y > 5.0f) {
				ship.Velocity = new Vector2 (ship.Velocity.X, 5.0f);
			}
			if (ship.Velocity.Y < -5.0f) {
				ship.Velocity = new Vector2 (ship.Velocity.X, -5.0f);
			}
		}

		private void DecelerateShip ()
		{
			ship.IsThrusting = false;
			if (ship.Velocity.X < 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X + 0.02f, ship.Velocity.Y);
			}
			
			if (ship.Velocity.X > 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X - 0.02f, ship.Velocity.Y);
			}
			
			if (ship.Velocity.Y < 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X, ship.Velocity.Y + 0.02f);
			}
			
			if (ship.Velocity.Y > 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X, ship.Velocity.Y - 0.02f);
			}
		}

		public void UpdateShip ()
		{
			ship.Position += ship.Velocity;
			
			if (ship.Position.X + ship.Width < 0) {
				ship.Position = new Vector2 (ScreenWidth, ship.Position.Y);
			}
			if (ship.Position.X - ship.Width > ScreenWidth) {
				ship.Position = new Vector2 (0, ship.Position.Y);
			}
			if (ship.Position.Y + ship.Height < 0) {
				ship.Position = new Vector2 (ship.Position.X, ScreenHeight);
			}
			if (ship.Position.Y - ship.Height > ScreenHeight) {
				ship.Position = new Vector2 (ship.Position.X, 0);
			}
		}

		private void CreateAsteroids ()
		{
			int value;
			
			for (int i = 0; i < baseAsteroids + level; i++) {
				int index = random.Next (0,asteroidTextures.Count - 1 );
				
				Sprite tempSprite = new Sprite (asteroidTextures[index]);
				asteroids.Add (tempSprite);
				asteroids[i].Index = 1;
				
				double xPos = 0;
				double yPos = 0;
				
				value = random.Next (0, 8);
				
				switch (value) {
				case 0:
				case 1:
					xPos = asteroids[i].Width + random.NextDouble () * 40;
					yPos = random.NextDouble () * ScreenHeight;
					break;
				case 2:
				case 3:
					xPos = ScreenWidth - random.NextDouble () * 40;
					yPos = random.NextDouble () * ScreenHeight;
					break;
				case 4:
				case 5:
					xPos = random.NextDouble () * ScreenWidth;
					yPos = asteroids[i].Height + random.NextDouble () * 40;
					break;
				default:
					xPos = random.NextDouble () * ScreenWidth;
					yPos = ScreenHeight - random.NextDouble () * 40;
					break;
				}
				
				asteroids[i].Position = new Vector2 ((float)xPos, (float)yPos);
				
				asteroids[i].Velocity = RandomVelocity ();
				
				asteroids[i].Rotation = (float)random.NextDouble () * MathHelper.Pi * 4 - MathHelper.Pi * 2;
				asteroids[i].RotationSpin = (float)random.NextDouble() * random.Next(-1,1) ;
				asteroids[i].Create ();
			}
		}

		private void UpdateAsteroids ()
		{
			foreach (Sprite a in asteroids) {
				a.Position += a.Velocity;
				a.Rotation += a.RotationSpin * .05f;
				
				if (a.Position.X + a.Width < 0) {
					a.Position = new Vector2 (ScreenWidth, a.Position.Y);
				}
				if (a.Position.Y + a.Height < 0) {
					a.Position = new Vector2 (a.Position.X, ScreenHeight);
				}
				if (a.Position.X - a.Width > ScreenWidth) {
					a.Position = new Vector2 (0, a.Position.Y);
				}
				if (a.Position.Y - a.Height > ScreenHeight) {
					a.Position = new Vector2 (a.Position.X, 0);
				}
				
				if (a.Alive && CheckShipCollision (a)) {
					GamePad.SetVibration(PlayerIndex.One,1f,1f);
					a.Kill ();
					lives--;
					SetupShip ();
					if (lives < 1)
						GameOver = true;
				}
			}
			
		}

		private bool CheckShipCollision (Sprite asteroid)
		{
			Vector2 position1 = asteroid.Position;
			Vector2 position2 = ship.Position;
			
			float Cathetus1 = Math.Abs (position1.X - position2.X);
			float Cathetus2 = Math.Abs (position1.Y - position2.Y);
			
			Cathetus1 *= Cathetus1;
			Cathetus2 *= Cathetus2;
			
			distance = (float)Math.Sqrt (Cathetus1 + Cathetus2);
			
			if ((int)distance < ship.Width)
				return true;
			
			return false;
		}

		private bool CheckAsteroidCollision (Sprite asteroid, Sprite bullet)
		{
			Vector2 position1 = asteroid.Position;
			Vector2 position2 = bullet.Position;
			
			float Cathetus1 = Math.Abs (position1.X - position2.X);
			float Cathetus2 = Math.Abs (position1.Y - position2.Y);
			
			Cathetus1 *= Cathetus1;
			Cathetus2 *= Cathetus2;
			
			distance = (float)Math.Sqrt (Cathetus1 + Cathetus2);
			
			return (int)distance < asteroid.Width;
		}
		private float bulletMaxDistance = -1;
		public float BulletMaxDistance
		{
			get {
				if(bulletMaxDistance == -1)
					bulletMaxDistance = Math.Max(UIApplication.SharedApplication.KeyWindow.Bounds.Width,UIApplication.SharedApplication.KeyWindow.Bounds.Height) /2;
				return bulletMaxDistance;
			}	
		}
		private void UpdateBullets ()
		{
			List<Sprite> destroyed = new List<Sprite> ();
			
			foreach (Sprite b in bullets) {
								
				b.Position += b.Velocity;
				b.DistanceTraveled += b.Velocity;
				
				if (b.Position.X + b.Width < 0) {
					b.Position = new Vector2 (ScreenWidth, b.Position.Y);
				}
				if (b.Position.Y + b.Height < 0) {
					b.Position = new Vector2 (b.Position.X, ScreenHeight);
				}
				if (b.Position.X - b.Width > ScreenWidth) {
					b.Position = new Vector2 (0, b.Position.Y);
				}
				if (b.Position.Y - b.Height > ScreenHeight) {
					b.Position = new Vector2 (b.Position.X, 0);
				}
				
				foreach (Sprite a in asteroids) {
					if (a.Alive && CheckAsteroidCollision (a, b)) {
						if (a.Index == 1)
							score += 25; 
						else if (a.Index == 2)
							score += 50;
						else
							score += 100;
						
						a.Kill ();
						destroyed.Add (a);
						b.Kill ();
					}
				}
				
				if(Math.Abs((int)b.DistanceTraveled.X) > BulletMaxDistance|| Math.Abs((int)b.DistanceTraveled.Y) > BulletMaxDistance)
					b.Kill();
				/*
				if (b.Position.X < 0)
					b.Kill (); else if (b.Position.Y < 0)
					b.Kill (); else if (b.Position.X > ScreenWidth)
					b.Kill (); else if (b.Position.Y > ScreenHeight)
					b.Kill ();
					*/
			}
			
			for (int i = 0; i < bullets.Count; i++) {
				if (!bullets[i].Alive) {
					bullets.RemoveAt (i);
					i--;
				}
			}
			
			foreach (Sprite a in destroyed) {
				SplitAsteroid (a);
			}
		}
		
		private int asteroidSplitCount(int index)
		{
			int count = 2;
			return count;
		}		
		private void SplitAsteroid (Sprite a)
		{
			int splitCount = asteroidSplitCount(a.Index);
			if (a.Index == 1) {
				for (int i = 0; i < splitCount; i++) {
					NewAsteroid (a, 2);
				}
			} else if (a.Index == 2) {
				for (int i = 0; i < splitCount; i++) {
					NewAsteroid (a, 3);
				}
			}
		}

		private void NewAsteroid (Sprite a, int index)
		{
			
			int texIndex = random.Next (0,asteroidTextures.Count - 1 );
			Sprite tempSprite = new Sprite (gameType == GameType.Facebook ? a.Texture : asteroidTextures[texIndex]);
			float scale = index == 2 ? .75f : .42f;
			tempSprite.Scale *= scale;
			
			tempSprite.Index = index;
			tempSprite.Position = a.Position;
			tempSprite.Velocity = RandomVelocity ();
			
			tempSprite.Rotation = (float)random.NextDouble () * MathHelper.Pi * 4 - MathHelper.Pi * 2;
			tempSprite.RotationSpin = (float)random.NextDouble();
			Console.WriteLine(tempSprite.Rotation);
			tempSprite.Create ();
			asteroids.Add (tempSprite);
		}

		private Vector2 RandomVelocity ()
		{
			float xVelocity = (float)(random.NextDouble () * 2 + .5);
			float yVelocity = (float)(random.NextDouble () * 2 + .5);
			
			if (random.Next (2) == 1)
				xVelocity *= Util.IsIpad ? -1.0f : -.75f;
			
			if (random.Next (2) == 1)
				yVelocity *= Util.IsIpad ? -1.0f : -.75f;
			
			return new Vector2 (xVelocity, yVelocity);
		}

		private void FireBullet ()
		{
			Sprite newBullet = new Sprite (bullet.Texture);
			
			Vector2 velocity = new Vector2 ((float)Math.Cos (ship.Rotation - (float)MathHelper.PiOver2), (float)Math.Sin (ship.Rotation - (float)MathHelper.PiOver2));
			
			velocity.Normalize ();
			velocity *= 6.0f;
			
			newBullet.Velocity = velocity;
			
			newBullet.Position = ship.Position + newBullet.Velocity;
			newBullet.DistanceTraveled = Vector2.Zero;
			newBullet.Create ();
			
			bullets.Add (newBullet);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.Black);
			
			Matrix _view = Matrix.CreateScale (scale);
			if (GameOver) {
				
				spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
				
				
				Vector2 position2 = new Vector2 (0.0f, 0.0f);
				spriteBatch.Draw (banner, position2, Color.White);
				
				string text = "GAME OVER";
				
				Vector2 size = myFont.MeasureString (text);
				
				position2 = new Vector2 ((ScreenWidth / 2) - (size.X / 2), (ScreenHeight / 2) - (size.Y));
				spriteBatch.DrawString (myFont, text, position2, Color.White);
				//,rotation,positionCenter,1f, SpriteEffects.None,1.0f);
				text = "TAP TO START";
				size = myFont.MeasureString (text);
				
				position2 = new Vector2 ((ScreenWidth / 2) - (size.X / 2), (ScreenHeight / 2) + (size.Y * 2));
				
				spriteBatch.DrawString (myFont, text, position2, Color.White);
				
				spriteBatch.End ();
				
				return;
			}
			
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
			//spriteBatch.Begin ();
			Vector2 position = new Vector2 (10, 10);
			
			spriteBatch.DrawString (myFont, "Score = " + score.ToString (), position, Color.White);
			
			Rectangle shipRect;
			
			for (int i = 0; i < lives; i++) {
				shipRect = new Rectangle (i * ship.Width + 10, 40, ship.Width, ship.Height);
				
				spriteBatch.Draw (ship.StandardTexture, shipRect, Color.White);
			}
			
			spriteBatch.Draw (ship.Texture, ship.Position, null, Color.White, ship.Rotation, ship.Center, ship.Scale, SpriteEffects.None, 1.0f);
			
			foreach (Sprite b in bullets) {
				if (b.Alive) {
					spriteBatch.Draw (b.Texture, b.Position, null, Color.White, b.Rotation, b.Center, b.Scale, SpriteEffects.None, 1.0f);
				}
			}
			
			foreach (Sprite a in asteroids) {
				if (a.Alive) {
					
					//spriteBatch.Draw (astroidBacking, a.Position, null, Color.White, a.Rotation, a.Center, a.Scale, SpriteEffects.None, 1.0f);
					spriteBatch.Draw (a.Texture, a.Position, null, Color.White, a.Rotation, a.Center, a.Scale, SpriteEffects.None, 1.0f);
				}
			}
			
			if(!useAccel)
				GamePad.Draw (gameTime, spriteBatch);
			
			spriteBatch.End ();
			/*
			//GL.Viewport (0, 0, Size.Width, Size.Height);
				
			GL.MatrixMode (All.Projection);
			GL.LoadIdentity ();
			GL.Ortho (-1.0f, 1.0f, -1.5f, 1.5f, -1.0f, 1.0f);
			GL.MatrixMode (All.Modelview);
			GL.Rotate (3.0f, 1.0f, 1.0f, 1.0f);
			
			GL.Enable(All.Texture2D);
			
			float[] light = {0.3f, 0.3f, 0.3f, 1.0f};
			//GL.light
			
			//GL.ClearColor (0.5f, 0.5f, 0.5f, 1.0f);
			//GL.Clear ((uint)All.ColorBufferBit);
			//
			
			GL.EnableClientState (All.VertexArray);			
			GL.EnableClientState (All.NormalArray);
			//GL.Enable(All.ColorArray);
			GL.NormalPointer(All.Float,0,AwesomeAsteroid.Normals);
			GL.VertexPointer(3,All.Float,0,AwesomeAsteroid.Verticies);
			GL.TexCoordPointer(2,All.Float,0,AwesomeAsteroid.TextureCoords);
			//GL.TexParameter(All.Texture2D,All.TextureWrapS,(int)All.ClampToEdge);
			//GL.TexParameter(All.Texture2D,All.TextureWrapT,(int)All.ClampToEdge);
			
			//GL.ActiveTexture(All.Texture1);
			//Texture.Bind();
			GL.ClientActiveTexture(All.Texture0);
			GL.BindTexture(All.Texture2D,asteroid.ID);	
			//GL.ColorPointer(3,All.ColorArray,0,squareColors);
			GL.DrawArrays (All.TriangleStrip, 0,AwesomeAsteroid.Verticies.Length);
			*/
			
			base.Draw (gameTime);
		}
		
		
		const int space = 10;

		const int VertexCount = (90 / space) * (360 / space) * 4;

		ObjModel CreateSphere (double R, double H, double K, double Z)
		{
		
			List<OpenTK.Vector3> Vertecies = new List<OpenTK.Vector3>();
			List<OpenTK.Vector2> TextCoords = new List<OpenTK.Vector2>();
			int n;
			double a;
			double b;
			n = 0;
			for (b = 0; b <= 90 - space; b += space) {				
				for (a = 0; a <= 360 - space; a += space) {
					var v1 = new OpenTK.Vector3((float)(R * Math.Sin ((a) / 180 * Math.PI ) * Math.Sin ((b) / 180 * Math.PI) - H),
					                     (float)(R * Math.Cos ((a) / 180 * Math.PI ) * Math.Sin ((b) / 180 * Math.PI ) + K),
					                     (float)(R * Math.Cos ((b) / 180 * Math.PI ) - Z));	
					var t1 = new OpenTK.Vector2((float)((2 * b) / 360),(float)((a) / 360));			
					n++;			
					
					var v2 = new OpenTK.Vector3((float)(R * Math.Sin ((a) / 180 * Math.PI ) * Math.Sin ((b + space) / 180 * Math.PI ) - H),
					                     (float)(R * Math.Cos ((a) / 180 * Math.PI ) * Math.Sin ((b + space) / 180 * Math.PI ) + K),
					                     (float)(R * Math.Cos ((b + space) / 180 * Math.PI ) - Z));
					var t2 = new OpenTK.Vector2((float)((2 * (b + space)) / 360),
					                     (float)((a) / 360));
					n++;
					
					var v3 = new OpenTK.Vector3((float)(R * Math.Sin ((a + space) / 180 * Math.PI ) * Math.Sin ((b) / 180 * Math.PI ) - H),
					                     (float)(R * Math.Cos ((a + space) / 180 * Math.PI ) * Math.Sin ((b) / 180 * Math.PI ) + K),
					                     (float)(R * Math.Cos ((b) / 180 * Math.PI ) - Z));
					var t3 = new OpenTK.Vector2((float)((2 * b) / 360),
					                     (float)((a + space) / 360));
					n++;
					
					var v4 = new OpenTK.Vector3((float)( R * Math.Sin ((a + space) / 180 * Math.PI ) * Math.Sin ((b + space) / 180 * Math.PI ) - H),
					                     (float)(R * Math.Cos ((a + space) / 180 * Math.PI ) * Math.Sin ((b + space) / 180 * Math.PI ) + K),
					                     (float)(R * Math.Cos ((b + space) / 180 * Math.PI ) - Z));
					var t4 = new OpenTK.Vector2((float)((2 * (b + space)) / 360),
					                     (float)((a + space) / 360));
					n++;
					
					Vertecies.Add(v1);
					Vertecies.Add(v2);
					Vertecies.Add(v3);
					Vertecies.Add(v4);
					
					TextCoords.Add(t1);
					TextCoords.Add(t2);
					TextCoords.Add(t3);
					TextCoords.Add(t4);
					
				}
			}
			return new ObjModel(){Verticies = Vertecies.ToArray(),
								TextureCoords = TextCoords.ToArray()};
		}
	}
}
