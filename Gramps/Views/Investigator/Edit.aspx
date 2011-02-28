<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.InvestigatorViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Investigator
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <ul class="btn">
        <li>
            <%: Html.ActionLink<ProposalController>(a => a.Edit(Model.Proposal.Guid), "Edit Proposal")%>
        </li>
    </ul>

    <h2>Create Investigator</h2>

	<%= Html.ClientSideValidation<Investigator>() %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.ValidationSummary("") %>
        <%: Html.HiddenFor(a => a.Investigator.Id) %>
        <%: Html.HiddenFor(a => a.Proposal.Guid) %>

        <fieldset>
            <legend>Fields</legend>
            
            <div class="editor-field">
                <%: Html.CheckBoxFor(model => model.Investigator.IsPrimary)%> <%: Html.Encode("Primary Investigator") %>
                <%: Html.ValidationMessageFor(model => model.Investigator.IsPrimary)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Name)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Name)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Institution)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Institution)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Institution)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address1)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address1)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address1)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address2)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address2)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address2)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Address3)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Address3)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Address3)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.City)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.City)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.City)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.State)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.State)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.State)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Zip)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Zip)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Zip)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Phone)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Phone)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Phone)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Email)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Email)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Email)%>
            </div>
            
<%--            <div class="editor-label">
                <%: Html.LabelFor(model => model.Investigator.Notes)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Investigator.Notes)%>
                <%: Html.ValidationMessageFor(model => model.Investigator.Notes)%>
            </div>--%>
            

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

