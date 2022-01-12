using SophosSessionHolder;
using System;
using System.IO;
using NinkyNonk.Shared.Logging;
using Newtonsoft.Json;

SophosSessionConfiguration? config = new SophosSessionConfiguration();

try {
    ConsoleLogger.LogProgramInfo();
    ConsoleLogger.LogInfo("Starting...");
    if (args.Length > 0) {
        if (args.Length >= 2) {
            config.Username = args[0];
            config.Password = args[1];
            if (args.Length == 3)
                config.EndpointRoot = args[2];
        }
        else {
            throw new ArgumentException("Not enough arguments!");
        }
    }
    else if (File.Exists("Config.json")) {
        string text = File.ReadAllText("Config.json");
        if (text == null)
            throw new Exception("Could not load config file");
        config = JsonConvert.DeserializeObject<SophosSessionConfiguration>(text);
        if (config == null) {
            throw new Exception("Could not load config");
        }
    }
    else {
        config.EndpointRoot = ConsoleLogger.AskInput("Host: ");
        config.Username = ConsoleLogger.AskInput("Username: ");
        config.Password = ConsoleLogger.AskInput("Password: ");
        ConsoleLogger.LogInfo("Saving config....");
        File.WriteAllText("Config.json", JsonConvert.SerializeObject(config));
    }

    ConsoleLogger.LogInfo("Testing connection...");
    SophosSession sesh = new(config);

    if (!(await sesh.Test())) {
        ConsoleLogger.LogError("Could not connect to host");
        Console.ReadKey();
        return;
    } 
    ConsoleLogger.LogInfo("Logging in...");
    await sesh.Login();
    ConsoleLogger.LogInfo("Logged in successfully - do not close program");
    await sesh.KeepAlive();
}
catch (Exception e) {
    ConsoleLogger.LogError(e.Message);
}

Console.ReadKey();





