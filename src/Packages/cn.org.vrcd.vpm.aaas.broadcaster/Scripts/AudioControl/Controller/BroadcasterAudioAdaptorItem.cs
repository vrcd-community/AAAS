using AAAS.Broadcaster.AudioControl.Source;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.AudioControl.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterAudioAdaptorItem : UdonSharpBehaviour {
        [SerializeField] internal BroadcasterAudioSourceAdaptorBase audioSourceAdaptor;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Slider rmsSlider;

        private void Start() {
            if (!audioSourceAdaptor) {
                Debug.LogError("[BroadcasterAudioAdaptorItem] Audio Source Adaptor is not assigned.", this);
                enabled = false;
                return;
            }

            if (!volumeSlider) {
                Debug.LogWarning("[BroadcasterAudioAdaptorItem] Volume Slider is not assigned.", this);
            }

            if (!rmsSlider) {
                Debug.LogWarning("[BroadcasterAudioAdaptorItem] RMS Slider is not assigned.", this);
            }
        }

        [PublicAPI]
        public void NotifyVolumeSliderUpdate() {

        }
    }
}
