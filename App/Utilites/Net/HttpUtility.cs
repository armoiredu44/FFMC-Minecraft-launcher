using System.Diagnostics;
using System.IO;
using System.Net.Http;
public class HttpUtility : Utilities, IDisposable
{
	private HttpClient? client;

    private bool _disposed = false;

	public HttpUtility()
	{
		client = new HttpClient();
	}
	
    public async Task<(bool, AllTypes)> GetAsync(string url, IProgress<(long, double?)> downloadProgress, IProgress<bool> fileCorruted, IProgress<long?> size, string type = "byte[]", bool progressSpecified = true, string hash = "")
    {
        if (_disposed)
            throw new ObjectDisposedException("HttpUtility");

        using (HttpClient httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debugger.SendError($"Couldn't get a valid response from {url}, got exception {ex}.");
                return (false, new AllTypes("", ""));
            }

            if (response.Content.Headers.ContentLength.HasValue)
            {
                long fileSize = response.Content.Headers.ContentLength.Value;
                size.Report(fileSize);
                //Debugger.SendInfo("Size is " + fileSize);
            }
            else
                size.Report(null);

            var stream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[8192];
            using var memoryStream = new MemoryStream();
            string computedHash = "";

            #region progress spcified
            if (progressSpecified)
            {
                List<AllTypes> recordedBytes = new List<AllTypes>();
                var stopWatch = Stopwatch.StartNew();
                long totalReadBytes = 0;

                int bytesRead;
                #region hash specified
                if (String.IsNullOrEmpty(hash))
                {
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);

                        totalReadBytes += bytesRead;

                        double seconds = stopWatch.Elapsed.TotalSeconds;

                        recordedBytes.Add(new AllTypes(seconds, totalReadBytes));

                        bool hadEnoughElements = false;
                        double speed = 0.0d;

                        for (int i = recordedBytes.Count - 1;  i > 0; i--) //this calculates the speed over a 1 sec span
                        {
                            double bytes = totalReadBytes - Convert.ToDouble(recordedBytes[i].Value);
                            if (seconds - (double)recordedBytes[i].Type > 1)
                            {
                                speed = getSpeedMBpS(bytes, 1.0d);
                                hadEnoughElements = true;
                                recordedBytes.RemoveRange(0, (int)(i*0.9)); //removes 90% (not 100% cuz margin error) of what's prior to the current index, because it's useless
                                break;

                            }
                        }
                        if (!hadEnoughElements)
                        {
                            speed = getSpeedMBpS(totalReadBytes, seconds);
                        }

                        downloadProgress.Report((totalReadBytes, speed));
                    }

                }
                #endregion hash unspecified
                #region hash specified
                else
                {
                    var hasher = new HashChecker.IncrementalHasher();
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);

                        hasher.AddBlock(buffer, bytesRead);

                        totalReadBytes += bytesRead;

                        double seconds = stopWatch.Elapsed.TotalSeconds;

                        recordedBytes.Add(new AllTypes(seconds, totalReadBytes));

                        bool hadEnoughElements = false;
                        double speed = 0.0d;

                        for (int i = recordedBytes.Count - 1; i > 0; i--) //this calculates the speed over a 1 sec span
                        {
                            double bytes = totalReadBytes - Convert.ToDouble(recordedBytes[i].Value);
                            if (seconds - (double)recordedBytes[i].Type > 1)
                            {
                                speed = getSpeedMBpS(bytes, 1.0d);
                                hadEnoughElements = true;
                                recordedBytes.RemoveRange(0, (int)(i * 0.9)); //removes 90% (not 100% cuz margin error) of what's prior to the current index, because it's useless
                                break;

                            }
                        }
                        if (!hadEnoughElements)
                        {
                            speed = getSpeedMBpS(totalReadBytes, seconds);
                        }

                        downloadProgress.Report((totalReadBytes, speed));
                    }
                    computedHash = hasher.FinalizeHash();
                }
                #endregion hash specified
                recordedBytes.Clear();
            }
            #endregion progess specified
            #region progress unspecified
            else //not what I should do ? Should I just use something else since I don't need to do something between the start and end of the download ?
            {
                int bytesRead;
                #region hash unspecifed
                if (string.IsNullOrEmpty(hash))
                {
                    while ((bytesRead =  await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                }
                #endregion hash unspecified
                #region hash specified
                else
                {
                    var hasher = new HashChecker.IncrementalHasher();
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                        hasher.AddBlock(buffer, bytesRead);
                    }
                    computedHash = hasher.FinalizeHash();
                }
                #endregion hash unspecified
            }
            #endregion progress unspecified

            if (!String.IsNullOrEmpty(hash))
            {

                bool isHashCorrect = computedHash == hash;
                Debugger.SendError($"comparing {computedHash} and {hash}. Are they the same ? {isHashCorrect}");
                if (!isHashCorrect)
                {
                    Debugger.SendError($"File downloaded from {url} is corrupted");
                    fileCorruted.Report(true);
                    return (false, new AllTypes("", ""));

                }
                    Debugger.SendInfo("hash is correct");
                    fileCorruted.Report(false);
            }

            memoryStream.Position = 0;
            switch (type)
            {
                case "byte[]":
                    return (true, new AllTypes(type, memoryStream.ToArray()));
                default:
                    Debugger.SendError("Specified type doesn't match availabilities for download conversion.");
                    return (false, new AllTypes("string", ""));
               

            }

        }


        
    }

    private double getSpeedMBpS(double bytes, double seconds)
    {
        double megabytes = ((double)bytes / (1024 * 1024));
        double speed = megabytes / seconds * 8;
        return speed;
    }

    public async Task<(bool, AllTypes)> GetAndWriteAsync(string url, string? fileName, string path, IProgress<(long, double?)> downloadProgress, IProgress<bool> fileCorruted, IProgress<long?> size, bool progressSpecified = true, string hash = "")
    {
        if (_disposed)
            throw new ObjectDisposedException("HttpUtility");

        using (HttpClient httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debugger.SendError($"Couldn't get a valid response from {url}, got exception {ex}.");
                return (false, new AllTypes("", ""));
            }

            if (size != null && response.Content.Headers.ContentLength.HasValue)
            {
                long fileSize = response.Content.Headers.ContentLength.Value;
                size.Report(fileSize);
                //Debugger.SendInfo("Size is " + fileSize);
            }
            else
                size?.Report(null);

            var stream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[8192];

            string computedHash = "";

            string fullpath;

            if (fileName != null)
            {
                fullpath = $@"{path}\{fileName}";
            }
            else
            {
                var uri = new Uri(url);
                fullpath = Path.GetFileName(uri.LocalPath);
            }


            FileStream fileStream;
            try
            {
                fileStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true);
            }
            catch (Exception ex)
            {
                Debugger.SendError("Error while trying to create a new file : " + ex);
                return (false, new AllTypes("string", ""));
            }
            
            using (fileStream)
            {
                #region progress specified
                if (progressSpecified)
                {
                    List<AllTypes> recordedBytes = new List<AllTypes>();
                    var stopWatch = Stopwatch.StartNew();
                    long totalReadBytes = 0;

                    int bytesRead;

                    #region hash unspecified
                    if (String.IsNullOrEmpty(hash))
                    {
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);

                            totalReadBytes += bytesRead;

                            double seconds = stopWatch.Elapsed.TotalSeconds;

                            recordedBytes.Add(new AllTypes(seconds, totalReadBytes));

                            bool hadEnoughElements = false;
                            double speed = 0.0d;

                            for (int i = recordedBytes.Count - 1; i > 0; i--) //this calculates the speed over a 1 sec span
                            {
                                double bytes = totalReadBytes - Convert.ToDouble(recordedBytes[i].Value);
                                if (seconds - (double)recordedBytes[i].Type > 1)
                                {
                                    speed = getSpeedMBpS(bytes, 1.0d);
                                    hadEnoughElements = true;
                                    recordedBytes.RemoveRange(0, (int)(i * 0.9)); //removes 90% (not 100% cuz margin error) of what's prior to the current index, because it's useless
                                    break;

                                }
                            }
                            if (!hadEnoughElements)
                            {
                                speed = getSpeedMBpS(totalReadBytes, seconds);
                            }

                            downloadProgress.Report((totalReadBytes, speed));
                        }
                        
                    }
                    #endregion hash unspecified
                    #region hash specified
                    else
                    {
                        var hasher = new HashChecker.IncrementalHasher();
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);

                            hasher.AddBlock(buffer, bytesRead);

                            totalReadBytes += bytesRead;

                            double seconds = stopWatch.Elapsed.TotalSeconds;

                            recordedBytes.Add(new AllTypes(seconds, totalReadBytes));

                            bool hadEnoughElements = false;
                            double speed = 0.0d;

                            for (int i = recordedBytes.Count - 1; i > 0; i--) //this calculates the speed over a 1 sec span
                            {
                                double bytes = totalReadBytes - Convert.ToDouble(recordedBytes[i].Value);
                                if (seconds - (double)recordedBytes[i].Type > 1)
                                {
                                    speed = getSpeedMBpS(bytes, 1.0d);
                                    hadEnoughElements = true;
                                    recordedBytes.RemoveRange(0, (int)(i * 0.9)); //removes 90% (not 100% cuz margin error) of what's prior to the current index, because it's useless
                                    break;

                                }
                            }
                            if (!hadEnoughElements)
                            {
                                speed = getSpeedMBpS(totalReadBytes, seconds);
                            }

                            downloadProgress.Report((totalReadBytes, speed));
                        }

                        hasher.FinalizeHash();
                    }
                    #endregion hash specified
                    recordedBytes.Clear();
                }
                #endregion progress specified
                #region progress unspecified
                else
                {
                    #region hash unspecified
                    if (String.IsNullOrEmpty(hash))
                    {
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                    #endregion hash unspecified
                    #region hash specified

                    else //not what I should do ? Should I just use something else since I don't need to do something between the start and end of the download ?
                    {
                        var hasher = new HashChecker.IncrementalHasher();
                        int bytesRead;
                        while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            hasher.AddBlock(buffer, bytesRead);
                        }

                        hasher.FinalizeHash();
                    }
                    #endregion hash specified
                }
                #endregion progress unspecified

            }


            if (!String.IsNullOrEmpty(hash))
            {
                bool isHashCorrect = computedHash == hash;
                Debugger.SendError($"comparing {computedHash} and {hash}. Are they the same ? {isHashCorrect}");
                if (!isHashCorrect)
                {
                    Debugger.SendError($"File downloaded from {url} is corrupted");
                    fileCorruted.Report(true);
                    return (false, new AllTypes("", ""));

                }
                Debugger.SendInfo("hash is correct");
                fileCorruted.Report(false);
                return (true, new AllTypes("", ""));
            }
            return (true, new AllTypes("", ""));

        }



    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool _disposing)
    {
        if (!_disposed)
        {
            if (_disposing)
            {
                if (client != null)
                    client.Dispose();
                client = null;
            }

            // Free unmanaged ressources here

            _disposed = true;
        }
    }

    ~HttpUtility()
    {
        Dispose(false);
    }
}
