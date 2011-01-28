<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EditorViewModel>" %>
<%@ Import Namespace="Gramps.Controllers.ViewModels" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Reviewer
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>CreateReviewer</h2>
    <%= Html.ClientSideValidation<Gramps.Core.Domain.Editor>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>

        <fieldset>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Editor.ReviewerEmail) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Editor.ReviewerEmail)%>
                <%: Html.ValidationMessageFor(model => model.Editor.ReviewerEmail)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Editor.ReviewerName)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Editor.ReviewerName)%>
                <%: Html.ValidationMessageFor(model => model.Editor.ReviewerName)%>
            </div>
            
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

