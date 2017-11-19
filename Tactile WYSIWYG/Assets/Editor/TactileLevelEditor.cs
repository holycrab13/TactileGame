using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TactileLevel))]
[CanEditMultipleObjects]
public class TactileLevelEditor : Editor
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


        castedTarget.GetComponent<SpriteRenderer>().sortingOrder = -1;
        //castedTarget.GetComponent<Image>().sprite = settings.gridSprite;
        //castedTarget.GetComponent<Image>().type = Image.Type.Tiled;
        //castedTarget.GetComponent<Image>().hideFlags = HideFlags.HideInInspector;
        //castedTarget.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<SpriteRenderer>().hideFlags = HideFlags.NotEditable;
        castedTarget.GetComponent<Transform>().hideFlags = HideFlags.NotEditable;
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

            myTarget.GetComponent<SpriteRenderer>().size = new Vector2(myTarget.width, myTarget.height);

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
