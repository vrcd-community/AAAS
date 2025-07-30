using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class VideoSwitchControllerInputListDisplay : UdonSharpBehaviour {
        [SerializeField] private VideoSwitchController videoSwitchController;

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

            var videoInputs = videoSwitchController.GetVideoInputs();
            for (var index = 0; index < videoInputs.Length; index++) {
                var itemGameObject = Instantiate(inputItemTemplatePrefab, transform);
                var itemController = itemGameObject.GetComponentInChildren<VideoSwitchControllerInputItem>();

                var text = itemGameObject.GetComponentInChildren<Text>();
                text.text = $"Input #{index}";

                itemController.SetInputIndex(index);
                itemController.videoSwitchController = videoSwitchController;
            }
        }
    }
}
