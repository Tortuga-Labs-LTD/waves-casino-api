using Newtonsoft.Json.Linq;

namespace WavesCasinoAPI.Clients
{
    public class NodeClient
    {
        string BaseUrl { get; set; }
        private static readonly HttpClient HttpClient = new HttpClient();
        public NodeClient(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task<List<JObject>> GetTransactionsByAddress(string address, string fromTxId, int limit)
        {
            var result = await HttpClient.GetAsync(BaseUrl + "/transactions/address/" + address + "/limit/" + limit + "?after=" + fromTxId);
            return JArray.Parse(await result.Content.ReadAsStringAsync()).First.ToObject<JArray>().ToObject<List<JObject>>();
        }
    }
}
