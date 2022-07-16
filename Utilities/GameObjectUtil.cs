using UnityEngine;

namespace UtilityFunctions
{
    public static class GameObjectUtil
    {
        public static void ParentToContainer(ref GameObject GameObjectParent,
        string GameObjectParentName,Transform childTransform)
        {
            if (GameObjectParent == null)
            {
                if (GameObject.Find(GameObjectParentName) == null)
                {
                    GameObjectParent = new GameObject();
                    GameObjectParent.name = GameObjectParentName;
                }
            }
            childTransform.SetParent(GameObjectParent.transform);            
        }        
    }
}

