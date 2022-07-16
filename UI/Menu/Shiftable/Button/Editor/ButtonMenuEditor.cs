using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UI.MenuController;
using UnityEngine;

[CustomEditor(typeof(ButtonMenu)), CanEditMultipleObjects]
public class ButtonMenuEditor : SelectableEditor
{
    private SerializedProperty _PressedParameter;

    protected override void OnEnable()
    {
        base.OnEnable();
        _PressedParameter= serializedObject.FindProperty("_PressedParameter");
    }
      public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_PressedParameter);
        serializedObject.ApplyModifiedProperties();

    }
}
