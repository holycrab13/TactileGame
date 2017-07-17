using System;
using System.Drawing;
using TactileGame.RPG.Controller;
using TactileGame.RPG.Events;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class Move : ActionBase 
	{	 
		WorldObject targetObject;

        public Direction direction;

        int speed;

		int steps;

        public string target;

        public Move()
        {
            speed = 2;
        }

        public Move(string target, Direction direction)
        {
            speed = 2;
            this.target = target;
            this.direction = direction;
        }

        public Move(Character character, Direction direction, int speed)
        {
            this.direction = direction;
            this.targetObject = character;
            this.speed = speed;
        }

        public override bool IsComplete()
		{
			return steps >= Constants.TILE_SIZE;
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

            targetObject.Rotation = direction;

			switch (direction)
			{
				case Direction.UP:
                    targetObject.Y -= speed;
					break;
				case Direction.LEFT:
                    targetObject.X -= speed;
					break;
				case Direction.DOWN:
                    targetObject.Y += speed;
					break;
				case Direction.RIGHT:
                    targetObject.X += speed;
					break;
			}

            steps += speed;
		}

        public override void Reset()
        {
            steps = 0;
        }
	}
}
