﻿using System;
using System.Collections.Generic;
using System.IO;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
//Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
#endif

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_Window : EditorWindow
    {
        public const int WINDOWMENUITEMPRIO = 11;
        public const string VERSION = "2.2.7";
        private static AH_Window m_window;
        public static float ButtonMaxHeight = 18;

        [SerializeField]
        private TreeViewState m_TreeViewState; // Serialized in the window layout file so it survives assembly reloading

        [SerializeField] private MultiColumnHeaderState m_MultiColumnHeaderState;

        [SerializeField] public AH_BuildInfoManager buildInfoManager;

        //Button guiContent
        [SerializeField] private GUIContent guiContentLoadBuildInfo;
        [SerializeField] private GUIContent guiContentSettings;
        [SerializeField] private GUIContent guiContentGenerateReferenceGraph;
        [SerializeField] private GUIContent guiContentDuplicates;

        //Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
        [SerializeField] private GUIContent guiContentBuildReport;
#endif
        [SerializeField] private GUIContent guiContentReadme;
        [SerializeField] private GUIContent guiContentDeleteAll;
        [SerializeField] private GUIContent guiContentRefresh;

        [NonSerialized] private bool m_Initialized;

        private SearchField m_SearchField;
        private AH_TreeViewWithTreeModel m_TreeView;

        //UI Rect
        public bool m_BuildLogLoaded { get; set; }

        private Rect toolbarRect => new(UiStartPos.x,
            UiStartPos.y + (AH_SettingsManager.Instance.HideButtonText ? 20 : 0), position.width - UiStartPos.x * 2,
            20f);

        private Rect multiColumnTreeViewRect => new(UiStartPos.x,
            UiStartPos.y + 20 + (AH_SettingsManager.Instance.HideButtonText ? 20 : 0),
            position.width - UiStartPos.x * 2,
            position.height - 90 - (AH_SettingsManager.Instance.HideButtonText ? 20 : 0));

        private Rect assetInfoRect => new(UiStartPos.x, position.height - 66f, position.width - UiStartPos.x * 2, 16f);

        private Rect bottomToolbarRect =>
            new(UiStartPos.x, position.height - 18, position.width - UiStartPos.x * 2, 16f);

        public Vector2 UiStartPos { get; set; } = new(10, 50);

        private void OnEnable()
        {
            AH_SerializationHelper.NewBuildInfoCreated += onBuildInfoCreated;
        }

        private void OnDisable()
        {
            AH_SerializationHelper.NewBuildInfoCreated -= onBuildInfoCreated;
        }

        private void OnDestroy()
        {
            AH_TreeViewSelectionInfo.OnAssetDeleted -= m_window.OnAssetDeleted;
#if UNITY_2018_1_OR_NEWER
            EditorApplication.projectChanged -= m_window.OnProjectChanged;
#elif UNITY_5_6_OR_NEWER
            EditorApplication.projectWindowChanged -= m_window.OnProjectChanged;
#endif
        }

        private void OnGUI()
        {
            /*if (Application.isPlaying)
                return;*/

            InitIfNeeded();
            doHeader();

            if (buildInfoManager == null || !buildInfoManager.HasSelection)
            {
                doNoBuildInfoLoaded();
                return;
            }

            if (buildInfoManager.IsProjectClean() && ((AH_MultiColumnHeader)m_TreeView.multiColumnHeader).ShowMode ==
                AH_MultiColumnHeader.AssetShowMode.Unused)
            {
                Heureka_WindowStyler.DrawCenteredImage(m_window, AH_EditorData.Instance.AchievementIcon.Icon);
                return;
            }

            doSearchBar(toolbarRect);
            doTreeView(multiColumnTreeViewRect);

            doBottomToolBar(bottomToolbarRect);
        }

        private void OnInspectorUpdate()
        {
            if (!m_window)
                initializeWindow();
        }

        //Add menu named "Asset Hunter" to the window menu  
        [MenuItem("Tools/Asset Hunter PRO/Asset Hunter PRO _%h", priority = WINDOWMENUITEMPRIO)]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Asset Hunter PRO", priority = WINDOWMENUITEMPRIO)]
        public static void OpenAssetHunter()
        {
            if (!m_window)
                initializeWindow();
        }

        private static AH_Window initializeWindow()
        {
            //Open ReadMe
            Heureka_PackageDataManagerEditor.SelectReadme();

            m_window = GetWindow<AH_Window>();

            AH_TreeViewSelectionInfo.OnAssetDeleted += m_window.OnAssetDeleted;
#if UNITY_2018_1_OR_NEWER
            EditorApplication.projectChanged += m_window.OnProjectChanged;
#elif UNITY_5_6_OR_NEWER
            EditorApplication.projectWindowChanged += m_window.OnProjectChanged;
#endif

            if (m_window.buildInfoManager == null)
                m_window.buildInfoManager = CreateInstance<AH_BuildInfoManager>();

            m_window.initializeGUIContent();

            //Subscribe to changes to list of ignored items
            AH_SettingsManager.Instance.IgnoreListUpdatedEvent += m_window.OnIgnoreListUpdatedEvent;

            return m_window;
        }

        internal static AH_BuildInfoManager GetBuildInfoManager()
        {
            if (!m_window)
                initializeWindow();

            return m_window.buildInfoManager;
        }

        private void OnProjectChanged()
        {
            buildInfoManager.ProjectDirty = true;
        }

        //Callback
        private void OnAssetDeleted()
        {
            //TODO need to improve the deletion of empty folder. Currently leaves meta file behind, causing warnings
            if (EditorUtility.DisplayDialog("Delete empty folders", "Do you want to delete any empty folders?", "Yes",
                    "No")) deleteEmptyFolders();

            //This might be called excessively
            if (AH_SettingsManager.Instance.AutoRefreshLog)
            {
                RefreshBuildLog();
            }
            else
            {
                if (EditorUtility.DisplayDialog("Refresh Asset Hunter Log", "Do you want to refresh the loaded log",
                        "Yes", "No")) RefreshBuildLog();
            }
        }

        //callback
        private void onBuildInfoCreated(string path)
        {
            if (EditorUtility.DisplayDialog(
                    "New buildinfo log created",
                    "Do you want to load it into Asset Hunter",
                    "Ok", "Cancel"))
            {
                m_Initialized = false;
                buildInfoManager.SelectBuildInfo(path);
            }
        }

        private void InitIfNeeded()
        {
            //We dont need to do stuff when in play mode
            if (buildInfoManager && buildInfoManager.HasSelection && !m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                var firstInit = m_MultiColumnHeaderState == null;
                var headerState =
                    AH_TreeViewWithTreeModel.CreateDefaultMultiColumnHeaderState(multiColumnTreeViewRect.width);
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new AH_MultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<AH_TreeviewElement>(buildInfoManager.GetTreeViewData());

                m_TreeView = new AH_TreeViewWithTreeModel(m_TreeViewState, multiColumnHeader, treeModel);

                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

                m_Initialized = true;
                buildInfoManager.ProjectDirty = false;
            }

            //This is an (ugly) fix to make sure we dotn loose our icons due to some singleton issues after play/stop
            if (guiContentRefresh.image == null)
                initializeGUIContent();
        }

        private void deleteEmptyFolders()
        {
            var emptyfolders = new List<string>();
            checkEmptyFolder(Application.dataPath, emptyfolders);

            if (emptyfolders.Count > 0)
            {
#if UNITY_2020_1_OR_NEWER
                var failedPaths = new List<string>();
                AssetDatabase.DeleteAssets(emptyfolders.ToArray(), failedPaths);
#else
            foreach (var folder in emptyfolders)
            {
                    FileUtil.DeleteFileOrDirectory(folder);
                //AssetDatabase.DeleteAsset(folder);
            }
#endif
                Debug.Log($"AH: Deleted {emptyfolders.Count} empty folders ");
                AssetDatabase.Refresh();
            }
        }

        private bool checkEmptyFolder(string dataPath, List<string> emptyfolders)
        {
            if (dataPath.EndsWith(".git", StringComparison.InvariantCultureIgnoreCase))
                return false;

            var files = Directory.GetFiles(dataPath);
            var hasValidAsset = false;

            for (var i = 0; i < files.Length; i++)
            {
                string relativePath;
                string assetID;
                AH_Utils.GetRelativePathAndAssetID(files[i], out relativePath, out assetID);

                //This folder has a valid asset inside
                if (!string.IsNullOrEmpty(assetID))
                {
                    hasValidAsset = true;
                    break;
                }
            }

            var folders = Directory.GetDirectories(dataPath);
            var hasFolderWithContents = false;

            for (var i = 0; i < folders.Length; i++)
            {
                var folderIsEmpty = checkEmptyFolder(folders[i], emptyfolders);
                if (!folderIsEmpty)
                    hasFolderWithContents = true;
                else
                    emptyfolders.Add(FileUtil.GetProjectRelativePath(folders[i]));
            }

            return !hasValidAsset && !hasFolderWithContents;
        }

        private void initializeGUIContent()
        {
            titleContent = new GUIContent("Asset Hunter", AH_EditorData.Instance.WindowPaneIcon.Icon);

            guiContentLoadBuildInfo = new GUIContent("Load", AH_EditorData.Instance.LoadLogIcon.Icon,
                "Load info from a previous build");
            guiContentSettings = new GUIContent("Settings", AH_EditorData.Instance.Settings.Icon, "Open settings");
            guiContentGenerateReferenceGraph = new GUIContent("Dependencies", AH_EditorData.Instance.RefFromIcon.Icon,
                "See asset dependency graph");
            guiContentDuplicates = new GUIContent("Duplicates", AH_EditorData.Instance.DuplicateIcon.Icon,
                "Find duplicate assets");

            //Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
            guiContentBuildReport = new GUIContent("Report", AH_EditorData.Instance.ReportIcon.Icon,
                "Build report overview (Build size information)");
#endif
            guiContentReadme = new GUIContent("Info", AH_EditorData.Instance.HelpIcon.Icon,
                "Open the readme file for all installed Heureka Games products");
            guiContentDeleteAll = new GUIContent("Clean ALL", AH_EditorData.Instance.DeleteIcon.Icon,
                "Delete ALL unused assets in project ({0}) Remember to manually exclude relevant assets in the settings window");
            guiContentRefresh = new GUIContent(AH_EditorData.Instance.RefreshIcon.Icon, "Refresh data");
        }

        private void doNoBuildInfoLoaded()
        {
            Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, 380f, 110f,
                "Buildinfo not yet loaded" + Environment.NewLine + "Load existing / create new build");
        }

        private void doHeader()
        {
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Pink, "ASSET HUNTER PRO", VERSION);
            EditorGUILayout.BeginHorizontal();

            var infoLoaded = buildInfoManager != null && buildInfoManager.HasSelection;
            if (infoLoaded)
            {
                var RefreshGUIContent = new GUIContent(guiContentRefresh);
                var origColor = GUI.color;
                if (buildInfoManager.ProjectDirty)
                {
                    GUI.color = Heureka_WindowStyler.clr_Red;
                    RefreshGUIContent.tooltip = string.Format("{0}{1}", RefreshGUIContent.tooltip,
                        " (Project has changed which means that treeview is out of date)");
                }

                if (doSelectionButton(RefreshGUIContent))
                    RefreshBuildLog();

                GUI.color = origColor;
            }


            if (doSelectionButton(guiContentLoadBuildInfo))
                openBuildInfoSelector();

            if (doSelectionButton(guiContentDuplicates))
                AH_DuplicatesWindow.Init(Docker.DockPosition.Left);

            if (doSelectionButton(guiContentGenerateReferenceGraph))
                AH_DependencyGraphWindow.Init(Docker.DockPosition.Right);

            //Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
            if (infoLoaded && doSelectionButton(guiContentBuildReport))
                AH_BuildReportWindow.Init();
#endif
            if (doSelectionButton(guiContentSettings))
                AH_SettingsWindow.Init(true);

            if (infoLoaded && m_TreeView.GetCombinedUnusedSize() > 0)
            {
                var sizeAsString = AH_Utils.GetSizeAsString(m_TreeView.GetCombinedUnusedSize());

                var instancedGUIContent = new GUIContent(guiContentDeleteAll);
                instancedGUIContent.tooltip = string.Format(instancedGUIContent.tooltip, sizeAsString);
                if (AH_SettingsManager.Instance.HideButtonText)
                    instancedGUIContent.text = null;

                GUIStyle btnStyle = "button";
                var newStyle = new GUIStyle(btnStyle);
                newStyle.normal.textColor = Heureka_WindowStyler.clr_Pink;

                m_TreeView.DrawDeleteAllButton(instancedGUIContent, newStyle,
                    GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText
                        ? ButtonMaxHeight * 2f
                        : ButtonMaxHeight));
            }

            GUILayout.FlexibleSpace();
            GUILayout.Space(20);

            if (m_TreeView != null)
                m_TreeView.AssetSelectionToolBarGUI();

            if (doSelectionButton(guiContentReadme))
            {
                Heureka_PackageDataManagerEditor.SelectReadme();
                if (AH_EditorData.Instance.Documentation != null)
                    AssetDatabase.OpenAsset(AH_EditorData.Instance.Documentation);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void doSearchBar(Rect rect)
        {
            if (m_TreeView != null)
                m_TreeView.searchString = m_SearchField.OnGUI(rect, m_TreeView.searchString);
        }

        private void doTreeView(Rect rect)
        {
            if (m_TreeView != null)
                m_TreeView.OnGUI(rect);
        }

        private void doBottomToolBar(Rect rect)
        {
            if (m_TreeView == null)
                return;

            GUILayout.BeginArea(rect);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUIStyle style = "miniButton";

                if (GUILayout.Button("Expand All", style)) m_TreeView.ExpandAll();

                if (GUILayout.Button("Collapse All", style)) m_TreeView.CollapseAll();

                GUILayout.Label("Build: " + buildInfoManager.GetSelectedBuildDate() + " (" +
                                buildInfoManager.GetSelectedBuildTarget() + ")");
                GUILayout.FlexibleSpace();
                GUILayout.Label(buildInfoManager.TreeView != null
                    ? AssetDatabase.GetAssetPath(buildInfoManager.TreeView)
                    : string.Empty);
                GUILayout.FlexibleSpace();

                if (((AH_MultiColumnHeader)m_TreeView.multiColumnHeader).mode == AH_MultiColumnHeader.Mode.SortedList ||
                    !string.IsNullOrEmpty(m_TreeView.searchString))
                    if (GUILayout.Button("Return to Treeview", style))
                        m_TreeView.ShowTreeMode();

                var exportContent =
                    new GUIContent("Export list", "Export all the assets in the list above to a json file");
                if (GUILayout.Button(exportContent, style)) AH_ElementList.DumpCurrentListToFile(m_TreeView);
            }

            GUILayout.EndArea();
        }

        private bool doSelectionButton(GUIContent content)
        {
            var btnContent = new GUIContent(content);
            if (AH_SettingsManager.Instance.HideButtonText)
                btnContent.text = null;

            return GUILayout.Button(btnContent,
                GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText
                    ? ButtonMaxHeight * 2f
                    : ButtonMaxHeight));
        }

        private void OnIgnoreListUpdatedEvent()
        {
            buildInfoManager.ProjectDirty = true;

            if (AH_SettingsManager.Instance.AutoOpenLog)
                RefreshBuildLog();
        }

        private void RefreshBuildLog()
        {
            if (buildInfoManager != null && buildInfoManager.HasSelection)
            {
                m_Initialized = false;
                buildInfoManager.RefreshBuildInfo();
            }
        }

        private void openBuildInfoSelector()
        {
            var fileSelected = EditorUtility.OpenFilePanel("", AH_SerializationHelper.GetBuildInfoFolder(),
                AH_SerializationHelper.BuildInfoExtension);
            if (!string.IsNullOrEmpty(fileSelected))
            {
                m_Initialized = false;
                buildInfoManager.SelectBuildInfo(fileSelected);
            }
        }
    }
}