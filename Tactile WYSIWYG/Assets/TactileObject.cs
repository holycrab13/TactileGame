using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class TactileObject : MonoBehaviour {

    public string objectName;

    public int x;

    public int y;

    public int rotation;

    [HideInInspector]
    public int width;

    [HideInInspector]
    public int height;

    public void UpdateObject()
    {
        DisplaySettings settings = FindObjectOfType<DisplaySettings>();

        if (settings == null)
        {
            Debug.Log("Display Settings are missing!");
            return;
        }

        gameObject.layer = 0;
        GetComponent<SpriteRenderer>().material = settings.spriteMaterial;

        List<LibraryObject> library = FindObjectsOfType<LibraryObject>().ToList();
        LibraryObject libraryObject = library.Find(o => o.id.Equals(objectName));

        if (libraryObject == null)
        {
            Debug.Log("No object " + objectName + " in your library!");
            return;
        }

        if (libraryObject.image == null)
        {
            Debug.Log("Image " + objectName + " has no image assigned!");
            return;
        }

        Transform t = GetComponent<Transform>();

        if (t == null)
        {
            Debug.Log("Trying to update object without rect transform!");
            return;
        }

        width = (int)(libraryObject.image.texture.width / settings.tileSize);
        height = (int)(libraryObject.image.texture.height / settings.tileSize);

        t.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * rotation);

        if (rotation == 0)
            t.localPosition = new Vector3(x, y, 10.0f);
        else if (rotation == 1)
            t.localPosition = new Vector3(x, y + width, 10.0f);
        else if (rotation == 2)
            t.localPosition = new Vector3(x + width, y + height, 10.0f);
        else if (rotation == 3)
            t.localPosition = new Vector3(x + height, y, 10.0f);
        else
            Debug.Log("Invalid rotation used!");


        GetComponent<SpriteRenderer>().sprite = libraryObject.image;
    }



    public void SnapToGrid()
    {
        DisplaySettings settings = FindObjectOfType<DisplaySettings>();

        if (settings == null)
        {
            Debug.Log("Display Settings are missing!");
            return;
        }

        List<LibraryObject> library = FindObjectsOfType<LibraryObject>().ToList();
        LibraryObject libraryObject = library.Find(o => o.id.Equals(objectName));

        if (libraryObject == null)
        {
            Debug.Log("No object " + objectName + " in your library!");
            return;
        }

        if (libraryObject.image == null)
        {
            Debug.Log("Image " + objectName + " has no image assigned!");
            return;
        }

        Transform t = GetComponent<Transform>();

        if (t == null)
        {
            Debug.Log("Trying to update object without rect transform!");
            return;
        }

        int width = (int)(libraryObject.image.texture.width / settings.tileSize);
        int height = (int)(libraryObject.image.texture.height / settings.tileSize);

        t.localPosition = new Vector3(Mathf.RoundToInt(t.localPosition.x), Mathf.RoundToInt(t.localPosition.y), 10.0f);

        Vector3 bottomLeft = Vector2.zero;

        if (rotation == 0)
            bottomLeft = t.localPosition;
        else if (rotation == 1)
            bottomLeft = t.localPosition - new Vector3(0.0f, width);
        else if (rotation == 2)
            bottomLeft = t.localPosition - new Vector3(width, height);
        else if (rotation == 3)
            bottomLeft = t.localPosition - new Vector3(height, 0.0f);
        else
            Debug.Log("Invalid rotation used!");

        x = (int)bottomLeft.x;
        y = (int)bottomLeft.y;

    }
}
