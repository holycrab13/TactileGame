using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Input;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Controller
{
    class CharacterController
    {
        /// <summary>
        /// The model
        /// </summary>
        private CharacterModel model;

        /// <summary>
        /// The game input model
        /// </summary>
        private GameInput input;
        
        /// <summary>
        /// The level model
        /// </summary>
        private LevelModel levelModel;


        /// <summary>
        /// The current movement of the model
        /// </summary>
        private Move currentMovement;
        
        /// <summary>
        /// The last input model state
        /// </summary>
        private InputState lastState;
        
        /// <summary>
        /// Updates the character controller
        /// </summary>
        internal void Update()
        {
            InputState inputState = input.GetState();

            if (Game.gameState == GameState.Exploration)
            {
                if (currentMovement == null || currentMovement.IsComplete())
                {
                    
                    if (inputState.IsKeyDown(InputButton.DOWN))
                    {
                        model.character.Rotation = Direction.DOWN;

                        if (levelModel.level.IsValidPosition(model.character, model.character.GetLookAt()))
                        {
                            currentMovement = new Move(model.character, Direction.DOWN, 2);
                        }
                    }
                    if (inputState.IsKeyDown(InputButton.UP))
                    {
                        model.character.Rotation = Direction.UP;

                        if (levelModel.level.IsValidPosition(model.character, model.character.GetLookAt()))
                        {
                            currentMovement = new Move(model.character, Direction.UP, 2);
                        }
                    }
                    if (inputState.IsKeyDown(InputButton.LEFT))
                    {
                        model.character.Rotation = Direction.LEFT;

                        if (levelModel.level.IsValidPosition(model.character, model.character.GetLookAt()))
                        {
                            currentMovement = new Move(model.character, Direction.LEFT, 2);
                        }
                    }
                    if (inputState.IsKeyDown(InputButton.RIGHT))
                    {
                        model.character.Rotation = Direction.RIGHT;

                        if (levelModel.level.IsValidPosition(model.character, model.character.GetLookAt()))
                        {
                            currentMovement = new Move(model.character, Direction.RIGHT, 2);
                        }
                    }
                }

                if (currentMovement != null && !currentMovement.IsComplete())
                {
                    currentMovement.Update(null);
                }

                if(currentMovement != null && currentMovement.IsComplete())
                {
                    levelModel.TriggerEventAt(model.character.X, model.character.Y);
                    currentMovement = null;
                }
            }

            lastState = inputState;
        }

        /// <summary>
        /// Sets the model
        /// </summary>
        /// <param name="character"></param>
        internal void SetModel(CharacterModel character)
        {
            this.model = character;
        }

        /// <summary>
        /// Sets the level
        /// </summary>
        /// <param name="level"></param>
        internal void SetLevel(LevelModel level)
        {
            this.levelModel = level;
        }

        /// <summary>
        /// Sets the input model
        /// </summary>
        /// <param name="gameInput"></param>
        internal void setInput(GameInput gameInput)
        {
            this.input = gameInput;

            if (input != null)
            {
                lastState = input.GetState();
            }
        }

      
    }
}
