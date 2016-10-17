<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalPermissionEditViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Grant Access
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Grant Access</h2>

    <% using (Html.BeginForm()) {%>
        <%: Html.ValidationSummary(true) %>
        <%: Html.Hidden("id", Model.Proposal.Guid) %>
        <%= Html.AntiForgeryToken() %>

        <fieldset>
            <p>Note! The person you add will be sent an email with login instruction.</p>
            <p>Allow Submit grants access to Allow Edit and Allow Review. Allow Edit grants access to Allow Review.</p>
            <br/>
            <ul>
                <li>
                    <%: Html.LabelFor(model => model.ProposalPermission.Email) %>
                    <%: Html.TextBoxFor(model => model.ProposalPermission.Email)%>
                    <%: Html.ValidationMessageFor(model => model.ProposalPermission.Email)%>
                </li>
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

