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


        internal static ApplicationState appState = ApplicationState.Start;

        /// <summary>
        /// The current game progress
        /// </summary>
        internal static Dictionary<string, bool> knowledge;

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
        private DialogueModel gameDialogue;
        private LevelModel levelModel;
        private CharacterModel characterModel;

        private RPG.Menu.MainMenu mainMenuModel;
        private BooleanCanvas[] buffers;
        private int bufferIndex;

        public Game()
        {

#if DEBUG
            // set the logger 'sensitivity': In DEBUG compilation every Message should be logged.
            // in release compilation the normal 'sensitivity' LogPriority.MIDDLE is used.
            if (logger != null) logger.Priority = LogPriority.DEBUG;
#endif
            initLL();
            initializeTui();

            mainMenuModel = new RPG.Menu.MainMenu(startTutorial, startNewGame, loadSavedGame, exitApplication);

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

        private void exitApplication()
        {
            
        }

        private void loadAndStartGame(string saveGame)
        {
            // Models
            gameInput = new GameInput();
            gameDialogue = new DialogueModel();

            SaveGame save = LevelLoader.LoadSaveGame(saveGame, ll);
            knowledge = save.Knowledge;

            levelModel = new LevelModel(LevelLoader.Load("police", ll));
            characterModel = new CharacterModel(levelModel.Avatar);
            characterModel.character.X = save.X;
            characterModel.character.Y = save.Y;

            // Controllers
            gameInputController = new GameInputController();
            gameInputController.SetModel(gameInput);

            characterController = new CharacterController();
            characterController.SetModel(characterModel);
            characterController.setInput(gameInput);
            characterController.SetLevel(levelModel);

            dialogueController = new DialogueController();
            dialogueController.setInput(gameInput);
            dialogueController.SetModel(gameDialogue);
            dialogueController.SetLevel(levelModel);

            levelController = new LevelController(this);
            levelController.SetModel(levelModel);
            levelController.SetInput(gameInput);
            levelController.SetDialogue(gameDialogue);

            buffers = new BooleanCanvas[]
            {
                new BooleanCanvas(mainRegion.GetWidth(), mainRegion.GetHeight()),
                new BooleanCanvas(mainRegion.GetWidth(), mainRegion.GetHeight())
            };

            bufferIndex = 0;
            // Start the game loop!
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Tick;
            timer.Start();

            detailregion.SetVisibility(false);

            io.HideView(MENU_SCREEN_NAME);
            io.ShowView(MAIN_SCREEN_NAME);
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

            buffers[bufferIndex].Clear();

            buffers[bufferIndex].X = (levelModel.Avatar.X + levelModel.Avatar.Width / 2) - buffers[bufferIndex].Width / 2;
            buffers[bufferIndex].Y = (levelModel.Avatar.Y + levelModel.Avatar.Height / 2) - buffers[bufferIndex].Height / 2;



            foreach (WorldObject obj in levelModel.level.Objects)
            {
                buffers[bufferIndex].Draw(obj);
            }

            buffers[bufferIndex].Draw(levelModel.Avatar);
            mainRegion.SetMatrix(buffers[bufferIndex].Data);

            bufferIndex = (bufferIndex + 1) % 2;

            if (gameState == GameState.Event && gameDialogue.HasAction())
            {
                detailregion.SetVisibility(true);

                if (detailregion.GetText() != gameDialogue.GetCurrent())
                {
                    detailregion.SetText(gameDialogue.GetCurrent());
                    audio.AbortCurrentSound();
                    audio.PlaySound(gameDialogue.GetCurrent());
                }
            }
            else
            {
                audio.AbortCurrentSound();
                detailregion.SetText(string.Empty);
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


        internal static bool HasKnowledge(string p)
        {
            if (knowledge.ContainsKey(p)) 
            {
                return knowledge[p];
            }

            return false;
        }



        internal void GoToLevel(string p1, int p2, int p3)
        {
            Level level = LevelLoader.Load(p1 + ".xml", ll);
            level.avatar.X = p2;
            level.avatar.Y = p3;

            levelModel.level = level;

            characterModel.character = level.avatar;
        }
    }
}
