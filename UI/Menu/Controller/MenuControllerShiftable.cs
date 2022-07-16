using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace UI.MenuController
{
    /*
    This Menu shifts in order to scroll through the buttons
    */
    /*
        Notes: 
        -Possible Change to HashSet if the List needs to find a specific item quickly. Like a 1000+ inventory
        -.Contains is the culprit of all of this.

        -Need to make this its own variant cause Container and Shiftable are essentially two variants. Just extract certain parts.

        Have Not Done
        * Updated So that the Rect created emcompasses the entire button layout. Cotainer has done that one
    */


    public abstract class MenuControllerShiftable : MenuController //, IPointerUpHandler ,IPointerDownHandler
    {
        [Header("Index Properties")]
        [SerializeField]
        protected List<IMenuShiftable> MenuShiftables;
        [Tooltip("The max index of MenuShiftables")]
        [SerializeField][ReadOnly]
        private int _MaxIndex;// { get; private set; }
        public int MaxIndex{get{return _MaxIndex;} set{_MaxIndex=Mathf.Max(value,0);}}
        [SerializeField]
        [Tooltip("How long it takes for theMenuController to transition back into current index")]
        protected float _MagnetTimeDuration = 0.25f;
        [Tooltip("Indicates if the TimeDuration used should be MagnetTimeDuration")]
        [SerializeField] [ReadOnly]
        protected bool _UseMagnet = false;

        [SerializeField]  [ReadOnly]
        protected Direction MovementDirection;
        [SerializeField] protected bool _keyDownLock;

        [Tooltip("Indicates if index should be changed immediately after it has changed. If not go to next frame.")]
        public bool ChangeIndexBeforeTransition;
        private int _previousChildCount;
        protected bool _removingButton;
        protected ScrollRectMenu _ScrollRectMenu;

#region Getters

        protected override float GetTimeDuration()
        {
            if (_UseMagnet)
                return _MagnetTimeDuration;
            return base.GetTimeDuration();
        }
        public override int GetCount()
        {
            if (MenuShiftables == null)
                return 0;
            return MenuShiftables.Count();
        }
        protected bool IsBusy()
        {
            return transitioning;
        }
#endregion

        protected override void Awake()
        {
            base.Awake();
            _ScrollRectMenu = transform.parent.GetComponent<ScrollRectMenu>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            if (_ScrollRectMenu)
            {
                _ScrollRectMenu.FinishedScrolling += ScrollRectHasFinishedScrolling;
            }

        }
        protected override void OnDisable()
        {
            base.OnDisable();
            if (_ScrollRectMenu)
            {
                _ScrollRectMenu.FinishedScrolling -= ScrollRectHasFinishedScrolling;
            }
        }     


        protected override void OnValidate()
        {
            //ValidationUtility.SafeOnValidate(() =>
            //{

                base.OnValidate();
                CreateMenuShiftablesFromFirstChildren();
                rectTransform = GetComponent<RectTransform>();
                if (rectTransform == null)
                    return;
                if(MaxIndex>0)
                {
                    currentIndexSelected = Math.Clamp(currentIndexSelected, 0, MaxIndex);
                }
                else
                {                    
                    currentIndexSelected = 0;
                }


                RefreshUI();
                MoveToInitialTransform();

            //});
        }
        /*
            Post: Creates New Menu. Does not add children that are not active
        */
        private void CreateMenuShiftablesFromFirstChildren()
        {
            MenuShiftables = new List<IMenuShiftable>();
            if (transform == null)
                return;
            for (int i = 0; i < transform.childCount; i++)
            {
                var shiftable = transform.GetChild(i).GetComponentInChildren<IMenuShiftable>();

                if (shiftable != null)
                {
                    MenuShiftables.Add(shiftable);
                }
            }
            SetupMenuButtons();
        }
#region UIBuilder
        public override void RefreshUI()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if (!_removingButton)
            {
                CreateMenuShiftablesFromFirstChildren();
            }
            if (HVLayoutGroup == null)
                return;
            if (Orientation == Orientation.Horizontal)
            {
                HandleSpacingDueToBeingHorizontal();
            }
            else
            {
                HandleSpacingDueToVertical();
            }
        }

        private void HandleSpacingDueToVertical()
        {
            if (MenuShiftables == null)
            {
                CreateMenuShiftablesFromFirstChildren();
            }
            if (MenuShiftables != null && MenuShiftables.Count > 0)
            {
                HVLayoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();

                HVLayoutGroup.padding.top = 0;
                HVLayoutGroup.padding.bottom = 0;
                HVLayoutGroup.padding.left = 0;
                HVLayoutGroup.padding.right = 0;
            }
        }

        virtual protected void MoveToInitialTransform()
        {
            int index = Mathf.Clamp(currentIndexSelected - 1, 0, MaxIndex - 1);
            if (MaxIndex != 0) //stops from going to beginning if no count
            {
                Debug.Log("index:" + index);
                rectTransform.anchoredPosition = GetIndexPosition(index);
            }
        }



        private void HandleSpacingDueToBeingHorizontal()
        {
            if (MenuShiftables == null)
            {
                CreateMenuShiftablesFromFirstChildren();
            }
            if (MenuShiftables != null && MenuShiftables.Count > 0)
            {
                HVLayoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();

                float ElementHeightPadding = GetFirstPixelHeight() / 2;
                HVLayoutGroup.padding.top = (int)((rectTransform.sizeDelta.y / 2) - ElementHeightPadding);
                HVLayoutGroup.padding.bottom = 0;
                HVLayoutGroup.padding.left = 0;
                HVLayoutGroup.padding.right = 0;
            }
        }

        private float GetFirstPixelHeight()
        {
            if(MenuShiftables[0]==null)// || MenuShiftables[0].Destroyed)
                return 0;
            return MenuShiftables[0].GetPixelHeight;
        }
        private float GetFirstPixelWidth()
        {
            if(MenuShiftables[0]==null)// || MenuShiftables[0].Destroyed)
                return 0;
            return MenuShiftables[0].GetPixelWidth;
        }
        #endregion

        ///<summary>Calls functions when The Scroll Rect Has Finished. In this case the MenuController magnets to the index</summary>
        virtual protected void ScrollRectHasFinishedScrolling()
        {
            if (transitioning == false && !_ScrollRectMenu.BeingDragged)
            {
                _UseMagnet = true;
                SetupTransition(GetIndexPosition(currentIndexSelected));
                _FuncToPlayAtEndOfTransition = () => { _UseMagnet = false; };
            }
        }
        protected override void Start()
        {
            base.Start();
            SetupMenuButtons();
            if (isFocused)
            {
                SetCurrentSelectable();
            }

        }

        

        virtual protected void Update()
        {
            HandleChangeInChildCount();
            HandleCursorOutOfBounds();
            //Bit Clunky Setup, Revisit Later 2/8/22
            if (transitioning)
            {
                TransitionToNewButton();
                if (!transitioning)
                {
                    ChangeInIndex(); //Some changes need to be done within the same frame
                }

                return;
            }
            if (!isFocused) //Don't Change Anything if the menu is not in focus
                return;

            //Get Movement
            CheckKeyPressStates();
            HandleMovement();
            if (!transitioning || ChangeIndexBeforeTransition)
            {
                ChangeInIndex();

            }
            if (!transitioning)
            {
                //Automatically Releases Movement Direction to unlock lock
                OnNavigateReleaseUp();
            }
        }

        ///<summary>Checks if there is a change in Index, if so Update Buttons.</summary>
        protected void ChangeInIndex()
        {
            if (_previousIndex != currentIndexSelected)
            {
                SelectCurrentSelectableAndPlaySound();
                _previousIndex = currentIndexSelected;
            }
        }
        
        //Decides whether or not to play Sound
        protected void SelectCurrentSelectableAndPlaySound()
        {
            var buttonSFX = MenuShiftables[currentIndexSelected].gameObject.GetComponent<ButtonSFX>();
            if (buttonSFX != null && buttonSFX.PlaySelectSound == false)
            {
                buttonSFX.PlaySelectSound = true;
                SetCurrentSelectable();
                buttonSFX.PlaySelectSound = false;
            }
            else
            {
                SetCurrentSelectable();
            }
        }

        /*
   This is usually called when a new Button has been selected so it needs to get rid of the previous
   selection and move to the new one.
*/
        public override void SetCurrentSelectable()
        {
            if (0 <= currentIndexSelected && currentIndexSelected < MenuShiftables.Count)
            {
                CurrentSelectedShiftable = MenuShiftables[currentIndexSelected];
            }
        }

        ///<summary>Handle Changes In Children Count that are done externtally not through any changes in through this MenuController</summary>
        virtual protected void HandleChangeInChildCount()
        {
            if (_previousChildCount != transform.childCount)
            {
                CreateMenuShiftablesFromFirstChildren();
            }
        }
        
        ///<summary>Checks if Cursor is out of bounds and appropriately moves it to the right positions.</summary>
        virtual protected void HandleCursorOutOfBounds(bool instant = false)
        {
            if (IsCurrentIndexOutofBounds())
            {
                if (currentIndexSelected > MaxIndex) //Cursor is beyond the number of shiftables
                {
                    currentIndexSelected = MaxIndex;
                    SetupTransition(GetEndPosition());
                    if (instant)
                    {
                        TransitionToNewButton(true);
                    }

                }
                else //Cursor is below the shiftables
                {
                    currentIndexSelected = 0;
                    SetupTransition(GetBeginningPosition());
                    if (instant)
                    {
                        TransitionToNewButton(true);
                    }
                }
            }
        }

        protected bool IsCurrentIndexOutofBounds()
        {
            return currentIndexSelected > MaxIndex || currentIndexSelected < 0;
        }

#region Movement

        private void CheckKeyPressStates()
        {
            if (_isPressPositive) MovementDirection = Direction.Positive;
            if (_isPressNegative) MovementDirection = Direction.Negative;
            if (!_isPressPositive && !_isPressNegative) MovementDirection = Direction.None;
        }

        private void HandleMovement()
        {
            if (MenuShiftables.Count <= 1)
                return;

            if (MovementDirection != Direction.None)
            {
                if (!_keyDownLock)
                {
                    if (MovementDirection == Direction.Negative)
                    {
                        ScrollDown();
                    }
                    else if (MovementDirection == Direction.Positive)
                    {
                        ScrollUp();
                    }
                    _keyDownLock = true;
                }
            }
            else
            {
                _keyDownLock = false;
            }
        }

        protected override void ScrollUp()
        {
            currentIndexSelected--;
            if (currentIndexSelected >= 0)
            {
                if (ShouldScroll())
                {
                    SetupTransition(UpDelta());
                }
            }
            else
            {
                currentIndexSelected = MaxIndex;
                SetupTransition(GetEndPosition());
            }
        }

        protected override void ScrollDown()
        {
            currentIndexSelected++;
            if (currentIndexSelected <= MaxIndex)
            {
                if (ShouldScroll())
                {
                    SetupTransition(DownDelta());
                }
            }
            else
            {
                SetupTransition(GetBeginningPosition());
                currentIndexSelected = 0;
            }
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
                    if(currentIndexSelected>_previousIndex)
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

        


        protected override Vector2 DownDelta()
        {
            return rectTransform.anchoredPosition - OrientVector(-GetPixelDimension());
        }

        protected override Vector2 UpDelta()
        {
            return rectTransform.anchoredPosition - OrientVector(GetPixelDimension());
        }
        protected override Vector2 GetIndexPosition(int index)
        {
            return OrientVector(index * GetPixelDimension());
        }
        protected override Vector2 GetEndPosition()
        {
            return OrientVector((MaxIndex) * GetPixelDimension());
        }
        protected override Vector2 GetBeginningPosition()
        {
            return OrientVector(0);
        }

        public override float GetPixelDimension()
        {
            if (MenuShiftables == null)
            {
                CreateMenuShiftablesFromFirstChildren();
            }
            if (MenuShiftables != null && MenuShiftables.Count > 0)
            {
                if (HVLayoutGroup == null)
                {
                    HVLayoutGroup = GetComponent<HorizontalOrVerticalLayoutGroup>();
                }
                if (Orientation == Orientation.Vertical)
                {
                    return GetFirstPixelHeight() + HVLayoutGroup.spacing;
                }
                else
                {
                    return GetFirstPixelWidth() + HVLayoutGroup.spacing;
                }
            }
            return 0;
        }

        

        #endregion

        #region Setup
        protected void SetupMenuButtons()
        {
             //The MenuButtons Themselves set their own indexes
            MaxIndex = MenuShiftables.Count - 1;
            _previousChildCount = transform.childCount;
        }

#endregion

        public override void OnEnterShiftable(IMenuShiftable menuButton)
        {
            if (!isFocused)
                return;
            if (!MenuShiftables.Contains(menuButton) || IsBusy())
                return;

            _previousIndex = currentIndexSelected;
            currentIndexSelected = menuButton.index;
            
            SelectCurrentSelectableAndPlaySound();
        }

        

        public override void OnExitShiftable(IMenuShiftable menuButton)
        {
            if (!isFocused)
                return;
            if (!MenuShiftables.Contains(menuButton) || IsBusy())
                return;

            menuButton.Deselect();
        }

        public override bool AddShiftable(IMenuShiftable addedButton, int insertAt)
        {
            if (MenuShiftables != null && MenuShiftables.Contains(addedButton))
                return false;
            MenuShiftables.Insert(insertAt, addedButton);


            SetupMenuButtons();
            if (currentIndexSelected >= MaxIndex)
            {
                SetupTransition(GetEndPosition());
            }
            else if (addedButton.index < currentIndexSelected)
            {
                ScrollUp();
            }


            _previousChildCount = transform.childCount;
            RefreshUI();
            
            return true;
        }

        //Add to the last part
        public override bool AddShiftable(IMenuShiftable addedButton)
        {

            if (MenuShiftables == null)
                return false;
            return AddShiftable(addedButton, MenuShiftables.Count);
        }

        

        public override bool RemoveShiftable(IMenuShiftable removedButton)
        {
            if (MenuShiftables.Count == 0 || !MenuShiftables.Contains(removedButton))
                return false;
            _removingButton = true;
            
            MenuShiftables.Remove(removedButton);

            //Update indexes and hierarachyOrder
            //Does not update the count

            //Potentially Just have SetupMenuButtons only
            if (removedButton.index < MenuShiftables.Count)
            {
                SetupMenuButtons();
            }
            else
            {
                MaxIndex = MenuShiftables.Count - 1;
            }


            RefreshUI();
            HandleScrollingMovementCausedByRemovedButton(removedButton);
            HandleIndexChanges(removedButton);
            _previousChildCount = transform.childCount;

            _removingButton = false;

            return true;
        }


        

        

        virtual protected void HandleIndexChanges(IMenuShiftable removedButton)
        {
            _previousIndex = currentIndexSelected;
            if (ShouldReduceIndexSelected(removedButton))
            {
                currentIndexSelected--;
                currentIndexSelected=Mathf.Max(0,currentIndexSelected);
            }
            currentIndexSelected = Mathf.Clamp(currentIndexSelected, 0, MaxIndex);
            if (removedButton.index <= currentIndexSelected)
            {
                SetCurrentSelectable();
            }
        }

        virtual protected bool ShouldReduceIndexSelected(IMenuShiftable removedButton)
        {
            return removedButton.index <= currentIndexSelected && currentIndexSelected - 1 >= 0;
        }

        /*
            Pre: Indexes must not be changed as it is pertinent as to how to scroll the menu due to button removal
        */
        virtual protected void HandleScrollingMovementCausedByRemovedButton(IMenuShiftable removedButton)
        {
            if (currentIndexSelected > MaxIndex)
            {
                currentIndexSelected = MaxIndex;
                SetupTransition(GetEndPosition());
            }
            else if (removedButton.index != currentIndexSelected)
            {
                if (removedButton.index < currentIndexSelected)
                {
                    ScrollUp();
                }
            }
        }

        


        public override void OnMove(MoveDirection moveDir)
        {
            ProcessMovement(moveDir);
        }
        public override void OnMove(Vector2 navigateValue)
        {
            if (Orientation == Orientation.Horizontal)
            {
                ProcessDeltaMovement(navigateValue.x);
            }
            else
            {
                ProcessDeltaMovement(navigateValue.y);
            }

        }
        public override void OnMove(float navigateValue)
        {
            ProcessDeltaMovement(navigateValue);
        }

        private void ProcessDeltaMovement(float navigateValue)
        {
            if (!isFocused)
                return;

            if (Orientation == Orientation.Vertical)
            {

                if (navigateValue >= 1)
                {
                    _isPressPositive = true;
                }
                else if (navigateValue <= -1)
                {
                    _isPressNegative = true;
                }

            }
            else
            {
                if (navigateValue >= 1)
                {
                    _isPressPositive = true;
                }
                else if (navigateValue <= -1)
                {
                    _isPressNegative = true;
                }
            }
        }

        protected void ProcessMovement(MoveDirection moveDir)
        {
            if (!isFocused)
                return;
            bool canMove = CanMoveFromMoveDir(moveDir);
            if (!canMove)
                return;


            switch (moveDir)
            {
                case MoveDirection.Up:
                    _isPressPositive = true;
                    break;
                case MoveDirection.Down:
                    _isPressNegative = true;
                    break;
                case MoveDirection.Right:
                    _isPressNegative = true;
                    break;
                case MoveDirection.Left:
                    _isPressPositive = true;
                    break;
                case MoveDirection.None:
                    OnNavigateReleaseUp();
                    break;
            }
        }

        private bool CanMoveFromMoveDir(MoveDirection moveDir)
        {
            bool HorizontalMovement = moveDir == MoveDirection.Left || moveDir == MoveDirection.Right;
            bool VerticalMovement = moveDir == MoveDirection.Up || moveDir == MoveDirection.Down;
            bool canMove = (HorizontalMovement && Orientation == Orientation.Horizontal)
            || (VerticalMovement && Orientation == Orientation.Vertical)
            || moveDir == MoveDirection.None;
            return canMove;
        }

        protected override void EndOfTransition()
        {
            OnNavigateReleaseUp();
            //Seems sus, have to double check if actions are remove properly
            _FuncToPlayAtEndOfTransition?.Invoke();
            _FuncToPlayAtEndOfTransition = () => { };
        }

        protected void OnNavigateReleaseUp()
        {
            _isPressPositive = false;
            _isPressNegative = false;
        }




        public override void SetFocus(bool focusMode)
        {
            if (focusMode == isFocused)
                return;

            isFocused = focusMode;
            if (GetCount() <= 0)
                return;
            if (!isFocused)
            {
                MenuShiftables[currentIndexSelected].Deselect();
            }
            else
            {
                SetCurrentSelectable();
            }

        }
        protected bool WithinListBounds(int i, IList List)
        {
            return 0 <= i && i < List.Count;
        }

        protected bool WithinBoundsInclusive(int index, int lower, int upper)
        {
            return lower <= index && index <= upper;
        }

        
        ///<summary>Moves the current index of the MenuControler by given index</summary>
        public override void TranslateIndexBy(int indexMovedby)
        {
            Debug.Log("CURRENT: " + currentIndexSelected + "|| NEW: " + (currentIndexSelected + indexMovedby));
            int newIndex = Mathf.Clamp(currentIndexSelected + indexMovedby, 0, MaxIndex);
            OnEnterShiftable(MenuShiftables[newIndex]);
        }

        public override IMenuShiftable GetMenuShiftable(int index)
        {
            if (WithinListBounds(index, MenuShiftables))
            {
                return MenuShiftables[index];

            }
            return null;
        }


    }
}