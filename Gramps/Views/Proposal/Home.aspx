<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalPublicListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Home
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Home</h2>

    <%if(Model.IsReviewer) {%>
       <fieldset>
            <legend><strong>Active Call For Proposals where you are a reviewer</strong></legend>
            <% Html.Grid(Model.CallForProposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.ReviewerIndex(x.Id, null, null), " ", new { @class = "details_button" })%>           
				<%}).Title("Details");
                                col.Bound(x => x.Name);
                                col.Bound(x => x.CreatedDate).Format("{0:d/M/yyyy hh:mm tt}");
                                col.Bound(x => x.EndDate);
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>
        </fieldset>
        <br />
    <%}%>

    <fieldset>
    <legend><strong>Your Proposals</strong></legend>
<% Html.Grid(Model.UsersProposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
                <%if(!x.IsSubmitted) {%>
				<%: Html.ActionLink<ProposalController>(a => a.Edit(x.Guid), " ", new { @class = "edit_button" })%>   
                <%}%>        
				<%}).Title("Edit");
            col.Template(x =>{%>
                <%if(x.IsSubmitted) {%>
                <%: Html.ActionLink<ProposalController>(a => a.Details(x.Guid), " ", new { @class = "details_button" })%>
                <%}%>
                <%}).Title("Details");
                col.Bound(x => x.CreatedDate).Format("{0:d/M/yyyy hh:mm tt}");
                                col.Bound(x => x.IsSubmitted);
                                col.Bound(x => x.Guid).Title("Proposal Id");
                                col.Bound(x => x.CallForProposal.EndDate);
                                col.Bound(x => x.CallForProposal.Name);
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

