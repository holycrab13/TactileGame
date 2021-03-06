﻿using BrailleIO;
using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Files;
using TactileGame.RPG.Models;
using tud.mci.LanguageLocalization;
using tud.mci.tangram.audio;

namespace TactileGame.RPG.Menu
{
    /// <summary>
    /// This is where the game loop is
    /// </summary>
    public class GameScreen : InteractionScreen
    {
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
        /// The shared game input model
        /// </summary>
        private GameInput gameInput;

        /// <summary>
        /// The shared game dialogue model
        /// </summary>
        private DialogueModel gameDialogue;

        /// <summary>
        /// The shared level model holding the current level
        /// </summary>
        private LevelModel levelModel;

        /// <summary>
        /// The shared characterModel holding the avatar
        /// </summary>
        private CharacterModel characterModel;

        /// <summary>
        /// The front and backbuffer for flicker-free rendering
        /// </summary>
        private BooleanCanvas[] buffers;

        /// <summary>
        /// The index of the current buffer (swapped every frame)
        /// </summary>
        private int bufferIndex;

        /// <summary>
        /// The BrailleIO framework region to render the main content to
        /// </summary>
        internal BrailleIOViewRange mainRegion = null;

        /// <summary>
        /// The BrailleIO framework region for info text
        /// </summary>
        internal BrailleIOViewRange detailRegion = null;

        /// <summary>
        /// The current state of the game (exploration or dialogue)
        /// </summary>
        public static GameState gameState;

        /// <summary>
        /// The language settings
        /// </summary>
        private LL ll;
        private Task gameThread;
        private bool gameRunning;
        private bool textRegionDirty;

        /// <summary>
        /// Creates a new gamescreen
        /// </summary>
        /// <param name="ll">Language settings</param>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        public GameScreen(LL ll, int width, int height) : base("GameScreen")
        {
            this.ll = ll;
            this.SetWidth(width);
            this.SetHeight(height);
         
            mainRegion = new BrailleIOViewRange(0, 0, width, height);
            detailRegion = new BrailleIOViewRange(0, height - 18, width, 18);
            detailRegion.SetBorder(1);
            detailRegion.SetPadding(1);

            AddViewRange(mainRegion);
            AddViewRange(detailRegion);

            gameState = GameState.Exploration;
        }

        /// <summary>
        /// Resizes the screen
        /// </summary>
        /// <param name="width">The new width</param>
        /// <param name="height">The new height</param>
        public override void Resize(int width, int height)
        {
            this.SetWidth(width);
            this.SetHeight(height);

            mainRegion.SetWidth(width);
            mainRegion.SetHeight(height);

            detailRegion.SetWidth(width);
            detailRegion.SetHeight(height);
        }

        /// <summary>
        /// Called every frame to drive the game logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tick()
        {
            // Push the last update to the screen

            if(!buffers[0].Equals(buffers[1]) || textRegionDirty)
            {
                textRegionDirty = false;
                Render();
            }

            // Update the controllers
            characterController.Update();
            levelController.Update();
            dialogueController.Update();


            // Do the rendering/sound stuff. TODO: Clean this up with views
            buffers[bufferIndex].Clear();

            buffers[bufferIndex].X = (levelModel.Avatar.X + levelModel.Avatar.Width / 2) - buffers[bufferIndex].Width / 2;
            buffers[bufferIndex].Y = (levelModel.Avatar.Y + levelModel.Avatar.Height / 2) - buffers[bufferIndex].Height / 2;

            foreach (WorldObject obj in levelModel.level.Objects)
            {
                if (!obj.isHidden)
                {
                    buffers[bufferIndex].Draw(obj);
                }
            }

            buffers[bufferIndex].Draw(levelModel.Avatar);
            mainRegion.SetMatrix(buffers[bufferIndex].Data);

            bufferIndex = (bufferIndex + 1) % 2;

            if (gameState == GameState.Event && gameDialogue.HasAction())
            {              

                if (detailRegion.GetText() != gameDialogue.GetCurrent())
                {
                    detailRegion.SetText(gameDialogue.GetCurrent());
                    textRegionDirty = true;
                    Audio.AbortCurrentSound();
                    Audio.PlaySound(gameDialogue.GetCurrent());
                }

                if (!detailRegion.IsVisible()) 
                {
                    detailRegion.SetVisibility(true);
                    textRegionDirty = true;
                }
            }
            else
            {
                Audio.AbortCurrentSound();
                detailRegion.SetText(string.Empty);

                if(detailRegion.IsVisible()) 
                {
                    textRegionDirty = true;
                    detailRegion.SetVisibility(false);
                }
            }
        }

        /// <summary>
        /// Handles interaction with the screen
        /// </summary>
        /// <param name="keyEvent"></param>
        public override void HandleInteraction(BrailleIO.Interface.BrailleIO_DeviceButtonStates keyEvent)
        {
            if (keyEvent != BrailleIO_DeviceButtonStates.None && !keyEvent.HasFlag(BrailleIO_DeviceButtonStates.Unknown))
            {
                gameInputController.UpdateButtonState(keyEvent);
            }
        }

        /// <summary>
        /// Called when this screen is shown
        /// </summary>
        protected override void OnShow()
        {
            detailRegion.SetVisibility(false);

            gameThread = new Task(new Action(() => gameLoop()));
            gameThread.Start();
        }

        private void gameLoop()
        {
            gameRunning = true;

            while (gameRunning)
            {
                Tick();

                Thread.Sleep(50);
            }

        }

        /// <summary>
        /// Called when this screen is hidden
        /// </summary>
        protected override void OnHide()
        {
            gameRunning = false;
        }

        /// <summary>
        /// Loads a game state to the screen
        /// </summary>
        /// <param name="saveGame"></param>
        public void LoadGame(string saveGame)
        {
            SaveGame save = LevelLoader.LoadSaveGame(saveGame, ll);

            applySaveGame(save);
        }

        private void applySaveGame(SaveGame save)
        {
            Game.knowledge = save.Knowledge;

            // Models
            gameInput = new GameInput();
            gameDialogue = new DialogueModel();

            levelModel = new LevelModel(LevelLoader.Load(save.LevelName, ll));
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

            levelController = new LevelController();
            levelController.SetModel(levelModel);
            levelController.SetInput(gameInput);
            levelController.SetDialogue(gameDialogue);

            buffers = new BooleanCanvas[]
            {
                new BooleanCanvas(GetWidth(), GetHeight()),
                new BooleanCanvas(GetWidth(), GetHeight())
            };

            bufferIndex = 0;
        }

        /// <summary>
        /// Loads a new level
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        internal void GoToLevel(string p1, int p2, int p3)
        {
            Direction rotation = levelModel.Avatar.Rotation;

            Level level = LevelLoader.Load(p1, ll);
            level.Avatar.X = p2;
            level.Avatar.Y = p3;

            levelModel.level = level;

            characterModel.character = level.Avatar;
            characterModel.character.Rotation = rotation;
        }

        /// <summary>
        /// Saves the current game to the desired saving slot
        /// </summary>
        /// <param name="p"></param>
        internal void SaveGame(string p)
        {
            LevelLoader.SaveGame(p, levelModel);
        }

        internal void StartNewGame()
        {
            SaveGame save = LevelLoader.CreateNewGame(ll);
            applySaveGame(save);
        }

        internal void OnKnowledgeChanged()
        {
            levelModel.OnKnowledgeChanged();
        }
    }
}
