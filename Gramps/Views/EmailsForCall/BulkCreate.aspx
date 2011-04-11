<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailsForCallViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Bulk Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Bulk Create</h2>

	<%= Html.ClientSideValidation<Gramps.Controllers.ViewModels.EmailsForCallViewModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>
        <fieldset>
            <legend>Paste list of emails</legend>

            <div class="editor-label">
                <%: Html.LabelFor(model => model.BulkLoadEmails) %>
            </div>
            <div class="editor-field">
                <%: Html.TextAreaFor(model => model.BulkLoadEmails, new {@style="width:900px; height:300px"})%>
                <%: Html.ValidationMessageFor(model => model.BulkLoadEmails)%>
            </div>
            <p>
                <input type="submit" value="BulkCreate" />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

