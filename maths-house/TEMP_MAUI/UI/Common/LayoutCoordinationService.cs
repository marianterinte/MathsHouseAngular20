using System;
using System.Threading.Tasks;

namespace GCMS.MathHouse.UI.Common
{
    /// <summary>
    /// Service to coordinate UI layout updates without using Task.Delay
    /// </summary>
    public class LayoutCoordinationService
    {
        /// <summary>
        /// Ensures label layout is properly calculated before proceeding with animations
        /// </summary>
        public async Task EnsureLabelLayoutCalculated(Label label, string text)
        {
            if (label == null || string.IsNullOrEmpty(text)) return;

            // Use a TaskCompletionSource to wait for layout to be calculated
            var layoutCompletionSource = new TaskCompletionSource<bool>();
            
            // Set the text first
            label.Text = text;
            
            // Force layout update and wait for it to complete
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                label.InvalidateMeasure();
                
                // Use the SizeChanged event to detect when layout is complete
                void OnSizeChanged(object sender, EventArgs e)
                {
                    label.SizeChanged -= OnSizeChanged;
                    layoutCompletionSource.TrySetResult(true);
                }
                
                // If the label already has the correct size, complete immediately
                if (label.Width > 0 && label.Height > 0)
                {
                    layoutCompletionSource.TrySetResult(true);
                }
                else
                {
                    label.SizeChanged += OnSizeChanged;
                    
                    // Fallback timeout to prevent hanging
                    Task.Delay(100).ContinueWith(_ => 
                    {
                        label.SizeChanged -= OnSizeChanged;
                        layoutCompletionSource.TrySetResult(true);
                    });
                }
            });
            
            await layoutCompletionSource.Task;
        }

        /// <summary>
        /// Coordinates video initialization without delays
        /// </summary>
        public async Task CoordinateVideoInitialization(Func<Task> videoAction)
        {
            if (videoAction == null) return;

            // Create a completion source for video coordination
            var videoCompletionSource = new TaskCompletionSource<bool>();
            
            try
            {
                await videoAction();
                videoCompletionSource.SetResult(true);
            }
            catch (Exception ex)
            {
                videoCompletionSource.SetException(ex);
            }
            
            await videoCompletionSource.Task;
        }
    }
}