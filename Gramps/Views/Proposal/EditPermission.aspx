<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalPermissionEditViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Access
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit Access</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        <%: Html.Hidden("id", Model.ProposalPermission.Id) %>
        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <legend><strong>For <%: Model.ProposalPermission.Email  %></strong></legend>
            <p>Note! No Emails will be generated when editing access. You will have to contact them directly.</p>
            <br/>
            <p>Allow Submit grants access to Allow Edit and Allow Review. Allow Edit grants access to Allow Review.</p>
            <p>You may remove all their access. It will still show up in their list, but the will not be able to review it.</p>
            <br/>
            <ul>

               
                <li>
                    <%= Html.CheckBox("ProposalPermission.AllowReview", Model.ProposalPermission.AllowReview)%> <%: Html.Encode("Allow Review") %>
                </li>
                <li>
                    <%= Html.CheckBox("ProposalPermission.AllowEdit", Model.ProposalPermission.AllowEdit)%> <%: Html.Encode("Allow Edit") %>
                </li>
                <li>
                    <%= Html.CheckBox("ProposalPermission.AllowSubmit", Model.ProposalPermission.AllowSubmit)%> <%: Html.Encode("Allow Submit") %>
                </li>
            </ul>
            <p>
                <input type="submit" value="Save" /> | <%: Html.ActionLink<ProposalController>(a => a.ProposalPermissionsIndex(Model.Proposal.Guid), "Cancel") %>
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

