namespace SophosSessionHolder {
    public class SophosSessionConfiguration {
        public string Username { get; set; }
        public string Password { get; set; }

        public string EndpointRoot {get; set;}

        public SophosSessionConfiguration() {
            Username = "";
            Password = "";
            EndpointRoot = "https://sophosxg.queenelizabeth.cumbria.sch.uk:8090/";
        }

        public SophosSessionConfiguration(string username, string password, string endpointRoot) {
            Username = username;
            Password = password;
            EndpointRoot = endpointRoot;
        }
        
    }
}