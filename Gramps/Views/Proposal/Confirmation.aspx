<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalConfirmationViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Confirmation
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Confirmation</h2>

    <h3><%: Model.Message1 %></h3>
    <h3><%: Model.Message2 %></h3>



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

