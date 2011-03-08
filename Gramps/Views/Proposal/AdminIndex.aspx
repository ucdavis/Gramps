<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposals Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Admin Proposals Index</h2>

    <div id="filter_container">
        <h3><a href="#">Filters</a></h3>
        <% using (Html.BeginForm("AdminIndex", "Proposal", FormMethod.Post)) { %>
            <%= Html.AntiForgeryToken() %>
            <ul>
            <li>
            <span id = "DecissionSpan">
            <label for="Decission"></label>
                <input type="radio" id="Approved" name="filterDecission" value="<%: StaticValues.RB_Decission_Approved %>" "<%=Model.FilterDecission == StaticValues.RB_Decission_Approved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="Denied" name="filterDecission" value="<%: StaticValues.RB_Decission_Denied %>" "<%= Model.FilterDecission == StaticValues.RB_Decission_Denied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="NotDecied" name="filterDecission" value="<%: StaticValues.RB_Decission_NotDecided %>" "<%= Model.FilterDecission == StaticValues.RB_Decission_NotDecided ? "checked" : string.Empty %>" /><label for="notDecied">Not Decied</label>
                <input type="radio" id="All" name="filterDecission" value="All" "<%= string.IsNullOrWhiteSpace(Model.FilterDecission) || Model.FilterDecission == "All" ? "checked" : string.Empty %>" /><label for="all">Not Filtered</label>
            </span>
            </li>
            <li>
            <span id = "NotifiedSpan">
            <label for="Notified"></label>
                <input type="radio" id="Notified" name="filterNotified" value="<%:StaticValues.RB_Notified_Notified%>" "<%=Model.FilterNotified == StaticValues.RB_Notified_Notified ? "checked" : string.Empty%>" /><label for="notified">Notified</label>
                <input type="radio" id="NotNotified" name="filterNotified" value="<%:StaticValues.RB_Notified_NotNotified%>" "<%= Model.FilterNotified == StaticValues.RB_Notified_NotNotified ? "checked" : string.Empty %>" /><label for="notNotified">Not Notified</label>                
                <input type="radio" id="NotifiedAll" name="filterNotified" value="All" "<%= string.IsNullOrWhiteSpace(Model.FilterNotified) || Model.FilterNotified == "All" ? "checked" : string.Empty %>" /><label for="notifiedAll">Not Filtered</label>
            </span>
            </li>
            <li>
            <span id = "SubmittedSpan">
            <label for="Submitted"></label>
                <input type="radio" id="Submitted" name="filterSubmitted" value="<%: StaticValues.RB_Submitted_Submitted %>" "<%=Model.FilterSubmitted == StaticValues.RB_Submitted_Submitted ? "checked" : string.Empty%>" /><label for="submitted">Submitted</label>
                <input type="radio" id="NotSubmitted" name="filterSubmitted" value="<%: StaticValues.RB_Submitted_NotSubmitted %>" "<%= Model.FilterSubmitted == StaticValues.RB_Submitted_NotSubmitted ? "checked" : string.Empty %>" /><label for="notSubmitted">Not Submitted</label>                
                <input type="radio" id="SubmittedAll" name="filterSubmitted" value="All" "<%= string.IsNullOrWhiteSpace(Model.FilterSubmitted) || Model.FilterSubmitted == "All" ? "checked" : string.Empty %>" /><label for="submittedAll">Not Filtered</label>
            </span>
            </li>
            <span id = "WarnedSpan">
            <label for="Warned"></label>
                <input type="radio" id="Warned" name="filterWarned" value="<%: StaticValues.RB_Warned_Warned %>" "<%=Model.FilterWarned == StaticValues.RB_Warned_Warned ? "checked" : string.Empty%>" /><label for="warned">Warned</label>
                <input type="radio" id="NotWarned" name="filterWarned" value="<%: StaticValues.RB_Warned_NotWarned %>" "<%= Model.FilterWarned == StaticValues.RB_Warned_NotWarned ? "checked" : string.Empty %>" /><label for="notWarned">Not Warned</label>                
                <input type="radio" id="WarnedAll" name="filterWarned" value="All" "<%= string.IsNullOrWhiteSpace(Model.FilterWarned) || Model.FilterWarned == "All" ? "checked" : string.Empty %>" /><label for="warnedAll">Not Filtered</label>
            </span>
            </li>
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
				<%: Html.ActionLink<ProposalController>(a => a.AdminEdit(x.Id, Model.CallForProposal.Id), " ", new { @class = "edit_button" })%>           
				<%}).Title("Edit");
			col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.AdminDetails(x.Id, Model.CallForProposal.Id), " ", new { @class = "details_button" })%>           
				<%}).Title("Details");
            col.Bound(x => x.Seq);
            col.Bound(x => x.Email);
            col.Bound(x => x.LastViewedDate).Title("Viewed Date");
            col.Bound(x => x.Approved).Title("Aprv.");
            col.Bound(x => x.Denied);
            col.Bound(x => x.NotifiedOfDecission).Title("Notified");
            col.Bound(x => x.Submitted).Title("Sub.");
            col.Bound(x => x.WarnedOfClosing).Title("Warned");                
            col.Bound(x => x.SubmittedDate);
            col.Bound(x => x.CreatedDate);  
            })
            .DataBinding(binding => binding.Server().Select<ProposalController>(a => a.AdminIndex(Model.CallForProposal.Id, Model.FilterDecission, Model.FilterNotified, Model.FilterSubmitted, Model.FilterWarned, Model.FilterEmail)))
            .Pageable()
            .Sortable(s => s.OrderBy(a => a.Add(b => b.LastViewedDate)))
            .Render(); 
        %>
    <br/>
    <div>
    <% using (Html.BeginForm<ProposalController>(b => b.SendCall(Model.CallForProposal.Id, Model.Immediate), FormMethod.Post)){%>
        <%:Html.AntiForgeryToken()%>          
        
        <%= Html.SubmitButton("Submit", "Send Warning of Close")%>        
    <%}%>  
    </div>
    <div>
    <% using (Html.BeginForm<ProposalController>(b => b.SendDecision(Model.CallForProposal.Id, Model.Immediate), FormMethod.Post)){%>
        <%:Html.AntiForgeryToken()%>          
        <%= Html.SubmitButton("Submit", "Send Decision")%>        
    <%}%> 
    </div>
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

