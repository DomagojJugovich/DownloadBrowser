namespace FileBrowser
{
    public static class Users
    {
        private static Dictionary<string,string> userPass = new Dictionary<string,string>();

        public static void init()
        {
            userPass.Add("MORH", "M0rh123#");
            userPass.Add("HP", "HP05ta678#");
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
