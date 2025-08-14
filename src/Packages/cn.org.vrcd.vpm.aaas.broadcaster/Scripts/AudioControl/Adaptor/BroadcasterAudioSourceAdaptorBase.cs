using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Source
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class BroadcasterAudioSourceAdaptorBase : UdonSharpBehaviour {
        [PublicAPI]
        public virtual bool IsVolumeGetterSupported() => false;
        [PublicAPI]
        public virtual bool IsVolumeSetterSupported() => false;
        [PublicAPI]
        public virtual bool IsRmsLoudnessSupported() => false;

        [PublicAPI]
        public virtual void SetVolume(float volume)
        {
            Debug.LogWarning("[BroadcasterAudioControllerBase] SetVolume called, but not implemented.", this);
        }

        [PublicAPI]
        public virtual float GetVolume()
        {
            Debug.LogWarning("[BroadcasterAudioControllerBase] GetVolume called, but not implemented.", this);
            return 0f;
        }

        [PublicAPI]
        public virtual float GetRmsLoudness()
        {
            Debug.LogWarning("[BroadcasterAudioControllerBase] GetRmsLoudness called, but not implemented.", this);
            return 0f;
        }
    }
}
