<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.CallNavigationViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>
    <ul class="btn">
        <li>
            <%=Html.ActionLink<CallForProposalController>(a => a.Index(null, null, null), "Call Index") %>
        </li>
        <li>
            <%: Html.ActionLink<CallForProposalController>(a => a.Launch(Model.CallForProposal.Id), "Details")%>            
        </li>
        <li>
            <%: Html.ActionLink<EmailsForCallController>(a => a.SendCall(Model.CallForProposal.Id), "Send Call")%>            
        </li>
        <li>
            <%: Html.ActionLink<EditorController>(a => a.SendCall(Model.CallForProposal.Id), "Notify Reviewers")%>            
        </li>
        <li>
            <%: Html.ActionLink<EmailQueueController>(a => a.Index(Model.CallForProposal.Id), "Emails")%>            
        </li>
        <li>
            <%: Html.ActionLink<ProposalController>(a => a.AdminIndex(Model.CallForProposal.Id), "Proposals")%>            
        </li>
        <li>
            <%: Html.ActionLink<CallForProposalController>(a => a.Edit(Model.CallForProposal.Id), "Reporting")%>            
        </li>
    </ul>