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
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.UserName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.UserName) %>
                <%: Html.ValidationMessageFor(model => model.UserName) %>
            </div>
            <br />
            
            <p>
                <%= Html.GenerateCaptcha() %>
                <%: Html.ValidationMessage("Captcha")%>
            </p>

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

