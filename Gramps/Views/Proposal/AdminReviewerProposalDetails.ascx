<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

    <h2>Proposal Detail Number: <%:Model.Proposal.Sequence %></h2>

    <fieldset>
        <legend>Fields</legend>
        <div class="display-label">Guid</div>
        <div class="display-field"><%: Model.Proposal.Guid %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%: Model.Proposal.Email%></div>
        
        <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" disabled="true" value="<%:StaticValues.Approved%>" "<%=Model.Proposal.IsApproved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.Denied%>" "<%= Model.Proposal.IsDenied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.NotDecided%>" "<%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %>" /><label for="notDecieded">Not Decided</label>
        </span>
<%--        <div class="display-field">
            <%= Html.CheckBox("IsApproved", Model.Proposal.IsApproved, new { @disabled = "True"}) %> <%: Html.Encode("Approved")%>
        </div>
        <div class="display-field">
            <%= Html.CheckBox("IsDenied", Model.Proposal.IsDenied, new { @disabled = "True" })%> <%: Html.Encode("Denied")%>
        </div>--%>
        <div class="display-field">
            <%= Html.CheckBox("IsNotified", Model.Proposal.IsNotified, new { @disabled = "True" })%> <%: Html.Encode("Notified")%>
        </div>
        <div class="display-field">
            <%= Html.CheckBox("IsSubmitted", Model.Proposal.IsSubmitted, new { @disabled = "True" })%> <%: Html.Encode("Submitted") %>
        </div>

<%--        <div class="display-label">IsApproved</div>
        <div class="display-field"><%: Model.Proposal.IsApproved%></div>
        
        <div class="display-label">IsDenied</div>
        <div class="display-field"><%: Model.Proposal.IsDenied%></div>
        
        <div class="display-label">IsNotified</div>
        <div class="display-field"><%: Model.Proposal.IsNotified%></div>
        
        <div class="display-label">IsSubmitted</div>
        <div class="display-field"><%: Model.Proposal.IsSubmitted%></div>--%>
        
        <div class="display-label">RequestedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%></div>
        
        <div class="display-label">ApprovedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.ApprovedAmount)%></div>
        
        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.CreatedDate)%></div>
        
        <div class="display-label">SubmittedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%></div>
        
        <div class="display-label">NotifiedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.NotifiedDate)%></div>

        <h1>Answers</h1>

        <%Html.RenderPartial("ProposalAnswerDetails"); %>
         
    </fieldset>