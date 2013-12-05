<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.TemplateViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Template
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Edit Template</h2>

	<%= Html.ClientSideValidation<Gramps.Controllers.ViewModels.TemplateViewModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Fields</legend>
        <ul>
            <li>
                <%:Html.Label("Name:") %>
                <%: Html.TextBoxFor(model => model.Template.Name, new { @class = "BigWidth" })%>
                <%: Html.ValidationMessageFor(model => model.Template.Name)%>
            </li>
            <li>
                <%: Html.CheckBoxFor(a => a.Template.IsActive) %>
                <%: Html.Encode("Active") %>
            </li>
            <li>
                <%: Html.CheckBoxFor(a => a.Template.HideInvestigators) %>
                <%: Html.Encode("Hide Investigators (do not allow them to be entered on the proposal)") %>
            </li>
        </ul>
        <p>
            <input type="submit" value="Save" />
        </p>
    </fieldset>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

