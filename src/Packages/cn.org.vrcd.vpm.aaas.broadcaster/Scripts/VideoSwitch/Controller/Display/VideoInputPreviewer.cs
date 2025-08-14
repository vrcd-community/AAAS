using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
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

        public void OnVideoTextureChanged() {
            previewRawImage.texture = videoInput.GetVideoTexture();
        }
    }
}
