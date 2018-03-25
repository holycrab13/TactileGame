using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TactileGame.RPG.Input;
using TactileGame.RPG.Menu;
using TactileGame.RPG.Models;
using tud.mci.tangram.audio;

namespace TactileGame
{
    class DialogueController
    {
        private InputState lastState;
        private GameInput input;
        private DialogueModel model;
        private LevelModel level;
        
        private Dictionary<InputButton, bool> pressedMap;

        public DialogueController()
        {
            pressedMap = new Dictionary<InputButton, bool>();
            pressedMap[InputButton.A] = false;
            pressedMap[InputButton.LEFT] = false;
            pressedMap[InputButton.RIGHT] = false;
            pressedMap[InputButton.DOWN] = false;
            pressedMap[InputButton.UP] = false;
        }

        internal void Update()
        {

            InputState inputState = input.GetState();

            if (GameScreen.gameState == RPG.GameState.Event)
            {

                if (model.HasAction())
                {
                    updateDialogue(inputState, InputButton.A);
                    updateDialogue(inputState, InputButton.LEFT);
                    updateDialogue(inputState, InputButton.RIGHT);
                    updateDialogue(inputState, InputButton.DOWN);
                    updateDialogue(inputState, InputButton.UP);
                }
                else
                {
                    pressedMap[InputButton.A] = false;
                    pressedMap[InputButton.LEFT] = false;
                    pressedMap[InputButton.RIGHT] = false;
                    pressedMap[InputButton.DOWN] = false;
                    pressedMap[InputButton.UP] = false;
                }
            }

            lastState = inputState;
        
        }

        private void updateDialogue(InputState inputState, InputButton button)
        {

            if (inputState.IsKeyDown(button) && !lastState.IsKeyDown(button))
            {
                pressedMap[button] = true;
            }

            if (!inputState.IsKeyDown(button) && lastState.IsKeyDown(button) && pressedMap[button])
            {
                model.Confirm(level);
                pressedMap[button] = false;
            }
        }


        private void readAnswer(string answer)
        {
           if(answer != null)
           {
               AudioRenderer.Instance.AbortCurrentSound();
               AudioRenderer.Instance.PlaySound(answer);
           }
        }

  
        internal void setInput(GameInput gameInput)
        {
            this.input = gameInput;

            if (input != null)
            {
                lastState = input.GetState();
            }
        }

        internal void SetModel(DialogueModel model)
        {
            this.model = model;
        }

        internal void SetLevel(LevelModel levelModel)
        {
            this.level = levelModel;
        }
    }
}


