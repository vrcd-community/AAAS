using AAAS.Broadcaster.AudioControl.Core;
using UdonSharp;
using UnityEngine;

namespace AAAS.Broadcaster.AudioControl.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class BroadcasterAudioUserController : UdonSharpBehaviour {
        [SerializeField] private BroadcasterAudioControlHub controlHub;
    }
}
