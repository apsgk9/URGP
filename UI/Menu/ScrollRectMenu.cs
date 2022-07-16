using System;
using System.Collections;
using System.Collections.Generic;
using UI.MenuController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScrollRectMenu : ScrollRect,IPointerEnterHandler,IPointerExitHandler  //IPointerUpHandler ,
{
    private const int DifferenceThreshold = 10;
    //Ideally this value should be low enough that the speed doesn't go over to the other index
    public float MinimumVelocityToStartSelecting=50f;
    private const float VelocityToDecelerateAt=5f;
    public RectTransform CursorRectTransform;
    [SerializeField][ReadOnly]
    private ShiftableMenuPanel _ShiftableMenuPanel;
    [SerializeField][ReadOnly]
    private MenuController _MenuController;
    private bool _previousTransition;

    public bool isScrolling { get{return hasScrolling();}}
    public bool _scrollingLock = false;
    private MovementType _defaultMovementType;
    public bool BeingDragged {get; private set;}

    private Vector2 _previousVelocity;
    private bool isDecelerating;
    private bool _CanDrag;

    public bool isSlowEnoughToSelect {get; private set;}
    public float MaxVelocity=1000f;

    public Action FinishedScrolling;
    public Action DragInitiated;
    public Action DragReleased;
    public int _previousMenuControllerCount;
    //public Action CanBeSelectedAgain;

    private bool hasScrolling()
    {
        return velocity!=Vector2.zero;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if(!Application.isPlaying)
            return;
        RegisterClickCancel();
    }
    protected override void OnDisable()
    {
        isSlowEnoughToSelect=true;
        base.OnDisable();
        if(!Application.isPlaying)
            return;
        DeRegisterClickCancel();
    }

    private void RegisterClickCancel()
    {
        if (UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.MenuControls.Click.performed += OnClickCall;
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.Click.performed += OnClickCall;
        }
    }

    private void DeRegisterClickCancel()
    {
        if (UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.MenuControls.Click.performed -= OnClickCall;
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.Click.performed -= OnClickCall;
        }
    }

    private void OnClickCall(InputAction.CallbackContext context)
    {        
        
        if(Mathf.RoundToInt(context.ReadValue<float>())==0)
        {
            EndDrag();
        }
        else
        {
            if(!_MenuController.isFocused)
            return;
            if(_CanDrag)
            {
                StartDrag();
            }
        }
        
    }
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
    }

    

    protected override void Start()
    {
        base.Start();
        Setup();
        _defaultMovementType=movementType;
        isSlowEnoughToSelect=true;
        _previousMenuControllerCount=_MenuController.transform.childCount;
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Setup();
    }
#endif
    private void Update()
    {
        if(!Application.isPlaying)
            return;
        //Debug.Log(velocity);
        isDecelerating= velocity.sqrMagnitude<_previousVelocity.sqrMagnitude;
        LimitMaxSpeed();
        HandleSlowDown();
        HandleStopMovementOnTransition();
        HandleScrolling();
        _previousVelocity=velocity;
        
        //if(_previousMenuControllerCount==0 && _MenuController.transform.childCount!=0)
        //{
        //    _previousMenuControllerCount=_MenuController.transform.childCount;
        //    StopMovement();
        //    Debug.Log("STOPMOVEMENT");
        //    Debug
        //}   
    }
    protected override void LateUpdate()
    {
        
        bool SmallContent = _previousMenuControllerCount <= 1 && _MenuController.transform.childCount != 0;
        _previousMenuControllerCount=_MenuController.transform.childCount;
        if (SmallContent)
            return;
        base.LateUpdate();
        //if(_MenuController.transform.childCount<=1)
        //{
        //    StopMovement();
        //}
    }

    private void LimitMaxSpeed()
    {
        if(velocity.sqrMagnitude>(MaxVelocity*MaxVelocity))
        {
            //velocity= velocity*(Time.deltaTime*2f);
            velocity=Vector2.ClampMagnitude(velocity,MaxVelocity);        
        }
    }

    /*
        Scroll Rect will always decelerate when it is not dragged
    */
    private void HandleSlowDown()
    {
        if(BeingDragged || !isDecelerating)
            return;
        float _currenVelocity=0;
        if(vertical==false && horizontal==true)
        {
            _currenVelocity=velocity.x;
        }
        else if(vertical==true && horizontal==false)
        {
            _currenVelocity=velocity.y;
        }
        else
        {
            Debug.LogError("SOMETHING WRONG HAPPENED");
        }
        
        if(MathF.Abs(_currenVelocity)<=VelocityToDecelerateAt)
        {
            velocity= velocity*(Time.deltaTime*2f);
            if(velocity.sqrMagnitude<0)
            {
                velocity=Vector2.zero;
            }
        }
        
    }

    private void HandleScrolling()
    {
        
        //Minimum Velocity Before Stop that the Scroll Rect Can Be selected Again
        isSlowEnoughToSelect = (MinimumVelocityToStartSelecting * MinimumVelocityToStartSelecting) >= velocity.sqrMagnitude;
        if (isScrolling  && _MenuController.isTransitioning==false )
        {
            //Set Focus To the currentMenuController
            if(_ShiftableMenuPanel!=null)
            {
                _MenuController.SetFocus(_ShiftableMenuPanel.Selected);                
            }
            else
            {
                _MenuController.SetFocus(true);
            }


            if(!CursorRectTransform)
                return;

            if (isSlowEnoughToSelect)
            {
                return;
            }



            int difference=0;
            int indexSelected=_MenuController.currentIndexSelected;
            if(_MenuController.GetMenuShiftable(indexSelected)==null)
                    return;
            
            Func<bool> IsWithinTarget=  () => 
            {
                return RectTransformUtil.RectContainsAnother(
                _MenuController.GetMenuShiftable(indexSelected).rectTransform,CursorRectTransform);
            };
            
            while(!IsWithinTarget())
            {
                if (isScrollingNegatively())
                {
                    if(_MenuController.GetOrientation()==Orientation.Vertical)
                    {
                        difference--;
                        indexSelected--;
                    }
                    else
                    {
                        difference++;
                        indexSelected++;
                    }
                    
                }
                else
                {
                    if(_MenuController.GetOrientation()==Orientation.Vertical)
                    {
                        difference++;
                        indexSelected++;
                    }
                    else
                    {
                        difference--;
                        indexSelected--;
                    }            
                }
                
                /*Kinda Janky, but essentially for some reason, for a single frame or so,
                its possible to miss the button transform, so it might go all the way to the end
                so its best to just skip this and go to next frame.
                */
                if(_MenuController.GetMenuShiftable(indexSelected)==null)
                    return;
                if(difference>= DifferenceThreshold)
                    return;
            }
            
            _MenuController.TranslateIndexBy(difference);            
            _scrollingLock=true;
        }        
        else 
        {
            if(_scrollingLock)
            {
                if(!BeingDragged)
                {
                    FinishedScrolling?.Invoke();
                    isSlowEnoughToSelect=true;
                }
                _scrollingLock=false;
            }
        }

        //Done Scrolling
    }

    private bool isScrollingNegatively()
    {
        return velocity.y < 0 || velocity.x < 0;
    }

    private void HandleStopMovementOnTransition()
    {
        if (_previousTransition != _MenuController.isTransitioning)
        {
            _previousTransition = _MenuController.isTransitioning;
            if (_MenuController.isTransitioning)
            {
                
                StopMovement();
                movementType=MovementType.Unrestricted;
            }
            else
            {
                movementType=_defaultMovementType;
            }
        }
    }

    private void Setup()
    {
        if (_ShiftableMenuPanel == null)
        {
            _ShiftableMenuPanel = GetComponentInParent<ShiftableMenuPanel>();
        }

        if (_MenuController == null && transform.childCount>0)
        {
            var ChildTransform = transform.GetChild(0);
            _MenuController = ChildTransform.GetComponent<MenuController>();
        }
        else
        {
            return;
        }

        if(_MenuController==null)
            return;
        if(_MenuController.GetOrientation()==Orientation.Horizontal)
        {
            vertical=false;
            horizontal=true;
        }
        else
        {            
            vertical=true;
            horizontal=false;
        }

        
    }
    

    public override void OnScroll(PointerEventData data)
    {
        if(_ShiftableMenuPanel)
        {
            _ShiftableMenuPanel.OnScroll(data);
        }
    }
    public void StartDrag()
    {
        //Debug.Log(Time.frameCount+ ": START DRAG "+gameObject.name);
        //Debug.Log(Time.frameCount+ ": START ANCHOREDPOSITION "+_MenuController.GetComponent<RectTransform>().anchoredPosition);
        _MenuController.StopTransition();
        BeingDragged = true;
        DragInitiated?.Invoke();
    }
    public void EndDrag()
    {
        if(BeingDragged==false)
            return;
        BeingDragged = false;
        DragReleased?.Invoke();
        //Debug.Log(Time.frameCount+ ": END DRAG "+gameObject.name);
        //Debug.Log(Time.frameCount+ ": END ANCHOREDPOSITION "+_MenuController.GetComponent<RectTransform>().anchoredPosition);
        if(velocity.sqrMagnitude==0)
        {
            FinishedScrolling?.Invoke();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_ShiftableMenuPanel)
        {
            _CanDrag=_ShiftableMenuPanel.ShouldScrollRectDrag();       
        }
        else
        {
            _CanDrag=true;   
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _CanDrag=false;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if(!_CanDrag || BeingDragged==false)
            return;
        base.OnBeginDrag(eventData);
        //Debug.Log("BEGIN DRAG");
    }
    public override void OnDrag(PointerEventData eventData)
    {        
        if(!_CanDrag || BeingDragged==false)
            return;
        base.OnDrag(eventData);
        //Debug.Log("OnDrag");
    }

}

