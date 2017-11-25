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

        internal void Update()
        {

            if (GameScreen.gameState == RPG.GameState.Event)
            {
                InputState inputState = input.GetState();

                if (model.HasAction())
                {
                    if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                    {
                        if (!model.Boop(level))
                        {
                            AudioRenderer.Instance.AbortCurrentSound();
                            AudioRenderer.Instance.PlaySound("Bitte wähle eine Antwort mit den Pfeiltasten aus.");
                        }
                    }


                    if (model.HasQuestion())
                    {

                        if (!inputState.IsKeyDown(InputButton.UP) && lastState.IsKeyDown(InputButton.UP))
                        {
                            if (model.GetQuestion().SetAnswer(0))
                            {
                                readAnswer(model.GetQuestion().GetAnswerText());
                            }
                        }

                        if (!inputState.IsKeyDown(InputButton.RIGHT) && lastState.IsKeyDown(InputButton.RIGHT))
                        {
                            if (model.GetQuestion().SetAnswer(1))
                            {
                                readAnswer(model.GetQuestion().GetAnswerText());
                            }
                        }

                        if (!inputState.IsKeyDown(InputButton.DOWN) && lastState.IsKeyDown(InputButton.DOWN))
                        {
                            if (model.GetQuestion().SetAnswer(2))
                            {
                                readAnswer(model.GetQuestion().GetAnswerText());
                            }
                        }

                        if (!inputState.IsKeyDown(InputButton.LEFT) && lastState.IsKeyDown(InputButton.LEFT))
                        {
                            if (model.GetQuestion().SetAnswer(3))
                            {
                                readAnswer(model.GetQuestion().GetAnswerText());
                            }
                        }
                    }
                }

                lastState = inputState;
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


