using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
    /// <summary>
    /// Triggert direkt ein Event! Vorsicht!!! Nicht auf sich selbst feuern oder ALLES IST AUUUUUSSS
    /// </summary>
	class Trigger : ActionBase 
	{
        public string trigger;
        private bool complete;

        public Trigger()
        {
            this.complete = false;
        }

        public override bool IsComplete()
		{
            return complete;
		}

        public override void Update(LevelController controller)
		{
            controller.TriggerEvent(trigger);

            complete = true;
		}

        public override void Reset()
        {
            complete = false;
        }
	}
}
