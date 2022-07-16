using System.Collections;
using System.Collections.Generic;
using CharacterProperties;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Character
{
    public class CharacterStaminaBar : MonoBehaviour, ICharacterUI
    {
        private GameObject _associatedCharacter;
        public GameObject AssociatedCharacter { get { return _associatedCharacter; } }

        [Header("References")]
        private ICharacterModifierStamina Stamina;
        public Image StaminaImage;
        public Image StaminaContainerImage;
        [Header("Fade Out Times")]
        public float FadeOutDelayTime = 0.5f;
        public float FadeOutTime = 1.0f;
        private Timer FadeOutDelayTimer;
        private Timer FadeOutTimer;
        [Range(0,1f)]

        public float MinThresholdToFade=0.75f;
        [Header("Color")]

        public Color NormalColor = Color.yellow;
        public Color DangerColor = Color.red;
        [Range(0,1f)]
        public float MaxAlphaFill = 0.7f;
        public float MaxAlphaContainer = 0.5f;
        [Range(0,1f)]
        public float ColorStartChangeThreshold = 0.4f;
        [Range(0,1f)]
        public float ColorEndChangeThreshold = 0.2f;

        private void Start()
        {
            if (_associatedCharacter != null)
            {
                Stamina = _associatedCharacter.GetComponent<ICharacterModifierStamina>();
                Stamina.OnStaminaChanged -= HandleStamina;
                Stamina.OnStaminaChanged += HandleStamina;
            }

            FadeOutDelayTimer = new Timer(FadeOutDelayTime);
            FadeOutTimer = new Timer(FadeOutTime);
            ChangeImageAlphaValue(ref StaminaImage,0f);

            FadeOutDelayTimer.FinishTimer();
            FadeOutTimer.FinishTimer();
        }

        private void HandleStamina(float CurrentStamina, float minStamina, float maxStamina, bool shouldResetTimer = true)
        {
            float currentAlpha = StaminaImage.color.a;
            HandleStaminaImageColor(CurrentStamina / maxStamina);
            ChangeImageAlphaValue(ref StaminaImage,currentAlpha);

            StaminaImage.fillAmount = System.Math.Abs(CurrentStamina - minStamina) / (maxStamina - minStamina);

            if (shouldResetTimer)
            {
                FadeOutDelayTimer.ResetTimer();
                FadeOutTimer.ResetTimer();
            }
        }

        void OnGUI()
        {
            if (!GameState.isPaused)
            {
                FadeOutSequence();
            }
        }


        private void OnValidate()
        {
            if(StaminaImage)
                StaminaImage.color = NormalColor;
        }



        private void FadeOutSequence()
        {
            if(MinThresholdToFade>StaminaImage.fillAmount)
            {
                FadeOutDelayTimer.ResetTimer();
                FadeOutTimer.ResetTimer();
            }

            if (!FadeOutDelayTimer.Activated)
            {
                FadeOutDelayTimer.Tick();
                ChangeImageAlphaValue(ref StaminaImage,MaxAlphaFill);
                ChangeImageAlphaValue(ref StaminaContainerImage,MaxAlphaContainer);
            }
            else if (!FadeOutTimer.Activated)
            {
                FadeOutTimer.Tick();
                float percent = (FadeOutTimer.time / FadeOutTime);
                float percentAlpha = (1.0f - percent);
                ChangeImageAlphaValue(ref StaminaImage,percentAlpha*MaxAlphaFill);
                ChangeImageAlphaValue(ref StaminaContainerImage,percentAlpha*MaxAlphaContainer);
            }
            else
            {
                ChangeImageAlphaValue(ref StaminaImage,0f);
                ChangeImageAlphaValue(ref StaminaContainerImage,0.0f);
            }
        }

        private void HandleStaminaImageColor(float percent)
        {
            if (percent <= ColorStartChangeThreshold && percent > ColorEndChangeThreshold)
            {
                float lerped = 1 - ((percent - ColorEndChangeThreshold) / (ColorStartChangeThreshold - ColorEndChangeThreshold));
                StaminaImage.color = Color.Lerp(NormalColor, DangerColor, lerped);
            }
            else if (percent <= ColorEndChangeThreshold)
            {
                StaminaImage.color = DangerColor;
            }
            else
            {
                StaminaImage.color = NormalColor;
            }
        }

        private void ChangeImageAlphaValue(ref Image image,float percentAlpha)
        {
            if(image==null)
                return;
            if (percentAlpha < 0)
            {
                percentAlpha = 0;
            }
            Color StaminaColor = image.color;
            StaminaColor.a = percentAlpha;
            image.color = StaminaColor;
        }

        public void SetCharacter(GameObject CharacterObject)
        {
            TurnOnImage();
            if (CharacterObject.GetComponent<ICharacterModifierStamina>() == null)
            {
                TurnOffImage();
                return;
            }

            bool changingFrompPreviousStamina = false;
            if (_associatedCharacter != null) //If changing to new one
            {
                Stamina.OnStaminaChanged -= HandleStamina;
                changingFrompPreviousStamina = true;
            }
            _associatedCharacter = CharacterObject.gameObject;

            if (_associatedCharacter.GetComponent<ICharacterModifierStamina>() == null)
            {
                TurnOffImage();
                return;
            }

            Stamina = _associatedCharacter.GetComponent<ICharacterModifierStamina>();
            Stamina.OnStaminaChanged -= HandleStamina;
            Stamina.OnStaminaChanged += HandleStamina;

            if (changingFrompPreviousStamina)
            {
                HandleStamina(Stamina.GetCurrentStamina(), Stamina.GetMinStamina(), Stamina.GetMaxStamina(), false);
            }
        }

        private void TurnOffImage()
        {
            StaminaImage.enabled = false;
        }
        private void TurnOnImage()
        {
            StaminaImage.enabled = true;
        }
    }
}