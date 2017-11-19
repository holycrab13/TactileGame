using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
public abstract class TactileObject : MonoBehaviour {

    public string objectName;

    public int x;

    public int y;

    public int dir;

    public void UpdateObject()
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

        RectTransform rect = GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.Log("Trying to update object without rect transform!");
            return;
        }

        int width = libraryObject.image.texture.width;
        int height = libraryObject.image.texture.height;

        float pivotX = 0.5f / (width / settings.tileSize);
        float pivotY = 0.5f / (height / settings.tileSize);


        rect.anchoredPosition = new Vector3((x + 0.5f) * settings.tileSize, (y + 0.5f) * settings.tileSize, 10.0f);
        rect.sizeDelta = new Vector2(width, height);
        rect.pivot = new Vector2(pivotX, pivotY);
        rect.rotation = Quaternion.Euler(0.0f, 0.0f, -90.0f * dir);
        GetComponent<Image>().sprite = libraryObject.image;
    }
}
