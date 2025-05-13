using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        [Header("Settings")] public bool fixOnEnable = true;

        public bool fixParent;
        public bool fixWithDelay;
        private readonly float fixDelay = 0.025f;

        private void OnEnable()
        {
            if (fixWithDelay == false)
            {
                if (fixOnEnable && fixParent == false)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                else if (fixOnEnable && fixParent)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            }

            else
            {
                StartCoroutine("FixDelay");
            }
        }

        public void FixLayout()
        {
            if (fixWithDelay == false)
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            else
                StartCoroutine("FixDelay");
        }

        private IEnumerator FixDelay()
        {
            yield return new WaitForSeconds(fixDelay);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}