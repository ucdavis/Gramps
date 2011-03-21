<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposal Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% Html.RenderPartial("CallNavigationButtons"); %>
    <ul>
    <% Html.RenderPartial("AdminReviewerProposalDetails"); %>

    <% if(Model.Proposal.File != null && Model.Proposal.File.Contents != null) {%> 
        <li>
        <%: Html.ActionLink<ProposalController>(a => a.AdminDownload(Model.Proposal.Id, Model.CallForProposal.Id), "Submitted PDF", new { @class = "bigpdf_button" })%>        
        </li>
    <%}%>
    </ul>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

