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
		
		bool hasAsteroids = true;
		#region Fields
		ContentManager Content;
		Ship ship;
		//SpriteBatch spriteBatch;

		int ScreenWidth, ScreenHeight;
		int level = 1;
		long score = 0;
		int lives;
		//float scale;
		bool useAccel;
		GameType gameType = GameType.Facebook;
		double invinisbleTimeLeft = 0;
		double shipResetSeconds = 0;
		double invinsibleResetTime = 3;
		long LocalHighScore = 0 ;
		bool brokeHighScore = false;
		int baseAsteroids = Util.IsIpad ? 5 : 3;
		const int freeManPoints = 10000;
		int pointsForFreeMan = 0;
		
		bool isFreeManVisible = false;
		Sprite freeManSprite;
		double freeManResetTime = 10;
		double freeManBlinkTime = 4;
		double freeManVisibleTimeLeft = 0;
		double freeManResetSeconds = 0;

		KeyboardState oldState;

		Sprite bullet;
		List<Sprite> bullets = new List<Sprite> ();
		Texture2D gamePadTexture,leftButtonTexture,rightButtonTexture,upButtonTexture,pauseButtonTexture;
		List<AsteroidTexture> asteroidTextures = new List<AsteroidTexture> ();
		List<Asteroid> asteroids = new List<Asteroid> ();

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
			brokeHighScore = false;
			level = 1;
			score = 0;
			pointsForFreeMan = 0;
			lives = 3;
			bullets.Clear ();
			LocalHighScore = Settings.HighScore;
			ScreenHeight = (int)UIScreen.MainScreen.Bounds.Width;
			ScreenWidth = (int)UIScreen.MainScreen.Bounds.Height;
			SetupShip ();
			CreateAsteroids ();
			invinisbleTimeLeft = -1;
			GameOver = false;
			isFreeManVisible = false;
			if(particles != null)
				particles.Reset();
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
			useAccel = Settings.UseAccel;
			if (Content == null)
				Content = new ContentManager (ScreenManager.Game.Services, "Content");
			// Create a new SpriteBatch, which can be used to draw textures.
			//spriteBatch = ScreenManager.SpriteBatch;
			//AwesomeAsteroid = ObjModelLoader.LoadFromFile ("Models/rock.obj");
			//asteroid = Texture2D.FromFile (ScreenManager.GraphicsDevice, "Models/rock.jpg", 256, 256);
			pauseButtonTexture = Content.Load<Texture2D>("pause.png");
			var ship1t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship.pdf";
			var ship2t = gameType == GameType.Retro ? "Content/Retro/ship.pdf" : "Content/ship-thrust.pdf";
			var ship1 = Texture2D.FromFile (ScreenManager.GraphicsDevice, ship1t, 35, 35);
			var ship2 = Texture2D.FromFile (ScreenManager.GraphicsDevice, ship2t, 35, 35);
			ship = new Ship (ship1, ship2);
			freeManSprite = new Sprite(ship2);
			//ship.Scale = .05f;
			bullet = new Sprite (Content.Load<Texture2D> ("shot-frame1"));
			//bullet.Scale = .05f;
			ReloadAsteroids ();
			
			myFont = Content.Load<SpriteFont> ("Fonts/SpriteFont1");
			
			alienFired = ScreenManager.Game.Content.Load<SoundEffect> ("fire");
			alienDied = ScreenManager.Game.Content.Load<SoundEffect> ("Alien_Hit");
			playerFired = ScreenManager.Game.Content.Load<SoundEffect> ("fire");
			playerDied = ScreenManager.Game.Content.Load<SoundEffect> ("Player_Hit");
			
			//Console.WriteLine (gamePadLeft);
			// Set the virtual GamePad
			GamePad.ButtonsDefinitions.Clear();
			var gamePadH = ScreenHeight - 100;
			var gamePadLeft = ScreenWidth -80;
			
			ButtonDefinition XButton = new ButtonDefinition ();
			XButton.Texture = pauseButtonTexture;
			XButton.Position = new Vector2 (gamePadLeft, 0 + 10 + (int)myFont.FontSize + 10);
			XButton.Type = Buttons.X;
			XButton.TextureRect = new Rectangle (0, 0, 30, 30);	
			GamePad.ButtonsDefinitions.Add (XButton);
			
			
			if(!useAccel)
			{
				//Setup the controller if they are not using the accelerometer
				gamePadTexture = Content.Load<Texture2D> ("gamepad.png");
				//leftButtonTexture = Content.Load<Texture2D>("leftArrow.png");
				//rightButtonTexture = Content.Load<Texture2D>("rightArrow.png");
				//upButtonTexture = Content.Load<Texture2D>("upArrow.png");
				//ButtonDefinition left = new ButtonDefinition();
				//Console.WriteLine (gamePadH);
				// Set the virtual GamePad
				/*
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
				
				ButtonDefinition LeftButton = new ButtonDefinition();
				LeftButton.Texture = leftButtonTexture;
				LeftButton.Position = new Vector2(50,gamePadH);
				LeftButton.Type = Buttons.DPadLeft;
				LeftButton.TextureRect = new Rectangle(0,0,40,40);
				
				ButtonDefinition RightButton = new ButtonDefinition();
				RightButton.Texture = rightButtonTexture;
				RightButton.Position = new Vector2(50 + 40 +4,gamePadH);
				RightButton.Type = Buttons.DPadRight;
				RightButton.TextureRect = new Rectangle(0,0,40,40);
				
				
				ButtonDefinition UpButton = new ButtonDefinition();
				UpButton.Texture = upButtonTexture;
				UpButton.Position = new Vector2(50 + 22,gamePadH - 44);
				UpButton.Type = Buttons.DPadUp;
				UpButton.TextureRect = new Rectangle(0,0,40,40);
				
				GamePad.ButtonsDefinitions.Add(LeftButton);
				GamePad.ButtonsDefinitions.Add(RightButton);
				GamePad.ButtonsDefinitions.Add(UpButton);
				GamePad.ButtonsDefinitions.Add (BButton);
				GamePad.ButtonsDefinitions.Add (AButton);	
				*/
				
			
				ThumbStickDefinition thumbStick = new ThumbStickDefinition ();
				thumbStick.Position = new Vector2 (50, gamePadH);
				thumbStick.Texture = gamePadTexture;
				thumbStick.TextureRect = new Rectangle (2, 2, 68, 68);
				
				
				GamePad.LeftThumbStickDefinition = thumbStick;
				
			}
			else
			{
				GamePad.LeftThumbStickDefinition = null;	
			}
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
				asteroidTextures.Add (new AsteroidTexture(ScreenManager.GraphicsDevice, "Content/asteroid.png"));
				return;
			} else if (gameType == GameType.Facebook) {
				foreach (var friendResult in Facebook.GetImages ()) {
					//Console.WriteLine (img);
					asteroidTextures.Add (new AsteroidTexture(ScreenManager.GraphicsDevice,friendResult.FileName,friendResult.Friend));
				}
				return;
			}
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (new AsteroidTexture(ScreenManager.GraphicsDevice, "Content/Retro/large" + i + ".pdf"));
			//60
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (new AsteroidTexture(ScreenManager.GraphicsDevice, "Content/Retro/medium" + i + ".pdf"));
			//45
			for (int i = 1; i < 4; i++)
				asteroidTextures.Add (new AsteroidTexture(ScreenManager.GraphicsDevice, "Content/Retro/small" + i + ".pdf"));
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
					lock(Database.Main)
					{
						Database.Main.Insert(new score((int)score,level,gameType,DateTime.Now));
						foreach(var asteroid in asteroidTextures.Where(a=> a.IsFriend).ToList())
							Database.Main.Update(asteroid.Friend);
							
					}
					string scoreString = "";
					if(brokeHighScore)
					{
						Settings.HighScore = (int)score;
						scoreString = "New High Score: " + score;
					}
					else
						scoreString = "Score: " + score;
					
					ContinueMenuScreen cont = new ContinueMenuScreen (scoreString);					
					cont.Continue += delegate { SetupGame (); };
					ScreenManager.AddScreen (cont, ControllingPlayer);
					Util.SubmitScores();
					return;
					
				}
							
				GamePadState gamepastatus = GamePad.GetState (PlayerIndex.One);
				if (gamepastatus.Buttons.X == ButtonState.Pressed) {
					ScreenManager.GlobalPause ();
					return;
				}
				else
				{
					if (useAccel)
						updateFromAccelerometer (gameTime, touchState);
					else
						updateFromGamePad (gameTime,touchState);
				}
				//Console.WriteLine(invinisbleTimeLeft);
				if (invinisbleTimeLeft > 0) {
					invinisbleTimeLeft = invinsibleResetTime - (gameTime.TotalRealTime.TotalSeconds - shipResetSeconds);
					//Console.WriteLine ("invinisbleTimeLeft: " + invinisbleTimeLeft);
				}
				
				if(freeManVisibleTimeLeft > 0)
					freeManVisibleTimeLeft = freeManResetTime - (gameTime.TotalRealTime.TotalSeconds - freeManResetSeconds);
				else 
					isFreeManVisible = false;
				
				UpdateShip ();
				UpdateAsteroids (gameTime);
				UpdateBullets (gameTime);
				AllDead ();
				
			}
			
			float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
			
			if (gameType != GameType.Retro)
				particles.Update (elapsed);
		}
		bool leftPressed;
		double lastAutoShotFired;
		const double autoShootTime = 333;
		private void updateFromAccelerometer (GameTime gameTime, TouchCollection touchState)
		{
			//PAUSE
			

			
			//gameTime.
			var accelState = Accelerometer.GetState ().Acceleration;
			
			var position = new Vector2 (0, 0);
			//if(Math.Abs(accelState.Y) > .1f)
			position.X = accelState.Y * 4 * Settings.Sensativity;
			
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
			var diff = gameTime.TotalGameTime.TotalMilliseconds - lastAutoShotFired ;
			//Console.WriteLine(diff);
			if (!RightSideTouchedOld && rightSideTouched)
			{
				FireBullet ();
				lastAutoShotFired = gameTime.TotalGameTime.TotalMilliseconds;
			}
			else if(rightSideTouched && diff >= autoShootTime)
			{
				FireBullet();
				lastAutoShotFired = gameTime.TotalGameTime.TotalMilliseconds;
			}
			
			if (leftSideTouched)
				AccelerateShip ();
			else
				DecelerateShip ();
			
			LeftSideTouchedOld = leftSideTouched;
			RightSideTouchedOld = rightSideTouched;
			
			
		}
		
		private static float WrapAngle(float radians) 
		{ 
		    while (radians < -MathHelper.Pi) 
		    { 
		        radians += MathHelper.TwoPi; 
		    } 
		    while (radians > MathHelper.Pi) 
		    { 
		        radians -= MathHelper.TwoPi; 
		    } 
		    return radians; 
		} 
		private void updateFromGamePad (GameTime gameTime, TouchCollection touchState)
		{
			float tollerance = .95f;
			GamePadState gamepastatus = GamePad.GetState (PlayerIndex.One);
			var position = new Vector2 (0, 0);
			//if(gamepastatus.ThumbSticks.Left.Y > tollerance)
			
			//if (gamepastatus.ThumbSticks.Left.Y < tollerance)
			position.X += (int)(gamepastatus.ThumbSticks.Left.X  * 4 * Settings.Sensativity);	
			
			//ship.Rotation -= 0.05f * position.X;
			var maxJoyStick = Math.Max(Math.Abs(gamepastatus.ThumbSticks.Left.Y),Math.Abs(gamepastatus.ThumbSticks.Left.X));
			var rotation = (float)Math.Atan2((double)gamepastatus.ThumbSticks.Left.X,(double)gamepastatus.ThumbSticks.Left.Y);
			if(maxJoyStick > 0.1f )
			{
				
				var rotationSpeed = 0.2f * Settings.Sensativity;
				float difference = WrapAngle(rotation - ship.Rotation); 
 
				// clamp that between -turnSpeed and turnSpeed. 
				difference = MathHelper.Clamp(difference, -rotationSpeed, rotationSpeed); 
				
				// so, the closest we can get to our target is currentAngle + difference. 
				// return that, using WrapAngle again. 
				ship.Rotation = WrapAngle(ship.Rotation + difference); 

			}
			
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
			
			var diff = gameTime.TotalGameTime.TotalMilliseconds - lastAutoShotFired ;
			//Console.WriteLine(diff);
			if (!RightSideTouchedOld && rightSideTouched)
			{
				FireBullet ();
				lastAutoShotFired = gameTime.TotalGameTime.TotalMilliseconds;
			}
			else if(rightSideTouched && diff >= autoShootTime)
			{
				FireBullet();
				lastAutoShotFired = gameTime.TotalGameTime.TotalMilliseconds;
			}
			
			
			//if (gamepastatus.Buttons.B == ButtonState.Pressed && oldGamePadState.Buttons.B == ButtonState.Released)
			//	FireBullet ();
			
			if (maxJoyStick > tollerance)
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
			RightSideTouchedOld = rightSideTouched;
		}

		private void AllDead ()
		{
			bool allDead = true;
			foreach (Sprite s in asteroids) {
				if (s.Alive)
					allDead = false;
			}
			
			if(!hasAsteroids)
				allDead = false;
			if (allDead) {
				level++;
				Util.BackgroundScreen.ChangeColor();
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
		const float Friction = 0.09f;
		const float MaxSpeed = 3f;
		private void AccelerateShip ()
		{
			ship.IsThrusting = true;
			
			ship.Velocity += new Vector2 ((float)(Math.Cos (ship.Rotation - MathHelper.PiOver2) * Friction), (float)((Math.Sin (ship.Rotation - MathHelper.PiOver2) * Friction)));
			
			if (ship.Velocity.X > MaxSpeed) {
				ship.Velocity = new Vector2 (MaxSpeed, ship.Velocity.Y);
			}
			if (ship.Velocity.X < -MaxSpeed) {
				ship.Velocity = new Vector2 (-MaxSpeed, ship.Velocity.Y);
			}
			if (ship.Velocity.Y > MaxSpeed) {
				ship.Velocity = new Vector2 (ship.Velocity.X, MaxSpeed);
			}
			if (ship.Velocity.Y < -MaxSpeed) {
				ship.Velocity = new Vector2 (ship.Velocity.X, -MaxSpeed);
			}
		}
		
		const float dec = 0.03f;
		private void DecelerateShip ()
		{
			ship.IsThrusting = false;
			if (ship.Velocity.X < 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X + dec, ship.Velocity.Y);
			}
			
			if (ship.Velocity.X > 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X - dec, ship.Velocity.Y);
			}
			
			if (ship.Velocity.Y < 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X, ship.Velocity.Y + dec);
			}
			
			if (ship.Velocity.Y > 0) {
				ship.Velocity = new Vector2 (ship.Velocity.X, ship.Velocity.Y - dec);
			}
		}

		public void UpdateShip ()
		{
			Util.BackgroundScreen.Velocity = ship.Velocity;
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
			if(isFreeManVisible)
			{
				if(CheckShipCollision(freeManSprite))
				{
					isFreeManVisible = false;
					lives ++;
				}
			}
		}

		private void CreateAsteroids ()
		{
			if(!hasAsteroids)
				return;
			int value;
			var asteroidCount = baseAsteroids + level;
			//asteroidCount = 1;
			for (int i = 0; i < asteroidCount; i++) {
				int index = random.Next (0, asteroidTextures.Count - 1);
				
				Asteroid tempAsteroid = new Asteroid (asteroidTextures[index]);
				asteroids.Add (tempAsteroid);
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
			foreach (Asteroid a in asteroids) {
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
					if (a.Index == 1)
							UpdateScore(20);
						else if (a.Index == 2)
							UpdateScore(50);
						else
							UpdateScore(100);
					GamePad.SetVibration (PlayerIndex.One, 1f, 1f);
					if (Settings.UseSound)
						playerDied.Play ();
					if (gameType != GameType.Retro)
						particles.CreatePlayerExplosion (new Vector2 (a.Position.X + a.Width / 2, a.Position.Y + a.Height / 2));
					a.Kill ();
					if(a.AsteroidTexture.IsFriend)
						a.AsteroidTexture.Friend.KilledByCount ++;
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

		private bool CheckShipCollision (Sprite sprite)
		{
			if (invinisbleTimeLeft > 0 && sprite is Asteroid)
				return false;
			Vector2 position1 = sprite.Position;
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
			if (sprite.Width > ship.Width)
				width += (sprite.Width - ship.Width) / 2;
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
		
		private void UpdateScore(int inScore)
		{
			score += inScore; 
			if(score > LocalHighScore)
			{
				brokeHighScore = true; 
				LocalHighScore = score;
			}
			pointsForFreeMan += inScore;
			if(pointsForFreeMan >= freeManPoints)
			{
				lives ++;
				pointsForFreeMan = pointsForFreeMan - freeManPoints;
			}
		}
		
		private float bulletMaxDistance = -1;
		public float BulletMaxDistance {
			get {
				if (bulletMaxDistance == -1)
					bulletMaxDistance = Math.Max (UIApplication.SharedApplication.KeyWindow.Bounds.Width, UIApplication.SharedApplication.KeyWindow.Bounds.Height) / 2;
				return bulletMaxDistance;
			}
		}
		private void UpdateBullets (GameTime gameTime)
		{
			List<Asteroid> destroyed = new List<Asteroid> ();
			
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
				
				foreach (Asteroid a in asteroids) {
					if (a.Alive && CheckAsteroidCollision (a, b)) {
						if (a.Index == 1)
							UpdateScore(20);
						else if (a.Index == 2)
							UpdateScore(50);
						else
							UpdateScore(100);
						
						a.Kill ();
						if(a.AsteroidTexture.IsFriend)
						{
							a.AsteroidTexture.Friend.HitCount ++;
						}
						destroyed.Add (a);
						b.Kill ();
						if (Settings.UseSound)
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
			
			foreach (Asteroid a in destroyed) {
				SplitAsteroid (a,gameTime);
			}
		}

		private int asteroidSplitCount (int index)
		{
			int count = 2;
			return count;
		}
		private void SplitAsteroid (Asteroid a,GameTime gameTime)
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
			spawnDrops(a.Position,gameTime);
		}
		Random spawnRandom = new Random();
		private void spawnDrops(Vector2 position,GameTime gameTime)
		{
			var number = spawnRandom.Next(100);
			if( number > 95)
			{
				if(isFreeManVisible)
					return;
				isFreeManVisible = true;
				freeManSprite.Position = position;
				freeManVisibleTimeLeft = freeManResetTime;
				freeManResetSeconds = gameTime.TotalRealTime.TotalSeconds;
			}
		}

		private void NewAsteroid (Asteroid a, int index)
		{
			
			int texIndex = random.Next (0, asteroidTextures.Count - 1);
			Asteroid tempAsteroid = new Asteroid (gameType == GameType.Facebook ? a.AsteroidTexture : asteroidTextures[texIndex]);
			float scale = index == 2 ? .75f : .42f;
			tempAsteroid.Scale *= scale;
			//Console.WriteLine(scale);
			tempAsteroid.Index = index;
			tempAsteroid.Position = a.Position;
			tempAsteroid.Velocity = RandomVelocity ();
			
			tempAsteroid.Rotation = (float)random.NextDouble () * MathHelper.Pi * 4 - MathHelper.Pi * 2;
			tempAsteroid.RotationSpin = (float)random.NextDouble ();
			//Console.WriteLine (tempSprite.Rotation);
			tempAsteroid.Create ();
			asteroids.Add (tempAsteroid);
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
			velocity += ship.Velocity;
			
			newBullet.Velocity = velocity;
			
			newBullet.Position = ship.Position + newBullet.Velocity;
			newBullet.DistanceTraveled = Vector2.Zero;
			newBullet.Create ();
			
			bullets.Add (newBullet);
			if (Settings.UseSound)
				playerFired.Play ();
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw (GameTime gameTime)
		{
			
			//ScreenManager.GraphicsDevice.Clear (Color.Black);
			
			//ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
			//                                   Color.CornflowerBlue, 0, 0);
			SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
			Matrix _view = Matrix.CreateScale (ScreenManager.Scale);
			
			
			//spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, _view);
			
			spriteBatch.Begin (SpriteSortMode.Deferred, null, null, null, null, null, _view);
			//spriteBatch.Begin();
			Vector2 position = new Vector2 (10, 10);
			
			spriteBatch.DrawString (myFont, "Score = " + score.ToString (), position, Color.White);
			
			var highScoreText = "High Score = " + LocalHighScore;
			var hSize = myFont.MeasureString(highScoreText);
			Vector2 highScorePos = new Vector2(ScreenWidth - 10 - hSize.X,10);			
			spriteBatch.DrawString (myFont, highScoreText, highScorePos, Color.White);
			
			Rectangle shipRect;
			/*
			for (int i = 0; i < lives; i++) {
				shipRect = new Rectangle (i * ship.Width + 10, 40, ship.Width, ship.Height);
				
				spriteBatch.Draw (ship.StandardTexture, shipRect, Color.White);
			}
			*/
			shipRect = new Rectangle (10, 40, ship.Width, ship.Height);
			
			spriteBatch.Draw (ship.StandardTexture, shipRect, Color.White);
			
			string livesString =  " x " + lives;
			spriteBatch.DrawString (myFont, livesString, new Vector2(ship.Width + shipRect.X,shipRect.Y) , Color.White);
			
			
			//if invinsible show every other second
			if (invinisbleTimeLeft <= 0 || (int)(invinisbleTimeLeft * 10) % 2 == 1)
				spriteBatch.Draw (ship.Texture, ship.Position, null, Color.White, ship.Rotation, ship.Center, ship.Scale, SpriteEffects.None, 1.0f);
			//Draw free man
			if(isFreeManVisible && (freeManVisibleTimeLeft > freeManBlinkTime || (int)(freeManVisibleTimeLeft * 10) % 2 == 1))
				spriteBatch.Draw(freeManSprite.Texture,freeManSprite.Position,null,Color.White,freeManSprite.Rotation,freeManSprite.Center,freeManSprite.Scale,SpriteEffects.None,1f);
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
			
			base.Draw(gameTime);
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
