using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace HeurekaGames.AssetHunterPRO
{
    public interface AH_IIgnoreListActions
    {
        string Header { get; }
        string FoldOutContent { get; }
        event EventHandler<IgnoreListEventArgs> IgnoredAddedEvent;
        void DrawIgnored(AH_IgnoreList ignoredList);
        void IgnoreCallback(Object obj, string identifier);
        string GetFormattedItem(string identifier);
        string GetFormattedItemShort(string identifier);
        string GetLabelFormattedItem(string item);
        bool ContainsElement(List<string> ignoredList, string element, string identifier = "");
    }
}