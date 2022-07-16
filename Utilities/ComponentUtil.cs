using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UtilityFunctions
{
    public static class ComponentUtil
    {
        public static T FindComponentWithinGameObject<T>(GameObject go) where T : Behaviour
        {
            T componentRef = null;
            if (componentRef == null)
            {
                componentRef = go.GetComponent<T>();
            }
            if (componentRef == null)
            {
                componentRef = go.GetComponentInParent<T>();
            }
            if (componentRef == null)
            {
                componentRef = go.transform.root.GetComponentInChildren<T>();
            }
            if (componentRef == null)
            {
                return null;
            }
            else
            {
                return componentRef;
            }
        }
    }
}

