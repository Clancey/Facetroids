#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
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
#endregion

namespace AsteroidsHD
{

	public enum GameType
	{
		Facebook = 0,
		Retro = 1,
		Modern = 2
	}

	/// <summary>
	/// This screen implements the actual game logic. It is just a
	/// placeholder to get the idea across: you'll probably want to
	/// put some more interesting gameplay in here!
	/// </summary>
	class GameplayScreen : GameScreen
	{
		#region Fields
		ContentManager Content;
		Ship ship;
		//SpriteBatch spriteBatch;

		int ScreenWidth, ScreenHeight;
		int level = 1;
		int score = 0;
		int lives;
		//float scale;
		bool useAccel = true;
		GameType gameType = GameType.Facebook;
		bool soundEnabled = true;
		double invinisbleTimeLeft = 0;
		double shipResetSeconds = 0;
		double invinsibleResetTime = 3;

		int baseAsteroids = Util.IsIpad ? 5 : 3;

		KeyboardState oldState;

		Sprite bullet;
		List<Sprite> bullets = new List<Sprite> ();
		Texture2D gamePadTexture;
		List<Texture2D> asteroidTextures = new List<Texture2D> ();
		List<Sprite> asteroids = new List<Sprite> ();

		SoundEffect alienFired;
		SoundEffect alienDied;
		SoundEffect playerFired;
		SoundEffect playerDied;

		SpriteFont myFont;

		float distance = 0.0f;

		Random random = new Random ();

		bool GameOver = true;


		ParticleSystem particles;

		#endregion

		#region Initialization


		/// <summary>
		/// Constructor.
		/// </summary>
		public GameplayScreen ()
		{
			
			TransitionOnTime = TimeSpan.FromSeconds (1.5);
			TransitionOffTime = TimeSpan.FromSeconds (0.5);
			ShouldPause = true;
			
		}



		#endregion

		#region Update and Draw


		/// <summary>
		/// Updates the state of the game. This method checks the GameScreen.IsActive
		/// property, so the game will stop updating when the pause menu is active,
		/// or if you tab away to a different application.
		/// </summary>



		/// <summary>
		/// Draws the gameplay screen.
		/// </summary>

		#endregion
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>

		private void SetupGame ()
		{
			level = 1;
			score = 0;
			lives = 3;
			ScreenHeight = (int)UIScreen.MainScreen.Bounds.Width;
			ScreenWidth = (int)UIScreen.MainScreen.Bounds.Height;
			SetupShip ();
			CreateAsteroids ();
			GameOver = false;
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
		public override void LoadContent ()
		{
			ScreenWidth = ScreenManager.Width;
			ScreenHeight = ScreenManager.Height;
			gameType = Settings.GameType;
			if (Content == null)
				Content = new ContentManager (ScreenManager.Game.Services, "Content");
			// Create a new SpriteBatch, which can be used to draw textures.
			//spriteBatch = ScreenManager.SpriteBatch;
			//AwesomeAsteroid = ObjModelLoader.LoadFromFile ("Models/rock.obj");
			//asteroid = Texture2D.FromFile (ScreenManager.GraphicsDevice, "Models/rock.jpg", 256, 256);
			
			gamePadTexture = Content.Load<Texture2D> ("pause.png");
			var ship1t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship.pdf";
			var ship2t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship-thrust.pdf";
			var ship1 = Texture2D.FromFile (ScreenManager.GraphicsDevice, ship1t, 35, 35);
			var ship2 = Texture2D.FromFile (ScreenManager.GraphicsDevice, ship2t, 35, 35);
			ship = new Ship (ship1, ship2);
			//ship.Scale = .05f;
			bullet = new Sprite (Content.Load<Texture2D> ("shot-frame1"));
			//bullet.Scale = .05f;
			ReloadAsteroids ();
			
			myFont = Content.Load<SpriteFont> ("Fonts/SpriteFont1");
			
			alienFired = ScreenManager.Game.Content.Load<SoundEffect> ("fire");
			alienDied = ScreenManager.Game.Content.Load<SoundEffect> ("Alien_Hit");
			playerFired = ScreenManager.Game.Content.Load<SoundEffect> ("fire");
			playerDied = ScreenManager.Game.Content.Load<SoundEffect> ("Player_Hit");
			
			var gamePadH = 0 + 10;
			var gamePadLeft = ScreenWidth - 80;
			//Console.WriteLine (gamePadLeft);
			// Set the virtual GamePad
			ButtonDefinition BButton = new ButtonDefinition ();
			BButton.Texture = gamePadTexture;
			BButton.Position = new Vector2 (gamePadLeft, gamePadH);
			BButton.Type = Buttons.B;
			BButton.TextureRect = new Rectangle (0, 0, 30, 30);
			
			GamePad.ButtonsDefinitions.Add (BButton);
			/*
			ButtonDefinition AButton = new ButtonDefinition ();
			AButton.Texture = gamePadTexture;
			AButton.Position = new Vector2 (gamePadLeft - 75, gamePadH + 10);
			AButton.Type = Buttons.A;
			AButton.TextureRect = new Rectangle (73, 114, 36, 36);
			*/			
			
			//GamePad.ButtonsDefinitions.Add (AButton);
			
			//ThumbStickDefinition thumbStick = new ThumbStickDefinition ();
			//thumbStick.Position = new Vector2 (50, gamePadH);
			//thumbStick.Texture = gamePadTexture;
			//thumbStick.TextureRect = new Rectangle (2, 2, 68, 68);
			
			
			//GamePad.LeftThumbStickDefinition = thumbStick;
			
			SetupGame ();
			
			if (gameType != GameType.Retro)
				particles = new ParticleSystem (ScreenManager.Game.Content, ScreenManager.SpriteBatch);
			ScreenManager.Game.ResetElapsedTime ();
			
		}

		public void ReloadAsteroids ()
		{
			//Console.WriteLine(gameType);
			asteroidTextures.Clear ();
			if (gameType == GameType.Modern) {
				asteroidTextures.Add (Texture2D.FromFile (ScreenManager.GraphicsDevice, "Content/asteroid-front.pdf", 60, 60));
				return;
			} else if (gameType == GameType.Facebook) {
				foreach (var img in Facebook.GetImages ()) {
					//Console.WriteLine (img);
					asteroidTextures.Add (Texture2D.FromFile (ScreenManager.GraphicsDevice, img, 60, 60));
				}
				return;
			}
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (Texture2D.FromFile (ScreenManager.GraphicsDevice, "Content/Retro/large" + i + ".pdf", 60, 60));
			//60
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (Texture2D.FromFile (ScreenManager.GraphicsDevice, "Content/Retro/medium" + i + ".pdf", 60, 60));
			//45
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (Texture2D.FromFile (ScreenManager.GraphicsDevice, "Content/Retro/small" + i + ".pdf", 60, 60));
			//25
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		public override void UnloadContent ()
		{
			asteroidTextures.Clear ();
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
		public override void Update (GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			//TODO: Add pause
			//ScreenManager.AddScreen (new PauseMenuScreen (), ControllingPlayer);
			base.Update (gameTime, otherScreenHasFocus, coveredByOtherScreen);
			
			if (IsActive) {
				//ScreenHeight = ScreenManager.GraphicsDevice.DisplayMode.Height;
				//ScreenWidth = ScreenManager.GraphicsDevice.DisplayMode.Width;
				//Console.WriteLine(ScreenWidth);
				KeyboardState newState = Keyboard.GetState ();
				
				TouchCollection touchState = TouchPanel.GetState ();
				
				if (GameOver) {
					asteroids.Clear ();
					ship.Kill ();
					ContinueMenuScreen cont = new ContinueMenuScreen ();
					cont.Continue += delegate { SetupGame (); };
					ScreenManager.AddScreen (cont, ControllingPlayer);
					return;
					
				}
				
				
				if (useAccel)
					updateFromAccelerometer (gameTime, touchState);
				else
					updateFromGamePad (gameTime);
				
				
				//Console.WriteLine(invinisbleTimeLeft);
				if (invinisbleTimeLeft > 0) {
					invinisbleTimeLeft = invinsibleResetTime - (gameTime.TotalRealTime.TotalSeconds - shipResetSeconds);
					//Console.WriteLine ("invinisbleTimeLeft: " + invinisbleTimeLeft);
				}
				UpdateShip ();
				UpdateAsteroids (gameTime);
				UpdateBullets ();
				AllDead ();
				
			}
			
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			if (gameType != GameType.Retro)
				particles.Update (elapsed);
		}
		bool leftPressed;
		private void updateFromAccelerometer (GameTime gameTime, TouchCollection touchState)
		{
			//PAUSE
			
			
			GamePadState gamepastatus = GamePad.GetState (PlayerIndex.One);
			if (gamepastatus.Buttons.B == ButtonState.Pressed) {
				ScreenManager.GlobalPause ();
				return;
			}
			
			//gameTime.
			var accelState = Accelerometer.GetState ().Acceleration;
			
			var position = new Vector2 (0, 0);
			//if(Math.Abs(accelState.Y) > .1f)
			position.X = accelState.Y * 4 * .5f;
			
			if (this.ScreenManager.GraphicsDevice.PresentationParameters.DisplayOrientation == DisplayOrientation.LandscapeRight)
				position.X *= -1;
			
			ship.Rotation -= 0.05f * position.X;
			
			var halfScreen = ScreenWidth / 2;
			bool leftSideTouched = false;
			bool rightSideTouched = false;
			foreach (var touch in touchState) {
				if (touch.State != TouchLocationState.Released || touch.State != TouchLocationState.Invalid) {
					if (touch.Position.X / ScreenManager.Scale < halfScreen)
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

		private void updateFromGamePad (GameTime gameTime)
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
				level++;
				asteroids.Clear ();
				CreateAsteroids ();
				bullets.Clear ();
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
			var asteroidCount = baseAsteroids + level;
			//asteroidCount = 1;
			for (int i = 0; i < asteroidCount; i++) {
				int index = random.Next (0, asteroidTextures.Count - 1);
				
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
				asteroids[i].RotationSpin = (float)random.NextDouble () * random.Next (-1, 1);
				asteroids[i].Create ();
			}
		}

		private void UpdateAsteroids (GameTime gameTime)
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
					GamePad.SetVibration (PlayerIndex.One, 1f, 1f);
					if (soundEnabled)
						playerDied.Play ();
					if (gameType != GameType.Retro)
						particles.CreatePlayerExplosion (new Vector2 (a.Position.X + a.Width / 2, a.Position.Y + a.Height / 2));
					a.Kill ();
					lives--;
					invinisbleTimeLeft = invinsibleResetTime;
					shipResetSeconds = gameTime.TotalRealTime.TotalSeconds;
					//Console.WriteLine ("shipResetSeconds: " + shipResetSeconds);
					SetupShip ();
					if (lives < 1) {
						GameOver = true;
					}
				}
			}
			
		}

		private bool CheckShipCollision (Sprite asteroid)
		{
			if (invinisbleTimeLeft > 0)
				return false;
			Vector2 position1 = asteroid.Position;
			Vector2 position2 = ship.Position;
			
			float Cathetus1 = Math.Abs (position1.X - position2.X);
			float Cathetus2 = Math.Abs (position1.Y - position2.Y);
			
			Cathetus1 *= Cathetus1;
			Cathetus2 *= Cathetus2;
			
			distance = (float)Math.Sqrt (Cathetus1 + Cathetus2);
			
			//Console.WriteLine("Asteroid:" + asteroid.Center + "," + asteroid.Width + "," + asteroid.Height);
			//Console.WriteLine("Ship:" + position2 + "," + ship.Width + "," + ship.Height);
			//Console.WriteLine("Distance:" + distance);
			var width = ship.Width;
			if (asteroid.Width > ship.Width)
				width += (asteroid.Width - ship.Width) / 2;
			if ((int)distance < width)
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
			
			//if(asteroid == asteroids[0])
			//	Console.WriteLine(distance + " : " + asteroid.Width);
			if ((int)distance < asteroid.Width / 2) {
				//Console.WriteLine(distance + "," + asteroid.Width + " : " + asteroid.Position + "," + bullet.Position);
				return true;
			} else
				return false;
		}
		private float bulletMaxDistance = -1;
		public float BulletMaxDistance {
			get {
				if (bulletMaxDistance == -1)
					bulletMaxDistance = Math.Max (UIApplication.SharedApplication.KeyWindow.Bounds.Width, UIApplication.SharedApplication.KeyWindow.Bounds.Height) / 2;
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
							score += 25; else if (a.Index == 2)
							score += 50;
						else
							score += 100;
						
						a.Kill ();
						destroyed.Add (a);
						b.Kill ();
						if (soundEnabled)
							alienDied.Play ();
						
						if (gameType != GameType.Retro)
							particles.CreateAlienExplosion (new Vector2 (a.Position.X + a.Width / 2, a.Position.Y + a.Height / 2));
					}
				}
				
				if (Math.Abs ((int)b.DistanceTraveled.X) > BulletMaxDistance || Math.Abs ((int)b.DistanceTraveled.Y) > BulletMaxDistance)
					b.Kill ();
			}
			/*
				if (b.Position.X < 0)
					b.Kill (); else if (b.Position.Y < 0)
					b.Kill (); else if (b.Position.X > ScreenWidth)
					b.Kill (); else if (b.Position.Y > ScreenHeight)
					b.Kill ();
					*/			
			
			
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

		private int asteroidSplitCount (int index)
		{
			int count = 2;
			return count;
		}
		private void SplitAsteroid (Sprite a)
		{
			int splitCount = asteroidSplitCount (a.Index);
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
			
			int texIndex = random.Next (0, asteroidTextures.Count - 1);
			Sprite tempSprite = new Sprite (gameType == GameType.Facebook ? a.Texture : asteroidTextures[texIndex]);
			float scale = index == 2 ? .75f : .42f;
			tempSprite.Scale *= scale;
			//Console.WriteLine(scale);
			tempSprite.Index = index;
			tempSprite.Position = a.Position;
			tempSprite.Velocity = RandomVelocity ();
			
			tempSprite.Rotation = (float)random.NextDouble () * MathHelper.Pi * 4 - MathHelper.Pi * 2;
			tempSprite.RotationSpin = (float)random.NextDouble ();
			//Console.WriteLine (tempSprite.Rotation);
			tempSprite.Create ();
			asteroids.Add (tempSprite);
		}

		private Vector2 RandomVelocity ()
		{
			float xVelocity = (float)(random.NextDouble () * 2 + .5);
			float yVelocity = (float)(random.NextDouble () * 2 + .5);
			
			if (random.Next (2) == 1)
				xVelocity *= Util.IsIpad ? -1.0f : -.5f;
			
			if (random.Next (2) == 1)
				yVelocity *= Util.IsIpad ? -1.0f : -.5f;
			
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
			if (soundEnabled)
				playerFired.Play ();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw (GameTime gameTime)
		{
			
			ScreenManager.GraphicsDevice.Clear (Color.Black);
			
			//ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
			//                                   Color.CornflowerBlue, 0, 0);
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			
			
			//spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, _view);
			
			spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, _view);
			//spriteBatch.Begin();
			Vector2 position = new Vector2 (10, 10);
			
			spriteBatch.DrawString (myFont, "Score = " + score.ToString (), position, Color.White);
			
			Rectangle shipRect;
			
			for (int i = 0; i < lives; i++) {
				shipRect = new Rectangle (i * ship.Width + 10, 40, ship.Width, ship.Height);
				
				spriteBatch.Draw (ship.StandardTexture, shipRect, Color.White);
			}
			//if invinsible show every other second
			if (invinisbleTimeLeft <= 0 || (int)(invinisbleTimeLeft * 10) % 2 == 1)
				spriteBatch.Draw (ship.Texture, ship.Position, null, Color.White, ship.Rotation, ship.Center, ship.Scale, SpriteEffects.None, 1.0f);
			
			foreach (Sprite b in bullets) {
				if (b.Alive) {
					spriteBatch.Draw (b.Texture, b.Position, null, Color.White, b.Rotation, b.Center, b.Scale, SpriteEffects.None, 1.0f);
				}
			}
			
			foreach (Sprite a in asteroids) {
				if (a.Alive) {
					
					//spriteBatch.Draw (astroidBacking, a.Position, null, Color.White, a.Rotation, a.Center, a.Scale, SpriteEffects.None, 1.0f);
					//Console.WriteLine("s:" +a.Scale + " p: " + a.Position + " :c" + a.Center);
					spriteBatch.Draw (a.Texture, a.Position, null, Color.White, a.Rotation, a.Center, a.Scale, SpriteEffects.None, 1.0f);
				}
			}
			
			//if (!useAccel)
			GamePad.Draw (gameTime, spriteBatch);
			
			if (gameType != GameType.Retro)
				particles.Draw ();
			spriteBatch.End ();
			
			if (TransitionPosition > 0)
				ScreenManager.FadeBackBufferToBlack (255 - TransitionAlpha);
		}
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
			
			//base.Draw (gameTime);
		
		
			}
}
