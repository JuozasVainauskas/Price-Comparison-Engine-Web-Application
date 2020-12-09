using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using PCE_Web.Models;
using PCE_Web.Tables;

//startup su interface, multiple interface, interface grupės, inicijuoti tik reikalingus
//group atvaizduoti prekių skaičių pagal kategoriją, pagal shop
//aggregate-count
//-join

namespace PCE_Web.Classes
{
    public class DatabaseManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;

        public DatabaseManager(PCEDatabaseContext pceDatabaseContext)
        {
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void SetRoleWithDataAdapter(string email, string role)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            var sqlDataAdapter = new SqlDataAdapter("SELECT Role, Email FROM UserData;", sqlConnection);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var update = new SqlCommand
                {
                    Connection = sqlConnection,
                    CommandType = CommandType.Text,
                    CommandText = "UPDATE UserData SET Role = @Role WHERE Email = @Email;"
                };

                update.Parameters.Add(new SqlParameter("@Role", SqlDbType.NVarChar, int.MaxValue, "Role"));
                update.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, int.MaxValue, "Email"));

                sqlDataAdapter.UpdateCommand = update;

                var dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "UserData");

                DataTable dataTable = dataSet.Tables["UserData"];
                dataTable.Rows[0]["Role"] = role;
                dataTable.Rows[0]["Email"] = email;

                sqlDataAdapter.Update(dataSet, "UserData");
            }
            catch (Exception ex)
            {
                //WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
            }
        }

       
        
        public void WriteReportsWithSql(string email, string report)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var sqlCommand = new SqlCommand("INSERT INTO Reports(Email, Comment, Date, Solved) VALUES (@Email, @Comment, @Date, @Solved)", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Email", email);
                sqlCommand.Parameters.AddWithValue("@Comment", report);
                sqlCommand.Parameters.AddWithValue("@Date", DateTime.UtcNow.ToString());
                sqlCommand.Parameters.AddWithValue("@Solved", 0);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

       
        public void DeleteReportsWithSql(int id)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            try
            {
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                var sqlCommand = new SqlCommand("DELETE FROM Reports WHERE ReportsId = @Id;", sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.AddWithValue("@Id", id);
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
