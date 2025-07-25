using AAAS.Slide.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Slide.Controller {
    /// <summary>
    /// Controller for user interactions with the slide system.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SlideController : UdonSharpBehaviour {
        [SerializeField] protected SlideCore slideCore;

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError("[SlideController] SlideCore is not assigned. Please assign a SlideCore instance in the inspector.", this);
                return;
            }
        }

        [PublicAPI]
        public void NextPage() {
            slideCore._NextPage();
        }

        [PublicAPI]
        public void PreviousPage() {
            slideCore._PreviousPage();
        }
    }
}
