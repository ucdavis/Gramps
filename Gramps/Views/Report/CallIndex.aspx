<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallReportListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Report Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Report Index</h2><br />


<p>
    <%: Html.ActionLink<ReportController>(a => a.CreateForCall(null, Model.CallForProposal.Id), "Create", new {@class="button"}) %>
</p>

<% Html.Grid(Model.ReportList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>           
				<%});
                col.Bound(x => x.Name);
                col.Bound(x => x.ReportColumns.Count).Title("# of Columns");
                col.Template(x => {%>
				<%: Html.ActionLink("Delete", "Delete", new { id = x.Id }) %>           
				<%});
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>


