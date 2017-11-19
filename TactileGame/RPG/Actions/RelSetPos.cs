using System;
using System.Collections.Generic;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class RelSetPos : ActionBase 
	{	 
        public string target;

        public int targetX;

        public int targetY;

        private bool complete;

        public RelSetPos()
        {

        }

        public override bool IsComplete()
		{
            return complete;
		}

        public override void Update(LevelController controller)
		{
            WorldObject targetObject = controller.GetTarget(target);
            WorldObject avatarObject = controller.GetTarget("avatar");

            if(targetObject == null || avatarObject == null)
            {
                // ERROR
                throw new InvalidOperationException("MOVE ACTION WITHOUT TARGET!");
            }

            targetObject.X = avatarObject.X + targetX * Constants.TILE_SIZE;
            targetObject.Y = avatarObject.Y + targetY * Constants.TILE_SIZE;

            complete = true;
		}

        public override void Reset()
        {
            
        }
	}
}
