using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace AAAS.Slide.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SlideUserController : UdonSharpBehaviour {
        [SerializeField] private SlideCore slideCore;

        [SerializeField] private VRCUrlInputField urlInputField;

        public void _LoadUrl() {
            if (!slideCore)
                return;

            var url = urlInputField.GetUrl();

            slideCore._LoadSlide(url);
        }
    }
}
