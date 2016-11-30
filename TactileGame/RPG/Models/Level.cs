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

        public Character avatar;

        public Level()
        {
            Objects = new List<WorldObject>();
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
				if (worldObject.Overlaps(x, y, character.Width, character.Height))
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
	}
}
