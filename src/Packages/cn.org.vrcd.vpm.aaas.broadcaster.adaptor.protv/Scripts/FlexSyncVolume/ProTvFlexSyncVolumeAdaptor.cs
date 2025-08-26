using AAAS.Broadcaster.AudioControl.Source;
using AAAS.Broadcaster.Tools;
using AAAS.ProTvPlugins;
using UdonSharp;

namespace AAAS.Broadcaster.AudioControl.Adaptor.ProTV.FlexSyncVolume {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ProTvFlexSyncVolumeAdaptor : BroadcasterAudioSourceAdaptorBase {
        public ProTvBroadcasterTvPlugin tvPlugin;
        public ProTvFlexSyncVolume flexSyncVolumePlugin;

        private float[] _audioData = new float[0];
        private float _lastRmsLoudness;

        public bool isLocalVolumeMode;

        public override bool IsVolumeGetterSupported() => IsValidProTvInstance();
        public override bool IsVolumeSetterSupported() => IsValidProTvInstance();

        public override bool IsRmsLoudnessSupported() => IsValidProTvInstance();

        public override void SetVolume(float volume) {
            if (isLocalVolumeMode) {
                flexSyncVolumePlugin._SetLocalVolume(volume);
                return;
            }

            flexSyncVolumePlugin._SetSyncVolume(volume);
        }

        public override float GetVolume() => isLocalVolumeMode
            ? flexSyncVolumePlugin._GetLocalVolume()
            : flexSyncVolumePlugin._GetSyncVolume();

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