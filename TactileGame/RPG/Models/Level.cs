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

        public Character Avatar { get; set; }

        public string Name { get; set; }

        public string OnLoadTrigger { get; set; }

        public Level()
        {
            Objects = new List<WorldObject>();
            Events = new List<EventBase>();
            Triggers = new List<EventTrigger>();
        }

		internal WorldObject GetTarget(Character character)
		{
            Rectangle lookAt = character.GetLookAt();
            List<WorldObject> targets = new List<WorldObject>();

            foreach (WorldObject worldObject in Objects)
            {
                if (worldObject.Overlaps(lookAt.X, lookAt.Y, lookAt.Width, lookAt.Height))
                {
                    targets.Add(worldObject);
                }
            }

            targets = targets.OrderByDescending(t => t.Priority).ToList();

            return targets.FirstOrDefault();
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

        internal void OnKnowledgeChanged()
        {
            foreach(WorldObject worldObject in Objects)
            {
                if (Game.HasKnowledge(worldObject.Conditions, worldObject.InverseConditions))
                {
                    worldObject.isHidden = false;
                }
                else
                {
                    worldObject.isHidden = true;
                }
            }
        }
    }
}
