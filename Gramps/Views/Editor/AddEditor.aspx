<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.AddEditorViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	AddEditor
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Add Editor</h2>

    <% using (Html.BeginForm("AddEditor", "Editor", FormMethod.Post)) { %>

        <%= Html.AntiForgeryToken() %>
        <ul class="registration_form">
            <%: Html.HiddenFor(a => a.TemplateId) %>
            <%: Html.HiddenFor(a => a.CallForProposalId) %>
            
            <li><strong>User:</strong>
                <%= this.Select("userId").Options(Model.Users, x=>x.Id, x=>x.FullName).Selected(Model.User != null ? Model.User.Id : 0).FirstOption("--Select a User--") %>
            </li>
            <li>
                <strong></strong>
                <%= Html.SubmitButton("submit", "Save") %>
            </li>

        </ul>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>
