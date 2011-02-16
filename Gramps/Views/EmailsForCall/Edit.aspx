<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailsForCallViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Edit</h2>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%= Html.ValidationSummary("Please correct all errors below") %>
	    <%= Html.ClientSideValidation<EmailsForCall>("EmailsForCall")%>    
        <%: Html.HiddenFor(a => a.EmailsForCall.Id) %> 
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>
        
        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.EmailsForCall.Email) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.EmailsForCall.Email)%>
                <%: Html.ValidationMessageFor(model => model.EmailsForCall.Email)%>
            </div>
            <%if(Model.IsCallForProposal) {%>
                <div class="editor-label">
                    <%: Html.LabelFor(model => model.EmailsForCall.HasBeenEmailed) %>
                </div>
                <div class="editor-field">
                    <%: Html.CheckBoxFor(model => model.EmailsForCall.HasBeenEmailed)%>
                    <%: Html.ValidationMessageFor(model => model.EmailsForCall.HasBeenEmailed)%>
                </div>
            <%}%>
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

