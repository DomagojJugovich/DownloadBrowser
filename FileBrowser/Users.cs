using Microsoft.Extensions.Configuration;

namespace FileBrowser
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class Users
    {
        private static Dictionary<string,string> userPass = new Dictionary<string,string>();

        public static void init(IConfiguration configuration)
        {
            var accounts = configuration.GetSection("UserCredentials:Accounts").Get<List<UserAccount>>() ?? new List<UserAccount>();

            foreach (var account in accounts)
            {
                userPass[account.Username.ToUpper()] = account.Password;
            }
        }

        public static bool checkUser(string user, string pass)
        {
            string result;
            bool found = userPass.TryGetValue(user, out result);
            if (found) {
                if (pass == result) { return true; }
            }

            return false;
        }

    }
}
