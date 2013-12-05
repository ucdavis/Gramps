<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Edit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Admin Edit Proposal Detail Number: <%:Model.Proposal.Sequence %></h2>

	<%= Html.ClientSideValidation<Proposal>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("") %>
        <%: Html.HiddenFor(a => a.CallForProposal.Id) %>

        <fieldset>
            <legend><strong>Editable</strong></legend>
            <ul>
            <li>
            <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" value="<%:StaticValues.RB_Decission_Approved%>" <%=Model.Proposal.IsApproved ? "checked" : string.Empty%> /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" value="<%:StaticValues.RB_Decission_Denied%>" <%= Model.Proposal.IsDenied ? "checked" : string.Empty %> /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" value="<%:StaticValues.RB_Decission_NotDecided%>" <%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %> /><label for="notDecieded">Not Decided</label>
            </span>
            </li>
            <li>
                <%: Html.CheckBoxFor(a => a.Proposal.IsNotified)%> <%:Html.Label("Is Notified") %>
                <%: Html.ValidationMessageFor(a => a.Proposal.IsNotified)%>
            </li>
            <li>
                <%: Html.CheckBoxFor(a => a.Proposal.IsSubmitted)%> <%:Html.Label("Is Submitted")%>
                <%: Html.ValidationMessageFor(a => a.Proposal.IsSubmitted) %>
            </li>
            <li>
                <%: Html.Label("Approved Amount:")%>
                <%: Html.TextBoxFor(model => model.Proposal.ApprovedAmount, String.Format("{0:F}", Model.Proposal.ApprovedAmount))%>
                <%: Html.ValidationMessageFor(model => model.Proposal.ApprovedAmount)%>
            </li>
            <li>
                <%: Html.Label("Comments:") %>
                <%= Html.TextArea("Comment.Text", Model.Comment != null ? Model.Comment.Text : string.Empty, new { @class = "BigAnswer" })%>
                <%= Html.ValidationMessageFor(a => a.Comment.Text)%> 
            </li>
            </ul>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <fieldset>
    <legend><strong>Details</strong></legend>
        <ul>
        <li>
           <%: Html.Label("Create Date:") %>
           <%: String.Format("{0:g}", Model.Proposal.CreatedDate)%>
        </li>
        <li>
            <%: Html.Label("Submitted Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%>
        </li>
        <li>
            <%: Html.Label("Notified Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.NotifiedDate)%>
        </li>
        </ul>
    </fieldset>

    <fieldset>
    <legend><strong>Answers</strong></legend>
        <ul>
        <li>
            <%: Html.Label("Requested Amount:")%>
            <%: String.Format("{0:C}", Model.Proposal.RequestedAmount)%>
        </li>            
        <%Html.RenderPartial("ProposalAnswerDetails"); %>
        </ul>
    </fieldset>

    <% if(Model.Proposal.File != null && Model.Proposal.File.Contents != null) {%> 
        <ul>
        <li>
        <%: Html.ActionLink<ProposalController>(a => a.AdminDownload(Model.Proposal.Id, Model.CallForProposal.Id), "Submitted PDF", new { @class = "bigpdf_button" })%>        
        </li>
        </ul>
    <%}%>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
<%--    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $("#Comment_Text").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "900" });
        });

   </script>--%>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

