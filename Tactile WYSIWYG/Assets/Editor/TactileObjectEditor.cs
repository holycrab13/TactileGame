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
        castedTarget.GetComponent<Image>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<CanvasRenderer>().hideFlags = HideFlags.HideInInspector;
        castedTarget.GetComponent<RectTransform>().hideFlags = HideFlags.NotEditable;
    }


    public override void OnInspectorGUI()
    {
        TactileObject myTarget = (TactileObject)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Update"))
        {
            myTarget.UpdateObject();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Move To Front"))
        {
            myTarget.transform.SetAsLastSibling();
        }

       
    }
}
