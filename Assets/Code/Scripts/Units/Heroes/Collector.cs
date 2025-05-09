using System;
using UnityEngine;

namespace Code.Scripts.Units.Heroes
{
    public class Collector : MonoBehaviour
    {
        public int CollectablesCount => SaveManager.Instance.SaveData.Progress.CollectablesCount;

        public void AddCollectable()
        {
            SaveManager.Instance.SaveData.Progress.CollectablesCount++;
        }
    }
}