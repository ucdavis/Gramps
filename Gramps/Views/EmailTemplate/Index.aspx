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
                    <%if(t.TemplateType == EmailTemplateType.ReadyForReview) {%>
                        <%: Html.HtmlEncode(string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p><p>{6}</p><p>{7}</p>"
                    , "An account has been created for you."
                    , "UserName johnnytest@test.com"
                    , "Password bdLJ&SftBN>%oe"
                    , "You may change your password (recommended) after logging in."
                    , "After you have logged in, you may use this link to review submitted proposals for this Grant Request:"
                    , "http://localhost:31701/Proposal/ReviewerIndex/8"
                    , "Or to view all active Call For Proposals you can use this link(Home):"
                    , "http://localhost:31701/Proposal/Home"))%>
                    <%}%>
                    <%if (t.TemplateType == EmailTemplateType.ProposalConfirmation){%> 
                        <%: Html.HtmlEncode(string.Format("{0}<p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p><p>{6}</p><p>{7}</p>"
                    , "An account has been created for you."
                    , "UserName johnnytest@test.com"
                    , "Password bdLJ&SftBN>%oe"
                    , "You may change your password (recommended) after logging in."
                    , "After you have logged in, you may use this link to edit your proposal:"
                    , "http://localhost:31701/Proposal/Edit/e18348ee-424c-4754-ab48-a23cc7d177a9"
                    , "Or you may access a list of your proposal(s) here:"
                    , "http://localhost:31701/Proposal/Home"))%>
                    <%}%>
                </div>
                <%if(t.TemplateType == EmailTemplateType.ReadyForReview) {%>
                <div>Email Alternate Footer Text:</div>
                <div class="template_Footer" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px; line-height:1.5em;">
                    <br />
                    <%: Html.HtmlEncode(string.Format("{0}<br /><p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p>"
                    , StaticValues.EmailAutomatedDisclaimer
                    , "You have an existing account. Use your email as the userName to login"
                    , "After you have logged in, you may use this link to review submitted proposals for this Grant Request:"
                    , "http://localhost:31701/Proposal/ReviewerIndex/8"
                    , "Or to view all active Call For Proposals you can use this link(Home):"
                    , "http://localhost:31701/Proposal/Home"))%>
                </div>
                <%}%>
                <%if(t.TemplateType == EmailTemplateType.ProposalConfirmation) {%>
                <div>Email Alternate Footer Text:</div>
                <div class="template_Footer" style="background-color:#f4f4f4; border:1px solid #666; margin:10px 20px 10px 20px; padding:10px; line-height:1.5em;">
                    <br />
                    <%: Html.HtmlEncode(string.Format("{0}<br /><p>{1}</p><p>{2}</p><p>{3}</p><p>{4}</p><p>{5}</p>"
                    , StaticValues.EmailAutomatedDisclaimer
                    , "You have an existing account. Use your email as the userName to login"
                    , "After you have logged in, you may use this link to edit your proposal:"
                    , "http://localhost:31701/Proposal/Edit/e18348ee-424c-4754-ab48-a23cc7d177a9"
                    , "Or you may access a list of your proposal(s) here:"
                    , "http://localhost:31701/Proposal/Home"))%>
                </div>
                <%}%>
                

                
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

