﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Models.LogOnModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Log On
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Log On</h2>

    


    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
        <%= Html.AntiForgeryToken() %>
        <div>
            <fieldset>
                <legend><strong>Account Information</strong></legend>
                <ul>
                <li>
                <div class="editor-label">
                    <strong><%: Html.LabelFor(m => m.UserName) %></strong>
                </div>
                <div class="editor-field">
                    <%: Html.TextBoxFor(m => m.UserName, new { style = "width: 25em" })%>
                    <%: Html.ValidationMessageFor(m => m.UserName) %>
                </div>
                </li>
                <li>
                <div class="editor-label">
                    <strong><%: Html.LabelFor(m => m.Password) %></strong>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.Password, new { style = "width: 25em" })%>
                    <%: Html.ValidationMessageFor(m => m.Password) %>
                    <%: Html.ActionLink<PublicController>(a => a.ForgotPassword(), "Forgot Password?") %>
                </div>
                </li>
                <li>
                <div class="editor-label">
                    <%: Html.CheckBoxFor(m => m.RememberMe) %>
                    <%: Html.LabelFor(m => m.RememberMe) %>
                </div>
                </li>
                </ul>
                <p>
                    <input type="submit" value="Log On" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
