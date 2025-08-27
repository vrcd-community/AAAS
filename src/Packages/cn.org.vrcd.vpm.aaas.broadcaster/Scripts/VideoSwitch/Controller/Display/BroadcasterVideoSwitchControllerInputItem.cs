using JetBrains.Annotations;
using UdonSharp;

namespace AAAS.Broadcaster.VideoSwitch.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoSwitchControllerInputItem : UdonSharpBehaviour {
        [PublicAPI]
        public BroadcasterVideoSwitchController videoSwitchController;

        [PublicAPI]
        public int inputIndex;

        [PublicAPI]
        public void SwitchInput() => videoSwitchController.SwitchVideoInput(inputIndex);
    }
}
