using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimatorStateList))][Serializable]
public class AnimatorStateListDrawer : PropertyDrawer
{
    private Animator sourceAnimator;
    private Animator CurrentAnimator;
    private const string AnimatorPropName = "Animator";
    private const string StateNamePropName = "Name";
    private const string LayerPropName= "Layer";
    private const string MotionIndexPropName= "m_motionIndex";
    private const float newlineThreshold = 500f;
    private int popOutIndex;
    private int Gap=5;

    public List<string> StateList;
    private float propertyWidth;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) 
    {
        var minY=position.min.y;
        var minX=position.min.x;
        propertyWidth=position.size.x;
        EditorGUI.BeginProperty(position, label, property);
        StateList=new List<string>();
        
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

        //Layer Field
        var rectLayer = new Rect(rectAnim.xMax+Gap,minY + lines * GetSingleLineHeightRect(), rightSizeWidth/10, EditorGUIUtility.singleLineHeight);        
        EditorGUI.PropertyField(rectLayer, property.FindPropertyRelative(LayerPropName),GUIContent.none);
        var LayerValue=property.FindPropertyRelative(LayerPropName).intValue;

        
        //Clamp Field
        var AnimatorProperty=property.FindPropertyRelative(AnimatorPropName);
        sourceAnimator= AnimatorProperty.objectReferenceValue as Animator;
        if(sourceAnimator!=null && !(sourceAnimator.GetCurrentAnimatorStateInfo(0). normalizedTime >= 1)) // the latter part check if animator is playing
        {
            int layerCount = (sourceAnimator.layerCount<=0)?0:(sourceAnimator.layerCount-1);
            int clampedValue = Mathf.Clamp(LayerValue, 0, layerCount);
            property.FindPropertyRelative(LayerPropName).intValue= clampedValue;
            LayerValue=clampedValue;            
        }
        

        var StateNameProperty=property.FindPropertyRelative(StateNamePropName);
        var rectStates= new Rect(rectLayer.xMax+Gap,minY + lines * GetSingleLineHeightRect(), rightSizeWidth-(rectLayer.width+rectAnim.width)-(2*Gap), EditorGUIUtility.singleLineHeight);       
        if(sourceAnimator!=null)
        { 
            StateList = AnimatorUtil.GetListOfStates(sourceAnimator.runtimeAnimatorController as AnimatorController,LayerValue);
            popOutIndex=property.FindPropertyRelative(MotionIndexPropName).intValue;
            popOutIndex = EditorGUI.Popup(rectStates,popOutIndex, StateList.ToArray());
            if(StateList.Count>0 && popOutIndex>=0 && popOutIndex<StateList.Count)
            {
                StateNameProperty.stringValue=StateList[popOutIndex];
            }
        }
        else
        {   
            popOutIndex = EditorGUI.Popup(rectStates,popOutIndex, StateList.ToArray());
            StateNameProperty.stringValue="";
            popOutIndex=-1;
        }
        
        property.FindPropertyRelative(MotionIndexPropName).intValue=popOutIndex;
        

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
