using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HeurekaGames.AssetHunterPRO
{
    public class AH_ignoredListEventActionBase
    {
        [SerializeField] private readonly Action<int> buttonDownCallBack;

        [SerializeField] private readonly int ignoredListIndex;

        public AH_ignoredListEventActionBase(int ignoredListIndex, Action<int> buttonDownCallBack)
        {
            this.ignoredListIndex = ignoredListIndex;
            this.buttonDownCallBack = buttonDownCallBack;
        }

        public virtual string Header { get; protected set; }
        public virtual string FoldOutContent { get; protected set; }
        public event EventHandler<IgnoreListEventArgs> IgnoredAddedEvent;

        protected void getObjectInfo(out string path, out bool isMain, out bool isFolder)
        {
            path = AssetDatabase.GetAssetPath(Selection.activeObject);
            isMain = AssetDatabase.IsMainAsset(Selection.activeObject);
            isFolder = AssetDatabase.IsValidFolder(path);
        }

        public void IgnoreCallback(Object obj, string identifier)
        {
            IgnoredAddedEvent(obj, new IgnoreListEventArgs(identifier));

            //Notify the list was changed
            buttonDownCallBack(ignoredListIndex);
        }

        /// <summary>
        ///     draw the Ignore button
        /// </summary>
        /// <param name="buttonText">What should the button read</param>
        /// <param name="ignoredList">The current list of Ignores this is supposed to be appended to</param>
        /// <param name="identifier">The unique identifier of the Ignore</param>
        /// <param name="optionalLegibleIdentifier">Humanly legible identifier</param>
        protected void drawButton(bool validSelected, string buttonText, AH_IgnoreList ignoredList, string identifier,
            string optionalLegibleIdentifier = "")
        {
            var btnUsable = validSelected && !ignoredList.ExistsInList(identifier);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!btnUsable);
            if (GUILayout.Button(buttonText, GUILayout.Width(150)) && btnUsable)
                IgnoreCallback(Selection.activeObject, identifier);
            EditorGUI.EndDisabledGroup();

            //Select the proper string to write on label
            var label = !btnUsable ? validSelected ? "Already Ignored" : "Invalid selection"
                : string.IsNullOrEmpty(optionalLegibleIdentifier) ? identifier : optionalLegibleIdentifier;

            var content = new GUIContent(label,
                EditorGUIUtility
                    .IconContent(!btnUsable ? validSelected ? "d_lightOff" : "d_orangeLight" : "d_greenLight").image);

            GUILayout.Label(content, GUILayout.MaxHeight(16));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        protected bool hasValidSelectionObject()
        {
            return Selection.activeObject != null && Selection.objects.Length == 1;
        }

        public virtual string GetFormattedItem(string identifier)
        {
            return identifier;
        }

        public virtual string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItem(identifier);
        }

        public virtual string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItem(identifier);
        }
    }

    public class IgnoredEventActionExtension : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionExtension(int ignoredListIndex, Action<int> buttonDownCallBack) : base(
            ignoredListIndex, buttonDownCallBack)
        {
            Header = "File extensions ignored from search";
            FoldOutContent = "See ignored file extensions";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            string element;
            pathContainsValidElement(path, out element);

            return ignoredList.Contains(element.ToLower());
        }

        //Check if the currectly selected asset if excludable as file extension
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            //if (isMain)
            {
                string extension;
                var validElement = pathContainsValidElement(path, out extension);

                drawButton(isMain && validElement, "Ignore file extension", ignoredList, extension);
            }
        }

        private bool pathContainsValidElement(string path, out string extension)
        {
            extension = "";
            var hasExtension = Path.HasExtension(path);
            if (hasExtension)
                extension = Path.GetExtension(path).ToLower();

            return hasExtension;
        }
    }

    public class IgnoredEventActionPathEndsWith : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionPathEndsWith(int ignoredListIndex, Action<int> buttonDownCallBack) : base(
            ignoredListIndex, buttonDownCallBack)
        {
            Header = "Folder paths with the following ending are ignored";
            FoldOutContent = "See ignored folder endings";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            return ignoredList.Contains(getIdentifier(path));
        }

        //Check if the currectly selected asset if excludable as path ending
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            drawButton(isMain && isFolder, "Ignore folder ending", ignoredList, getIdentifier(path));
        }

        private string getIdentifier(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            var fullPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar);
            return Path.DirectorySeparatorChar + Path.GetFileName(fullPath).ToLower();
        }
    }

    public class IgnoredEventActionType : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionType(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex,
            buttonDownCallBack)
        {
            Header = "Asset types ignored";
            FoldOutContent = "See ignored types";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            Type assetType;
            return ignoredList.Contains(getIdentifier(path, out assetType));
        }

        //Check if the currectly selected asset if excludable as type
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            Type assetType;
            drawButton(isMain && !isFolder, "Ignore Type", ignoredList, getIdentifier(path, out assetType),
                assetType != null ? assetType.ToString() : "");
        }

        private string getIdentifier(string path, out Type assetType)
        {
            assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
            if (assetType != null)
                return Heureka_Serializer.SerializeType(assetType);
            return "";
        }
    }

    public class IgnoredEventActionFile : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionFile(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex,
            buttonDownCallBack)
        {
            Header = "Specific files ignored";
            FoldOutContent = "See ignored files";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
                return ignoredList.Contains(assetId);
            return ignoredList.Contains(getIdentifier(path));
        }

        //Check if the currectly selected asset if excludable as file
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            var assetGUID = getIdentifier(path);
            drawButton(isMain && !isFolder, "Ignore file", ignoredList, assetGUID,
                GetFormattedItemShort(assetGUID, 45));
        }

        public override string GetFormattedItem(string identifier)
        {
            return AssetDatabase.GUIDToAssetPath(identifier);
        }

        public override string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItemShort(identifier, 50);
        }

        public override string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItemShort(identifier, 60);
        }

        private string getIdentifier(string path)
        {
            return AssetDatabase.AssetPathToGUID(path);
        }

        public string GetFormattedItemShort(string identifier, int charCount)
        {
            return AH_Utils.ShrinkPathEnd(GetFormattedItem(identifier), charCount);
        }
    }

    public class IgnoredEventActionFolder : AH_ignoredListEventActionBase, AH_IIgnoreListActions
    {
        public IgnoredEventActionFolder(int ignoredListIndex, Action<int> buttonDownCallBack) : base(ignoredListIndex,
            buttonDownCallBack)
        {
            Header = "Specific folders ignored";
            FoldOutContent = "See ignored folders";
        }

        public bool ContainsElement(List<string> ignoredList, string path, string assetId)
        {
            if (!string.IsNullOrEmpty(assetId))
                return ignoredList.Contains(assetId);
            return ignoredList.Contains(getIdentifier(path));
        }

        //Check if the currectly selected asset if excludable as folder
        public void DrawIgnored(AH_IgnoreList ignoredList)
        {
            if (!hasValidSelectionObject())
                return;

            string path;
            bool isMain;
            bool isFolder;
            getObjectInfo(out path, out isMain, out isFolder);

            var assetGUID = getIdentifier(path);
            drawButton(isMain && isFolder, "Ignore folder", ignoredList, assetGUID,
                GetFormattedItemShort(assetGUID, 40));
        }

        public override string GetFormattedItemShort(string identifier)
        {
            return GetFormattedItemShort(identifier, 50);
        }

        public override string GetFormattedItem(string identifier)
        {
            return AssetDatabase.GUIDToAssetPath(identifier);
        }

        public override string GetLabelFormattedItem(string identifier)
        {
            return GetFormattedItemShort(identifier, 60);
        }

        private string getIdentifier(string path)
        {
            return AssetDatabase.AssetPathToGUID(path);
        }

        private string GetFormattedItemShort(string identifier, int charCount)
        {
            return AH_Utils.ShrinkPathEnd(GetFormattedItem(identifier), charCount);
        }
    }
}