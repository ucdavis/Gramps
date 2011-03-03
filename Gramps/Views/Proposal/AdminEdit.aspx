<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Core.Resources" %>

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
            <legend>Fields</legend>
            <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" value="<%:StaticValues.Approved%>" "<%=Model.Proposal.IsApproved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" value="<%:StaticValues.Denied%>" "<%= Model.Proposal.IsDenied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" value="<%:StaticValues.NotDecided%>" "<%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %>" /><label for="notDecieded">Not Decided</label>
            </span>

            <div class="editor-label">Is Notified</div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(a => a.Proposal.IsNotified)%>
                <%: Html.ValidationMessageFor(a => a.Proposal.IsNotified)%>
            </div>

            <div class="editor-label">Is Submitted</div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(a => a.Proposal.IsSubmitted)%>
                <%: Html.ValidationMessageFor(a => a.Proposal.IsSubmitted) %>
            </div>

            <div class="editor-label">Approved Amount</div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Proposal.ApprovedAmount, String.Format("{0:F}", Model.Proposal.ApprovedAmount))%>
                <%: Html.ValidationMessageFor(model => model.Proposal.ApprovedAmount)%>
            </div>

            <div class="editor-label">Comments</div>
            <div class="editor-field">
                <%= Html.TextArea("Comment.Text", Model.Comment != null ? Model.Comment.Text : string.Empty)%>
                <%= Html.ValidationMessageFor(a => a.Comment.Text)%> 
            </div>
            <p>
                <input type="submit" value="Save" />
            </p>
        </fieldset>

    <% } %>

    <fieldset>
    <legend><strong>Details</strong></legend>
        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.CreatedDate)%></div>
        
        <div class="display-label">SubmittedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%></div>
        
        <div class="display-label">NotifiedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.NotifiedDate)%></div>
    </fieldset>

    <fieldset>
    <legend><strong>Answers</strong></legend>
        <div class="display-label">RequestedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%></div>
            
        <%Html.RenderPartial("ProposalAnswerDetails"); %>
    </fieldset>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            $("#Comment_Text").enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "900" });
        });

   </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

