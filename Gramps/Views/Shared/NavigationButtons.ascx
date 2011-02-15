<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.NavigationViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>
    <ul class="btn">
        <li>
            <%if (Model.IsTemplate){%>
                <%=Html.ActionLink<TemplateController>(a => a.Index(), "Template List")%>
            <%}%>
            <%else if (Model.IsCallForProposal){%>
                <%=Html.ActionLink<CallForProposalController>(a => a.Index(), "Call Index") %>
            <%}%>
        </li>
        <li>
            <%if (Model.IsTemplate){%>
                <%:Html.ActionLink<TemplateController>(a => a.Edit((int)Model.TemplateId), "Details")%>
            <%}else if(Model.IsCallForProposal){%>
                <%:Html.ActionLink<CallForProposalController>(a => a.Edit((int) Model.CallForProposalId), "Details")%>
            <%}%>
        </li>
        <li>
            <%: Html.ActionLink<EditorController>(a => a.Index(Model.TemplateId, Model.CallForProposalId), "Editors/Reviewers")%>            
        </li>
        <li>
            <%: Html.ActionLink<EmailsForCallController>(a => a.Index(Model.TemplateId, Model.CallForProposalId), "Call List")%>
        </li>
        <li>
            <%: Html.ActionLink<EmailTemplateController>(a => a.Index(Model.TemplateId, Model.CallForProposalId), "Email Templates")%>
        </li>
        <li>
            <%: Html.ActionLink<QuestionController>(a => a.Index(Model.TemplateId, Model.CallForProposalId), "Questions")%>
        </li>
    </ul>