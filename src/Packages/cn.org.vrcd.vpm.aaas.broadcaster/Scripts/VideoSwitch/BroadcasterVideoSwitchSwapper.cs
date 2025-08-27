using AAAS.Broadcaster.VideoSwitch.Controller;
using AAAS.Broadcaster.VideoSwitch.Core;
using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoSwitchSwapper : UdonSharpBehaviour {
        public BroadcasterVideoSwitchController aVideoSwitch;
        public BroadcasterVideoSwitchController bVideoSwitch;

        public bool overrideBWhenInputNotFoundInA = true;

        [PublicAPI]
        public void Swap() {
            if (!aVideoSwitch || !bVideoSwitch) {
                Debug.LogWarning("[BroadcasterVideoSwitchSwap] Swap failed: One or both video switches are not assigned.");
                return;
            }

            var aCurrentInputIndex = aVideoSwitch.GetCurrentInputIndex();
            var bCurrentInputIndex = bVideoSwitch.GetCurrentInputIndex();

            var aVideoInputs = aVideoSwitch.GetVideoInputs();
            var bVideoInputs = bVideoSwitch.GetVideoInputs();

            var bTargetIndex = FindInArray(bVideoInputs, aVideoInputs[aCurrentInputIndex]);
            var aTargetIndex = FindInArray(aVideoInputs, bVideoInputs[bCurrentInputIndex]);

            if (aTargetIndex == -1 && overrideBWhenInputNotFoundInA) {
                if (bTargetIndex == -1) {
                    Debug.LogWarning("[BroadcasterVideoSwitchSwap] Swap failed: Input from A not found in B");
                    return;
                }

                bVideoSwitch.SwitchVideoInput(bTargetIndex);
                return;
            }

            if (bTargetIndex == -1 || aTargetIndex == -1) {
                Debug.LogWarning("[BroadcasterVideoSwitchSwap] Swap failed: One of the inputs was not found in the other video switch's inputs.");
                return;
            }

            aVideoSwitch.SwitchVideoInput(aTargetIndex);
            bVideoSwitch.SwitchVideoInput(bTargetIndex);
        }

        private static int FindInArray(BroadcasterVideoInputBase[] array, BroadcasterVideoInputBase value) {
            for (var i = 0; i < array.Length; i++) {
                if (array[i] == value) {
                    return i;
                }
            }
            return -1; // Not found
        }
    }
}
