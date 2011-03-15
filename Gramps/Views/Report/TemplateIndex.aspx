<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.TemplateReportListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Report Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Report Index</h2><br />


<p>
    <%: Html.ActionLink<ReportController>(a => a.CreateForTemplate(Model.TemplateId, Model.CallForProposalId), "Create", new {@class="button"}) %>
</p>

<% Html.Grid(Model.ReportList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink<ReportController>(a => a.EditForTemplate(x.Id, Model.TemplateId, Model.CallForProposalId), " ", new { @class = "edit_button" })%>           
				<%}).Title("Edit");
                col.Bound(x => x.Name);
                col.Bound(x => x.ShowUnsubmitted).Title("Show All");
                col.Bound(x => x.ReportColumns.Count).Title("# of Columns");
            col.Template(x => { %>                                                       
                <% using (Html.BeginForm("Delete", "Report", FormMethod.Post)) { %>
                    <%= Html.AntiForgeryToken() %>
                    <%: Html.Hidden("ReportId", x.Id) %>
                    <%: Html.Hidden("TemplateId", Model.TemplateId) %>
                    <%: Html.Hidden("CallForProposalId", Model.CallForProposalId) %>
                    <%= Html.SubmitButton("Submit", " ", new {@class="remove_button"}) %>                                                                           
                <%}%>                                       
            <% }).Title("Delete");
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

