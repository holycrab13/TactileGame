using System;
using System.Drawing;
using TactileGame.RPG.Models;

namespace TactileGame.RPG
{
	class Movement 
	{	 
		Character character;

		Direction direction;

        private Level level;

        int speed;

		int steps;

        private bool validated;

        public Movement(Character character, Level level, Direction direction, int speed)
        {
            this.direction = direction;
            this.character = character;
            this.level = level;
            this.speed = speed;
        }

		public bool IsComplete()
		{
			return steps >= Constants.TILE_SIZE;
		}

		public void Update()
		{
            character.Rotation = direction;

            if (!validated)
            {
                validated = true;
                Rectangle rec = character.GetLookAt();
                if (!level.IsValidPosition(character, rec))
                {
                    steps = Constants.TILE_SIZE;
                    return;
                }
            }

			switch (direction)
			{
				case Direction.UP:
                    character.Y -= speed;
					break;
				case Direction.LEFT:
                    character.X -= speed;
					break;
				case Direction.DOWN:
                    character.Y += speed;
					break;
				case Direction.RIGHT:
                    character.X += speed;
					break;
			}

            steps += speed;
		}
	}
}
