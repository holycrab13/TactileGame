using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[CustomEditor(typeof(TactileObject), true)]
[CanEditMultipleObjects]
public class TactileObjectEditor : Editor
{

    void OnEnable()
    {
        var castedTarget = (target as TactileObject);
        castedTarget.GetComponent<Transform>().hideFlags = HideFlags.NotEditable;
        castedTarget.GetComponent<SpriteRenderer>().hideFlags = HideFlags.NotEditable;

        DisplaySettings settings = FindObjectOfType<DisplaySettings>();

        if (settings == null)
        {
            Debug.Log("Display Settings are missing!");
            return;
        }

        castedTarget.GetComponent<SpriteRenderer>().material = settings.spriteMaterial;
    }


    public override void OnInspectorGUI()
    {
        TactileObject myTarget = (TactileObject)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply Values"))
        {
             foreach(var obj in Selection.gameObjects)
             {
                 TactileObject objScript = obj.GetComponent<TactileObject>();

                 objScript.UpdateObject();
             }

        }

        if (GUILayout.Button("Transform to Position"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                TactileObject objScript = obj.GetComponent<TactileObject>();

                objScript.SnapToGrid();
            }

        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Move To Front"))
        {
            myTarget.transform.SetAsLastSibling();
        }

        serializedObject.ApplyModifiedProperties();
       
    }

    private void OnSceneGUI()
    {
        TactileObject myTarget = (TactileObject)target;

        Vector3[] verts =
        {
            new Vector3(myTarget.x, myTarget.y, 9.0f),
            new Vector3(myTarget.x + myTarget.width, myTarget.y, 9.0f),
            new Vector3(myTarget.x + myTarget.width, myTarget.y + myTarget.height, 9.0f),
            new Vector3(myTarget.x, myTarget.y + myTarget.height, 9.0f),

        };
    
        Handles.DrawSolidRectangleWithOutline(verts, new Color(0,1,1,0.2f), new Color(0,0,0,0));



    }
}
