using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.UserPad.Router {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public sealed class PadPageMetadata : UdonSharpBehaviour {
        public string key;
        public GameObject page;
    }
}