﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallForProposalViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Call
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Edit Call</h2>

	<%= Html.ClientSideValidation<CallForProposal>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("") %>
        <%:Html.HiddenFor(a => a.CallForProposal.Id) %>

        <fieldset>
            <legend>Fields</legend>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallForProposal.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CallForProposal.Name)%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallForProposal.IsActive)%>
            </div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.CallForProposal.IsActive)%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.IsActive)%>
            </div>           
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallForProposal.EndDate)%>
            </div>
            <div class="editor-field">
                <%: Html.EditorFor(model => model.CallForProposal.EndDate, Model.CallForProposal.EndDate.Date) %>
                 <%--<%: Html.TextBoxFor(model => model.CallForProposal.EndDate, Model.CallForProposal.EndDate.Date)%> --%>               
                <%--<%: Html.TextBox("CallForProposal.EndDate", Model.CallForProposal.EndDate.ToString("d")) %>--%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.EndDate)%>
            </div>          
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallForProposal.ProposalMaximum) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.CallForProposal.ProposalMaximum, String.Format("{0:F}", Model.CallForProposal.ProposalMaximum))%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.ProposalMaximum)%>
            </div> 

            <div class="editor-label">
                <%: Html.LabelFor(model => model.CallForProposal.Description) %>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.Description)%>
            </div>
            <div class="editor-field">
                <%: Html.TextAreaFor(model => model.CallForProposal.Description)%>                
            </div> 
            
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>
    
    <script type="text/javascript">
        $(document).ready(function () {
            $("#CallForProposal_EndDate").datepicker();
            $("#CallForProposal_Description").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "700" });
        });
    </script>



</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

