using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_SceneReferenceWindow : EditorWindow
    {
        private static AH_SceneReferenceWindow m_window;

        private static readonly string WINDOWNAME = "AH Scenes";

        [SerializeField] private float btnMinWidthSmall = 50;

        private List<string> m_allDisabledScenesInBuildSettings;
        private List<string> m_allEnabledScenesInBuildSettings;
        private List<string> m_allScenesInBuildSettings;

        private List<string> m_allScenesInProject;
        private List<string> m_allUnreferencedScenes;
        private Vector2 scrollPos;

        private void OnGUI()
        {
            if (!m_window)
                Init();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Heureka_WindowStyler.DrawGlobalHeader(Heureka_WindowStyler.clr_Dark, "SCENE REFERENCES");

            //Show all used types
            EditorGUILayout.BeginVertical();

            //Make sure this window has focus to update contents
            Repaint();

            if (m_allEnabledScenesInBuildSettings.Count == 0)
                Heureka_WindowStyler.DrawCenteredMessage(m_window, AH_EditorData.Instance.WindowHeaderIcon.Icon, 310f,
                    110f, "There are no enabled scenes in build settings");

            drawScenes("These scenes are added and enabled in build settings", m_allEnabledScenesInBuildSettings);
            drawScenes("These scenes are added to build settings but disabled", m_allDisabledScenesInBuildSettings);
            drawScenes("These scenes are not referenced anywhere in build settings", m_allUnreferencedScenes);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private void OnFocus()
        {
            GetSceneInfo();
        }

        [MenuItem("Tools/Asset Hunter PRO/Scene overview")]
        [MenuItem("Window/Heureka/Asset Hunter PRO/Scene overview")]
        public static void Init()
        {
            m_window = GetWindow<AH_SceneReferenceWindow>(WINDOWNAME, true, typeof(AH_Window));
            m_window.titleContent.image = AH_EditorData.Instance.SceneIcon.Icon;
            m_window.GetSceneInfo();
        }

        private void GetSceneInfo()
        {
            m_allScenesInProject = AH_Utils.GetAllSceneNames().ToList();
            m_allScenesInBuildSettings = AH_Utils.GetAllSceneNamesInBuild().ToList();
            m_allEnabledScenesInBuildSettings = AH_Utils.GetEnabledSceneNamesInBuild().ToList();
            m_allDisabledScenesInBuildSettings =
                SubtractSceneArrays(m_allScenesInBuildSettings, m_allEnabledScenesInBuildSettings);
            m_allUnreferencedScenes = SubtractSceneArrays(m_allScenesInProject, m_allScenesInBuildSettings);
        }

        //Get the subset of scenes where we subtract "secondary" from "main"
        private List<string> SubtractSceneArrays(List<string> main, List<string> secondary)
        {
            return main.Except(secondary).ToList();
        }

        private void drawScenes(string headerMsg, List<string> scenes)
        {
            if (scenes.Count > 0)
            {
                EditorGUILayout.HelpBox(headerMsg, MessageType.Info);
                foreach (var scenePath in scenes)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Ping", GUILayout.Width(btnMinWidthSmall)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(Object));
                        EditorGUIUtility.PingObject(Selection.activeObject);
                    }

                    EditorGUILayout.LabelField(scenePath);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Separator();
            }
        }
    }
}