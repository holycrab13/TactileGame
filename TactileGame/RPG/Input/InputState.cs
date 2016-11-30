using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TactileGame.RPG.Input
{
    class InputState
    {
        internal Dictionary<InputButton, bool> Keystates = new Dictionary<InputButton, bool>();

        public bool IsKeyDown(InputButton key)
        {
            return Keystates[key];
        }
    }
}
