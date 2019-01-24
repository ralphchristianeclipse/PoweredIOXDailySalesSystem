using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace PowerediOXDailySales
{
    public static class Accounts
    {
        public static List<string> SecretQuestionList = new List<string>() { "Who is your first crush?", "What is your ambition?", "Where do you live?", "When did you graduate highschool?", "Why do you do this?" };
        public static Dictionary<string, string> AccountsList = new Dictionary<string, string>();
        public static DataSet AccountsDataSet;
        public static DataTable AccountTable;
        public static void InitializeDatabase()
        {
            GetAccountsData();
            //Create default login "admin" if table is not existing in the accounts database
            if (AccountTable == null)
            {
                AccountsDataSet.Tables.Add("Accounts");
                AccountTable = AccountsDataSet.Tables["Accounts"];
                AccountTable.AddColumns("", "Username", "Password", "ManagerName", "SecretQuestion", "SecretAnswer", "Status");
                AccountTable.SetKey("Username");
                AccountTable.Rows.Add("admin", "admin", "admin", "admin", "admin", "admin");
                AccountsDataSet.SaveXML();
            }
        }

        public static void GetAllAccounts()
        {
            AccountsDataSet.SaveXML();
            AccountsList.Clear();
            AccountTable.Rows?.Cast<DataRow>().ToList().ForEach(row => AccountsList.Add(row["Username"].ToString().ToLower(), row["Password"].ToString()));
        }

        public static DataRow SetUserStatus(string userName, string statusValue = "")
        {
            var row = AccountTable.Select($"Username='{userName}'");
            if (row.Length < 1) return null;
            if (statusValue != "")
                row[0]["Status"] = statusValue;
            return row[0];
        }

        public static bool CheckUserStatus(string userName, string statusValue = "Offline")
        {
            var row = SetUserStatus(userName);
            var status = row["Status"].ToString();
            return status == statusValue;
        }
        public static bool IsAccountExist(string user)
        {
            return AccountsList.ContainsKey(user.ToLower());
        }

        public static bool IsAdmin(string user)
        {
            var adminUser = AccountTable.Rows.Find(user);
            return adminUser != null && adminUser["Status"].ToString() == "admin";
        }
        public static void GetAccountsData()
        {
            AccountsDataSet = new DataSet("PowerIOXAccounts");
            AccountsDataSet.LoadXML();
            AccountTable = AccountsDataSet.Tables["Accounts"];
            AccountTable.SetKey("Username");
        }
    }
}
