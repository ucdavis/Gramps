<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallNavigationViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Details</h2>

     <fieldset>
    <legend><%: Html.Encode(Model.CallForProposal.Name) %> </legend>
        <%: Html.HtmlEncode(Model.CallForProposal.Description) %>
    </fieldset>

    <fieldset>
        <legend>Details</legend>
        
        <div class="display-field"><%: Html.CheckBoxFor(a => a.CallForProposal.IsActive, new {@disabled = "true"}) %> <%: Html.Encode("Active") %></div>
        
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

