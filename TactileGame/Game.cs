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
using TactileGame.RPG.Menu;
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
        internal  AudioRenderer audio = AudioRenderer.Instance;

        /// <summary>
        /// The current game progress
        /// </summary>
        public static Dictionary<string, bool> knowledge;

        /// <summary>
        /// A small localization framework for providing translated text templates. 
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/DotNet_LanguageLocalization"/>
        /// </summary>
        public static LL ll;

        private UpDownMenu mainMenuModel;

        private UpDownMenu pauseMenuModel;

        private UpDownMenu saveMenuModel;

        private UpDownMenu loadMenuModel;

        public Game()
        {

#if DEBUG
            // set the logger 'sensitivity': In DEBUG compilation every Message should be logged.
            // in release compilation the normal 'sensitivity' LogPriority.MIDDLE is used.
            if (logger != null) logger.Priority = LogPriority.DEBUG;
#endif

            initLL();

            // Setup menu structure
            mainMenuModel = new UpDownMenu(() => GoToScreen(startScreen), new UpDownMenuItem[] 
            {
                new UpDownMenuItem(startTutorial, ll.GetTrans("game.menu.tutorial")),
                new UpDownMenuItem(startNewGame, ll.GetTrans("game.menu.new")),
                new UpDownMenuItem(() => GoToScreen(loadMenuScreen), ll.GetTrans("game.menu.load")),
                new UpDownMenuItem(exitApplication, ll.GetTrans("game.menu.exit")),
                
            });
       
            pauseMenuModel = new UpDownMenu(() => GoToScreen(gameScreen), new UpDownMenuItem[] 
            {
                new UpDownMenuItem(() => GoToScreen(gameScreen), ll.GetTrans("game.pause.resume")),
                new UpDownMenuItem(() => GoToScreen(saveMenuScreen), ll.GetTrans("game.pause.save")),
                new UpDownMenuItem(() => GoToScreen(mainMenuScreen), ll.GetTrans("game.pause.exit")),
                
            });

            saveMenuModel = new UpDownMenu(() => GoToScreen(pauseMenuScreen), new UpDownMenuItem[] 
            {
                new UpDownMenuItem(() => saveGame(0), ll.GetTrans("game.save.savetoslot") + " 1"),
                new UpDownMenuItem(() => saveGame(1), ll.GetTrans("game.save.savetoslot") + " 2"),
                new UpDownMenuItem(() => saveGame(2), ll.GetTrans("game.save.savetoslot") + " 3"),
                new UpDownMenuItem(() => saveGame(3), ll.GetTrans("game.save.savetoslot") + " 4"),
                new UpDownMenuItem(() => saveGame(4), ll.GetTrans("game.save.savetoslot") + " 5"),
                new UpDownMenuItem(() => saveGame(5), ll.GetTrans("game.save.savetoslot") + " 6"),
                
            });

            loadMenuModel = new UpDownMenu(() => GoToScreen(mainMenuScreen), new UpDownMenuItem[] 
            {
                new UpDownMenuItem(() => loadGame(0), ll.GetTrans("game.load.loadfromslot") + " 1"),
                new UpDownMenuItem(() => loadGame(1), ll.GetTrans("game.load.loadfromslot") + " 2"),
                new UpDownMenuItem(() => loadGame(2), ll.GetTrans("game.load.loadfromslot") + " 3"),
                new UpDownMenuItem(() => loadGame(3), ll.GetTrans("game.load.loadfromslot") + " 4"),
                new UpDownMenuItem(() => loadGame(4), ll.GetTrans("game.load.loadfromslot") + " 5"),
                new UpDownMenuItem(() => loadGame(5), ll.GetTrans("game.load.loadfromslot") + " 6"),
                
            });

            initializeTui();

            // Insert desired screen
            GoToScreen(startScreen);
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

    }
}
