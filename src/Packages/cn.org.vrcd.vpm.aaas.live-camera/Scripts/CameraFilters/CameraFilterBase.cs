using AAAS.LiveCamera.Positions;
using UdonSharp;
using UnityEngine;

namespace AAAS.LiveCamera.CameraFilters {
    public abstract class CameraFilterBase : UdonSharpBehaviour {
        public abstract bool _ApplyFilter(Camera liveCamera, CameraPositionBase position);
    }
}