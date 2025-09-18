using JetBrains.Annotations;
using UdonSharp;

namespace AAAS.UserPad.Router {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public abstract class PadRouterMiddleware : UdonSharpBehaviour {
        public virtual bool _OnBeforeResolve(string key, PadRouter router) => true;
        public virtual bool _OnBeforeNavigated(string key, PadRouter router, PadPageMetadata pageMetadata) => true;
        public virtual void _OnNavigated(string key, PadRouter router, PadPageMetadata padPageMetadata) { }
    }
}