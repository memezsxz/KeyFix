using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Scripts.Interactable;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Managers
{
    public class HallwaysManager : Singleton<HallwaysManager>, IDataPersistence
    {
        [FormerlySerializedAs("doorInteract")] [SerializeField]
        private List<Door> doors = new();

        [SerializeField] private TextMeshProUGUI collectablesText;

        private void Start()
        { 
            GameObject.FindGameObjectsWithTag("Door").ToList()
                .ForEach(gm => doors.Add(gm.GetComponentInChildren<Door>()));
            // Debug.Log("Doors found: " + doors.Count);

            doors.ForEach(d =>
            {
                if (SaveManager.Instance.SaveData.Progress.RepairedKeys.Contains(d.Scene))
                    d.gameObject.layer = LayerMask.NameToLayer("Default");
            });
        }

        public void SaveData(ref SaveData data)
        {
            var psd = data.CharacterStates.FirstOrDefault(cse => cse.Type == CharacterType.Robot)?.State;
            var tf = GameManager.Instance.GetPlayerTransform();
            if (psd != null && tf != null)
            {
                psd.HallwaysPosition = tf.position;
                psd.HallwaysRotation = tf.rotation;
            }
        }

        public void LoadData(ref SaveData data)
        {
            var psd = data.CharacterStates.FirstOrDefault(cse => cse.Type == CharacterType.Robot)?.State;
            if (psd != null) GameManager.Instance.MovePlayerTo(psd.HallwaysPosition, psd.HallwaysRotation);

            UpdateInteractablesUI(data.Progress.CollectablesCount);
        }


        public void HandleVictory(GameManager.Scenes scene)
        {
            StartCoroutine(DelayedMoveToExit(scene));
        }

        private IEnumerator DelayedMoveToExit(GameManager.Scenes scene)
        {
            // Debug.Log("Doors count at victory: " + doors.Count);

            yield return null; // Wait 1 frame
            yield return new WaitUntil(() => GameManager.Instance.GetPlayerTransform() != null);

            var door = doors.FirstOrDefault(d => d.Scene == scene && d.type == Door.DoorType.Exit);
            if (door != null)
                door.HandleExitDoor();
            // Debug.Log("Player moved to exit door of " + scene);
            else
                Debug.LogWarning("Exit door not found for scene: " + scene);
        }

        public void UpdateInteractablesUI(int value)
        {
            collectablesText.text = value.ToString();
        }
    }
}