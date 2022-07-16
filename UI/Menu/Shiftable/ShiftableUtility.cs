using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.MenuController;

namespace Shiftable
{
    public static partial class Utility
    {

        public static float PixelHeight(bool isQuitting,ref RectTransform _rectTransform,GameObject gameObject)
        {
            if (isQuitting)
                return 0;

            var LayoutElement=gameObject.GetComponent<UnityEngine.UI.LayoutElement>();
            if (LayoutElement!=null)
            {
                return LayoutElement.preferredHeight;                
            }
            
            if (_rectTransform == null)
                _rectTransform = gameObject.GetComponent<RectTransform>();

            return _rectTransform.sizeDelta.y;
        }
        public static float PixelWidth(bool isQuitting,ref RectTransform _rectTransform,GameObject gameObject)
        {
            if (isQuitting)
                return 0;

            var LayoutElement=gameObject.GetComponent<UnityEngine.UI.LayoutElement>();
            if (LayoutElement!=null)
            {
                return LayoutElement.preferredWidth;                
            }
            if (_rectTransform == null)
                _rectTransform = gameObject.GetComponent<RectTransform>();

            return _rectTransform.sizeDelta.x;
        }
        public static void Setup(Transform transform, ref MenuController _MenuControllerParent,
        ref RectTransform _rectTransform)//, ref int _index)
        {
            if (transform.parent)
            {
                _MenuControllerParent = transform.parent.GetComponent<MenuController>();
            }
            _rectTransform = transform.gameObject.GetComponent<RectTransform>();
            //_index = transform.GetSiblingIndex();
        }
        public static void RemoveFromMenuParent(bool isQuitting, Transform transform, IMenuShiftable menuShiftable)
        {
                
            if (isQuitting || !Application.isPlaying)
                return;
            if (transform.parent == null)
                return;
            var MenuController = transform.parent.GetComponent<MenuController>();
            if (MenuController)
            {
                MenuController.RemoveShiftable(menuShiftable);
            }
        }

        public static void AddToMenuParent(bool isQuitting,Transform transform, IMenuShiftable menuShiftable)
        {
            if (isQuitting || !Application.isPlaying)
                return;
            if (transform.parent == null)
                return;
            var MenuController = transform.parent.GetComponent<MenuController>();
            if (MenuController)
            {
                MenuController.AddShiftable(menuShiftable);
            }
        }
        public static void SetIndex(int setIndex, Transform transform, ref int indexProperty)
        {
            indexProperty = setIndex;
            if ( transform.GetSiblingIndex() != setIndex)
            {
                transform.SetSiblingIndex(setIndex);
            }
            
        }
    }

}
