using System.Collections;
using System.Collections.Generic;
using UI.MenuController;
using UnityEngine;

public abstract class MenuShiftable : MonoBehaviour, IMenuShiftable
{
    //------------------------
    public bool Selected { get => _Selected; set => _Selected=value; }
    [Header("MenuShiftable")]
    [SerializeField] [Tooltip("Determines if the the shiftable is currently selected.")]
    protected bool _Selected;
    [SerializeField] [Tooltip("Determines if the the shiftable can be interacted with.")]
    protected bool _interactable;
    
    private bool isQuitting=false;
    
    [SerializeField] [ReadOnly]
    protected MenuController _MenuControllerParent;
    protected RectTransform _rectTransform;
    public RectTransform rectTransform => _rectTransform;
    //protected int _index;
    public int index {get {return transform.GetSiblingIndex();}}
    public float GetPixelHeight { get {return PixelHeight();}}
    public float GetPixelWidth { get {return PixelWidth();}}

    public bool interactable { get {return _interactable;}}

    public bool Destroyed {get; private set;}
    private void OnDestroy()
    {
        Destroyed=true;
    }

    virtual protected float PixelHeight()
    {        
        return Shiftable.Utility.PixelHeight(isQuitting,ref _rectTransform,gameObject);
    }
    virtual protected float PixelWidth()
    {
        return Shiftable.Utility.PixelWidth(isQuitting,ref _rectTransform,gameObject);
    }

    virtual protected void Awake()
    {
        Shiftable.Utility.Setup(transform,ref _MenuControllerParent,ref _rectTransform);//,ref _index);
    }
    private void OnValidate()
    {        
        Shiftable.Utility.Setup(transform,ref _MenuControllerParent,ref _rectTransform);//ref _index);        
    }

    virtual protected void OnEnable()
    {
        Shiftable.Utility.AddToMenuParent(isQuitting,transform,this);        
    }

    virtual protected void OnDisable()
    {
        Shiftable.Utility.RemoveFromMenuParent(isQuitting,transform,this);
    }

    public abstract void Select();
    public abstract void Deselect();
    
    private void OnApplicationQuit()
    {
        isQuitting=true;
    }
    /*

    virtual public void SetIndex(int setIndex)
    {
        if(isQuitting || !_MenuControllerParent.Active)
            return;
        Shiftable.Utility.SetIndex(setIndex,transform,ref _index);
    }*/

    virtual public void SetInteractable(bool canInteract)
    {
        _interactable=canInteract;
        if(transform.childCount>0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(canInteract);
            }
        }
        var canvasRenderer= GetComponent<CanvasRenderer>();
        if(canvasRenderer!=null)
        {
            canvasRenderer.SetAlpha(System.Convert.ToInt32(canInteract));
        }

    }
}
