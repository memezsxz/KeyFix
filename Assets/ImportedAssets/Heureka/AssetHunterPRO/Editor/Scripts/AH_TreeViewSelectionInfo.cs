﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_TreeViewSelectionInfo
    {
        public delegate void AssetDeletedHandler();

        public const float Height = 64;

        private AH_MultiColumnHeader multiColumnHeader;
        private List<AH_TreeviewElement> selection;

        public bool HasSelection { get; private set; }

        public static event AssetDeletedHandler OnAssetDeleted;

        internal void Reset()
        {
            selection = null;
            HasSelection = false;
        }

        internal void SetSelection(AH_TreeViewWithTreeModel treeview, IList<int> selectedIds)
        {
            multiColumnHeader = (AH_MultiColumnHeader)treeview.multiColumnHeader;
            selection = new List<AH_TreeviewElement>();

            foreach (var itemID in selectedIds) selection.Add(treeview.treeModel.Find(itemID));

            HasSelection = selection.Count > 0;

            //If we have more, select the assets in project view
            if (HasSelection)
            {
                if (selection.Count > 1)
                {
                    var selectedObjects = new Object[selection.Count];
                    for (var i = 0; i < selection.Count; i++)
                        selectedObjects[i] = AssetDatabase.LoadMainAssetAtPath(selection[i].RelativePath);
                    Selection.objects = selectedObjects;
                }
                else
                {
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(selection[0].RelativePath);
                }

                AH_Utils.PingObjectAtPath(selection[selection.Count - 1].RelativePath, false);
            }
        }

        internal void OnGUISelectionInfo(Rect selectionRect)
        {
            GUILayout.BeginArea(selectionRect);
            //TODO MAKE SURE WE DONT DO ALL OF THIS EACH FRAME, BUT CACHE THE SELECTION DATA

            using (new EditorGUILayout.HorizontalScope())
            {
                if (selection.Count == 1)
                    drawSingle();
                else
                    drawMulti();
            }

            GUILayout.EndArea();
        }

        private void drawSingle()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            drawAssetPreview(true);
            EditorGUILayout.EndVertical();

            //Draw info from single asset
            EditorGUILayout.BeginVertical();

            GUILayout.Label(selection[0].RelativePath);
            if (!selection[0].IsFolder) GUILayout.Label("(" + selection[0].AssetType + ")");

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            if (selection[0].IsFolder)
                DrawDeleteFolderButton(selection[0]);
            else
                drawDeleteAssetsButton();

            EditorGUILayout.EndHorizontal();
        }

        private void drawMulti()
        {
            //Make sure we have not selected folders
            var allFolders = selection.All(val => val.IsFolder);
            var allFiles = !selection.Any(val => val.IsFolder);
            var allSameType = selection.All(var => var.AssetType == selection[0].AssetType);

            var containsNested = false;
            foreach (var item in selection)
            {
                if (!item.IsFolder)
                    continue;

                foreach (var other in selection)
                {
                    if (other == item)
                        continue;

                    if (!other.RelativePath.StartsWith(item.RelativePath))
                        continue;

                    var dirInfo = new DirectoryInfo(item.RelativePath);

                    if (other.IsFolder)
                    {
                        var otherDir = new DirectoryInfo(other.RelativePath);

                        if (!dirInfo.GetDirectories(otherDir.Name, SearchOption.AllDirectories)
                                .Any(x => x.FullName == otherDir.FullName))
                            continue;

                        /*if (dirInfo.Parent.FullName == otherDir.Parent.FullName)
                            continue;*/
                    }
                    else
                    {
                        var fi = new FileInfo(other.RelativePath);

                        if (!dirInfo.GetFiles(fi.Name, SearchOption.AllDirectories).Any(x => x.FullName == fi.FullName))
                            continue;
                    }

                    containsNested = true;
                }

                if (containsNested)
                    break;
            }

            drawAssetPreview(allSameType);

            EditorGUILayout.BeginHorizontal();
            //Draw info from multiple
            EditorGUILayout.BeginVertical();

            //Identical files
            if (allSameType && allFiles)
                GUILayout.Label(selection[0].AssetType + " (" + selection.Count() + ")");
            //all folders
            else if (allSameType)
                GUILayout.Label("Folders (" + selection.Count() + ")");
            //Non identical selection
            else
                GUILayout.Label("Items (" + selection.Count() + ")");

            EditorGUILayout.EndVertical();

            if (!containsNested)
            {
                drawDeleteAssetsButton();
            }
            else
            {
                GUILayout.FlexibleSpace();
                var s = new GUIStyle(EditorStyles.textField);
                s.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Nested selection is not allowed", s);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void drawDeleteAssetsButton()
        {
            if (multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused)
                return;

            long combinedSize = 0;
            foreach (var item in selection)
                if (item.IsFolder)
                    combinedSize += item.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused);
                else
                    combinedSize += item.FileSize;
            if (GUILayout.Button("Delete " + AH_Utils.GetSizeAsString(combinedSize), GUILayout.Width(160),
                    GUILayout.Height(32)))
                deleteUnusedAssets();
        }

        private void DrawDeleteFolderButton(AH_TreeviewElement folder)
        {
            if (multiColumnHeader.ShowMode != AH_MultiColumnHeader.AssetShowMode.Unused)
                return;

            var description = "Delete unused assets from folder";
            var content =
                new GUIContent(
                    "Delete " + AH_Utils.GetSizeAsString(
                        folder.GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode.Unused)), description);
            var style = new GUIStyle(GUI.skin.button);
            DrawDeleteFolderButton(content, folder, style, description,
                "Do you want to delete all unused assets from:" + Environment.NewLine + folder.RelativePath,
                GUILayout.Width(160), GUILayout.Height(32));
        }

        public void DrawDeleteFolderButton(GUIContent content, AH_TreeviewElement folder, GUIStyle style,
            string dialogHeader, string dialogDescription, params GUILayoutOption[] layout)
        {
            if (GUILayout.Button(content, style, layout))
                deleteUnusedFromFolder(dialogHeader, dialogDescription, folder);
        }

        private void drawAssetPreview(bool bDraw)
        {
            var content = new GUIContent();

            //Draw asset preview
            if (bDraw && !selection[0].IsFolder)
            {
                var preview =
                    AssetPreview.GetAssetPreview(AssetDatabase.LoadMainAssetAtPath(selection[0].RelativePath));
                content = new GUIContent(preview);
            }
            //Draw Folder icon
            else if (bDraw)
            {
                content = EditorGUIUtility.IconContent("Folder Icon");
            }

            GUILayout.Label(content, GUILayout.Width(Height), GUILayout.Height(Height));
        }

        private void deleteUnusedAssets()
        {
            var choice = EditorUtility.DisplayDialogComplex("Delete unused assets",
                "Do you want to delete the selected assets", "Yes", "Cancel", "Backup (Very slow)");
            var affectedAssets = new List<string>();


            if (choice == 0) //Delete
            {
                foreach (var item in selection)
                    if (item.IsFolder)
                        affectedAssets.AddRange(item.GetUnusedPathsRecursively());
                    else
                        affectedAssets.Add(item.RelativePath);
                deleteMultipleAssets(affectedAssets);
            }
            else if (choice == 2) //Backup
            {
                foreach (var item in selection)
                    if (item.IsFolder)
                        affectedAssets.AddRange(item.GetUnusedPathsRecursively());
                    else
                        affectedAssets.Add(item.RelativePath);
                exportAssetsToPackage("Backup as unitypackage", affectedAssets);
            }
        }

        private void deleteUnusedFromFolder(string header, string description, AH_TreeviewElement folder)
        {
            var choice = EditorUtility.DisplayDialogComplex(header, description, "Yes", "Cancel", "Backup (Very slow)");

            var affectedAssets = new List<string>();
            if (choice != 1) //Not Cancel
                //Collect affected assets
                affectedAssets = folder.GetUnusedPathsRecursively();
            if (choice == 0) //Delete
                deleteMultipleAssets(affectedAssets);
            else if (choice == 2) //Backup
                exportAssetsToPackage("Backup as unitypackage", affectedAssets);
        }

        private void exportAssetsToPackage(string header, List<string> affectedAssets)
        {
            var filename = Environment.UserName + "_Backup_" + "_" + AH_SerializationHelper.GetDateString();
            var savePath = EditorUtility.SaveFilePanel(
                header,
                AH_SerializationHelper.GetBackupFolder(),
                filename,
                "unitypackage");

            if (!string.IsNullOrEmpty(savePath))
            {
                EditorUtility.DisplayProgressBar("Backup", "Creating backup of " + affectedAssets.Count() + " assets",
                    0f);
                AssetDatabase.ExportPackage(affectedAssets.ToArray<string>(), savePath, ExportPackageOptions.Recurse);
                EditorUtility.ClearProgressBar();
                EditorUtility.RevealInFinder(savePath);

                deleteMultipleAssets(affectedAssets);
            }
        }

        private void deleteMultipleAssets(List<string> affectedAssets)
        {
#if UNITY_2020_1_OR_NEWER
            EditorUtility.DisplayProgressBar("Deleting unused assets",
                $"Deleting {affectedAssets.Count()} unused assets", .5f);
            var failedPaths = new List<string>();
            AssetDatabase.DeleteAssets(affectedAssets.ToArray(), failedPaths);
            EditorUtility.ClearProgressBar();
#else
            foreach (var asset in affectedAssets)
            {
                EditorUtility.DisplayProgressBar("Deleting unused assets", $"Deleting {affectedAssets.IndexOf(asset)}:{affectedAssets.Count()}", affectedAssets.IndexOf(asset)/ affectedAssets.Count());
                //AssetDatabase.DeleteAsset(asset);
                FileUtil.DeleteFileOrDirectory(asset);
            }
            EditorUtility.ClearProgressBar();
#endif

            AssetDatabase.Refresh();

            if (OnAssetDeleted != null)
                OnAssetDeleted();
        }
    }
}