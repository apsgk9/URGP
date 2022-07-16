using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityFunctions;

public abstract class CharacterAnimatorMonoBehaviour<CC>  : MonoBehaviour where CC : Character
{
    [Header("Essential")]
    [SerializeField]
    protected Animator _Animator;
    [SerializeField]
    protected CC _characterController;
    //[SerializeField]
    protected RuntimeAnimatorController _RuntimeAnimatorController;

    private void OnValidate()
    {
        if(_Animator)
        {
            GetAnimatorControllers();
        }
    }

    virtual protected void Awake()
    {
        FindAnimator();
        FindCharacterStateMachine();
    }

    

    private void FindAnimator()
    {
        if(_Animator==null)
        {
            _Animator= ComponentUtil.FindComponentWithinGameObject<Animator>(gameObject);
        }
        if(_Animator)
        {
            GetAnimatorControllers();
        }
    }

    private void GetAnimatorControllers()
    {
        _RuntimeAnimatorController = _Animator.runtimeAnimatorController;
    }

    private void FindCharacterStateMachine()
    {
        if(_characterController==null)
        {
            _characterController=ComponentUtil.FindComponentWithinGameObject<CC>(gameObject);
        }
    }


}
