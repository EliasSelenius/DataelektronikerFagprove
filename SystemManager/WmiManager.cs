using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;

using SystemManager.Models;


namespace SystemManager {
    public static class WmiManager {


        private static CimSession createSession(RegisteredServer registeredServer) {
            var options = new DComSessionOptions();
            options.Impersonation = ImpersonationType.Impersonate;
            options.AddDestinationCredentials(new CimCredential(PasswordAuthenticationMechanism.Default, registeredServer.Domain, registeredServer.Username, registeredServer.Password));

            var s = CimSession.Create(registeredServer.Domain, options);

         
            return s;
        }

        public static Server GetServerInfo(RegisteredServer registeredServer) {
            using var session = createSession(registeredServer);
            var server = new Server();

            if (!session.TestConnection()) {
                server.ConnectionError = true;
                return server;
            }

            server.Id = registeredServer.Id;
            server.Name = registeredServer.Domain;

            var oprsys = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_OperatingSystem").ElementAt(0);

            foreach (var item in oprsys.CimInstanceProperties) {
                server.OperatingSystemProps.Add(item.Name, item.Value?.ToString() ?? "null");
            }


            var services = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_Service");

            foreach (var item in services) {
                server.Services.Add(item);
            }

            return server;
            
        }

        public static void InvokeOnService(RegisteredServer registeredServer, string serviceName, string methodName) {
            using var session = createSession(registeredServer);

            var service = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_Service where Name = '" + serviceName + "'").ElementAt(0);

            var res = session.InvokeMethod(service, methodName, null);

        }

        public static void RebootServer(RegisteredServer registeredServer) {
            using var session = createSession(registeredServer);

            var oprsys = session.QueryInstances(@"root\cimv2", "WQL", "SELECT * FROM Win32_OperatingSystem").ElementAt(0);

            var res = session.InvokeMethod(oprsys, "Reboot", null);
            
        }

    }
}
