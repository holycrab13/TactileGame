using BrailleIO.Interface;
using Gestures.Recognition;
using Gestures.Recognition.Preprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame
{
    partial class Game
    {
        /// <summary>
        /// Handles the keyStateChanged event of the adapter control.
        /// Occurs when device buttons gets pressed or released.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrailleIO_KeyStateChanged_EventArgs"/> instance containing the event data.</param>
        void adapter_keyStateChanged(object sender, BrailleIO_KeyStateChanged_EventArgs e)
        {
            if (e != null)
            {
                // check general buttons
                if (e.keyCode != BrailleIO_DeviceButtonStates.None && !e.keyCode.HasFlag(BrailleIO_DeviceButtonStates.Unknown))
                {
                    gameInputController.UpdateButtonState(e.keyCode);
                }
            }
        }

    }
}
