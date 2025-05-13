using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_DuplicateDataManager : ScriptableSingleton<AH_DuplicateDataManager>, ISerializationCallbackReceiver
    {
        [SerializeField] public bool IsDirty = true;

        public bool RequiresScrollviewRebuild { get; internal set; }
        public bool HasCache { get; private set; }

        public void OnBeforeSerialize()
        {
            _duplicateDictKeys.Clear();
            _duplicateDictValues.Clear();

            foreach (var kvp in Entries)
            {
                _duplicateDictKeys.Add(kvp.Key);
                _duplicateDictValues.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Entries = new Dictionary<string, DuplicateAssetData>();
            for (var i = 0; i != Math.Min(_duplicateDictKeys.Count, _duplicateDictValues.Count); i++)
                Entries.Add(_duplicateDictKeys[i], new DuplicateAssetData(_duplicateDictValues[i].Guids));
        }

        internal bool HasDuplicates()
        {
            return Entries.Count > 0;
        }

        internal void RefreshData()
        {
            //We need to analyze the scrollview to optimize how we draw it           
            RequiresScrollviewRebuild = true;

            Entries = new Dictionary<string, DuplicateAssetData>();
            var hashDict = new Dictionary<string, List<string>>();

            var paths = AssetDatabase.GetAllAssetPaths();
            var pathCount = paths.Length;

            using (var md5 = MD5.Create())
            {
                string assetPathGuid;
                string hash;
                Object LoadedObj;

                var maxReadCount =
                    30; //We dont need to read every line using streamreader. We only need the m_name property, and that comes early in the file
                var lineCounter = 0;

                for (var i = 0; i < pathCount; i++)
                {
                    var path = paths[i];
                    if (AssetDatabase.IsValidFolder(path) ||
                        !path.StartsWith("Assets")) //Slow, could be done recusively
                        continue;

                    if (EditorUtility.DisplayCancelableProgressBar("Finding duplicates", path, i / (float)pathCount))
                    {
                        Entries = new Dictionary<string, DuplicateAssetData>();
                        break;
                    }

                    assetPathGuid = AssetDatabase.AssetPathToGUID(path);
                    LoadedObj = AssetDatabase.LoadMainAssetAtPath(path);
                    var line = "";
                    var foundName = false;

                    if (path.EndsWith("LightingData.asset") || path.Contains("NavMesh"))
                        Debug.Log("LOOK HERE");

                    if (LoadedObj != null)
                        try
                        {
                            using (var stream = File.OpenRead(path))
                            {
                                //We need to loop through certain native types (such as materials) to remove name from metadata - if we dont they wont have same hash
                                if (AssetDatabase.IsNativeAsset(LoadedObj) &&
                                    !LoadedObj.GetType().IsSubclassOf(typeof(ScriptableObject)))
                                {
                                    var appendString = "";
                                    using (var sr = new StreamReader(stream))
                                    {
                                        //bool foundFileName = false;
                                        lineCounter = 0;
                                        while (!sr.EndOfStream)
                                        {
                                            lineCounter++;
                                            if (lineCounter >= maxReadCount)
                                            {
                                                appendString += sr.ReadToEnd();
                                            }
                                            else
                                            {
                                                line = sr.ReadLine();
                                                foundName = line.Contains(LoadedObj.name);

                                                if (!foundName) //we want to ignore the m_name property, since that modifies the hashvalue
                                                    appendString += line;
                                                else
                                                    appendString += sr.ReadToEnd();
                                            }
                                        }
                                    }

                                    hash = BitConverter.ToString(UnicodeEncoding.Unicode.GetBytes(appendString));
                                }
                                else
                                {
                                    hash = BitConverter.ToString(md5.ComputeHash(stream));
                                }

                                if (!hashDict.ContainsKey(hash))
                                    hashDict.Add(hash, new List<string> { assetPathGuid });
                                else
                                    hashDict[hash].Add(assetPathGuid);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                }

                foreach (var pair in hashDict)
                    if (pair.Value.Count > 1)
                        Entries.Add(pair.Key, new DuplicateAssetData(pair.Value));

                IsDirty = false;
                HasCache = true;
                EditorUtility.ClearProgressBar();
            }
        }

        [Serializable]
        public class DuplicateAssetData
        {
            public List<string> Guids;
            private List<string> paths;
            private Texture2D preview;

            public DuplicateAssetData(List<string> guids)
            {
                Guids = guids;
            }

            public Texture2D Preview
            {
                get
                {
                    if (preview != null)
                        return preview;
                    var loadedObj = AssetDatabase.LoadMainAssetAtPath(Paths[0]);
                    return preview = AssetPreview.GetAssetPreview(loadedObj);
                }
            }

            public List<string> Paths
            {
                get
                {
                    if (paths != null)
                        return paths;
                    return paths = Guids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToList();
                }
            }
        }

        #region serializationHelpers

        [SerializeField] private List<string> _duplicateDictKeys = new();
        [SerializeField] private List<DuplicateAssetData> _duplicateDictValues = new();

        public Dictionary<string, DuplicateAssetData> Entries { get; private set; } = new();

        #endregion
    }
}