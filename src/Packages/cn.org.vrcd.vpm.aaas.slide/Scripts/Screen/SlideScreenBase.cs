using AAAS.Slide.Core;
using UdonSharp;
using UnityEngine;

namespace AAAS.Slide.Screen {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class SlideScreenBase : UdonSharpBehaviour {
        [SerializeField] protected SlideCore slideCore;

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError("[SlideScreenBase] SlideCore is not assigned. Please assign a SlideCore in the inspector.", this);
                return;
            }

            var playerTexture = slideCore._GetSlidePlayerTexture();

            SetScreenTexture(playerTexture);
        }

        protected virtual void SetScreenTexture(Texture texture) {
            Debug.LogWarning("[SlideScreenBase] SetScreenTexture is not implemented. Please override this method in a derived class.", this);
        }
    }
}
