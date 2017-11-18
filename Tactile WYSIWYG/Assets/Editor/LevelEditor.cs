using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TactileLevel))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{

    void OnEnable()
    {
        var castedTarget = (target as TactileLevel);
       
        DisplaySettings settings = FindObjectOfType<DisplaySettings>();

        if (settings == null)
        {
            Debug.Log("Display Settings are missing!");
            return;
        }

        castedTarget.GetComponent<Image>().sprite = settings.gridSprite;
        castedTarget.GetComponent<Image>().type = Image.Type.Tiled;
        castedTarget.GetComponent<Image>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<Canvas>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<RectTransform>().hideFlags = HideFlags.NotEditable;
    }

    public override void OnInspectorGUI()
    {
        TactileLevel myTarget = (TactileLevel)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Update Dimensions"))
        {
            DisplaySettings settings = FindObjectOfType<DisplaySettings>();

            if(settings == null)
            {
                Debug.Log("Display Settings are missing!");
                return;
            }

            myTarget.GetComponent<RectTransform>().pivot = new Vector2(.5f, .5f);
            myTarget.GetComponent<RectTransform>().sizeDelta = new Vector2(myTarget.width * settings.tileSize, myTarget.height * settings.tileSize);
        }

        if (GUILayout.Button("Positions to Grid"))
        {
            DisplaySettings settings = FindObjectOfType<DisplaySettings>();

            if (settings == null)
            {
                Debug.Log("Display Settings are missing!");
                return;
            }

            TactileObject[] tactileObjects = myTarget.GetComponentsInChildren<TactileObject>();

            foreach (TactileObject tactileObject in tactileObjects)
            {
                RectTransform rect = tactileObject.GetComponent<RectTransform>();

                if (rect == null)
                {
                    Debug.Log("Trying to update object without rect transform!");
                    return;
                }

                Vector2 position = rect.anchoredPosition;

                float posX = (position.x - settings.tileSize / 2.0f) / settings.tileSize;
                float posY = (position.y - settings.tileSize / 2.0f) / settings.tileSize;



                tactileObject.x = Mathf.RoundToInt(posX);
                tactileObject.y = Mathf.RoundToInt(posY);

                position.x = tactileObject.x * settings.tileSize + settings.tileSize / 2.0f;
                position.y = tactileObject.y * settings.tileSize + settings.tileSize / 2.0f;

                rect.anchoredPosition = position;
            }
        }

        if (GUILayout.Button("Save to XML"))
        {
            myTarget.Save();
        }

        if (GUILayout.Button("Load from XML"))
        {
            myTarget.Load();
        }
    }
}
