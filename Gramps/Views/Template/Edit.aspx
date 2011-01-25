<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.TemplateViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%= Html.ActionLink<TemplateController>(a => a.Index() , "Back to List") %>
        </li>
        <li>
            <%: Html.ActionLink<TemplateController>(a => a.Edit(Model.Template.Id), "Details")%>
        </li>
        <li>
            <%: Html.ActionLink<TemplateController>(a => a.EditEditors(Model.Template.Id), "Editors/Reviewers")%>
        </li>

    </ul>

    <h2>Details</h2>

	<%= Html.ClientSideValidation<Gramps.Controllers.TemplateViewModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary(true) %>

        <fieldset>
            <legend>Fields</legend>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Template.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Template.Name)%>
                <%: Html.ValidationMessageFor(model => model.Template.Name)%>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Template.IsActive)%>
            </div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.Template.IsActive)%>
                <%: Html.ValidationMessageFor(model => model.Template.IsActive)%>
            </div>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

