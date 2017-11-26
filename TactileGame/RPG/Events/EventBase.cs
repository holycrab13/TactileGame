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
    abstract class EventBase
    {
        public string[] conditions;

        public string[] inverseConditions;

        public string id;

        public abstract void Update(LevelController levelController);

        public abstract bool IsComplete();

        public abstract void Reset();

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
