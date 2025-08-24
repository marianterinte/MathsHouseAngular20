using CommunityToolkit.Maui.Views;
using System.Collections.Concurrent;

namespace GCMS.MathHouse;
public static class AudioPlayerService
{
    // Thread-safe collection to track active MediaElements and their associated files
    private static readonly ConcurrentDictionary<MediaElement, string> _activeElements = new();
    
    // Dictionary to track temp file paths for cleanup
    private static readonly ConcurrentDictionary<string, string> _tempFilePaths = new();

    public async static Task LoadAndPlayAudio(MediaElement element, string audioFileName)
    {
        try
        {
            // Stop all currently playing audio before loading new one
            await StopAllAudio();

            var assetPath = audioFileName;
            string tempFilePath = null;

            using (var assetStream = await FileSystem.OpenAppPackageFileAsync(assetPath))
            {
                if (assetStream == null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        throw new Exception($"Nu s-a putut deschide asset-ul audio: {assetPath}"));
                    return;
                }

                var tempFileName = $"{Guid.NewGuid()}_{audioFileName}";
                var cacheDir = FileSystem.CacheDirectory;
                tempFilePath = Path.Combine(cacheDir, tempFileName);

                using (var fileStream = File.OpenWrite(tempFilePath))
                {
                    await assetStream.CopyToAsync(fileStream);
                }
            }

            if (File.Exists(tempFilePath))
            {
                // Track this element and its associated file
                _activeElements[element] = audioFileName;
                _tempFilePaths[audioFileName] = tempFilePath;

                element.Source = MediaSource.FromFile(tempFilePath);
                
                // Subscribe to MediaEnded event to clean up when audio finishes
                element.MediaEnded += OnMediaEnded;
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                throw new Exception($"Nu s-a putut deschide asset-ul audio: {assetPath}"));
            }
        }
        catch (Exception ex)
        {
            // Remove from tracking if loading failed
            _activeElements.TryRemove(element, out _);
        }
    }

    public static async Task StopAudio(MediaElement audioElement)
    {
        if (audioElement == null)
            return;

        try
        {
            audioElement.Stop();
            
            // Clean up tracking for this element
            if (_activeElements.TryRemove(audioElement, out string fileName))
            {
                // Unsubscribe from events
                audioElement.MediaEnded -= OnMediaEnded;
                
                // Clean up temp file if it exists
                if (_tempFilePaths.TryRemove(fileName, out string tempFilePath))
                {
                    CleanupTempFile(tempFilePath);
                }
            }
        }
        catch (Exception ex)
        {
            // Silent fail - audio stopping error
        }
    }

    // Stop all currently active audio elements
    public static async Task StopAllAudio()
    {
        var elementsToStop = _activeElements.Keys.ToList();
        
        foreach (var element in elementsToStop)
        {
            await StopAudio(element);
        }
    }

    // Get list of currently active audio files
    public static IEnumerable<string> GetActiveAudioFiles()
    {
        return _activeElements.Values.ToList();
    }

    // Check if a specific audio file is currently playing
    public static bool IsAudioActive(string audioFileName)
    {
        return _activeElements.Values.Contains(audioFileName);
    }

    private static void OnMediaEnded(object sender, EventArgs e)
    {
        if (sender is MediaElement element)
        {
            // Clean up when media ends naturally
            _ = Task.Run(async () => await StopAudio(element));
        }
    }

    private static void CleanupTempFile(string tempFilePath)
    {
        try
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
        catch
        {
            // Silent fail - file cleanup error
        }
    }

    // Method to clean up all temp files (useful for app cleanup)
    public static void CleanupAllTempFiles()
    {
        var tempFiles = _tempFilePaths.Values.ToList();
        _tempFilePaths.Clear();
        
        foreach (var tempFile in tempFiles)
        {
            CleanupTempFile(tempFile);
        }
    }
}
