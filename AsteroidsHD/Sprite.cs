using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoTouch.UIKit;

namespace AsteroidsHD
{
    class Sprite
    {
        Texture2D texture;

        Vector2 position;
        Vector2 center;
        Vector2 velocity;

        float rotation;
        float scale;

        bool alive;

        int index;

        public Sprite(Texture2D texture)
        {
            this.texture = texture;
			
            Rotation = 0.0f;
			if(UIScreen.MainScreen.Bounds.Width > 500)
            	Scale = 1.0f;//UIScreen.MainScreen.Scale;
			else
				Scale = .6f;
            position = Vector2.Zero;
            center = new Vector2(Width / 2, Height / 2);
            velocity = Vector2.Zero;


            alive = false;

            index = 0;
        }

        public virtual Texture2D Texture
        {
            get { return texture; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Center
        {
            get { return center;}
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
		
		public Vector2 DistanceTraveled {get;set;}

        public float Rotation
        {
            get { return rotation; }
            set 
            { 
                rotation = value;
                if (rotation < -MathHelper.TwoPi)
                    rotation = MathHelper.TwoPi;
                if (rotation > MathHelper.TwoPi)
                    rotation = -MathHelper.TwoPi;
            }
        }
		public float RotationSpin{get;set;}

        public float Scale
        {
            get { return scale; }
            set { scale = value; 
			//center = new Vector2(Width / 2, Height / 2);
			}
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public int Width
        {
            get { return (int)(texture.Width * Scale); }
        }

        public int Height
        {
            get { return (int)(texture.Height * Scale); }
        }

        public void Create()
        {
            alive = true;
        }

        public void Kill()
        {
            alive = false;
        }
    }
}
