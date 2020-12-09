using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using PCE_Web.Models;
using PCE_Web.Tables;

namespace PCE_Web.Classes
{
    public class ReportsManagerWithSql : IReportsManager
    {
        private readonly PCEDatabaseContext _pceDatabaseContext;
        private readonly IExceptionsManager _exceptionsManager;

        public ReportsManagerWithSql(IExceptionsManager exceptionsManager, PCEDatabaseContext pceDatabaseContext)
        {
            _exceptionsManager = exceptionsManager;
            _pceDatabaseContext = pceDatabaseContext;
        }

        public void WriteReports(string email, string report)
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
                _exceptionsManager.WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public List<Report> ReadReports(string email, int solvedId)
        {
            var comments = _pceDatabaseContext.Reports.Where(column => column.Email == email && column.Solved == solvedId).Select(column => new Report { Comment = column.Comment, ID = column.ReportsId, Date = column.Date, Email = column.Email }).ToList();
            return comments;
        }

        public void DeleteReports(int id)
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
                _exceptionsManager.WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        public bool IsReported(string email)
        {
            var item = _pceDatabaseContext.Reports.Where(column => column.Email == email).Select(column => new Reports { Email = column.Email, Comment = column.Comment, Solved = column.Solved, Date = column.Date }).ToList();
            if (item.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MarkAsSolved(int id)
        {
            var sqlConnection = new SqlConnection(_pceDatabaseContext.Database.GetDbConnection().ConnectionString);
            var sqlDataAdapter = new SqlDataAdapter("SELECT ReportsId, Solved FROM Reports;", sqlConnection);
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
                    CommandText = "UPDATE Reports SET Solved = 1 WHERE ReportsId = @ReportsId AND Solved = 0;"
                };

                update.Parameters.Add(new SqlParameter("@ReportsId", SqlDbType.NVarChar, int.MaxValue, "ReportsId"));

                sqlDataAdapter.UpdateCommand = update;

                var dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Reports");

                DataTable dataTable = dataSet.Tables["Reports"];
                dataTable.Rows[0]["ReportsId"] = id;

                sqlDataAdapter.Update(dataSet, "Reports");
            }
            catch (Exception ex)
            {
                _exceptionsManager.WriteLoggedExceptions(ex.Message, ex.Source, ex.StackTrace, DateTime.UtcNow.ToString());
            }
            finally
            {
                sqlConnection.Close();
                sqlDataAdapter.Dispose();
            }
        }
    }
}
