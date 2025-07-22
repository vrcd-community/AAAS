using System;
using AAAS.Slide.Tools;
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
        [SerializeField] private SlideVideoPlayerBase videoPlayer;

        [UdonSynced] [CanBeNull] [FieldChangeCallback(nameof(SlideUrl))]
        private VRCUrl _slideUrl;

        [PublicAPI]
        public VRCUrl SlideUrl {
            get => _slideUrl;
            private set {
                _slideUrl = value;
                LoadSlideInternal();
            }
        }

        [UdonSynced] [FieldChangeCallback(nameof(SlidePageIndex))]
        private int _slidePageIndex;

        [PublicAPI]
        public int SlidePageIndex {
            get => _slidePageIndex;
            private set {
                Debug.Log("Current slide page index: " + value);
                _slidePageIndex = value;
                UpdatePlayerPosition();
            }
        }

        /// <summary>
        /// Total Page Count
        /// </summary>
        [PublicAPI]
        public uint PageTotal { get; private set; }
        /// <summary>
        /// Seconds Per Page
        /// </summary>
        private double _step;

        private void Start() {
            if (!videoPlayer) {
                enabled = false;

                Debug.LogError(
                    "[SlideCore] Video player is not assigned. Please assign a video player in the inspector.");
                return;
            }

            videoPlayer.SetVideoErrorEventReceiver(this, nameof(_OnSlideVideoError));
            videoPlayer.SetVideoReadyEventReceiver(this, nameof(_OnSlideVideoReady));
        }

        [PublicAPI]
        public void _LoadSlide(VRCUrl url, int pageIndex = 0) {
            TakeOwnership();

            SlideUrl = url;
            SlidePageIndex = 0;

            RequestSerialization();
        }

        [PublicAPI]
        public void _NextPage() {
            if (!videoPlayer.GetIsReady()) return;
            if (SlidePageIndex >= PageTotal) return;

            TakeOwnership();
            SlidePageIndex++;
            RequestSerialization();
        }

        [PublicAPI]
        public void _PreviousPage() {
            if (!videoPlayer.GetIsReady()) return;
            if (SlidePageIndex <= 0) return;

            TakeOwnership();
            SlidePageIndex--;
            RequestSerialization();
        }

        private void LoadSlideInternal() {
            if (SlideUrl == null || SlideUrl.Get() == "") {
                Debug.LogError("[SlideCore] Slide URL is empty, cannot load video.");
                return;
            }

            SlidePageIndex = 0;
            _step = 0;
            PageTotal = 0;
            videoPlayer.LoadVideo(SlideUrl);
        }

        private void UpdatePlayerPosition() {
            if (!videoPlayer.GetIsReady())
                return;

            var position = CalculateSlidePositionInVideo();
            videoPlayer.Seek(position);
        }

        private double CalculateSlidePositionInVideo() {
            return MathTools.Clamp(SlidePageIndex * _step, 0, videoPlayer.GetDuration());
        }

        private void CalculateSlideData() {
            var duration = videoPlayer.GetDuration();

            PageTotal = (uint)Math.Floor(videoPlayer.GetDuration());
            _step = duration / PageTotal;
        }

    #region Video Player Event Handle

        // "a network event which should only be called by the local player"
        [NetworkCallable(
            maxEventsPerSecond:
            1)] // won't affect local "network" event call, see https://creators.vrchat.com/worlds/udon/networking/events#rate-limiting
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

            CalculateSlideData();
            UpdatePlayerPosition();
        }

    #endregion

    #region Networking

        private void TakeOwnership() {
            if (Networking.IsOwner(gameObject))
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

    #endregion
    }
}
