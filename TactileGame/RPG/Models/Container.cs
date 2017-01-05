using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class Container : WorldObject
    {
        public bool IsLocked { get; set; }

        public string[] KeyIds { get; set; }

        public List<Item> Items { get; private set; }

        public Container()
        {
            Items = new List<Item>();
        }
    }
}
