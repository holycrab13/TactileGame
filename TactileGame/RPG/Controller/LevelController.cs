using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Input;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Controller
{
    /// <summary>
    /// Handles all the stuff going on in the level
    /// </summary>
    class LevelController
    {
        /// <summary>
        /// The level model
        /// </summary>
        private Level model;

        /// <summary>
        /// The game input model
        /// </summary>
        private GameInput gameInput;

        /// <summary>
        /// The controller can start dialogues
        /// </summary>
        private GameDialogue gameDialogue;

        /// <summary>
        /// The last input state
        /// </summary>
        private InputState lastState;

        /// <summary>
        /// Updates the controller
        /// </summary>
        internal void Update()
        {
            InputState inputState = gameInput.GetState();

            if (Game.gameState == GameState.Exploration)
            {
  
                if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                {
                    // Find the target of the A action
                    WorldObject target = model.GetTarget(model.avatar);

                    if (target != null)
                    {
                        // Do a target specific action
                        if (target is NPC)
                        {
                            UpdateNPC(target as NPC);
                        }
                        else if (target is Item)
                        {
                            UpdateItem(target as Item);
                        }
                        else if (target is Container)
                        {
                            UpdateContainer(target as Container);
                        }
                        else
                        {
                            UpdateWorldObject(target);
                        }
                    }
                }
            }

            lastState = inputState;
        }

        private void UpdateContainer(Container container)
        {
            //if (container.IsLocked)
            //{
            //    Item key = model.avatar.Inventory.Find(item => container.KeyIds.Contains(item.Id));

            //    if (key == null)
            //    {
            //        gameDialogue.SetDialogue(Sentence.Create(container.Description + " Sie ist verschlossen."));
            //    }
            //    else
            //    {
            //        gameDialogue.SetDialogue(Sentence.Create("Du hast " + container.Name + " mit " + key.Name + " aufgeschlossen"));
            //        container.IsLocked = false;
            //    }
            //}
            //else
            //{
            //    if (container.Items.Count == 0)
            //    {
            //        gameDialogue.SetDialogue(Sentence.Create(container.Description + " Sie ist leer."));
            //    }
            //    else
            //    {
            //        string[] phrases = new string[container.Items.Count + 1];
            //        phrases[0] = container.Description;

            //        for (int i = 0; i < container.Items.Count; i++)
            //        {
            //            phrases[i + 1] = "Du hast " + container.Items[i].Name + " gefunden!";
            //        }

            //        gameDialogue.SetDialogue(Sentence.Create(phrases));
            //        model.avatar.Inventory.AddRange(container.Items);
            //        container.Items.Clear();
            //    }
            //}
        }

        private void UpdateItem(Item item)
        {
            gameDialogue.SetDialogue(item.Dialogues[0]);
            model.Objects.Remove(item);
            model.avatar.Inventory.Add(item);
            
        }

        /// <summary>
        /// Updates world objects (show their description in the dialog)
        /// </summary>
        /// <param name="target"></param>
        private void UpdateWorldObject(WorldObject target)
        {
            gameDialogue.SetDialogue(Sentence.Create(target.Description));
        }

        /// <summary>
        /// Update NPCs (rotate and do their specific action)
        /// </summary>
        /// <param name="target"></param>
        private void UpdateNPC(NPC target)
        {
            switch (model.avatar.Rotation)
            {
                case Direction.UP:
                    target.Rotation = Direction.DOWN;
                    break;
                case Direction.DOWN:
                    target.Rotation = Direction.UP;
                    break;
                case Direction.LEFT:
                    target.Rotation = Direction.RIGHT;
                    break;
                case Direction.RIGHT:
                    target.Rotation = Direction.LEFT;
                    break;
            }


            gameDialogue.SetDialogue(target.GetDialogue());
            gameDialogue.SetTarget(target);
        }

        /// <summary>
        /// Sets the model
        /// </summary>
        /// <param name="model"></param>
        internal void SetModel(Level model)
        {
            this.model = model;
        }

        /// <summary>
        /// Sets the input model
        /// </summary>
        /// <param name="gameInput"></param>
        internal void SetInput(GameInput gameInput)
        {
            this.gameInput = gameInput;

            if (gameInput != null)
            {
                lastState = gameInput.GetState();
            }
        }

        /// <summary>
        /// Sets the dialog model
        /// </summary>
        /// <param name="gameDialogue"></param>
        internal void SetDialogue(GameDialogue gameDialogue)
        {
            this.gameDialogue = gameDialogue;
        }
    }
}
