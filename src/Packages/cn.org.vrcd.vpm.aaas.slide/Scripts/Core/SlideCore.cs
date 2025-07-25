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
        public SlideCoreStatus Status { get; private set; } = SlideCoreStatus.NoSlideLoaded;
        [PublicAPI]
        public VideoError LastVideoError { get; private set; } = VideoError.Unknown;

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
                if (value < 0 || value >= PageTotal) {
                    Debug.LogWarning($"[SlideCore] Attempted to set SlidePageIndex out of bounds: {value}. " +
                                     $"Valid range is 0 to {PageTotal - 1}.", this);
                    return;
                }

                _slidePageIndex = value;
                UpdatePlayerPosition();
                SendSlidePageChangedEvent();
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

        private UdonSharpBehaviour[] _slideStatueChangedEventListeners = new UdonSharpBehaviour[0];
        private string[] _slideStatueChangedEventNames = new string[0];

        private UdonSharpBehaviour[] _slidePageChangedEventListeners = new UdonSharpBehaviour[0];
        private string[] _slidePageChangedEventNames = new string[0];

        private void Start() {
            if (!videoPlayer) {
                enabled = false;

                Debug.LogError(
                    "[SlideCore] Video player is not assigned. Please assign a video player in the inspector.", this);
                return;
            }

            videoPlayer.SetVideoErrorEventReceiver(this, nameof(_OnSlideVideoError));
            videoPlayer.SetVideoReadyEventReceiver(this, nameof(_OnSlideVideoReady));
        }

        [PublicAPI]
        public Texture _GetSlidePlayerTexture() {
            return videoPlayer.GetVideoPlayerTexture();
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

            var targetPageIndex = SlidePageIndex + 1;
            if (targetPageIndex >= PageTotal) return;

            TakeOwnership();
            SlidePageIndex = targetPageIndex;
            RequestSerialization();
        }

        [PublicAPI]
        public void _PreviousPage() {
            if (!videoPlayer.GetIsReady()) return;

            var targetPageIndex = SlidePageIndex - 1;
            if (targetPageIndex < 0) return;

            TakeOwnership();
            SlidePageIndex = targetPageIndex;
            RequestSerialization();
        }

        [PublicAPI]
        public bool _AddSlideStatueChangedEventListener(UdonSharpBehaviour listener, string eventName) {
            if (!listener || string.IsNullOrWhiteSpace(eventName))
                return false;

            _slideStatueChangedEventListeners = ArrayTools.Add(_slideStatueChangedEventListeners, listener);
            _slideStatueChangedEventNames = ArrayTools.Add(_slideStatueChangedEventNames, eventName);

            return true;
        }

        private void SendSlideStatueChangedEvent() {
            for (var listenerIndex = 0; listenerIndex < _slideStatueChangedEventListeners.Length; listenerIndex++) {
                var listener = _slideStatueChangedEventListeners[listenerIndex];
                var eventName = _slideStatueChangedEventNames[listenerIndex];

                if (!listener) continue;

                listener.SendCustomEvent(eventName);
            }
        }

        private void UpdateSlideStatus(SlideCoreStatus status, VideoError error = VideoError.Unknown) {
            if (Status == status && LastVideoError == error)
                return;

            Status = status;
            LastVideoError = error;

            SendSlideStatueChangedEvent();
        }

        [PublicAPI]
        public bool _AddSlidePageChangedEventListener(UdonSharpBehaviour listener, string eventName) {
            if (!listener || string.IsNullOrWhiteSpace(eventName))
                return false;

            _slidePageChangedEventListeners = ArrayTools.Add(_slidePageChangedEventListeners, listener);
            _slidePageChangedEventNames = ArrayTools.Add(_slidePageChangedEventNames, eventName);

            return true;
        }

        private void SendSlidePageChangedEvent() {
            for (var listenerIndex = 0; listenerIndex < _slidePageChangedEventListeners.Length; listenerIndex++) {
                var listener = _slidePageChangedEventListeners[listenerIndex];
                var eventName = _slidePageChangedEventNames[listenerIndex];

                if (!listener) continue;

                listener.SendCustomEvent(eventName);
            }
        }

        private void LoadSlideInternal() {
            if (SlideUrl == null || SlideUrl.Get() == "") {
                Debug.LogError("[SlideCore] Slide URL is empty, cannot load video.", this);
                UpdateSlideStatus(SlideCoreStatus.VideoError, VideoError.InvalidURL);

                return;
            }

            UpdateSlideStatus(SlideCoreStatus.Loading);

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
                Debug.LogWarning("[SlideCore] Video error received with no calling player, ignoring.", this);
                return;
            }

            if (!NetworkCalling.CallingPlayer.isLocal) {
                Debug.LogWarning("[SlideCore] Video error received from non-local player, ignoring.", this);
                return;
            }

            var errorType = (VideoError)videoError;
            Debug.LogError($"[SlideCore] Video error occurred: {errorType}", this);

            UpdateSlideStatus(SlideCoreStatus.VideoError, errorType);
        }

        public void _OnSlideVideoReady() {
            Debug.Log("[SlideCore] Video is ready to play.", this);

            CalculateSlideData();
            UpdatePlayerPosition();
            UpdateSlideStatus(SlideCoreStatus.Ready);
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

    public enum SlideCoreStatus {
        NoSlideLoaded,
        Loading,
        Ready,
        VideoError
    }
}
