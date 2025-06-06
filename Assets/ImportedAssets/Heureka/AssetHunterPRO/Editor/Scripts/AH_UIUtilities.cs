﻿using UnityEngine;

namespace HeurekaGames.AssetHunterPRO
{
    public static class AH_UIUtilities
    {
        public static bool DrawSelectionButton(GUIContent content)
        {
            var btnContent = new GUIContent(content);
            if (AH_SettingsManager.Instance.HideButtonText)
                btnContent.text = null;

            return GUILayout.Button(btnContent,
                GUILayout.MaxHeight(AH_SettingsManager.Instance.HideButtonText
                    ? AH_Window.ButtonMaxHeight * 2f
                    : AH_Window.ButtonMaxHeight));
        }
    }
}