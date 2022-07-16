using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePrompts;
using System;

public class KeyPromptManager : Singleton<KeyPromptManager>
{
    public const string SOPlaystationPath = "Core/Prompts/PSPromptsScheme";
    public const string SOXboxPath = "Core/Prompts/XboxPromptsScheme";
    public const string SOPCPath = "Core/Prompts/PCPromptScheme";
    public const string SOMISCCPath = "Core/Prompts/MiscPromptsScheme";

    private GamepadPromptsScheme _PlaystationPrompts;
    public GamepadPromptsScheme PlaystationPrompts 
    {
        get
        {
             if(_PlaystationPrompts==null)
             {
                 _PlaystationPrompts=Resources.Load(SOPlaystationPath, typeof(GamepadPromptsScheme)) as GamepadPromptsScheme;
             }
             return _PlaystationPrompts;
        }
    }

    private GamepadPromptsScheme _XboxPrompts;
    public GamepadPromptsScheme XboxPrompts 
    {
        get
        {
             if(_XboxPrompts==null)
             {
                 _XboxPrompts=Resources.Load(SOXboxPath, typeof(GamepadPromptsScheme)) as GamepadPromptsScheme;
             }
             return _XboxPrompts;
        }
    }

    
    private PCPromptsScheme _PCPrompts;
    public PCPromptsScheme PCPrompts
    {
        get
        {
             if(_PCPrompts==null)
             {
                 _PCPrompts=Resources.Load(SOPCPath, typeof(PCPromptsScheme)) as PCPromptsScheme;
             }
             return _PCPrompts;
        }
    }

    private MiscPromptsScheme _MiscPrompts;
    public MiscPromptsScheme MiscPrompts
    {
        get
        {
             if(_MiscPrompts==null)
             {
                 _MiscPrompts=Resources.Load(SOPCPath, typeof(MiscPromptsScheme)) as MiscPromptsScheme;
             }
             return _MiscPrompts;
        }
    }
    // Start is called before the first frame update
    private void Awake()
    {
        this.gameObject.name=typeof(KeyPromptManager).Name.ToUpper();
        DontDestroyOnLoad(this);
        SETUP();
    }

    

    private void SETUP()
    {
        if(_PlaystationPrompts==null)
            _PlaystationPrompts = Resources.Load(SOPlaystationPath, typeof(GamepadPromptsScheme)) as GamepadPromptsScheme;
        if(_XboxPrompts==null)
            _XboxPrompts = Resources.Load(SOXboxPath, typeof(GamepadPromptsScheme)) as GamepadPromptsScheme;
        if(_PCPrompts==null)
            _PCPrompts = Resources.Load(SOPCPath, typeof(PCPromptsScheme)) as PCPromptsScheme;
        if(_MiscPrompts==null)
            _MiscPrompts = Resources.Load(SOMISCCPath, typeof(MiscPromptsScheme)) as MiscPromptsScheme;
    }

    public Sprite GetIcon(string iconName)
    {
        //Debug.Log("GETTING: " + iconName);
        if (GetCurrentUserControllerType() == UserControllerType.PC)
            return GetPCSprite(iconName);

        if(GetCurrentUserControllerType() == UserControllerType.PlayStation )
            return GetControllerSprite(iconName,PlaystationPrompts);
        else if(GetCurrentUserControllerType() ==UserControllerType.Xbox)
            return GetControllerSprite(iconName,XboxPrompts);

        return GetUnknownSprite();

    }

    private Sprite GetControllerSprite(string iconName,GamepadPromptsScheme GpadPrompts)
    {
        switch (iconName)
        {
            case "Right Trigger":
                return GpadPrompts.RightTrigger;
            case "Left Trigger":
                return GpadPrompts.LeftTrigger;
            default:
                break;
        }
        Debug.Log("NON");

        return GetUnknownSprite();
    }

    private Sprite GetUnknownSprite()
    {
        return MiscPrompts.Unknown;
    }

    private static UserControllerType GetCurrentUserControllerType()
    {
        return UserInput.Instance.CurrentUserControllerType;
    }

    private Sprite GetPCSprite(string iconName)
    {
        switch(iconName)
        {
            case "Scroll":
                return PCPrompts.MouseMiddle;
            default:
                break;
        }
        Debug.Log("NON");
        
        return GetUnknownSprite();
    }
}
