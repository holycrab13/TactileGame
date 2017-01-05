using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TactileGame.RPG.Input;
using TactileGame.RPG.Models;

namespace TactileGame
{
    class DialogueController
    {
        private InputState lastState;
        private GameInput input;
        private GameDialogue model;


        

        internal void Update()
        {
            if (model.HasPhrase())
            {
                Game.gameState = RPG.GameState.Dialogue;
            }

            if (Game.gameState == RPG.GameState.Dialogue)
            {
                InputState inputState = input.GetState();
                
                if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                {
                    if (model.HasNext())
                    {
                        model.SetNext();
                    }
                    else
                    {
                        model.SetDialogue(null);
                        Game.gameState = RPG.GameState.Exploration;
                    }
                }

                lastState = inputState;
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

        internal void SetModel(GameDialogue model)
        {
            this.model = model;
        }
    }
}


