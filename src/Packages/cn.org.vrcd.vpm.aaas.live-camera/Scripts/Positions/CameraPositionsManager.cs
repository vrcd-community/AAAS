using UdonSharp;

namespace AAAS.LiveCamera.Positions {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class CameraPositionsManager : UdonSharpBehaviour {
        public CameraPositionBase[] cameraPositions;
    }
}