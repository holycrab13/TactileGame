using BrailleIO;
using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tud.mci.extensibility;

namespace TactileGame
{
    /// <summary>
    /// partial part of class Game containing the creation and handling of the 
    /// tactile interface (OUTPUT)
    /// </summary>
    public partial class Game
    {

        #region Members

        /// <summary>
        /// relative file path to the directory where extension can be found
        /// </summary>
        const String EXT_DIR_PATH = @".\EXT";

        /// <summary>
        /// The basic entry point to the BrailleIO framework - Access to tactile output and devices for input control.
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/BrailleIO"/>
        /// [Singleton]
        /// </summary>
        internal BrailleIOMediator io = BrailleIOMediator.Instance;

        /// <summary>
        /// Debug monitor and virtual input/output device for the BrailleIO framework
        /// <see cref="https://github.com/TUD-INF-IAI-MCI/BrailleIO"/>
        /// </summary>
        internal IBrailleIOShowOffMonitor monitor = null;

        /// <summary>
        /// The main screen of this tactile user interface.
        /// A screen can contain multiple regions for content.
        /// Only one Screen can be visible at a time. 
        /// </summary>
        internal BrailleIOScreen mainScreen = null;
        /// <summary>
        /// The name of the main screen
        /// </summary>
        internal const String MAIN_SCREEN_NAME = "GameScreen";
        private string START_SCREEN_NAME = "StartScreen";
        private string MENU_SCREEN_NAME = "MenuScreen";
        private string PAUSE_SCREEN_NAME = "PauseMenu";

        private string MENU_REGION_TUTORIAL_NAME = "MenuRegionTutorial";
        private string MENU_REGION_NEW_NAME = "MenuRegionNewGame";
        private string MENU_REGION_LOAD_NAME = "MenuRegionLoadGame";
        private string MENU_REGION_EXIT_NAME = "MenuRegionExit";


        private string PAUSE_REGION_RESUME_NAME = "PauseRegionResume";
        private string PAUSE_REGION_SAVE_NAME = "PauseRegionSave";
        private string PAUSE_REGION_EXIT_NAME = "PauseRegionExit";
        /// <summary>
        /// The main region for content.
        /// </summary>
        /// <summary>
        /// The name for the main content region
        /// </summary>
        internal const String START_REGION_NAME = "StartRegion";

        internal const String MAIN_MENU_NAME = "MainMenu";
        internal const String MAIN_MENU_SELECTION_NAME = "MainMenuSelection";

     
        /// <summary>
        /// The name of the detail region on the bottom of the display
        /// </summary>
        internal const String DETAIL_REGION_NAME = "DetailRegion";

        
        private BrailleIOScreen menuScreen;
        private BrailleIOScreen startScreen;


        internal BrailleIOViewRange mainRegion = null;
        internal BrailleIOViewRange startRegion = null;
        internal BrailleIOViewRange[] mainMenuItemRegions = null;
        internal BrailleIOViewRange[] pauseMenuItemRegions = null;
        internal BrailleIOViewRange detailRegion = null;
        private BrailleIOScreen pauseScreen;

        #endregion

        bool initializeTui()
        {
            bool success = false;

            if (io != null) // check if framework can be loaded
            {
                // check for basic events from the BrailleIO framework 
                listenToBrailleIOEvents();

                // load real hardware devices (tactile displays)
                loadExtensionDevices(EXT_DIR_PATH);
                // load the debug monitor for the framework
                initDebugGui();


                // create the tactile user interface structure (output)
                setUpTui();
            }

            return success;
        }

        #region Tactile User Interface - OUTPUT

        /// <summary>
        /// Sets up the frame for the tactile user interface output.
        /// </summary>
        private void setUpTui()
        {
            if (io != null)
            {
                // get the current active Adapter and get its dimensions
                // this is necessary to create content regions in correct size
                int width = 120;
                int height = 60;

                // get the dimensions of the current active devices' display area
                getActiveAdapterDimensions(ref width, ref height);

                // create the main screen - a container for content regions
                mainScreen = new BrailleIOScreen();
                startScreen = new BrailleIOScreen();
                menuScreen = new BrailleIOScreen();
                pauseScreen = new BrailleIOScreen();

                mainMenuItemRegions = new BrailleIOViewRange[4];
                pauseMenuItemRegions = new BrailleIOViewRange[3];

                createOrUpdateViewRanges(width, height);

                // Setup Main Screen
                mainScreen.AddViewRange(MAIN_MENU_NAME, mainRegion);
                mainScreen.AddViewRange(DETAIL_REGION_NAME, detailRegion);
                
                // Setup Start Screen
                startScreen.AddViewRange(START_REGION_NAME, startRegion);

                // Setup Menu Screen
                menuScreen.AddViewRange(MENU_REGION_TUTORIAL_NAME, mainMenuItemRegions[0]);
                menuScreen.AddViewRange(MENU_REGION_NEW_NAME, mainMenuItemRegions[1]);
                menuScreen.AddViewRange(MENU_REGION_LOAD_NAME, mainMenuItemRegions[2]);
                menuScreen.AddViewRange(MENU_REGION_EXIT_NAME, mainMenuItemRegions[3]);


                pauseScreen.AddViewRange(PAUSE_REGION_RESUME_NAME, pauseMenuItemRegions[0]);
                pauseScreen.AddViewRange(PAUSE_REGION_SAVE_NAME, pauseMenuItemRegions[1]);
                pauseScreen.AddViewRange(PAUSE_REGION_EXIT_NAME, pauseMenuItemRegions[2]);

                // Add the screens 
                io.AddView(MAIN_SCREEN_NAME, mainScreen);
                io.AddView(START_SCREEN_NAME, startScreen);
                io.AddView(MENU_SCREEN_NAME, menuScreen);
                io.AddView(PAUSE_SCREEN_NAME, pauseScreen);

                // Show the start screen
                goToStart();
            }
        }

        private void createOrUpdateViewRanges(int width, int height)
        {
            createOrUpdateViewRange(ref mainRegion, 0, 0, width, height, 0, 0);
            createOrUpdateViewRange(ref detailRegion, height - 16, 0, width, 16, 1, 1);
            createOrUpdateViewRange(ref startRegion, 0, 0, width, height, 0, 2, ll.GetTrans("game.title"));
            createOrUpdateViewRange(ref mainMenuItemRegions[0], 8, 8, width - 16, 8, 0, 1, ll.GetTrans("game.menu.tutorial"));
            createOrUpdateViewRange(ref mainMenuItemRegions[1], 18, 8, width - 16, 8, 0, 1, ll.GetTrans("game.menu.new"));
            createOrUpdateViewRange(ref mainMenuItemRegions[2], 28, 8, width - 16, 8, 0, 1, ll.GetTrans("game.menu.load"));
            createOrUpdateViewRange(ref mainMenuItemRegions[3], 38, 8, width - 16, 8, 0, 1, ll.GetTrans("game.menu.exit"));
            createOrUpdateViewRange(ref pauseMenuItemRegions[0], 8, 8, width - 16, 8, 0, 1, ll.GetTrans("game.pause.resume"));
            createOrUpdateViewRange(ref pauseMenuItemRegions[1], 18, 8, width - 16, 8, 0, 1, ll.GetTrans("game.pause.save"));
            createOrUpdateViewRange(ref pauseMenuItemRegions[2], 28, 8, width - 16, 8, 0, 1, ll.GetTrans("game.pause.exit"));
        }

        private void updateMainMenuTui()
        {
            for(int i = 0; i < mainMenuItemRegions.Length; i++)
            {
                if(i == mainMenuModel.index)
                {
                    mainMenuItemRegions[i].SetBorder(1);
                    mainMenuItemRegions[i].SetPadding(1);


                    audio.AbortCurrentSound();
                    audio.PlaySound(mainMenuItemRegions[i].GetText());
                }
                else
                {
                    mainMenuItemRegions[i].SetBorder(0);
                    mainMenuItemRegions[i].SetPadding(2);
                }
            }

            io.RenderDisplay();
        }

        private void updatePauseMenuTui()
        {
            for (int i = 0; i < pauseMenuItemRegions.Length; i++)
            {
                if (i == pauseMenuModel.index)
                {
                    pauseMenuItemRegions[i].SetBorder(1);
                    pauseMenuItemRegions[i].SetPadding(1);

                    audio.AbortCurrentSound();
                    audio.PlaySound(pauseMenuItemRegions[i].GetText());
                }
                else
                {
                    pauseMenuItemRegions[i].SetBorder(0);
                    pauseMenuItemRegions[i].SetPadding(2);
                }
            }

            io.RenderDisplay();
        }

        private void createOrUpdateViewRange(ref BrailleIOViewRange viewRange, int top, int left, int width, int height, uint border = 0, 
            uint padding = 0, string text = null)
        {
            if (viewRange == null)
            {
                viewRange = new BrailleIOViewRange(top, left, width, height);
            }

            viewRange.SetTop(top);
            viewRange.SetWidth(width);
            viewRange.SetHeight(height);
            viewRange.SetLeft(left);
            viewRange.SetBorder(border, border, border);
            viewRange.SetPadding(padding);

            if(text != null)
            {
                viewRange.SetText(text);
            }
        }



        /// <summary>
        /// Updates the tactile interface with the new display dimensions.
        /// </summary>
        private void updateDisplayDimensions()
        {
            // get the current active Adapter and get its dimensions
            // this is necessary to create content regions in correct size
            int width = 120;
            int height = 60;

            // get the dimensions of the current active devices' display area
            getActiveAdapterDimensions(ref width, ref height);

            createOrUpdateViewRanges(width, height);
        }

        #endregion

        #region Device Handling

        /// <summary>
        /// Try to find real hardware device implementations. 
        /// Thereto we search for defined interface-implementations or classes as compiled dlls in a special directory.
        /// ATTENTION: every "extension" should be in it's own directory.  
        /// </summary>
        /// <param name="extensionPath">The path to the directory where extension can be found.</param>
        private List<Type> loadExtensionDevices(string extensionPath)
        {
            var extTypes = new List<Type>();
            try
            {
                var dir = new DirectoryInfo(extensionPath);
                if (dir.Exists)
                {
                    var dirs = dir.GetDirectories();
                    // check every subdirectory
                    foreach (var directoryInfo in dirs)
                    {
                        // let the Extension loader fetch all dlls (classes) that implement a certain type or interface
                        // in this example the type to search for is IBrailleIOAdapter --> which are wrappers to real tactile display hardware devices
                        var extension = ExtensionLoader.LoadAllExtensions(typeof(IBrailleIOAdapter), directoryInfo.FullName).SelectMany(x => x.Value);
                        extTypes.AddRange(extension);
                    }
                }

                // now we can instantiate the single identified extension classes
                if (io != null && io.AdapterManager != null)
                {
                    foreach (var extType in extTypes)
                    {
                        // you can also add constructor parameters such as the example object someConstrParam
                        var adapter = Activator.CreateInstance(extType, io.AdapterManager);
                        if (adapter != null)
                        {
                            io.AdapterManager.AddAdapter(adapter as IBrailleIOAdapter);
                            // make the loaded adapter the Active one 
                            // --> is seems to be the replacement for the automatically loaded ShowOff adapter.
                            io.AdapterManager.ActiveAdapter = adapter as IBrailleIOAdapter;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.Log(tud.mci.tangram.LogPriority.MIDDLE, this, "Exception while loading extensions", e);
                }
            }
            return extTypes;
        }

        /// <summary>
        /// Initializes a debug monitor and registers to its events.
        /// </summary>
        private void initDebugGui()
        {
            monitor = loadShowOffAdapter(ll.GetTrans("some.key.title") + " Monitor");
            if (monitor != null)
            {
                // listen to device events
                monitor.Disposed += monitor_Disposed;
            }
        }

        void monitor_Disposed(object sender, EventArgs e)
        {
            // kill the whole application if the debug monitor GUI gets closed.
            Application.Exit();
        }

        /// <summary>
        /// Sets up a virtual hardware device. 
        /// This debug implementation for a real hardware device should enable you to 
        /// develop and debug your application without the need of a real hardware device.
        /// It emulates a BrailleDis 7200 device from the Metec AG
        /// </summary>
        /// <param name="title">The title of the device form.</param>
        /// <returns>The instantiated Monitor (can be used for further debug output)</returns>
        IBrailleIOShowOffMonitor loadShowOffAdapter(string title)
        {
            IBrailleIOShowOffMonitor monitor = null;

            if (io != null)
            {
                // create a debug adapter instance (form etc. )
                monitor = new ShowOff();
                ((ShowOff)monitor).SetTitle(title);

                // get the "faked" hardware implementation of this debug monitor
                var showOffAdapter = monitor.GetAdapter(io.AdapterManager);
                // set flag, that this adapter should "mirror" all outputs to the active adapter as well.
                showOffAdapter.Synch = true;

                if (io.AdapterManager != null)
                {
                    // add this debug adapter to the current available AdapterManager to make it available to the BrailleIO users
                    io.AdapterManager.AddAdapter(showOffAdapter);
                    // set it as the main adapter for input and output if no other is available yet
                    if (io.AdapterManager.ActiveAdapter == null)
                    {
                        io.AdapterManager.ActiveAdapter = showOffAdapter;
                    }
                }
            }

            return monitor;
        }

        /// <summary>
        /// Gets the active adapters' display dimensions.
        /// </summary>
        /// <param name="width">The width of the display area (pins).</param>
        /// <param name="height">The height of the display area (pins).</param>
        private void getActiveAdapterDimensions(ref int width, ref int height)
        {
            if (io.AdapterManager != null && io.AdapterManager.ActiveAdapter != null && io.AdapterManager.ActiveAdapter.Device != null)
            {
                var aadpt = io.AdapterManager.ActiveAdapter;
                width = aadpt.Device.DeviceSizeX;
                height = aadpt.Device.DeviceSizeY;
            }
        }

        #region Events

        /// <summary>
        /// registers to some important events of the BarilleIO frameworks' Mediator object
        /// </summary>
        private void listenToBrailleIOEvents()
        {
            if (io != null)
            {
                if (io.AdapterManager != null) registerToAdapterManagerEvents(io.AdapterManager);
                io.ActiveAdapterChanged += io_ActiveAdapterChanged;
                io.AdapterManagerChanged += io_AdapterManagerChanged;
            }
        }


        /// <summary>
        /// Handles the AdapterManagerChanged event of the io control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void io_AdapterManagerChanged(object sender, EventArgs e)
        {
            if (io != null && io.AdapterManager != null)
            {
                unregisterFromAdapterManagerEvents(io.AdapterManager);
                registerToAdapterManagerEvents(io.AdapterManager);
            }
        }

        /// <summary>
        /// Handles the ActiveAdapterChanged event of the io control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrailleIO.Interface.IBrailleIOAdapterEventArgs"/> instance containing the event data.</param>
        void io_ActiveAdapterChanged(object sender, BrailleIO.Interface.IBrailleIOAdapterEventArgs e)
        {
            // update the tactile interface to the new dimensions!
            updateDisplayDimensions();
        }

        /// <summary>
        /// Registers to some important events from hardware devices.
        /// </summary>
        /// <param name="adapter">The adapter to register to.</param>
        void registerToDeviceEvents(IBrailleIOAdapter adapter)
        {
            if (adapter != null)
            {
                try
                {
                    // first unrgister to prevent double interpretation
                    unregsiterFromDeviceEvents(adapter);

                    adapter.initialized += adapter_initialized;
                    adapter.keyStateChanged += adapter_keyStateChanged;
                }
                catch (Exception ex)
                {
                    // log the occurred exception to log file.
                    if (logger != null) logger.Log(tud.mci.tangram.LogPriority.IMPORTANT, this, "Exception while register to device events.", ex);
                }
            }
        }

        /// <summary>
        /// Unregsiter from device events to prevent a double registration or deactivate.
        /// </summary>
        /// <param name="adapter">The adapter to unregister from.</param>
        void unregsiterFromDeviceEvents(IBrailleIOAdapter adapter)
        {
            if (adapter != null)
            {
                try
                {
                    adapter.initialized -= adapter_initialized;
                    adapter.keyStateChanged -= adapter_keyStateChanged;
                }
                catch (Exception ex)
                {
                    // log the occurred exception to log file.
                    if (logger != null) logger.Log(tud.mci.tangram.LogPriority.IMPORTANT, this, "Exception while unregister from device events.", ex);
                }
            }
        }

        /// <summary>
        /// Registers to adapter manager events.
        /// </summary>
        /// <param name="manager">The manager.</param>
        void registerToAdapterManagerEvents(IBrailleIOAdapterManager manager)
        {
            if (manager != null)
            {
                manager.AdapterRemoved += manager_AdapterRemoved;
                manager.NewAdapterRegistered += manager_NewAdapterRegistered;
            }
        }

        /// <summary>
        /// Unregisters from adapter manager events.
        /// </summary>
        /// <param name="manager">The manager.</param>
        void unregisterFromAdapterManagerEvents(IBrailleIOAdapterManager manager)
        {
            if (manager != null)
            {
                manager.AdapterRemoved -= manager_AdapterRemoved;
                manager.NewAdapterRegistered -= manager_NewAdapterRegistered;
            }
        }

        /// <summary>
        /// Handles the NewAdapterRegistered event of the manager control.
        /// Happens if a new adapter was registered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrailleIO.Interface.IBrailleIOAdapterEventArgs"/> instance containing the event data.</param>
        void manager_NewAdapterRegistered(object sender, IBrailleIOAdapterEventArgs e)
        {
            // TODO: implement
            if (e != null && e.Adapter != null)
            {
                // register to the adapter events
                unregsiterFromDeviceEvents(e.Adapter);
                registerToDeviceEvents(e.Adapter);
            }
        }

        /// <summary>
        /// Handles the AdapterRemoved event of the manager control.
        /// Happens if an adapter was removed from the framework.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrailleIO.Interface.IBrailleIOAdapterEventArgs"/> instance containing the event data.</param>
        void manager_AdapterRemoved(object sender, IBrailleIOAdapterEventArgs e)
        {
            // TODO: handle
            // TODO: check which adapter should be used now for Active one etc.
            if (e != null && e.Adapter != null)
            {
                unregsiterFromDeviceEvents(e.Adapter);
            }
        }

        /// <summary>
        /// Handles the initialized event of the adapter control.
        /// Happens if an adapter was started and has finished his initialization routines - is now ready for use.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="BrailleIO_Initialized_EventArgs"/> instance containing the event data.</param>
        void adapter_initialized(object sender, BrailleIO_Initialized_EventArgs e)
        {
            // example: send an auditory information to the user for this new device.
            if (sender != null && audio != null && ll != null)
            {
                String adptrName = String.Empty;
                if (sender is IBrailleIOAdapter && ((IBrailleIOAdapter)sender).Device != null)
                {
                    adptrName = ((IBrailleIOAdapter)sender).Device.Name;
                }

                
            }
        }

        #endregion

        #endregion


    }
}
