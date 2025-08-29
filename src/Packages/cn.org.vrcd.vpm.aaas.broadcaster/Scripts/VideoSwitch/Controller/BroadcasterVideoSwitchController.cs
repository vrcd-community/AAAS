using AAAS.Broadcaster.VideoSwitch.Core;
using AAAS.Broadcaster.VideoSwitch.Input;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.VideoSwitch.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterVideoSwitchController : UdonSharpBehaviour {
        [SerializeField] private BroadcasterVideoSwitch videoSwitch;

        private void Start() {
            if (!videoSwitch) {
                Debug.LogError("[BroadcasterVideoSwitchController] No video switch assigned. Disabling controller.", this);
                enabled = false;
            }
        }
        
        [PublicAPI]
        public int GetCurrentInputIndex() => videoSwitch.CurrentInputIndex;

        [PublicAPI]
        public BroadcasterVideoInputBase[] GetVideoInputs() => videoSwitch._GetVideoInputs();

        [PublicAPI]
        public BroadcasterVideoSwitch GetVideoSwitch() => videoSwitch;

        [PublicAPI]
        public bool SwitchVideoInput(int index) => videoSwitch._SwitchVideoInput(index);
    }
}
