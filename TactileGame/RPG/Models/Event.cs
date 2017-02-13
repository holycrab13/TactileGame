using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Event
    {
        public string[] sets;

        public string[] clears;

        public string nextEvent;

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
    }
}
