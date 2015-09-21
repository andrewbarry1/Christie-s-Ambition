using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
namespace TheGame
{
	public class AnimationHandler
	{
		private List<int> frames;
		private List<int> durations;
		private int currentFrame;
		private double t;
		private int spriteWidth;
		private int spriteHeight;

		public int currentAnimation;
		public Rectangle textureSection;

		public AnimationHandler (int height, int width, List<int> frameCounts, List<int> frameDurations)
		{
			spriteHeight = height;
			spriteWidth = width;
			frames = frameCounts;
			durations = frameDurations;
			textureSection = new Rectangle (0, 0, width, height);
			t = 0;
		}

		public int animationCount()
		{
			return durations.Count;
		}

		public void setAnimation(int n, bool resetTime)
		{
			if (n < 0 || n > this.animationCount () - 1)
				return;
			currentAnimation = n;
			textureSection = new Rectangle (currentFrame * spriteWidth, spriteHeight * n, spriteWidth, spriteHeight);
			if (resetTime) {
				t = 0;
				currentFrame = 0;
			} else if (currentFrame >= frames [currentAnimation]) {
				currentFrame = 0;
			}
		}

		public void update(double dt)
		{
			if (animationCount() == 0) {
				return;
			}
			int nextFrameT = (currentFrame + 1) * durations [currentAnimation];
			if (t + dt >= nextFrameT)
			{	
				currentFrame++;
				textureSection = new Rectangle (textureSection.X + spriteWidth, textureSection.Y, spriteWidth, spriteHeight);
			}

			if (currentFrame > frames [currentAnimation] - 1)
			{
				currentFrame = 0;
				textureSection = new Rectangle (0, textureSection.Y, spriteWidth, spriteHeight);
				t = 0;
			}

			t += dt;

		}


	}
}

