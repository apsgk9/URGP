using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(AnimatorLocomotionTransition))]
public class AnimatorLocomotionTransitionDrawer : PropertyDrawer 
{
    private const string StateToEndPropname = "StateToTransitionToAtEnd";
    private const string LayerPropName = "Layer";
    private const string ChangeStartPropName = "ChangeStart";
    private const string StartLocomotionModeSwitchPropName = "StartLocomotionModeSwitch";
    private const string ChangeEndPropName = "ChangeEnd";
    private const string EndLocomotionModeSwitchPropName = "EndLocomotionModeSwitch";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        var originalPosition=position;
        EditorGUI.BeginProperty(position, label, property);
        Rect rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);
        int lines = 1;
        EditorGUI.indentLevel++;
        if (property.isExpanded)
        {
            Rect rectState = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectState, property.FindPropertyRelative(StateToEndPropname));

            Rect rectLayer = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectLayer, property.FindPropertyRelative(LayerPropName));

            //Start Mode
            Rect rectStartwith = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectStartwith, property.FindPropertyRelative(ChangeStartPropName));
            var StartModeProperty=property.FindPropertyRelative(ChangeStartPropName);
            EditorGUI.indentLevel++;
            if(StartModeProperty.boolValue)
            {
                Rect rectLocomotion = CreateRect(ref position, ref lines);
                EditorGUI.PropertyField(rectLocomotion, property.FindPropertyRelative(StartLocomotionModeSwitchPropName));
                lines++;
            }
            EditorGUI.indentLevel--;
            
            //End Mode
            Rect rectEndwith = CreateRect(ref position, ref lines);
            EditorGUI.PropertyField(rectEndwith, property.FindPropertyRelative(ChangeEndPropName));
            var EndModeProperty=property.FindPropertyRelative(ChangeEndPropName);
            EditorGUI.indentLevel++;
            if(EndModeProperty.boolValue)
            {
                Rect rectLocomotion = CreateRect(ref position, ref lines);
                EditorGUI.PropertyField(rectLocomotion, property.FindPropertyRelative(EndLocomotionModeSwitchPropName));
                lines++;
            }
            EditorGUI.indentLevel--;

            position=originalPosition;
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
        int totalLines = 1;

        if (property.isExpanded) 
        {
            totalLines += 4;

            var StartModeProperty=property.FindPropertyRelative(ChangeStartPropName);
            if (StartModeProperty.boolValue) 
            {
                totalLines += 2;
            }

            var EndModeProperty=property.FindPropertyRelative(ChangeEndPropName);
            if (EndModeProperty.boolValue) 
            {
                totalLines += 2;
            }
        }
        

        return GetSingleLineHeightRect() * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}