using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeurekaGames
{
    [Serializable]
    public class Heureka_PackageData : ScriptableObject
    {
        public Texture Icon;
        public int PackageShowPrio;
        public string AssetName;
        public string Subheader;
        public string AssetIdentifier;
        public string Description;
        public List<PackageLinks> Links = new();
    }

    [Serializable]
    public struct PackageLinks
    {
        public bool ActiveLink;
        public string Name;
        public string Link;

        public PackageLinks(string Name, string Link)
        {
            this.Name = Name;
            this.Link = Link;
            ActiveLink = true;
        }

        public PackageLinks(string Name, string Link, bool LinkActive)
        {
            this.Name = Name;
            this.Link = Link;
            ActiveLink = LinkActive;
        }
    }
}