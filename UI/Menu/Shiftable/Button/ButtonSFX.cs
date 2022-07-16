using System;
using System.Collections;
using System.Collections.Generic;
using UI.Button;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioClip SelectAudioClip;
    public AudioClip ConfirmAudioClip;
    private IButtonSelectCallback SelectComponent;
    private IButtonConfirmCallback ConfirmComponent;
    public bool PlaySelectSound=false;


    // Start is called before the first frame update
    private void Awake()
    {
        //_audioSource= GetComponent<AudioSource>();
        SelectComponent= GetComponent<IButtonSelectCallback>();
        ConfirmComponent= GetComponent<IButtonConfirmCallback>();
    }
    private void OnEnable()
    {
        if(SelectComponent!=null)
        {
            SelectComponent.OnButtonSelect+=OnButtonSelect;
        }
        if(ConfirmComponent!=null)
        {
            ConfirmComponent.OnButtonConfirm+=OnButtonConfirm;
        }
    }
    private void OnDisable()
    {
        if(SelectComponent!=null)
        {
            SelectComponent.OnButtonSelect-=OnButtonSelect;
        }
        if(ConfirmComponent!=null)
        {
            ConfirmComponent.OnButtonConfirm-=OnButtonConfirm;
        }
        
    }

    private void OnButtonConfirm()
    {
        AudioManager.Instance.UIPlayOneShot(ConfirmAudioClip);
    }

    

    private void OnButtonSelect()
    {
        if(PlaySelectSound)
        {
            AudioManager.Instance.UIPlayOneShot(SelectAudioClip);
        }
    }

}
