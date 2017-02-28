using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Models
{
    class LevelModel
    {
        public Level level;

        public LevelModel(Level level)
        {
            this.level = level;
        }

        public Character Avatar
        {
            get { return level.avatar; }
        }
    }
}
