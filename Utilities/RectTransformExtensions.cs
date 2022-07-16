
using UnityEngine;

public static class RectTransformExtensions
 {
     /*
     Assumes Rect is not rotated
     */
     public static Rect GetWorldRect(this RectTransform rectTransform)
     {
         Vector3[] corners = new Vector3[4];
         rectTransform.GetWorldCorners(corners);
         // Get the bottom left corner.
         Vector3 position = corners[0];
         
         Vector2 size = new Vector2(
             rectTransform.lossyScale.x * rectTransform.rect.size.x,
             rectTransform.lossyScale.y * rectTransform.rect.size.y);
 
         return new Rect(position, size);
     }

     public static bool Overlaps(this RectTransform a, RectTransform b) {
        return a.WorldRect().Overlaps(b.WorldRect());
    }
    public static bool Overlaps(this RectTransform a, RectTransform b, bool allowInverse) {
        return a.WorldRect().Overlaps(b.WorldRect(), allowInverse);
    }

    public static Rect WorldRect(this RectTransform rectTransform) {
        Vector2 sizeDelta = rectTransform.sizeDelta;
        float rectTransformWidth = sizeDelta.x * rectTransform.lossyScale.x;
        float rectTransformHeight = sizeDelta.y * rectTransform.lossyScale.y;

        Vector3 position = rectTransform.position;
        return new Rect(position.x - rectTransformWidth / 2f, position.y - rectTransformHeight / 2f, rectTransformWidth, rectTransformHeight);
    }
 }
 public static partial class RectTransformUtil
 {
     public static bool RectContainsAnother (RectTransform rct, RectTransform another)
    {
        return rct.Overlaps(another);
        
        //var r = rct.GetWorldRect();
        //var a = another.GetWorldRect();
        //return r.xMin <= a.xMin && r.yMin <= a.yMin && r.xMax >= a.xMax && r.yMax >= a.yMax;
    }
 }