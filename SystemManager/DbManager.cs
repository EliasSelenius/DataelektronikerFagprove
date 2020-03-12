using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;

using SystemManager.Models;

namespace SystemManager {
    public static class DbManager {

        private static readonly string password = "*KjøttkakerSchadenfreudeUniform12";
        private static SqlConnection newConnection() => new SqlConnection($"Data Source=10.10.8.79;Initial Catalog=TEMP_ELIAS_FAGPROVE;User id=SA;Password={password};");

        public static bool RemoveServer(int id) {
            int t = Exec("delete from TEMP_ELIAS_FAGPROVE.dbo.Servers where id = @id", new { id });
            return t == 1;
        }

        public static bool AddServer(string domain, string username, string password) {
            int t = Exec("insert into TEMP_ELIAS_FAGPROVE.dbo.Servers (Domain, Username, [Password]) values (@domain, @username, @password)", new { domain, username, password });
            return t == 1;
        }

        public static List<RegisteredServer> GetServers() {
            var data = Query("select * from TEMP_ELIAS_FAGPROVE.dbo.Servers");

            var res = new List<RegisteredServer>();

            foreach (var item in data) {
                res.Add(new RegisteredServer((int)item["id"], (string)item["Domain"], (string)item["Username"], (string)item["Password"]));
            }

            return res;
        }

        public static RegisteredServer GetServer(int id) {
            var data = Query("select top 1 * from TEMP_ELIAS_FAGPROVE.dbo.Servers where id = @id", new { id }).FirstOrDefault();

            var res = new RegisteredServer((int)data["id"], (string)data["Domain"], (string)data["Username"], (string)data["Password"]);

            return res;
        }

        public static IEnumerable<Dictionary<string, object>> Query(string command, dynamic vars = null) {
            var res = new List<Dictionary<string, object>>();
            var sapConnection = newConnection();
            sapConnection.Open();
            using (var cmd = sapConnection.CreateCommand()) {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = command;

                if (vars != null) {
                    System.Reflection.PropertyInfo[] varprops = vars.GetType().GetProperties();
                    foreach (var prop in varprops) {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(vars));
                    }
                }

                using (var reader = cmd.ExecuteReader()) {

                    while (reader.Read()) {
                        var dict = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++) {
                            dict.Add(reader.GetName(i), reader.GetValue(i));
                        }
                        res.Add(dict);
                    }


                    reader.Close();
                    sapConnection.Close();
                }
            }
            return res;
        }

        public static int Exec(string proc, dynamic variables = null) {
            var con = newConnection();
            con.Open();
            using (var cmd = con.CreateCommand()) {
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = proc;

                if (variables != null) {
                    System.Reflection.PropertyInfo[] varprops = variables.GetType().GetProperties();
                    foreach (var prop in varprops) {
                        cmd.Parameters.AddWithValue(prop.Name, prop.GetValue(variables) ?? DBNull.Value);
                    }
                }

                int res = cmd.ExecuteNonQuery();
                con.Close();
                return res;
            }
        }

    }
}
