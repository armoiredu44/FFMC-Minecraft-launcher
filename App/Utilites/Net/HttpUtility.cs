using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Diagnostics;
using System.Text;
public class HttpUtility : Utilities, IDisposable
{
	private HttpClient? client;

    private bool _disposed = false;

	public HttpUtility()
	{
		client = new HttpClient();
	}

	
    public async Task<(bool, AllTypes)> GetAsync(string url, IProgress<(long, double)> downloadProgress, IProgress<bool> fileCorruted, string type = "byte[]", bool progressSpecified = true, string hash = "")
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
            var stream = await response.Content.ReadAsStreamAsync();

            var buffer = new byte[16384];
            using var memoryStream = new MemoryStream();
            

            if (progressSpecified)
            {
                var stopWatch = Stopwatch.StartNew();
                long totalReadBytes = 0;
                long intervalBytes = 0;

                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                /* **EXPLANATION**
                 * the line above essentially means, "as long a the stream isn't empty", and what is also does is
                 * at each iteration, the stream fills the buffer a certain amount, and then we do stuff with bytesRead in the loop
                 * at the following iteration, the content of bytesRead and the buffer is OVERWRITTEN. That's all.*/
                {
                    memoryStream.Write(buffer, 0, bytesRead);

                    totalReadBytes += bytesRead;
                    intervalBytes += bytesRead;

                    
                    double seconds = stopWatch.Elapsed.TotalSeconds;

                    double megabytes = ((double)intervalBytes / (1024 * 1024));
                    double speed = megabytes / seconds;
                    downloadProgress.Report((totalReadBytes, speed));
                    intervalBytes = 0;
                    stopWatch.Restart();
                    


                }
            }
            else //not what I should do ? Should I just use something else since I don't need to do something between the start and end of the download ?
            {
                int bytesRead;
                while ((bytesRead =  await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
            }

            if (!String.IsNullOrEmpty(hash))
            {
                memoryStream.Position = 0;
                bool isHashCorrect = HashChecker.isHashTheSame(memoryStream, hash);
                if (!isHashCorrect)
                {
                    Debugger.SendError($"File downloaded from {url} seems corrupted");
                    fileCorruted.Report(true);
                    return (false, new AllTypes("", ""));

                }
                else
                    Debugger.SendInfo("File doesn't seem corrupted");
                    fileCorruted.Report(false);
            }

            memoryStream.Position = 0;
            switch (type)
            {
                case "byte[]":
                    return (true, new AllTypes(type, memoryStream.ToArray()));
                case "string":
                    using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
                    {
                        string memoryStreamContent = streamReader.ReadToEnd();
                        return (true, new AllTypes(type, memoryStreamContent));
                    }
                default:
                    return (false, new AllTypes("string", ""));
               

            }

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
