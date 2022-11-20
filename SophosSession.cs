using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NinkyNonk.Shared.Data;
using NinkyNonk.Shared.Environment;
using NinkyNonk.Shared.Framework.Exception;

namespace SophosSessionHolder;

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
    private bool _loggedIn;

    public int TotalSessionRequests { get => _goodRequests + _failedRequests; }
    
    public SophosSession(SophosSessionConfiguration config)
    {
        _username = config.Username;
        _password = config.Password;
        _host = config.EndpointRoot;
        _heartbeatTimeout = config.HeartbeatMiliseconds;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Referrer = new Uri(_host);
        _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
    }
        
    private string EncodeUsername() {
        return _username.Replace("@", "%40");
    }
        
    public async Task<bool> Test()
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(_host);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception e)
        {
            Project.LoggingProxy.LogError(e.Message);
            throw new FatalException("Could not connect to host.");
        }
    }

    private async Task LoginLoop()
    {
        while (!_loggedIn)
        {
            try
            {
                await Login();
                _loggedIn = true;
            }
            catch (Exception e)
            {
                Project.LoggingProxy.LogError(e.Message);
                Project.LoggingProxy.LogUpdate("Retrying login in 5 seconds.");
                await Task.Delay(5000);
            }
        }
        Project.LoggingProxy.LogSuccess("Logged in successfully");
    }
    
    public async Task Login()
    {
        string url = _host + Endpoint;

        Dictionary<string, string> body = new Dictionary<string, string>
        {
            {"mode", "191"},
            {"username", _username},
            {"password", _password},
            {"a", DateTime.Now.GetEpoch().ToString()},
            {"producttype", "0"},
        };
        
        HttpResponseMessage msg = await _client.PostAsync(url, new FormUrlEncodedContent(body));

        if (!msg.CheckSuccess())
            throw new WebException("Request failed: " + msg.StatusCode);
    }

    private string FillEndpoint(string endpoint) {
        return _host + endpoint.Replace("{username}", EncodeUsername())
            .Replace("{password}", _password)
            .Replace("{time}", DateTime.Now.GetEpoch().ToString());
    }

    public async Task KeepAlive()
    {
        string url = FillEndpoint(LiveEndpoint);
        
        ThreadPool.QueueUserWorkItem(async _ => {
            while (true) {
                await Task.Delay(300000);
                Project.LoggingProxy.LogUpdate($"Sent {TotalSessionRequests} heartbeats in the past five minutes. {_goodRequests}/{TotalSessionRequests} succeeded");
                _goodRequests = 0;
                _failedRequests = 0;
            }
        });

        while (true) {
            await Task.Delay(_heartbeatTimeout);
            var msg = await _client.GetAsync(url);
            if (!msg.CheckSuccess())
            {
                _failedRequests++;
                Project.LoggingProxy.LogInfo("Re-logging in due to error...");
                _loggedIn = false;
                await LoginLoop();
            }
            else
                _goodRequests++;
        }
    }
}