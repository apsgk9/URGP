using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimatorParamaterList))][Serializable]
public class AnimatorParamaterListDrawer : PropertyDrawer
{
    private Animator sourceAnimator;
    private const string AnimatorPropName = "Animator";
    private const string ParameterNamePropName = "Name";
    private const string ParameterHashPropName = "Hash";
    private const string IndexPropName= "m_Index";
    private const string ParamTypePropName= "Type";
    private int popOutIndex;
    private AnimatorController paramType;
    private int Gap=5;

    public List<string> ParameterList;
    private float propertyWidth;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        var minY=position.min.y;
        var minX=position.min.x;
        propertyWidth=position.size.x;
        EditorGUI.BeginProperty(position, label, property);
        ParameterList=new List<string>();
        
        int lines = 0;

        //Label
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label.text));
        var labelRect = new Rect(minX,minY + lines * GetSingleLineHeightRect(), EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect,label.text);
        
        float rightSidemin=labelRect.xMax+2.5f;

        if(EditorGUIUtility.wideMode==false)
        {
            rightSidemin=minX;
            lines++;
        }
        var rightSizeWidth=position.xMax-rightSidemin;
        
        var rectAnim = new Rect(rightSidemin,minY + lines * GetSingleLineHeightRect(), rightSizeWidth/4, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rectAnim, property.FindPropertyRelative(AnimatorPropName),GUIContent.none);

        
        //Clamp Field
        var AnimatorProperty=property.FindPropertyRelative(AnimatorPropName);
        sourceAnimator= AnimatorProperty.objectReferenceValue as Animator;
        

        var ParamterNameProperty=property.FindPropertyRelative(ParameterNamePropName);
        var ParamterHashProperty=property.FindPropertyRelative(ParameterHashPropName);
        var rectStates= new Rect(rectAnim.xMax+Gap,minY + lines * GetSingleLineHeightRect(), (rightSizeWidth-(rectAnim.width))*2/3, EditorGUIUtility.singleLineHeight); 
        string ParamType="";      
        if(sourceAnimator!=null)
        { 
            List<AnimatorControllerParameterType> ParamTypes;
            ParameterList = AnimatorUtil.GetParameters(sourceAnimator.runtimeAnimatorController as AnimatorController,out ParamTypes);
            popOutIndex=property.FindPropertyRelative(IndexPropName).intValue;
            popOutIndex = EditorGUI.Popup(rectStates,popOutIndex, ParameterList.ToArray());
            if(ParameterList.Count>0 && popOutIndex>=0 && popOutIndex<ParameterList.Count)
            {
                ParamterNameProperty.stringValue=ParameterList[popOutIndex];
                ParamterHashProperty.intValue=  Animator.StringToHash(ParameterList[popOutIndex]);
                
                //Type
                var ParamTypeNameProperty=property.FindPropertyRelative(ParamTypePropName);
                var enumValueFound=ParamTypes[popOutIndex];
                ParamTypeNameProperty.intValue=(int)ParamTypes[popOutIndex];
                ParamType=ParamTypes[popOutIndex].ToString();
                
            }
        }
        else
        {   
            popOutIndex = EditorGUI.Popup(rectStates,popOutIndex, ParameterList.ToArray());
            ParamterNameProperty.stringValue="";
            popOutIndex=-1;
        }
        var rectType= new Rect(rectStates.xMax+Gap,minY + (lines) * GetSingleLineHeightRect(), rightSizeWidth-(rectAnim.width+rectAnim.width)-(2*Gap), EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(rectType,ParamType);
        
        property.FindPropertyRelative(IndexPropName).intValue=popOutIndex;
        

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

        if(EditorGUIUtility.wideMode==false)
        {
            totalLines++;
        }

        

        return GetSingleLineHeightRect() * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}
