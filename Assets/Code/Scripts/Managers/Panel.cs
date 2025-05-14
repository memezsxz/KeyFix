using UnityEngine;


    public class Panel : MonoBehaviour
    {
        [SerializeField] private PressButton.PressedColor color;
        private Material _material;

        public PressButton.PressedColor AssignedColor => color;

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
