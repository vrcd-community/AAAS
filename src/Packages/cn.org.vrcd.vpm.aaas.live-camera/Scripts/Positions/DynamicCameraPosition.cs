using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.Positions {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DynamicCameraPosition : CameraPositionBase {
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;

        private void LateUpdate() {
            if (_lastPosition != transform.position || _lastRotation != transform.rotation)
                NotifyPositionChanged();
            
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }
    }
}