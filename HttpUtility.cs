using System.Net.Http;
using System.Windows;

public class HttpUtility
{
	private readonly HttpClient client;

	public HttpUtility()
	{
		client = new HttpClient();
	}

	public async Task<string?> GetAsync(string url) //only does string, consider making it better in the future
	{
		try
		{
			HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (HttpRequestException ex)
		{
			MessageBox.Show(ex.ToString());
			return null; //do something
		}
	}
}
