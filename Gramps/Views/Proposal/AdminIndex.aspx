<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposals Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Admin Proposals Index</h2>



<% Html.Grid(Model.Proposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>           
				<%});
			col.Template(x => {%>
				<%: Html.ActionLink("Details", "Details", new { id = x.Id }) %>           
				<%});
            col.Bound(x => x.Email);
            col.Bound(x => x.CreatedDate);
            col.Bound(x => x.Approved);
            col.Bound(x => x.Denied);
            col.Bound(x => x.Submitted);
            col.Bound(x => x.SubmittedDate);
            col.Bound(x =>x.LastViewedDate);
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

