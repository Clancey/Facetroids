using System;
using Microsoft.Xna.Framework.Graphics;
namespace AsteroidsHD
{
	public class AsteroidTexture
	{
		public AsteroidTexture (GraphicsDevice graphicsDevice,string file,Friend friend)
		{
			TextureFile = file;
			Friend = friend;
			IsFriend = friend != null;
			initialize(graphicsDevice);
		}
		public AsteroidTexture(GraphicsDevice graphicsDevice,string file) : this(graphicsDevice,file,null)
		{
			
		}
		
		private void initialize(GraphicsDevice graphicsDevice)
		{
			Texure = Texture2D.FromFile (graphicsDevice, TextureFile, 60, 60);
		}
		
		public bool IsFriend {get;set;}
		public Friend Friend {get;set;}
		public string TextureFile {get;set;} 
		public Texture2D Texure {get;set;}
	}
}

