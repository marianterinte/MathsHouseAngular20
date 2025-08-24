using System;
using System.Threading.Tasks;

namespace GCMS.MathHouse.UI.Common
{
    /// <summary>
    /// Service to coordinate user feedback display without using Task.Delay
    /// </summary>
    public class FeedbackCoordinationService
    {
        /// <summary>
        /// Shows feedback to user and coordinates subsequent actions without blocking
        /// </summary>
        public void ShowFeedbackAndNavigate(Action showFeedbackAction, int delayMs, Func<Task> navigationAction)
        {
            if (showFeedbackAction == null || navigationAction == null) return;

            // Show feedback immediately
            showFeedbackAction();

            // Use Device Timer for non-blocking coordination
            Device.StartTimer(TimeSpan.FromMilliseconds(delayMs), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await navigationAction();
                });
                return false; // Don't repeat
            });
        }

        /// <summary>
        /// Shows feedback with character change and then navigates
        /// </summary>
        public void ShowFeedbackSequenceAndNavigate(
            Action initialFeedbackAction, 
            int firstDelayMs,
            Action characterChangeAction,
            int secondDelayMs,
            Func<Task> navigationAction)
        {
            if (initialFeedbackAction == null || characterChangeAction == null || navigationAction == null) return;

            // Show initial feedback
            initialFeedbackAction();

            // First timer for character change
            Device.StartTimer(TimeSpan.FromMilliseconds(firstDelayMs), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    characterChangeAction();

                    // Second timer for navigation
                    Device.StartTimer(TimeSpan.FromMilliseconds(secondDelayMs), () =>
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await navigationAction();
                        });
                        return false; // Don't repeat
                    });
                });
                return false; // Don't repeat
            });
        }

        /// <summary>
        /// Coordinates tutorial display without blocking
        /// </summary>
        public void ShowTutorialAfterSettle(int settleDelayMs, Func<Task> tutorialAction)
        {
            if (tutorialAction == null) return;

            Device.StartTimer(TimeSpan.FromMilliseconds(settleDelayMs), () =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await tutorialAction();
                });
                return false; // Don't repeat
            });
        }
    }
}