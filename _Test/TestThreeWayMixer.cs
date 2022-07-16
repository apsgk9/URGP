using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TestThreeWayMixer : MonoBehaviour
{
    [Header("AnimationClip")]
    public List<AnimationClip> Clips;
    public List<AnimationClipPlayable> PlayablesClips;
    public List<float> PlayablesWeights;
    private List<float> PreviousPlayablesWeights;
    public int changedVariable;

    
    
    //Playables

    PlayableGraph playableGraph;
    private AnimationPlayableOutput playableOutput;
    private AnimationMixerPlayable mixerPlayable;
    private Animator _AnimatorPrivate;
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
        playableGraph = PlayableGraph.Create();

        playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", _Animator);

        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, Clips.Count);
        
        playableOutput.SetSourcePlayable(mixerPlayable);

        PlayablesClips = new List<AnimationClipPlayable>();
        PlayablesWeights= new List<float>();
            Debug.Log("Count: "+ Clips.Count);

        for (int i = 0; i < Clips.Count; i++)
        {
            Debug.Log(i + ": "+ Clips[i].name);
            PlayablesClips.Add(AnimationClipPlayable.Create(playableGraph, Clips[i]));
            playableGraph.Connect(PlayablesClips[i] , 0, mixerPlayable, i);
            PlayablesClips[i].Play();
            PlayablesWeights.Add(0);
        }

        PreviousPlayablesWeights=new List<float>(PlayablesWeights);
        
        PlayablesWeights[0]=1;
        
        //mixerPlayable.Play();
        
        
    }
    

    void Update()
    {
        if(GameState.isPaused)
        {
            mixerPlayable.Pause();
            return;
        }
        else
        {
            mixerPlayable.Play();
        }

        HandleWeights(ref PreviousPlayablesWeights,ref PlayablesWeights,ref changedVariable);
        for (int i = 0; i < GetCount(); i++)
        {
            mixerPlayable.SetInputWeight(i, PlayablesWeights[i]);
        }

    }

    private void HandleWeights(ref List<float> pWeights,ref List<float> cWeights,ref int cVar)
    {
        for (int i = 0; i < GetCount(); i++)
        {
            cWeights[i] = Mathf.Clamp01(cWeights[i]);
        }
        //Find Difference
        float difference;
        bool hasChanged=ChangeInFloats(pWeights,cWeights,out cVar,out difference);
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

                for (int i = 0; i < GetCount(); i++)
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
        for (int i = 0; i < PreviousPlayablesWeights.Count; i++)
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

    private int GetCount()
    {
        return Clips.Count;
    }

    void OnDisable()
    {

        // Destroys all Playables and Outputs created by the graph.

        playableGraph.Destroy();

    }
    
}
