using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AcmeLib
{
    /// <summary>
    /// BankService is the entry point to all banking actions.  This class supports the user interface.
    /// Note that the ConnectionString property should be set before using any other functions.
    /// </summary>
    public static class BankService
    {
        /// <summary>
        /// Executes the SQL from filename.  Use this function to reset the database to its 
        /// original condition.
        /// </summary>
        /// <param name="filename">file containing SQL</param>
        public static void DbReset(string filename)
        {
            using (System.IO.StreamReader stream = new System.IO.StreamReader(filename))
            using (SqlConnection conn = Connection)
            {
                var sql = stream.ReadToEnd();
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns a list of all accounts associated with the user
        /// </summary>
        /// <param name="userId">the ID of the user</param>
        /// <returns>Accounts associated with the user</returns>
        public static IEnumerable<Account> GetAccounts(int userId)
        {
            var list = new List<Account>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();

                // Creating parametrized query to avoid SQL injection
                SqlParameter userIdParam = new SqlParameter();
                userIdParam.ParameterName = "@userId";
                userIdParam.Value = userId;

                cmd.CommandText = "Select * from Accounts where userId='"
                    + @userId + "'";
                var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var acct = new Account();
                    acct.Id = rdr.GetInt32(rdr.GetOrdinal("Id"));
                    acct.StartBalance = (float)rdr.GetDecimal(rdr.GetOrdinal("Balance"));
                    acct.AccountType = rdr.GetInt32(rdr.GetOrdinal("type"));
                    list.Add(acct);
                }
                return list;
            }

        }

        /// <summary>
        /// The db connection string.  Must be set before any other function can be used
        /// </summary>
        public static string ConnectionString { get; set; }
        
        /// <summary>
        /// Convenience property for creating new db connections
        /// </summary>
        public static SqlConnection Connection
        {
            get
            {
                return new SqlConnection(ConnectionString);
            }
        }

        /// <summary>
        /// Find and return the user associated with the ID.
        /// </summary>
        /// <param name="id">The ID of the user to find</param>
        /// <returns>User or null if no user with the ID exists</returns>
        public static User GetUser(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "Select id,email,firstname,lastname,password,phone"
                    + " from Users where id=@userId";
                cmd.Parameters.AddWithValue("userId", id);
                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    User u = new User();
                    u.Id = rdr.GetInt32(0);
                    u.Email = rdr.GetString(1);
                    u.Firstname = rdr.GetString(2);
                    u.Lastname = rdr.GetString(3);
                    u.Password = rdr.GetString(4);
                    u.Phone = rdr.GetString(5);
                    return u;
                }
                return null;
            }
        }

        /// <summary>
        /// Return the user with the email and password or null if no user exists
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static User Login(string email, string password)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();

                SqlParameter emailParam = new SqlParameter();
                emailParam.ParameterName = "@email";
                emailParam.Value = email;

                SqlParameter passwordParam = new SqlParameter();
                passwordParam.ParameterName = "@password";
                passwordParam.Value = password;

                cmd.CommandText = "Select id,email,firstname,lastname,password,phone"
                    + " from Users where email= @email and password= @password";

                cmd.Parameters.Add(emailParam);
                cmd.Parameters.Add(passwordParam);

                var rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    User u = new User();
                    u.Id = rdr.GetInt32(0);
                    u.Email = rdr.GetString(1);
                    u.Firstname = rdr.GetString(2);
                    u.Lastname = rdr.GetString(3);
                    u.Password = rdr.GetString(4);
                    u.Phone = rdr.GetString(5);
                    return u;
                }
                return null;
            }
        }

        /// <summary>
        /// Updates the user profile in the database
        /// </summary>
        /// <param name="user">Modified user information to persist</param>
        public static void SaveProfile(User user)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "Update Users set " +
                    "email= @email, firstname = @firstName,lastname = @lastName,phone = @phone where id = @id";

                cmd.Parameters.AddWithValue("email", user.Email);
                cmd.Parameters.AddWithValue("firstName", user.Firstname);
                cmd.Parameters.AddWithValue("lastName", user.Lastname);
                cmd.Parameters.AddWithValue("phone", user.Phone);
                cmd.Parameters.AddWithValue("id", user.Id);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns the account object from the databse associated with ID
        /// </summary>
        /// <param name="id">the ID of the account to retrieve</param>
        /// <returns>the account</returns>
        public static Account GetAccount(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "Select * from Accounts where id= @id";
                cmd.Parameters.AddWithValue("id", id);

                var result = cmd.ExecuteReader();
                if (result.Read())
                {
                    var account = new Account();
                    account.Id = result.GetInt32(result.GetOrdinal("id"));
                    account.StartBalance = (float)result.GetDecimal(result.GetOrdinal("balance"));
                    account.AccountType = result.GetInt32(result.GetOrdinal("type"));
                    return account;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns all the transactions associated with accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static IEnumerable<Transaction> GetTransactions (int accountId)
        {
            var txlist = new List<Transaction>();
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "Select * from Transactions where acctid= @acctId";
                cmd.Parameters.AddWithValue("acctId", accountId);
                var result = cmd.ExecuteReader();
                while (result.Read())
                {
                    var tx = new Transaction();
                    tx.Id = result.GetInt32(result.GetOrdinal("ID"));
                    tx.Amount = (float)result.GetDecimal(result.GetOrdinal("amount"));
                    tx.Date = result.GetDateTime(result.GetOrdinal("TransDate"));
                    tx.Payee = result.GetString(result.GetOrdinal("Payee"));
					tx.Type = result.GetInt32(result.GetOrdinal("type"));
                    txlist.Add(tx);
                }
                return txlist;
            }

        }

        /// <summary>
        /// Transfers funds from one account to another.  Two transactions are created with the same amount.  
        /// one credits the to account and one debits the from account.  Note, this can cause an overdraft.
        /// </summary>
        /// <param name="fromAcct">The account from which to remove funds</param>
        /// <param name="toAcct">The account to which to add funds</param>
        /// <param name="amount">The amount to transfer</param>
        public static void Transfer(int fromAcct, int toAcct, float amount)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                DateTime now = DateTime.Now;
                var sdate = string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day);

                cmd.CommandText = "INSERT INTO [dbo].[Transactions] " +
                    "([TransDate], [AcctId], [Amount], [Payee], [Type]) " +
                    "VALUES(@transDate, @acctId, @amount, 'Transfer',1)";

                cmd.Parameters.AddWithValue("transDate", sdate);
                cmd.Parameters.AddWithValue("acctId", fromAcct);
                cmd.Parameters.AddWithValue("amount", amount);

                cmd.ExecuteNonQuery();

                cmd.Parameters["acctId"].Value = toAcct;
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Creates a bill pay transaction.  Note, this can cause an overdraft
        /// </summary>
        /// <param name="fromAcct">The account from which to debit funds</param>
        /// <param name="payee">The name or email of the person being paid</param>
        /// <param name="amount">The amount.</param>
		public static void PayBill(int fromAcct, string payee, float amount)
		{
			using (SqlConnection conn = Connection)
			{
				conn.Open();
				var cmd = conn.CreateCommand();
				DateTime now = DateTime.Now;
				var sdate = string.Format("{0}-{1}-{2}", now.Year, now.Month, now.Day);

				cmd.CommandText = "INSERT INTO [dbo].[Transactions] " +
					"([TransDate], [AcctId], [Amount], [Payee], [Type]) " +
					"VALUES(@transDate, @acctId, @amount, @payee, 1)";

                cmd.Parameters.AddWithValue("transDate", sdate);
                cmd.Parameters.AddWithValue("acctId", fromAcct);
                cmd.Parameters.AddWithValue("amount", amount);
                cmd.Parameters.AddWithValue("payee", payee);

				cmd.ExecuteNonQuery();
			}
		}
	}
}
