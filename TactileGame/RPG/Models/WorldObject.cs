
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
        /// The object id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The object description
        /// </summary>
        public string Description { get; set; }

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
        /// The Id of the event being fired on interaction
        /// </summary>
        public string Trigger { get; set; }

        /// <summary>
        /// The dialogue to display on interaction
        /// </summary>
        public EventBase Event { get; set; }

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

        public BooleanTexture Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                CreatePattern();
            }
        }

        /// <summary>
        /// The current object rotation
        /// </summary>
        private Direction rotation;

        private BooleanTexture texture;

        public WorldObject()
        {
            rotation = Direction.DOWN;
        }

        /// <summary>
        /// Creates the pattern from the base pattern with the current rotation
        /// </summary>
        protected void CreatePattern()
        {
            if (texture != null)
            {
                switch (rotation)
                {
                    case Direction.DOWN:
                        Pattern = new byte[texture.Data.GetLength(0), texture.Data.GetLength(1)];
                        for (int i = 0; i < texture.Data.GetLength(0); i++)
                        {
                            for (int j = 0; j < texture.Data.GetLength(1); j++)
                            {
                                Pattern[i, j] = texture.Data[i, j];
                            }
                        }
                        break;
                    case Direction.UP:
                        Pattern = new byte[texture.Data.GetLength(0), texture.Data.GetLength(1)];
                        for (int i = 0; i < texture.Data.GetLength(0); i++)
                        {
                            for (int j = 0; j < texture.Data.GetLength(1); j++)
                            {
                                Pattern[i, j] = texture.Data[texture.Data.GetLength(0) - i - 1, texture.Data.GetLength(1) - j - 1];
                            }
                        }
                        break;
                    case Direction.LEFT:
                        Pattern = new byte[texture.Data.GetLength(1), texture.Data.GetLength(0)];
                        for (int i = 0; i < texture.Data.GetLength(1); i++)
                        {
                            for (int j = 0; j < texture.Data.GetLength(0); j++)
                            {
                                Pattern[i, j] = texture.Data[j, texture.Data.GetLength(1) - i - 1];
                            }
                        }
                        break;
                    case Direction.RIGHT:
                        Pattern = new byte[texture.Data.GetLength(1), texture.Data.GetLength(0)];
                        for (int i = 0; i < texture.Data.GetLength(1); i++)
                        {
                            for (int j = 0; j < texture.Data.GetLength(0); j++)
                            {
                                Pattern[i, j] = texture.Data[texture.Data.GetLength(1) - j - 1, i];
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
		public bool Collides(int x, int y, int width, int height)
		{
			return BlocksPath && (x <= X + Width - 1 && X <= x + width - 1) && (y <= Y + Height - 1 && Y <= y + height - 1);
		}

        /// <summary>
        /// Checks whether the object overlaps the passed object
        /// </summary>
        /// <param name="worldObject"></param>
        /// <returns></returns>
        public bool Collides(WorldObject worldObject)
        {
            return Collides(worldObject.X, worldObject.X, worldObject.Width, worldObject.Height);
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
            return (x <= X + Width - 1 && X <= x + width - 1) && (y <= Y + Height - 1 && Y <= y + height - 1);
        }

    }
}
