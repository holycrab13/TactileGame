using System;
using System.Collections.Generic;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class GotoLevel : ActionBase 
	{	 
        public string targetLevel;

        public int targetX;

        public int targetY;

        private bool complete;

        public GotoLevel()
        {

        }

        public override bool IsComplete()
		{
            return complete;
		}

        public override void Update(LevelController controller)
		{
            Game.gameScreen.GoToLevel(targetLevel, targetX, targetY);
            complete = true;
		}

        public override void Reset()
        {
            
        }
	}
}
