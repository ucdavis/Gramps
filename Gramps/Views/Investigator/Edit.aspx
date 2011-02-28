<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.InvestigatorViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Investigator
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%: Html.ActionLink<ProposalController>(a => a.Edit(Model.Proposal.Guid), "Edit Proposal")%>
        </li>
    </ul>

    <h2>Create Investigator</h2>

	<%= Html.ClientSideValidation<Investigator>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("") %>
        <%: Html.HiddenFor(a => a.Investigator.Id) %>
        <%: Html.HiddenFor(a => a.Proposal.Guid) %>

        <fieldset>
            <legend>Fields</legend>            
            <% Html.RenderPartial("InvestigatorForm");%>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <%
} %>



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

