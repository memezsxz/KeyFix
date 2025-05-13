using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HeurekaGames
{
    public class Heureka_PackageDataVersioned : Heureka_PackageData
    {
        public List<PackageVersion> VersionData = new();
        internal bool FoldOutVersionHistory;

        private void Item_OnChanged()
        {
            EditorUtility.SetDirty(this);
        }

        public void AddNewVersion(int major, int minor, int patch)
        {
            var newPackageVersion = new PackageVersion(major, minor, patch);
            VersionData.Add(newPackageVersion);
        }

        public void CollapseAll()
        {
            foreach (var item in VersionData) item.FoldOut = false;
        }
    }

    [Serializable]
    public class PackageVersion
    {
        private const float btnWidth = 150;
        [SerializeField] public PackageVersionNum VersionNum;
        [SerializeField] public List<string> VersionChanges = new();

        internal bool FoldOut;

        private bool initialized;
        private PackageVersionNum newVersionNum;
        private ReorderableList reorderableList;

        public PackageVersion(int major, int minor, int patch)
        {
            VersionNum = newVersionNum = new PackageVersionNum(major, minor, patch);
            FoldOut = true;
        }

        private void initialize()
        {
            initialized = true;
            reorderableList = new ReorderableList(VersionChanges, typeof(string), true, true, true, true);

            reorderableList.drawHeaderCallback += DrawHeader;
            reorderableList.drawElementCallback += DrawElement;

            reorderableList.onAddCallback += AddItem;
            reorderableList.onRemoveCallback += RemoveItem;
        }

        /// <summary>
        ///     Draws the header of the list
        /// </summary>
        /// <param name="rect"></param>
        private void DrawHeader(Rect rect)
        {
            GUI.Label(rect, "Version changes");
        }

        /// <summary>
        ///     Draws one element of the list (ListItemExample)
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="index"></param>
        /// <param name="active"></param>
        /// <param name="focused"></param>
        private void DrawElement(Rect rect, int index, bool active, bool focused)
        {
            VersionChanges[index] = EditorGUI.TextField(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height),
                VersionChanges[index]);
        }

        private void AddItem(ReorderableList list)
        {
            VersionChanges.Add("");
        }

        private void RemoveItem(ReorderableList list)
        {
            VersionChanges.RemoveAt(list.index);
        }

        public void OnGUI()
        {
            if (!initialized || reorderableList == null)
                initialize();

            GUILayout.Space(10);
            FoldOut = EditorGUILayout.Foldout(FoldOut, VersionNum.GetVersionString());
            if (FoldOut)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Update Version", GUILayout.Width(btnWidth)))
                    updateVersion();

                //Allow for changing version num
                newVersionNum.Major = EditorGUILayout.IntField(newVersionNum.Major);
                newVersionNum.Minor = EditorGUILayout.IntField(newVersionNum.Minor);
                newVersionNum.Patch = EditorGUILayout.IntField(newVersionNum.Patch);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();

                //versionDescription = GUILayout.TextArea(versionDescription);
                if (reorderableList.count > 0 && GUILayout.Button("Copy to clipboard"))
                {
                    var clipboardString = "";
                    foreach (var item in reorderableList.list) clipboardString += item + Environment.NewLine;
                    EditorGUIUtility.systemCopyBuffer = clipboardString;
                }

                EditorGUILayout.EndHorizontal();
                {
                    reorderableList.DoLayoutList();
                }
            }
        }

        private void updateVersion()
        {
            VersionNum = new PackageVersionNum(newVersionNum.Major, newVersionNum.Minor, newVersionNum.Patch);
            //TODO SORT Parent list
        }
    }

    [Serializable]
    public struct PackageVersionNum : IComparable<PackageVersionNum>
    {
        [SerializeField] public int Major;
        [SerializeField] public int Minor;
        [SerializeField] public int Patch;

        public PackageVersionNum(int major, int minor, int path)
        {
            Major = major;
            Minor = minor;
            Patch = path;
        }

        public int CompareTo(PackageVersionNum other)
        {
            if (Major != other.Major)
                return Major.CompareTo(other.Major);
            if (Minor != other.Minor)
                return Minor.CompareTo(other.Minor);
            return Patch.CompareTo(other.Patch);
        }

        public class VersionComparer : IComparer<PackageVersionNum>
        {
            int IComparer<PackageVersionNum>.Compare(PackageVersionNum objA, PackageVersionNum objB)
            {
                return objA.CompareTo(objB);
            }
        }

        public string GetVersionString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }

        public bool IsEmpty()
        {
            return Major == 0 && Minor == 0 && Patch == 0;
        }
    }
}