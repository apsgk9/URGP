
using UnityEngine;

using UnityEngine.Playables;

using UnityEngine.Animations;
[RequireComponent(typeof(Animator))]
public class PlayWithTimeControlSample : MonoBehaviour

{

    public AnimationClip clip;

    public float time;

    PlayableGraph playableGraph;

    AnimationClipPlayable playableClip;

    void Start()

    {

        playableGraph = PlayableGraph.Create();

        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "Animation", GetComponent<Animator>());

        // Wrap the clip in a playable

        playableClip = AnimationClipPlayable.Create(playableGraph, clip);

        // Connect the Playable to an output

        playableOutput.SetSourcePlayable(playableClip);

        // Plays the Graph.

        playableGraph.Play();

        // Stops time from progressing automatically.

        playableClip.Pause();

    }

    void Update () 

    {

        // Control the time manually

        playableClip.SetTime(time);

    }

    

    void OnDisable()

    {

        // Destroys all Playables and Outputs created by the graph.

        playableGraph.Destroy();

    }

}