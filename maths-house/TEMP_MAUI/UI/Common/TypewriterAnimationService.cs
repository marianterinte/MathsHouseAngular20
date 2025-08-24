using System.ComponentModel;

namespace GCMS.MathHouse.UI.Common
{
    public class TypewriterAnimationService : INotifyPropertyChanged
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isAnimating = false;
        private bool _isCompleted = false;
        private Label _currentTargetLabel; // Track which label is currently being animated
        
        // Dictionary to track which messages have been animated for each label
        private readonly Dictionary<Label, HashSet<string>> _animatedMessages = new Dictionary<Label, HashSet<string>>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<bool> AnimationStateChanged; // True = completed, False = animating

        public bool IsAnimating => _isAnimating;
        public bool IsCompleted => _isCompleted;

        public async Task StartTypewriterAnimation(Label targetLabel, string fullText, int typingDelayMs = 10, int punctuationDelayMs = 200)
        {
            System.Diagnostics.Debug.WriteLine($"TypewriterAnimationService: StartTypewriterAnimation called for text: {fullText?.Substring(0, Math.Min(50, fullText?.Length ?? 0))}...");
            
            // Check if this message has already been animated for this label
            if (HasMessageBeenAnimated(targetLabel, fullText))
            {
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Message already animated, showing static text");
                // Just set the text directly without animation
                if (MainThread.IsMainThread)
                {
                    targetLabel.Text = fullText;
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => targetLabel.Text = fullText);
                }
                return;
            }
            
            // If we're already animating the same label with the same text, don't start again
            if (_isAnimating && _currentTargetLabel == targetLabel && targetLabel.Text?.Length > 0)
            {
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Skipping - already animating same label");
                return;
            }

            // If we're already animating a different label, stop the previous animation
            if (_isAnimating && _currentTargetLabel != targetLabel)
            {
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Stopping previous animation on different label");
                StopAnimation();
            }

            // Additional check: if we're already animating and the target is the same, just ignore
            if (_isAnimating && _currentTargetLabel == targetLabel)
            {
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Already animating this label, ignoring duplicate request");
                return;
            }

            // Cancel any previous animation
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();

            _currentTargetLabel = targetLabel;
            _isAnimating = true;
            _isCompleted = false;
            OnAnimationStateChanged(false);

            try
            {
                var currentText = "";
                
                // Clear the label text on the main thread synchronously
                if (MainThread.IsMainThread)
                {
                    targetLabel.Text = "";
                }
                else
                {
                    await MainThread.InvokeOnMainThreadAsync(() => targetLabel.Text = "");
                }

                for (int i = 0; i < fullText.Length; i++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        return;

                    currentText += fullText[i];
                    
                    // Update UI synchronously to prevent double rendering
                    if (MainThread.IsMainThread)
                    {
                        targetLabel.Text = currentText;
                    }
                    else
                    {
                        await MainThread.InvokeOnMainThreadAsync(() => targetLabel.Text = currentText);
                    }

                    // Add a brief pause after punctuation for natural reading flow
                    if (fullText[i] == '.' || fullText[i] == '!' || fullText[i] == '?')
                    {
                        await Task.Delay(punctuationDelayMs, _cancellationTokenSource.Token);
                    }
                    else
                    {
                        await Task.Delay(typingDelayMs, _cancellationTokenSource.Token);
                    }
                }

                // Mark this message as animated for this label
                MarkMessageAsAnimated(targetLabel, fullText);

                // Animation complete
                _isCompleted = true;
                _isAnimating = false;
                _currentTargetLabel = null;
                OnAnimationStateChanged(true);
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Animation completed successfully");
            }
            catch (OperationCanceledException)
            {
                // Animation was cancelled
                _isAnimating = false;
                _currentTargetLabel = null;
                OnAnimationStateChanged(false);
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Animation cancelled");
            }
            catch (Exception ex)
            {
                // Handle any other exceptions gracefully
                _isAnimating = false;
                _isCompleted = false;
                _currentTargetLabel = null;
                OnAnimationStateChanged(false);
                System.Diagnostics.Debug.WriteLine($"TypewriterAnimationService: Animation failed with error: {ex.Message}");
            }
        }

        public void CompleteAnimation(Label targetLabel, string fullText)
        {
            // Only complete if this is the current target label
            if (_currentTargetLabel != targetLabel)
                return;

            _cancellationTokenSource?.Cancel();
            
            if (MainThread.IsMainThread)
            {
                targetLabel.Text = fullText;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => targetLabel.Text = fullText);
            }
            
            // Mark this message as animated
            MarkMessageAsAnimated(targetLabel, fullText);
            
            _isCompleted = true;
            _isAnimating = false;
            _currentTargetLabel = null;
            OnAnimationStateChanged(true);
        }

        public void StopAnimation()
        {
            _cancellationTokenSource?.Cancel();
            _isAnimating = false;
            _isCompleted = false;
            _currentTargetLabel = null;
            OnAnimationStateChanged(false);
        }

        /// <summary>
        /// Checks if a message has already been animated for a specific label
        /// </summary>
        private bool HasMessageBeenAnimated(Label label, string message)
        {
            if (!_animatedMessages.ContainsKey(label))
                return false;
                
            return _animatedMessages[label].Contains(message);
        }

        /// <summary>
        /// Marks a message as animated for a specific label
        /// </summary>
        private void MarkMessageAsAnimated(Label label, string message)
        {
            if (!_animatedMessages.ContainsKey(label))
            {
                _animatedMessages[label] = new HashSet<string>();
            }
            
            _animatedMessages[label].Add(message);
        }

        /// <summary>
        /// Clears the animation history for a specific label or all labels
        /// </summary>
        public void ClearAnimationHistory(Label label = null)
        {
            if (label == null)
            {
                _animatedMessages.Clear();
                System.Diagnostics.Debug.WriteLine("TypewriterAnimationService: Cleared all animation history");
            }
            else if (_animatedMessages.ContainsKey(label))
            {
                _animatedMessages[label].Clear();
                System.Diagnostics.Debug.WriteLine($"TypewriterAnimationService: Cleared animation history for specific label");
            }
        }

        /// <summary>
        /// Forces animation even if the message was previously animated
        /// </summary>
        public async Task ForceTypewriterAnimation(Label targetLabel, string fullText, int typingDelayMs = 10, int punctuationDelayMs = 200)
        {
            // Remove the message from animated history if it exists
            if (_animatedMessages.ContainsKey(targetLabel))
            {
                _animatedMessages[targetLabel].Remove(fullText);
            }
            
            // Now start the animation normally
            await StartTypewriterAnimation(targetLabel, fullText, typingDelayMs, punctuationDelayMs);
        }

        private void OnAnimationStateChanged(bool isCompleted)
        {
            AnimationStateChanged?.Invoke(this, isCompleted);
            OnPropertyChanged(nameof(IsAnimating));
            OnPropertyChanged(nameof(IsCompleted));
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _currentTargetLabel = null;
            _animatedMessages.Clear();
        }
    }
}