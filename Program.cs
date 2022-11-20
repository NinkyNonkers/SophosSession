using System;
using System.IO;
using SophosSessionHolder;
using Newtonsoft.Json;
using NinkyNonk.Shared.Environment;
using NinkyNonk.Shared.Framework.Exception;

SophosSessionConfiguration? config = new SophosSessionConfiguration();

try
{
    Project.LoggingProxy.LogProgramInfo();
    Project.LoggingProxy.LogInfo("Starting...");
    if (args.Length > 0)
    {
        if (args.Length >= 2)
        {
            config.Username = args[0];
            config.Password = args[1];
            if (args.Length == 3)
                config.EndpointRoot = args[2];
        }
        else
            throw new FatalException("Not enough arguments!");
    }
    else if (File.Exists("Config.json"))
    {
        string text = File.ReadAllText("Config.json");
        if (string.IsNullOrEmpty(text))
            throw new FatalException("Could not load config file");
        config = JsonConvert.DeserializeObject<SophosSessionConfiguration>(text);
        if (config == null)
            throw new FatalException("Could not load config");
    }
    else
    {
        config.EndpointRoot = Project.LoggingProxy.AskInput("Host: ");
        if (!config.EndpointRoot.Contains("https://") || !config.EndpointRoot.Contains("http://"))
            config.EndpointRoot = "http://" + config.EndpointRoot;
        config.Username = Project.LoggingProxy.AskInput("Username: ");
        config.Password = Project.LoggingProxy.AskInput("Password: ");
        Project.LoggingProxy.LogInfo("Saving config....");
        File.WriteAllText("Config.json", JsonConvert.SerializeObject(config));
    }

    Project.LoggingProxy.LogInfo("Testing connection...");
    SophosSession sesh = new(config);

    if (!await sesh.Test())
    {
        Console.ReadKey();
        return;
    }

    Project.LoggingProxy.LogInfo("Logging in...");
    await sesh.Login();
    Project.LoggingProxy.LogSuccess("Logged in successfully - do not close program");
    await sesh.KeepAlive();
}
catch (FatalException e)
{
    Project.LoggingProxy.LogFatal(e.Message);
    Project.LoggingProxy.Log("Press any key to exit...");
}
catch (Exception e)
{
    Project.LoggingProxy.LogError(e.Message);
}

Console.ReadKey();





