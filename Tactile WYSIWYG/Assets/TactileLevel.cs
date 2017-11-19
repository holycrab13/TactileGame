using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using System.Xml;

public class TactileLevel : MonoBehaviour {

    public string levelName;

    public string fileName;

    public int width;

    public int height;

    public void Load()
    {
        GameObject libGo = GameObject.FindGameObjectWithTag("Library");

        List<LibraryObject> library = FindObjectsOfType<LibraryObject>().ToList();

        // Create XML reader settings
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;                        

        // Create reader based on settings
        XmlReader reader = XmlReader.Create(fileName, settings);
        XmlDocument doc = new XmlDocument();
        doc.Load(reader);

        XmlNode objectNode = doc.DocumentElement.SelectSingleNode("definition/objects");

        foreach (XmlNode node in objectNode.ChildNodes)
        {
            string id =  XmlUtil.Get(node, "id", string.Empty);

            LibraryObject libraryObject = library.Find(o => o.id.Equals(id));

            if (libraryObject == null)
            {
                Debug.Log("Creating new Library Object " + id);

                GameObject gameObject = new GameObject(id);
                libraryObject = gameObject.AddComponent<LibraryObject>();
                libraryObject.id = id;
                libraryObject.label = XmlUtil.Get(node, "name", string.Empty);
                libraryObject.description = XmlUtil.Get(node, "desc", string.Empty);
                libraryObject.block = XmlUtil.Get(node, "block", true);

                string imageName = XmlUtil.Get(node, "img", string.Empty);

                imageName = imageName.Split('.')[0].Split('/')[1];

                libraryObject.image = Resources.Load<Sprite>(imageName);

                gameObject.transform.SetParent(libGo.transform, false);
            }
        }

        library = FindObjectsOfType<LibraryObject>().ToList();

        XmlNode decoNode = doc.DocumentElement.SelectSingleNode("decos");

        if (decoNode != null)
        {
            foreach (XmlNode node in decoNode.ChildNodes)
            {
             

                if (node.Name.Equals("range"))
                {
                    int x = XmlUtil.Get(node, "x", 0);
                    int y = XmlUtil.Get(node, "y", 0);
                    int width = XmlUtil.Get(node, "width", 0);
                    int height = XmlUtil.Get(node, "height", 0);

                    for (int i = x; i < x + width; i++)
                    {
                        for (int j = y; j < y + height; j++)
                        {
                            createDeco(library, node.ChildNodes[0], i, j);
                        }
                    }
                }
                else
                {
                    int x = XmlUtil.Get(node, "x", 0);
                    int y = XmlUtil.Get(node, "y", 0);

                    createDeco(library, node, x, y);

                }
            }
        }

        XmlNode doorNode = doc.DocumentElement.SelectSingleNode("doors");

        if (doorNode != null)
        {
            foreach (XmlNode node in doorNode.ChildNodes)
            {
                
        
                string id = XmlUtil.Get(node, "obj", string.Empty);

                LibraryObject libraryObject = library.Find(o => o.id.Equals(id));

                if (libraryObject == null)
                {
                    Debug.Log("WARNING: file contains instance with invalid object");
                    return;
                }

                TactileDoor door = new GameObject("Door_" + id).AddComponent<TactileDoor>();
                door.x = XmlUtil.Get(node, "x", 0);
                door.y = XmlUtil.Get(node, "y", 0);
                door.rotation = XmlUtil.Get(node, "dir", 0);
                door.target = XmlUtil.Get(node, "target", string.Empty);
                door.targetX = XmlUtil.Get(node, "targetX", 0);
                door.targetY = XmlUtil.Get(node, "targetY", 0);
                door.objectName = id;
                door.transform.SetParent(transform, true);

                door.UpdateObject();
            }
        }
    }

    private void createDeco(List<LibraryObject> library, XmlNode xmlNode, int i, int j)
    {
        string id = XmlUtil.Get(xmlNode, "obj", string.Empty);

        LibraryObject libraryObject = library.Find(o => o.id.Equals(id));

        if (libraryObject == null)
        {
            Debug.Log("WARNING: file contains instance with invalid object");
            return;
        }

        TactileDeco deco = new GameObject("Object_" + id).AddComponent<TactileDeco>();
        deco.x = i;
        deco.y = j;
        deco.rotation = XmlUtil.Get(xmlNode, "dir", 0);
        deco.objectName = id;
        deco.transform.SetParent(transform, true);


        deco.UpdateObject();

    }

    public void Save()
    {
        List<LibraryObject> library = FindObjectsOfType<LibraryObject>().ToList();

        Dictionary<string, LibraryObject> referencedObjects = new Dictionary<string, LibraryObject>();

        TactileDeco[] decos = FindObjectsOfType<TactileDeco>();
        TactileDoor[] doors = FindObjectsOfType<TactileDoor>();

        XmlDocument doc = new XmlDocument();

        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement root = doc.DocumentElement;
        doc.InsertBefore(xmlDeclaration, root);

        XmlElement nodeLevel = doc.CreateElement(string.Empty, "level", string.Empty);
        nodeLevel.Attributes.Append(createAttribute(doc, "width", width.ToString()));
        nodeLevel.Attributes.Append(createAttribute(doc, "height", height.ToString()));
        doc.AppendChild(nodeLevel);

        XmlElement nodeDef = doc.CreateElement(string.Empty, "definition", string.Empty);
        nodeLevel.AppendChild(nodeDef);

        XmlElement nodeObjects = doc.CreateElement(string.Empty, "objects", string.Empty);
        nodeDef.AppendChild(nodeObjects);

        XmlElement nodeDecos = doc.CreateElement(string.Empty, "decos", string.Empty);
        nodeLevel.AppendChild(nodeDecos);

        XmlElement nodeDoors = doc.CreateElement(string.Empty, "doors", string.Empty);
        nodeLevel.AppendChild(nodeDoors);

        foreach(TactileDeco deco in decos)
        {
            LibraryObject referencedObject = library.Find(o => o.id.Equals(deco.objectName));

            if (referencedObject == null)
            {
                Debug.Log("WARNING: project contains invalid instances");
                continue;
            }

            XmlElement nodeDeco = doc.CreateElement(string.Empty, "deco", string.Empty);

            if(!referencedObjects.ContainsKey(deco.objectName))
            {
                referencedObjects.Add(deco.objectName, referencedObject);
            }

            nodeDeco.Attributes.Append(createAttribute(doc, "obj", deco.objectName));
            nodeDeco.Attributes.Append(createAttribute(doc, "x", deco.x.ToString()));
            nodeDeco.Attributes.Append(createAttribute(doc, "y", deco.y.ToString()));
            nodeDeco.Attributes.Append(createAttribute(doc, "dir", deco.rotation.ToString()));

            nodeDecos.AppendChild(nodeDeco);
        }

        foreach (TactileDoor door in doors)
        {

            LibraryObject referencedObject = library.Find(o => o.id.Equals(door.objectName));

            if (referencedObject == null)
            {
                Debug.Log("WARNING: project contains invalid instances");
                continue;
            }

            XmlElement nodeDoor = doc.CreateElement(string.Empty, "deco", string.Empty);

            if (!referencedObjects.ContainsKey(door.objectName))
            {
                referencedObjects.Add(door.objectName, referencedObject);
            }

            nodeDoor.Attributes.Append(createAttribute(doc, "obj", door.objectName));
            nodeDoor.Attributes.Append(createAttribute(doc, "x", door.x.ToString()));
            nodeDoor.Attributes.Append(createAttribute(doc, "y", door.y.ToString()));
            nodeDoor.Attributes.Append(createAttribute(doc, "dir", door.rotation.ToString()));
            nodeDoor.Attributes.Append(createAttribute(doc, "target", door.target.ToString()));
            nodeDoor.Attributes.Append(createAttribute(doc, "targetX", door.targetX.ToString()));
            nodeDoor.Attributes.Append(createAttribute(doc, "targetY", door.targetY.ToString()));


            nodeDoors.AppendChild(nodeDoor);
        }

        foreach(LibraryObject libraryObject in referencedObjects.Values)
        {
            XmlElement nodeObject = doc.CreateElement(string.Empty, "object", string.Empty);
            nodeObject.Attributes.Append(createAttribute(doc, "id", libraryObject.id));
            nodeObject.Attributes.Append(createAttribute(doc, "name", libraryObject.name));
            nodeObject.Attributes.Append(createAttribute(doc, "desc", libraryObject.description));
            nodeObject.Attributes.Append(createAttribute(doc, "img", "bmps/" + libraryObject.image.name + ".bmp"));
            nodeObject.Attributes.Append(createAttribute(doc, "block", libraryObject.block.ToString()));

            nodeObjects.AppendChild(nodeObject);
        }

        


      
        doc.Save(fileName);

        Debug.Log("Level Saved!");
    }

    private XmlAttribute createAttribute(XmlDocument doc, string p1, string p2)
    {
        XmlAttribute attrHeight = doc.CreateAttribute(string.Empty, p1, string.Empty);
        attrHeight.Value = p2;

        return attrHeight;
    }

}
