using System;
namespace AsteroidsHD
{
	class Asteroid : Sprite
	{
		public AsteroidTexture AsteroidTexture {get;set;}
		public Asteroid (AsteroidTexture texture) : base (texture.Texure)
		{
			AsteroidTexture = texture;
		}
	}
}

