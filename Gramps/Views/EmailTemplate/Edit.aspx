<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailTemplateViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Edit - <%: Html.Encode(Model.DescriptionDict[(EmailTemplateType)Model.EmailTemplate.TemplateType]) %></h2>

    <%= Html.ValidationSummary("Save was unsuccessful. Please correct the errors and try again.") %>
    <%= Html.ClientSideValidation<EmailTemplate>("") %>

    <% using (Html.BeginForm()) {%>

        <%= Html.AntiForgeryToken() %>
        <%: Html.HiddenFor(a => a.EmailTemplate.Id) %>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>

            <ul class="registration_form" id="left_bar">

            <li>
                <strong>Subject: </strong>
                <%: Html.TextBox("EmailTemplate.Subject", Model.EmailTemplate != null ? Model.EmailTemplate.Subject : string.Empty, new { @style = "width:20em;" })%>
            </li>
            <li>
                <strong>BodyText:</strong>
                <%= Html.TextArea("EmailTemplate.Text", Model.EmailTemplate != null ? Model.EmailTemplate.Text : string.Empty)%>
                <%= Html.ValidationMessageFor(a => a.EmailTemplate.Text)%> 
            </li>
            <li>
                <div>Email Footer Text:</div>
                <div class="template_Footer" style="background-color:#f4f4f4; border:1px solid #666; padding:10px; line-height:1.5em;">
                    <%: Html.HtmlEncode(Model.FooterText) %>
                </div>
                <%if (!string.IsNullOrWhiteSpace(Model.AlternateFooterText)) { %>
                    <div>Email Alternate Footer Text:</div>
                    <div class="template_Footer" style="background-color:#f4f4f4; border:1px solid #666; padding:10px; line-height:1.5em;">
                        <%: Html.HtmlEncode(Model.AlternateFooterText)%>
                    </div>
                <%} %>
            </li>
            <li>
                <input type="submit" value="Save" />
                <input type="button" value="Send Test Email" id="send-test" />
            </li>
            </ul>
            <%if(Model.Tokens.Count > 0) {%> 
                <div id="right_bar">
                    <strong style="font-family:Trebuchet MS,Arial,Georgia; font-size:1.2em;">Template Fields: </strong>
                    <% foreach (var a in Model.Tokens) { %>
                        <a href="javascript:;" class="add-token" name = "{<%:a%>}"><%:a%></a><br>         
                    <% } %>
   
               </div>  
            <%}%>                
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $("#EmailTemplate_Text").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "700" });
            $(".add-token").click(function (event) {
                var pasteValue = $(this).attr("name");
                tinyMCE.execInstanceCommand("EmailTemplate_Text", "mceInsertContent", false, pasteValue);
            });
            $("#send-test").click(function () {
                var url = '<%: Url.Action("SendTestEmail", "EmailTemplate") %>';
                var subject = $("#EmailTemplate_Subject").val();
                var txt = tinyMCE.get("EmailTemplate_Text").getContent();
                var antiForgeryToken = $("input[name='__RequestVerificationToken']").val();
                $.post(url, { subject: subject, message: txt, __RequestVerificationToken: antiForgeryToken }, function (result) {
                    if (result) alert("Message has been mailed to you.");
                    else alert("there was an error sending test email");
                });


            });
        });

   </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>
