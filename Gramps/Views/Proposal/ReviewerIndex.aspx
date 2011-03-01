<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalReviewerListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Reviewer Proposals Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%: Html.ActionLink<ProposalController>(a => a.ReviewerIndex(Model.CallForProposal.Id), "Proposals")%>            
        </li>
    </ul>

    <h2>Reviewer Proposals Index</h2>



    <% Html.Grid(Model.Proposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
			col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.ReviewerDetails(x.Id, Model.CallForProposal.Id), " ", new { @class = "details_button" })%>           
				<%}).Title("Details");
            col.Bound(x => x.Email);
            col.Bound(x => x.LastViewedDate);
            col.Bound(x => x.Approved);
            col.Bound(x => x.Denied);              
            col.Bound(x => x.SubmittedDate);
            col.Bound(x => x.CreatedDate);  
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

