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
        private Character model;

        /// <summary>
        /// The game input model
        /// </summary>
        private GameInput input;
        
        /// <summary>
        /// The level model
        /// </summary>
        private Level level;


        /// <summary>
        /// The current movement of the model
        /// </summary>
        private Movement currentMovement;
        
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
                        currentMovement = new Movement(model, level, Direction.DOWN, 2);
                    }
                    if (inputState.IsKeyDown(InputButton.UP))
                    {
                        currentMovement = new Movement(model, level, Direction.UP, 2);
                    }
                    if (inputState.IsKeyDown(InputButton.LEFT))
                    {
                        currentMovement = new Movement(model, level, Direction.LEFT, 2);
                    }
                    if (inputState.IsKeyDown(InputButton.RIGHT))
                    {
                        currentMovement = new Movement(model, level, Direction.RIGHT, 2);
                    }
                }

                if (currentMovement != null && !currentMovement.IsComplete())
                {
                    currentMovement.Update();
                }
            }

            lastState = inputState;
        }

        /// <summary>
        /// Sets the model
        /// </summary>
        /// <param name="character"></param>
        internal void SetModel(Character character)
        {
            this.model = character;
        }

        /// <summary>
        /// Sets the level
        /// </summary>
        /// <param name="level"></param>
        internal void SetLevel(Level level)
        {
            this.level = level;
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
