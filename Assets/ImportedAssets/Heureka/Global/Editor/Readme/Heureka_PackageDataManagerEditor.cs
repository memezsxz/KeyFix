using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames
{
    [CustomEditor(typeof(Heureka_PackageDataManager))]
    [InitializeOnLoad]
    public class Heureka_PackageDataManagerEditor : Editor
    {
        public static readonly string ShowedReadmeProjectStateName = "HeurekaGames.PackageDataManager.ShowedReadme";

        private static readonly float kSpace = 16f;
        [SerializeField] private GUIStyle m_LinkStyle;
        [SerializeField] private GUIStyle m_TitleStyle;
        [SerializeField] private GUIStyle m_HeadingStyle;
        [SerializeField] private GUIStyle m_BodyStyle;

        private bool m_Initialized;

        private Heureka_PackageDataManager readmeManager;

        static Heureka_PackageDataManagerEditor()
        {
            EditorApplication.delayCall += SelectReadmeAutomatically;
        }

        private GUIStyle LinkStyle => m_LinkStyle;

        private GUIStyle TitleStyle => m_TitleStyle;

        private GUIStyle HeadingStyle => m_HeadingStyle;

        private GUIStyle BodyStyle => m_BodyStyle;

        private void OnEnable()
        {
            readmeManager = SelectReadme(); // (Heureka_PackageDataManager)target;
            //populate sections
            var guids = AssetDatabase.FindAssets($"t:{nameof(Heureka_PackageData)}");
            var tmpList = new List<Heureka_PackageData>();
            foreach (var item in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                tmpList.Add(AssetDatabase.LoadAssetAtPath<Heureka_PackageData>(path));
            }

            readmeManager.sections = tmpList.ToArray();
            //readmeManager.sections = Resources.FindObjectsOfTypeAll<Heureka_PackageData>();

            //Sorted lidt by show Priority
            readmeManager.sections = readmeManager.sections.OrderByDescending(val => val.PackageShowPrio).ToArray();

            var listUniqueEntries = new List<Heureka_PackageData>();
            foreach (var item in readmeManager.sections)
                //If we dont have this asset identifier in list already
                if (!listUniqueEntries.Any(val => val.AssetIdentifier == item.AssetIdentifier))
                {
                    listUniqueEntries.Add(item);
                }
                //If it IS in list already find the one that is NOT a promo, and put that in list
                else
                {
                    //If the one we look at right now is a promo, just ignore
                    if (item.GetType() == typeof(Heureka_PackageDataPromo))
                        continue;
                    //Remove the promo from list and insert the new one with similar identifier (Which should be a readme)
                    listUniqueEntries.Remove(listUniqueEntries.Find(val =>
                        val.AssetIdentifier == item.AssetIdentifier));
                    listUniqueEntries.Add(item);
                }

            readmeManager.sections = listUniqueEntries.ToArray();
        }

        private static void SelectReadmeAutomatically()
        {
            if (!EditorPrefs.GetBool(getUniqueReadMeStatePrefKey(), false))
            {
                SelectReadme();
                EditorPrefs.SetBool(getUniqueReadMeStatePrefKey(), true);
            }
        }

        private static string getUniqueReadMeStatePrefKey()
        {
            return ShowedReadmeProjectStateName + "." + Application.dataPath;
        }

        [MenuItem("Window/Heureka/Readme", priority = 10)]
        public static Heureka_PackageDataManager SelectReadme()
        {
            var ids = AssetDatabase.FindAssets("t:" + typeof(Heureka_PackageDataManager));

            if (ids.Length == 1)
            {
                var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));

                Selection.objects = new[] { readmeObject };
                return (Heureka_PackageDataManager)readmeObject;
            }

            Debug.Log("Couldn't find a readme");
            return null;
        }

        protected override void OnHeaderGUI()
        {
            Init();

            //Make sure we have the proper readme's
            SelectReadme();

            var iconWidth = 96; // Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical();
                GUILayout.Space(10);
                GUILayout.Label(readmeManager.icon, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Label(readmeManager.title, TitleStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                foreach (var link in readmeManager.Links.Where(val => val.ActiveLink))
                    if (LinkLabel(new GUIContent(link.Name)))
                        Application.OpenURL(link.Link);

                EditorGUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(20);
            Init();

            foreach (var section in readmeManager.sections)
            {
                drawSeparator();

                if (!string.IsNullOrEmpty(section.AssetName))
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Box(section.Icon, GUIStyle.none, GUILayout.Width(64), GUILayout.Height(64));
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label(section.AssetName, TitleStyle);
                    GUILayout.Label(section.Subheader, HeadingStyle);
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(kSpace);
                }

                if (!string.IsNullOrEmpty(section.Description)) GUILayout.Label(section.Description, BodyStyle);
                if (section.Links != null)
                    foreach (var link in section.Links.Where(val => val.ActiveLink))
                        if (LinkLabel(new GUIContent(link.Name)))
                            Application.OpenURL(link.Link);

                GUILayout.Space(10);

                //If this is a versioned data package
                if (section.GetType() == typeof(Heureka_PackageDataVersioned))
                {
                    var versionedSection = (Heureka_PackageDataVersioned)section;
                    if (versionedSection.VersionData != null && versionedSection.VersionData.Count > 0)
                    {
                        versionedSection.FoldOutVersionHistory =
                            EditorGUILayout.Foldout(versionedSection.FoldOutVersionHistory, "Version History");
                        if (versionedSection.FoldOutVersionHistory)
                        {
                            for (var i = versionedSection.VersionData.Count() - 1; i >= 0; i--)
                            {
                                EditorGUI.indentLevel++;
                                {
                                    GUILayout.Label(versionedSection.VersionData[i].VersionNum.GetVersionString(),
                                        HeadingStyle);

                                    EditorGUI.indentLevel += 2;
                                    foreach (var versionChange in versionedSection.VersionData[i].VersionChanges)
                                        EditorGUILayout.LabelField("- " + versionChange, BodyStyle);
                                    EditorGUI.indentLevel -= 2;
                                }
                                EditorGUI.indentLevel--;
                                GUILayout.Space(4);
                            }

                            GUILayout.Space(kSpace);
                        }
                    }
                }
            }
        }

        private void drawSeparator()
        {
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void Init()
        {
            if (m_Initialized)
                return;

            m_BodyStyle = new GUIStyle(EditorStyles.label);
            m_BodyStyle.wordWrap = true;
            m_BodyStyle.fontSize = 12;

            m_TitleStyle = new GUIStyle(m_BodyStyle);
            m_TitleStyle.wordWrap = false;
            m_TitleStyle.fontSize = 18;

            m_HeadingStyle = new GUIStyle(m_BodyStyle);
            m_HeadingStyle.wordWrap = false;
            m_HeadingStyle.fontSize = 14;

            m_LinkStyle = new GUIStyle(m_BodyStyle);
            m_LinkStyle.wordWrap = false;
            // Match selection color which works nicely for both light and dark skins
            m_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            m_LinkStyle.stretchWidth = false;

            m_Initialized = true;
        }

        private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }
    }
}