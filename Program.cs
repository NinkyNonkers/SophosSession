using SophosSessionHolder;
using NinkyNonk.Shared.Logging;
using Newtonsoft.Json;
using NinkyNonk.Shared.Environment;

SophosSessionConfiguration? config = new SophosSessionConfiguration();

try {
    Project.LoggingProxy.LogProgramInfo();
    Project.LoggingProxy.LogInfo("Starting...");
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
        config.EndpointRoot = Project.LoggingProxy.AskInput("Host: ");
        config.Username = Project.LoggingProxy.AskInput("Username: ");
        config.Password = Project.LoggingProxy.AskInput("Password: ");
        ConsoleLogger.LogInfo("Saving config....");
        File.WriteAllText("Config.json", JsonConvert.SerializeObject(config));
    }

    Project.LoggingProxy.LogInfo("Testing connection...");
    SophosSession sesh = new(config);

    if (!(await sesh.Test())) {
        Project.LoggingProxy.LogError("Could not connect to host");
        Console.ReadKey();
        return;
    } 
    Project.LoggingProxy.LogInfo("Logging in...");
    await sesh.Login();
    Project.LoggingProxy.LogSuccess("Logged in successfully - do not close program");
    await sesh.KeepAlive();
}
catch (Exception e) {
    Project.LoggingProxy.LogError(e.Message);
}

Console.ReadKey();





