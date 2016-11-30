
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Models
{
    /// <summary>
    /// An object in the game world
    /// </summary>
    class WorldObject
    {
        /// <summary>
        /// The object name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The object description
        /// </summary>
        public Dialogue Description { get; set; }

        /// <summary>
        /// The texture of the object
        /// </summary>
        public byte[,] Pattern { get; private set; }

        /// <summary>
        /// The x position of the object in the world
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// The y position of the object in the world
        /// </summary>
		public int Y { get; set; }

        /// <summary>
        /// The width of the object
        /// </summary>
        public int Width { get { return Pattern.GetLength(0); } }

        /// <summary>
        /// The height of the object
        /// </summary>
        public int Height { get { return Pattern.GetLength(1); } }

        /// <summary>
        /// Indicates whether characters can walk through the object or not
        /// </summary>
        public bool BlocksPath { get; set; }

        /// <summary>
        /// The getter and setter for the current rotation
        /// </summary>
        public Direction Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                // If the rotation changed, the pattern will be rewritten
                if (value != rotation)
                {
                    rotation = value;
                    CreatePattern();
                }
            }
        }

        /// <summary>
        /// The texture in the base rotation
        /// </summary>
        protected byte[,] basePattern;

        /// <summary>
        /// The current object rotation
        /// </summary>
        private Direction rotation;

        /// <summary>
        /// Creates a world object from texture
        /// </summary>
        /// <param name="texture"></param>
        public WorldObject(byte[,] pattern)
        {
            rotation = Direction.DOWN;
            basePattern = pattern;
            CreatePattern();
        }

        public WorldObject()
        {
            rotation = Direction.DOWN;
        }

        /// <summary>
        /// Creates the pattern from the base pattern with the current rotation
        /// </summary>
        protected void CreatePattern()
        {
            if (basePattern != null)
            {
                switch (rotation)
                {
                    case Direction.DOWN:
                        Pattern = new byte[basePattern.GetLength(0), basePattern.GetLength(1)];
                        for (int i = 0; i < basePattern.GetLength(0); i++)
                        {
                            for (int j = 0; j < basePattern.GetLength(1); j++)
                            {
                                Pattern[i, j] = basePattern[i, j];
                            }
                        }
                        break;
                    case Direction.UP:
                        Pattern = new byte[basePattern.GetLength(0), basePattern.GetLength(1)];
                        for (int i = 0; i < basePattern.GetLength(0); i++)
                        {
                            for (int j = 0; j < basePattern.GetLength(1); j++)
                            {
                                Pattern[i, j] = basePattern[i, basePattern.GetLength(1) - j - 1];
                            }
                        }
                        break;
                    case Direction.LEFT:
                        Pattern = new byte[basePattern.GetLength(1), basePattern.GetLength(0)];
                        for (int i = 0; i < basePattern.GetLength(1); i++)
                        {
                            for (int j = 0; j < basePattern.GetLength(0); j++)
                            {
                                Pattern[i, j] = basePattern[j, basePattern.GetLength(1) - i - 1];
                            }
                        }
                        break;
                    case Direction.RIGHT:
                        Pattern = new byte[basePattern.GetLength(1), basePattern.GetLength(0)];
                        for (int i = 0; i < basePattern.GetLength(1); i++)
                        {
                            for (int j = 0; j < basePattern.GetLength(0); j++)
                            {
                                Pattern[i, j] = basePattern[j, i];
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Checks whether the object overlaps the passed rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
		public bool Overlaps(int x, int y, int width, int height)
		{
			return BlocksPath && (x <= X + Width - 1 && X <= x + width - 1) && (y <= Y + Height - 1 && Y <= y + height - 1);
		}

        /// <summary>
        /// Checks whether the object overlaps the passed object
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public bool Overlaps(WorldObject worldObject)
        {
            return Overlaps(worldObject.X, worldObject.X, worldObject.Width, worldObject.Height);
        }

    }
}
