using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.Web.Services;
using ALIS2.Filter.Page.Site;
using System.Configuration;

namespace ALIS2.Filter.Page.Site
{
    public partial class _Default : System.Web.UI.Page
    {
        //strings that are used to store keylist and asset paramerters from query string
        public static string KeyListRequested, AssetRequested;
        //Filter list that is populated/cleaared from client side webservice
        static public List<ReportFilter> ReportFilters = new List<ReportFilter>();

        //class to hold reportfilter objects
        public class ReportFilter
        {
            public string FilterField { get; set; } //field to filter
            public string Operator { get; set; }    //operator to filter with
            public string Value { get; set; }       //value to filter with
        }
      
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
				//string KeyListRequested1 = "Manual Valve Register1";test;
				KeyListRequested =  Request.QueryString["KeyList"];  //set keylist parameter ;
                AssetRequested =  Request.QueryString["Asset"];      //set asset parameter;

                // Set the processing mode for the ReportViewer to Remote
                KeyListViewer.ProcessingMode = ProcessingMode.Remote;

                ServerReport serverReport = KeyListViewer.ServerReport;

                // Set the report server URL and report path
                serverReport.ReportServerUrl =
                    new Uri(ConfigurationManager.AppSettings["reportServerURL"].ToString());
                serverReport.ReportPath =
                    ConfigurationManager.AppSettings["reportServerPath"].ToString() + KeyListRequested;

                //Set Asset
                ReportParameter Asset = new ReportParameter();
                Asset.Name = "Asset";
                Asset.Values.Add(AssetRequested);
                KeyListViewer.ServerReport.SetParameters(
                new ReportParameter[] { Asset });

            }
        }

        //Method used when Apply Filters button is pressed. This applies the filters in the ReportFilters object to the SSRS report.
        public void filterReport(object sender, EventArgs e)
        {
            ApplyFilters.CssClass = "k-button";
            ClearFilters.CssClass = "k-button";

            string parameter = "";
            
            //iterate through reportfilters list
            foreach (ReportFilter rf in ReportFilters)
            {

                String
                    FilterField = rf.FilterField,
                    FilterValue = rf.Value,
                    FilterType = rf.Operator,
                    operand = "";                    

                //check that filterfield and type are not empty
                if (!String.IsNullOrEmpty(FilterField) &&
                    !String.IsNullOrEmpty(FilterType))
                {
                    //if parameter is not null, then there must be multiple filters. append the and predicate to the parameter.
                    if (!String.IsNullOrEmpty(parameter))
                    {
                        parameter += " and ";
                    }

                    //switch operator with correct SQL expression
                    switch (FilterType.ToUpper())
                    {
                        case "EQUALS":
                            operand = "=";
                            break;

                        case "LESS THAN":
                            operand = "<";
                            break;

                        case "GREATER THAN":
                            operand = ">";
                            break;

                        case "CONTAINS":
                            operand = "LIKE";
                            FilterValue = "%" + FilterValue + "%";
                            break;

						case "IN":
							operand = "IN";
							string newFiltervalue = string.Empty;
							foreach (string filtVal in FilterValue.Split(','))
							{

								newFiltervalue += "'" +filtVal+"',";
							}
							FilterValue = newFiltervalue.TrimEnd(',') ;
							break;

						case "NOT EQUAL TO":
                            operand = "<>";
                            break;

                        default:
                            break;
                    }
					//build SQL where clause list
					if (operand == "IN")
					{
						parameter += "[" + FilterField + "] " + operand + " (" + FilterValue + ")";
					}
					else
					{
						parameter += "[" + FilterField + "] " + operand + " '" + FilterValue + "'";
					}
				//	parameter += "[" + FilterField + "] " + operand + " ('" + "02CV758001B','02CV758001A" + "')";
				}

            }
			//create report parameter called whereclause (as defined on the report). inject the parameter value into the report and refresh it.
			ReportParameter WhereClause = new ReportParameter();
                WhereClause.Name = "WhereClause";
			
			 WhereClause.Values.Add(parameter);
			KeyListViewer.ServerReport.SetParameters(
                new ReportParameter[] { WhereClause });
                KeyListViewer.ServerReport.Refresh();
            
        }

        //Method to remove parameters from the SSRS report, called by the clear filters button
        protected void clearReportFilters(object sender, EventArgs e)
        {
            ApplyFilters.CssClass = "k-button notvisible";
            ClearFilters.CssClass = "k-button notvisible";

            ReportFilters.Clear();                                  //clear the ReportFilters object
            ReportParameter WhereClause = new ReportParameter();
            WhereClause.Name = "WhereClause";
            WhereClause.Values.Add("");                             //Set WhereClause value to an empty string
            KeyListViewer.ServerReport.SetParameters(
            new ReportParameter[] { WhereClause });                 //Inject parameter into SSRS
            KeyListViewer.ServerReport.Refresh();                   //Refresh report

            
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}


