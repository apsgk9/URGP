using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UI.MenuController;
using System;
using UI.Button;

public class ButtonMenu :ShiftableSelectable, IMenuShiftable,  ISubmitHandler , IPointerClickHandler,
IButtonConfirmCallback,IButtonSelectCallback,IButtonDeselectCallback
{
    [SerializeField]
    private AnimatorParamaterList _PressedParameter;
    [SerializeField]
    private ScrollRectMenu _ScrollRectMenu;
    private new Animator animator {get{return GetAnimator();}}

    
    private Animator _animator;
    [SerializeField]
    public Action OnButtonConfirm { get ; set; }
    public Action OnButtonSelect { get; set; }
    public Action OnButtonDeselect { get; set; }

    public Action<ButtonMenu> HasBeenDestroyed { get ; set ; }
    
    //Ideally There would be only one selected, but just letting this be so I can catch errors.

    private Animator GetAnimator()
    {
        if(_animator==null)
        {
            _animator = GetComponent<Animator>();
        }
        return _animator;
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        GetScrollRect();
    }
#endif
    protected override void Setup()
    {
        GetScrollRect();
        base.Setup();
        Check();
        _Selected=false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        HasBeenDestroyed?.Invoke(this);
    }

    private void GetScrollRect()
    {
        _ScrollRectMenu=transform.GetComponentInParent<ScrollRectMenu>();
    }

    private void Check()
    {
        if(_PressedParameter==null)
        {
            return;
        }

        if (_PressedParameter.Type != AnimatorControllerParameterType.Int)
        {
            Debug.LogWarning("Selected Parameter must be an int");
        }
    }

    public override void Select()
    {
        if(_Selected)
            return;

        //For Multiple MenuControllers        
        if(_MenuControllerParent!=null && !_MenuControllerParent.isFocused)
        {
            Deselect();
            return;
        }

        if(!gameObject.activeSelf)
            gameObject.SetActive(true);

        if(animator.isActiveAndEnabled)
        {   
            animator.SetInteger(_PressedParameter.Hash, (int)ButtonState.Selected);
        }
              
        DoStateTransition(SelectionState.Selected, false);
        _Selected=true;
        //Debug.Log(Time.frameCount + " : SELECT "+gameObject.name);
        base.Select();
        OnButtonSelect?.Invoke();
        //}
    }
    public override void Deselect()
    {
        if(_Selected==false)
            return;

        if(animator.isActiveAndEnabled)
        {   
            animator.SetInteger(_PressedParameter.Hash, (int)ButtonState.Deselected);
        }
        if(interactable)
        {
            DoStateTransition(SelectionState.Normal, false);
        }
        else
        {
            DoStateTransition(SelectionState.Disabled, false);
        }
        _Selected=false;
        OnButtonDeselect?.Invoke();
        //Debug.Log(Time.frameCount + " : DESELECT "+gameObject.name);
    }
    private void LateUpdate()
    {
        FixDeselection();
    }

    /*
        This is a hacky way to do it. This basically fixes the deselect problem
        when the user tries to drag and ends up deselecting the currently selectedButton.

    */
    private void FixDeselection()
    {
        if(_Selected && _MenuControllerParent!=null && _MenuControllerParent.isFocused && !_MenuControllerParent.isTransitioning &&
         EventSystem.current!=null && EventSystem.current.currentSelectedGameObject!=gameObject)
        {
            //Debug.Log(Time.frameCount+ ": FIXDESELECT" + gameObject.name);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        
    }
    public override void OnSelect(BaseEventData eventData)
    {        
        base.OnSelect(eventData);
    }

    

    

    private bool ButtonCanBeSelected(bool checkCursor=true)
    {
        if(checkCursor==true && (!Cursor.visible || Cursor.lockState == CursorLockMode.Locked) )
        {
            //Debug.Log("CURSOR");
            return false;
        }
        //Scroll Conditions
        if(_ScrollRectMenu!=null && (_ScrollRectMenu.isSlowEnoughToSelect==false|| (_ScrollRectMenu.BeingDragged&& !_ScrollRectMenu.isScrolling) ) )
        {
            //Debug.Log("_ScrollRectMenu.isSlowEnoughToSelect:"+_ScrollRectMenu.isSlowEnoughToSelect);
            //Debug.Log("_ScrollRectMenu.BeingDragged:"+_ScrollRectMenu.BeingDragged);
            //Debug.Log("_ScrollRectMenu.isScrolling:"+_ScrollRectMenu.isScrolling);
            //return false;
        }     
        if(_MenuControllerParent!=null)
        {
            //Debug.Log("_MenuControllerParent.isFocused: "+_MenuControllerParent.isFocused);
            return _MenuControllerParent.isFocused;

        }
        return true;

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if(!ButtonCanBeSelected())
            return;
        

        if(interactable)
        {
            base.OnPointerEnter(eventData);
            if(_MenuControllerParent)
            {
                _MenuControllerParent.OnEnterShiftable(this);
            }
        }
    }    
                        
    public override void  OnPointerExit(PointerEventData eventData)
    {
        if(!ButtonCanBeSelected())
            return;

        base.OnPointerExit(eventData);
        if(_MenuControllerParent)
        {
            _MenuControllerParent.OnExitShiftable(this);
        }
    }

        
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!Selected)
            return;
        //if(!ButtonCanBeSelected())
        //    return;
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {        
        //This should only play when OnPointerDown is called
        //if(!ButtonCanBeSelected())
        //    return;
        base.OnPointerUp(eventData);
    }
    /*
        ConfirmStart occurs when button is fully pressed down
    */
    public void OnPointerClick(PointerEventData eventData)
    {
        if(!Selected)
            return;
        if(!ButtonCanBeSelected())
            return;
        ConfirmStart();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if(!ButtonCanBeSelected(false))
            return;
        SubmitStart();
    }


    private void SubmitStart()
    {
        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private void ConfirmStart()
    {
        if (animator.isActiveAndEnabled)
        {
            animator.SetInteger(_PressedParameter.Hash, (int)ButtonState.Pressed);
        }
        OnButtonConfirm?.Invoke();
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        ConfirmStart();

        DoStateTransition(currentSelectionState, false);//transitionBack
    }

    public override void OnMove(AxisEventData eventData)
    {
        if(interactable)
        {
            if(_MenuControllerParent)
            {
                _MenuControllerParent.OnMove(eventData.moveDir);
            }
        }        
    }

    
}
