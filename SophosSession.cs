using System.Net;
using NinkyNonk.Shared.Logging;


namespace SophosSessionHolder {
    public class SophosSession
    {
        private const string Endpoint = "login.xml?username={username}&mode=191&password={password}&producttype=0&a={time}";
        private const string LiveEndpoint = "live?mode=192&username={username}&a={time}&producttype=0";
        private readonly string _username;
        private readonly string _password;
        private readonly string _host;

        private readonly HttpClient _client;

        private int _goodRequests;
        private int _failedRequests;

        
        public SophosSession(string username, string password, string host) {
            _username = System.Web.HttpUtility.UrlEncode(username);
            _password = System.Web.HttpUtility.UrlEncode(password);
            _host = host;
            _client = new HttpClient();
        }


        public async Task<bool> Test() {
            HttpResponseMessage response = await _client.GetAsync(_host);
            return response.StatusCode != HttpStatusCode.OK;
        }


        public async Task Login() {
            string url = FillEndpoint(Endpoint);

            HttpResponseMessage msg = await _client.PostAsync(url, 
            new FormUrlEncodedContent(new Dictionary<string, string>()));

            if (msg.EnsureSuccessStatusCode() == null) {
                throw new WebException("Request failed: " + msg.StatusCode.ToString());
            }


            ThreadPool.QueueUserWorkItem(async (o) => {
                while (true) {
                    await Task.Delay(300000);
                    ConsoleLogger.LogInfo($"Sent {_goodRequests + _failedRequests} heartbeats in the past five minutes.");
                    ConsoleLogger.LogInfo($"{_goodRequests}/{_failedRequests + _goodRequests} succeeded");
                }
            });

            await KeepAlive();
            
        }

        private string FillEndpoint(string endpoint) {
            return _host + endpoint.Replace("{username}", _username)
            .Replace("{password}", _password)
            .Replace("{time}", DateTime.Now.TimeOfDay.GetEpoch().ToString());
        }

        private async Task KeepAlive() {

            string url = FillEndpoint(LiveEndpoint);
            HttpResponseMessage msg;
            while (true) {
                await Task.Delay(1000);
                msg = await _client.PostAsync(LiveEndpoint, new FormUrlEncodedContent(new Dictionary<string, string>()));
                if (msg.StatusCode != HttpStatusCode.OK)
                    _failedRequests++;
                else
                    _goodRequests++;
            }
        }



    }
}