using AAAS.Broadcaster.AudioControl.Source;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterAudioControlHub : UdonSharpBehaviour {
        [SerializeField]
        private BroadcasterAudioSourceAdaptorBase[] audioSourceAdaptors = new BroadcasterAudioSourceAdaptorBase[0];

        [PublicAPI]
        public BroadcasterAudioSourceAdaptorBase[] _GetAudioSourceAdaptors() {
            var newAudioSources = new BroadcasterAudioSourceAdaptorBase[audioSourceAdaptors.Length];
            System.Array.Copy(audioSourceAdaptors, newAudioSources, audioSourceAdaptors.Length);
            return newAudioSources;
        }

        [PublicAPI]
        public float _GetAdaptorVolume(int index) {
            if (index < 0 || index >= audioSourceAdaptors.Length) {
                Debug.LogError($"[BroadcasterAudioControlHub] Invalid index {index} for audio source adaptors.", this);
                return -1f;
            }

            var adaptor = audioSourceAdaptors[index];
            if (!adaptor.IsVolumeGetterSupported()) {
                Debug.LogWarning(
                    $"[BroadcasterAudioControlHub] Volume Getter not supported for adaptor at index {index}.", this);
                return -1f;
            }

            return adaptor.GetVolume();
        }

        [PublicAPI]
        public bool _SetAdaptorVolume(int index, float volume) {
            if (index < 0 || index >= audioSourceAdaptors.Length) {
                Debug.LogError($"[BroadcasterAudioControlHub] Invalid index {index} for audio source adaptors.", this);
                return false;
            }

            var adaptor = audioSourceAdaptors[index];
            if (!adaptor.IsVolumeSetterSupported()) {
                Debug.LogWarning(
                    $"[BroadcasterAudioControlHub] Volume Setter not supported for adaptor at index {index}.", this);
                return false;
            }

            adaptor.SetVolume(volume);
            return true;
        }

        [PublicAPI]
        public float _GetAdaptorRmsLoudness(int index) {
            if (index < 0 || index >= audioSourceAdaptors.Length) {
                Debug.LogError($"[BroadcasterAudioControlHub] Invalid index {index} for audio source adaptors.", this);
                return -1f;
            }

            var adaptor = audioSourceAdaptors[index];
            if (!adaptor.IsRmsLoudnessSupported()) {
                Debug.LogWarning(
                    $"[BroadcasterAudioControlHub] RMS Loudness not supported for adaptor at index {index}.", this);
                return -1f;
            }

            return adaptor.GetRmsLoudness();
        }
    }
}
