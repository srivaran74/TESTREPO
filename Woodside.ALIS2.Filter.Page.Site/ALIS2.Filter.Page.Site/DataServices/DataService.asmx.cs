using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;

namespace DataServices
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
 
    public class DataService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<String> FilterChoiceList()
        {
            string connectionString = "Server=PERDBS126;Database=ALIS2_SSRS;User Id=ALIS2_SSRS;Password=3tAFR2gadasPuF3ut7ax3weJacrefe;";
            List<String> FilterList = new List<String>();
            SqlDataReader dr;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_retrieve_keylist_columns", connection))
                {
                    SqlParameter p = new SqlParameter();
                    p.ParameterName = "LibraryKeyListName";
                    p.Value = "Master Tag List";

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(p);
                    connection.Open();

                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            FilterList.Add(dr[0].ToString());
                        }
                    }

                }
            }

            return FilterList;
        }

        [WebMethod]
        public List<String> ValueListEnumerator()
        {
            string connectionString = "Server=PERDBS126;Database=ALIS2_SSRS;User Id=ALIS2_SSRS;Password=3tAFR2gadasPuF3ut7ax3weJacrefe;";
            List<String> ValueList = new List<String>();
            SqlDataReader dr;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_retrieve_keylist_values", connection))
                {
                    SqlParameter p1 = new SqlParameter();
                    SqlParameter p2 = new SqlParameter();
                    p1.ParameterName = "libraryAttributeName";
                    //p1.Value = FilterChoiceList.SelectedValue.ToString();
                    p2.ParameterName = "AssetCode";
                    p2.Value = "BLP";

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);

                    connection.Open();

                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ValueList.Add(dr[0].ToString());
                        }
                    }

                }
            }
            return ValueList;
        }



    }



}
