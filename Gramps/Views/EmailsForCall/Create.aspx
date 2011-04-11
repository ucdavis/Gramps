<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailsForCallViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%= Html.ValidationSummary("Please correct all errors below") %>
	    <%= Html.ClientSideValidation<EmailsForCall>("EmailsForCall")%>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>

        <fieldset>
            <legend>Fields</legend>
            <ul>
            <li>
                <%: Html.LabelFor(model => model.EmailsForCall.Email) %>
                <%: Html.TextBoxFor(model => model.EmailsForCall.Email)%>
                <%--<%: Html.ValidationMessageFor(model => model.EmailsForCall.Email)%>--%>
                <%= Html.ValidationMessage("Email") %>
            </li>
            </ul>      
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

