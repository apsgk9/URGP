using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
    Would need to initialized first before calling
*/
public class SettingsManager : GameServiceBehaviour<SettingsManager>, Service.IGameService,IHasInitialized
{
    public Settings Settings;
    public bool hasInitialized {get;private set;}

    public IEnumerator Start() 
    {
        hasInitialized=false;
        var opHandle = Addressables.LoadAssetAsync<Settings>(AddressableLabelNames.UserSettings);
        yield return opHandle;

        if (opHandle.Status == AsyncOperationStatus.Succeeded) 
        {
            Settings = opHandle.Result;
        }
        hasInitialized=true;
    }


    public InputSettings GetInputSettings()
    {
        if(Settings==null)
            return null;
        if(Settings.InputSettings!=null)
            return Settings.InputSettings;
        return null;
    }
    public MenuSettings GetMenuSettings()
    {
        if(Settings==null)
            return null;        
        if(Settings.MenuSettings!=null)
            return Settings.MenuSettings;
        return null;
    }

    public SystemSettings GetSystemSettings()
    {
        if(Settings==null)
            return null;        
        if(Settings.MenuSettings!=null)
            return Settings.SystemSettings;
        return null;
    }
}
