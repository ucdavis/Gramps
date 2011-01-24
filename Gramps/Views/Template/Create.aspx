<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.TemplateViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        <%= Html.AntiForgeryToken() %>
        <fieldset>
            <legend>Fields</legend>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Template.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Template.Name)%>
                <%: Html.ValidationMessageFor(model => model.Template.Name)%>
            </div>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%: Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

