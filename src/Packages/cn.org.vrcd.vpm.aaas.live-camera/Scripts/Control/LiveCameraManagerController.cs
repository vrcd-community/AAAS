using System;
using AAAS.LiveCamera.CameraManagers;
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
        
        public void ChangeCameraPosition(int index) {
            if (!liveCameraManager) {
                Debug.LogError("[LiveCameraManagerController] LiveCameraManager is null", this);
                return;
            }

            liveCameraManager.ChangeCameraPosition(index);
        }
    }
}