using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace Michsky.UI.Shift
{
    [CreateAssetMenu(fileName = "New UI Manager", menuName = "Shift UI/New UI Manager")]
    public class UIManager : ScriptableObject
    {
        public enum BackgroundType
        {
            BASIC,
            ADVANCED
        }

        public enum ButtonThemeType
        {
            BASIC,
            CUSTOM
        }

        [HideInInspector] public bool enableDynamicUpdate = true;
        [HideInInspector] public bool enableExtendedColorPicker = true;
        [HideInInspector] public bool editorHints = true;

        // [Header("BACKGROUND")]
        public Color backgroundColorTint = new(255, 255, 255, 255);
        public BackgroundType backgroundType;
        public Sprite backgroundImage;
        public VideoClip backgroundVideo;
        public bool backgroundPreserveAspect;
        [Range(0.1f, 5)] public float backgroundSpeed = 1;

        // [Header("COLORS")]
        public Color primaryColor = new(255, 255, 255, 255);
        public Color secondaryColor = new(255, 255, 255, 255);
        public Color primaryReversed = new(255, 255, 255, 255);
        public Color negativeColor = new(255, 255, 255, 255);
        public Color backgroundColor = new(255, 255, 255, 255);

        // [Header("FONTS")]
        public TMP_FontAsset lightFont;
        public TMP_FontAsset regularFont;
        public TMP_FontAsset mediumFont;
        public TMP_FontAsset semiBoldFont;
        public TMP_FontAsset boldFont;

        // [Header("LOGO")]
        public Sprite gameLogo;
        public Color logoColor = new(255, 255, 255, 255);

        // [Header("PARTICLES")]
        public Color particleColor = new(255, 255, 255, 255);

        // [Header("SOUNDS")]
        public AudioClip backgroundMusic;
        public AudioClip hoverSound;
        public AudioClip clickSound;
    }
}