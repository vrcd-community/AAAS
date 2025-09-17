using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
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
                var rawImageInObject = GetComponent<RawImage>();
                if (!rawImageInObject) {
                    Debug.LogError("[VideoInputPreviewer] No preview RawImage assigned or in same game object.", this);
                    enabled = false;
                    return;
                }
                
                previewRawImage = rawImageInObject;
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

            Debug.Log("[VideoInputPreviewer] Registering video texture changed listener.", this);
            videoInput.RegisterVideoTextureChangedListener(this, nameof(_OnVideoTextureChanged));
            previewRawImage.texture = videoInput.GetVideoTexture();
        }

        public void _OnVideoTextureChanged() {
            var texture = videoInput.GetVideoTexture();
            Debug.Log("[VideoInputPreviewer] Video texture changed, updating preview.", this);
            Debug.Log("[VideoInputPreviewer] Texture", texture);
            previewRawImage.texture = videoInput.GetVideoTexture();
        }
    }
}
