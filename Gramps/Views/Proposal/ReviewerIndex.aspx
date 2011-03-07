﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalReviewerListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

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
                <input type="radio" id="Approved" name="filterDecission" value="Approved" "<%=Model.FilterDecission == "Approved" ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="Denied" name="filterDecission" value="Denied" "<%= Model.FilterDecission == "Denied" ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="NotDecied" name="filterDecission" value="NotDecied" "<%= Model.FilterDecission == "NotDecied" ? "checked" : string.Empty %>" /><label for="notDecied">Not Decied</label>
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
				<%}).Title("Details");
            col.Bound(x => x.Seq);    
            col.Bound(x => x.Email);
            col.Bound(x => x.LastViewedDate).Title("Viewed Date");
            col.Bound(x => x.Approved);
            col.Bound(x => x.Denied);              
            col.Bound(x => x.SubmittedDate);
            col.Bound(x => x.CreatedDate);  
            })
            .DataBinding(binding => binding.Server().Select<ProposalController>(a => a.ReviewerIndex(Model.CallForProposal.Id, Model.FilterDecission, Model.FilterEmail)))
            .Pageable()
            .Sortable(s => s.OrderBy(a => a.Add(b => b.LastViewedDate)))
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
     <script type="text/javascript">
         $(function () {
             $("#filter_container").accordion({ collapsible: true, autoHeight: false, active: false });
         });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

