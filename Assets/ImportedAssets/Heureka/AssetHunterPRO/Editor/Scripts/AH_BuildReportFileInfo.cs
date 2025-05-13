//Only avaliable in 2018
#if UNITY_2018_1_OR_NEWER
using System;
using UnityEditor.Build.Reporting;

namespace HeurekaGames.AssetHunterPRO
{
    [Serializable]
    public class AH_BuildReportFileInfo
    {
        public string Path;
        public string Role;
        public ulong Size;

        public AH_BuildReportFileInfo(BuildFile file)
        {
            Path = file.path;
            Role = file.role;
            Size = file.size;
        }
    }
}
#endif