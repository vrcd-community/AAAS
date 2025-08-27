using AAAS.Broadcaster.VideoSwitch.Core;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoInputActiveIndicator : UdonSharpBehaviour {
        public BroadcasterVideoSwitch videoSwitch;
        public int inputIndex;

        public GameObject activeIndicator;

        private void Start() {
            if (!videoSwitch) {
                Debug.LogError("[VideoInputsTactSwitch] Video switch is not assigned. Please assign a video switch in the inspector.", this);
                return;
            }

            videoSwitch._RegisterOutputTextureChangedListener(this, nameof(OnVideoInputTextureChanged));

            OnVideoInputTextureChanged();
        }

        public void OnVideoInputTextureChanged() {
            activeIndicator.SetActive(videoSwitch.CurrentInputIndex == inputIndex);
        }
    }
}
