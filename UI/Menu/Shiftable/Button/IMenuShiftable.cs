using UnityEngine;
using UnityEngine.EventSystems;
using UI.MenuController;
public interface IMenuShiftable :IShiftable
{
    
    bool Selected{get; set;}
    bool Destroyed { get; }

    void Select();
    void Deselect();

    

    //bool Destroyed {get;}
}
