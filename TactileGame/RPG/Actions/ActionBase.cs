using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Controller;

namespace TactileGame.RPG.Events
{
    abstract class ActionBase
    {
        public string[] sets;

        public string[] clears;

        public abstract void Update(LevelController levelController);

        public abstract bool IsComplete();

        public virtual void Reset()
        {

        }
    }
}
