using System;
using Microsoft.Xna.Framework.Graphics;
namespace AsteroidsHD
{
	public class AsteroidTexture
	{
		private GraphicsDevice GraphicsDevice;
		public AsteroidTexture (GraphicsDevice graphicsDevice, string file, Friend friend)
		{
			TextureFile = file;
			Friend = friend;
			IsFriend = friend != null;
			GraphicsDevice = graphicsDevice;
			//initialize(graphicsDevice);
		}
		public AsteroidTexture (GraphicsDevice graphicsDevice, string file) : this(graphicsDevice, file, null)
		{
			
		}

		private void Initialize ()
		{
			if (texture == null)
				texture = Texture2D.FromFile (GraphicsDevice, TextureFile, 60, 60);
		}

		public bool IsFriend { get; set; }
		public Friend Friend { get; set; }
		public string TextureFile { get; set; }
		private Texture2D texture;
		public Texture2D Texture {
			get {
				if (texture == null)
					Initialize ();
				return texture;
			}
		}
		private int useCount = 0;
		public void InUse()
		{
			useCount ++;
		}
		public void Done()
		{
			useCount --;
			if(useCount == 0)
			{
				Console.WriteLine("disposing texture");
				texture.Dispose();
			}
			texture = null;
		}
	}
}

