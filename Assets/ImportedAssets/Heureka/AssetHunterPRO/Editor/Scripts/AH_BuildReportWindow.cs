﻿//Only avaliable in 2018

#if UNITY_2018_1_OR_NEWER

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_BuildReportWindow : EditorWindow
    {
        private static AH_BuildReportWindow m_window;

        //Adding same string multiple times in order to show more green and yellow than orange and red
        public static readonly List<string> ColorDotIconList = new()
        {
            "sv_icon_dot6_pix16_gizmo",
            "sv_icon_dot5_pix16_gizmo",
            "sv_icon_dot5_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot4_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo",
            "sv_icon_dot3_pix16_gizmo"
        };

        protected AH_BuildInfoManager buildInfoManager;
        private AH_BuildReportWindowData data;
        private Vector2 scrollPos;

        private void OnDestroy()
        {
            m_window.buildInfoManager.OnBuildInfoSelectionChanged -= m_window.OnBuildInfoSelectionChanged;
        }

        private void OnGUI()
        {
            if (!m_window)
                Init();
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Dark, "BUILD REPORT", AH_Window.VERSION);

            if (buildInfoManager == null || buildInfoManager.HasSelection == false)
            {
                Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, 310f,
                    110f, "No buildinfo currently loaded in main window");
                return;
            }

            if (buildInfoManager.IsMergedReport())
            {
                Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, 366f,
                    110f, "Buildreport window does not work with merged buildreports");
                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            data.OnGUI();

            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Tools/Asset Hunter PRO/Build report")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Build report")]
        public static void Init()
        {
            //Make sure it exists so we can attach this window next to it
            AH_Window.GetBuildInfoManager();

            var alreadyExist = m_window != null;
            if (!alreadyExist)
            {
                m_window = GetWindow<AH_BuildReportWindow>("AH Report", true, typeof(AH_Window));
                m_window.titleContent.image = AH_EditorData.Instance.ReportIcon.Icon;

                m_window.buildInfoManager = AH_Window.GetBuildInfoManager();
                m_window.buildInfoManager.OnBuildInfoSelectionChanged += m_window.OnBuildInfoSelectionChanged;
                m_window.populateBuildReportWindowData();
            }
        }

        private void OnBuildInfoSelectionChanged()
        {
            populateBuildReportWindowData();
        }

        private void populateBuildReportWindowData()
        {
            if (buildInfoManager.HasSelection)
            {
                data = new AH_BuildReportWindowData(buildInfoManager.GetSerializedBuildInfo());
                data.SetRelativeValuesForFiles();
            }
        }

        [Serializable]
        private class AH_BuildReportWindowData
        {
            [SerializeField] private ulong buildSize;
            [SerializeField] private string buildTarget;
            [SerializeField] private string buildDate;
            [SerializeField] private List<AH_BuildReportWindowRoleInfo> roleInfoList;

            public AH_BuildReportWindowData(AH_SerializedBuildInfo buildInfo)
            {
                buildSize = buildInfo.TotalSize;
                buildTarget = buildInfo.buildTargetInfo;
                buildDate = buildInfo.dateTime;

                roleInfoList = new List<AH_BuildReportWindowRoleInfo>();
                foreach (var item in buildInfo.BuildReportInfoList)
                    //Check if role exists already
                    if (roleInfoList.Exists(val => val.roleName.Equals(item.Role)))
                        roleInfoList.First(val => val.roleName.Equals(item.Role)).AddToRoleInfo(item);
                    //If not, add new roleentry
                    else
                        roleInfoList.Add(new AH_BuildReportWindowRoleInfo(item));

                //Sort roles
                IEnumerable<AH_BuildReportWindowRoleInfo> tmp =
                    roleInfoList.OrderByDescending(val => val.combinedRoleSize);
                roleInfoList = tmp.ToList();

                //Sort elements in roles
                foreach (var item in roleInfoList) item.Order();
            }

            internal void OnGUI()
            {
                if (buildSize <= 0)
                {
                    Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon,
                        462f, 120f,
                        "The selected buildinfo lacks information. It was probably created with older version. Create new with this version");
                    return;
                }

                var guiWidth = 260;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Combined Build Size:", Heureka_EditorData.Instance.HeadlineStyle,
                    GUILayout.Width(guiWidth));
                EditorGUILayout.LabelField(AH_Utils.GetSizeAsString(buildSize),
                    Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Build Target:", Heureka_EditorData.Instance.HeadlineStyle,
                    GUILayout.Width(guiWidth));
                EditorGUILayout.LabelField(buildTarget, Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" Build Time:", Heureka_EditorData.Instance.HeadlineStyle,
                    GUILayout.Width(guiWidth));
                var parsedDate = DateTime
                    .ParseExact(buildDate, AH_SerializationHelper.DateTimeFormat, CultureInfo.CurrentCulture)
                    .ToString();
                EditorGUILayout.LabelField(parsedDate, Heureka_EditorData.Instance.HeadlineStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                foreach (var item in roleInfoList)
                {
                    item.OnGUI();
                    EditorGUILayout.Space();
                }
            }

            internal void SetRelativeValuesForFiles()
            {
                //Find the relative value of all items so we can show which files are taking up the most space
                //A way to keep track of the sorted values
                var tmpList = new List<AH_BuildReportWindowFileInfo>();
                foreach (var infoList in roleInfoList)
                foreach (var fileInfo in infoList.fileInfoList)
                    tmpList.Add(fileInfo);

                var sortedFileInfo = tmpList.OrderByDescending(val => val.size).ToList();
                for (var i = 0; i < sortedFileInfo.Count; i++)
                {
                    var groupSize = ColorDotIconList.Count;
                    //Figure out which icon to show (create 4 groups from sortedlist)
                    var groupIndex = Mathf.FloorToInt(i / (float)sortedFileInfo.Count * groupSize);
                    sortedFileInfo[i].SetFileSizeGroup(groupIndex);
                }
            }
        }

        [Serializable]
        internal class AH_BuildReportWindowRoleInfo
        {
            [SerializeField] internal ulong combinedRoleSize;
            [SerializeField] internal string roleName;
            [SerializeField] internal List<AH_BuildReportWindowFileInfo> fileInfoList;

            public AH_BuildReportWindowRoleInfo(AH_BuildReportFileInfo item)
            {
                roleName = item.Role;
                fileInfoList = new List<AH_BuildReportWindowFileInfo>();
                addFile(item);
            }

            internal void AddToRoleInfo(AH_BuildReportFileInfo item)
            {
                combinedRoleSize += item.Size;
                addFile(item);
            }

            private void addFile(AH_BuildReportFileInfo item)
            {
                combinedRoleSize += item.Size;
                fileInfoList.Add(new AH_BuildReportWindowFileInfo(item));
            }

            internal void OnGUI()
            {
                EditorGUILayout.HelpBox(roleName + " combined: " + AH_Utils.GetSizeAsString(combinedRoleSize),
                    MessageType.Info);
                foreach (var item in fileInfoList) item.OnGUI();
            }

            internal void Order()
            {
                IEnumerable<AH_BuildReportWindowFileInfo> tmp = fileInfoList.OrderByDescending(val => val.size);
                fileInfoList = tmp.ToList();
            }
        }

        [Serializable]
        internal class AH_BuildReportWindowFileInfo
        {
            [SerializeField] internal string fileName;
            [SerializeField] internal string path;
            [SerializeField] internal ulong size;
            [SerializeField] internal string sizeString;
            [SerializeField] private GUIContent content = new();
            [SerializeField] private int fileSizeGroup;

            public AH_BuildReportWindowFileInfo(AH_BuildReportFileInfo item)
            {
                path = item.Path;
                fileName = Path.GetFileName(path);
                size = item.Size;
                sizeString = AH_Utils.GetSizeAsString(size);

                content.text = fileName;
                content.tooltip = path;
            }

            internal void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(content, GUILayout.MinWidth(300));
                EditorGUILayout.LabelField(sizeString, GUILayout.MaxWidth(80));
                GUILayout.Label(EditorGUIUtility.IconContent(ColorDotIconList[fileSizeGroup]), GUILayout.MaxHeight(16));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            internal void SetFileSizeGroup(int groupIndex)
            {
                fileSizeGroup = groupIndex;
            }
        }
    }
}
#endif