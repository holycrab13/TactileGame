using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Events;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// An event that changes knowledge in the game
    /// </summary>
    class EventGroup : EventBase
    {
        public List<Event> events;

        private Event currentEvent;

        private int index;

        public EventGroup()
        {
            events = new List<Event>();
        }

        public override void Update(Controller.LevelController levelController)
        {
            if(currentEvent == null)
            {
                while (index < events.Count && !events[index].IsAvailable())
                {
                    index++;
                }

                if(index < events.Count)
                {
                    currentEvent = events[index];
                }
            }

            if(currentEvent != null && !currentEvent.IsComplete())
            {
                currentEvent.Update(levelController);
            }

            if (currentEvent != null && currentEvent.IsComplete())
            {
                currentEvent = null;
                index++;
            }
        }

        public override bool IsComplete()
        {
            return index >= events.Count;
        }

        public override void Reset()
        {
            foreach(Event e in events)
            {
                e.Reset();
            }

            index = 0;
        }
    }
}
