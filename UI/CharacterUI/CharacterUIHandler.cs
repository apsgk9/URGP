using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Follow;
using Attachment;

namespace UI.Character
{
    public class CharacterUIHandler : MonoBehaviour
    {
        public GameObject CharacterObject;
        public List<AttachmentObject> UIToSpawn;

        // Start is called before the first frame update
        void Start()
        {            
            foreach(var aObject in UIToSpawn)
            {
                if (aObject.AssetRefObject.RuntimeKeyIsValid() == false)
                {
                    Debug.Log("Invalid Key " + aObject.AssetRefObject.RuntimeKey.ToString());
                    continue;
                }

                aObject.AssetRefObject.InstantiateAsync().Completed += (asyncOperationHandle) =>
                {
                    GameObject resultGO = asyncOperationHandle.Result;
                    AddressableFunctions.Handler.AddNotifyOnDestroy(aObject.AssetRefObject, resultGO);

                    //FollowUtil.AddFollowScript(aObject, resultGO);

                    FollowUtil.AddFollowScript(aObject.Offset,aObject.Anchor,resultGO,true,aObject.Type,aObject.SmoothSpeed);
                    
                    SetupCharacterUI(resultGO);
                };
            }
        }

        private void SetupCharacterUI(GameObject resultGO)
        {
            //--POTENTIALL MIGHT GET INTENSIVE SINCE IT HAS TO SEARCH FOR EVERY CHILD OBJECT---
            var charUIarr= resultGO.GetComponentsInChildren<ICharacterUI>();
            //ICharacterUI CharUI= resultGO.GetComponent<ICharacterUI>();
            for (int i = 0; i < charUIarr.Length; i++)
            {                
                charUIarr[i].SetCharacter(CharacterObject);                
            }
        }
    }

}
