using System;

namespace datahold
{
    public static class Config
    {
        public static string Server { get; set; }
        public static string Database { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }

        // Call this to get the connection string that was set at login
        public static string GetConnectionString()
        {
            return $"Server={Server};Database={Database};User Id={Username};Password={Password};";
        }
    }
}
