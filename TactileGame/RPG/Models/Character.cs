
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Models
{
    class Character : WorldObject
    {
        public List<Item> Inventory { get; private set; }

        public Character() 
        {
            Inventory = new List<Item>();
        }

        internal Rectangle GetLookAt()
        {
            switch (Rotation)
            {
                case Direction.DOWN:
                    return new Rectangle(X, Y + Constants.TILE_SIZE, Width, Height);
                case Direction.UP:
                    return new Rectangle(X, Y - Constants.TILE_SIZE, Width, Height);
                case Direction.LEFT:
                    return new Rectangle(X - Constants.TILE_SIZE, Y, Width, Height);
                case Direction.RIGHT:
                    return new Rectangle(X + Constants.TILE_SIZE, Y, Width, Height);
            }

            return new Rectangle();
        }

      
    }
}
