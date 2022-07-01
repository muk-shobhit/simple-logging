using Newtonsoft.Json;

public interface ICatFactClient
{
    Task<CatFact> GetCatFact();
}

public class CatFactClient : ICatFactClient
{
    private readonly HttpClient client;

    public CatFactClient(HttpClient client)
    {
        this.client = client;
    }

    public async Task<CatFact> GetCatFact()
    {
        var httpResponse = await client.GetAsync("https://catfact.ninja/fact");

        if (!httpResponse.IsSuccessStatusCode)
        {
            throw new Exception("Cannot retrieve tasks");
        }
        var content = await httpResponse.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<CatFact>(content);
    }
}

public class CatFact
{
    public string Fact { get; set; }
    public int Length { get; set; }
}