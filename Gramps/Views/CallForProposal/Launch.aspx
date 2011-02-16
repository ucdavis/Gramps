<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallNavigationViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Launch
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Launch</h2>

    <fieldset>
        <legend>Details</legend>
        
        <div class="display-label">Name</div>
        <div class="display-field"><%: Model.CallForProposal.Name %></div>
        
        <div class="display-label">IsActive</div>
        <div class="display-field"><%: Model.CallForProposal.IsActive%></div>
        
        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.CallForProposal.CreatedDate)%></div>
        
        <div class="display-label">EndDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.CallForProposal.EndDate)%></div>
        
        <div class="display-label">CallsSentDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.CallForProposal.CallsSentDate)%></div>
        
        <div class="display-label">Id</div>
        <div class="display-field"><%: Model.CallForProposal.Id%></div>
        
    </fieldset>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

