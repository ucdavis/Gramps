<%@ Page Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password
</asp:Content>

<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Change Password</h2>
    <p>
        Use the form below to change your password. 
    </p>
    <p>
        New passwords are required to be a minimum of <%: ViewData["PasswordLength"] %> characters in length.
    </p>

    <% using (Html.BeginForm()) { %>
        <%: Html.ValidationSummary(true, "Password change was unsuccessful. Please correct the errors and try again.") %>
        <%= Html.AntiForgeryToken() %>
        <div>
            <fieldset>
                <legend><strong>Account Information</strong></legend>
                
                <ul>
                <li>
                <div class="editor-label">
                    <strong><%: Html.LabelFor(m => m.OldPassword) %></strong>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.OldPassword, new { style = "width: 25em" })%>
                    <%: Html.ValidationMessageFor(m => m.OldPassword) %>
                </div>
                </li>
                <li>
                <div class="editor-label">
                    <strong><%: Html.LabelFor(m => m.NewPassword) %></strong>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.NewPassword, new { style = "width: 25em" })%>
                    <%: Html.ValidationMessageFor(m => m.NewPassword) %>
                </div>
                </li>
                <li>
                <div class="editor-label">
                    <strong><%: Html.LabelFor(m => m.ConfirmPassword) %></strong>
                </div>
                <div class="editor-field">
                    <%: Html.PasswordFor(m => m.ConfirmPassword) %>
                    <%: Html.ValidationMessageFor(m => m.ConfirmPassword) %>
                </div>
                </li>
                </ul>
                <p>
                    <input type="submit" value="Change Password" />
                </p>
            </fieldset>
        </div>
    <% } %>
</asp:Content>
