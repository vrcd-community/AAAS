using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Video.Components;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace AAAS.Slide.VideoPlayer {
    public class UnitySlideVideoPlayer : SlideVideoPlayerBase {
        [SerializeField] private VRCUnityVideoPlayer videoPlayer;

        public override double GetDuration() => videoPlayer.IsReady ? videoPlayer.GetDuration() : -1;
        public override double GetPosition() => videoPlayer.IsReady ? videoPlayer.GetTime() : -1;

        [CanBeNull]
        private VRCUrl _videoUrl;

        [CanBeNull]
        private UdonSharpBehaviour _videoErrorEventReceiver;
        [CanBeNull]
        private UdonSharpBehaviour _videoReadyEventReceiver;

        private string _videoErrorEventName = "OnSlideVideoError";
        private string _videoReadyEventName = "OnSlideVideoReady";

        public override VRCUrl GetVideoUrl() {
            return _videoUrl;
        }

        public override void LoadVideo(VRCUrl url) {
            _videoUrl = url;
            videoPlayer.LoadURL(url);
        }

        public override bool Seek(double position) {
            if (!videoPlayer.IsReady)
                return false;

            videoPlayer.SetTime((float)position);
            return true;
        }

        public override void Unload() {
            _videoUrl = null;
            videoPlayer.Stop();
        }

        public override bool GetIsReady() {
            return videoPlayer.IsReady;
        }

        public override void SetVideoErrorEventReceiver(UdonSharpBehaviour receiver,
            string eventName = "OnSlideVideoError") {
            _videoErrorEventName = eventName;
            _videoErrorEventReceiver = receiver;
        }

        public override void SetVideoReadyEventReceiver(UdonSharpBehaviour receiver,
            string eventName = "OnSlideVideoReady") {
            _videoReadyEventName = eventName;
            _videoReadyEventReceiver = receiver;
        }

        public override void OnVideoError(VideoError videoError) {
            if (!_videoErrorEventReceiver)
                return;

            _videoErrorEventReceiver.SendCustomNetworkEvent(NetworkEventTarget.Self, _videoErrorEventName,
                (uint)videoError);
        }

        public override void OnVideoReady() {
            if (!_videoReadyEventReceiver)
                return;

            _videoReadyEventReceiver.SendCustomEvent(_videoReadyEventName);
        }
    }
}
