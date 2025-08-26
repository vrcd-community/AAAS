using ArchiTech.ProTV;
using JetBrains.Annotations;
using UdonSharp;

namespace AAAS.ProTvPlugins {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ProTvFlexSyncVolume : TVPlugin {
        [UdonSynced]
        private float _syncVolume = 1f;
        
        private float _localVolumeScale = 1f;
        
        [PublicAPI]
        public float _GetSyncVolume() => _syncVolume;
        [PublicAPI]
        public float _GetLocalVolume() => _localVolumeScale;

        [PublicAPI]
        public bool _SetSyncVolume(float volume) {
            if (volume < 0 || volume > 1f) {
                Error("Sync Volume must be between 0 and 1");
                return false;
            }

            Owner = localPlayer;
            _syncVolume = volume;
            RequestSerialization();
            
            UpdateVolumeCore();
            return true;
        }

        [PublicAPI]
        public bool _SetLocalVolume(float volume) {
            if (volume < 0 || volume > 1f) {
                Error("Local Volume Scale must be between 0 and 1");
                return false;
            }
            
            _localVolumeScale = volume;
            UpdateVolumeCore();
            return true;
        }

        private void UpdateVolumeCore() {
            var targetVolume = _syncVolume * _localVolumeScale;
            tv._ChangeVolume(targetVolume);
        }

        public override void OnDeserialization() {
            UpdateVolumeCore();
        }
    }
}