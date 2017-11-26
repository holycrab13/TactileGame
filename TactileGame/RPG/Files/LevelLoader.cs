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

        private static readonly string S_RANGE = "range";



        /// <summary>
        /// Loads the level from a txt file at a given path
        /// </summary>
        /// <param name="levelName"></param>
        /// <returns></returns>
        public static Level Load(string levelName, LL ll)
        {

            worldObjects = new Dictionary<string, WorldObject>();
            dialogues = new Dictionary<string, Phrase[]>();

            // Create XML reader settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;                         // Exclude comments

            // Create reader based on settings
            XmlReader reader = XmlReader.Create("Resources/levels/" + levelName + ".xml", settings);
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
                Texture = BooleanTexture.FromFile("Resources/bmps/avatar_2.bmp"),
            };

            result.avatar = character;


            XmlNode eventNode = doc.DocumentElement.SelectSingleNode("definition/events");

            if (eventNode != null)
            {
                foreach (XmlNode node in eventNode.ChildNodes)
                {
                    string name = node.Name;

                    if(name == "eventgroup")
                    {
                        EventGroup group = new EventGroup();
                        group.id = XmlUtil.Get(node, "id", string.Empty);
                        group.conditions = XmlUtil.GetArray(node, "if", ' ');
                        group.inverseConditions = XmlUtil.GetArray(node, "not", ' ');

                        foreach (XmlNode childNode in node.ChildNodes)
                        {
                            group.events.Add(loadEvent(childNode));
                        }

                        result.Events.Add(group);
                    }

                    if (name == "event")
                    {
                        result.Events.Add(loadEvent(node));
                    }
                }
            }

            XmlNode triggerNode = doc.DocumentElement.SelectSingleNode("triggers");

            if (triggerNode != null)
            {
                foreach (XmlNode node in triggerNode.ChildNodes)
                {
                    EventTrigger trigger = new EventTrigger();

                    trigger.x = Constants.TILE_SIZE * XmlUtil.Get(node, "x", 0);
                    trigger.y = Constants.TILE_SIZE * XmlUtil.Get(node, "y", 0);
                    trigger.width = Constants.TILE_SIZE * XmlUtil.Get(node, "width", 1);
                    trigger.height = Constants.TILE_SIZE * XmlUtil.Get(node, "height", 1);
                    trigger.levelEvent = XmlUtil.Get(node, "event", string.Empty);

                    result.Triggers.Add(trigger);
                }
            }

            XmlNode decoNode = doc.DocumentElement.SelectSingleNode("decos");

            if (decoNode != null)
            {
                foreach (XmlNode node in decoNode.ChildNodes)
                {
                    if (node.Name.Equals(S_RANGE))
                    {
                        int x = XmlUtil.Get(node, "x", 0);
                        int y = XmlUtil.Get(node, "y", 0);
                        int width = XmlUtil.Get(node, "width", 1);
                        int height = XmlUtil.Get(node, "height", 1);

                        WorldObject blueprint = worldObjects[XmlUtil.Get(node.ChildNodes[0], "obj", "default")];
                        int dimX = blueprint.Width / Constants.TILE_SIZE;
                        int dimY = blueprint.Height / Constants.TILE_SIZE;

                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                result.Objects.Add(createDeco(node.ChildNodes[0], x + i * dimX, y + j * dimY));
                            }
                        }
                    }
                    else
                    {
                        int x = XmlUtil.Get(node, "x", 0);
                        int y = XmlUtil.Get(node, "y", 0);

                        result.Objects.Add(createDeco(node, x, y));

                       
                    }
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
                    };

                    if (!Game.HasKnowledge(item.Id + "_gefunden"))
                    {
                        Phrase talk = new Phrase()
                            {  
                                text = "Du hast " + blueprint.Name + " gefunden.",
                            sets = new string[] { "hat_" + item.Id, item.Id + "_gefunden" }
                        };

                        item.Event = new Event(talk);
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
                        Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
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
                        BlocksPath = blueprint.BlocksPath,
                        Texture = blueprint.Texture,
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                        Trigger = XmlUtil.Get(node, "trigger", string.Empty)
                    };

                    result.Objects.Add(npc);
                }
            }

            return result;
        }

        private static WorldObject createDeco(XmlNode node, int x, int y)
        {
            WorldObject blueprint = worldObjects[XmlUtil.Get(node, "obj", "default")];

            return new WorldObject()
            {
                X = Constants.TILE_SIZE * x,
                Y = Constants.TILE_SIZE * y,
                Rotation = XmlUtil.Get(node, "r", Direction.DOWN),
                Id = XmlUtil.Get(node, "id", "object"),
                BlocksPath = blueprint.BlocksPath,
                Texture = blueprint.Texture,
                Name = blueprint.Name,
                Description = blueprint.Description,
                Event = new Event(Phrase.Create(blueprint.Description)),
            };
        }

        private static Event loadEvent(XmlNode node)
        {
            Event evnt = new Event();
            evnt.id = XmlUtil.Get(node, "id", string.Empty);
            evnt.conditions = XmlUtil.GetArray(node, "if", ' ');
            evnt.inverseConditions = XmlUtil.GetArray(node, "not", ' ');

            foreach (XmlNode actionNode in node.ChildNodes)
            {
                evnt.actions.Add(loadAction(actionNode));
              
            }

            return evnt;
        }

        private static Events.ActionBase loadAction(XmlNode actionNode)
        {
            string actionType = actionNode.Name;

            if (actionType == "phrase")
            {
                Phrase phrase = new Phrase();
                phrase.text = actionNode.InnerText;
                phrase.sets = XmlUtil.GetArray(actionNode, "set", ' ');
                phrase.clears = XmlUtil.GetArray(actionNode, "clear", ' ');

                return phrase;
            }

            if (actionType == "question")
            {
                Question question = new Question();

                foreach (XmlNode node in actionNode.ChildNodes)
                {
                    if (node.Name == "text")
                    {
                        question.text = node.InnerText;
                    }

                    if (node.Name == "answer")
                    {
                        Answer answer = new Answer();
                        answer.text = node.InnerText;
                        answer.sets = XmlUtil.GetArray(node, "set", ' ');
                        answer.clears = XmlUtil.GetArray(node, "clear", ' ');
                        answer.trigger = XmlUtil.Get(node, "trigger", string.Empty);
                        question.answers.Add(answer);
                    }
                }

                return question;
            }

            if (actionType == "move")
            {
                Move move = new Move();
                move.direction = directionFromString(actionNode.InnerText);
                move.target = XmlUtil.Get(actionNode, "target", string.Empty);

                // Default target is the avatar
                if (move.target.Equals(string.Empty))
                {
                    move.target = "avatar";
                }

                return move;
            }

            if (actionType == "goto")
            {
                Goto move = new Goto();
                move.target = XmlUtil.Get(actionNode, "target", string.Empty);
                move.targetX = XmlUtil.Get(actionNode, "x", 0);
                move.targetY = XmlUtil.Get(actionNode, "y", 0);

                // Default target is the avatar
                if (move.target.Equals(string.Empty))
                {
                    move.target = "avatar";
                }

                return move;
            }

            if (actionType == "relgoto")
            {
                RelGoto move = new RelGoto();
                move.target = XmlUtil.Get(actionNode, "target", string.Empty);
                move.targetX = XmlUtil.Get(actionNode, "x", 0);
                move.targetY = XmlUtil.Get(actionNode, "y", 0);

                // Default target is the avatar
                if (move.target.Equals(string.Empty))
                {
                    move.target = "avatar";
                }

                return move;
            }

            if (actionType == "relsetpos")
            {
                RelSetPos move = new RelSetPos();
                move.target = XmlUtil.Get(actionNode, "target", string.Empty);
                move.targetX = XmlUtil.Get(actionNode, "x", 0);
                move.targetY = XmlUtil.Get(actionNode, "y", 0);

                // Default target is the avatar
                if (move.target.Equals(string.Empty))
                {
                    move.target = "avatar";
                }

                return move;
            }

            if (actionType == "setpos")
            {
                SetPos move = new SetPos();
                move.target = XmlUtil.Get(actionNode, "target", string.Empty);
                move.targetX = XmlUtil.Get(actionNode, "x", 0);
                move.targetY = XmlUtil.Get(actionNode, "y", 0);

                // Default target is the avatar
                if (move.target.Equals(string.Empty))
                {
                    move.target = "avatar";
                }

                return move;
            }

            if (actionType == "turn")
            {
                Turn turn = new Turn();
                turn.direction = directionFromString(actionNode.InnerText);
                turn.target = XmlUtil.Get(actionNode, "target", string.Empty);

                // Default target is the avatar
                if (turn.target.Equals(string.Empty))
                {
                    turn.target = "avatar";
                }

                return turn;
            }

            if (actionType == "interact")
            {
                Interact interact = new Interact();
                interact.target = XmlUtil.Get(actionNode, "target", string.Empty);

                return interact;
            }

            return null;
        }

        private static Direction directionFromString(string p)
        {
            if (p.Equals("Up"))
                return Direction.UP;

            if (p.Equals("Left"))
                return Direction.LEFT;

            if (p.Equals("Right"))
                return Direction.RIGHT;

            if (p.Equals("Down"))
                return Direction.DOWN;

            return Direction.UP;
        }

        private static DialogueAction loadDialogue(XmlNode text)
        {
            return null;
            //DialogueAction dialogue = new DialogueAction();
            //dialogue.conditions = XmlUtil.GetArray(text, "if", ' ');
            //dialogue.sets = XmlUtil.GetArray(text, "set", ' ');
            //dialogue.clears = XmlUtil.GetArray(text, "clear", ' ');

            //dialogue.phrases = new Talk[text.ChildNodes.Count];

            //int i = 0;

            //foreach (XmlNode phrase in text.ChildNodes)
            //{
            //    if (phrase.LocalName.Equals("phrase"))
            //    {
            //        dialogue.phrases[i++] = loadSentence(phrase);
            //    }

            //    if (phrase.LocalName.Equals("question"))
            //    {
            //        dialogue.phrases[i++] = loadQuestion(phrase);
            //    }
            //}

            //return dialogue;
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
            //answer.conditions = XmlUtil.GetArray(node, "if", ' ');
            //answer.sets = XmlUtil.GetArray(node, "set", ' ');
            //answer.clears = XmlUtil.GetArray(node, "clear", ' ');

            //answer.phrases = new Talk[node.ChildNodes.Count];

            //int i = 0;

            //foreach (XmlNode phrase in node.ChildNodes)
            //{
            //    if (phrase.LocalName.Equals("phrase"))
            //    {
            //        answer.phrases[i++] = loadSentence(phrase);
            //    }

            //    if (phrase.LocalName.Equals("question"))
            //    {
            //        answer.phrases[i++] = loadQuestion(phrase);
            //    }
            //}

            return answer;
        }

        /// <summary>
        /// Loads the current knowledge base to a dictionary of string (knowledge-id) and boolean (known or unknown)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ll"></param>
        /// <returns></returns>
        internal static Dictionary<string, bool> LoadKnowledge(XmlNode knowledgeNode, LL ll)
        {
            Dictionary<string, bool> investigation = new Dictionary<string, bool>();

            foreach (XmlNode node in knowledgeNode.ChildNodes)
            {

                string id = XmlUtil.Get(node, "id", string.Empty);

                if (id != string.Empty)
                {
                    investigation[id] = Boolean.Parse(node.InnerText);
                }
            }

            return investigation;
        }

        internal static SaveGame LoadSaveGame(string saveGame, LL ll)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Resources/" + saveGame + ".xml");

            SaveGame save = new SaveGame();

            save.Knowledge = LoadKnowledge(doc.DocumentElement.SelectSingleNode("investigation"), ll);

            XmlNode levelNode = doc.DocumentElement.SelectSingleNode("level");
            save.LevelName = XmlUtil.Get(levelNode, "name", string.Empty);
            save.X = Constants.TILE_SIZE * XmlUtil.Get(levelNode, "x", 0);
            save.Y = Constants.TILE_SIZE * XmlUtil.Get(levelNode, "y", 0);

            return save;
        }
    }
}
