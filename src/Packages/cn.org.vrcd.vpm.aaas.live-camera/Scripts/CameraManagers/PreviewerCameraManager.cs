using AAAS.LiveCamera.CameraFilters;
using AAAS.LiveCamera.Positions;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.CameraManagers {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PreviewerCameraManager : UdonSharpBehaviour {
        public Camera referenceCamera;
        private Camera _previewerCamera;

        public CameraPositionsManager cameraPositionsManager;

        public RenderTexture[] previewerTextures;
        
        public CameraFilterBase[] cameraFilters;
        
        public bool enablePreviewUpdate;
        
        private int _lastUpdatedCameraPositionIndex = -1;

        private void Start() {
            if (!referenceCamera) {
                Debug.LogError("[PreviewerCameraManager] Reference Camera is not assigned.");
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
            
            referenceCamera.gameObject.SetActive(false);
            referenceCamera.enabled = false;
            
            _previewerCamera = Instantiate(referenceCamera.gameObject).GetComponent<Camera>();
            
            _previewerCamera.enabled = true;
            _previewerCamera.gameObject.SetActive(true);

            _previewerCamera.CopyFrom(referenceCamera);
        }

        private void LateUpdate() {
            if (!enablePreviewUpdate)
                return;
            
            var cameraPositions = cameraPositionsManager.cameraPositions;
            
            var updateCameraPositionIndex = _lastUpdatedCameraPositionIndex + 1;
            if (updateCameraPositionIndex >= cameraPositions.Length) {
                updateCameraPositionIndex = 0;
            }
            
            _lastUpdatedCameraPositionIndex = updateCameraPositionIndex;

            var cameraPosition = cameraPositionsManager.cameraPositions[updateCameraPositionIndex];
            UpdateCamera(cameraPosition);
            
            _previewerCamera.targetTexture = previewerTextures[updateCameraPositionIndex];
        }
        
        private void UpdateCamera(CameraPositionBase position) {
            _previewerCamera.CopyFrom(referenceCamera);

            var positionTransform = position._GetCameraTransform();
            
            _previewerCamera.transform.SetPositionAndRotation(positionTransform.position, positionTransform.rotation);
            
            foreach (var filter in cameraFilters) {
                if (filter._ApplyFilter(_previewerCamera, position))
                    break;
            }
        }
    }
}