<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailTemplateListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

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
                    <%= Html.Encode(Model.DescriptionDict[(EmailTemplateType)t.TemplateType]) %>
                </div>
                <div>Subject:</div>
                <div class="template_subject" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px;">
                    <%: t.Subject %>
                </div>
                <div>Body:</div>
                <div class="template_body" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px; line-height:1.5em;">
                    <%: Html.HtmlEncode(t.Text) %>
                </div>

                <div>Email Footer Text:</div>
                <div class="template_Footer" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px; line-height:1.5em;">
                    <br />
                    <%: Html.Encode(StaticValues.EmailAutomatedDisclaimer) %> <br />
                    <%if(t.TemplateType == EmailTemplateType.InitialCall) {%>
                        <%: Html.Encode(StaticValues.EmailCreateProposal) %> <br />
                        <%: Html.Encode("This will be replaced with the link to create a proposal") %>
                    <%}%>
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

