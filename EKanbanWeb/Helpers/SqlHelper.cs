using EKanbanWeb.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EKanbanWeb.Helpers
{
    public class SqlHelper
    {
        private IConfiguration Conf { get; }
        private DbConf _db;

        public SqlHelper(IConfiguration Configuration, IHostEnvironment hostEnvironment)
        {
            Conf = Configuration;
            _db = new DbConf(Conf, hostEnvironment);
        }

        public DataTable ExecQuery(string sp, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                using SqlConnection con = new SqlConnection(_db.GetConnectionString());
                con.Open();

                using (SqlCommand comm = new SqlCommand("SET ARITHABORT ON", con))
                {
                    comm.ExecuteNonQuery();
                }

                SqlCommand cmd = new SqlCommand(sp, con);
                foreach (SqlParameter param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;
                SqlDataAdapter da = new SqlDataAdapter
                {
                    SelectCommand = cmd
                };

                da.Fill(ds);
                dt = ds.Tables[0];
                con.Close();
            }
            catch (Exception e)
            {
            }
            return dt;
        }

        public DataTable ExecQueryCommand(string command)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                using SqlConnection con = new SqlConnection(_db.GetConnectionString());
                con.Open();

                using (SqlCommand comm = new SqlCommand("SET ARITHABORT ON", con))
                {
                    comm.ExecuteNonQuery();
                }

                SqlCommand cmd = new SqlCommand(command, con);

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 120;
                SqlDataAdapter da = new SqlDataAdapter
                {
                    SelectCommand = cmd
                };

                da.Fill(ds);
                dt = ds.Tables[0];
                con.Close();
            }
            catch (Exception e)
            {
            }
            return dt;
        }

        public bool ExecNonQuery(string sp, List<SqlParameter> parameters)
        {
            bool status = false;
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                using SqlConnection con = new SqlConnection(_db.GetConnectionString());
                con.Open();
                SqlCommand cmd = new SqlCommand(sp, con);
                foreach (SqlParameter param in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                }

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                con.Close();
                status = true;
            }
            catch (Exception e)
            {
            }
            return status;
        }

        public List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName.ToLower()).ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row => {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name.ToLower()))
                    {
                        try
                        {
                            pro.SetValue(objT, row[pro.Name]);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                return objT;
            }).ToList();
        }
    }
}
