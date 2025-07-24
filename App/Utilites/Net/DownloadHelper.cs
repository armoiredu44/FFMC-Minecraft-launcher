//god bless programming
namespace Minecraft_launcher
{
    internal static class DownloadHelper
    {
        public static async Task<(bool, AllTypes)> DownloadWithProgressAsync(
            string url,
            string type,
            Action<long>? onProgressUpdate = null,
            Action<double?>? onSpeedUpdate = null,
            Action<bool>? onCorruptionCheck = null,
            Action<long>? onSizeObtained = null,
            string hash = "")
        {
            var Stopwatch = System.Diagnostics.Stopwatch.StartNew();
            double time = 0;

            using (HttpUtility client = new HttpUtility())
            {

                var downloadProgress = new Progress<(long totalReadByte, double? downloadSpeed)>(progress =>
                {
                    if (Stopwatch.Elapsed.TotalMilliseconds - time < 16.6) //delay between UI updates to avoid overloading it
                        return;

                    onProgressUpdate?.Invoke(progress.totalReadByte);

                    if (Stopwatch.Elapsed.TotalMilliseconds - time >= 150 && progress.downloadSpeed != null) //updating text does not need to be as smooth as updating a progress bar
                    {
                        onSpeedUpdate?.Invoke(progress.downloadSpeed);
                        time = Stopwatch.Elapsed.TotalMilliseconds; //there was a mistake
                    }
                });

                var fileCorrupted = new Progress<bool>(corruption =>
                {
                    if (corruption)
                        onCorruptionCheck?.Invoke(true);
                });

                var fileSize = new Progress<long?>(size =>
                {
                    if (size != null)
                    {
                        onSizeObtained?.Invoke((long)size);
                    }
                    
                });

                bool bytesProgressSpecified = true;
                if (onSpeedUpdate == null && onProgressUpdate == null)
                    bytesProgressSpecified = false;

                return await client.GetAsync(url, downloadProgress, fileCorrupted, fileSize, type, bytesProgressSpecified, hash);

            }
        }

        public static async Task<(bool, AllTypes)> DownloadWithProgressAndWriteAsync(
            string url,
            string?fileName,
            string path,
            Action<long>? onProgressUpdate = null,
            Action<double?>? onSpeedUpdate = null,
            Action<bool>? onCorruptionCheck = null,
            Action<long>? onSizeObtained = null,
            string hash = "")
        {
            var Stopwatch = System.Diagnostics.Stopwatch.StartNew();
            double time = 0;

            using (HttpUtility client = new HttpUtility())
            {

                var downloadProgress = new Progress<(long totalReadByte, double? downloadSpeed)>(progress =>
                {
                    if (Stopwatch.Elapsed.TotalMilliseconds - time < 16.6) //delay between UI updates to avoid overloading it
                        return;

                    onProgressUpdate?.Invoke(progress.totalReadByte);

                    if (Stopwatch.Elapsed.TotalMilliseconds - time >= 150 && progress.downloadSpeed != null) //updating text does not need to be as smooth as updating a progress bar
                    {
                        onSpeedUpdate?.Invoke(progress.downloadSpeed);
                        time = Stopwatch.Elapsed.TotalMilliseconds; //there was a mistake
                    }
                });

                var fileCorrupted = new Progress<bool>(corruption =>
                {
                    if (corruption)
                        onCorruptionCheck?.Invoke(true);
                });

                var fileSize = new Progress<long?>(size =>
                {
                    if (size != null)
                    {
                        onSizeObtained?.Invoke((long)size);
                    }

                });

                bool bytesProgressSpecified = true;
                if (onSpeedUpdate == null && onProgressUpdate == null)
                    bytesProgressSpecified = false;

                return await client.GetAndWriteAsync(url, fileName, path, downloadProgress, fileCorrupted, fileSize, bytesProgressSpecified, hash);

            }
        }
    }
}
