<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ALIS2.Filter.Page.Site._Default" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="Scripts/jquery-1.7.1.js" type="text/javascript"></script>
    <script src="Scripts/ALIS2filterpage.js" type="text/javascript"></script>
    <script src="https://kendo.cdn.telerik.com/2015.3.1111/js/kendo.all.min.js"></script>
    <script type="text/javascript">
        function applyFilters() {
            var Filters = [];
            $("#Filters > tbody > tr").each(function (i, el) {
                var $tds = $(this).find('td  input'),
                    Filter =
                        {
                            FilterField: $tds.eq(0).val(),
                            Operator: $tds.eq(1).val(),
                            Value: $tds.eq(2).val()
                        };
                Filters.push(Filter);
            })

            $.ajax({
                type: "POST",
                cache: false,
                dataType: 'json',
                data: JSON.stringify({ "Filters": Filters }),
                url: 'DataService.asmx/ApplyFilter',
                contentType: "application/json; charset=utf-8"
            });
        };

        function clearFilters() {
            $("#Filters tbody").empty();
        };
    </script>
    <style>
        /*Styles for Kendo drop down and autocomplete boxes*/
        .k-autocomplete.k-header {
            width: 300px;
        }

        .filtersection table {
            margin: 3px 0px 5px 0px;
        }

        .k-input, .k-popup {
            border-radius: 2px;
            font-family: Tahoma !important;
            font-size: 12px !important;
            color: #4D4D4D !important;
            background-color: lightgoldenrodyellow !important;
        }

        .k-header {
            padding-top: 1px !important;
            padding-bottom: 1px !important;
        }

        .value td {
            padding-top: 2px !important;
            padding-bottom: 2px !important;
        }

        .k-item {
            border-width: 2px !important;
        }

        .k-button {
            box-shadow: none !important;
            border-color: #e6e6e6 !important;
            font-family: Tahoma !important;
            font-size: 12px !important;
            color: #4D4D4D !important;
        }

        .notvisible {
            visibility: hidden !important;
        }

        .filtersection {
            background-color: #395D7D;
            border-radius: 2px;
            margin: 2px 2px 2px 2px;
            padding: 5px 5px 5px 5px;
        }
    </style>
    <link rel="Stylesheet" href="Styles/kendo.common.min.css" />
    <link rel="Stylesheet" href="Styles/kendo.material.min.css" />
    <title></title>
</head>
<body>



    <form id="form1" runat="server">
        <asp:ScriptManager ID="KeyListScriptManager" runat="server"></asp:ScriptManager>
        <div>
            <p></p>
            <div class="filtersection">
                <!-- Section for Filter Table: note the tbody section is used for client side filter row insertions -->
                <input id="AddFilter" type="button" value="Add Filter" class="k-button" />
                <table id="Filters">
                    <thead>
                        <tr>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                    </tbody>
                </table>

                <div>
                    <!-- Update panel used to prevent destruction of Filter Table when the postback for the below buttons and SSRS report occurs -->
                    <asp:UpdatePanel ID="uPanel1" runat="server">
                        <ContentTemplate>
                            <!-- button to apply filters, calls client side function to populate ReportFilters variable on the codebehind, and then calls a codebehind function that sets the SSRS report filters -->
                            <asp:Button ID="ApplyFilters" runat="server" CssClass="k-button" OnClientClick="applyFilters()" OnClick="filterReport" Text="Apply All" />
                            <!-- button to destroy filters, calls client side function to clear ReportFilters variable on the codebehind, and then calls a codebehind function that destroys the SSRS report filters -->
                            <asp:Button ID="ClearFilters" runat="server" CssClass="k-button" OnClientClick="clearFilters()" OnClick="clearReportFilters" Text="Clear All Filters" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div style="height: 100%">
                <!-- Place in seperate update panel to separate post back events -->
                <!-- SSRS report placeholder -->
                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                    <ContentTemplate>
                        <rsweb:ReportViewer
                            ID="KeyListViewer"
                            AsyncRendering="true"
                            BackColor="AliceBlue"
                            runat="server"
                            ShowRefreshButton="False"
                            PageCountMode="Actual"
                            SizeToReportContent="false"
                            ProcessingMode="Remote"
                            Width="95%" Height="1000px">
                        </rsweb:ReportViewer>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>
