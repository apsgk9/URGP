using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

/*
   A class that allows multiple objects to have one single main object to focus on.
   This can be cameras which can their only be one camera or a characters which one character
   can only be a main character for which a main camera can focus on.
*/
/*
   Might be a problem when there are more than one MainObjects. 
      -  probably either have a priority onto who becomes Main Object
      -  or whoever has the main object last becaomes the main object.
*/
/*
   Assumes there's only one Main Object in the transform. The renaming part might be a wonky
   - Since I haven't tested it.
*/

namespace MainObject
{

    public abstract class MainObject<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("Object Properties")]
        public static MainObject<T> Main; //for some reason it doesn't differentiate prefab editor or scene
        public static List<MainObject<T>> MObjectList = new List<MainObject<T>>();
        public static List<T> ComponentList = new List<T>();
        public List<MainObject<T>> _MObjectList;
        public bool isMain { get { return _isMain; } set { SetMainStatus(value); } }
        public T Component; //For Editor purposes 

        [SerializeField]
        private bool _isMain;
        private bool _previousMain;
        public static string MainPrefixName { get { return mainPrefix; } }


        private static string mainPrefix = "-[Main " + typeof(T).Name + "]-";
        public static event Action<MainObject<T>> NewMainObjectHasBeenSet;
        public static event Action<MainObject<T>> OldMainObjectHasBeenReplaced;
        public static event Action<MainObject<T>> NewObjectHasSpawned;
        public static event Action<MainObject<T>> ObjectHasBeenDestroyed;
        public MainObject<T> LocalMainRef; //For Editor purposes
        virtual protected void Awake()
        {
            _MObjectList=  new List<MainObject<T>>();
            Component= GetComponent<T>();//.GetCurrentStageHandle().FindComponentOfType<T>();
            if(!MObjectList.Contains(this))
            {
                MObjectList.Add(this);
                ComponentList.Add(Component);
                NewObjectHasSpawned?.Invoke(this);
            }
            HandleMain();
        }
        virtual protected void OnDestroy()
        {
            if(MObjectList.Contains(this))
            {
                MObjectList.Remove(this);
                ComponentList.Remove(Component);
            }
            if(MObjectList.Count<=0) //seems to persist even in editor, so remove runtime
            {
                Main=null;
            }
            ObjectHasBeenDestroyed?.Invoke(this);
        }
        private void OnApplicationQuit()
        {
            Main=null;
            ComponentList.Clear();
            MObjectList.Clear();
        }

        virtual protected void OnValidate()
        {
            Component= GetComponent<T>();//.GetCurrentStageHandle().FindComponentOfType<T>();
            HandleMain();
            _previousMain=_isMain;
        }

        private void HandleMain()
        {
            if (_isMain == true)
            {
                SetAsMain();
            }
            else
            {
                RemoveAsMain();
            }
        }

        virtual protected void Update()
        {
            _MObjectList=MObjectList;
           bool hasChanged= _previousMain!=_isMain;
           if(hasChanged)
           {
              HandleMain();
              _previousMain=_isMain;
           }
            
        }

        #region Set or Remove As Main
        public static void SetAsMain(MainObject<T> tobeMainObject)
        {
            bool isInPrefabEditor=false;
#if UNITY_EDITOR
            //Seems to be more stable rather than (tobeMainObject == Main). This can let the locamMainRef be stil there
            //when unticked from ticked.
            isInPrefabEditor= PrefabStageUtility.GetCurrentPrefabStage()!=null;
            if(isInPrefabEditor && tobeMainObject == tobeMainObject.LocalMainRef)
            {                
                //Debug.Log(tobeMainObject.name + " is already Main.");
                return;
            }
            
#endif
            if(!isInPrefabEditor && tobeMainObject == Main)
            {                
                //Debug.Log(tobeMainObject.name + " is already Main.");
                return;
            }

            var previousMain = SetToMainObject(tobeMainObject);
            RemoveMainStatusFromAll_T_Objects(tobeMainObject);
            SetLocalRefToAll_T_Objects(tobeMainObject);
            tobeMainObject._isMain = true;
            NewMainObjectHasBeenSet?.Invoke(tobeMainObject);
            OldMainObjectHasBeenReplaced?.Invoke(previousMain);
            //Debug.Log("SetAsMain: "+tobeMainObject.name);
        }
        
        public void SetAsMain()
        {
            SetAsMain(this);
        }
        public static void RemoveAsMain(MainObject<T> tobeRemovedAsMain)
        {
            RemovePrefixName(tobeRemovedAsMain);
            if (tobeRemovedAsMain.LocalMainRef == tobeRemovedAsMain)
            {
                //Debug.Log("Removing All cause this is main: "+tobeRemovedAsMain.name);
                SetLocalRefToAll_T_Objects(null);
            }
            //tobeRemovedAsMain.LocalMainRef = null;
            tobeRemovedAsMain._isMain = false;
            //Debug.Log("RemoveAsMain: "+tobeRemovedAsMain.name);
        }
        public void RemoveAsMain()
        {
            RemoveAsMain(this);
        }
        private void SetMainStatus(bool Status)
        {
            if(Status)
            {                
                SetAsMain();
            }
            else
            {
                RemoveAsMain();
            }
        }
        #endregion

        private static MainObject<T> SetToMainObject(MainObject<T> mainTObject)
        {
            var previousMain=Main;
            Main = mainTObject;
            string mainTObjectName = mainTObject.gameObject.name;
            if(!mainTObjectName.Contains(mainPrefix))
            {
                mainTObject.gameObject.name = mainTObjectName.Insert(0, mainPrefix);
                //Debug.Log("Added Prefix Name to: "+mainTObjectName);
            }
                //Debug.Log("Prefix Name already added to: "+mainTObjectName);
            return previousMain;
        }


        private static void RemoveMainStatusFromAll_T_Objects(MainObject<T> mainObject)
        {
            //Debug.Log("THEEE MAIN OBJECT: "+mainObject.name);
            //if(Main)
            //{
            //    Debug.Log("Current MAIN OBJECT: "+Main.name);
            //}
            //else
            //{
            //    Debug.Log("Current MAIN IS NULL");
            //}
            MainObject<T>[] AllMainObjectsinScene = GetAllMainObjectsinScene();
            foreach (var mObject in AllMainObjectsinScene)
            {
                if (mObject != mainObject)
                {
                    RemovePrefixName(mObject);
                    mObject._isMain = false;
                }
            }
        }

        private static MainObject<T>[] GetAllMainObjectsinScene()
        {
#if UNITY_EDITOR
            return StageUtility.GetCurrentStageHandle().FindComponentsOfType<MainObject<T>>();
#else
            return  GameObject.FindObjectsOfType<MainObject<T>>();
#endif
        }

        private static void SetLocalRefToAll_T_Objects(MainObject<T> mainObject)
        {
            MainObject<T>[] AllMainObjectsinScene = GetAllMainObjectsinScene();
            foreach (var mObject in AllMainObjectsinScene)
            {
                mObject.LocalMainRef = mainObject;
            }
        }

        private static void RemovePrefixName(MainObject<T> mObject)
        {
            string oldTObjectName = mObject.gameObject.name;
            mObject.gameObject.name = oldTObjectName.Replace(mainPrefix, "");
           // Debug.Log("Removed Prefix Name from: "+oldTObjectName);
        }
        public static void Refresh()
        {
            MainObject<T>[] AllMainObjectsinScene = GetAllMainObjectsinScene();
            foreach (var mObject in AllMainObjectsinScene)
            {
                if(mObject.isMain)
                {
                    SetAsMain(mObject);
                    return;
                }
            }
        }
    }
}