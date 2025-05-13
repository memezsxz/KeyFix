using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames
{
    public static class Heureka_AddDefineSymbols
    {
        /// <summary>
        ///     Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        public static void AddDefineSymbols(string[] Symbols)
        {
            var definesString =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var allDefines = definesString.Split(';').ToList();

            var newDefines = Symbols.Except(allDefines);
            if (newDefines.Count() > 0)
            {
                Debug.Log($"Adding Compile Symbols {string.Join("; ", newDefines.ToArray())}");
                allDefines.AddRange(newDefines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    EditorUserBuildSettings.selectedBuildTargetGroup,
                    string.Join(";", allDefines.ToArray()));
            }
        }
    }
}