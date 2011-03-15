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
				<%: Html.ActionLink<ReportController>(a => a.EditForCall(x.Id, null, Model.CallForProposal.Id), " ", new { @class = "edit_button" })%>           
				<%}).Width(25).Title("Edit");
            col.Template(x => {%>
                <%: Html.ActionLink<ReportController>(a => a.Launch(x.Id, Model.CallForProposal.Id), " ", new { @class = "launch_button" })%>          
				<%}).Width(30).Title("Launch");
            col.Template(x => {%>
                <%: Html.ActionLink<ReportController>(a => a.ExportExcell(x.Id, Model.CallForProposal.Id), " ", new { @class = "excell_button" })%>          
				<%}).Width(30).Title("Export");      
                col.Bound(x => x.Name);
                col.Bound(x => x.ShowUnsubmitted).Title("Show All");
                col.Bound(x => x.ReportColumns.Count).Width(45).Title("# of Columns");
            col.Template(x => { %>                                                       
                <% using (Html.BeginForm("Delete", "Report", FormMethod.Post)) { %>
                    <%= Html.AntiForgeryToken() %>
                    <%: Html.Hidden("ReportId", x.Id) %>
                    <%: Html.Hidden("TemplateId", null) %>
                    <%: Html.Hidden("CallForProposalId", Model.CallForProposal.Id) %>
                    <%= Html.SubmitButton("Submit", " ", new {@class="remove_button"}) %>                                                                           
                <%}%>                                       
            <% }).Width(30).Title("Delete");
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


