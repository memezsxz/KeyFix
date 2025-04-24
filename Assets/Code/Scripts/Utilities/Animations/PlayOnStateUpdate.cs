using System.Reflection;
using UnityEngine;

namespace Code.Scripts.Utilities.Animations
{
    public class PlayOnStateUpdate : StateMachineBehaviour
    {
        [SerializeField] private AudioClip _audioClip;

        [SerializeField, Range(-1f, 1f)]
        private float _volume = -1f;
 
        public float Volume
        {
            get => _volume == 0f ? 0.001f : _volume;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (_volume > 0)
            {SoundManager.Instance.PlaySound(_audioClip, Volume);
            }
            else {SoundManager.Instance.PlaySound(_audioClip);}
        }
        
    }
}