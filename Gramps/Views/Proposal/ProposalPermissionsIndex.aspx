<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalPermissionsViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Proposal Permissions
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Proposal Permissions</h2>
    <br/>
    <p>
        <%: Html.ActionLink<ProposalController>(a => a.AddPermission(Model.Proposal.Guid), "Grant Access", new {@class="button"}) %>
        <%: Html.ActionLink<ProposalController>(a => a.Home(), "Back", new {@class="button"}) %>
    </p>
    
    
        <fieldset>
    <legend><strong>People Granted Access</strong></legend>
<% Html.Grid(Model.Permissions) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.EditPermission(x.Id), " ", new { @class = "edit_button" })%>        
				<%}).Title("Edit");
            col.Bound(x => x.Email);
            col.Bound(x => x.DateAdded).Format("{0:d/M/yyyy hh:mm tt}").Title("Added");
            col.Bound(x => x.DateUpdated).Format("{0:d/M/yyyy hh:mm tt}").Title("Updated");
            col.Bound(x => x.AllowReview).Title("Allow Review");
            col.Bound(x => x.AllowEdit).Title("Allow Edit");
            col.Bound(x => x.AllowSubmit).Title("Allow Submit");
            })
            //.Pageable()
            //.Sortable()
            .Render(); 
        %>
        </fieldset>
      
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

