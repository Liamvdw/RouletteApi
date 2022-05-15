using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Roulette.Providers.Interface;
using System.Data;
using Microsoft.Extensions.Configuration;
using Roulette.Models;

namespace Roulette.Providers
{
    public class Repository: IRepository
    {
        public Repository()
        {
            if (!System.IO.File.Exists(@".\RouletteDB.sqlite"))
            {
                SQLiteConnection.CreateFile("RouletteDB.sqlite");

                SQLiteConnection m_dbConnection = new SQLiteConnection("Data Source=RouletteDB.sqlite;Version=3;");
                m_dbConnection.Open();

                SQLiteCommand command = new SQLiteCommand(QueryConstants.CreateTable, m_dbConnection);

                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
        }

        public void ExecuteWrite(string query, Dictionary<string, object> args)
        {
            //setup the connection to the database
            using (var con = new SQLiteConnection("Data Source=RouletteDB.sqlite;Version=3;"))
            {
                con.Open();

                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                con.Close();

            }
        }

        public DataTable Execute(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;

            using (var con = new SQLiteConnection("Data Source=RouletteDB.sqlite;Version=3;"))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }

                    var da = new SQLiteDataAdapter(cmd);

                    var dt = new DataTable();
                    da.Fill(dt);

                    da.Dispose();
                    return dt;
                }
            }
        }
    }
}
