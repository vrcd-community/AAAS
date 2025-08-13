using System;
using ArchiTech.ProTV;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Adaptor.ProTV {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ProTvBroadcasterTvPlugin : TVPlugin {
        private int _lastVpManagerHashCode = -1;

        private AudioSource[] _cacheAudioSources = new AudioSource[0];

        internal float GetVolume() => OUT_VOLUME;
        internal void SetVolume(float volume) => tv._ChangeVolume(volume);

        internal bool IsValidProTvInstance() {
            return tv && tv.isReady && GetAudioSources().Length > 0;
        }

        internal AudioSource[] GetAudioSources() {
            if (tv.ActiveManager.GetHashCode() == _lastVpManagerHashCode)
                return _cacheAudioSources;

            return GetAudioSourcesCore();
        }

        internal bool IsOwnerOfPlayer() => tv.IsOwner;

        private AudioSource[] GetAudioSourcesCore() {
            var vpManager = tv.ActiveManager;

            var spatialSpeakers = (AudioSource[])vpManager.GetProgramVariable("spatialSpeakers");
            var stereoSpeakers = (AudioSource[])vpManager.GetProgramVariable("stereoSpeakers");

            var audioSources = new AudioSource[spatialSpeakers.Length + stereoSpeakers.Length];
            Array.Copy(spatialSpeakers, 0, audioSources, 0, spatialSpeakers.Length);
            Array.Copy(stereoSpeakers, 0, audioSources, spatialSpeakers.Length, stereoSpeakers.Length);

            _lastVpManagerHashCode = vpManager.GetHashCode();
            _cacheAudioSources = audioSources;
            return audioSources;
        }
    }
}
