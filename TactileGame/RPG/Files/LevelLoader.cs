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
        private static Dictionary<string, Phrase[]> dialogues;

        private static Dictionary<string, bool> investigation;



        /// <summary>
        /// Loads the level from a txt file at a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Level Load(string path, LL ll)
        {

            investigation = new Dictionary<string, bool>();
            worldObjects = new Dictionary<string, WorldObject>();
            dialogues = new Dictionary<string, Phrase[]>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Resources/" + path);

            XmlNode investigationNode = doc.DocumentElement.SelectSingleNode("definition/investigation");

            foreach (XmlNode node in investigationNode.ChildNodes)
            {

                string id  = XmlUtil.Get(node, "id", string.Empty);

                if(id != string.Empty)
                {
                    investigation[id] = Boolean.Parse(node.InnerText);
                }
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

                Item item = new Item()
                {
                    X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                    Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                    Id = XmlUtil.Get(node, "id", "object"),
                    Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                    BlocksPath = blueprint.BlocksPath,
                    Texture = blueprint.Texture,
                    Name = blueprint.Name,
                    Description = blueprint.Description,
                    Dialogues = new List<Dialogue>()
                };

                if (!Game.HasKnowledge(item.Id + "_gefunden")) 
                {
                    Dialogue itemDialogue = new Dialogue()
                    {
                        phrases = new Phrase[] 
                    { 
                        new Sentence() 
                        {  
                            text = "Du hast " + blueprint.Name + " gefunden.",
                            sets = new string[] { "hat_" + item.Id, item.Id + "_gefunden" }
                        }
                    }

                    };

                    item.Dialogues.Add(itemDialogue);
                    result.Objects.Add(item);
                }
            }

            //XmlNode containerNode = doc.DocumentElement.SelectSingleNode("containers");

            //foreach (XmlNode node in containerNode.ChildNodes)
            //{
            //    WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

            //    Container container = new Container()
            //    {
            //        Id = XmlUtil.Get(node, "id", "object"),
            //        X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
            //        Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
            //        Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
            //        BlocksPath = blueprint.BlocksPath,
            //        Texture = blueprint.Texture,
            //        Name = blueprint.Name,
            //        Description = blueprint.Description,
            //        IsLocked = Boolean.Parse(node.Attributes["locked"].Value),
            //        KeyIds = XmlUtil.Get(node, "key", new char[] { ' ' })
            //    };

            //    foreach (XmlNode item in node.ChildNodes)
            //    {
            //        WorldObject itemBlueprint = worldObjects[XmlUtil.Get(item, "obj", "default")];

            //        container.Items.Add(new Item()
            //        {
            //            Id = XmlUtil.Get(node, "id", "object"),
            //            X = container.X,
            //            Y = container.Y,
            //            Rotation = container.Rotation,
            //            BlocksPath = itemBlueprint.BlocksPath,
            //            Texture = itemBlueprint.Texture,
            //            Name = itemBlueprint.Name,
            //            Description = itemBlueprint.Description
            //        });
            //    }

            //    result.Objects.Add(container);
            //}

             XmlNode npcNode = doc.DocumentElement.SelectSingleNode("npcs");

             foreach (XmlNode node in npcNode.ChildNodes)
             {
                 WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                 NPC npc = new NPC()
                 {
                     Id = XmlUtil.Get(node, "id", "object"),
                     X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                     Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                     Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                     Dialogues = new List<Dialogue>(),
                     BlocksPath = blueprint.BlocksPath,
                     Texture = blueprint.Texture,
                     Name = blueprint.Name,
                     Description = blueprint.Description,
                 };


                 foreach (XmlNode dialogueNode in node.ChildNodes)
                 {
                     Dialogue dialogue = new Dialogue();
                     dialogue.conditions = XmlUtil.GetArray(dialogueNode, "if", ' ');

                     dialogue.phrases = new Phrase[dialogueNode.ChildNodes.Count];

                     int i = 0;

                     foreach (XmlNode text in dialogueNode.ChildNodes)
                     {
                         if (text.LocalName.Equals("phrase"))
                         {
                             dialogue.phrases[i++] = loadSentence(text);
                         }

                         if (text.LocalName.Equals("question"))
                         {
                             dialogue.phrases[i++] = loadQuestion(text);
                         }
                     }

                     npc.Dialogues.Add(dialogue);
                 }

                 result.Objects.Add(npc);
             }

            return result;
        }

        private static Sentence loadSentence(XmlNode text)
        {
            Sentence result = new Sentence();
            result.text = text.InnerText;

            result.sets = XmlUtil.GetArray(text, "set", ' ');
            result.clears = XmlUtil.GetArray(text, "clear", ' ');
            result.nextEvent = XmlUtil.Get(text, "event", string.Empty);
            return result;
        }

        private static Question loadQuestion(XmlNode text)
        {
            Question result = new Question();
            result.text = XmlUtil.Get(text, "text", string.Empty);

            foreach (XmlNode node in text.ChildNodes)
            {
                result.answers.Add(loadSentence(node));
            }

            result.sets = XmlUtil.GetArray(text, "set", ' ');
            result.clears = XmlUtil.GetArray(text, "clear", ' ');

            return result;
        }

        internal static Dictionary<string, bool> LoadKnowledge(string p, LL ll)
        {
            investigation = new Dictionary<string, bool>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Resources/" + p);

            XmlNode investigationNode = doc.DocumentElement.SelectSingleNode("definition/investigation");

            foreach (XmlNode node in investigationNode.ChildNodes)
            {

                string id = XmlUtil.Get(node, "id", string.Empty);

                if (id != string.Empty)
                {
                    investigation[id] = Boolean.Parse(node.InnerText);
                }
            }

            return investigation;
        }
    }
}
