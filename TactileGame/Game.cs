using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TactileGame.RPG;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Files;
using TactileGame.RPG.Input;
using TactileGame.RPG.Models;
using tud.mci.LanguageLocalization;
using tud.mci.tangram;
using tud.mci.tangram.audio;

namespace TactileGame
{
   

    public partial class Game
    {
        /// <summary>
        /// A logger to log messages to a log file for debugging or error logging.
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/DotNet_Logger"/>
        /// [Singleton]
        /// </summary>
        internal static Logger logger = Logger.Instance;

        /// <summary>
        /// A small audio renderer for playing sounds as text-to-speech or wav files.
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/DotNet_AudioRenderer"/>
        /// [Singleton]
        /// </summary>
        internal static AudioRenderer audio = AudioRenderer.Instance;

        /// <summary>
        /// The current game state
        /// </summary>
        internal static GameState gameState = GameState.Exploration;

        /// <summary>
        /// A small localization framework for providing translated text templates. 
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/DotNet_LanguageLocalization"/>
        /// </summary>
        internal LL ll;

        /// <summary>
        /// The controller for the game input
        /// </summary>
        private GameInputController gameInputController;

        /// <summary>
        /// the character controller
        /// </summary>
        private CharacterController characterController;

        /// <summary>
        /// the dialogue controller
        /// </summary>
        private DialogueController dialogueController;

        /// <summary>
        /// The level controller
        /// </summary>
        private LevelController levelController;

        /// <summary>
        /// The current level
        /// </summary>
        private Level level;

        /// <summary>
        /// the canvas to draw to
        /// </summary>
        private BooleanCanvas canvas;

        /// <summary>
        /// The timer running the game loop
        /// </summary>
        private Timer timer;
        
        /// <summary>
        /// The shared game input model
        /// </summary>
        private GameInput gameInput;

        /// <summary>
        /// The shared game dialogue model
        /// </summary>
        private GameDialogue gameDialogue;

        public Game()
        {

#if DEBUG
            // set the logger 'sensitivity': In DEBUG compilation every Message should be logged.
            // in release compilation the normal 'sensitivity' LogPriority.MIDDLE is used.
            if (logger != null) logger.Priority = LogPriority.DEBUG;
#endif
            initLL();
            initializeTui();

            // Models
            gameInput = new GameInput();
            gameDialogue = new GameDialogue();

            level = LevelLoader.Load("test_level2.txt");

            // Controllers
            gameInputController = new GameInputController();
            gameInputController.SetModel(gameInput);

            characterController = new CharacterController();
            characterController.SetModel(level.avatar);
            characterController.setInput(gameInput);
            characterController.SetLevel(level);
            characterController.SetDialogue(gameDialogue);

            dialogueController = new DialogueController();
            dialogueController.setInput(gameInput);
            dialogueController.SetModel(gameDialogue);

            levelController = new LevelController();
            levelController.SetModel(level);
            levelController.SetInput(gameInput);
            levelController.SetDialogue(gameDialogue);

            canvas = new BooleanCanvas(mainRegion.GetWidth(), mainRegion.GetHeight());

            // Start the game loop!
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Tick;
            timer.Start();

            detailregion.SetVisibility(false);
        }

        /// <summary>
        /// This runs the RPG game - it measures the time and fires the update and render method of the RPG
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tick(object sender, EventArgs e)
        {
            // Push the last update to the screen
            io.RenderDisplay();

            // Update the controllers
            characterController.Update();
            levelController.Update();
            dialogueController.Update();

         
            // Do the rendering/sound stuff. TODO: Clean this up with views
            detailregion.SetVisibility(false);

            canvas.X = (level.avatar.X + level.avatar.Width / 2) - canvas.Width / 2;
            canvas.Y = (level.avatar.Y + level.avatar.Height / 2) - canvas.Height / 2;

            canvas.Clear();

            foreach (WorldObject obj in level.Objects)
            {
                canvas.Draw(obj);
            }

            canvas.Draw(level.avatar);
            mainRegion.SetMatrix(canvas.Data);
           

            if (gameState == GameState.Dialogue)
            {
                detailregion.SetVisibility(true);

                if (detailregion.GetText() != gameDialogue.GetCurrent())
                {
                    detailregion.SetText(gameDialogue.GetCurrent());
                    audio.PlaySound(gameDialogue.GetCurrent());
                }
            }
        }



        /// <summary>
        /// Initializes the localization framework with a translation file.
        /// </summary>
        private void initLL()
        {
            // Load an XML file contain the different translations.
            // You can load a file from a path or a file delivered through the project resources.
            ll = new LL(Properties.Resources.language);

            // use this by calling the function:
            // ll.GetTrans(String key, params string[] strs)
        }

    }
}
