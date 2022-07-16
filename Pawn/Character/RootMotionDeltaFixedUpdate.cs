using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RootMotionDeltaFixedUpdate : MonoBehaviour
{
    
    public Animator Animator { get; private set; }
    public Vector3 VectorDelta;
    public Vector3 AdditionalVectorDelta;
    public bool ForceRootMotion=false;
    [SerializeField] [ ReadOnly]
      private float timestep;

    public event Action<Vector3,float,bool> OnRootMotionUpdate;

    private void Awake()
    {        
        if (Animator == null)
        {
            Animator = gameObject.GetComponent<Animator>();
        }
    }
    void OnAnimatorMove()
    {
        if(GameState.isPaused)
        {
            return;
        }
        VectorDelta+=Animator.deltaPosition +AdditionalVectorDelta;
        AdditionalVectorDelta=Vector3.zero;
        Animator.transform.localPosition=Vector3.zero;

        if(Time.inFixedTimeStep)
        {
            timestep+=Time.fixedDeltaTime;
            //Debug.Log("ON ANIMATOR MOVE F");
        }
        else
        {            
            timestep+=Time.deltaTime;
            //Debug.Log("ON ANIMATOR MOVE N");
        }
        OnRootMotionUpdate?.Invoke(VectorDelta,timestep,ForceRootMotion);
        VectorDelta=Vector3.zero;
        timestep=0f;
        
    }
    //private void Update()
    //{
    //    if(GameState.isPaused)
    //    {
    //        return;
    //    }
    //}
}
