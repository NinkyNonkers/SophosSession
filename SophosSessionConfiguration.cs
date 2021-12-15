namespace SophosSessionHolder {
    public class SophosSessionConfiguration {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EndpointRoot {get; set;}
        public int HeartbeatMiliseconds { get; set;}


        public SophosSessionConfiguration() {
            Username = "";
            Password = "";
            EndpointRoot = "https://sophosxg.queenelizabeth.cumbria.sch.uk:8090/";
            HeartbeatMiliseconds = 1000;
        }

        public SophosSessionConfiguration(string username, string password, string endpointRoot, int heartbeatMiliseconds) {
            Username = username;
            Password = password;
            EndpointRoot = endpointRoot;
            HeartbeatMiliseconds = heartbeatMiliseconds;
        }
        
    }
}