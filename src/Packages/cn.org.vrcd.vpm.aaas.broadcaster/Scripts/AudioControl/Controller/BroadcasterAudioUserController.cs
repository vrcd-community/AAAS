using AAAS.Broadcaster.AudioControl.Core;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterAudioUserController : UdonSharpBehaviour {
        [SerializeField] private BroadcasterAudioControlHub controlHub;

        private void Start() {
            if (!controlHub) {
                Debug.LogError("[BroadcasterAudioUserController] No control hub assigned", this);
            }
        }

        public BroadcasterAudioControlHub GetControlHub() {
            if (!controlHub) {
                Debug.LogError("[BroadcasterAudioUserController] No control hub assigned", this);
            }
            
            return controlHub;
        }
    }
}
