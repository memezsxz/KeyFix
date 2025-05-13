using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames
{
    public class Heureka_EditorData : ScriptableObject
    {
        public delegate void EditorDataRefreshDelegate();

        private static Heureka_EditorData m_instance;

        public GUIStyle HeadlineStyle;

        public static Heureka_EditorData Instance
        {
            get
            {
                if (!m_instance) m_instance = loadData();

                return m_instance;
            }
        }

        public static event EditorDataRefreshDelegate OnEditorDataRefresh;

        private static Heureka_EditorData loadData()
        {
            //LOGO ON WINDOW
            var configData = AssetDatabase.FindAssets("EditorData t:" + typeof(Heureka_EditorData), null);
            if (configData.Length >= 1)
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(configData[0]),
                    typeof(Heureka_EditorData)) as Heureka_EditorData;

            Debug.LogError("Failed to find config data");
            return null;
        }

        internal void RefreshData()
        {
            if (OnEditorDataRefresh != null)
                OnEditorDataRefresh();
        }
    }

    [Serializable]
    public class ConfigurableIcon
    {
        [SerializeField] private bool isUsingDarkSkin;

        [SerializeField] private string buildInIconName = "";
        [SerializeField] private Texture iconCached;

        [SerializeField] private Texture m_iconNormalOverride;
        [SerializeField] private Texture m_iconProSkinOverride;

        [SerializeField] private bool m_darkSkinInvert;

        public ConfigurableIcon()
        {
            Heureka_EditorData.OnEditorDataRefresh += onEditorDataRefresh;
        }

        public Texture Icon
        {
            get
            {
                //TODO A way to make sure we update, if the user have changed skin
                if (isUsingDarkSkin != EditorGUIUtility.isProSkin)
                {
                    iconCached = null;
                    isUsingDarkSkin = EditorGUIUtility.isProSkin;
                }

                return iconCached != null ? iconCached : iconCached = GetInvertedForProSkin();
            }
        }

        private void onEditorDataRefresh()
        {
            iconCached = null;
        }

        protected Texture GetInvertedForProSkin()
        {
            var imageToUse = EditorGUIUtility.isProSkin ? m_iconProSkinOverride : m_iconNormalOverride;

            //If we want to use default unity icons and nothing has been setup to override
            if (imageToUse == null && !string.IsNullOrEmpty(buildInIconName))
                if (EditorGUIUtility.IconContent(buildInIconName) != null)
                    imageToUse = EditorGUIUtility.IconContent(buildInIconName).image;

            //Return current image if we dont have proskin, or dont want to invert
            if (!EditorGUIUtility.isProSkin || (EditorGUIUtility.isProSkin && !m_darkSkinInvert))
                return imageToUse;

            var readableTexture = getReadableTexture(imageToUse);
            var inverted = new Texture2D(readableTexture.width, readableTexture.height, TextureFormat.ARGB32, false);
            for (var x = 0; x < readableTexture.width; x++)
            for (var y = 0; y < readableTexture.height; y++)
            {
                var origColor = readableTexture.GetPixel(x, y);
                var invertedColor = new Color(1 - origColor.r, 1 - origColor.g, 1 - origColor.b, origColor.a);
                inverted.SetPixel(x, y, origColor.a > 0 ? invertedColor : origColor);
            }

            inverted.Apply();
            return inverted;
        }

        private Texture2D getReadableTexture(Texture imageToUse)
        {
            // Create a temporary RenderTexture of the same size as the texture
            var tmp = RenderTexture.GetTemporary(
                imageToUse.width,
                imageToUse.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);

            // Blit the pixels on texture to the RenderTexture
            Graphics.Blit(imageToUse, tmp);
            // Backup the currently set RenderTexture
            var previous = RenderTexture.active;
            // Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
            // Create a new readable Texture2D to copy the pixels to it
            var myTexture2D = new Texture2D(imageToUse.width, imageToUse.height);
            // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            // Reset the active RenderTexture
            RenderTexture.active = previous;
            // Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }
    }
}