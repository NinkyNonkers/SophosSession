using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NinkyNonk.Shared.Logging;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace SophosSessionHolder {
    public class SophosSession
    {
        private const string Endpoint = "login.xml";
        private const string LiveEndpoint = "live?mode=192&username={username}&a={time}&producttype=0";
        private readonly string _username;
        private readonly string _password;
        private readonly string _host;
        private readonly int _heartbeatTimeout;

        private readonly HttpClient _client;

        private int _goodRequests;
        private int _failedRequests;

        public int TotalSessionRequests { get => _goodRequests + _failedRequests; }

        
        public SophosSession(SophosSessionConfiguration config) {
            _username = config.Username;
            _password = config.Password;
            _host = config.EndpointRoot;
            _heartbeatTimeout = config.HeartbeatMiliseconds;
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Referrer = new Uri(_host);
        }


        private string EncodeUsername() {
            return _username.Replace("@", "%40");
        }


        public async Task<bool> Test() {
            HttpResponseMessage response = await _client.GetAsync(_host);
            response.EnsureSuccessStatusCode();
            return true;
        }


        public async Task Login() {
            string url = _host + Endpoint;

            Dictionary<string, string> body = new Dictionary<string, string>() {
                {"mode", "191"},
                {"username", _username},
                {"password", _password},
                {"a", DateTime.Now.GetEpoch().ToString()},
                {"producttype", "0"},
            };

            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            
            HttpResponseMessage msg = 
                    await _client.PostAsync(url, new FormUrlEncodedContent(body));

            if (msg.EnsureSuccessStatusCode() == null) {
                throw new WebException("Request failed: " + msg.StatusCode);
            }
            
        }

        private string FillEndpoint(string endpoint) {
            return _host + endpoint.Replace("{username}", EncodeUsername())
            .Replace("{password}", _password)
            .Replace("{time}", DateTime.Now.GetEpoch().ToString());
        }

        public async Task KeepAlive() {

            string url = FillEndpoint(LiveEndpoint);
            HttpResponseMessage msg;


            ThreadPool.QueueUserWorkItem(async (o) => {
                while (true) {
                    await Task.Delay(300000);
                    ConsoleLogger.LogInfo($"Sent {TotalSessionRequests} heartbeats in the past five minutes. {_goodRequests}/{TotalSessionRequests} succeeded");
                    _goodRequests = 0;
                    _failedRequests = 0;
                }
            });

            while (true) {
                await Task.Delay(_heartbeatTimeout);
                msg = await _client.GetAsync(url);
                if (!msg.CheckSuccess())
                    _failedRequests++;
                else
                    _goodRequests++;
            }
        }



    }
}