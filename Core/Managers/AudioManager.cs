using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
    Manages Game Wide audio. This can include UI or music that is not tied to down to the level itself
*/
public class AudioManager : Singleton<AudioManager> , IHasInitialized
{
    private const string UIPath = "UI";
    private GameObject _UIAudioSourceGO;
    public AudioSource UIAudioSource;

    public bool hasInitialized {get;private set;}
    public AudioMixer MasterAudioMixer { get; private set; }
    public List<AudioClip> AudioClipsPlayingThisFrame;
    private int _currentFrame;

    private void Awake()
    {       
        hasInitialized=false;

        _UIAudioSourceGO = new GameObject("UIAudioSource");
        _UIAudioSourceGO.transform.parent=this.transform;
        UIAudioSource= _UIAudioSourceGO.AddComponent<AudioSource>();
        UIAudioSource.playOnAwake=false;

        AudioClipsPlayingThisFrame= new List<AudioClip>();
    }

    public IEnumerator Start() 
    {
        hasInitialized=false;
        var opHandle = Addressables.LoadAssetAsync<AudioMixer>(AddressableLabelNames.MasterAudioMixer);
        yield return opHandle;

        if (opHandle.Status == AsyncOperationStatus.Succeeded) 
        {
            MasterAudioMixer = opHandle.Result;
        }
        SetupMixer();
        hasInitialized=true;
    }
    private void LateUpdate()
    {
        AudioClipsPlayingThisFrame.Clear(); 
    }

    private void SetupMixer()
    {
        AudioMixerGroup[] audioMixGroup = MasterAudioMixer.FindMatchingGroups(UIPath);
        UIAudioSource.outputAudioMixerGroup=audioMixGroup[0];
    }

    internal void UIPlayOneShot(AudioClip selectAudioClip)
    {
        
        if(!AudioClipsPlayingThisFrame.Contains(selectAudioClip))
        {
            UIAudioSource.PlayOneShot(selectAudioClip);
            AudioClipsPlayingThisFrame.Add(selectAudioClip);
        }
    }
}
