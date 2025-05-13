using UnityEngine;

namespace Polyperfect.Common
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [DefaultExecutionOrder(-100)]
    public class AnimationOffset : MonoBehaviour
    {
        [Tooltip("Seconds to offset the animation")] [SerializeField]
        private float ConstantOffset;

        [Tooltip("An additional random offset to be added")] [SerializeField]
        private float RandomOffset;

        private void Start()
        {
            if (TryGetComponent(out AnimationDelay delay))
                delay.OnAnimStart.AddListener(HandleOffset);
            else
                HandleOffset();
        }

        private void OnValidate()
        {
            RandomOffset = Mathf.Clamp(RandomOffset, 0f, float.MaxValue);
        }

        private void HandleOffset()
        {
            GetComponent<Animator>().Update(ConstantOffset + Random.Range(0f, RandomOffset));
        }
    }
}