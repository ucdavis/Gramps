<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

    <h2>Proposal Detail Number: <%:Model.Proposal.Sequence %></h2>

    <fieldset>
        <legend>Details</legend>
        <li>
            <%: Html.Label("Proposal Unique ID:") %>
            <%: Model.Proposal.Guid %>
        </li>
        <li>
            <%: Html.Label("Email:") %>
            <%: Model.Proposal.Email%>
        </li>
        <li>
        <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Approved%>" <%=Model.Proposal.IsApproved ? "checked" : string.Empty%> /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Denied%>" <%= Model.Proposal.IsDenied ? "checked" : string.Empty %> /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_NotDecided%>" <%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %> /><label for="notDecieded">Not Decided</label>
        </span>
        </li>
        <li>
            <%= Html.CheckBox("IsNotified", Model.Proposal.IsNotified, new { @disabled = "True" })%> <%: Html.Encode("Notified")%>
        </li>
        <li>
            <%= Html.CheckBox("IsSubmitted", Model.Proposal.IsSubmitted, new { @disabled = "True" })%> <%: Html.Encode("Submitted") %>
        </li>
        <li>
            <%: Html.Label("Requested Amount:")%>
            <%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%>
        </li>
        <li>
            <%: Html.Label("Approved Amount:")%>
            <%: String.Format("{0:F}", Model.Proposal.ApprovedAmount)%>
        </li>
        <li>
            <%: Html.Label("Created Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.CreatedDate)%>
        </li>
        <li>
            <%: Html.Label("Submitted Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%>
        </li>
        <li>
            <%: Html.Label("Notified Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.NotifiedDate)%>
        </li>

        <h2>Answers</h2>

        <%Html.RenderPartial("ProposalAnswerDetails"); %>
         
    </fieldset>