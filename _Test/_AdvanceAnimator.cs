
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/*
    This is an incomplete script. Will add the functions that I need to have when have it working by that point.
    For now, the basic setup should be setup already for the future functions to work.
*/
/*
[DisallowMultipleComponent]
public class AdvanceAnimator : MonoBehaviour
{
    private const string suffixNameToGraph = " AdvanceAnimator";
    private bool MixerTransition=false;
    [SerializeField]
    private List<float> MixerWeightList;
    private List<float> MixerWeightListatStartofTransition;
    private List<float> previousMixerWeightLists;
    private Slot CurrentMixerSlot= Slot.A;
    private int varChanged;

    //[Header("Animators")]
    private RuntimeAnimatorController RuntimeAnimatorControllerA;
    //[Min(0)]
    //private double TimeRuntimeAnimatorControllerA=0;
    //private RuntimeAnimatorController RuntimeAnimatorControllerB;
    private Slot CurrentAnimatorSlot= Slot.A;


    //[Range(0,1)]
    private float WeightControllerAtoB=0;

    //[Header("AnimationClip")]
    private AnimationClip AnimationClipA;
    private AnimationClip AnimationClipB;


    //[Range(0,1)]
    private float WeightClipAtoB=0;
    private Slot CurrentClipSlot= Slot.A;

    private double TransitionStartTime=0;
    private double TransitionTimeDuration=0;
    //[SerializeField]
    private float currentValWeightControllerAtoBToLerp;
    //[SerializeField]
    private float currentValWeightAnimationClipAtoBToLerp;

    
    
    //Playables

    PlayableGraph playableGraph;
    private AnimationPlayableOutput playableOutput;
    private AnimationMixerPlayable mixerClipPlayable;
    private AnimationMixerPlayable mixerCtrlPlayable;
    private AnimationMixerPlayable mixerTimelinePlayable;
    private AnimationMixerPlayable mixerMainPlayable;
    private AnimationClipPlayable clipPlayableA;
    private AnimationClipPlayable clipPlayableB;
    private AnimatorControllerPlayable ctrlPlayableA;
    private AnimatorControllerPlayable ctrlPlayableB;
    private Playable timelinePlayable;
    private Animator _AnimatorPrivate;

    //[Header("AnimationClip")]
    private PlayableAsset playableAsset;
    private AnimationClip _clipToFade;
    private float _fadeTime;
    private bool _reset;
    private bool _fadeClipTriggered;
    private float _transitionOffset;

    private Animator _Animator {get{return GetAnimator();}}

    private Animator GetAnimator()
    {
        if(_AnimatorPrivate==null)
        {
            _AnimatorPrivate=GetComponent<Animator>();
        }
        return _AnimatorPrivate;
    }

    void Start()
    {
        // Creates the graph, the mixer and binds them to the Animator.

        CreateGraph();



        // Plays the Graph.

        playableGraph.Play();

    }

    private void CreateGraph()
    {
        playableGraph = PlayableGraph.Create(this.name + suffixNameToGraph);

        playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", _Animator);

        mixerClipPlayable = AnimationMixerPlayable.Create(playableGraph, 2);

        //mixerCtrlPlayable = AnimationMixerPlayable.Create(playableGraph, 2); //Doing Only one animator for now
        mixerCtrlPlayable = AnimationMixerPlayable.Create(playableGraph, 1);

        //mixerTimelinePlayable = AnimationMixerPlayable.Create(playableGraph, 1);

        mixerMainPlayable = AnimationMixerPlayable.Create(playableGraph, 2);

        MixerWeightList= new List<float>();
        MixerWeightList.Add(1);    //Animator
        MixerWeightList.Add(0);    //Clip
        //MixerWeightList.Add(0);    //Director Timeline
        previousMixerWeightLists = new List<float>(MixerWeightList);
        MixerWeightListatStartofTransition = new List<float>(MixerWeightList);
        
        playableOutput.SetSourcePlayable(mixerClipPlayable);
        playableOutput.SetSourcePlayable(mixerCtrlPlayable);

        //playableOutput.SetSourcePlayable(mixerTimelinePlayable);
        playableOutput.SetSourcePlayable(mixerMainPlayable);

        // Creates AnimationClipPlayable and connects them to the mixer.
        

        clipPlayableA = AnimationClipPlayable.Create(playableGraph, AnimationClipA);
        clipPlayableB = AnimationClipPlayable.Create(playableGraph, AnimationClipB);

        ctrlPlayableA = AnimatorControllerPlayable.Create(playableGraph, RuntimeAnimatorControllerA);
        //ctrlPlayableB = AnimatorControllerPlayable.Create(playableGraph, RuntimeAnimatorControllerB);

        if(playableAsset!=null)
        {
            timelinePlayable= playableAsset.CreatePlayable(playableGraph,gameObject);
        }
        


        //Connect
        playableGraph.Connect(clipPlayableA, 0, mixerClipPlayable, 0);
        playableGraph.Connect(clipPlayableB, 0, mixerClipPlayable, 1);
        
        playableGraph.Connect(ctrlPlayableA, 0, mixerCtrlPlayable, 0);
        //playableGraph.Connect(ctrlPlayableB, 0, mixerCtrlPlayable, 1);
        
        //playableGraph.Connect(timelinePlayable, 0, mixerTimelinePlayable, 0);

        playableGraph.Connect(mixerCtrlPlayable, 0, mixerMainPlayable, 0); //mixers
        playableGraph.Connect(mixerClipPlayable, 0, mixerMainPlayable, 1);
        //playableGraph.Connect(mixerTimelinePlayable, 0, mixerMainPlayable, 2);
        
        //Animator Plays first always-- for now
        mixerCtrlPlayable.Play();
        ctrlPlayableA.Play();
        //ctrlPlayableB.Pause();

        mixerClipPlayable.Play();
        clipPlayableA.Play();
        clipPlayableB.Play();
        

        
        //mixerTimelinePlayable.Play();
        //if(playableAsset)
        //{
        //    timelinePlayable.Play();
        //}

    }

    void Update()
    {
        if (!AdvanceAnimatorCanRun())
        {
            mixerMainPlayable.Pause();
            return;
        }
        else
        {
            mixerMainPlayable.Play();
        }

        //Check if animator values have changed
        
        if(_fadeClipTriggered) // has to be done here to prevent the character from hitching
        {            
            _fadeClipTriggered=false;
            FadeClipInternal(_clipToFade,_fadeTime,_reset);
        }

        HandleTransitionWeights();

        HandleMixerPausePlay();

        SetInputWeights();
        //string log="Weight: ";
        //foreach(var weight in MixerWeightList)
        //{
        //    log+=weight +" ";
        //}
        //Debug.Log(log);
        

    }

    private void SetInputWeights()
    {
        

        mixerMainPlayable.SetInputWeight(0, MixerWeightList[0]);
        mixerMainPlayable.SetInputWeight(1, MixerWeightList[1]);
        //mixerMainPlayable.SetInputWeight(2, MixerWeightList[2]);

        mixerCtrlPlayable.SetInputWeight(0, 1.0f - WeightControllerAtoB);
        //mixerCtrlPlayable.SetInputWeight(1, WeightControllerAtoB);

        mixerClipPlayable.SetInputWeight(0, 1.0f - WeightClipAtoB);
        mixerClipPlayable.SetInputWeight(1, WeightClipAtoB);



        //mixerTimelinePlayable.SetInputWeight(0, 1);
    }

    private void HandleMixerPausePlay()
    {
        if (MixerWeightList[0] == 0 && mixerCtrlPlayable.GetPlayState() == PlayState.Playing)
        {
            mixerCtrlPlayable.Pause();
        }
        else if (MixerWeightList[0] >0 && mixerCtrlPlayable.GetPlayState() == PlayState.Paused)
        {
            mixerCtrlPlayable.Play();
        }

        if (MixerWeightList[1] == 0 && mixerClipPlayable.GetPlayState() == PlayState.Playing)
        {
            mixerClipPlayable.Pause();
        }
        else if (MixerWeightList[1] >0 && mixerClipPlayable.GetPlayState() == PlayState.Paused)
        {
            mixerClipPlayable.Play();
        }

        if(playableAsset!=null)
        {
            if (MixerWeightList[2] == 0  && mixerTimelinePlayable.GetPlayState() == PlayState.Playing)
            {
                mixerTimelinePlayable.Pause();
            }
            else if (MixerWeightList[2] >0 && mixerTimelinePlayable.GetPlayState() == PlayState.Paused)
            {
                mixerTimelinePlayable.Play();
            }
        }
        
    }

    private void OnValidate()
    {
        //CreateGraph();
        GetBaseRunTimeControllers();
    }


    private void GetBaseRunTimeControllers()
    {
        if(_Animator)
        {
            RuntimeAnimatorControllerA= _Animator.runtimeAnimatorController;
        }
    }

    //Just Fade to New Clip
    public void FadeClip(float timeToFade)
    {
        if (!AdvanceAnimatorCanRun())
            return;
        //Debug.Log("FadeClip");
        FadeClipSetup(timeToFade);

        AssignCurrentWeightValues();
    }

    private static bool AdvanceAnimatorCanRun()
    {
        return !(GameState.isPaused);
    }

    //FadeClipInternal a problem when you call it directly, thus this method is called instead and FadeClipInternal is called in the update loop
    public void FadeClip(AnimationClip newClip,float timeToFade,bool resetifOldClipSameasNewClip=true)
    {
        _clipToFade=newClip;
        _fadeTime=timeToFade;
        _reset=resetifOldClipSameasNewClip;
        _fadeClipTriggered=true;
        _transitionOffset=0;
    }
    public void FadeClip(AnimationClip newClip,float timeToFade,float transitionOffset,bool resetifOldClipSameasNewClip=true)
    {
        _clipToFade = newClip;
        _fadeTime = timeToFade;
        _reset = resetifOldClipSameasNewClip;
        _fadeClipTriggered = true;
        _transitionOffset = transitionOffset;
    }


    //Decide Whether to fade to not create a new clip or fade to the same clip
    private void FadeClipInternal(AnimationClip newClip,float timeToFade,bool resetifOldClipSameasNewClip=true)
    {
        if (!AdvanceAnimatorCanRun())
            return;
        FadeClipSetup(timeToFade);

        if (CurrentClipSlot == Slot.A) //Transition to B
        {
            if (clipPlayableA.GetAnimationClip() == null) //prevent bikepose
            {
                WeightClipAtoB = 1;
                CreateNode(ref playableGraph, ref mixerClipPlayable, ref clipPlayableA, newClip, 0);
                AnimationClipA = newClip;
            }

            //If new clip is the same clip to the new one should just reset to new
            if (resetifOldClipSameasNewClip == true && clipPlayableB.GetAnimationClip() == newClip)
            {
                clipPlayableB.SetTime(_transitionOffset); //offset is defaulted to 0 if none is passed
            }
            else if (clipPlayableB.GetAnimationClip() != newClip || clipPlayableB.GetAnimationClip() == null)
            {
                CreateNode(ref playableGraph, ref mixerClipPlayable, ref clipPlayableB, newClip, 1);
                if(_transitionOffset>0) //might not work for negative values
                {
                    clipPlayableB.SetTime(_transitionOffset*newClip.length);
                }
                
            }

            CurrentClipSlot = Slot.B;
            AnimationClipB = newClip;


        }
        else if (CurrentClipSlot == Slot.B)  //Transition to A
        {
            if (clipPlayableB.GetAnimationClip() == null) //prevent bikepose
            {
                WeightClipAtoB = 0;
                CreateNode(ref playableGraph, ref mixerClipPlayable, ref clipPlayableB, newClip, 1);
                AnimationClipB = newClip;
            }

            if (resetifOldClipSameasNewClip == true && clipPlayableA.GetAnimationClip() == newClip)
            {
                clipPlayableA.SetTime(_transitionOffset); //offset is defaulted to 0 if none is passed
            }
            else if (clipPlayableA.GetAnimationClip() != newClip || clipPlayableA.GetAnimationClip() == null)
            {
                CreateNode(ref playableGraph, ref mixerClipPlayable, ref clipPlayableA, newClip, 0);
                if(_transitionOffset>0) //might not work for negative values
                {
                    clipPlayableA.SetTime(_transitionOffset*newClip.length);
                }
                
            }

            CurrentClipSlot = Slot.A;
            AnimationClipA = newClip;
        }

        AssignCurrentWeightValues();
    }

    private void FadeClipSetup(float timeToFade)
    {
        InitiateTransition(timeToFade);
        mixerClipPlayable.Play();
        CurrentMixerSlot = Slot.B;
    }

    private void CreateNode(ref PlayableGraph pGraph,ref AnimationMixerPlayable mixerclip, ref AnimationClipPlayable clipPlayable,
     AnimationClip clip,int inputSocket)
    {
        
        clipPlayable = AnimationClipPlayable.Create(pGraph, clip);
        
        mixerClipPlayable.DisconnectInput(inputSocket);
        pGraph.Connect(clipPlayable, 0, mixerClipPlayable, inputSocket);
    }


    public void FadeAnimator(float timeToFade)
    {
        if (!AdvanceAnimatorCanRun())
            return;
        InitiateTransition(timeToFade);
        CurrentMixerSlot = Slot.A;


        AssignCurrentWeightValues();

    }

    private void InitiateTransition(float timeToFade)
    {
        MixerTransition=true;
        TransitionStartTime = GetNormalTime();
        TransitionTimeDuration = timeToFade;
    }

    private void AssignCurrentWeightValues()
    {
        MixerWeightListatStartofTransition = new List<float>(MixerWeightList);
        currentValWeightControllerAtoBToLerp=WeightControllerAtoB;
        currentValWeightAnimationClipAtoBToLerp=WeightClipAtoB;
    }






    void OnDisable()
    {

        // Destroys all Playables and Outputs created by the graph.

        playableGraph.Destroy();

    }
    private void HandleTransitionWeights()
    {
        HandleMainMixerWeights();

        if (CurrentAnimatorSlot == Slot.B)
        {
            //Debug.Log("A2");
            WeightControllerAtoB = LerpCheck(currentValWeightControllerAtoBToLerp, 1, GetCurrentLerpValue());
        }
        else if (CurrentAnimatorSlot == Slot.A)
        {
            //Debug.Log("B2");
            WeightControllerAtoB = LerpCheck(currentValWeightControllerAtoBToLerp, 0, GetCurrentLerpValue());
        }

        if (CurrentClipSlot == Slot.B)
        {
            //Debug.Log("A3");
            WeightClipAtoB = LerpCheck(currentValWeightAnimationClipAtoBToLerp, 1, GetCurrentLerpValue());
        }
        else if (CurrentClipSlot == Slot.A)
        {
            //Debug.Log("B3");
            WeightClipAtoB = LerpCheck(currentValWeightAnimationClipAtoBToLerp, 0, GetCurrentLerpValue());
        }

    }

    private void HandleMainMixerWeights()
    {
        if (CurrentMixerSlot == Slot.A && MixerTransition)
        {
            //Debug.Log("A1");
            MixerWeightList[0] = LerpCheck(MixerWeightListatStartofTransition[0], 1, GetCurrentLerpValue());
            
            if(MixerWeightList[0]==1 && MixerWeightList[1]==0)// && MixerWeightList[2]==0))
            {
                MixerTransition=false;
            }

        }
        else if (CurrentMixerSlot == Slot.B && MixerTransition)
        {
            //Debug.Log("B1");
            MixerWeightList[1] = LerpCheck(MixerWeightListatStartofTransition[1], 1, GetCurrentLerpValue());
            if(MixerWeightList[0]==0 && MixerWeightList[1]==1)// && MixerWeightList[2]==0))
            {
                MixerTransition=false;
            }
        }
        //else if (CurrentMixerSlot == Slot.C && MixerTransition)
        //{
        //    //Debug.Log("C1");
        //    MixerWeightList[2] = LerpCheck(MixerWeightListatStartofTransition[2], 1, GetCurrentLerpValue());
        //    if((MixerWeightList[0]==0 && MixerWeightList[1]==0 && MixerWeightList[2]==1))
        //    {
        //        MixerTransition=false;
        //    }
        //}
        
        //if( (MixerWeightList[0]==1 && MixerWeightList[1]==0 && MixerWeightList[2]==0) ||
        //    (MixerWeightList[0]==0 && MixerWeightList[1]==1 && MixerWeightList[2]==0) ||
        //    (MixerWeightList[0]==0 && MixerWeightList[1]==0 && MixerWeightList[2]==1))
        //{
        //    MixerTransition=false;
        //}
        HandleWeights(ref previousMixerWeightLists,ref MixerWeightList,ref varChanged);
    }

    private float LerpCheck(float a, int b, float v)
    {
        var val= Mathf.Lerp(a,b, v);
        if(Mathf.Abs(a-b)<=0.001)
        {
            val=b;
        }
        return val;
    }

    private float GetCurrentLerpValue()
    {
        if(TransitionTimeDuration==0)
            return 1;
        return  Mathf.Clamp01((float) (((GetNormalTime() - TransitionStartTime) )/ TransitionTimeDuration));
    }
    private bool isDoneTransitioning()
    {
        return GetCurrentLerpValue()>=1;
    }

     private double GetNormalTime()
    {
        return Time.timeAsDouble;
    }
    

    private void HandleWeights(ref List<float> pWeights,ref List<float> cWeights,ref int cVar)
    {
        for (int i = 0; i < pWeights.Count; i++)
        {
            cWeights[i] = Mathf.Clamp01(cWeights[i]);
        }
        //Find Difference
        float difference;
        bool hasChanged;
         //Todo: later on might not need this since this is just for manual adjustments through inspector
        hasChanged= ChangeInFloats(pWeights,cWeights,out cVar,out difference);

        if(hasChanged)
        {
            if(difference<0 && pWeights[cVar]==1)
            {
                cWeights[cVar]=1;
                cVar=-1;
            }
            else
            {
                float previousLeftover= 1-pWeights[cVar];
                float leftover= 1-cWeights[cVar];

                for (int i = 0; i < pWeights.Count; i++)
                {
                    if(i!=cVar)
                    {
                        var previousPercentage=pWeights[i]/previousLeftover;
                        cWeights[i]=previousPercentage*leftover;
                    }
                }
            }            
        }
        pWeights = new List<float>(cWeights);

    }

    private bool ChangeInFloats(List<float> previous,List<float> current,out int indexChanged,out float difference)
    {
        float diff;
        for (int i = 0; i < current.Count; i++)
        {
            diff = current[i]-previous[i];
            if (Mathf.Abs(diff) > 0) //Assumes only one variable has changed
            {
                indexChanged = i;
                difference=diff;
                return true;
            }
        }
        indexChanged=-1;
        difference=0;
        return false;
    }

    public enum Slot
{
    A,
    B,
    C
}

}
*/