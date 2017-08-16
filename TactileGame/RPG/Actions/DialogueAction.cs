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
    /// A dialogue
    /// </summary>
    abstract class DialogueAction : ActionBase
    {
        private bool pushed;

        private bool isComplete;

        public abstract bool Complete(LevelModel level);

        public bool Boop(LevelModel level)
        {
            isComplete = Complete(level);
            return isComplete;
        }

        public abstract string GetText();

        public override void Update(LevelController levelController)
        {
            if (!pushed)
            {
                levelController.GetDialogue().SetDialogue(this);
            }
        }

        public override bool IsComplete()
        {
            return isComplete;
        }

        protected void ApplySetsAndClears()
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

        public override void Reset()
        {
            pushed = false;
            isComplete = false;
        }
    }
}
