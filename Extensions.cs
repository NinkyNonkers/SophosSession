using System;
using System.Net.Http;
using NinkyNonk.Shared.Environment;

namespace SophosSessionHolder;

public static class Extensions {
    public static bool CheckSuccess(this HttpResponseMessage msg)
    {
        try
        {
            msg.EnsureSuccessStatusCode();
            if (msg.Content.ToString() == null) 
                throw new Exception("Did not receive proper response");
            if (msg.Content.ToString()!.ToLower().Contains("invalid user name")) 
                throw new Exception("Invalid username or password!");
        }
        catch (Exception e)
        {
            Project.LoggingProxy.LogError(e.Message);
            return false;
        }
        return true;
    }
}