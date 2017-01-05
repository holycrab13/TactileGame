using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TactileGame.Properties;
using TactileGame.RPG.Models;
using TactileGame.RPG.Util;
using tud.mci.LanguageLocalization;

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
        private static Dictionary<string, WorldObject> worldObjects;


        /// <summary>
        /// Helper dictionary for dialogues
        /// </summary>
        private static Dictionary<string, Dialogue> dialogues;



        /// <summary>
        /// Loads the level from a txt file at a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Level Load(string path, LL ll)
        {
            
            worldObjects = new Dictionary<string, WorldObject>();
            dialogues = new Dictionary<string, Dialogue>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Resources/" + path);


            XmlNode dialogueNode = doc.DocumentElement.SelectSingleNode("definition/dialogues");

            foreach (XmlNode node in dialogueNode.ChildNodes)
            {
                string[] phrases = new string[node.ChildNodes.Count];
                int i = 0;

                foreach (XmlNode phrase in node.ChildNodes)
                {
                    phrases[i++] = phrase.InnerText;
                }

                dialogues.Add(XmlUtil.Get(node, "id", "error"), new Dialogue(phrases));
            }

            XmlNode objectNode = doc.DocumentElement.SelectSingleNode("definition/objects");

            foreach (XmlNode node in objectNode.ChildNodes)
            {
                WorldObject obj = new WorldObject()
                {
                    Name = XmlUtil.Get(node, "name", ll.GetTrans("resource.default.name")),
                    Description = XmlUtil.Get(node, "desc", ll.GetTrans("resource.default.desc")),
                    BlocksPath = XmlUtil.Get(node, "block", true),
                    Texture = BooleanTexture.FromFile("Resources/" + XmlUtil.Get(node, "img", ll.GetTrans("resource.default.texture"))),
                };

                worldObjects.Add(XmlUtil.Get(node, "id", "error"), obj);
            }

            Level result = new Level();

            Character character = new Character()
            {
                BlocksPath = true,
                Id = "avatar",
                Name = "Avatar",
                Description = "This is me!",
                Texture = BooleanTexture.FromFile("Resources/avatar.bmp"),
            };

            result.avatar = character;



            XmlNode decoNode = doc.DocumentElement.SelectSingleNode("decos");

            foreach (XmlNode node in decoNode.ChildNodes)
            {
                WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                result.Objects.Add(new WorldObject()
                {
                    X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                    Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                    Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                    Id = XmlUtil.Get(node, "id", "object"),
                    BlocksPath = blueprint.BlocksPath,
                    Texture = blueprint.Texture,
                    Name = blueprint.Name,
                    Description = blueprint.Description
                });
            }

            XmlNode itemNode = doc.DocumentElement.SelectSingleNode("items");

            foreach (XmlNode node in itemNode.ChildNodes)
            {
                WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                result.Objects.Add(new Item()
                {
                    X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                    Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                    Id = XmlUtil.Get(node, "id", "object"),
                    Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                    BlocksPath = blueprint.BlocksPath,
                    Texture = blueprint.Texture,
                    Name = blueprint.Name,
                    Description = blueprint.Description
                });
            }

            XmlNode containerNode = doc.DocumentElement.SelectSingleNode("containers");

            foreach (XmlNode node in containerNode.ChildNodes)
            {
                WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                Container container = new Container()
                {
                    Id = XmlUtil.Get(node, "id", "object"),
                    X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                    Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                    Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                    BlocksPath = blueprint.BlocksPath,
                    Texture = blueprint.Texture,
                    Name = blueprint.Name,
                    Description = blueprint.Description,
                    IsLocked = Boolean.Parse(node.Attributes["locked"].Value),
                    KeyIds = XmlUtil.Get(node, "key", new char[] { ' ' })
                };

                foreach (XmlNode item in node.ChildNodes)
                {
                    WorldObject itemBlueprint = worldObjects[XmlUtil.Get(item, "obj", "default")];

                    container.Items.Add(new Item()
                    {
                        Id = XmlUtil.Get(node, "id", "object"),
                        X = container.X,
                        Y = container.Y,
                        Rotation = container.Rotation,
                        BlocksPath = itemBlueprint.BlocksPath,
                        Texture = itemBlueprint.Texture,
                        Name = itemBlueprint.Name,
                        Description = itemBlueprint.Description
                    });
                }

                result.Objects.Add(container);
            }

             XmlNode npcNode = doc.DocumentElement.SelectSingleNode("npcs/talkers");

             foreach (XmlNode node in npcNode.ChildNodes)
             {
                 WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                 Talker talker = new Talker()
                 {
                     Id = XmlUtil.Get(node, "id", "object"),
                     X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                     Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                     Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                     Dialogue = dialogues[XmlUtil.Get(node, "dialogue", "default")],
                     BlocksPath = blueprint.BlocksPath,
                     Texture = blueprint.Texture,
                     Name = blueprint.Name,
                     Description = blueprint.Description,
                 };

                 result.Objects.Add(talker);
             }


            return result;
        }
    }
}
