using System;
using AAAS.Slide.Tools;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;

namespace AAAS.Slide.Core {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public sealed class SlideCoreOutput : UdonSharpBehaviour {
        [PublicAPI]
        public SlideCore _GetSlideCore() => slideCore;

        [SerializeField] private SlideCore slideCore;

        [SerializeField] [CanBeNull] private Texture noSlidePlayingTexture;

        [SerializeField] [CanBeNull] private Texture defaultVideoErrorTexture;
        [SerializeField] [CanBeNull] private Texture videoLoadingTexture;

        [SerializeField] [CanBeNull] private Texture unknownVideoErrorTexture;
        [SerializeField] [CanBeNull] private Texture invalidUrlErrorTexture;
        [SerializeField] [CanBeNull] private Texture accessDeniedErrorTexture;
        [SerializeField] [CanBeNull] private Texture playerErrorTexture;
        [SerializeField] [CanBeNull] private Texture rateLimitedTexture;

        private UdonSharpBehaviour[] _outputChangedEventListeners = new UdonSharpBehaviour[0];
        private string[] _outputChangedEventNames = new string[0];

        [CanBeNull] private Texture _currentOutputTexture;

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError(
                    "[SlideCoreOutput] SlideCore is not assigned. Please assign a SlideCore in the inspector.", this);
                return;
            }

            slideCore._AddSlideStatueChangedEventListener(this, nameof(_OnSlideCoreStatusChanged));
            _OnSlideCoreStatusChanged();
        }

        public void _OnSlideCoreStatusChanged() => UpdateOutputTexture();

        [PublicAPI]
        public Texture _GetSlideOutputTexture() {
            if (!_currentOutputTexture)
                UpdateOutputTexture();

            return _currentOutputTexture;
        }

        [PublicAPI]
        public bool _AddOutputChangedEventListener(UdonSharpBehaviour listener, string eventName) {
            if (!listener || string.IsNullOrWhiteSpace(eventName)) {
                return false;
            }

            _outputChangedEventListeners = ArrayTools.Add(_outputChangedEventListeners, listener);
            _outputChangedEventNames = ArrayTools.Add(_outputChangedEventNames, eventName);

            return true;
        }

        private void SendOutputChangedEventListener() {
            for (var index = 0; index < _outputChangedEventListeners.Length; index++) {
                var listener = _outputChangedEventListeners[index];
                var eventName = _outputChangedEventNames[index];

                if (!listener) return;

                listener.SendCustomEvent(eventName);
            }
        }

        private void UpdateOutputTexture() {
            UpdateOutputTextureCore();
            SendOutputChangedEventListener();
        }

        private void UpdateOutputTextureCore() {
            // Switch for unnormal status handle
            switch (slideCore.Status) {
                case SlideCoreStatus.NoSlideLoaded:
                    if (noSlidePlayingTexture) {
                        _currentOutputTexture = noSlidePlayingTexture;
                        return;
                    }

                    break;
                case SlideCoreStatus.Loading:
                    if (videoLoadingTexture) {
                        _currentOutputTexture = videoLoadingTexture;
                        return;
                    }

                    break;
                case SlideCoreStatus.VideoError:
                    switch (slideCore.LastVideoError) {
                        case VideoError.InvalidURL:
                            if (invalidUrlErrorTexture) {
                                _currentOutputTexture = invalidUrlErrorTexture;
                                return;
                            }

                            break;
                        case VideoError.AccessDenied:
                            if (accessDeniedErrorTexture) {
                                _currentOutputTexture = accessDeniedErrorTexture;
                                return;
                            }

                            break;
                        case VideoError.PlayerError:
                            if (playerErrorTexture) {
                                _currentOutputTexture = playerErrorTexture;
                                return;
                            }

                            break;
                        case VideoError.RateLimited:
                            if (rateLimitedTexture) {
                                _currentOutputTexture = rateLimitedTexture;
                                return;
                            }

                            break;
                        case VideoError.Unknown:
                            if (unknownVideoErrorTexture) {
                                _currentOutputTexture = unknownVideoErrorTexture;
                                return;
                            }

                            break;
                    }

                    if (defaultVideoErrorTexture) {
                        _currentOutputTexture = defaultVideoErrorTexture;
                        return;
                    }

                    break;
            }

            _currentOutputTexture = slideCore._GetSlidePlayerTexture();
        }
    }
}
