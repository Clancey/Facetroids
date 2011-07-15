using System;
namespace AsteroidsHD
{
	class Asteroid : Sprite
	{
		public AsteroidTexture AsteroidTexture {get;set;}
		public Asteroid (AsteroidTexture texture) : base (texture.Texture)
		{
			AsteroidTexture = texture;
		}
		public override void Create ()
		{
			base.Create ();
			AsteroidTexture.InUse();
		}
		public override Microsoft.Xna.Framework.Graphics.Texture2D Texture {
			get {
				return AsteroidTexture.Texture;
			}
		}
	}
}

