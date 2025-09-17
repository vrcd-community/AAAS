using AAAS.Broadcaster.VideoSwitch.Output;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Bridge {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterBridgeOutput : BroadcasterVideoOutputBase {
        private BroadcasterBridgeInput[] _bridgeInputs = new BroadcasterBridgeInput[0];

        protected override void UpdateOutputTexture(Texture texture) {
            foreach (var broadcasterBridgeInput in _bridgeInputs) {
                if (!broadcasterBridgeInput) {
                    Debug.LogError("[BroadcasterBridgeOutput] Found a null input in the bridge inputs array.");
                    continue;
                }

                broadcasterBridgeInput.UpdateOutputTexture(texture);
            }
        }

        public void RegisterInput(BroadcasterBridgeInput input) {
            if (!input) {
                Debug.LogError("[BroadcasterBridgeOutput] Attempted to register a null input.");
                return;
            }

            var newInputs = new BroadcasterBridgeInput[_bridgeInputs.Length + 1];
            _bridgeInputs.CopyTo(newInputs, 0);
            newInputs[_bridgeInputs.Length] = input;

            _bridgeInputs = newInputs;
        }

        [CanBeNull]
        public Texture GetOutputTexture() {
            return broadcasterSwitch._GetOutputTexture();
        }
    }
}
