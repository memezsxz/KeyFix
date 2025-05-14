using UnityEngine;

namespace Code.Scripts.Units.Heroes
{
    /// <summary>
    /// Tracks the number of collectables the player has acquired,
    /// and updates the save data accordingly.
    /// </summary>
    public class Collector : MonoBehaviour
    {
        /// <summary>
        /// Gets the current number of collectables from save data.
        /// </summary>
        public int CollectablesCount => SaveManager.Instance.SaveData.Progress.CollectablesCount;

        /// <summary>
        /// Increments the collectable count in the save data by 1.
        /// </summary>
        public void AddCollectable()
        {
            SaveManager.Instance.SaveData.Progress.CollectablesCount++;
        }
    }
}