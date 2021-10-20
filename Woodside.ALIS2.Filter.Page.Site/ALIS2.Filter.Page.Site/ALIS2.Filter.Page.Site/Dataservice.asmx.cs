using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Configuration;

namespace ALIS2.Filter.Page.Site
{
    /* Webservices to support client side drop down list and auto complete binding
     * 
     * 
     */
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
 
    public class DataService : System.Web.Services.WebService
    {
        //Class used to populate autocomplete suggestions
        public class Value
        {
            public int id { get; set; }
            public string value { get; set; }
        }

        //Class used to populate filter drop down list
        public class Filter
        {
            public string SPFAttribute { get; set; }
            public string AttributeDisplayName { get; set; } 
        }

        //Web service to set the Filter list that is called each time the apply or clear button is pressed
        [WebMethod]
        public void ApplyFilter(List<ALIS2.Filter.Page.Site._Default.ReportFilter> Filters)
        {
            ALIS2.Filter.Page.Site._Default.ReportFilters = Filters; //set object
        }

        //Web service to retrieve keylist column name and SPF name to populate drop down menu
        [WebMethod]
        public List<Filter> FilterListEnumerator()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["reportServerConnectionString"].ToString(); //connect to DB
            List<Filter> FilterList = new List<Filter>();
            SqlDataReader dr;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_retrieve_keylist_columns", connection))  //execute stored procedure
                {
                    SqlParameter p = new SqlParameter();
                    p.ParameterName = "LibraryKeyListName";
                    p.Value = ALIS2.Filter.Page.Site._Default.KeyListRequested; //pass in the requested keylist

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(p);  //inject parameter into stored procedure
                    connection.Open();

                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if (dr.HasRows)                                     //loop through result set
                    {
                        while (dr.Read())
                        {
                            Filter f = new Filter();
                            f.AttributeDisplayName = dr[0].ToString(); //bind Display Name (used for drop down list text) to object
                            f.SPFAttribute = dr[1].ToString();         //bind SPFAttribute name (used for where clause) to object 
                            FilterList.Add(f);                         //add object to list
                        }
                    }

                }
            }

            return FilterList;
        }

        //web service to return suggested filter values as a user types to the autocomplete drop down box
        [WebMethod]
        public List<Value> ValueListEnumerator( string FilterValue,     //Field to use as filter field
                                                   int items,           //number of items to fetch
                                                string SuggestFilter    //input from autocomplete dropdown box to suggest results
        )
        {
            string connectionString = ConfigurationManager.ConnectionStrings["reportServerConnectionString"].ToString();    //connect to db
            List<Value> ValueList = new List<Value>();
            SqlDataReader dr;
            int i = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_retrieve_keylist_values", connection))   
                {
                    SqlParameter p1 = new SqlParameter();
                    SqlParameter p2 = new SqlParameter();
                    SqlParameter p3 = new SqlParameter();
                    SqlParameter p4 = new SqlParameter();
                    SqlParameter p5 = new SqlParameter();

                    p1.ParameterName = "libraryAttributeSPFProperty";   //add SPF Property value from drop down list
                    p1.Value = FilterValue;

                    p2.ParameterName = "AssetCode";                     //add asset code on query string
                    p2.Value = ALIS2.Filter.Page.Site._Default.AssetRequested;

                    p3.ParameterName = "Items";                         //add item parameters
                    p3.Value = items;

                    p4.ParameterName = "SuggestFilter";                 //add the suggested input text
                    p4.Value = SuggestFilter.Replace("'", "");

                    p5.ParameterName = "LibraryKeylistName";
                    p5.Value = ALIS2.Filter.Page.Site._Default.KeyListRequested;    //add the keylist requested on the query string

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    cmd.Parameters.Add(p3);
                    cmd.Parameters.Add(p4);
                    cmd.Parameters.Add(p5);

                    connection.Open();
                    dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);    //execute stored proc
                    if (dr.HasRows)                 
                    {
                        while (dr.Read())                                       //iterate through result set
                        {
                            ValueList.Add(new Value() { id = i, value = dr[0].ToString() }); //bind generated id and suggested value to object list
                            i += 1;
                        }
                    }

                }
            }
            return ValueList;
        }



    }



}
