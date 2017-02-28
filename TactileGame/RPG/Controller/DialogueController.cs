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
        private DialogueModel model;

        internal void Update()
        {
            if (model.HasPhrase())
            {
                Game.gameState = RPG.GameState.Dialogue;
            }

            if (Game.gameState == RPG.GameState.Dialogue)
            {
                InputState inputState = input.GetState();

                if (model.HasSentence())
                {
                    if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                    {
                        model.FireEvent();

                        if (model.HasNext())
                        {
                            model.SetNext();
                        }
                        else
                        {
                            model.Clear();
                            Game.gameState = RPG.GameState.Exploration;
                        }
                    }
                }
                else
                {
                    if (!inputState.IsKeyDown(InputButton.UP) && lastState.IsKeyDown(InputButton.UP))
                    {
                        Answer answer = model.GetQuestion().GetAnswer(0);

                        readAnswer(answer);
                        model.SetAnswer(answer);
                    }

                    if (!inputState.IsKeyDown(InputButton.RIGHT) && lastState.IsKeyDown(InputButton.RIGHT))
                    {
                        Answer answer = model.GetQuestion().GetAnswer(1);

                        readAnswer(answer);
                        model.SetAnswer(answer);
                    }

                    if (!inputState.IsKeyDown(InputButton.DOWN) && lastState.IsKeyDown(InputButton.DOWN))
                    {
                        Answer answer = model.GetQuestion().GetAnswer(2);

                        readAnswer(answer);
                        model.SetAnswer(answer);
                    }

                    if (!inputState.IsKeyDown(InputButton.LEFT) && lastState.IsKeyDown(InputButton.LEFT))
                    {
                        Answer answer = model.GetQuestion().GetAnswer(3);

                        readAnswer(answer);
                        model.SetAnswer(answer);
                    }

                    if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                    {
                        if (model.GetAnswer() != null)
                        {
                            model.GetAnswer().Fire();

                            model.SetDialogue(model.GetAnswer());
                        }
                        else
                        {
                            Game.audio.PlaySound("Wähle eine Antwort aus.");
                        }
                    }
                }

                lastState = inputState;
            }
          
           
        }

        private void readAnswer(Answer answer)
        {
           if(answer != null)
           {
               Game.audio.PlaySound(answer.text);
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
    }
}


