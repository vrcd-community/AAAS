using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class VideoInputPreviewer : UdonSharpBehaviour {
        [PublicAPI]
        public BroadcasterVideoInputBase videoInput;

        [PublicAPI]
        public RawImage previewRawImage;

        private void Start() {
            if (!videoInput) {
                Debug.LogError("[VideoInputPreviewer] No video input assigned.", this);
                enabled = false;
                return;
            }

            if (!previewRawImage) {
                Debug.LogError("[VideoInputPreviewer] No preview RawImage assigned.", this);
                enabled = false;
                return;
            }

            videoInput.RegisterVideoTextureChangedListener(this, nameof(OnVideoTextureChanged));
            previewRawImage.texture = videoInput.GetVideoTexture();
        }

        // "a network event which should only be called by the local player"
        [NetworkCallable(
            maxEventsPerSecond:
            1)] // won't affect local "network" event call, see https://creators.vrchat.com/worlds/udon/networking/events#rate-limiting
        public void OnVideoTextureChanged(string _) {
            if (NetworkCalling.CallingPlayer == null) {
                Debug.LogWarning(
                    "[VideoInputPreviewer] Video Input Texture Changed received with no calling player, ignoring.",
                    this);
                return;
            }

            if (!NetworkCalling.CallingPlayer.isLocal) {
                Debug.LogWarning(
                    "[VideoInputPreviewer] Video Input Texture Changed received from non-local player, ignoring.",
                    this);
                return;
            }
            
            previewRawImage.texture = videoInput.GetVideoTexture();
        }
    }
}
