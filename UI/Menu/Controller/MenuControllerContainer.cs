using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UI.MenuController
{
/*
    TODO: Maybe add 
    Unsure about highlighting tbh. In this case, highlight and selected seem to be both the same.
*/
public class MenuControllerContainer : MenuControllerShiftable
{
    [Header("Container Properties")]
    [Min(1)] [SerializeField] private int containerSize = 3;
    [Min(1)] [SerializeField] private int _cursorIndex = 1;
    [SerializeField] private int _lowerButtonIndex;
    [SerializeField] private int _upperButtonIndex;
    private int _previouslowerButtonIndex;
    private int _previousUpperButtonIndex;
    private int _previousCursorIndex;
    private int _previousCurrentIndexSelected;
    [SerializeField] private int _containerbuffer = 2;

    private int GetMaxIndexContainerSize()
    {
        return containerSize - 1;
    }
    public int GetContainerSize()
    {
        return containerSize;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //cursorIndex = BeginningCursorIndex();
        SetupButtonInteractable();
        _previousCurrentIndexSelected = currentIndexSelected;
    }

    ///<summary>Calls functions when The Scroll Rect Has Finished. In this case the MenuController magnets to the bottom of the container</summary>
    protected override void ScrollRectHasFinishedScrolling()
    {
        if (transitioning == false && !_ScrollRectMenu.BeingDragged && !EqualToCurrentAnchoredPosition(GetIndexPosition(_lowerButtonIndex)))
        {
            _UseMagnet = true;
            //Debug.Log(GetIndexPosition(_lowerButtonIndex));
            //Debug.Log(rectTransform.anchoredPosition);
            SetupTransition(GetIndexPosition(_lowerButtonIndex));
            _FuncToPlayAtEndOfTransition = () => { _UseMagnet = false; };
        }
    }

    
    protected override void OnValidate()
    {
        if(containerSize<=0)
        {
            containerSize=1;
        }
        base.OnValidate();
        _previousCursorIndex = _cursorIndex;
        _previousCurrentIndexSelected = currentIndexSelected;
        
    }
    protected override void MoveToInitialTransform()
    {
        rectTransform.anchoredPosition = GetIndexPosition(_lowerButtonIndex);
    }

    public override void RefreshUI()
    {
        base.RefreshUI();

        int changeInIndexMovement = currentIndexSelected - _previousCurrentIndexSelected;
        SetCursorIndex(_cursorIndex);

        if (currentIndexSelected <= GetMaxIndexContainerSize())
        {
            _cursorIndex = currentIndexSelected + 1;
        }

        if (!_removingButton) //let RemoveShiftable handle the interactions
        {
            SetupButtonInteractable();
        }
    }

    #region Scrolling Transforms
    protected override Vector2 GetBeginningPosition()
    {
        if (MenuShiftables.Count > containerSize)
        {
            float vector = -((MenuShiftables.Count - containerSize) * GetPixelDimension()) / 2;
            return OrientVector(vector);
        }
        return OrientVector(0);
    }
    protected override Vector2 GetEndPosition()
    {
        if (MenuShiftables.Count > containerSize)
        {
            float count = (MenuShiftables.Count - containerSize);
            float beginningCount = -((count) * GetPixelDimension()) / 2;
            float EndCount = ((count) * GetPixelDimension());
            return OrientVector(beginningCount + EndCount);
        }
        return OrientVector(0);
    }
    protected override Vector2 DownDelta()
    {
        if (Orientation == Orientation.Horizontal)
        {
            return GetIndexPosition(currentIndexSelected - (GetMaxIndexContainerSize()));
        }
        return GetIndexPosition(currentIndexSelected - (GetMaxIndexContainerSize()));
        //return GoToIndex(0);
    }

    protected override Vector2 UpDelta()
    {
        if (Orientation == Orientation.Horizontal)
        {
            return GetIndexPosition(currentIndexSelected);
        }
        return GetIndexPosition(currentIndexSelected);
        //return GoToIndex(0);
    }
    protected override Vector2 GetIndexPosition(int index)
    {
        return GetBeginningPosition() + OrientVector(index * GetPixelDimension());
        //return GoToBeginningTransform() + OrientVector(index * GetPixelDimension());
    }
    #endregion

    private void SetupButtonInteractable()
    {

        UpdateContainerIndexBounds();
        if (!_canChangeInteractableProperty)
        {
            for (int i = 0; i < MenuShiftables.Count; i++)
            {
                MenuShiftables[i].SetInteractable(true);
            }
        }
        else
        {
            for (int i = 0; i < MenuShiftables.Count; i++)
            {
                MenuShiftables[i].SetInteractable(WithinContainerBounds(i, _containerbuffer));
            }

        }

    }

    private bool WithinContainerBounds(int i, int offset = 0)
    {
        return _lowerButtonIndex - offset <= i && i <= _upperButtonIndex + offset;
    }

    //This assumes that the cursor is and current index selected has a proper container
    //Cursor is not out of bounds
    private void UpdateContainerIndexBounds()
    {
        if (IsCurrentIndexOutofBounds())
        {
            Debug.LogWarning("OUT OF BOUNDS");            
            Debug.LogWarning("Current Index: " + currentIndexSelected);
        }
        
        _previouslowerButtonIndex = _lowerButtonIndex;
        _previousUpperButtonIndex = _upperButtonIndex;

        if(MenuShiftables.Count<containerSize)
        {
            _lowerButtonIndex=0;
            _upperButtonIndex= (MenuShiftables.Count==0)? 0: MenuShiftables.Count-1;
            return;
        }

        //Debug.Log("BEFORE: " +_lowerButtonIndex +" || "+ _cursorIndex + " || " +_upperButtonIndex + " || " + currentIndexSelected);
        _lowerButtonIndex = currentIndexSelected - GetCorrectCursorIndex();
        if (_lowerButtonIndex < 0)
        {
            Debug.LogError(Time.frameCount + "--" + GetCorrectCursorIndex() + "-- cursorIndex");
            Debug.LogError(Time.frameCount + "--" + currentIndexSelected + "-- currentIndexSelected");
            Debug.LogError(Time.frameCount + "--" + _upperButtonIndex + "-- _upperButtonIndex");
            Debug.LogError(Time.frameCount + "--" + _lowerButtonIndex + "-- _lowerButtonIndex");
            Debug.LogError(Time.frameCount + "--" + gameObject.name + "-- lowerButtonIndex is below 0");
            _lowerButtonIndex=0;
            return;
        }
        _upperButtonIndex = _lowerButtonIndex + GetMaxIndexContainerSize();
        //Debug.Log("AFTER: " +_lowerButtonIndex +" || "+ _cursorIndex + " || " +_upperButtonIndex + " || " + currentIndexSelected);
        if (_upperButtonIndex > MaxIndex)
        {
            int difference = _upperButtonIndex - MaxIndex;
            _lowerButtonIndex = _lowerButtonIndex - difference;
            _lowerButtonIndex = Mathf.Clamp(_lowerButtonIndex, 0, MaxIndex);
            _upperButtonIndex = Mathf.Clamp(_lowerButtonIndex + GetMaxIndexContainerSize(), 0, MaxIndex);
        }


    }

    private int GetCorrectCursorIndex()
    {
        return (_cursorIndex - 1);
    }

    protected override void ScrollUp()
    {
        currentIndexSelected--;
        if (currentIndexSelected >= 0)
        {
            if (ShouldScroll())
            {
                SetupTransition(UpDelta());
                _FuncToPlayAtEndOfTransition = MoveBorderUp;
            }
            else
            {
                AddCursorIndex(-1);
            }
        }
        else
        {
            SetCursorsToEnd();
            if (IsThereMoreButtonsThanContainerSize())
            {
                SetupTransition(GetEndPosition());
                _FuncToPlayAtEndOfTransition = UpdateToNewBorderConstraints;
                TransitionToNewButton(true);
            }
        }
    }



    private bool IsThereMoreButtonsThanContainerSize()
    {
        return MenuShiftables.Count > containerSize;
    }
    private bool IsThereEqualOrMoreButtonsThanContainerSize()
    {
        return MenuShiftables.Count >= containerSize;
    }



    protected override void ScrollDown()
    {
        currentIndexSelected++;
        if (currentIndexSelected <= MaxIndex)
        {
            if (ShouldScroll())
            {
                SetupTransition(DownDelta());

                _FuncToPlayAtEndOfTransition = MoveBorderDown;
            }
            else
            {
                AddCursorIndex(1);
            }
        }
        else
        {
            SetCursorsToBeginning();

            if (IsThereMoreButtonsThanContainerSize())
            {
                SetupTransition(GetBeginningPosition());
                _FuncToPlayAtEndOfTransition = UpdateToNewBorderConstraints;
                TransitionToNewButton(true);
            }
        }
    }

    private void MoveBorderUp()
    {
        if (_canChangeInteractableProperty)
        {
            if (WithinRange(MenuShiftables, GetBufferedLowerButtonIndex() - 1))
            {
                MenuShiftables[GetBufferedLowerButtonIndex() - 1].SetInteractable(true);
            }
            if (WithinRange(MenuShiftables, GetBufferedUpperButtonIndex()))
            {
                MenuShiftables[GetBufferedUpperButtonIndex()].SetInteractable(false);
            }

        }

        _upperButtonIndex--;
        _lowerButtonIndex--;
    }

    private int GetBufferedUpperButtonIndex()
    {
        return _upperButtonIndex + _containerbuffer;
    }

    private int GetBufferedLowerButtonIndex()
    {
        return _lowerButtonIndex - _containerbuffer;
    }

    private int GetBufferedPreviousUpperButtonIndex()
    {
        return _previousUpperButtonIndex + _containerbuffer;
    }

    private int GetBufferedPreviousLowerButtonIndex()
    {
        return _previouslowerButtonIndex - _containerbuffer;
    }

    private void MoveBorderDown()
    {
        if (_canChangeInteractableProperty)
        {
            if (WithinRange(MenuShiftables,GetBufferedLowerButtonIndex()))
            {
                MenuShiftables[GetBufferedLowerButtonIndex()].SetInteractable(false);
            }
            if (WithinRange(MenuShiftables,GetBufferedUpperButtonIndex() + 1))
            {
                MenuShiftables[GetBufferedUpperButtonIndex() + 1].SetInteractable(true);
            }
        }

        _upperButtonIndex++;
        _lowerButtonIndex++;
    }

    private bool WithinRange(IList List, int index)
    {
        return 0 <= index && index < List.Count;
    }

    private void UpdateToNewBorderConstraints()
    {
        UpdateContainerIndexBounds();
        NewBorderButtons(GetBufferedLowerButtonIndex(), GetBufferedUpperButtonIndex(),
       GetBufferedPreviousLowerButtonIndex(),GetBufferedPreviousUpperButtonIndex());
    }


    private void NewBorderButtons(int newLowerLimit, int newUpperLimit, int oldLowerLimit, int oldUpperLimit)
    {
        if (!_canChangeInteractableProperty)
            return;

        for (int i = oldLowerLimit; i <= oldUpperLimit; i++)
        {
            if (WithinListBounds(i, MenuShiftables) && !WithinBoundsInclusive(i, newLowerLimit, newUpperLimit))
            {
                MenuShiftables[i].SetInteractable(false);
            }
        }
        for (int i = newLowerLimit; i <= newUpperLimit; i++)
        {
            if (WithinListBounds(i, MenuShiftables))
            {
                MenuShiftables[i].SetInteractable(true);
            }
        }
    }



    private int BeginningCursorIndex()
    {
        return 1;
    }
    private int MiddleCursorIndex()
    {
        return Mathf.CeilToInt((EndCursorIndex() + BeginningCursorIndex()) / 2);
    }

    private int EndCursorIndex()
    {
        return containerSize;
    }

/*
    protected override void ScrollToIndex(int index)
    {
        if (ShouldScroll())
        {
            SetupTransition(GetIndexPosition(index));
        }
        else
        {
            if (currentIndexSelected > _previousIndex)
            {
                SetupTransition(GetEndPosition());
            }
            else
            {
                SetupTransition(GetBeginningPosition());
            }
        }
    }
    */

    protected override bool ShouldScroll()
    {
        if (MovementDirection == Direction.Positive)
        {
            return _cursorIndex <= BeginningCursorIndex() || currentIndexSelected < 0;
        }
        else
        {
            return _cursorIndex >= EndCursorIndex() || currentIndexSelected > MaxIndex;
        }
    }
    /*

        Assumes what is enterned is a selectable content within the containers.
    */
    public override void OnEnterShiftable(IMenuShiftable menuButton)
    {
        if (!isFocused)
            return;
        if (!MenuShiftables.Contains(menuButton) || IsBusy())
            return;

        _previousIndex = currentIndexSelected;
        currentIndexSelected = menuButton.index;
        if (WithinContainerBounds(menuButton.index))
        {
            SetCursorIndex(menuButton.index - _lowerButtonIndex + 1);
        }
        else
        {
            //Set Cursor to edges
            if (menuButton.index > _upperButtonIndex)
            {
                _cursorIndex = EndCursorIndex();
            }
            else if (menuButton.index < _lowerButtonIndex)
            {
                _cursorIndex = BeginningCursorIndex();
            }
            else
            {
                Debug.LogError("WTF HAPPENED HERE");
            }
        }

        SelectCurrentSelectableAndPlaySound();
    }

    protected override void HandleCursorOutOfBounds(bool instant = false)
    {
        if (IsCurrentIndexOutofBounds())
        {
            if (currentIndexSelected > MaxIndex)
            {
                SetCursorsToEnd();
                if (IsThereEqualOrMoreButtonsThanContainerSize())
                {
                    SetupTransition(GetEndPosition());
                }
                else
                {
                    SetupTransition(GetBeginningPosition());
                }
                _FuncToPlayAtEndOfTransition = UpdateToNewBorderConstraints;
                if (instant)
                {
                    TransitionToNewButton(true);
                }
            }
            else if (currentIndexSelected < 0)
            {
                SetCursorsToBeginning();

                if (IsThereEqualOrMoreButtonsThanContainerSize())
                {
                    SetupTransition(GetBeginningPosition());
                    _FuncToPlayAtEndOfTransition = UpdateToNewBorderConstraints;
                    if (instant)
                    {
                        TransitionToNewButton(true);
                    }
                }


            }

        }
    }
    protected int SetCursorIndex(int setTo)
    {
        _cursorIndex = Mathf.Clamp(setTo, BeginningCursorIndex(), EndCursorIndex());
        return _cursorIndex;
    }

    protected void AddCursorIndex(int change)
    {
        _cursorIndex = Mathf.Clamp(_cursorIndex + change, BeginningCursorIndex(), EndCursorIndex());
    }


    private void SetCursorsToBeginning()
    {
        currentIndexSelected = 0;
        _cursorIndex = BeginningCursorIndex();
    }

    private void SetCursorsToEnd()
    {
        currentIndexSelected = MaxIndex;
        _cursorIndex = EndCursorIndex();
    }

    public override bool AddShiftable(IMenuShiftable addedButton, int insertAt)
    {
        bool success = base.AddShiftable(addedButton, insertAt);
        if (!success)
            return false;
        

        if (_canChangeInteractableProperty)
        {
            MenuShiftables[insertAt].SetInteractable(WithinContainerBounds(insertAt));
        }
        SetupTransition(GetIndexPosition(_lowerButtonIndex));
        TransitionToNewButton(true);
        if(_ScrollRectMenu!=null) // prevent movement
        {
            _ScrollRectMenu.StopMovement();
        }

        

        return true;
    }


    #region WhenButtonIsRemoved
    /*
        Just Scroll do not update AnyBounds here. if possible    
    */
    //Don't Update Cursor Bounds Here If possible
    protected override void HandleScrollingMovementCausedByRemovedButton(IMenuShiftable removedButton)
    {

        //the removed button is from the container bounds
        //Debug.Log(previouslowerButtonIndex + " | " + removedButton.index + " | " + previousUpperButtonIndex);
        //Debug.Log(lowerButtonIndex + " | " + removedButton.index + " | " + upperButtonIndex);
        bool RemoveButtonWasWithinContainerBounds = _lowerButtonIndex <= removedButton.index && removedButton.index <= _upperButtonIndex;
        //Debug.Log(cursorIndex +" cursorIndex ");     
        if (RemoveButtonWasWithinContainerBounds)
        {
            //Debug.Log("RemoveButtonWasWithinContainerBounds");
            //Change CursorIndex
            //Debug.Log(removedButton.index +" removedButton.index ");             


            //Debug.Log(MaxIndex +" | "+ _upperButtonIndex );
            bool RangeOfContainerisAtEnd = MaxIndex < _upperButtonIndex;
            if (RangeOfContainerisAtEnd)
            {
                //Debug.Log("RangeOfContainerisAtEnd");
                if (IsThereEqualOrMoreButtonsThanContainerSize())
                {
                    //Debug.Log("1");
                    ShiftContainerOneUp();//Go back up to where it should be then transition 

                    SetupTransition(GetEndPosition());
                    _FuncToPlayAtEndOfTransition = UpdateToNewBorderConstraints;
                }
            }
            else
            {
                //Debug.Log("NOT RangeOfContainerisAtEnd");

                //Debug.Log(MenuShiftables.Count + " > " + MaxIndex);
                //Debug.Log(removedButton.index + " removed | currentselected " + currentIndexSelected);
                //Debug.Log("3");
                if (removedButton.index > currentIndexSelected)
                {
                    //Debug.Log("RangeOfContainerisAtTheStart");
                    ShiftContainerOneUp();
                }
                else
                {
                    if (_lowerButtonIndex == 0)
                    {
                        //Debug.Log("lowerButtonIndex == 0");
                        ShiftContainerOneUp();
                    }
                    else
                    {
                        //Debug.Log("34");
                        ShiftContainerOneDown();
                    }

                }


            }
        }
        else //Should not transition
        {
            //Debug.Log("NOT RemoveButtonWasWithinContainerBounds");
            if (removedButton.index < currentIndexSelected)
            {
                ShiftContainerOneDown();

            }
            else
            {
                ShiftContainerOneUp();

            }
        }
    }

    private void ShiftContainerOneDown()
    {
        //Debug.Log("ShiftContainerOneDown ");
        rectTransform.offsetMax -= OrientVector(GetPixelDimension() / 2);
        rectTransform.offsetMin -= OrientVector(GetPixelDimension() / 2);
    }

    private void ShiftContainerOneUp()
    {
        //Debug.Log("ShiftContainerOneUp ");
        rectTransform.offsetMax += OrientVector(GetPixelDimension() / 2);
        rectTransform.offsetMin += OrientVector(GetPixelDimension() / 2);
    }

    protected override void HandleIndexChanges(IMenuShiftable removedButton)
    {
        //has to be done before base since it might remove one
        if (removedButton.index <= currentIndexSelected && _lowerButtonIndex <= 0)
        {
            SetCursorIndex(_cursorIndex - 1);
        }

        base.HandleIndexChanges(removedButton);

        //Debug.Log("currentIndexSelected: "+currentIndexSelected);
        //Debug.Log(lowerButtonIndex + " | " + removedButton.index + " | " + upperButtonIndex);


        if (transitioning == false)
        {
            UpdateToNewBorderConstraints();
        }
    }

    #endregion





    public override void TranslateIndexBy(int indexMovedby)
    {
        if (indexMovedby == 0)
            return;

        int newIndex = Mathf.Clamp(currentIndexSelected + indexMovedby, 0, MaxIndex);
        _previousIndex = currentIndexSelected;
        currentIndexSelected = newIndex;

        SetScrollingCursor(indexMovedby);

        SelectCurrentSelectableAndPlaySound();

        //Please recheck this I think this is wrong
        UpdateInteractablesBasedOnItCanBeChanged();
    }

    ///<summary>Call this when the MenuController is being scrolled by the scroll rect. Currentindexselected must be calculated first</summary>
    private void SetScrollingCursor(int indexMovedby)
    {
        int middleCursorIndex = MiddleCursorIndex();

        if (CurrentlySelectedIndexAwayFromEdgeBy(middleCursorIndex))
        {
            SetCursorIndex(middleCursorIndex);
            //Debug.Log(1);
        }
        else
        {
            if (currentIndexSelected <= middleCursorIndex)
            {
                SetCursorIndex(currentIndexSelected + 1);
                //Debug.Log(2);
            }
            else if (currentIndexSelected >= (MaxIndex - middleCursorIndex))
            {
                int InvertedIndex = ((MaxIndex + 1) - currentIndexSelected) - 1;
                SetCursorIndex(containerSize - (InvertedIndex));
                //Debug.Log(3);
            }
        }
    }

    private bool CurrentlySelectedIndexAwayFromEdgeBy(int indexBy)
    {
        bool isOdd= containerSize%2 != 0; //this fixes a bug where if the container count is +1 the container size, it doesn't update boundaries properly
        return indexBy <= currentIndexSelected && currentIndexSelected - (Convert.ToInt32(isOdd))<= (MaxIndex - indexBy);
    }

    /*
        Try Using this as calling SetupButtonInteractable will call all interactables if false
    */

    private void UpdateInteractablesBasedOnItCanBeChanged()
    {
        if (CanChangeInteractableProperty)
        {
            SetupButtonInteractable();
        }
        else
        {
            UpdateContainerIndexBounds();
        }
    }

}
}