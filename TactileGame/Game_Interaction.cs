using BrailleIO.Interface;
using Gestures.Recognition;
using Gestures.Recognition.Preprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                        goToMainMenu();
                    }
                }
                else if(appState == RPG.ApplicationState.Menu)
                {
                    if (e.keyCode == BrailleIO_DeviceButtonStates.UpUp)
                    {
                        mainMenuModel.Previous();
                        updateMainMenuTui();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.DownUp)
                    {
                        mainMenuModel.Next();
                        updateMainMenuTui();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.EnterUp)
                    {
                        mainMenuModel.Select();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.AbortUp)
                    {
                        goToStart();
                    }
                }
                else if (appState == RPG.ApplicationState.Paused)
                {
                    if (e.keyCode == BrailleIO_DeviceButtonStates.UpUp)
                    {
                        pauseMenuModel.Previous();
                        updatePauseMenuTui();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.DownUp)
                    {
                        pauseMenuModel.Next();
                        updatePauseMenuTui();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.EnterUp)
                    {
                        pauseMenuModel.Select();
                    }

                    if (e.keyCode == BrailleIO_DeviceButtonStates.AbortUp)
                    {
                        resumeGame();
                    }
                    // Update pause menu
                }
                else if (appState == RPG.ApplicationState.Playing)
                {
                    // check general buttons
                    if (e.keyCode != BrailleIO_DeviceButtonStates.None && !e.keyCode.HasFlag(BrailleIO_DeviceButtonStates.Unknown))
                    {
                        gameInputController.UpdateButtonState(e.keyCode);
                    }
                }
            }
        }

        private void resumeGame()
        {
            appState = RPG.ApplicationState.Playing;

            io.HideView(PAUSE_SCREEN_NAME);
            io.ShowView(MAIN_SCREEN_NAME);

            io.RenderDisplay();

            timer.Start();
        }

        private void startTutorial()
        {
            loadAndStartGame("tutorial");
        }

        private void startNewGame()
        {
            loadAndStartGame("game_state_new");
        }

        private void loadSavedGame()
        {

        }

        private void saveGame()
        {

        }

        private void exitApplication()
        {
            Application.Exit();
        }

        private void goToStart()
        {
            appState = RPG.ApplicationState.Start;

            audio.PlaySound(ll.GetTrans("game.title"));

            io.HideView(MAIN_SCREEN_NAME);
            io.HideView(MENU_SCREEN_NAME);

            io.ShowView(START_SCREEN_NAME);
            io.RenderDisplay();

            mainMenuModel.index = 0;
        }

        private void goToMainMenu()
        {
            appState = RPG.ApplicationState.Menu;

            timer.Stop();

            io.HideView(MAIN_SCREEN_NAME);
            io.HideView(START_SCREEN_NAME);

            io.ShowView(MENU_SCREEN_NAME);

            mainMenuModel.index = 0;
            updateMainMenuTui();
        }

        internal void pauseGame()
        {
            appState = RPG.ApplicationState.Paused;

            timer.Stop();

            io.HideView(MAIN_SCREEN_NAME);
            io.ShowView(PAUSE_SCREEN_NAME);

            pauseMenuModel.index = 0;
            updatePauseMenuTui();
        }
    }
}
