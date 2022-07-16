using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.MenuController
{
    

public enum Orientation
{
    Vertical,
    Horizontal
}
public enum Direction
{
    Positive = 1,
    Negative = -1,
    None = 0
}


//There will be a problem if you change orientation midway since the offsets aren't properly aligned.
//By Default, Menu is deselected.
/*
    When a new shiftable has been selected. The old shiftable will be deselected.
*/
[RequireComponent(typeof(MenuControllerUIRefresher))]
public abstract class MenuController : MonoBehaviour
{
    [Header("Base Properties")]
    public bool isFocused;    
    public bool CanChangeInteractableProperty {get {return _canChangeInteractableProperty; }}
    [SerializeField] protected bool _canChangeInteractableProperty = false;
    
    public int currentIndexSelected;
    protected int _previousIndex;
    [SerializeField] 
    protected RectTransform rectTransform;
    public HorizontalOrVerticalLayoutGroup HVLayoutGroup;
    [SerializeField]
    protected Orientation Orientation = Orientation.Vertical;
    [SerializeField] [ReadOnly]
    protected bool transitioning = false;
    public bool isTransitioning  {get{return transitioning;}}
    [SerializeField]
    protected float TimeDuration {get{return GetTimeDuration();}}
    [SerializeField]
    protected bool UseDefaultTime = false;

    [SerializeField]
    protected float DefaultTimeDuration = 0.25f;


    protected float TransitionCurrentTime;
    protected float TransitionStartTime;

    [SerializeField] [ReadOnly]
    protected Vector2 CurrentOffsetMax;
    [SerializeField] [ReadOnly]
    protected Vector2 TargetOffSetMax;
    public bool isPressPositive { get{return _isPressPositive;}}
    public bool isPressNegative { get{return _isPressNegative;}}

    public abstract int GetCount();

    public bool isPressConfirmed { get{return _isPressConfirmed;}}

    protected bool _isPressPositive;
    protected bool _isPressNegative;
    protected bool _isPressConfirmed;
    private MenuSettings _MenuSettings;
    protected Action _FuncToPlayAtEndOfTransition;
    public bool Active {get;private set;}

    public AnimationCurve TransitionCurve=AnimationCurve.EaseInOut(0,0,1,1);
    [HideInInspector]

    public IMenuShiftable CurrentSelectedShiftable {get{return m_CurrentSelectedShiftable;}
    set
        {
            if(m_CurrentSelectedShiftable!=value && m_CurrentSelectedShiftable!=null && !m_CurrentSelectedShiftable.Destroyed)// && !m_CurrentSelectedShiftable.Destroyed)
            {
                m_CurrentSelectedShiftable.Deselect();
            }            
            m_CurrentSelectedShiftable=value;
            m_CurrentSelectedShiftable.Select();
        }
    }
    [SerializeField] [ReadOnly]
    private IMenuShiftable m_CurrentSelectedShiftable;

        virtual protected float GetTimeDuration()
    {
        if(_MenuSettings==null || UseDefaultTime)
        {
            return DefaultTimeDuration;
        }
        else
        {
            return _MenuSettings.MenuSpeed;
        }
    }

    virtual protected void Awake()
    {
        if(rectTransform==null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        try
        {
            if(Service.ServiceLocator.Current!=null && Service.ServiceLocator.Current.Exists<SettingsManager>())
            {
                _MenuSettings=Service.ServiceLocator.Current.Get<SettingsManager>().GetMenuSettings();
            }
        }
        catch{}
        
        HVLayoutGroup=GetComponent<HorizontalOrVerticalLayoutGroup>();
        SetFocus(isFocused);
    }
    virtual protected void OnValidate(){}
    virtual protected void OnEnable()
    {
        Active=true;
    }
    virtual protected void OnDisable()
    {
        Active=false;
    }


    virtual protected void Start()
    {
        _isPressPositive = _isPressNegative = false;        
    }
    ///<summary>Refreshes currently selected Button</summary>
    public abstract void SetCurrentSelectable();
    public abstract bool RemoveShiftable(IMenuShiftable shiftable);
    ///<summary>Adds Shiftable to the system at the end when possible</summary>
    public abstract bool AddShiftable(IMenuShiftable shiftable);
    ///<summary>Adds Shiftable to the system at the inserted index (0 base) when possible</summary>
    public abstract bool AddShiftable(IMenuShiftable shiftable, int insertAt);

    ///<summary>Refresh the UI Controller based on the current parameters. (i.e. change the size when number of Shiftables change)</summary>
    public abstract void RefreshUI();
    ///<summary>Do a Scroll Up Movement based on the Type of MenuController/summary>
    protected abstract void ScrollUp();

    ///<summary>Do a Scroll Down Movement based on the Type of MenuController/summary>
    protected abstract void ScrollDown();

    //protected abstract void ScrollToIndex(int index);

    ///<summary>Decides whether or not if the MenuController should scroll or stay in place.</summary>
    protected abstract bool ShouldScroll();
    ///<summary>Returns the Vector2 position that moves the MenuControler in the negative direction.</summary>
    protected abstract Vector2 DownDelta();
    ///<summary>Returns the Vector2 position that moves the MenuControler in the positive direction.</summary>
    protected abstract Vector2 UpDelta();
    ///<summary>Returns the Vector2 position that can move the MenuController to the end.</summary>
    protected abstract Vector2 GetEndPosition();
    ///<summary>Returns the Vector2 position that can move the MenuController to the beginning.</summary>
    protected  abstract Vector2 GetBeginningPosition();

    ///<summary>Returns the Vector2 position of the button at the given index.</summary>
    protected abstract Vector2 GetIndexPosition(int index);
    ///<summary>Returns the pixel dimensions of the shiftables this MenuController is using.</summary>
    public abstract float GetPixelDimension();
    protected Vector2 OrientVector(float vector)
    {
        if (Orientation == Orientation.Vertical)
        {
            return new Vector2(0, vector);
        }
        else
        {
            return new Vector2(-vector, 0);
        }
    }
    ///<summary>Process what to do when shiftable has been entered by a pointer.</summary>

    public abstract void OnEnterShiftable(IMenuShiftable menuButton);

    ///<summary>Process what to do when shiftable has been exited by a pointer.</summary>
    public abstract void OnExitShiftable(IMenuShiftable menuButton);

    ///<summary>Stops the current transition if occuring and calls EndOfTransition. This does not finish the transition</summary>
    virtual public void StopTransition()
    {
        if(transitioning==false)
            return;
        transitioning = false;
        EndOfTransition();
    }

    virtual public void FinishTransition()
    {
        if(transitioning==false)
            return;
        TransitionToNewButton(true);
    }

    ///<summary>Setups transitions in order for TransitionToNewButton to work.</summary>
    virtual protected void SetupTransition(Vector2 Target)
    {
        TransitionStartTime = Time.time;
        TransitionCurrentTime = 0f;

        CurrentOffsetMax = rectTransform.anchoredPosition;
        TargetOffSetMax = Target;
        transitioning = true;
    }


    protected bool EqualToCurrentAnchoredPosition(Vector2 position)
    {
        return (rectTransform.anchoredPosition==position);
    }

    
    ///<summary>Moves the anchored position of the rectransform from CurrentOffsetMax to TargetOffSetMax </summary>
    protected void TransitionToNewButton(bool instant = false)
    {

        if (TimeDuration < 0.0001 || instant)
        {
            TransitionCurrentTime=Mathf.Infinity;
        }
        else if(TransitionCurrentTime < TimeDuration)
        {
            TransitionCurrentTime += Time.deltaTime;
            float t = TransitionCurrentTime / TimeDuration;
            float value = TransitionCurve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.Lerp(CurrentOffsetMax, TargetOffSetMax, value);
        }
        
        if (TransitionCurrentTime > TimeDuration)
        {
            TransitionCurrentTime=Mathf.Infinity;
            rectTransform.anchoredPosition=TargetOffSetMax;
            transitioning = false;
            EndOfTransition();
        }
    }
    

    ///<summary>Calls functions that should be processed at the end of transitions.</summary>
    protected abstract void EndOfTransition();

    public void OnPressConfirm()
    {
        _isPressConfirmed = true;
    }

    public void OnPressDown()
    {
        _isPressNegative = true;
    }


    public void onPressUp()
    {
        _isPressPositive = true;
    }

    public void OnReleaseConfirm()
    {
        _isPressConfirmed = false;
    }

    public void OnReleaseDown()
    {
        _isPressNegative = false;
    }

    public void OnReleaseUp()
    {
        _isPressPositive = false;
    }
    

    ///<summary>Receives a MoveDirection and calculates how the menu should move.</summary>
    public abstract void OnMove(MoveDirection moveDir);
    ///<summary>Receives a Vector2 and calculates how the menu should move.</summary>
    public abstract void OnMove(Vector2 NavigateVector);
    ///<summary>Receives a float value and calculates how the menu should move.</summary>
    public abstract void  OnMove(float navigateValue);

    /*
        Controls how the Menu Control should act when set to a certain focus mode. Ideally it shouldn't move
        when not in focus and vice versa.
    */
    /// 
    ///<summary>Changes the focus mode of the MenuController.</summary>
    public abstract void SetFocus(bool focusMode);
    public Orientation GetOrientation()
    {
        return Orientation;
    }
    public void ChangeInteractable(bool SetInteractable)
    {
        _canChangeInteractableProperty=SetInteractable;
    }
    ///<summary>Moves the current index of the MenuControler by given index</summary>
    public abstract void TranslateIndexBy(int indexToMoveBy);
    public abstract IMenuShiftable GetMenuShiftable(int index);
}

}