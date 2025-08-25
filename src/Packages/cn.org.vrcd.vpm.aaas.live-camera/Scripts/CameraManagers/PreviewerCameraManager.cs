using AAAS.LiveCamera.Positions;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.CameraManagers {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PreviewerCameraManager : UdonSharpBehaviour {
        public Camera previewerCamera;

        public CameraPositionsManager cameraPositionsManager;

        public RenderTexture[] previewerTextures;
        
        public float updateInterval = 0.1f;
        
        private int _lastUpdatedCameraPositionIndex = -1;
        private float _lastUpdateTime;

        private void Start() {
            if (!previewerCamera) {
                Debug.LogError("[PreviewerCameraManager] Previewer Camera is not assigned.");
                enabled = false;
                return;
            }
            
            if (!cameraPositionsManager) {
                Debug.LogError("[PreviewerCameraManager] Camera Positions Manager is not assigned.");
                enabled = false;
                return;
            }
            
            if (previewerTextures == null || previewerTextures.Length == 0) {
                Debug.LogError("[PreviewerCameraManager] Previewer Textures are not assigned.");
                enabled = false;
                return;
            }
            
            if (cameraPositionsManager.cameraPositions.Length != previewerTextures.Length) {
                Debug.LogError("[PreviewerCameraManager] The number of Camera Positions must match the number of Previewer Textures.");
                enabled = false;
                return;
            }
        }

        private void LateUpdate() {
            if (Time.time - _lastUpdateTime < updateInterval)
                return;
            
            var cameraPositions = cameraPositionsManager.cameraPositions;
            
            var updateCameraPositionIndex = _lastUpdatedCameraPositionIndex + 1;
            if (updateCameraPositionIndex >= cameraPositions.Length) {
                updateCameraPositionIndex = 0;
            }
            
            _lastUpdatedCameraPositionIndex = updateCameraPositionIndex;

            var cameraPosition = cameraPositionsManager.cameraPositions[updateCameraPositionIndex];
            var cameraPositionTransform = cameraPosition._GetCameraTransform();
            
            previewerCamera.transform.position = cameraPositionTransform.position;
            previewerCamera.transform.rotation = cameraPositionTransform.rotation;
            
            previewerCamera.targetTexture = previewerTextures[updateCameraPositionIndex];
        }
    }
}