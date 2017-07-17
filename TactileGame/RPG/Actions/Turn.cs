using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class Turn : ActionBase 
	{	 
		WorldObject targetObject;

        public Direction direction;

        public string target;

        private bool isComplete;

        public Turn()
        {

        }

        public override bool IsComplete()
		{
            return isComplete;
		}

        public override void Update(LevelController controller)
		{
            if (targetObject == null)
            {
                targetObject = controller.GetTarget(target);
            }

            if(targetObject == null)
            {
                // ERROR
                return;
            }

            targetObject.Rotation = direction;

            isComplete = true;
		}

        public override void Reset()
        {
            isComplete = false;
        }
	}
}
