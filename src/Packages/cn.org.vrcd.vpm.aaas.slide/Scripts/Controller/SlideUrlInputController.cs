using AAAS.Slide.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace AAAS.Slide.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class SlideUrlInputController : UdonSharpBehaviour {
        [SerializeField] private SlideCore slideCore;

        [SerializeField] private VRCUrlInputField urlInputField;

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError(
                    "[SlideUrlInputController] SlideCore is not assigned. Please assign a SlideCore in the inspector.",
                    this);
                return;
            }

            if (!urlInputField) {
                enabled = false;
                Debug.LogError(
                    "[SlideUrlInputController] VRCUrlInputField is not assigned. Please assign a VRCUrlInputField in the inspector.",
                    this);
                return;
            }
        }

        [PublicAPI]
        public void TriggerLoadUrl() {
            var url = urlInputField.GetUrl();
            slideCore._LoadSlide(url);
        }
    }
}
