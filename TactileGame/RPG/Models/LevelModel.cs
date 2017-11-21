﻿using System;
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

        public List<EventBase> events;

        public LevelModel(Level level)
        {
            this.level = level;
            this.events = new List<EventBase>();
        }

        public Character Avatar
        {
            get { return level.avatar; }
        }

        internal void TriggerEvent(string p)
        {
            EventBase e = level.FindEvent(p);

            if(e != null)
            {
                events.Add(e);
            }
        }

        internal void TriggerEventAt(int p1, int p2)
        {
            EventTrigger eventTrigger = level.FindTrigger(p1, p2);

            if(eventTrigger != null)
            {
                TriggerEvent(eventTrigger.levelEvent);
            }
        }
    }
}
