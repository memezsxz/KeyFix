using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.AssetTreeView
{
    [Serializable]
    public class AH_TreeviewElement : TreeElement, ISerializationCallbackReceiver
    {
        public AH_TreeviewElement(string absPath, int depth, int id, string relativepath, string assetID,
            List<string> scenesReferencing, bool isUsedInBuild) : base(Path.GetFileName(absPath), depth, id)
        {
            this.absPath = absPath;
            var assetPath = relativepath;
            guid = AssetDatabase.AssetPathToGUID(assetPath);
            scenesReferencingAsset = scenesReferencing;
            usedInBuild = isUsedInBuild;

            //Return if its a folder
            if (isFolder = AssetDatabase.IsValidFolder(assetPath))
                return;

            //Return if its not an asset
            if (!string.IsNullOrEmpty(guid))
            {
                AssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                updateIconDictEntry();
            }
        }

        internal long GetFileSizeRecursively(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (combinedAssetSizeInFolder == null)
                combinedAssetSizeInFolder = new Dictionary<AH_MultiColumnHeader.AssetShowMode, long>();

            if (combinedAssetSizeInFolder.ContainsKey(showMode))
                return combinedAssetSizeInFolder[showMode];

            //TODO store these values instead of calculating each and every time?

            long combinedChildrenSize = 0;
            //Combine the size of all the children
            if (hasChildren)
                foreach (AH_TreeviewElement item in children)
                {
                    var validAsset = showMode == AH_MultiColumnHeader.AssetShowMode.All ||
                                     (showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !item.usedInBuild) ||
                                     (showMode == AH_MultiColumnHeader.AssetShowMode.Used && item.usedInBuild);

                    //Loop thropugh folders and assets thats used not in build
                    if (validAsset || item.isFolder)
                        combinedChildrenSize += item.GetFileSizeRecursively(showMode);
                }

            combinedChildrenSize += FileSize;

            //Cache the value
            combinedAssetSizeInFolder.Add(showMode, combinedChildrenSize);

            return combinedChildrenSize;
        }

        internal bool AssetMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            //Test if we want to add this element (We dont want to show "used" when window searches for "unused"
            return AssetType != null && (showMode == AH_MultiColumnHeader.AssetShowMode.All ||
                                         (showMode == AH_MultiColumnHeader.AssetShowMode.Used && usedInBuild) ||
                                         (showMode == AH_MultiColumnHeader.AssetShowMode.Unused && !usedInBuild));
        }

        internal bool HasChildrenThatMatchesState(AH_MultiColumnHeader.AssetShowMode showMode)
        {
            if (!hasChildren)
                return false;

            //Check if a valid child exit somewhere in this branch
            foreach (AH_TreeviewElement child in children)
            {
                if (child.AssetMatchesState(showMode))
                    return true;
                if (child.HasChildrenThatMatchesState(showMode))
                    return true;
            }

            return false;
        }

        internal List<string> GetUnusedPathsRecursively()
        {
            var unusedAssetsInFolder = new List<string>();

            //Combine the size of all the children
            if (hasChildren)
                foreach (AH_TreeviewElement item in children)
                    if (item.isFolder)
                        unusedAssetsInFolder.AddRange(item.GetUnusedPathsRecursively());
                    //Loop thropugh folders and assets thats used not in build
                    else if (!item.usedInBuild)
                        unusedAssetsInFolder.Add(item.RelativePath);
            return unusedAssetsInFolder;
        }

        internal static List<string> GetStoredIconTypes()
        {
            var iconTypesSerialized = new List<string>();
            foreach (var item in iconDictionary) iconTypesSerialized.Add(Heureka_Serializer.SerializeType(item.Key));
            return iconTypesSerialized;
        }

        internal static List<Texture> GetStoredIconTextures()
        {
            var iconTexturesSerialized = new List<Texture>();
            foreach (var item in iconDictionary) iconTexturesSerialized.Add(item.Value);
            return iconTexturesSerialized;
        }

        private void updateIconDictEntry()
        {
            if (AssetType != null && !iconDictionary.ContainsKey(AssetType))
                iconDictionary.Add(AssetType, EditorGUIUtility.ObjectContent(null, AssetType).image);
        }

        internal static void UpdateIconDictAfterSerialization(List<string> serializationHelperListIconTypes,
            List<Texture> serializationHelperListIconTextures)
        {
            iconDictionary = new Dictionary<Type, Texture>();
            for (var i = 0; i < serializationHelperListIconTypes.Count; i++)
            {
                var deserializedType = Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]);
                if (deserializedType != null)
                    iconDictionary.Add(Heureka_Serializer.DeSerializeType(serializationHelperListIconTypes[i]),
                        serializationHelperListIconTextures[i]);
            }
        }

        internal static Texture GetIcon(Type assetType)
        {
            return iconDictionary[assetType];
        }

        #region Fields

        [SerializeField] private string absPath;

        //[SerializeField]
        private string relativePath;

        [SerializeField] private string guid;

        //[SerializeField]

        [SerializeField] private string assetTypeSerialized;

        private long assetSize;

        //private string assestSizeStringRepresentation;
        //[SerializeField]
        private long fileSize;

        //[SerializeField]
        //private string fileSizeStringRepresentation;
        [SerializeField] private List<string> scenesReferencingAsset;

        [SerializeField] private bool usedInBuild;

        [SerializeField] private bool isFolder;

        private Dictionary<AH_MultiColumnHeader.AssetShowMode, long> combinedAssetSizeInFolder = new();

        //Dictionary of asset types and their icons (Cant be serialized)
        private static Dictionary<Type, Texture> iconDictionary = new();

        #endregion

        #region Properties

        public string RelativePath
        {
            get
            {
                if (!string.IsNullOrEmpty(relativePath))
                    return relativePath;
                return relativePath = AssetDatabase.GUIDToAssetPath(GUID);
            }
        }

        public string GUID => guid;

        public Type AssetType { get; private set; }

        //TODO, make this threaded
        public string AssetTypeSerialized
        {
            get
            {
                if (string.IsNullOrEmpty(assetTypeSerialized) && AssetType != null)
                    assetTypeSerialized = Heureka_Serializer.SerializeType(AssetType);
                return assetTypeSerialized;
            }
        }

        public long AssetSize
        {
            get
            {
                if (UsedInBuild && assetSize == 0)
                {
                    var asset = AssetDatabase.LoadMainAssetAtPath(RelativePath);
                    //#if UNITY_2017_1_OR_NEWER
                    if (asset != null)
                        return assetSize = Profiler.GetRuntimeMemorySizeLong(asset) / 2;
                    return -1;
                }

                return assetSize;
            }
        }

        public string AssetSizeStringRepresentation => AH_Utils.GetSizeAsString(AssetSize);

        //TODO, make this threaded
        public long FileSize
        {
            get
            {
                if (fileSize != 0)
                    return fileSize;
                var fileInfo = new FileInfo(absPath);
                if (fileInfo.Exists)
                    return fileSize = fileInfo != null ? fileInfo.Length : 0;
                return -1;
            }
        }

        public string FileSizeStringRepresentation => AH_Utils.GetSizeAsString(fileSize);

        public List<string> ScenesReferencingAsset => scenesReferencingAsset;

        public int SceneRefCount => scenesReferencingAsset != null ? scenesReferencingAsset.Count : 0;

        public bool UsedInBuild => usedInBuild;

        public bool IsFolder => isFolder;

        #endregion

        #region Serialization callbacks

        //TODO Maybe we can store type infos in BuildInfoTreeView instead of on each individual element, might be performance heavy

        //Store serializable string so we can retrieve type after serialization
        public void OnBeforeSerialize()
        {
            if (AssetType != null)
                assetTypeSerialized = Heureka_Serializer.SerializeType(AssetType);
        }

        //Set type from serialized property
        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(AssetTypeSerialized))
                AssetType = Heureka_Serializer.DeSerializeType(AssetTypeSerialized);
            //assetTypeSerialized = "";
        }

        #endregion
    }
}