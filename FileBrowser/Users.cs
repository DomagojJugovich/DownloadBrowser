using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FileBrowser
{
    public class UserAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public static class Users
    {
        private const string EncryptedPrefix = "ENC:";
        private const string ProtectorPurpose = "FileBrowser.UserCredentials.v1";

        private static Dictionary<string, string> userPass = new Dictionary<string, string>();

        public static void init(IConfiguration configuration, string configFilePath)
        {
            // Keys live next to appsettings.json so they survive a self-contained republish
            // (DeleteExistingFiles=false). Losing this folder makes existing ENC: passwords
            // permanently undecryptable - recover by replacing the value in appsettings.json
            // with a fresh plaintext password so it gets re-encrypted on next start.
            var keyDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(configFilePath)!, "keys"));
            var protector = DataProtectionProvider.Create(keyDirectory, builder =>
                {
                    builder.SetApplicationName("FileBrowser");
                    builder.ProtectKeysWithDpapi(protectToLocalMachine: true);
                })
                .CreateProtector(ProtectorPurpose);

            var accounts = configuration.GetSection("UserCredentials:Accounts").Get<List<UserAccount>>() ?? new List<UserAccount>();
            bool configChanged = false;

            foreach (var account in accounts)
            {
                string plainPassword;

                if (account.Password.StartsWith(EncryptedPrefix, StringComparison.Ordinal))
                {
                    plainPassword = protector.Unprotect(account.Password.Substring(EncryptedPrefix.Length));
                }
                else
                {
                    plainPassword = account.Password;
                    account.Password = EncryptedPrefix + protector.Protect(plainPassword);
                    configChanged = true;
                }

                userPass[account.Username.ToUpper()] = plainPassword;
            }

            if (configChanged)
            {
                PersistEncryptedPasswords(configFilePath, accounts);
            }
        }

        private static void PersistEncryptedPasswords(string configFilePath, List<UserAccount> accounts)
        {
            var root = JsonNode.Parse(File.ReadAllText(configFilePath))!.AsObject();
            var accountsArray = root["UserCredentials"]!["Accounts"]!.AsArray();

            for (int i = 0; i < accounts.Count; i++)
            {
                accountsArray[i]!["Password"] = accounts[i].Password;
            }

            File.WriteAllText(configFilePath, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
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
