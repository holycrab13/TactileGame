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
        private LevelModel model;

        /// <summary>
        /// The game input model
        /// </summary>
        private GameInput gameInput;

        /// <summary>
        /// The controller can start dialogues
        /// </summary>
        private DialogueModel gameDialogue;

        /// <summary>
        /// The last input state
        /// </summary>
        private InputState lastState;

        private Game game;

        public LevelController(Game game)
        {
            this.game = game;
        }
        /// <summary>
        /// Updates the controller
        /// </summary>
        internal void Update()
        {
            InputState inputState = gameInput.GetState();

            Game.gameState = model.events.Count > 0 ? GameState.Event : GameState.Exploration;

            if (Game.gameState == GameState.Exploration)
            {
                if (!inputState.IsKeyDown(InputButton.A) && lastState.IsKeyDown(InputButton.A))
                {
                    // Find the target of the A action
                    WorldObject target = model.level.GetTarget(model.level.avatar);

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
                        else if(target is Door)
                        {
                            UpdateDoor(target as Door);
                        }
                        else
                        {
                            UpdateWorldObject(target);
                        }
                    }
                }

               
            }

            if (Game.gameState == GameState.Event)
            {
                EventBase levelEvent = model.events[0];

                if (levelEvent.IsComplete())
                {
                    levelEvent.Reset();
                    gameDialogue.Clear();
                    model.events.RemoveAt(0);
                }
                else
                {
                    levelEvent.Update(this);
                }
            }
           
            lastState = inputState;
        }

        private void UpdateDoor(Door door)
        {
            game.GoToLevel(door.Target, door.TargetX, door.TargetY);
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
            model.events.Add(item.Event);
            model.level.Objects.Remove(item);
            model.level.avatar.Inventory.Add(item);
            
        }

        /// <summary>
        /// Updates world objects (show their description in the dialog)
        /// </summary>
        /// <param name="target"></param>
        private void UpdateWorldObject(WorldObject target)
        {
            model.events.Add(target.Event);
        }

        /// <summary>
        /// Update NPCs (rotate and do their specific action)
        /// </summary>
        /// <param name="target"></param>
        private void UpdateNPC(NPC target)
        {
            switch (model.level.avatar.Rotation)
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

            model.TriggerEvent(target.Trigger);
        }

        /// <summary>
        /// Sets the model
        /// </summary>
        /// <param name="model"></param>
        internal void SetModel(LevelModel model)
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
        internal void SetDialogue(DialogueModel gameDialogue)
        {
            this.gameDialogue = gameDialogue;
        }

        public DialogueModel GetDialogue()
        {
            return gameDialogue;
        }

        internal Level GetLevel()
        {
            return model.level;
        }

        internal WorldObject GetTarget(string target)
        {
            if (target.Equals("avatar") || target.Equals(string.Empty))
            {
                return GetLevel().avatar;
            }
            else
            {
                return GetLevel().FindObject<WorldObject>(target);
            }
        }
    }
}
