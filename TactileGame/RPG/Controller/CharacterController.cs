using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Input;
using TactileGame.RPG.Menu;
using TactileGame.RPG.Models;
using tud.mci.tangram.audio;

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

        private Dictionary<InputButton, Direction> directionMap;
        private string bumpSoundPath;

        public CharacterController()
        {
            directionMap = new Dictionary<InputButton, Direction>();
            directionMap[InputButton.LEFT] = Direction.LEFT;
            directionMap[InputButton.RIGHT] = Direction.RIGHT;
            directionMap[InputButton.UP] = Direction.UP;
            directionMap[InputButton.DOWN] = Direction.DOWN;

            var path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path.Substring(6);

            bumpSoundPath = path + "\\Resources\\sounds\\bump.wav";
        }
        /// <summary>
        /// Updates the character controller
        /// </summary>
        internal void Update()
        {
            InputState inputState = input.GetState();

            if (GameScreen.gameState == GameState.Exploration)
            {
                if (currentMovement == null || currentMovement.IsComplete())
                {
                    updateMovementInput(inputState, InputButton.DOWN);
                    updateMovementInput(inputState, InputButton.UP);
                    updateMovementInput(inputState, InputButton.RIGHT);
                    updateMovementInput(inputState, InputButton.LEFT);
                   
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

        private void updateMovementInput(InputState inputState, InputButton button)
        {
            Direction direction = directionMap[button];
            bool directionChanged = direction != model.character.Rotation;

            if (inputState.IsKeyDown(button))
            {
                model.character.Rotation = direction;

                if (levelModel.level.IsValidPosition(model.character, model.character.GetLookAt()))
                {
                    currentMovement = new Move(model.character, direction, 2);
                }
                else if(!lastState.IsKeyDown(button) && !directionChanged)
                {
                    // Sollte es probleme geben, einfach dieses statement auskommentieren
                    AudioRenderer.Instance.PlayWaveImmediately(bumpSoundPath);
                }
            }

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
