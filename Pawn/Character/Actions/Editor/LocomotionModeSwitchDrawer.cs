using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocomotionModeSwitch))]
public class LocomotionModeSwitchDrawer : PropertyDrawer 
{
    private const string Toggle = "Toggle";
    private const string LocomotionPropName = "LocomotionMode";
    private const string AirbornePropName = "AirborneMode";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        
        position.y-=GetSingleLineHeightRect();
        EditorGUI.BeginProperty(position, label, property);


        int lines = 1;

        Rect rectEndwith = CreateRect(ref position, ref lines);
        EditorGUI.PropertyField(rectEndwith, property.FindPropertyRelative(Toggle));
        var EndModeProperty=property.FindPropertyRelative(Toggle);

        EditorGUI.indentLevel++;
        if(EndModeProperty.boolValue)
        {
            Rect rectAirborne = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectAirborne, property.FindPropertyRelative(AirbornePropName));
        }
        else
        {            
            Rect rectLocomotion = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectLocomotion, property.FindPropertyRelative(LocomotionPropName));
        }       
        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    private static Rect CreateRect(ref Rect position, ref int lines)
    {
        return new Rect(position.min.x, position.min.y + lines++ * GetSingleLineHeightRect(), position.size.x, EditorGUIUtility.singleLineHeight);
    }

    private static float GetSingleLineHeightRect()
    {
        return EditorGUIUtility.singleLineHeight*1.1f;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        int totalLines = 2;


        return GetSingleLineHeightRect() * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}
