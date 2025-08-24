namespace GCMS.MathHouse.UI.Common
{
    /// <summary>
    /// Static notification service to handle app-wide events using delegates instead of MessagingCenter
    /// </summary>
    public static class NotificationService
    {
        /// <summary>
        /// Delegate for video end notifications
        /// </summary>
        public delegate void VideoEndDelegate();

        /// <summary>
        /// Event that fires when a video has ended
        /// </summary>
        public static event VideoEndDelegate NotifyVideoEnd;

        /// <summary>
        /// Triggers the video end notification
        /// </summary>
        public static void TriggerVideoEnd()
        {
            NotifyVideoEnd?.Invoke();
        }
    }
}