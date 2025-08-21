using UdonSharp;
using VRC.SDKBase;

namespace AAAS.Broadcaster.VideoSwitch.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BroadcasterVideoSwitchNetworking : UdonSharpBehaviour {
        [UdonSynced] [FieldChangeCallback(nameof(CurrentInputIndex))]
        private int _currentInputIndex;

        private int CurrentInputIndex {
            get => _currentInputIndex;
            set {
                _currentInputIndex = value;
                _broadcasterSwitch.SendCustomEvent(nameof(BroadcasterVideoSwitch._OnVideoInputIndexChanged));
            }
        }

        private BroadcasterVideoSwitch _broadcasterSwitch;

        internal int GetInputIndex() {
            return _currentInputIndex;
        }

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
