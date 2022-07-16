using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestAdvanceAnimator : MonoBehaviour
{
    public AdvanceAnimator AdvanceAnimator { get; private set; }
    public AnimationClip ActionClip1;
    public bool transitionBackIntoClip1=false;
    public AnimationClip ActionClip2;
    public bool transitionBackIntoClip2=false;
    PlayerInputActions PlayerInput;
    bool ActionPressed;
    public float IntroTime=0.1f;
    public float CancellableTime=0.4f;

    public float StartTimeOfAction;

    public float ResumeTime;
    public float ExitTime=0.6f;
    public float ExitTimeDuration=0.4f;
    [SerializeField]
    private float ExitTimeReal;

    bool ActionOccuring=false;
    [SerializeField]
    private bool transitioned=true;

    bool useClip1=false;

    // Start is called before the first frame update
    private void Awake()
    {
        
        PlayerInput=new PlayerInputActions();
    }
    void Start()
    {
        AdvanceAnimator = GetComponent<AdvanceAnimator>();
        ResumeTime=Time.time;
        
    }
    private void OnEnable()
    {
        PlayerInput.Enable();
        PlayerInput.PlayerControls.Action_A.started +=ActionDown;
    }
    private void OnDisable()
    {
        PlayerInput.Disable();
        PlayerInput.PlayerControls.Action_A.started -=ActionDown;
    }

    private void Update()
    {
        if(ResumeTime<Time.time)
        {
            ActionOccuring=false;
        }
        

        if (transitioned == false)
        {
            if (StartTimeOfAction+ExitTime < Time.time)
            {
                AdvanceAnimator.FadeAnimator(ExitTimeDuration);
                transitioned = true;
            }
        }
    }

    private void TriggerAction()
    {
        //Debug.Log("TRIGGER ACTION START");
        if(useClip1)
        {
            AdvanceAnimator.FadeClip(ActionClip1,IntroTime,transitionBackIntoClip1);
            useClip1=!useClip1;
        }
        else
        {
            AdvanceAnimator.FadeClip(ActionClip2,IntroTime,transitionBackIntoClip2);
            useClip1=!useClip1;
        }


        StartTimeOfAction = Time.time;
        ResumeTime=Time.time+CancellableTime;
        ExitTimeReal=Time.time+StartTimeOfAction;

        ActionOccuring=true;
        transitioned=false;
        //Debug.Log("TRIGGER ACTION END");
    }

    private void ActionDown(InputAction.CallbackContext obj)
    {
        //Debug.Log("ACTION DOWN");
        if(!ActionOccuring && ResumeTime<Time.time)
        {
            TriggerAction();
        }
    }

}
