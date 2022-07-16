using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
/*
public abstract class AnimatorStateMachineBase
{
    protected static AnimatorController CreateController(string smName="StateMachine")
    {
        var name=smName;
        var path = "";
        int cntr = 0;
        var selectedObject = Selection.activeObject;

        System.Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();

        Debug.Log(pathToCurrentFolder);

        path = pathToCurrentFolder + "/" + name + ".asset";

        while (!ObjectTypeDoesntExists(path, typeof(AnimatorController)))
        {
            path = pathToCurrentFolder + "/" + name + cntr.ToString() + ".asset";
            Debug.Log(path);

            cntr++;
            if (cntr > 10)
            {
                break;
            }
        }

        return AnimatorController.CreateAnimatorControllerAtPath(path);
    }

    protected static bool ObjectTypeDoesntExists(string path, System.Type type)
    {
        var ap = AssetDatabase.LoadAssetAtPath(path, type);
        if (ap == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
*/