﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.NavigationViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>
    <ul class="btn">
        <li>
            <%if (Model.IsTemplate){%>
                <%=Html.ActionLink<TemplateController>(a => a.Index(), "Template List")%>
            <%}%>
        </li>
        <li>
            <%if (Model.IsTemplate){%>
                <%:Html.ActionLink<TemplateController>(a => a.Edit((int)Model.TemplateId), "Details")%>
            <%}else{%>
            <%--<%:Html.ActionLink<TemplateController>(a => a.Edit((int) Model.templateId), "Details")%>--%>
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