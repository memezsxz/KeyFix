using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_EditorData : ScriptableObject
    {
        public delegate void EditorDataRefreshDelegate();

        private static AH_EditorData m_instance;

        public DefaultAsset Documentation;
        [SerializeField] public ConfigurableIcon WindowPaneIcon = new();
        [SerializeField] public ConfigurableIcon WindowHeaderIcon = new();
        [SerializeField] public ConfigurableIcon SceneIcon = new();
        [SerializeField] public ConfigurableIcon Settings = new();
        [SerializeField] public ConfigurableIcon LoadLogIcon = new();
        [SerializeField] public ConfigurableIcon GenerateIcon = new();
        [SerializeField] public ConfigurableIcon RefreshIcon = new();
        [SerializeField] public ConfigurableIcon MergerIcon = new();
        [SerializeField] public ConfigurableIcon HelpIcon = new();
        [SerializeField] public ConfigurableIcon AchievementIcon = new();
        [SerializeField] public ConfigurableIcon ReportIcon = new();
        [SerializeField] public ConfigurableIcon DeleteIcon = new();
        [SerializeField] public ConfigurableIcon RefToIcon = new();
        [SerializeField] public ConfigurableIcon RefFromIcon = new();
        [SerializeField] public ConfigurableIcon RefFromWhiteIcon = new();
        [SerializeField] public ConfigurableIcon DuplicateIcon = new();
        [SerializeField] public ConfigurableIcon DuplicateWhiteIcon = new();

        public static AH_EditorData Instance
        {
            get
            {
                if (!m_instance) m_instance = loadData();

                return m_instance;
            }
        }

        public static event EditorDataRefreshDelegate OnEditorDataRefresh;

        private static AH_EditorData loadData()
        {
            //LOGO ON WINDOW
            var configData = AssetDatabase.FindAssets("EditorData t:" + typeof(AH_EditorData), null);
            if (configData.Length >= 1)
                return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(configData[0]),
                    typeof(AH_EditorData)) as AH_EditorData;

            Debug.LogError("Failed to find config data");
            return null;
        }

        internal void RefreshData()
        {
            if (OnEditorDataRefresh != null)
                OnEditorDataRefresh();
        }
    }
}