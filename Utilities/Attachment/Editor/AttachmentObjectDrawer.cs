using System;
using UnityEditor;
using UnityEngine;
using Attachment;

[CustomPropertyDrawer(typeof(AttachmentObject))][Serializable]
public class AttachmentObjectDrawer : PropertyDrawer
{        
    private const int Gap=5;
    private float propertyWidth;
    public string ObjectPropName="AssetRefObject";
    public string AnchorPropName="Anchor";
    public string FollowTypePropName="Type";    
    public string SmoothSpeedPropName="SmoothSpeed";
    public string OffsetPropName="Offset";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        var minY=position.min.y;
        var minX=position.min.x;
        propertyWidth=position.size.x;
        EditorGUI.BeginProperty(position, label, property);
        
        int lines = 0;

        //Label
        //var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label.text));
        var labelRect = new Rect(minX,minY + lines * GetSingleLineHeightRect(), EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect,label.text);
        
        float rightSidemin=labelRect.xMax+2.5f;

        if(EditorGUIUtility.wideMode==false)
        {
            rightSidemin=minX;
            lines++;
        }
        var rightSizeWidth=position.xMax-rightSidemin;
        
        var rectObject = new Rect(rightSidemin,minY + lines * GetSingleLineHeightRect(), rightSizeWidth/2, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rectObject, property.FindPropertyRelative(ObjectPropName),GUIContent.none);

        
        var rectAnchor = new Rect(rectObject.xMax+Gap,minY + lines * GetSingleLineHeightRect(), rightSizeWidth/2, EditorGUIUtility.singleLineHeight);        
        EditorGUI.PropertyField(rectAnchor, property.FindPropertyRelative(AnchorPropName),GUIContent.none);

        lines++;
        
        var labelOffsetRect = new Rect(minX,minY + lines * GetSingleLineHeightRect(), EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelOffsetRect,OffsetPropName);
        
        var rectOffset = new Rect(rightSidemin,minY + lines * GetSingleLineHeightRect(), rightSizeWidth, EditorGUIUtility.singleLineHeight);        
        EditorGUI.PropertyField(rectOffset, property.FindPropertyRelative(OffsetPropName),GUIContent.none);
        
        lines++;

        var rectType = new Rect(rightSidemin,minY + lines * GetSingleLineHeightRect(), rightSizeWidth, EditorGUIUtility.singleLineHeight);        
        EditorGUI.PropertyField(rectType, property.FindPropertyRelative(FollowTypePropName),GUIContent.none);

        var enumVal=property.FindPropertyRelative(FollowTypePropName).enumValueIndex;
        if(enumVal==1) //smooth
        {            
            lines++;

            //var textSSDimensions = GUI.skin.label.CalcSize(new GUIContent(SmoothSpeedPropName));
            //var labelSSRect = new Rect(minX,minY + lines * GetSingleLineHeightRect(), EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            //EditorGUI.LabelField(labelSSRect,SmoothSpeedPropName);
//
            var rectSmoothSpeed = new Rect(rightSidemin,minY + lines * GetSingleLineHeightRect(), rightSizeWidth, EditorGUIUtility.singleLineHeight);        
            //EditorGUI.PropertyField(rectSmoothSpeed, property.FindPropertyRelative(SmoothSpeedPropName),GUIContent.none);
            float ssFloat=property.FindPropertyRelative(SmoothSpeedPropName).floatValue;
            float newFloat = Mathf.Max(0,EditorGUI.FloatField(rectSmoothSpeed,SmoothSpeedPropName,ssFloat));
            
            property.FindPropertyRelative(SmoothSpeedPropName).floatValue = newFloat;
        }


        
        

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
        int totalLines = 3;

        if(EditorGUIUtility.wideMode==false)
        {
            totalLines++;
        }

        if(property.FindPropertyRelative(FollowTypePropName).enumValueIndex==1)
            totalLines++;

        

        return GetSingleLineHeightRect() * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}
