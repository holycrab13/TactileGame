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
                if(appState == RPG.ApplicationState.Start)
                {
                    if (e.keyCode == BrailleIO_DeviceButtonStates.EnterUp)
                    {
                        goToMenu();
                    }
                }
                else if(appState == RPG.ApplicationState.Menu)
                {
                    if (e.keyCode == BrailleIO_DeviceButtonStates.EnterUp)
                    {
                        appState = RPG.ApplicationState.Game;

                        startNewGame();
                    }
                }
                else if (appState == RPG.ApplicationState.Game)
                {
                    // check general buttons
                    if (e.keyCode != BrailleIO_DeviceButtonStates.None && !e.keyCode.HasFlag(BrailleIO_DeviceButtonStates.Unknown))
                    {
                        gameInputController.UpdateButtonState(e.keyCode);
                    }
                }
            }
        }

        private void goToMenu()
        {
            appState = RPG.ApplicationState.Menu;

            io.HideView(MAIN_SCREEN_NAME);
            io.HideView(START_SCREEN_NAME);

            io.ShowView(MENU_SCREEN_NAME);
            io.RenderDisplay();

            mainMenuModel.index = 0;
        }

    }
}
