using System;
using UnityEngine;

namespace Code.Scripts.Managers
{
    public class Panel : MonoBehaviour
    {
        private Material _material;

        [SerializeField] PressButton.PressedColor color;
        public PressButton.PressedColor AssignedColor
        {
            get { return color;} }

        private void Start()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        public void ChangeColor(PressButton.PressedColor color)
        {
            this.color = color;
            switch (color)
            {
                case PressButton.PressedColor.Blue:
                    _material.color = Color.blue;
                    break;
                case PressButton.PressedColor.Red:
                    _material.color = Color.red;
                    break;
                case PressButton.PressedColor.Green:
                    _material.color = Color.green;
                    break;
            }
        }
    }
}