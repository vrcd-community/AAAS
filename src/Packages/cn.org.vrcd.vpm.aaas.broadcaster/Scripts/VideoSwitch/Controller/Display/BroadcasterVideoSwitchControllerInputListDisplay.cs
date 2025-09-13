using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class BroadcasterVideoSwitchControllerInputListDisplay : UdonSharpBehaviour {
        [SerializeField] private BroadcasterVideoSwitchController videoSwitchController;

        [SerializeField] private GameObject inputItemTemplatePrefab;

        private void Start() {
            if (!videoSwitchController) {
                Debug.LogError("[VideoSwitchControllerInputListDisplay] No video switch controller assigned. Disabling display.", this);
                enabled = false;
                return;
            }

            if (!inputItemTemplatePrefab) {
                Debug.LogError("[VideoSwitchControllerInputListDisplay] No input item template prefab assigned. Disabling display.", this);
                enabled = false;
                return;
            }

            // Load after broadcaster video switch controller is initialized
            SendCustomEventDelayedFrames(nameof(_UpdateDisplay), 1);
        }

        public void _UpdateDisplay() {
            var videoInputs = videoSwitchController.GetVideoInputs();
            for (var index = 0; index < videoInputs.Length; index++) {
                var itemGameObject = Instantiate(inputItemTemplatePrefab, transform);
                var itemController = itemGameObject.GetComponentInChildren<BroadcasterVideoSwitchControllerInputItem>();

                var text = itemGameObject.GetComponentInChildren<Text>();
                text.text = $"Input #{index}";

                itemController.inputIndex = index;
                itemController.videoSwitchController = videoSwitchController;

                var previewer = itemGameObject.GetComponentInChildren<BroadcasterVideoInputPreviewer>();
                if (!previewer)
                    continue;

                var videoInput = videoInputs[index];
                previewer.videoInput = videoInput;
            }
        }
    }
}
