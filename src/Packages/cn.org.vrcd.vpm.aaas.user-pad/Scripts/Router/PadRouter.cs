using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace AAAS.UserPad.Router {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [PublicAPI]
    public sealed class PadRouter : UdonSharpBehaviour {
        [SerializeField] private PadRouterMiddleware[] _middlewares = new PadRouterMiddleware[0];
        [SerializeField] private PadPageMetadata[] _pageMetadata = new PadPageMetadata[0];

        [SerializeField] public Transform _pageContainer;

        [SerializeField] private string defaultKey;

        [PublicAPI]
        public string CurrentKey { get; private set; }

        [PublicAPI]
        public PadPageMetadata CurrentPage { get; private set; }

        private void Start() {
            if (string.IsNullOrEmpty(defaultKey))
                return;

            foreach (var metadata in _pageMetadata) {
                metadata.page.SetActive(false);
            }
            
            Navigate(defaultKey);
        }

        [PublicAPI]
        public void Navigate(string key) {
            if (string.IsNullOrEmpty(key))
                return;

            foreach (var middleware in _middlewares) {
                if (!middleware._OnBeforeResolve(key, this))
                    return;
            }

            foreach (var pageMetadata in _pageMetadata) {
                if (pageMetadata.key != key)
                    continue;

                NavigateCore(pageMetadata);
                return;
            }
        }

        private void NavigateCore(PadPageMetadata pageMetadata) {
            var key = pageMetadata.key;
            foreach (var middleware in _middlewares) {
                if (!middleware._OnBeforeNavigated(key, this, pageMetadata))
                    return;
            }

            if (CurrentPage) {
                CurrentPage.page.SetActive(false);
            }

            pageMetadata.page.SetActive(true);
            pageMetadata.page.transform.SetParent(_pageContainer);
            pageMetadata.page.transform.localPosition = Vector3.zero;
            pageMetadata.page.transform.localRotation = Quaternion.identity;
            pageMetadata.page.transform.localScale = Vector3.one;

            CurrentKey = key;
            CurrentPage = pageMetadata;

            foreach (var middleware in _middlewares) {
                middleware._OnNavigated(key, this, pageMetadata);
            }
        }
    }
}