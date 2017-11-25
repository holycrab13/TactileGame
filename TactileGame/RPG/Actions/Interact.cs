using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class Interact : ActionBase 
	{
        public string target;

        private bool complete;

        public Interact()
        {
            this.complete = false;
        }

        public override bool IsComplete()
		{
            return complete;
		}

        public override void Update(LevelController controller)
		{
            WorldObject targetObject = controller.GetTarget(target);

            if (targetObject != null)
            {
                controller.TriggerEvent(targetObject.Trigger);
            }

            complete = false;
		}

        public override void Reset()
        {
            complete = false;
        }
	}
}
