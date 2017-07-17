using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// An event that changes knowledge in the game
    /// </summary>
    class Event : EventBase
    {
        public List<ActionBase> actions;

        public string[] conditions;

        public string[] inverseConditions;

        private int index;

        public Event()
        {
            actions = new List<ActionBase>();
        }

        public Event(params ActionBase[] actions)
        {
            this.actions = new List<ActionBase>();
            this.actions.AddRange(actions);
        }

        public override void Update(LevelController levelController)
        {
            ActionBase action = actions[index];

            if (action.IsComplete())
            {
                index++;
            }
            else
            {
                action.Update(levelController);
            }
        }

        public override bool IsComplete()
        {
            return index >= actions.Count;
        }

        public override void Reset()
        {
            foreach(ActionBase action in actions)
            {
                action.Reset();
            }

            index = 0;
        }

        public bool IsAvailable()
        {
            foreach (string condition in conditions)
            {
                if (!Game.HasKnowledge(condition))
                {
                    return false;
                }
            }

            foreach (string condition in inverseConditions)
            {
                if (Game.HasKnowledge(condition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
