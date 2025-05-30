using System.Net.Http;
using System.Windows;

public class HttpUtility : IDisposable
{
	private HttpClient? client;

    private bool _disposed = false; // "_" mean it's for the class itself

	public HttpUtility()
	{
		client = new HttpClient();
	}

	public async Task<string?> GetStringAsync(string url) //only does string, consider making it better in the future
	{
        if (_disposed)
            throw new ObjectDisposedException("HttpUtility");
		try
		{
			HttpResponseMessage response = await client!.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (HttpRequestException ex)
		{
			MessageBox.Show(ex.ToString());
			return null; //do something
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
