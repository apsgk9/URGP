using System.Collections;
using System.Collections.Generic;
using Service;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionMenuHandler : MonoBehaviour , IGameService
{    
    public bool dontDeleteOnAwake=false;
    public ButtonMenu ButtonMenuTemplate;
    public Transform TransformToSpawnButtonsIn;
    public List<ButtonMenu> ButtonList;
    [SerializeField] [Min(0)]
    private float fadeTime;
    [SerializeField] [ReadOnly]
    private bool _previousActive;
    public bool Active{get{return TransformToSpawnButtonsIn.childCount>0;}}
    public CanvasGroup CanvasGroup;
    private Coroutine currentCoroutine;
    [SerializeField]
    private ShiftableMenuPanel ShiftableMenuPanel;

    private void Awake()
    {
        ButtonList=new List<ButtonMenu>();
        if(!dontDeleteOnAwake)
        {
            foreach (Transform child in TransformToSpawnButtonsIn.transform) 
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        int finalAlpha= System.Convert.ToInt32(Active);
        CanvasGroup.alpha=finalAlpha;
        

        _previousActive=Active;
        
        ServiceLocator.Current.Register<InteractionMenuHandler>(this as InteractionMenuHandler);
    }
    private void OnEnable()
    {
        if(Active)
        {
            RegisterMovementCommands();
        }
    }
    private void OnDisable()
    {
        DeregisterMovementCommands();
    }
    private void Update()
    {
        //Call if Menu has been opened up or closed
        if(Active!=_previousActive)
        {
            _previousActive=Active;
            if(currentCoroutine!=null)
                StopCoroutine(currentCoroutine);
            currentCoroutine=StartCoroutine(StartTransitionForActivationState());

            //Menu is opened up
            if(Active)
            {
                ShiftableMenuPanel.Select();

                //ButtonSFX ButonSFX = GetCurrentlySelectedButtonSFX();

                //if (ButonSFX && ButonSFX.PlaySelectSound == true) //Prevent Select sound when opening up. Can be annoying to hear every single time
                //{
                //    ButonSFX.PlaySelectSound = false;
                //    ShiftableMenuPanel.MenuControllerToFocus.SetCurrentSelectable();
                //    ButonSFX.PlaySelectSound = true;
                //}
                //else
                //{
                    ShiftableMenuPanel.MenuControllerToFocus.SetCurrentSelectable();
                //}


                RegisterMovementCommands();
            }
            else
            {
                DeregisterMovementCommands();
            }

        }
    }

    private ButtonSFX GetCurrentlySelectedButtonSFX()
    {
        int currentIndexSelected = ShiftableMenuPanel.MenuControllerToFocus.currentIndexSelected;
        var ButonSFX = ShiftableMenuPanel.MenuControllerToFocus.transform.GetChild(currentIndexSelected).gameObject.GetComponent<ButtonSFX>();
        return ButonSFX;
    }

    private void RegisterMovementCommands()
    {
        if (UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.ScrollWheel.performed += OnScrollWheel;
        }
    }
    private void DeregisterMovementCommands()
    {
        if (UserInput.CanAccess)
        {
            UserInput.Instance.PlayerInputActions.LimitedMenuControls.ScrollWheel.performed -= OnScrollWheel;
        }
    }

    private void OnScrollWheel(InputAction.CallbackContext context)
    {
        if(ShiftableMenuPanel!=null && ShiftableMenuPanel.MenuControllerToFocus!=null)
        {
            Vector2 navigateVector = context.ReadValue<Vector2>();
            ShiftableMenuPanel.MenuControllerToFocus.OnMove(navigateVector.y);
        }
    }

    private IEnumerator StartTransitionForActivationState()
    {
        var elapsedTime = 0f;
        int finalAlpha= System.Convert.ToInt32(Active);
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            CanvasGroup.alpha= Mathf.Lerp(CanvasGroup.alpha,finalAlpha,elapsedTime/fadeTime);
            yield return null;
        }
        CanvasGroup.alpha=finalAlpha;
    }

    private void OnDestroy()
    {
        ServiceLocator.Current.Unregister<InteractionMenuHandler>();
    }
    public ButtonMenu CreateButton()
    {
        if(TransformToSpawnButtonsIn==null)
        {
            Debug.LogError("No Transform to spawn Buttons in. Button was not created.");
            return null;
        }
        if(ButtonList==null)
        {
            ButtonList=new List<ButtonMenu>();
        }
        
        var ButtonSpawned=GameObject.Instantiate(ButtonMenuTemplate,Vector3.zero,Quaternion.identity,TransformToSpawnButtonsIn);
        //Debug.Log(ShiftableMenuPanel.MenuControllerToFocus.GetComponent<RectTransform>().anchoredPosition);
        ButtonSpawned.name+= ButtonSpawned.transform.GetSiblingIndex();
        ButtonList.Add(ButtonSpawned);
        ButtonSpawned.HasBeenDestroyed+= OnButtonDestroyed;
        return ButtonSpawned;
    }

    private void OnButtonDestroyed(ButtonMenu btnDestroyed)
    {
        if(ButtonList.Contains(btnDestroyed))
        {
            ButtonList.Remove(btnDestroyed);
        }
    }

    public void RemoveButton(ButtonMenu btnToRemove)
    {
        if(TransformToSpawnButtonsIn==null || ButtonList==null)
        {
            Debug.LogError("Unable to remove button.");
            return;
        }

        if(ButtonList.Contains(btnToRemove))
        {
            Destroy(btnToRemove.gameObject);
            ButtonList.Remove(btnToRemove);
        }
    }
}
