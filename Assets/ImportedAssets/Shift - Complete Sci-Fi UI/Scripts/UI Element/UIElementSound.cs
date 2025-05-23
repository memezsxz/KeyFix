﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIElementSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [Header("Resources")] public UIManager UIManagerAsset;

        public AudioSource audioObject;

        [Header("Custom SFX")] public AudioClip hoverSFX;

        public AudioClip clickSFX;

        [Header("Settings")] public bool enableHoverSound = true;

        public bool enableClickSound = true;
        public bool checkForInteraction = true;

        private Button sourceButton;

        private void OnEnable()
        {
            if (UIManagerAsset == null)
                try
                {
                    UIManagerAsset = Resources.Load<UIManager>("Shift UI Manager");
                }
                catch
                {
                    Debug.Log("<b>[UI Element Sound]</b> No UI Manager found.", this);
                    enabled = false;
                }

            if (Application.isPlaying && audioObject == null)
                try
                {
                    audioObject = GameObject.Find("UI Audio").GetComponent<AudioSource>();
                }
                catch
                {
                    Debug.Log("<b>[UI Element Sound]</b> No Audio Source found.", this);
                }

            if (checkForInteraction) sourceButton = gameObject.GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (checkForInteraction && sourceButton != null && sourceButton.interactable == false)
                return;

            if (enableClickSound)
            {
                if (clickSFX == null)
                    audioObject.PlayOneShot(UIManagerAsset.clickSound);
                else
                    audioObject.PlayOneShot(clickSFX);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (checkForInteraction && sourceButton != null && sourceButton.interactable == false)
                return;

            if (enableHoverSound)
            {
                if (hoverSFX == null)
                    audioObject.PlayOneShot(UIManagerAsset.hoverSound);
                else
                    audioObject.PlayOneShot(hoverSFX);
            }
        }
    }
}