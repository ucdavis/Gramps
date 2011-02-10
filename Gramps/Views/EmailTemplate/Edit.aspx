<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailTemplateViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

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
                <input type="submit" value="Save" />
                <input type="button" value="Send Test Email" id="send-test" />
            </li>
            </ul>
                 
<%--           <div id="right_bar">
                <ul class="registration_form">
                    <% foreach (var a in Model.TemplateTypes) { %>
                        <div id="<%: a.Code %>" class="tokens" style='<%: Model.Template != null && Model.Template.TemplateType.Code == a.Code ? "display:block;" : "display:none;" %>'>
                            <% foreach (var b in a.TemplateTokens) { %>
                                <li><a href="javascript:;" class="add_token" data-token="<%: b.Token %>"><%: b.Name %></a></li>
                            <% } %>
                        </div>
                    <% } %>
                </ul>
           </div>--%>
    <% } %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        var templatecodes = [];

        $(document).ready(function () {
            $("#EmailTemplate_Text").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "700" });

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
