using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.TempAssembly.Common {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(AudioSource))]
    public class PlayOneShotAudio : UdonSharpBehaviour {
        public AudioClip audioClip;

        [PublicAPI]
        public void PlayAudio() {
            if (!audioClip) {
                Debug.LogWarning("PlayOneShotAudio: No audio clip assigned.", this);
                return;
            }

            var audioSource = GetComponent<AudioSource>();
            if (!audioSource) {
                Debug.LogWarning("PlayOneShotAudio: No AudioSource component found.", this);
                return;
            }

            audioSource.PlayOneShot(audioClip);
        }
    }
}
