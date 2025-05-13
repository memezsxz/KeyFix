using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[Serializable]
public class AH_SerializableAssetInfo
{
    public string ID;

    /// <summary>
    ///     In 2.1.5 and older this property is a list of paths
    /// </summary>
    public List<string> Refs;

    public AH_SerializableAssetInfo()
    {
    }

    public AH_SerializableAssetInfo(string assetPath, List<string> scenes)
    {
        ID = AssetDatabase.AssetPathToGUID(assetPath);
        Refs = scenes;
    }

    internal void ChangePathToGUID()
    {
        Refs = Refs.Select(x => AssetDatabase.AssetPathToGUID(x)).ToList();
    }
}