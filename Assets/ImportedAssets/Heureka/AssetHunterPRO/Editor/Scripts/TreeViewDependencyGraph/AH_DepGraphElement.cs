using System;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph
{
    [Serializable]
    public class AH_DepGraphElement : TreeElement, ISerializationCallbackReceiver
    {
        public AH_DepGraphElement(string name, int depth, int id, string relativepath) : base(name, depth, id)
        {
            relativePath = relativepath;
            var stringSplit = relativepath.Split('/');
            //this.assetName = stringSplit.Last();
            AssetType = AssetDatabase.GetMainAssetTypeAtPath(relativepath);
            if (AssetType != null)
                assetTypeSerialized = Heureka_Serializer.SerializeType(AssetType);
            Icon = EditorGUIUtility.ObjectContent(null, AssetType).image;
        }

        #region Fields

        [SerializeField] private string relativePath;

        /*[SerializeField]
        private string assetName;*/

        [SerializeField] private string assetTypeSerialized;

        #endregion

        #region Properties

        public string RelativePath => relativePath;

        public string AssetName => m_Name;

        public Type AssetType { get; private set; }

        public Texture Icon { get; }

        public string AssetTypeSerialized => assetTypeSerialized;

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
        }

        #endregion
    }
}