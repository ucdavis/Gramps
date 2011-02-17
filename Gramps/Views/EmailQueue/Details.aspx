<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailQueueViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Details</h2>

    <fieldset>
        <legend>Fields</legend>
                
        <div class="display-label">Pending</div>
        <div class="display-field"><%: Model.EmailQueue.Pending%></div>

        <div class="display-label">Immediate</div>
        <div class="display-field"><%: Model.EmailQueue.Immediate%></div>

        <div class="display-label">EmailAddress</div>
        <div class="display-field"><%: Model.EmailQueue.EmailAddress %></div>
        
        <div class="display-label">Created</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.EmailQueue.Created)%></div>       
        
        <div class="display-label">SentDateTime</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.EmailQueue.SentDateTime)%></div>
        
        <div class="display-label">Subject</div>
        <div class="display-field"><%: Model.EmailQueue.Subject%></div>
        
        <div class="display-label">Body</div>
        <div class="display-field"><%: Html.HtmlEncode(Model.EmailQueue.Body)%></div>

    </fieldset>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

