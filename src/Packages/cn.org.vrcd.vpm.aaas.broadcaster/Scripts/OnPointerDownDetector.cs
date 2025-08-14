using UdonSharp;
using UnityEngine;
using VRC.SDK3.Rendering;
using VRC.SDKBase;

namespace AAAS.Broadcaster {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(Collider))]
    public class OnPointerDownDetector : UdonSharpBehaviour {
        private Collider _collider;

        private VRCPlayerApi _player;
        private bool _isVr;

        private bool _isPointerDown;

        private void Start() {
            _collider = GetComponent<Collider>();

            _player = Networking.LocalPlayer;
            _isVr = _player.IsUserInVR();

            if (!_collider) {
                enabled = false;
                Debug.LogError("[OnPointerDownDetector] Collider component is not found.", this);
            }
        }

        private void Update() {
            if (!_isVr)
                return;

            var playerCameraPosition = VRCCameraSettings.ScreenCamera.Position;
            var ray = new Ray(playerCameraPosition, playerCameraPosition - _collider.transform.position);

            if (!_collider.Raycast(ray, out var hitInfo, Mathf.Infinity)) {
                _isPointerDown = false;
                return;
            }

            _isPointerDown = true;
        }

        private void OnMouseDown() {
            _isPointerDown = true;
        }

        private void OnMouseUp() {
            _isPointerDown = false;
        }

        public bool IsPointerDown() => _isPointerDown;
    }
}
