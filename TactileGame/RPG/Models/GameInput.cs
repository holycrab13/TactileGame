using System.Collections.Generic;
using TactileGame.RPG.Input;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// This class simulates the game boy like input controls
    /// </summary>
    class GameInput
    {
        /// <summary>
        /// We have 6 buttons
        /// 0 - A
        /// 1 - B
        /// 2 - LEFT
        /// 3 - RIGHT
        /// 4 - UP
        /// 5 - DOWN
        /// </summary>
        private bool[] _keystates = new bool[6];

  
        /// <summary>
        /// Gets a snapshot of the current control state
        /// </summary>
        /// <returns></returns>
        public InputState GetState()
        {
            return new InputState()
            {
                Keystates = new Dictionary<InputButton, bool>() 
                {
                    { InputButton.A, _keystates[0] },
                    { InputButton.B, _keystates[1] },
                    { InputButton.LEFT, _keystates[2] },
                    { InputButton.RIGHT, _keystates[3] },
                    { InputButton.UP, _keystates[4] },
                    { InputButton.DOWN, _keystates[5] },
                }
            };
        }

        /// <summary>
        /// Presses a button
        /// </summary>
        /// <param name="p">The button index</param>
        public void PressButton(int p)
        {
            _keystates[p] = true;
        }

        /// <summary>
        /// Releases a button
        /// </summary>
        /// <param name="p">the button index</param>
        public void LiftButton(int p)
        {
            _keystates[p] = false;
        }
    }
}
