using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ScrollRectMenu)), CanEditMultipleObjects]
public class ScrollRectMenuEditor : ScrollRectEditor
{
   
    private SerializedProperty _ShiftableMenuPanel;
    private SerializedProperty _MenuController;
    private SerializedProperty CursorRectTransform;
    private SerializedProperty MinimumVelocityToStartSelecting;
    private SerializedProperty MaxVelocity;

    protected override void OnEnable()
    {
        base.OnEnable();
        _ShiftableMenuPanel= serializedObject.FindProperty("_ShiftableMenuPanel");
        _MenuController= serializedObject.FindProperty("_MenuController");
        CursorRectTransform= serializedObject.FindProperty("CursorRectTransform");
        MinimumVelocityToStartSelecting= serializedObject.FindProperty("MinimumVelocityToStartSelecting");
        MaxVelocity= serializedObject.FindProperty("MaxVelocity");
    }
      public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(MaxVelocity);
        EditorGUILayout.PropertyField(MinimumVelocityToStartSelecting);
        EditorGUILayout.PropertyField(CursorRectTransform);
        
        EditorGUILayout.PropertyField(_ShiftableMenuPanel);
        EditorGUILayout.PropertyField(_MenuController);
        serializedObject.ApplyModifiedProperties();

    }
}
