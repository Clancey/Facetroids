using System;
using Microsoft.Xna.Framework.Graphics;
namespace AsteroidsHD
{
	class Ship : Sprite
	{
		Texture2D ThrustTexture;
		public Ship (Texture2D normalTexture, Texture2D thrustTexture) : base (normalTexture)
		{
			ThrustTexture = thrustTexture;
		}
		public bool IsThrusting;
		
		public override Texture2D Texture {
			get {
				return IsThrusting ? ThrustTexture: base.Texture;
			}
		}
		public Texture2D StandardTexture
		{
			get{return base.Texture;}	
		}
		public bool Invinsible
		{
			get;set;
		}
		
	}
}

