using System;
using Microsoft.Xna.Framework;
using MonoTouch.UIKit;
namespace AsteroidsHD
{
    /// <summary>
    /// Sample showing how to manage different game states, with transitions
    /// between menu screens, a loading screen, the game itself, and a pause
    /// menu. This main game class is extremely simple: all the interesting
    /// stuff happens in the ScreenManager component.
    /// </summary>
    public class MainGame : Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        public ScreenManager ScreenManager;


        int BufferWidth = 320;
        int BufferHeight = 480;

        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public MainGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = BufferWidth;
            graphics.PreferredBackBufferHeight = BufferHeight;
			graphics.PreferMultiSampling = true;
			
			graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

        }
		
		protected override void Initialize ()
		{
			base.Initialize ();
			            // Create the screen manager component.
            ScreenManager = new ScreenManager(this);

            Components.Add(ScreenManager);

            // Activate the first screens.
			Util.BackgroundScreen.AutoMove = true;
            ScreenManager.AddScreen(Util.BackgroundScreen, null);
            ScreenManager.AddScreen(new MainMenuScreen(), null);
		}


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// 
        
		protected override void Draw(GameTime gameTime)
        {
			//FaceDetection.DetectFaces();
            //graphics.GraphicsDevice.Clear(Color.Black);
			//Console.WriteLine("drawing");
            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }
		 

        #endregion
    }
}
