using JetBrains.Annotations;
using UdonSharp;
using VRC.SDKBase;

namespace AAAS.Broadcaster.VideoSwitch.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BroadcasterVideoSwitchNetworking : UdonSharpBehaviour {
        [UdonSynced] [FieldChangeCallback(nameof(CurrentInputIndex))]
        private int _currentInputIndex;

        [PublicAPI]
        internal int CurrentInputIndex {
            get => _currentInputIndex;
            private set {
                _currentInputIndex = value;
                _broadcasterSwitch.SendCustomEvent(nameof(BroadcasterVideoSwitch._OnVideoInputIndexChanged));
            }
        }

        private BroadcasterVideoSwitch _broadcasterSwitch;

        internal void SetInputIndex(int index) {
            TakeOwnership();
            _currentInputIndex = index;
            RequestSerialization();
        }

        internal void Initialize(BroadcasterVideoSwitch broadcasterSwitch) {
            _broadcasterSwitch = broadcasterSwitch;
        }

        private void TakeOwnership() {
            if (Networking.GetOwner(gameObject).isLocal)
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
    }
}
