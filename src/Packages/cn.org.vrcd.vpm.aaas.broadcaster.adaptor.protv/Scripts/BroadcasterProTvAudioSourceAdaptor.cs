using AAAS.Broadcaster.AudioControl.Source;
using AAAS.Broadcaster.Tools;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Adaptor.ProTV {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterProTvAudioSourceAdaptor : BroadcasterAudioSourceAdaptorBase {
        [SerializeField] private ProTvBroadcasterTvPlugin tvPlugin;

        private float[] _audioData = new float[0];
        private float _lastRmsLoudness;

        public override bool IsVolumeGetterSupported() => IsValidProTvInstance();
        public override bool IsVolumeSetterSupported() => IsValidProTvInstance() && tvPlugin.IsOwnerOfPlayer();

        public override bool IsRmsLoudnessSupported() => IsValidProTvInstance();

        public override void SetVolume(float volume) => tvPlugin.SetVolume(volume);

        public override float GetVolume() => tvPlugin.GetVolume();

        public override float GetRmsLoudness() {
            if (!IsValidProTvInstance())
                return 0f;

            if (_audioData.Length == 0) {
                var windowSize = LoudnessTools.GetWindowSize();
                _audioData = new float[windowSize];
            }

            var audioSources = tvPlugin.GetAudioSources();
            var tempSource = audioSources[0];

            tempSource.GetOutputData(_audioData, 0);

            _lastRmsLoudness = LoudnessTools.GetRmsLoudness(_audioData, lastRms: _lastRmsLoudness);
            return _lastRmsLoudness;
        }

        private bool IsValidProTvInstance() {
            return tvPlugin && tvPlugin.IsValidProTvInstance();
        }
    }
}
