using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.MenuController;
public abstract class ShiftableSelectable : Selectable , IMenuShiftable, IEventSystemHandler 
{
    //------------------------
    public bool Selected { get => _Selected; set => _Selected=value; }
    protected bool _Selected;
    [SerializeField] [ReadOnly]
    protected MenuController _MenuControllerParent;
    protected RectTransform _rectTransform;
    //protected int _index;
    private bool isQuitting=false;
    public int index {get {return transform.GetSiblingIndex();}}
    public float GetPixelHeight { get {return PixelHeight();}}
    public float GetPixelWidth { get {return PixelWidth();}}
    public bool Destroyed {get; private set;}

    public RectTransform rectTransform => _rectTransform;


    virtual protected float PixelHeight()
    {        
        return Shiftable.Utility.PixelHeight(isQuitting,ref _rectTransform,gameObject);
    }
    virtual protected float PixelWidth()
    {
        return Shiftable.Utility.PixelWidth(isQuitting,ref _rectTransform,gameObject);
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if(!Application.isPlaying)
            return;
        base.OnValidate();
        targetGraphic.raycastTarget=interactable;
        image.enabled=true;
        Shiftable.Utility.AddToMenuParent(isQuitting,transform,this);
    }
#endif


    protected override void Awake()
    {
        //prevents problems when spawning
        Selected=false;
        base.Awake();        
        Setup();
    }

    virtual protected void Setup()
    {
        Shiftable.Utility.Setup(transform,ref _MenuControllerParent,ref _rectTransform);//,ref _index);
        HandleNavigation();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        
    }
    public override void OnSelect(BaseEventData eventData)
    {        
        base.OnSelect(eventData);
    }

    protected override void OnEnable()
    {
        HandleNavigation();
        base.OnEnable();
        Shiftable.Utility.AddToMenuParent(isQuitting,transform,this);
    }

    protected override void OnDestroy()
    {
        Destroyed=true;
        if (_MenuControllerParent != null && !_MenuControllerParent.Active)
            return;
        Shiftable.Utility.RemoveFromMenuParent(isQuitting, transform, this);
    }

    /*
    public void SetIndex(int setIndex)
    {
        if(isQuitting || (_MenuControllerParent!=null && !_MenuControllerParent.Active))
        {
            return;
        }
        Shiftable.Utility.SetIndex(setIndex,transform,ref _index);
    }*/

    //public abstract void Select(); Select is implemented by Selectable
    public abstract void Deselect();

    private void OnApplicationQuit()
    {
        isQuitting=true;
    }    

    public void SetInteractable(bool canInteract)
    {
        if(canInteract==interactable)
            return;
        //Debug.Log("SET INTERACT: "+ gameObject.name + " : "+canInteract);
        interactable=canInteract;
        targetGraphic.raycastTarget=canInteract;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(canInteract);            
        }

//#if UNITY_EDITOR
//        if(!Application.isPlaying)
//            return;
//#endif
        var behaviours = GetComponents<Behaviour>();
        foreach (var item in behaviours)
        {
            if(item!=this && !(item is LayoutElement))
            {
                item.enabled=canInteract;
            }
            else
            {
                item.enabled=true;
            }            
        }
    }
    private void HandleNavigation()
    {
        //UnityEngine.UI.Navigation N = new UnityEngine.UI.Navigation();
        //N.mode=UnityEngine.UI.Navigation.Mode.Automatic;
        //navigation=N;

        if(_MenuControllerParent!=null && _MenuControllerParent is MenuControllerShiftable)
        {
            UnityEngine.UI.Navigation N = new UnityEngine.UI.Navigation();
            N.mode=UnityEngine.UI.Navigation.Mode.None;
            navigation=N;
        }
        
    }
//-------------------

}
