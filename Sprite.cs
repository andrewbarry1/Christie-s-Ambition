using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TheGame
{

	// TODO subclass this for NPCs, etc
	public class Sprite
	{

		public Texture2D texture;
		public Vector2 position;

		public bool hasCollision;

		public AnimationHandler animHandler;

		public Rectangle boundingRectangle;

		public Sprite (string filename, ContentManager Content, Vector2 vec)
		{
			texture = Content.Load<Texture2D>("Sprites/" + filename);


			hasCollision = true;

			this.position = vec;
			this.animHandler = new AnimationHandler (texture.Height,texture.Width,new List<int>(),new List<int>());
			this.animHandler.setAnimation (0,true);
			boundingRectangle = new Rectangle ((int)vec.X, (int)vec.Y, texture.Width, texture.Height);
		}

		public void update(double dt)
		{
			boundingRectangle = new Rectangle ((int)position.X, (int)position.Y, boundingRectangle.Width, boundingRectangle.Height);
			this.animHandler.update (dt);
		}

		public void draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw (this.texture, this.position, this.animHandler.textureSection, Color.White);
		}

		public void draw(SpriteBatch spriteBatch, Vector2 coordinates) { // used when this.position is not in window's coordinate system
			spriteBatch.Draw (this.texture, coordinates, this.animHandler.textureSection, Color.White);
		}

		public void collide() {
			return;
		}

		public void interact() {
			return;
		}

		public static double getDistance(Sprite one, Sprite two) {
			return Math.Sqrt (Math.Pow (one.boundingRectangle.Center.X - two.boundingRectangle.Center.X, 2) + Math.Pow (one.boundingRectangle.Center.Y - two.boundingRectangle.Center.Y, 2));
		}


        public static double getRectangleDistance(Point one, Point two)
        {
            return Math.Sqrt(Math.Pow(one.X - two.X, 2) + Math.Pow(one.Y - two.Y, 2));
        }
    }
}

