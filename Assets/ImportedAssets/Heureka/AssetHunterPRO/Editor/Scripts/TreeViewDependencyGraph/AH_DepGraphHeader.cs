﻿using UnityEditor.IMGUI.Controls;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph
{
    internal class AH_DepGraphHeader : MultiColumnHeader
    {
        public enum Mode
        {
            Treeview,
            SortedList
        }

        private Mode m_Mode;

        public AH_DepGraphHeader(MultiColumnHeaderState state) : base(state)
        {
            mode = Mode.Treeview;
        }

        public Mode mode
        {
            get => m_Mode;
            set
            {
                m_Mode = value;
                switch (m_Mode)
                {
                    case Mode.Treeview:
                        canSort = true;
                        height = DefaultGUI.minimumHeight;
                        break;
                    case Mode.SortedList:
                        canSort = true;
                        height = DefaultGUI.defaultHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
        {
            if (mode == Mode.Treeview) mode = Mode.SortedList;

            base.ColumnHeaderClicked(column, columnIndex);
        }
    }
}