using AAAS.LiveCamera.CameraManagers;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.Control {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PreviewerCameraManagerController : UdonSharpBehaviour {
        public PreviewerCameraManager previewerCameraManager;

        private void Start() {
            if (!previewerCameraManager) {
                Debug.LogError("[PreviewerCameraManager] PreviewerCameraManager is not assigned.");
                enabled = false;
                return;
            }
        }
        
        [PublicAPI]
        public void _SetEnablePreviewUpdate(bool enable) {
            previewerCameraManager.enablePreviewUpdate = enable;
        }

        [PublicAPI]
        public void _ToggleEnablePreviewUpdate() {
            _SetEnablePreviewUpdate(!previewerCameraManager.enablePreviewUpdate);
        }
    }
}