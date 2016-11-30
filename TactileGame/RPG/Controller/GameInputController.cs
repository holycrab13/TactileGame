using BrailleIO.Interface;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Input;
using TactileGame.RPG.Models;

namespace TactileGame
{
    /// <summary>
    /// The input controller, updated by the framework method
    /// </summary>
    class GameInputController
    {
        /// <summary>
        /// The input model
        /// </summary>
        private GameInput model;

        /// <summary>
        /// Updates the control state with the current brailleIO device button states
        /// </summary>
        /// <param name="brailleIO_DeviceButtonStates"></param>
        internal void UpdateButtonState(BrailleIO.Interface.BrailleIO_DeviceButtonStates brailleIO_DeviceButtonStates)
        {
            switch (brailleIO_DeviceButtonStates)
            {
                case BrailleIO_DeviceButtonStates.EnterDown:
                    model.PressButton(0);
                    break;
                case BrailleIO_DeviceButtonStates.EnterUp:
                    model.LiftButton(0);
                    break;
                case BrailleIO_DeviceButtonStates.AbortDown:
                    model.PressButton(1);
                    break;
                case BrailleIO_DeviceButtonStates.AbortUp:
                    model.LiftButton(1);
                    break;
                case BrailleIO_DeviceButtonStates.LeftDown:
                    model.PressButton(2);
                    break;
                case BrailleIO_DeviceButtonStates.LeftUp:
                    model.LiftButton(2);
                    break;
                case BrailleIO_DeviceButtonStates.RightDown:
                    model.PressButton(3);
                    break;
                case BrailleIO_DeviceButtonStates.RightUp:
                    model.LiftButton(3);
                    break;
                case BrailleIO_DeviceButtonStates.UpDown:
                    model.PressButton(4);
                    break;
                case BrailleIO_DeviceButtonStates.UpUp:
                    model.LiftButton(4);
                    break;
                case BrailleIO_DeviceButtonStates.DownDown:
                    model.PressButton(5);
                    break;
                case BrailleIO_DeviceButtonStates.DownUp:
                    model.LiftButton(5);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the model
        /// </summary>
        /// <param name="model"></param>
        public void SetModel(GameInput model)
        {
            this.model = model;
        }

    }
}
