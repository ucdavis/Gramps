<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalReviewerListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Reviewer Proposals Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%: Html.ActionLink<ProposalController>(a => a.ReviewerIndex(Model.CallForProposal.Id, null, null), "Proposals")%>            
        </li>
    </ul>

    <h2>Reviewer Submitted Proposals Index</h2>

        <div id="filter_container">
        <h3><a href="#">Filters</a></h3>
        <% using (Html.BeginForm("ReviewerIndex", "Proposal", FormMethod.Post)) { %>
            <%= Html.AntiForgeryToken() %>
            <ul>
            <li>
            <span id = "DecissionSpan">
            <label for="Decission"></label>
                <input type="radio" id="Approved" name="filterDecission" value="<%: StaticValues.RB_Decission_Approved %>" "<%=Model.FilterDecission == StaticValues.RB_Decission_Approved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="Denied" name="filterDecission" value="<%: StaticValues.RB_Decission_Denied %>" "<%= Model.FilterDecission == StaticValues.RB_Decission_Denied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="NotDecided" name="filterDecission" value="<%: StaticValues.RB_Decission_NotDecided %>" "<%= Model.FilterDecission == StaticValues.RB_Decission_NotDecided ? "checked" : string.Empty %>" /><label for="notDecided">Not Decided</label>
                <input type="radio" id="All" name="filterDecission" value="All" "<%= string.IsNullOrWhiteSpace(Model.FilterDecission) || Model.FilterDecission == "All" ? "checked" : string.Empty %>" /><label for="all">Not Filtered</label>
            </span>
            <li>
            <%: Html.Label("Email Contains(leave empty to not filter):") %>
            <%: Html.TextBoxFor(a => a.FilterEmail) %>
            </li>
            <li><strong></strong><%= Html.SubmitButton("Submit", "Filter") %></li>
        </ul>
        <% } %>
    </div>

    <% Html.Grid(Model.Proposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
			col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.ReviewerDetails(x.Id, Model.CallForProposal.Id), " ", new { @class = "details_button" })%>           
                <%: Html.ActionLink<PrintController>(a => a.ProposalReviewer(Model.CallForProposal.Id, x.Id), " ", new { @class = "small_print_button" })%>
				<%}).Title("Actions");
            col.Bound(x => x.Seq);    
            col.Bound(x => x.Email);
            col.Bound(x => x.LastViewedDate).Format("{0:d/M/yyyy hh:mm tt}").Title("Viewed Date");
            col.Bound(x => x.Approved);
            col.Bound(x => x.Denied);
            col.Bound(x => x.SubmittedDate).Format("{0:d/M/yyyy hh:mm tt}");
            col.Bound(x => x.CreatedDate).Format("{0:d/M/yyyy hh:mm tt}");  
            })
            .DataBinding(binding => binding.Server().Select<ProposalController>(a => a.ReviewerIndex(Model.CallForProposal.Id, Model.FilterDecission, Model.FilterEmail)))
            .Pageable()
            .Sortable(s => s.OrderBy(a => a.Add(b => b.LastViewedDate)))
            .Render(); 
        %>
    <br />
    <div>
        <%: Html.ActionLink<PrintController>(a => a.ProposalReviewer(Model.CallForProposal.Id, null), " ", new { @class = "big_print_button" })%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
     <script type="text/javascript">
         $(function () {
             $("#filter_container").accordion({ collapsible: true, autoHeight: false, active: false });
         });

         $(document).ready(function () {
             $(".big_print_button").bt("Print all submitted proposals to PDF");
             $(".small_print_button").bt("Print this proposal to PDF");
         });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

