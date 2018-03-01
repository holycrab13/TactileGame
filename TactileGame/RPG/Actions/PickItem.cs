using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class PickItem : ActionBase 
	{
        private bool complete;

        private Item target;

        public PickItem(Item target)
        {
            this.target = target;
            this.complete = false;
        }

        public override bool IsComplete()
		{
            return complete;
		}

        public override void Update(LevelController controller)
		{
            if (target != null)
            {
                controller.UpdateItem(target);
            }

            complete = true;
		}

        public override void Reset()
        {
            complete = false;
        }
	}
}
