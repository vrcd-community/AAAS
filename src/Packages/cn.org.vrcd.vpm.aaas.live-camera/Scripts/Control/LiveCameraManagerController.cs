using System;
using AAAS.LiveCamera.CameraManagers;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.Control {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class LiveCameraManagerController : UdonSharpBehaviour {
        public LiveCameraManager liveCameraManager;

        private void Start() {
            if (!liveCameraManager) {
                Debug.LogError("[LiveCameraManagerController] LiveCameraManager not found", this);
            }
        }
        
        [PublicAPI]
        public void ChangeCameraPosition(int index) {
            if (!liveCameraManager) {
                Debug.LogError("[LiveCameraManagerController] LiveCameraManager is null", this);
                return;
            }

            liveCameraManager.ChangeCameraPosition(index);
        }
        
        [PublicAPI]
        public void SetCameraEnabled(bool isEnabled) {
            if (!liveCameraManager) {
                Debug.LogError("[LiveCameraManagerController] LiveCameraManager is null", this);
                return;
            }

            liveCameraManager.enableCamera = isEnabled;
        }
        
        [PublicAPI]
        public void ToggleCameraEnabled() {
            if (!liveCameraManager) {
                Debug.LogError("[LiveCameraManagerController] LiveCameraManager is null", this);
                return;
            }

            liveCameraManager.enableCamera = !liveCameraManager.enableCamera;
        }
    }
}