<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.QuestionViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

        <%= Html.AntiForgeryToken() %>
        <%= Html.ValidationSummary("Please correct all errors below") %>
	    <%= Html.ClientSideValidation<Question>("Question")%>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>

        <span style="display:none;" id="TypesWithOptions"><%= string.Join(",", Model.QuestionTypes.Where(a => a.HasOptions).Select(a => a.Id.ToString()).ToArray()) %></span>
