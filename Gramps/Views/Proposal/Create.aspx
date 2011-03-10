﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalViewModel>" %>
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
    <legend><%: Html.Encode(Model.CallForProposal.Name) %> </legend>
        <%: Html.HtmlEncode(Model.CallForProposal.Description) %>
    </fieldset>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("There were Validation Errors") %>
        <%: Html.HiddenFor(a => a.CallForProposal.Id) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Proposal.Email) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Proposal.Email, new { @style = "width:50em;" })%>
                <%: Html.ValidationMessageFor(model => model.Proposal.Email)%>
            </div>

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


