﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailTemplateListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Email Templates</h2>


<div id="template_container">
    
        <% if (Model.EmailTemplateList.Count() > 0) { %>

        <% foreach(var t in Model.EmailTemplateList) { %>
        
            <fieldset class="template">
                <legend>
                    <%= Html.ActionLink<EmailTemplateController>(a => a.Edit(t.Id, Model.TemplateId, Model.CallForProposalId), t.TemplateType.ToString())%>
                </legend>
                
                <div class="template_description" style="margin-bottom:20px;">
                    <%= Html.Encode(t.TemplateType.ToString()) %>
                </div>
                <div>Subject:</div>
                <div class="template_subject" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px;">
                    <%: t.Subject %>
                </div>
                <div>Body:</div>
                <div class="template_body" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px; line-height:1.5em;">
                    <%: Html.HtmlEncode(t.Text) %>
                </div>
                
            </fieldset>
        
        <% } %>
    
        <% } else { %>

            No email templates have been created.

        <% } %>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

