<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ReportLaunchViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Launch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("CallNavigationButtons"); %>
    
    <h2><%: Html.Encode(Model.Report.Name) %></h2><br />

    <div id="Report" class="t-widget t-grid">
        <table cellpadding="0">
            <thead>
                <tr>
                <% foreach(var ch in Model.ColumnNames) { %>
                    <td class="t-header"><%= Html.Encode(ch) %></td>
                <% } %>
                </tr>
            </thead>
            <tbody>
                <% foreach(var row in Model.RowValues) { %>
                    <tr>
                    <% foreach(var cell in row) { %> 
                        <td><%: Html.HtmlEncode(cell) %></td>
                    <% } %>
                    </tr>
                <% } %>
                <tr></tr>
            </tbody>
        </table>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

