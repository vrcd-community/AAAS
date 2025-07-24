using AAAS.Slide.Core;
using JetBrains.Annotations;
using TMPro;
using UdonSharp;
using UnityEngine;

namespace AAAS.Slide.Controller.Display {
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SlideControllerDisplay : UdonSharpBehaviour {
        [SerializeField] protected SlideCore slideCore;

        [SerializeField] [CanBeNull] protected TMP_Text slideVideoUrlText;

        [SerializeField] [CanBeNull] protected TMP_Text errorMessageText;

        [SerializeField] [CanBeNull] protected TMP_Text currentPageText;
        [SerializeField] [CanBeNull] protected TMP_Text totalPagesText;

        [SerializeField] [CanBeNull] protected TMP_Text pageStatusFormatText;

        [SerializeField] [CanBeNull] protected string pageStatusFormat = "{0} / {1}";

        private void Start() {
            if (!slideCore) {
                enabled = false;
                Debug.LogError(
                    "[SlideControllerDisplay] SlideCore is not assigned. Please assign a SlideCore in the inspector.",
                    this);
                return;
            }

            slideCore._AddSlideStatueChangedEventListener(this, nameof(_OnSlideCoreStatusChanged));
            slideCore._AddSlidePageChangedEventListener(this, nameof(_OnSlideCorePageChanged));
        }

        public void _OnSlideCoreStatusChanged() {
            UpdatePageText();
        }

        public void _OnSlideCorePageChanged() {
            UpdateSlideStatusText();
            UpdatePageText();
        }

        [PublicAPI]
        protected void UpdatePageText() {
            var slidePage = (slideCore.SlidePageIndex + 1).ToString();
            var totalPages = slideCore.PageTotal.ToString();

            if (currentPageText)
                currentPageText.text = slidePage;

            if (totalPagesText)
                totalPagesText.text = totalPages;

            if (pageStatusFormatText && !string.IsNullOrWhiteSpace(pageStatusFormat))
                pageStatusFormatText.text = string.Format(pageStatusFormat, slidePage, totalPages);
        }

        [PublicAPI]
        protected void UpdateSlideStatusText() {
            var videoUrl = slideCore.SlideUrl.Get();
            var errorMessage = slideCore.Status == SlideCoreStatus.VideoError
                ? slideCore.LastVideoError.ToString()
                : "";

            if (slideVideoUrlText)
                slideVideoUrlText.text = videoUrl;

            if (errorMessageText)
                errorMessageText.text = errorMessage;
        }
    }
}
