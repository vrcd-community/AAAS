using AAAS.LiveCamera.Positions;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.CameraManagers {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PreviewerCameraManager : UdonSharpBehaviour {
        public Camera previewerCamera;

        [CanBeNull]
        public LiveCameraManager liveCameraManager;

        public CameraPositionsManager cameraPositionsManager;

        public RenderTexture[] previewerTextures;
    }
}