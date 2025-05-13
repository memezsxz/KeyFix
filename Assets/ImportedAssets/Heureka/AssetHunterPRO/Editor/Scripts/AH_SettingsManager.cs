using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SettingsManager
    {
        #region singleton

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AH_SettingsManager()
        {
            Instance.Init();
        }

        private AH_SettingsManager()
        {
        }

        public static AH_SettingsManager Instance { get; } = new();

        #endregion

        public delegate void IgnoreListUpdatedHandler();

        public event IgnoreListUpdatedHandler IgnoreListUpdatedEvent;

        #region Fields

        [SerializeField] private int ignoredListChosenIndex;

        private static readonly string
            ProjectPostFix =
                "." + Application
                    .dataPath; // AssetDatabase.AssetPathToGUID(FileUtil.GetProjectRelativePath(Application.dataPath));

        private static readonly string PrefsAutoCreateLog = "AH.AutoCreateLog" + ProjectPostFix;
        private static readonly string PrefsAutoOpenLog = "AH.AutoOpenLog" + ProjectPostFix;
        private static readonly string PrefsAutoRefreshLog = "AH.AutoRefreshLog" + ProjectPostFix;
        private static readonly string PrefsEstimateAssetSize = "AH.PrefsEstimateAssetSize" + ProjectPostFix;

        private static readonly string PrefsHideButtonText = "AH.HideButtonText" + ProjectPostFix;
        private static readonly string PrefsIgnoreScriptFiles = "AH.IgnoreScriptfiles" + ProjectPostFix;
        private static readonly string PrefsIgnoredTypes = "AH.DefaultIgnoredTypes" + ProjectPostFix;
        private static readonly string PrefsIgnoredPathEndsWith = "AH.IgnoredPathEndsWith" + ProjectPostFix;
        private static readonly string PrefsIgnoredExtensions = "AH.IgnoredExtensions" + ProjectPostFix;
        private static readonly string PrefsIgnoredFiles = "AH.IgnoredFiles" + ProjectPostFix;
        private static readonly string PrefsIgnoredFolders = "AH.IgnoredFolders" + ProjectPostFix;
        private static readonly string PrefsUserPrefPath = "AH.UserPrefPath" + ProjectPostFix;
        private static readonly string PrefsBuildInfoPath = "AH.BuildInfoPath" + ProjectPostFix;

        internal static readonly bool InitialValueAutoCreateLog = true;
        internal static readonly bool InitialValueAutoOpenLog = false;
        internal static readonly bool InitialValueAutoRefreshLog = false;
        internal static readonly bool InitialValueEstimateAssetSize = false;
        internal static readonly bool InitialValueHideButtonText = true;
        internal static readonly bool InitialIgnoreScriptFiles = true;

        internal static readonly string InitialUserPrefPath =
            Application.dataPath + Path.DirectorySeparatorChar + "AH_Prefs";

        internal static readonly string InitialBuildInfoPath = Directory.GetParent(Application.dataPath).FullName +
                                                               Path.DirectorySeparatorChar + "SerializedBuildInfo";


        //Types to Ignore by default
#if UNITY_2017_3_OR_NEWER
        internal static readonly List<Type> InitialValueIgnoredTypes = new()
        {
#if UNITY_2021_2_OR_NEWER
            typeof(ShaderInclude), //Have to exclude this here because Unitys AssetDatabase.GetDependencies() does not include shaderincludes for some reason :(
#endif
            typeof(AssemblyDefinitionAsset)
#if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            , typeof(MonoScript)
#endif
        };
#else
        internal readonly static List<Type> InitialValueIgnoredTypes = new List<Type>() {
#if !AH_SCRIPT_ALLOW //DEFINED IN AH_PREPROCESSOR
            typeof(MonoScript)
#endif
        };
#endif

        //File extensions to Ignore by default
        internal static readonly List<string> InitialValueIgnoredExtensions = new()
        {
            ".dll",
            "." + AH_SerializationHelper.SettingsExtension,
            "." + AH_SerializationHelper.BuildInfoExtension
        };

        //List of strings which, if contained in asset path, is ignored (Editor, Resources, etc)
        internal static readonly List<string> InitialValueIgnoredPathEndsWith = new()
        {
            string.Format("{0}heureka", Path.DirectorySeparatorChar),
            string.Format("{0}editor", Path.DirectorySeparatorChar),
            string.Format("{0}plugins", Path.DirectorySeparatorChar),
            string.Format("{0}gizmos", Path.DirectorySeparatorChar),
            string.Format("{0}editor default resources", Path.DirectorySeparatorChar)
        };

        internal static readonly List<string> InitialValueIgnoredFiles = new();
        internal static readonly List<string> InitialValueIgnoredFolders = new();

        [SerializeField] private AH_ExclusionTypeList ignoredListTypes;
        [SerializeField] private AH_IgnoreList ignoredListPathEndsWith;
        [SerializeField] private AH_IgnoreList ignoredListExtensions;
        [SerializeField] private AH_IgnoreList ignoredListFiles;
        [SerializeField] private AH_IgnoreList ignoredListFolders;

        #endregion

        #region Properties

        public bool AutoCreateLog
        {
            get => (!EditorPrefs.HasKey(PrefsAutoCreateLog) && InitialValueAutoCreateLog) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoCreateLog));
            internal set => EditorPrefs.SetInt(PrefsAutoCreateLog, AH_Utils.BoolToInt(value));
        }

        public bool AutoOpenLog
        {
            get => (!EditorPrefs.HasKey(PrefsAutoOpenLog) && InitialValueAutoOpenLog) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoOpenLog));
            internal set => EditorPrefs.SetInt(PrefsAutoOpenLog, AH_Utils.BoolToInt(value));
        }

        public bool AutoRefreshLog
        {
            get => (!EditorPrefs.HasKey(PrefsAutoRefreshLog) && InitialValueAutoRefreshLog) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsAutoRefreshLog));
            internal set => EditorPrefs.SetInt(PrefsAutoRefreshLog, AH_Utils.BoolToInt(value));
        }

        public bool EstimateAssetSize
        {
            get => (!EditorPrefs.HasKey(PrefsEstimateAssetSize) && InitialValueEstimateAssetSize) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsEstimateAssetSize));
            internal set => EditorPrefs.SetInt(PrefsEstimateAssetSize, AH_Utils.BoolToInt(value));
        }

        public bool HideButtonText
        {
            get => (!EditorPrefs.HasKey(PrefsHideButtonText) && InitialValueHideButtonText) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsHideButtonText));
            internal set => EditorPrefs.SetInt(PrefsHideButtonText, AH_Utils.BoolToInt(value));
        }

        public bool IgnoreScriptFiles
        {
            get => (!EditorPrefs.HasKey(PrefsIgnoreScriptFiles) && InitialIgnoreScriptFiles) ||
                   AH_Utils.IntToBool(EditorPrefs.GetInt(PrefsIgnoreScriptFiles));
            internal set => EditorPrefs.SetInt(PrefsIgnoreScriptFiles, AH_Utils.BoolToInt(value));
        }

        public string UserPreferencePath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsUserPrefPath))
                    return EditorPrefs.GetString(PrefsUserPrefPath);
                return InitialUserPrefPath;
            }
            internal set => EditorPrefs.SetString(PrefsUserPrefPath, value);
        }

        public string BuildInfoPath
        {
            get
            {
                if (EditorPrefs.HasKey(PrefsBuildInfoPath))
                    return EditorPrefs.GetString(PrefsBuildInfoPath);
                return InitialBuildInfoPath;
            }
            internal set => EditorPrefs.SetString(PrefsBuildInfoPath, value);
        }

        public GUIContent[] GUIcontentignoredLists = new GUIContent[5]
        {
            new("Endings"),
            new("Types"),
            new("Folders"),
            new("Files"),
            new("Extentions")
        };

        #endregion

        private void Init()
        {
            ignoredListPathEndsWith = new AH_IgnoreList(new IgnoredEventActionPathEndsWith(0, onIgnoreButtonDown),
                InitialValueIgnoredPathEndsWith, PrefsIgnoredPathEndsWith);
            ignoredListTypes = new AH_ExclusionTypeList(new IgnoredEventActionType(1, onIgnoreButtonDown),
                InitialValueIgnoredTypes, PrefsIgnoredTypes);
            ignoredListFolders = new AH_IgnoreList(new IgnoredEventActionFolder(2, onIgnoreButtonDown),
                InitialValueIgnoredFolders, PrefsIgnoredFolders);
            ignoredListFiles = new AH_IgnoreList(new IgnoredEventActionFile(3, onIgnoreButtonDown),
                InitialValueIgnoredFiles, PrefsIgnoredFiles);
            ignoredListExtensions = new AH_IgnoreList(new IgnoredEventActionExtension(4, onIgnoreButtonDown),
                InitialValueIgnoredExtensions, PrefsIgnoredExtensions);

            //Todo subscribing to these 5 times, means that we might refresh buildinfo 5 times when reseting...We might be able to batch that somehow
            ignoredListPathEndsWith.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListTypes.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListFolders.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListFiles.ListUpdatedEvent += OnListUpdatedEvent;
            ignoredListExtensions.ListUpdatedEvent += OnListUpdatedEvent;
        }

        private void OnListUpdatedEvent()
        {
            if (IgnoreListUpdatedEvent != null)
                IgnoreListUpdatedEvent();
        }

        internal void ResetAll()
        {
            ignoredListPathEndsWith.Reset();
            ignoredListTypes.Reset();
            ignoredListExtensions.Reset();
            ignoredListFiles.Reset();
            ignoredListFolders.Reset();

            AutoCreateLog = InitialValueAutoCreateLog;
            AutoOpenLog = InitialValueAutoOpenLog;
            AutoRefreshLog = InitialValueAutoRefreshLog;
            EstimateAssetSize = InitialValueEstimateAssetSize;
            HideButtonText = InitialValueHideButtonText;
            IgnoreScriptFiles = InitialIgnoreScriptFiles;
            UserPreferencePath = InitialUserPrefPath;
            BuildInfoPath = InitialBuildInfoPath;
        }

        internal void DrawIgnored()
        {
            EditorGUILayout.HelpBox("IGNORE ASSETS" + Environment.NewLine + "-Select asset in project view to ignore",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            ignoredListChosenIndex = GUILayout.Toolbar(ignoredListChosenIndex, GUIcontentignoredLists);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            drawIgnoreButtons();

            switch (ignoredListChosenIndex)
            {
                case 0:
                    ignoredListPathEndsWith.OnGUI();
                    break;
                case 1:
                    ignoredListTypes.OnGUI();
                    break;
                case 2:
                    ignoredListFolders.OnGUI();
                    break;
                case 3:
                    ignoredListFiles.OnGUI();
                    break;
                case 4:
                    ignoredListExtensions.OnGUI();
                    break;
            }
        }

        private void drawIgnoreButtons()
        {
            GUILayout.Space(12);
            ignoredListPathEndsWith.DrawIgnoreButton();
            ignoredListTypes.DrawIgnoreButton();
            ignoredListFolders.DrawIgnoreButton();
            ignoredListFiles.DrawIgnoreButton();
            ignoredListExtensions.DrawIgnoreButton();
            GUILayout.Space(4);
        }

        //Callback from Ignore button down
        private void onIgnoreButtonDown(int exclusionIndex)
        {
            ignoredListChosenIndex = exclusionIndex;
        }

        //public List<Type> GetIgnoredTypes() { return ignoredListTypes.GetIgnored(); }
        public List<string> GetIgnoredPathEndsWith()
        {
            return ignoredListPathEndsWith.GetIgnored();
        }

        public List<string> GetIgnoredFileExtentions()
        {
            return ignoredListExtensions.GetIgnored();
        }

        public List<string> GetIgnoredFiles()
        {
            return ignoredListFiles.GetIgnored();
        }

        public List<string> GetIgnoredFolders()
        {
            return ignoredListFolders.GetIgnored();
        }

        private int drawSetting(string title, int value, int min, int max, string prefixAppend)
        {
            EditorGUILayout.PrefixLabel(title + prefixAppend);
            return EditorGUILayout.IntSlider(value, min, max);
        }

        internal void DrawSettings()
        {
            EditorGUILayout.HelpBox("File save locations", MessageType.None);

            UserPreferencePath = drawSettingsFolder("User prefs", UserPreferencePath, InitialUserPrefPath);
            BuildInfoPath = drawSettingsFolder("Build info", BuildInfoPath, InitialBuildInfoPath);
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Settings", MessageType.None);
            AutoCreateLog = drawSetting("Auto create log when building", AutoCreateLog, InitialValueAutoCreateLog);
            AutoOpenLog = drawSetting("Auto open log location after building", AutoOpenLog, InitialValueAutoOpenLog);
            AutoRefreshLog = drawSetting("Auto refresh when project changes", AutoRefreshLog,
                InitialValueAutoRefreshLog);
            EstimateAssetSize = drawSetting("Estimate runtime filesize for each asset", EstimateAssetSize,
                InitialValueEstimateAssetSize);
            HideButtonText = drawSetting("Hide buttontexts", HideButtonText, InitialValueHideButtonText);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            IgnoreScriptFiles = drawSetting("Ignore script files", IgnoreScriptFiles, InitialIgnoreScriptFiles);

            if (EditorGUI.EndChangeCheck())
            {
                //ADD OR REMOVE DEFINITION FOR PREPROCESSING
                AH_PreProcessor.AddDefineSymbols(AH_PreProcessor.DefineScriptAllow, !IgnoreScriptFiles);
                ignoredListTypes.IgnoreType(typeof(MonoScript), IgnoreScriptFiles);

                if (!IgnoreScriptFiles)
                    EditorUtility.DisplayDialog("Now detecting unused scripts",
                        "This is an experimental feature, and it cannot promise with any certainty that script files marked as unused are indeed unused. Only works with scripts that are directly used in a scene - Use at your own risk",
                        "Ok");
            }

            var content = new GUIContent("EXPERIMENTAL FEATURE!",
                EditorGUIUtility.IconContent("console.warnicon.sml").image,
                "Cant be 100% sure script files are usused, so you need to handle with care");
            //TODO PARTIAL CLASSES
            //INHERITANCE
            //AddComponent<Type>
            //Reflection
            //Interfaces

            EditorGUILayout.LabelField(content, EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private string drawSettingsFolder(string title, string path, string defaultVal)
        {
            var validPath = path;
            var newPath = "";

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select", GUILayout.ExpandWidth(false)))
                newPath = EditorUtility.OpenFolderPanel("Select folder", path, "");

            if (newPath != "")
                validPath = newPath;

            var content = new GUIContent(title + ": " + AH_Utils.ShrinkPathMiddle(validPath, 44),
                title + " is saved at " + validPath);

            GUILayout.Label(content, defaultVal != path ? EditorStyles.boldLabel : EditorStyles.label);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            return validPath;
        }

        private bool drawSetting(string title, bool value, bool defaultVal)
        {
            return EditorGUILayout.ToggleLeft(title, value,
                defaultVal != value ? EditorStyles.boldLabel : EditorStyles.label);
        }


        internal bool HasIgnoredFolder(string folderPath, string assetID)
        {
            var IgnoredEnding = ignoredListPathEndsWith.ContainsElement(folderPath, assetID);
            var folderIgnored = ignoredListFolders.ContainsElement(folderPath, assetID);

            return IgnoredEnding || folderIgnored;
        }

        internal void AddIgnoredFolder(string element)
        {
            ignoredListFolders.AddToignoredList(element);
        }

        internal void AddIgnoredAssetTypes(string element)
        {
            ignoredListTypes.AddToignoredList(element);
        }

        internal void AddIgnoredAssetGUIDs(string element)
        {
            ignoredListFiles.AddToignoredList(element);
        }

        internal bool HasIgnoredAsset(string relativePath, string assetID)
        {
            var IgnoredType = ignoredListTypes.ContainsElement(relativePath, assetID);
            var IgnoredFile = ignoredListFiles.ContainsElement(relativePath, assetID);
            var IgnoredExtension = ignoredListExtensions.ContainsElement(relativePath, assetID);

            return IgnoredType || IgnoredFile || IgnoredExtension;
        }

        internal void SaveToFile()
        {
            var path = EditorUtility.SaveFilePanel(
                "Save current settings",
                AH_SerializationHelper.GetSettingFolder(),
                "AH_UserPrefs_" + Environment.UserName,
                AH_SerializationHelper.SettingsExtension);

            if (path.Length != 0)
                AH_SerializationHelper.SerializeAndSave(Instance, path);

            AssetDatabase.Refresh();
        }

        internal void LoadFromFile()
        {
            var path = EditorUtility.OpenFilePanel(
                "settings",
                AH_SerializationHelper.GetSettingFolder(),
                AH_SerializationHelper.SettingsExtension
            );

            if (path.Length != 0)
            {
                AH_SerializationHelper.LoadSettings(Instance, path);
                ignoredListTypes.Save();
                ignoredListPathEndsWith.Save();
                ignoredListTypes.Save();
                ignoredListExtensions.Save();
                ignoredListFolders.Save();
            }
        }
    }
}