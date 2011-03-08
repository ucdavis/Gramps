<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposal Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("CallNavigationButtons"); %>

    <% Html.RenderPartial("AdminReviewerProposalDetails"); %>

    <% if(Model.Proposal.File != null && Model.Proposal.File.Contents != null) {%> 
        <%: Html.ActionLink<ProposalController>(a => a.AdminDownload(Model.Proposal.Id, Model.CallForProposal.Id), " ", new { @class = "bigpdf_button" })%>       
        <br /><%: Html.Encode("Attached PDF") %>
    <%}%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

