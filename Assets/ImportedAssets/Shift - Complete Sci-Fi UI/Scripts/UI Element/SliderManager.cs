using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class SliderManager : MonoBehaviour
    {
        [Header("Resources")] public TextMeshProUGUI valueText;

        [Header("Saving")] public bool enableSaving;

        public string sliderTag = "Tag Text";
        public float defaultValue = 1;

        [Header("Settings")] public bool usePercent;

        public bool showValue = true;
        public bool useRoundValue;

        private Slider mainSlider;
        private float saveValue;

        private void Start()
        {
            mainSlider = gameObject.GetComponent<Slider>();

            if (showValue == false)
                valueText.enabled = false;

            if (enableSaving)
            {
                if (PlayerPrefs.HasKey(sliderTag + "SliderValue") == false)
                    saveValue = defaultValue;
                else
                    saveValue = PlayerPrefs.GetFloat(sliderTag + "SliderValue");

                mainSlider.value = saveValue;

                mainSlider.onValueChanged.AddListener(delegate
                {
                    saveValue = mainSlider.value;
                    PlayerPrefs.SetFloat(sliderTag + "SliderValue", saveValue);
                });
            }
        }

        private void Update()
        {
            if (useRoundValue)
            {
                if (usePercent)
                    valueText.text = Mathf.Round(mainSlider.value * 1.0f) + "%";
                else
                    valueText.text = Mathf.Round(mainSlider.value * 1.0f).ToString();
            }

            else
            {
                if (usePercent)
                    valueText.text = mainSlider.value.ToString("F1") + "%";
                else
                    valueText.text = mainSlider.value.ToString("F1");
            }
        }
    }
}