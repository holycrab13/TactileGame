using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Events
{
    abstract class ActionBase
    {
        public string[] sets;

        public string[] clears;

        public abstract void Update(LevelController levelController);

        public abstract bool IsComplete();

        protected void ApplySetsAndClears()
        {
            if (sets != null)
            {
                foreach (string set in sets)
                {
                    if (Game.knowledge.ContainsKey(set))
                    {
                        if (!Game.knowledge[set])
                        {
                            Game.knowledge[set] = true;
                            LevelController.OnKnowledgeChanged();
                        }
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

        public virtual void Reset()
        {

        }
    }
}
