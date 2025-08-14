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

        private OnPointerDownDetector _pointerDownDetector;

        private bool _isVolumeSliderAssigned;
        private bool _isRmsSliderAssigned;
        private bool _isPointerDownDetectorAssigned;

        private void Start() {
            if (!audioSourceAdaptor) {
                Debug.LogError("[BroadcasterAudioAdaptorItem] Audio Source Adaptor is not assigned.", this);
                enabled = false;
                return;
            }

            if (!volumeSlider) {
                Debug.LogWarning("[BroadcasterAudioAdaptorItem] Volume Slider is not assigned.", this);
            }
            else {
                _isVolumeSliderAssigned = true;
            }

            if (!rmsSlider) {
                Debug.LogWarning("[BroadcasterAudioAdaptorItem] RMS Slider is not assigned.", this);
            }
            else {
                _isRmsSliderAssigned = true;
            }

            _pointerDownDetector = GetComponentInParent<OnPointerDownDetector>();

            if (!_pointerDownDetector) {
                Debug.LogWarning(
                    "[BroadcasterAudioAdaptorItem] OnPointerDownDetector is not assigned. Slider will be unable to interact!",
                    this);

                if (_isVolumeSliderAssigned)
                    volumeSlider.interactable = false;
            }
            else {
                _isPointerDownDetectorAssigned = true;
            }
        }

        private void LateUpdate() {
            if (_isRmsSliderAssigned) {
                rmsSlider.value = audioSourceAdaptor.GetRmsLoudness();
            }

            if (_isPointerDownDetectorAssigned && _pointerDownDetector.IsPointerDown())
                return;

            if (_isVolumeSliderAssigned) {
                volumeSlider.value = audioSourceAdaptor.GetVolume();
            }
        }

        [PublicAPI]
        public void NotifyVolumeSliderUpdate() {
            var volume = volumeSlider.value;
            audioSourceAdaptor.SetVolume(volume);
        }
    }
}
