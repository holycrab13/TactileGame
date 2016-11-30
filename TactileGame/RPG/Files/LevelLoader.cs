using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactileGame.Properties;
using TactileGame.RPG.Models;

namespace TactileGame.RPG.Files
{
    /// <summary>
    /// Loads the level
    /// </summary>
    class LevelLoader
    {
        /// <summary>
        /// Helper dictionary for world objects
        /// </summary>
        private static Dictionary<int, WorldObject> worldObjects;

        /// <summary>
        /// Helper dictionary for dialogues
        /// </summary>
        private static Dictionary<int, Dialogue> dialogues;

        /// <summary>
        /// Loads the level from a txt file at a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Level Load(string path)
        {
            worldObjects = new Dictionary<int, WorldObject>();
            dialogues = new Dictionary<int, Dialogue>();

            string[] lines = System.IO.File.ReadAllLines("Resources/" + path);

            Level level = new Level();
            try
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith("player"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 5);

                        BooleanTexture texture = BooleanTexture.FromBitmap((Bitmap)Image.FromFile("Resources/" + tokens[4], true));
                        Character obj = new Character(texture.Data);

                        obj.BlocksPath = true;
                        obj.Name = "Avatar";
                        obj.Description = new Dialogue("This is me!");

                        obj.X = Constants.TILE_SIZE * Int32.Parse(tokens[1]);
                        obj.Y = Constants.TILE_SIZE * Int32.Parse(tokens[2]);
                        obj.Rotation = (Direction)Int32.Parse(tokens[3]);

                        level.avatar = obj;
                    }

                    if (line.StartsWith("o"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 6);

                        BooleanTexture texture = BooleanTexture.FromBitmap((Bitmap)Image.FromFile("Resources/" + tokens[3], true));
                        WorldObject obj = new WorldObject(texture.Data);

                        obj.BlocksPath = Boolean.Parse(tokens[4]);
                        obj.Name = tokens[2];
                        obj.Description = new Dialogue(tokens[5]);

                        worldObjects.Add(Int32.Parse(tokens[1]), obj);

                    }

                    if (line.StartsWith("d"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 3);
                        string[] phrases = tokens[2].Split(new[] { '#' });
                        dialogues.Add(Int32.Parse(tokens[1]), new Dialogue(phrases));
                    }

                    if (line.StartsWith("i"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 5);
                        WorldObject blueprint = worldObjects[Int32.Parse(tokens[1])];

                        WorldObject instance = new WorldObject(blueprint.Pattern)
                        {
                            BlocksPath = blueprint.BlocksPath,
                            Name = blueprint.Name,
                            Description = blueprint.Description
                        };

                        instance.X = Constants.TILE_SIZE * Int32.Parse(tokens[2]);
                        instance.Y = Constants.TILE_SIZE * Int32.Parse(tokens[3]);
                        instance.Rotation = (Direction)Int32.Parse(tokens[4]);
                       
                        level.Objects.Add(instance);
                    }

                    if (line.StartsWith("talker"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 8);

                        BooleanTexture texture = BooleanTexture.FromBitmap((Bitmap)Image.FromFile("Resources/" + tokens[6], true));
                        Dialogue dialogue = dialogues[Int32.Parse(tokens[5])];
                        Talker npc = new Talker(texture.Data, dialogue)
                        {
                            Name = tokens[1],
                            X = Constants.TILE_SIZE * Int32.Parse(tokens[2]),
                            Y = Constants.TILE_SIZE * Int32.Parse(tokens[3]),
                            Rotation = (Direction)Int32.Parse(tokens[4]),
                            Description = new Dialogue(tokens[7]),
                            BlocksPath = true
                        };

                        level.Objects.Add(npc);
                    }

                    if (line.StartsWith("shop"))
                    {
                        string[] tokens = line.Split(new[] { ' ' }, 8);

                        BooleanTexture texture = BooleanTexture.FromBitmap((Bitmap)Image.FromFile("Resources/" + tokens[6], true));
                        Shopkeeper npc = new Shopkeeper(texture.Data)
                        {
                            Name = tokens[1],
                            X = Constants.TILE_SIZE * Int32.Parse(tokens[2]),
                            Y = Constants.TILE_SIZE * Int32.Parse(tokens[3]),
                            Rotation = (Direction)Int32.Parse(tokens[4]),
                            Description = new Dialogue(tokens[7]),
                            BlocksPath = true
                        };

                        level.Objects.Add(npc);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return level;
        }
    }
}
