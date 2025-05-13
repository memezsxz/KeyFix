using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    internal class AH_SerializationHelper
    {
        public delegate void NewBuildInfoCreatedDelegate(string path);

        public const string BuildInfoExtension = "ahbuildinfo";
        public const string SettingsExtension = "ahsetting";
        public const string FileDumpExtension = "ahfiledump";

        public const string DateTimeFormat = "yyyy_MM_dd_HH_mm_ss";
        public static NewBuildInfoCreatedDelegate NewBuildInfoCreated;

        internal static void SerializeAndSave(AH_SerializedBuildInfo ahBuildInfo)
        {
            var buildinfoFileName = ahBuildInfo.buildTargetInfo + "_" + ahBuildInfo.dateTime + "." + BuildInfoExtension;
            var filePath = GetBuildInfoFolder() + Path.DirectorySeparatorChar + buildinfoFileName;
            Directory.CreateDirectory(GetBuildInfoFolder());

            File.WriteAllText(filePath, JsonUtility.ToJson(ahBuildInfo));
            if (AH_SettingsManager.Instance.AutoOpenLog)
                EditorUtility.RevealInFinder(filePath);

            if (NewBuildInfoCreated != null)
                NewBuildInfoCreated(filePath);
        }

        internal static string GetDateString()
        {
            return DateTime.Now.ToString(DateTimeFormat);
        }

        internal static void SerializeAndSave(object instance, string path)
        {
            File.WriteAllText(path, JsonUtility.ToJson(instance));
        }

        internal static AH_SerializedBuildInfo LoadBuildReport(string path)
        {
            var fileContent = "";
            try
            {
                fileContent = File.ReadAllText(path);
            }
            catch (FileNotFoundException e)
            {
                EditorUtility.DisplayDialog(
                    "File Not Found",
                    "Unable to find:" + Environment.NewLine + path,
                    "Ok");

                Debug.LogError("AH: Unable to find: " + path + Environment.NewLine + e);

                return null;
            }

            try
            {
                var buildInfo = JsonUtility.FromJson<AH_SerializedBuildInfo>(fileContent);
                buildInfo.Sort();
                return buildInfo;
            }
            catch (Exception e)
            {
                Debug.LogError("AH: JSON Parse error of " + path + Environment.NewLine + "- " + e);
                return null;
            }
        }

        internal static string GetBuildInfoFolder()
        {
            return AH_SettingsManager.Instance.BuildInfoPath;
        }

        internal static string GetSettingFolder()
        {
            var userpreferencesPath = AH_SettingsManager.Instance.UserPreferencePath;
            var dirInfo = Directory.CreateDirectory(userpreferencesPath);
            return dirInfo.FullName;
        }

        internal static string GetBackupFolder()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        internal static void LoadSettings(AH_SettingsManager instance, string path)
        {
            var text = File.ReadAllText(path);
            try
            {
                EditorJsonUtility.FromJsonOverwrite(text, instance);
            }
            catch (Exception e)
            {
                Debug.LogError("AH: JSON Parse error of " + path + Environment.NewLine + "- " + e);
            }
        }
    }
}