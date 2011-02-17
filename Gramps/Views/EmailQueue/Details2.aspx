<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Core.Domain.EmailQueue>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details2
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details2</h2>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">EmailAddress</div>
        <div class="display-field"><%: Model.EmailAddress %></div>
        
        <div class="display-label">Created</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Created) %></div>
        
        <div class="display-label">Pending</div>
        <div class="display-field"><%: Model.Pending %></div>
        
        <div class="display-label">SentDateTime</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.SentDateTime) %></div>
        
        <div class="display-label">Subject</div>
        <div class="display-field"><%: Model.Subject %></div>
        
        <div class="display-label">Body</div>
        <div class="display-field"><%: Model.Body %></div>
        
        <div class="display-label">Immediate</div>
        <div class="display-field"><%: Model.Immediate %></div>
        
    </fieldset>
    <p>
        <%: Html.ActionLink("Edit", "Edit", new { id = Model.Id }) %> |
        <%: Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

