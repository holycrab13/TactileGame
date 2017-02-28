using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// An event that changes knowledge in the game
    /// </summary>
    class Event
    {
        /// <summary>
        /// The list of facts that is going to be set by the event
        /// </summary>
        public string[] sets;

        /// <summary>
        /// The list of facts that is going to be cleared by the event
        /// </summary>
        public string[] clears;

        /// <summary>
        /// OPTIONAL: The event that will be executed after this event
        /// </summary>
        public string nextEvent;

        /// <summary>
        /// An array of preliminaries for this event
        /// </summary>
        public string[] conditions;

        public void Fire()
        {
            if (sets != null)
            {
                foreach (string set in sets)
                {
                    if (Game.knowledge.ContainsKey(set))
                    {
                        Game.knowledge[set] = true;
                    }
                    else
                    {
                        Game.knowledge.Add(set, true);
                    }
                }
            }

            if (clears != null)
            {
                foreach (string clear in clears)
                {
                    if (Game.knowledge.ContainsKey(clear))
                    {
                        Game.knowledge[clear] = false;
                    }
                    else
                    {
                        Game.knowledge.Add(clear, false);
                    }
                }
            }
        }

        internal bool IsAvailable()
        {
            foreach (string condition in conditions)
            {
                if (!Game.HasKnowledge(condition))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
