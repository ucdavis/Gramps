<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.CallForProposalViewModel>" %>
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
            <legend>Details</legend>
            <ul>
            <li>
                <%: Html.Label("Name:") %>
                <%: Html.TextBoxFor(model => model.CallForProposal.Name, new { @class = "BigWidth" })%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.Name)%>
            </li>
            <li>
                <%: Html.CheckBoxFor(model => model.CallForProposal.IsActive)%> <%: Html.Label("Active") %>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.IsActive)%>          
            </li>
            <li>
                <%: Html.CheckBoxFor(model => model.CallForProposal.HideInvestigators)%> <%: Html.Label("Hide Investigators (do not allow them to be entered on the proposal)") %>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.HideInvestigators)%>          
            </li>
            <li>
                <%: Html.Label("End Date:") %>
                <%: Html.EditorFor(model => model.CallForProposal.EndDate, Model.CallForProposal.EndDate.Date) %>
                 <%--<%: Html.TextBoxFor(model => model.CallForProposal.EndDate, Model.CallForProposal.EndDate.Date)%> --%>               
                <%--<%: Html.TextBox("CallForProposal.EndDate", Model.CallForProposal.EndDate.ToString("d")) %>--%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.EndDate)%>      
            </li>
            <li>
                <%: Html.Label("Proposal Maximum:")%>
                <%: Html.TextBoxFor(model => model.CallForProposal.ProposalMaximum, String.Format("{0:F}", Model.CallForProposal.ProposalMaximum))%>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.ProposalMaximum)%>
            </li>
            <li>
                <%: Html.LabelFor(model => model.CallForProposal.Description) %>
                <%: Html.ValidationMessageFor(model => model.CallForProposal.Description)%>
                <%: Html.TextAreaFor(model => model.CallForProposal.Description)%>                

            </li>
            </ul>
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

