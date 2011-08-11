<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Models.ForgotPasswordModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	ForgotPassword
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ForgotPassword</h2>

	<%= Html.ClientSideValidation<Gramps.Models.ForgotPasswordModel>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("") %>

        <fieldset>
            <ul>
            <li>
            <div class="editor-label">
                <strong><%: Html.LabelFor(model => model.UserName) %></strong>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.UserName, new { style = "width: 25em" })%>
                <%: Html.ValidationMessageFor(model => model.UserName) %>
            </div>
            </li>
            
            
            <li>
                <%= Html.GenerateCaptcha() %>
                <%: Html.ValidationMessage("Captcha")%>
            </li>
            </ul>
            <p>
                <input type="submit" value="Reset Password" />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

