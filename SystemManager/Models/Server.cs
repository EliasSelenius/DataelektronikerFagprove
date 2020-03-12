using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Management.Infrastructure;

namespace SystemManager.Models {
    public class Server {

        public bool ConnectionError = false;
        public int Id;
        public string Name;

        public Dictionary<string, string> OperatingSystemProps = new Dictionary<string, string>();

        public List<CimInstance> Services = new List<CimInstance>();

    }
}
