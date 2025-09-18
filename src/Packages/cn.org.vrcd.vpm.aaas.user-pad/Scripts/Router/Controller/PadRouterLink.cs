using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.UserPad.Router.Controller {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class PadRouterLink : UdonSharpBehaviour {
        public PadRouter padRouter;

        [CanBeNull] public string key;
        [CanBeNull] public PadPageMetadata padPageMetadata;
        
        [PublicAPI]
        public void Navigate() {
            if (!padRouter) {
                Debug.LogError("[PadRouterLink] PadRouter is not assigned.", this);
                return;
            }
            
            if (padPageMetadata) {
                padRouter.Navigate(padPageMetadata.key);
                return;
            }
            
            if (!string.IsNullOrEmpty(key)) {
                padRouter.Navigate(key);
                return;
            }
            
            Debug.LogError("[PadRouterLink] Neither key nor padPageMetadata is assigned.", this);
        }
    }
}