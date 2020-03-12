using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SystemManager.Models {
    public class RegisteredServer {

        public int Id;

        public string Domain;
        public string Username;
        public System.Security.SecureString Password;

        public RegisteredServer(int id, string domain, string username, string password) {
            Id = id;
            Domain = domain;
            Username = username;
            Password = new System.Security.SecureString();
            for (int i = 0; i < password.Length; i++) Password.AppendChar(password[i]);
        }

    }
}
