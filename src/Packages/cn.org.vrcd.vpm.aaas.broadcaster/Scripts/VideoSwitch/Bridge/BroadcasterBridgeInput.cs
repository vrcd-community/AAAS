using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Bridge {
    public class BroadcasterBridgeInput : BroadcasterVideoInputBase {
        public BroadcasterBridgeOutput bridgeParentOutput;

        [CanBeNull]
        private Texture _outputTexture;

        private void Start() {
            if (!bridgeParentOutput) {
                Debug.LogError("[BroadcasterBridgeInput] No BroadcasterBridgeOutput found. Please ensure the component is attached to the same GameObject.");
                enabled = false;
                return;
            }

            bridgeParentOutput.RegisterInput(this);
            _outputTexture = bridgeParentOutput.GetOutputTexture();
        }

        public void UpdateOutputTexture(Texture texture) {
            _outputTexture = texture;
            NotifyVideoTextureChanged();
        }

        public override Texture GetVideoTexture() {
            return _outputTexture;
        }
    }
}
