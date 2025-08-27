using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.Control {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class ChangeLiveCameraPosition : UdonSharpBehaviour {
        public LiveCameraManagerController liveCameraManager;
        
        public int cameraPositionIndex = 0;
        
        [PublicAPI]
        public void ChangePosition() {
            if (!liveCameraManager) {
                Debug.LogError("[ChangeLiveCameraPosition] LiveCameraManager is null", this);
                return;
            }
            
            liveCameraManager.ChangeCameraPosition(cameraPositionIndex);
        }
    }
}