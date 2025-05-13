using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_BuildInfoMergerWindow : EditorWindow
    {
        private static AH_BuildInfoMergerWindow m_window;
        private List<BuildInfoSelection> buildInfoFiles;
        private string buildInfoFolder;

        private Vector2 scrollPos;

        private void OnGUI()
        {
            if (!m_window)
                Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Dark, "BUILD INFO MERGER");
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Select a folder that contains buildinfo files", MessageType.Info);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Change", GUILayout.ExpandWidth(false)))
            {
                buildInfoFolder = EditorUtility.OpenFolderPanel("Buildinfo folder", buildInfoFolder, "");
                updateBuildInfoFiles();
            }

            EditorGUILayout.LabelField("Current folder: " + buildInfoFolder);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            //Show all used types
            EditorGUILayout.BeginVertical();

            foreach (var item in buildInfoFiles)
                item.Selected = EditorGUILayout.ToggleLeft(item.BuildInfoFile.Name, item.Selected);

            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(buildInfoFiles.Count(val => val.Selected) < 2);
            if (GUILayout.Button("Merge Selected", GUILayout.ExpandWidth(false)))
            {
                var merged = new AH_SerializedBuildInfo();
                foreach (var item in buildInfoFiles.FindAll(val => val.Selected))
                    merged.MergeWith(item.BuildInfoFile.FullName);
                merged.SaveAfterMerge();

                EditorUtility.DisplayDialog("Merge completed",
                    "A new buildinfo was created by combined existing buildinfos", "Ok");
                //Reset
                buildInfoFiles.ForEach(val => val.Selected = false);
                updateBuildInfoFiles();
            }

            EditorGUI.EndDisabledGroup();
            //Make sure this window has focus to update contents
            Repaint();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Tools/Asset Hunter PRO/Merge tool")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Merge tool")]
        public static void Init()
        {
            m_window = GetWindow<AH_BuildInfoMergerWindow>("AH Merger", true, typeof(AH_Window));
            m_window.titleContent.image = AH_EditorData.Instance.MergerIcon.Icon;

            m_window.buildInfoFolder = AH_SerializationHelper.GetBuildInfoFolder();
            m_window.updateBuildInfoFiles();
        }

        private void updateBuildInfoFiles()
        {
            buildInfoFiles = new List<BuildInfoSelection>();

            var directoryInfo = new DirectoryInfo(buildInfoFolder);
            foreach (var item in directoryInfo.GetFiles("*." + AH_SerializationHelper.BuildInfoExtension)
                         .OrderByDescending(val => val.LastWriteTime)) buildInfoFiles.Add(new BuildInfoSelection(item));
        }

        [Serializable]
        private class BuildInfoSelection
        {
            public bool Selected;
            public FileInfo BuildInfoFile;

            public BuildInfoSelection(FileInfo buildInfoFile)
            {
                BuildInfoFile = buildInfoFile;
            }
        }
    }
}