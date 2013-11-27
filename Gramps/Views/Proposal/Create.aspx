<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create Proposal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create Proposal</h2>
    <% if (Request.IsAuthenticated){%>
        <% Model.Proposal.Email = Html.Encode(Page.User.Identity.Name);%>
    <%
}%>

	<%= Html.ClientSideValidation<Proposal>() %>
    <fieldset>
        <legend><strong><%: Html.HtmlEncode(Model.CallForProposal.Name)%></strong></legend>

        <div style="margin-top: 5px">
            <%: Html.HtmlEncode(Model.CallForProposal.Description) %>
        </div>
    </fieldset>
    <br />

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("There were Validation Errors") %>
        <%: Html.HiddenFor(a => a.CallForProposal.Id) %>

        <fieldset>
            <legend><strong>Create your proposal</strong></legend>
            <br/>
            <p>To start your proposal, enter your email and the "re CAPTCHA" text shown. Then click the Create button.</p>
            <p>You will receive an email within about 5 minutes with further instructions and a link to edit your proposal.</p>
            <br />
            <ul>
            <li>
            <div class="editor-label">
                <strong><%: Html.LabelFor(model => model.Proposal.Email) %></strong>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Proposal.Email, new { @style = "width:50em;" })%>
                <%: Html.ValidationMessageFor(model => model.Proposal.Email)%>
            </div>
            </li>
            </ul>
            <br />
            <p>
                <%= Html.GenerateCaptcha() %>
            </p>

            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div id="ContactInfo">
        If you are having problems, you may contact <a href="<%: Html.Encode("mailto:" + Model.ContactEmail) %>"><%:Html.Encode(Model.ContactEmail) %></a>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>


