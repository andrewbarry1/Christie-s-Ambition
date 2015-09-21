using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TheGame
{
    class County
    {

        private Sprite individualSprite;
        private Sprite mapSprite;

        public int occupied;
        public int troops;
        public int revenue;
        public Rectangle mapRectangle;
        public string name;
        public int[] edges;

        public int value;

        public County(string countyName, Sprite individual, Sprite inMap, int rev, int pop, int[] adj)
        {

            occupied = 0;
            troops = 0;
            revenue = rev;
            troops = pop;
            name = countyName;
            edges = adj;

            value = (pop + (2 * rev));

            individualSprite = individual;
            mapSprite = inMap;

            int mrx = inMap.boundingRectangle.Width - individual.boundingRectangle.Width;
            int mry = inMap.boundingRectangle.Height - individual.boundingRectangle.Height;
//			mapRectangle = new Rectangle (mrx, mry, individual.boundingRectangle.Width, individual.boundingRectangle.Height); // Linux
            mapRectangle = new Rectangle(new Point(mrx, mry), individual.boundingRectangle.Size); // Windows
        }

        public string getOwner()
        {
            switch (occupied)
            {
			case 0:
				{
					return "UNOCCUPIED";
					break; }
			case 1:
				{
					return "\\3CHRISTIE";
					break; }
			case 2:
				{
					return "\\4LABOR UNION";
					break;}
            }
            return "UNOCCUPIED";
        }

        public void drawMap(SpriteBatch spriteBatch)
        {
            Color occupationColor = Color.White;
            switch (occupied)
            {
                case 0: occupationColor = Color.White; break;
                case 1: occupationColor = Color.Red; break;
                case 2: occupationColor = Color.Cyan; break;
            }

            spriteBatch.Draw(mapSprite.texture, new Vector2(0,0), occupationColor);

        }

        public void drawIndividual(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(individualSprite.texture, individualSprite.position, Color.White);  // TODO - fix this Vector2
        }


    }
}