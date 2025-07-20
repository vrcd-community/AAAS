using AAAS.Slide.VideoPlayer;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace AAAS.Slide.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SlideCore : UdonSharpBehaviour {
        [SerializeField] [CanBeNull] private SlideVideoPlayerBase videoPlayer;

        private void Start() {
            if (!videoPlayer) {
                enabled = false;

                Debug.LogError("[SlideCore] Video player is not assigned. Please assign a video player in the inspector.");
                return;
            }

            videoPlayer.SetVideoErrorEventReceiver(this, nameof(_OnSlideVideoError));
            videoPlayer.SetVideoReadyEventReceiver(this, nameof(_OnSlideVideoReady));
        }

        // "a network event which should only be called by the local player"
        [NetworkCallable(maxEventsPerSecond: 1)] // won't affect local "network" event call, see https://creators.vrchat.com/worlds/udon/networking/events#rate-limiting
        public void _OnSlideVideoError(uint videoError) {
            if (NetworkCalling.CallingPlayer == null) {
                Debug.LogWarning("[SlideCore] Video error received with no calling player, ignoring.");
                return;
            }

            if (!NetworkCalling.CallingPlayer.isLocal) {
                Debug.LogWarning("[SlideCore] Video error received from non-local player, ignoring.");
                return;
            }

            var errorType = (VideoError)videoError;
            Debug.LogError($"[SlideCore] Video error occurred: {errorType}");
        }

        public void _OnSlideVideoReady() {
            Debug.Log("[SlideCore] Video is ready to play.");
        }
    }
}
