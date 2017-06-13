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



        /// <summary>
        /// Loads the level from a txt file at a given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Level Load(string path, LL ll)
        {

            worldObjects = new Dictionary<string, WorldObject>();
            dialogues = new Dictionary<string, Phrase[]>();

            // Create XML reader settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;                         // Exclude comments

            // Create reader based on settings
            XmlReader reader = XmlReader.Create("Resources/levels/" + path, settings);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            
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
                Texture = BooleanTexture.FromFile("Resources/bmps/avatar.bmp"),
            };

            result.avatar = character;



            XmlNode decoNode = doc.DocumentElement.SelectSingleNode("decos");

            if (decoNode != null)
            {
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
            }

            XmlNode itemNode = doc.DocumentElement.SelectSingleNode("items");

            if (itemNode != null)
            {
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
                            new Phrase() 
                            {  
                                text = "Du hast " + blueprint.Name + " gefunden.",
                            }
                        },

                            sets = new string[] { "hat_" + item.Id, item.Id + "_gefunden" }

                        };

                        item.Dialogues.Add(itemDialogue);
                        result.Objects.Add(item);
                    }
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

            XmlNode doorNode = doc.DocumentElement.SelectSingleNode("doors");

            if (doorNode != null)
            {
                foreach (XmlNode node in doorNode.ChildNodes)
                {
                    WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

                    Door door = new Door()
                    {
                        X = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0),
                        Y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0),
                        Target = XmlUtil.Get(node, "target", ""),
                        TargetX = Constants.TILE_SIZE * XmlUtil.Get(node, "targetX", 0),
                        TargetY = Constants.TILE_SIZE * XmlUtil.Get(node, "targetY", 0),
                        BlocksPath = blueprint.BlocksPath,
                        Texture = blueprint.Texture,
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                    };

                    result.Objects.Add(door);
                }
            }


            XmlNode npcNode = doc.DocumentElement.SelectSingleNode("npcs");

            if (npcNode != null)
            {
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
                        npc.Dialogues.Add(loadDialogue(dialogueNode));
                    }

                    result.Objects.Add(npc);
                }
            }

            return result;
        }

        private static Dialogue loadDialogue(XmlNode text)
        {
            Dialogue dialogue = new Dialogue();
            dialogue.conditions = XmlUtil.GetArray(text, "if", ' ');
            dialogue.sets = XmlUtil.GetArray(text, "set", ' ');
            dialogue.clears = XmlUtil.GetArray(text, "clear", ' ');

            dialogue.phrases = new Phrase[text.ChildNodes.Count];

            int i = 0;

            foreach (XmlNode phrase in text.ChildNodes)
            {
                if (phrase.LocalName.Equals("phrase"))
                {
                    dialogue.phrases[i++] = loadSentence(phrase);
                }

                if (phrase.LocalName.Equals("question"))
                {
                    dialogue.phrases[i++] = loadQuestion(phrase);
                }
            }

            return dialogue;
        }

        /// <summary>
        /// Loads a sentence event 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Phrase loadSentence(XmlNode text)
        {
            Phrase result = new Phrase();
            result.text = text.InnerText;
            return result;
        }

        /// <summary>
        /// Loads a question event
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static Question loadQuestion(XmlNode text)
        {
            Question result = new Question();
            result.text = XmlUtil.Get(text, "text", string.Empty);

            foreach (XmlNode node in text.ChildNodes)
            {
                result.answers.Add(loadAnswer(node));
            }

            return result;
        }

        private static Answer loadAnswer(XmlNode node)
        {
            Answer answer = new Answer();
            answer.text = XmlUtil.Get(node, "text", "");
            answer.conditions = XmlUtil.GetArray(node, "if", ' ');
            answer.sets = XmlUtil.GetArray(node, "set", ' ');
            answer.clears = XmlUtil.GetArray(node, "clear", ' ');

            answer.phrases = new Phrase[node.ChildNodes.Count];

            int i = 0;

            foreach (XmlNode phrase in node.ChildNodes)
            {
                if (phrase.LocalName.Equals("phrase"))
                {
                    answer.phrases[i++] = loadSentence(phrase);
                }

                if (phrase.LocalName.Equals("question"))
                {
                    answer.phrases[i++] = loadQuestion(phrase);
                }
            }

            return answer;
        }

        /// <summary>
        /// Loads the current knowledge base to a dictionary of string (knowledge-id) and boolean (known or unknown)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ll"></param>
        /// <returns></returns>
        internal static Dictionary<string, bool> LoadKnowledge(string p, LL ll)
        {
            Dictionary<string, bool> investigation = new Dictionary<string, bool>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Resources/" + p);

            XmlNode investigationNode = doc.DocumentElement.SelectSingleNode("investigation");

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
