using JetBrains.Annotations;
using UdonSharp;

namespace AAAS.UserPad.Spawn {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class SpawnToPosition : UdonSharpBehaviour {
        public PadSpawner padSpawner;
        
        [PublicAPI]
        public void TeleportToPosition() {
            padSpawner.TeleportPadTo(transform.position, transform.rotation);
        }

        public override void Interact() => TeleportToPosition();
    }
}