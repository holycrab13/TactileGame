﻿using System;
using System.Collections.Generic;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class Goto : ActionBase 
	{
        WorldObject targetObject;

        List<Move> movements;

        public string target;

        public int targetX;

        public int targetY;

        public Goto()
        {

        }

        public override bool IsComplete()
		{
            return movements != null && movements.Count == 0;
		}

        public override void Update(LevelController controller)
		{
            if (targetObject == null)
            {
                targetObject = controller.GetTarget(target);
            }

            if(targetObject == null)
            {
                // ERROR
                throw new InvalidOperationException("MOVE ACTION WITHOUT TARGET!");
            }

            if(movements == null)
            {
                createMovements();
            }

            if (movements.Count > 0)
            {
                Move movement = movements[0];

                if (movement.IsComplete())
                {
                    movements.RemoveAt(0);
                }
                else
                {
                    movement.Update(controller);
                }
            }
		}

        public override void Reset()
        {
            movements = null;
        }

        private void createMovements()
        {
            movements = new List<Move>();

            int diffX = (targetObject.X - targetX * Constants.TILE_SIZE) / Constants.TILE_SIZE;
            int diffY = (targetObject.Y - targetY * Constants.TILE_SIZE) / Constants.TILE_SIZE;

            while(diffX != 0)
            {
                if(diffX > 0)
                {
                    movements.Add(new Move(target, Direction.LEFT));
                    diffX--;
                }

                if (diffX < 0)
                {
                    movements.Add(new Move(target, Direction.RIGHT));
                    diffX++;
                }
            }

            while (diffY != 0)
            {
                if (diffY > 0)
                {
                    movements.Add(new Move(target, Direction.UP));
                    diffY--;
                }

                if (diffY < 0)
                {
                    movements.Add(new Move(target, Direction.DOWN));
                    diffY++;
                }
            }
        }
    }
}
