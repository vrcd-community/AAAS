using AAAS.Slide.Core;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;

namespace AAAS.Slide.Screen {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class SlideScreenBase : UdonSharpBehaviour {
        private SlideCore _slideCore;

        [SerializeField] protected SlideCoreOutput slideCoreOutput;

        [SerializeField] [CanBeNull] protected Texture overrideNoSlidePlayingTexture;

        [SerializeField] [CanBeNull] protected Texture overrideDefaultVideoErrorTexture;
        [SerializeField] [CanBeNull] protected Texture overrideVideoLoadingTexture;

        [SerializeField] [CanBeNull] protected Texture overrideUnknownVideoErrorTexture;
        [SerializeField] [CanBeNull] protected Texture overrideInvalidUrlErrorTexture;
        [SerializeField] [CanBeNull] protected Texture overrideAccessDeniedErrorTexture;
        [SerializeField] [CanBeNull] protected Texture overridePlayerErrorTexture;
        [SerializeField] [CanBeNull] protected Texture overrideRateLimitedTexture;

        private void Start() {
            if (!slideCoreOutput) {
                enabled = false;
                Debug.LogError(
                    "[SlideScreenBase] SlideCoreOutput is not assigned. Please assign a SlideCore in the inspector.",
                    this);
                return;
            }

            _slideCore = slideCoreOutput._GetSlideCore();
            slideCoreOutput._AddOutputChangedEventListener(this, nameof(_OnSlideCoreOutputChanged));

            UpdateSlideScreenTexture();
        }

        public void _OnSlideCoreOutputChanged() => UpdateSlideScreenTexture();

        private void UpdateSlideScreenTexture() {
            var playerTexture = slideCoreOutput._GetSlideOutputTexture();
            SetScreenTexture(playerTexture, _slideCore.Status, _slideCore.LastVideoError);
        }

        [PublicAPI]
        protected virtual void SetScreenTexture(Texture texture) {
            Debug.LogWarning(
                "[SlideScreenBase] SetScreenTexture is not implemented. Please override this method in a derived class.",
                this);
        }

        [PublicAPI]
        protected virtual void SetScreenTexture(Texture texture, SlideCoreStatus coreStatus,
            VideoError videoError = VideoError.Unknown) {
            var screenTexture = GetScreenTexture(texture, coreStatus, videoError);

            SetScreenTexture(screenTexture);
        }

        [PublicAPI]
        protected Texture GetScreenTexture(Texture outputTexture, SlideCoreStatus coreStatus, VideoError videoError) {
            switch (coreStatus) {
                case SlideCoreStatus.NoSlideLoaded:
                    if (overrideNoSlidePlayingTexture) {
                        return overrideNoSlidePlayingTexture;
                    }

                    break;
                case SlideCoreStatus.Loading:
                    if (overrideVideoLoadingTexture) {
                        return overrideVideoLoadingTexture;
                    }

                    break;
                case SlideCoreStatus.VideoError:
                    switch (videoError) {
                        case VideoError.InvalidURL:
                            if (overrideInvalidUrlErrorTexture) {
                                return overrideInvalidUrlErrorTexture;
                            }

                            break;
                        case VideoError.AccessDenied:
                            if (overrideAccessDeniedErrorTexture) {
                                return overrideAccessDeniedErrorTexture;
                            }

                            break;
                        case VideoError.PlayerError:
                            if (overridePlayerErrorTexture) {
                                return overridePlayerErrorTexture;
                            }

                            break;
                        case VideoError.RateLimited:
                            if (overrideRateLimitedTexture) {
                                return overrideRateLimitedTexture;
                            }

                            break;
                        case VideoError.Unknown:
                            if (overrideUnknownVideoErrorTexture) {
                                return overrideUnknownVideoErrorTexture;
                            }

                            break;
                    }

                    if (overrideDefaultVideoErrorTexture) {
                        return overrideDefaultVideoErrorTexture;
                    }

                    break;
            }

            return outputTexture;
        }
    }
}
