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
using TactileGame.RPG.Menu;

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
            if(currentScreen != null)
            {
                Task t = new Task(new Action(() => { currentScreen.HandleInteraction(e.keyCode); }));
                t.Start();
               
            }
        }
    
        private void startTutorial()
        {
            gameScreen.LoadGame("tutorial");
            GoToScreen(gameScreen);
        }

        private void startNewGame()
        {
            gameScreen.StartNewGame();
            GoToScreen(gameScreen);
        }

        private void saveGame(int index)
        {
            gameScreen.SaveGame("game_state_" + index);
            GoToScreen(pauseMenuScreen);
        }

        private void loadGame(int index)
        {
            gameScreen.LoadGame("game_state_" + index);
            GoToScreen(gameScreen);
        }

        private void exitApplication()
        {
            Application.Exit();
        }

        public static void GoToScreen(InteractionScreen screen)
        {
            if(currentScreen != null)
            {
                currentScreen.Hide();
            }

            currentScreen = screen;

            if(currentScreen != null)
            {
                currentScreen.Show();
            }
        }
    }
}
