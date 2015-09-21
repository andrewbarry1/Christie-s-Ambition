using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TheGame
{
	public class InputHandler
	{

		private Dictionary<Keys,bool> keyStates;
        private int leftClick;

		public InputHandler ()
		{
			keyStates = new Dictionary<Keys,bool> ();
		}

		// update key states
		public void update(KeyboardState state) {
			Keys[] pressedKeys = state.GetPressedKeys ();


			List<Keys> handledKeys = new List<Keys>(keyStates.Keys);
			foreach (Keys k in handledKeys) {
				if (!((IList<Keys>)pressedKeys).Contains (k))
					keyStates.Remove (k);
			}

			foreach (Keys k in pressedKeys) {
				if (keyStates.ContainsKey (k)) {
					bool isPressed = keyStates [k];	
					if (isPressed)	
						keyStates [k] = false;
				} else {
					keyStates.Add (k, true);
				}
			}

		}


		// return true if a single keypress event is allowed.
		public bool allowSinglePress(Keys k) {
			if (keyStates.ContainsKey(k))
				return keyStates [k];
			return false;
		}

		// return true if key is being held down
		public bool allowMultiPress(Keys k) {
			return (keyStates.ContainsKey(k));
		}

        public void updateMouse(MouseState state)
        {
            if (state.LeftButton == ButtonState.Released)
            {
                leftClick = 0;
            }
            else if (state.LeftButton == ButtonState.Pressed)
            {
                if (leftClick == 0) leftClick = 1;
                else leftClick = 2;
            }
        }

        public bool allowSingleClick()
        {
            return (leftClick == 1);
        }

        public bool allowMultiClick()
        {
            return (leftClick != 0);
        }
    }
}

