using JetBrains.Annotations;
using UdonSharp;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VideoSwitchControllerInputItem : UdonSharpBehaviour {
        [PublicAPI]
        public VideoSwitchController videoSwitchController;

        [PublicAPI]
        public int inputIndex;

        [PublicAPI]
        public void SwitchInput() => videoSwitchController.SwitchVideoInput(inputIndex);

        [PublicAPI]
        public void SetInputIndex(int index) => inputIndex = index;
    }
}
