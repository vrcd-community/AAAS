using AAAS.Slide.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace AAAS.Slide.Controller {
    /// <summary>
    /// Controller for user interactions with the slide system.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SlideController : UdonSharpBehaviour {
        [SerializeField] protected SlideCore slideCore;

        [SerializeField] private VRCUrlInputField urlInputField;

        [PublicAPI]
        public void LoadUrl() {
            if (!slideCore || !urlInputField)
                return;

            var url = urlInputField.GetUrl();

            slideCore._LoadSlide(url);
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
