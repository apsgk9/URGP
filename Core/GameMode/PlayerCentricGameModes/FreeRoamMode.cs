using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

namespace GameMode
{
    public class FreeRoamMode : PlayerCentricMode<FreeRoamMode>
    {
        public bool PlayerUIInitialized {get {return UIManager.Instance.PlayerUILoaded;}}
        public bool PlayerCharacterInitialized {get; private set;}
        private GameObject PlayerObject {get {return GameMode.IPlayerCentric.Player;} set {GameMode.IPlayerCentric.Player=value;}}

        public bool CanPause =true;
        private bool _menuKeyDown;

        private SystemSettings _systemSettings;

        public IEnumerator LoadUI()
        {

            //Initialize PlayerUI
            StartCoroutine(UIManager.Instance.LoadPlayerUI());
            while(PlayerUIInitialized==false)
            {
                yield return null;
            }
        }

        public IEnumerator LoadCharacter(AssetReference PlayerAssetReference)
        {
            PlayerCharacterInitialized=false;
            GameObject player = GameMode.IPlayerCentric.Player;
            if (player==null)
            {
                AddressableFunctions.Handler.SpawnAssetReference(PlayerAssetReference,(obj)=> player=obj);
                while( player==null)
                {
                    yield return null;        
                }
            }
            //CameraPlayer.Main.Component.SetMainFocusTo(player.transform);
            GameMode.IPlayerCentric.SwitchCameraToNewPlayer(null,player);

            PlayerObject = player;
            PlayerCharacterInitialized=true;
        }

        private void OnEnable()
        {            
            UserInput.Instance.PlayerInputActions.UserControls.MenuKey.started += MenuKeyActivated;
        }
        private void OnDisable()
        {
            if(UserInput.Instance)
                UserInput.Instance.PlayerInputActions.UserControls.MenuKey.started -= MenuKeyActivated;
        }
        

        public void SpawnCharacterInASpawnPosition()
        {
            AssignControlToPlayer(PlayerObject.GetComponent<UnitSystem.GameUnit>());
            List<Transform> Targets= new List<Transform>();
            Targets.Add(PlayerObject.transform);
            Targets.Add(CameraManager.Instance.GetCurrentVirtualCamTransform().transform);
            SpawnPosition.SpawnAtAnyPosition(Targets);
        }

        private void Update()
        {
            if(!isRunning || GameTransitionManager.Instance.isTransitioning)
                return;
        
            if(CanPause && _menuKeyDown)
            {
                _menuKeyDown=false;
                TogglePause();
            }
        }

        private void TogglePause()
        {
            if(GameState.isPaused)
            {
                const float NormalTimeScale = 1.0f;
                Time.timeScale = NormalTimeScale;
                Time.fixedDeltaTime= 1/GameState.GetTickRate();
                GameState.isPaused=false;
            }
            else
            {
                const float PausedTimeScale = 0.0f;
                Time.timeScale = PausedTimeScale;
                Time.fixedDeltaTime= 1/GameState.GetTickRate();
                GameState.isPaused=true;
            }
        }

        private void MenuKeyActivated(InputAction.CallbackContext obj)
        {
            _menuKeyDown=true;
        }

        public override void Begin()
        {
            
            _systemSettings= Service.ServiceLocator.Current.Get<SettingsManager>().GetSystemSettings();

            if(isReady())
            {
                //CameraManager.Instance.PlayerCameraManagerReference.GetCinemachineBrain().ManualUpdate();
                isRunning=true;
                StartCoroutine(SetCamerasToDefaultStart());
            }
            else
            {
                Debug.LogError("FreeRoamMode cannot start.");
                if(_systemSettings==null)
                {
                    Debug.LogError("System Settings cannot be found.");
                }
                if(PlayerUIInitialized==false)
                {
                    Debug.LogError("Player UI has not been initialized.");
                }
                
                if(GameMode.IPlayerCentric.Player==null)
                {
                    Debug.LogError("Player is not set.");
                }

                
                
                isRunning=false;
            }

            if(isRunning)
            {
                //Fade IN
                GameTransitionManager.Instance.StandardFadeIn();
            }
        }

        private IEnumerator SetCamerasToDefaultStart()
        {
            yield return new WaitForSecondsRealtime(_systemSettings.DefaultInitialWaitTimeBeforeTransitionDuration/2);
            CameraManager.Instance.PlayerCameraManagerReference.ResetCamerasToDefaultStates();
        }

        public override void End()
        {
            isRunning=false;
            GameTransitionManager.Instance.StandardFadeOut();
        }

        public override bool isReady()
        {
            bool SystemSettingsExist = _systemSettings!=null;
            bool PlayerExists=GameMode.IPlayerCentric.Player!=null;
            return PlayerUIInitialized && (PlayerExists) && SystemSettingsExist;
        }
    }
}
