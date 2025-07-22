using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AAAS.Slide.VideoPlayer {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class SlideVideoPlayerBase : UdonSharpBehaviour {
        /// <summary>
        /// Duration of the video in seconds, will be -1 if the video is not read or duration is unknown
        /// </summary>
        [PublicAPI]
        public virtual double GetDuration() {
            return -1;
        }

        /// <summary>
        /// Position of the video in seconds, will be -1 if the video is not read or position is unknown
        /// </summary>
        [PublicAPI]
        public virtual double GetPosition() {
            return -1;
        }

        /// <summary>
        /// Whether the video is currently playing, will be null if the video is not loaded
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public virtual VRCUrl GetVideoUrl() {
            return null;
        }

        /// <summary>
        /// Is video loaded and ready to play
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public virtual bool GetIsReady() {
            return false;
        }

        /// <summary>
        /// Load a video from a VRCUrl
        /// </summary>
        /// <param name="url">VRCUrl to the video</param>
        [PublicAPI]
        public virtual void LoadVideo(VRCUrl url) { }

        /// <summary>
        /// Jump to a specific time in the video
        /// </summary>
        /// <param name="position">position in second</param>
        /// <returns>true if the seek was successful, false if the video is not loaded or seek failed</returns>
        [PublicAPI]
        public virtual bool Seek(double position) {
            return false;
        }

        /// <summary>
        /// Unload the current video
        /// </summary>
        [PublicAPI]
        public virtual void Unload() { }

        /// <summary>
        /// Get texture of the video screen
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public virtual Texture GetVideoPlayerTexture() {
            return null;
        }

        /// <summary>
        /// Called when a video error occurs, such as unsupported format or network issues. Will send a custom **network** event target to self with the <see cref="VRC.SDK3.Components.Video.VideoError"/> in the uint
        /// </summary>
        /// <param name="receiver">UdonSharpBehaviour Receiver</param>
        /// <param name="eventName">Event name to call on the receiver</param>
        [PublicAPI]
        public virtual void SetVideoErrorEventReceiver([CanBeNull] UdonSharpBehaviour receiver,
            string eventName = "OnSlideVideoError") { }

        /// <summary>
        /// Called when the video is ready to play, such as when the video has loaded and is ready to be played. Will send a custom network event with no parameters
        /// </summary>
        /// <param name="receiver">UdonSharpBehaviour Receiver</param>
        /// <param name="eventName">Event name to call on the receiver</param>
        [PublicAPI]
        public virtual void SetVideoReadyEventReceiver([CanBeNull] UdonSharpBehaviour receiver,
            string eventName = "OnSlideVideoReady") { }
    }
}
