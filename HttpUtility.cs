using System.Net.Http;

public class HttpUtility
{
	private readonly HttpClient client;

	public HttpUtility()
	{
		client = new HttpClient();
	}

	public async Task<string?> GetAsync(string url)
	{
		try
		{
			HttpResponseMessage response = await client.GetAsync(url);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
		catch (HttpRequestException ex)
		{
			return null; //do something
		}
	}
}
