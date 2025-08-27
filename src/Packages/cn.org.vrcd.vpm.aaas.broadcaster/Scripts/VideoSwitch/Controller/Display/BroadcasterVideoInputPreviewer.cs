using AAAS.Broadcaster.VideoSwitch.Core;
using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.UdonNetworkCalling;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BroadcasterVideoInputPreviewer : UdonSharpBehaviour {
        [PublicAPI]
        [Header("If VideoInput assigned, use VideoInput")]
        public BroadcasterVideoInputBase videoInput;

        [PublicAPI]
        public int inputIndex;
        [PublicAPI]
        public BroadcasterVideoSwitchController videoSwitchController;

        [PublicAPI]
        public RawImage previewRawImage;

        private void Start() {
            if (!videoInput && !videoSwitchController) {
                Debug.LogError("[VideoInputPreviewer] No video input or switch controller assigned.", this);
                enabled = false;
                return;
            }

            if (!previewRawImage) {
                Debug.LogError("[VideoInputPreviewer] No preview RawImage assigned.", this);
                enabled = false;
                return;
            }

            if (!videoInput) {
                var inputs = videoSwitchController.GetVideoInputs();
                if (inputIndex < 0 || inputIndex >= inputs.Length) {
                    Debug.LogError(
                        $"[VideoInputPreviewer] Input index {inputIndex} is out of range. There are {inputs.Length} inputs.",
                        this);
                    enabled = false;
                    return;
                }
                
                videoInput = inputs[inputIndex];
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
