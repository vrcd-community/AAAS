using AAAS.LiveCamera.Positions;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace AAAS.LiveCamera.CameraFilters {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public sealed class PlayerTrackingCameraFilter : CameraFilterBase {
        [UdonSynced]
        private bool _enableTracking;
        [UdonSynced]
        private float _headWidth = 10f;
        [UdonSynced]
        private int _trackingPlayerNetworkId;
        
        [PublicAPI]
        public bool _GetEnableTracking() {
            return _enableTracking;
        }
        
        [PublicAPI]
        public void _SetEnableTracking(bool enable) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _enableTracking = enable;
            RequestSerialization();
        }
        
        [PublicAPI]
        public float _GetHeadWidth() {
            return _headWidth;
        }

        [PublicAPI]
        public void _SetHeadWidth(float width) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _headWidth = width;
            RequestSerialization();
        }
        
        [PublicAPI]
        public int _GetTrackingPlayerNetworkId() {
            return _trackingPlayerNetworkId;
        }

        [PublicAPI]
        public void _SetTrackingPlayerNetworkId(int networkId) {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            _trackingPlayerNetworkId = networkId;
            RequestSerialization();
        }

        public override bool _ApplyFilter(Camera liveCamera, CameraPositionBase position) {
            if (!_enableTracking)
                return false;

            var player = VRCPlayerApi.GetPlayerById(_trackingPlayerNetworkId);
            if (!Utilities.IsValid(player) || !player.IsValid()) {
                return false;
            }
            
            var headTrackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            
            var distanceToHead = Vector3.Distance(liveCamera.transform.position, headTrackingData.position);
            
            var fov = 2 * Mathf.Atan2(_headWidth, distanceToHead * 2);

            liveCamera.fieldOfView = fov;
            liveCamera.transform.LookAt(headTrackingData.position);
            
            return false;
        }
    }
}