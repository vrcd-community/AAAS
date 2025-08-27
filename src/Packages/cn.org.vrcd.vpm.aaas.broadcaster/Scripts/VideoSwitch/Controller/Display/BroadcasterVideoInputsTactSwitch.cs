using AAAS.Broadcaster.VideoSwitch.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoInputsTactSwitch : UdonSharpBehaviour {
        public BroadcasterVideoSwitchController videoSwitchController;
        public int[] inputIndices;

        private int lastIndexOfInputIndex = 0;

        private void Start() {
            if (!videoSwitchController) {
                Debug.LogError("[VideoInputsTactSwitch] Video switch controller is not assigned. Please assign a video switch in the inspector.", this);
                return;
            }

            if (inputIndices == null || inputIndices.Length == 0) {
                Debug.LogError("[VideoInputsTactSwitch] No input indices assigned. Please assign input indices in the inspector.", this);
                return;
            }
        }

        [PublicAPI]
        public void OnTactSwitchPressed() {
            if (!videoSwitchController) {
                Debug.LogError("[VideoInputsTactSwitch] Video switch controller is not assigned. Please assign a video switch in the inspector.", this);
                return;
            }

            if (inputIndices.Length == 0) {
                Debug.LogError("[VideoInputsTactSwitch] No input indices assigned. Please assign input indices in the inspector.", this);
                return;
            }

            var currentInput = videoSwitchController.GetCurrentInputIndex();
            var indexOfInputInArray = FindIndexOfInputInArray(currentInput);

            if (indexOfInputInArray < 0) {
                Debug.LogWarning(
                    $"[VideoInputsTactSwitch] Current input index {currentInput} not found in input indices array. Jumping to first input.",
                    this);
                JumpToInput(0);
                return;
            }

            JumpToNextInput();
        }

        private void JumpToNextInput() {
            var nextIndex = lastIndexOfInputIndex + 1;
            if (nextIndex >= inputIndices.Length) {
                nextIndex = 0; // Loop back to the first input
            }

            lastIndexOfInputIndex = nextIndex;

            videoSwitchController.SwitchVideoInput(inputIndices[nextIndex]);
        }

        private void JumpToInput(int indexOfInputIndex) {
            if (indexOfInputIndex < 0 || indexOfInputIndex >= inputIndices.Length) {
                Debug.LogError($"[VideoInputsTactSwitch] Index {indexOfInputIndex} is out of bounds for input indices array.", this);
                return;
            }

            lastIndexOfInputIndex = indexOfInputIndex;

            videoSwitchController.SwitchVideoInput(inputIndices[indexOfInputIndex]);
        }

        private int FindIndexOfInputInArray(int inputIndex) {
            for (var index = 0; index < inputIndices.Length; index++) {
                var videoInputIndex = inputIndices[index];
                if (videoInputIndex == inputIndex) {
                    return index;
                }
            }

            return -1;
        }
    }
}
