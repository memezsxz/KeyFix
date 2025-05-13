using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl
{
    [Serializable]
    public class TreeElement
    {
        [SerializeField] internal int m_ID;
        [SerializeField] internal string m_Name;
        [SerializeField] internal int m_Depth;
        [SerializeField] private bool m_Enabled;
        [NonSerialized] private List<TreeElement> m_Children;
        [NonSerialized] private TreeElement m_Parent;

        public TreeElement()
        {
        }

        public TreeElement(string name, int depth, int id)
        {
            m_Name = name;
            m_ID = id;
            m_Depth = depth;
        }

        public int depth
        {
            get => m_Depth;
            set => m_Depth = value;
        }

        public TreeElement parent
        {
            get => m_Parent;
            set => m_Parent = value;
        }

        public List<TreeElement> children
        {
            get => m_Children;
            set => m_Children = value;
        }

        public bool hasChildren => children != null && children.Count > 0;

        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public int id
        {
            get => m_ID;
            set => m_ID = value;
        }

        public bool Enabled
        {
            get => m_Enabled;

            set => m_Enabled = value;
        }
    }
}