using Microsoft.Extensions.Configuration;
using System.IO;

namespace Himitsu.Variable
{
    public class Variables
    {
        private readonly string File = Path.Combine(Directory.GetCurrentDirectory(), "secret.json");
        public string MySQLAddress { get; }
        public string DBUsername { get; }
        public string Password { get; }
        public string Schema { get; }
        public string Connection { get; }
        public Variables()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile(File, true);
            IConfigurationRoot data = builder.Build();
            MySQLAddress = data[nameof(MySQLAddress)];
            DBUsername = data[nameof(DBUsername)];
            Password = data[nameof(Password)];
            Schema = data[nameof(Schema)];

            Connection = $"Server={MySQLAddress};Database={Schema};Uid={DBUsername};Pwd={Password};";
        }
    }
}
