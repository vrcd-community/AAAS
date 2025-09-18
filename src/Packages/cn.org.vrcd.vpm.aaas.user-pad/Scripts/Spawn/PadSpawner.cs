using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AAAS.UserPad.Spawn {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PadSpawner : UdonSharpBehaviour {
        public Transform padTransform;

        [PublicAPI]
        public void TeleportPadTo(Vector3 position, Quaternion rotation) {
            padTransform.SetPositionAndRotation(position, rotation);
        }

        [PublicAPI]
        public void TeleportPadToPlayer() {
            var localPlayer = Networking.LocalPlayer;

            var headTrackingData = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            var headPosition = headTrackingData.position;
            var headRotation = headTrackingData.rotation;
            
            var spawnPosition = headPosition + headRotation * Vector3.forward * 0.5f;
            var spawnRotation = Quaternion.Euler(0f, headRotation.eulerAngles.y, 0f);
            
            TeleportPadTo(spawnPosition, spawnRotation);
        }
    }
}