using UdonSharp;
using UnityEngine;

namespace AAAS.UserPad.Spawn {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SpawnWithKeyboard : UdonSharpBehaviour {
        public PadSpawner padSpawner;

        public KeyCode keyCode;

        private void Update() {
            if (Input.GetKeyDown(keyCode)) {
                padSpawner.TeleportPadToPlayer();
            }
        }
    }
}