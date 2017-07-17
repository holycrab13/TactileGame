using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Level
    {
        public List<WorldObject> Objects { get; private set; }

        public List<EventBase> Events { get; private set; }

        public List<EventTrigger> Triggers { get; private set; }

        public Character avatar;

        public Level()
        {
            Objects = new List<WorldObject>();
            Events = new List<EventBase>();
            Triggers = new List<EventTrigger>();
        }

		internal WorldObject GetTarget(Character character)
		{
            Rectangle lookAt = character.GetLookAt();

            foreach (WorldObject worldObject in Objects)
            {
                if (worldObject.Overlaps(lookAt.X, lookAt.Y, lookAt.Width, lookAt.Height))
                {
                    return worldObject;
                }
            }

            return null;
		}

		internal bool IsValidPosition(Character character, int x, int y)
		{
			foreach (WorldObject worldObject in Objects)
			{
				if (worldObject.Collides(x, y, character.Width, character.Height))
				{
					return false;
				}
			}

			return true;
		}

        internal bool IsValidPosition(Character character, Rectangle rec)
        {
            return IsValidPosition(character, rec.X, rec.Y);
        }

        internal EventBase FindEvent(string p)
        {
            return Events.Find(e => e.id == p);
        }

        internal T FindObject<T>(string p) where T : WorldObject
        {
            return Objects.Find(e => e.Id == p) as T;
        }

        internal EventTrigger FindTrigger(int x, int y)
        {
            return Triggers.Find(t => x >= t.x && x < t.x + t.width && y >= t.y && y < t.y + t.height);
        }
    }
}
