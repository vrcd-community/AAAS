using AAAS.LiveCamera.Positions;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;

namespace AAAS.LiveCamera.CameraManagers {
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public sealed class LiveCameraManager : UdonSharpBehaviour {
        public Camera liveCamera;

        public CameraPositionsManager cameraPositionsManager;

        [UdonSynced]
        [FieldChangeCallback(nameof(CurrentCameraPositionIndex))]
        private int _currentCameraPositionIndex;

        public int CurrentCameraPositionIndex {
            get => _currentCameraPositionIndex;
            private set {
                if (value < 0 || value >= cameraPositionsManager.cameraPositions.Length) {
                    Debug.LogWarning("[LiveCameraManager] Set CurrentInputIndex: Index out of range", this);
                    return;
                }

                _currentCameraPositionIndex = value;
                ChangeCameraPosition(_currentCameraPositionIndex);
            }
        }

        private void Start() {
            if (!liveCamera) {
                Debug.LogError("[LiveCameraManager] LiveCamera not found", this);
                enabled = false;
                return;
            }
            
            if (!cameraPositionsManager) {
                Debug.LogError("[LiveCameraManager] CameraPositionsManager not found", this);
                enabled = false;
                return;
            }
            
            if (cameraPositionsManager.cameraPositions.Length == 0) {
                Debug.LogError("[LiveCameraManager] No camera positions found in CameraPositionsManager", this);
                enabled = false;
                return;
            }

            ChangeCameraPositionInternal(0);

            for (var index = 0; index < cameraPositionsManager.cameraPositions.Length; index++) {
                var cameraPosition = cameraPositionsManager.cameraPositions[index];
                if (!cameraPosition) {
                    Debug.LogError("[LiveCameraManager] One or more camera positions are null or invalid", this);
                    continue;
                }

                cameraPosition.RegisterPositionChangedEvent(this, nameof(_OnCameraPositionChanged), index.ToString());
            }
        }

        [PublicAPI]
        public CameraPositionBase[] GetCameraPositions() {
            var newArray = new CameraPositionBase[cameraPositionsManager.cameraPositions.Length];
            cameraPositionsManager.cameraPositions.CopyTo(newArray, 0);
            return newArray;
        }

        [PublicAPI]
        public bool ChangeCameraPosition(int index) {
            if (index < 0 || index >= cameraPositionsManager.cameraPositions.Length) {
                Debug.LogWarning("[LiveCameraManager] ChangeCameraPosition: Index out of range", this);
                return false;
            }

            var position = cameraPositionsManager.cameraPositions[index];

            if (!position) {
                Debug.LogWarning(
                    "[LiveCameraManager] ChangeCameraPosition: Position not found (null or invalid object)", this);
                return false;
            }

            var positionTransform = position._GetCameraTransform();

            TakeOwnership();
            _currentCameraPositionIndex = index;
            RequestSerialization();
            
            liveCamera.transform.SetPositionAndRotation(positionTransform.position, positionTransform.rotation);
            return true;
        }

        private void ChangeCameraPositionInternal(int index) {
            var positionTransform = cameraPositionsManager.cameraPositions[index]._GetCameraTransform();
            liveCamera.transform.SetPositionAndRotation(positionTransform.position, positionTransform.rotation);
        }

        // "a network event which should only be called by the local player"
        [NetworkCallable(
            maxEventsPerSecond:
            1)] // won't affect local "network" event call, see https://creators.vrchat.com/worlds/udon/networking/events#rate-limiting
        public void _OnCameraPositionChanged(string nonce) {
            if (NetworkCalling.CallingPlayer == null) {
                Debug.LogWarning(
                    "[LiveCameraManager] Video Input Texture Changed received with no calling player, ignoring.",
                    this);
                return;
            }

            if (!NetworkCalling.CallingPlayer.isLocal) {
                Debug.LogWarning(
                    "[LiveCameraManager] Video Input Texture Changed received from non-local player, ignoring.",
                    this);
                return;
            }

            if (!int.TryParse(nonce, out var inputIndex)) {
                Debug.LogError($"[LiveCameraManager] Invalid nonce received: {nonce}. Expected an integer index.",
                    this);
                return;
            }

            var cameraPositions = cameraPositionsManager.cameraPositions;

            if (inputIndex < 0 || inputIndex >= cameraPositions.Length) {
                Debug.LogError(
                    $"[LiveCameraManager] Received a _OnCameraPositionChanged event with index {inputIndex} is out of bounds for position array. Must be between 0 and {cameraPositions.Length - 1}.",
                    this);
                return;
            }

            if (CurrentCameraPositionIndex == inputIndex)
                ChangeCameraPositionInternal(CurrentCameraPositionIndex);
        }
        
        private void TakeOwnership() {
            if (!Networking.IsOwner(gameObject)) {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
        }
    }
}