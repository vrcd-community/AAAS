using AAAS.Broadcaster.VideoSwitch.Core;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoInputActiveIndicator : UdonSharpBehaviour {
        public BroadcasterVideoSwitchController videoSwitchController;
        public int inputIndex;

        public GameObject activeIndicator;

        private BroadcasterVideoSwitch _videoSwitch;

        private void Start() {
            if (!videoSwitchController) {
                Debug.LogError("[BroadcasterVideoInputActiveIndicator] Video switch controller is not assigned. Please assign a video switch in the inspector.", this);
                return;
            }

            _videoSwitch = videoSwitchController.GetVideoSwitch();
            _videoSwitch._RegisterOutputTextureChangedListener(this, nameof(OnVideoInputTextureChanged));

            OnVideoInputTextureChanged();
        }

        public void OnVideoInputTextureChanged() {
            activeIndicator.SetActive(_videoSwitch.CurrentInputIndex == inputIndex);
        }
    }
}
