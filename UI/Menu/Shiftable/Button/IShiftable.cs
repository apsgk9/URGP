using UnityEngine;

public interface IShiftable
{
    bool interactable { get;}
    float GetPixelHeight { get; }
    float GetPixelWidth { get; }
    int index { get; }
    GameObject gameObject { get; }
    RectTransform rectTransform { get; }
    //void SetIndex(int setIndex);
    void SetInteractable(bool canInteract);
}