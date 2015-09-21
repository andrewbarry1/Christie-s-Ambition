using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace TheGame
{
	public class GameStateMainMenu : GameState
	{

		private List<Sprite> buttons;
		private int buttonCursor;
		private InputHandler inputHandler;
		private Game1 game;
		private ContentManager Content;
        private Menu m;
        private TextWindow tw;
        private Sprite bg;

		public GameStateMainMenu (ContentManager c, Game1 g)
		{
			Content = c;


			inputHandler = new InputHandler ();
			buttons = new List<Sprite> ();

			game = g;

            m = new Menu(new Vector2(10, 10), new String[] { "NEW GAME", "CREDITS", "QUIT" }, new Menu.menuAction[] { playerCount, showCredits, quitGame }, 3, 1, "test-semifont.png", Content);

            bg = new Sprite("title-bg.png", Content, new Vector2(0, 0));

		}

        public void playerCount()
        {
            m = new Menu(new Vector2(10, 10), new String[] { "ONE PLAYER", "TWO PLAYERS" }, new Menu.menuAction[] { startOneP, startTwoP }, 2, 1, "test-semifont.png", Content);
        }

        public void startOneP()
        {
            startGame(1);
        }
        public void startTwoP()
        {
            startGame(2);
        }

        private void startGame(int nPlayers)
        {
            m = new Menu(new Vector2(10, 10), new String[] { "NEW GAME", "CREDITS", "QUIT" }, new Menu.menuAction[] { playerCount, showCredits, quitGame }, 3, 1, "test-semifont.png", Content);
            game.doStateTransition(new GameStateMap(Content, game, nPlayers));
        }
        private void showCredits()
        {
            m = null;
            tw = new TextWindow(new string[] { "DEVELOPED BY\\n\\2ANDREW BARRY" }, Content, new Vector2(25,25), new Vector2(200,50));
        }

        private void quitGame()
        {
            m = null;
            game.Exit();
        }


        public void update(double dt)
		{
            if (tw != null)
            {
                tw.update(dt);
            }
            else if (m != null)
            {
                m.update(dt);
            }
		}

		public void draw(SpriteBatch spriteBatch)
		{
            bg.draw(spriteBatch);
            if (tw != null)
            {
                tw.draw(spriteBatch);
            }
            else if (m != null)
            {
                m.draw(spriteBatch);
            }
		}

		public void handleInput()
		{
			inputHandler.update (Keyboard.GetState ());

            if (m != null)
            {
                m.handleInput(inputHandler);
            }

    		else if (inputHandler.allowSinglePress(Keys.Down) && buttonCursor != 2) {
				buttons [buttonCursor].animHandler.setAnimation (0,false);
				buttonCursor++;
				buttons [buttonCursor].animHandler.setAnimation (1,false);
			}
			else if (inputHandler.allowSinglePress(Keys.Up) && buttonCursor != 0) {
				buttons [buttonCursor].animHandler.setAnimation(0,false);
				buttonCursor--;
				buttons[buttonCursor].animHandler.setAnimation(1,false);
			}

            if (inputHandler.allowSinglePress(Keys.Enter) && tw != null)
            {
                if (tw != null)
                {
                    if (tw.waitingForContinue)
                    {
                        tw.advanceLine();
                    }
                    else
                    {
                        tw.fastForward();
                    }
                    if (tw.done)
                    {
                        tw = null;
                        m = new Menu(new Vector2(10, 10), new String[] { "NEW GAME", "CREDITS", "QUIT" }, new Menu.menuAction[] { playerCount, showCredits, quitGame }, 3, 1, "test-semifont.png", Content);
                    }
                }
            }
		}




	}
}

