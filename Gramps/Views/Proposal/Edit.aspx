﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Proposal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit Proposal</h2>

     <fieldset>
    <legend><%: Html.Encode(Model.CallForProposal.Name) %> </legend>
        <%: Html.HtmlEncode(Model.CallForProposal.Description) %>
    </fieldset>

	<%= Html.ClientSideValidation<Proposal>() %>

    <%: Html.ValidationSummary("There were validation Errors.") %>

    <fieldset>
    <legend>Non Editable</legend>
        <div class="display-label">Proposal Id</div>
        <div class="display-field"><%: Model.Proposal.Guid %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%: Model.Proposal.Email%></div>

        <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Approved%>" "<%=Model.Proposal.IsApproved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Denied%>" "<%= Model.Proposal.IsDenied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_NotDecided%>" "<%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %>" /><label for="notDecieded">Not Decided</label>
        </span>        
        
        <div class="display-field">
            <%= Html.CheckBox("IsSubmitted", Model.Proposal.IsSubmitted, new { @disabled = "True" })%> <%: Html.Encode("Submitted") %>
        </div>

        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.CreatedDate)%></div>
            
    </fieldset>

    <fieldset>
    <legend>Investigators</legend>
    <br /><br />
    <p>
        <%: Html.ActionLink<InvestigatorController>(a => a.Create(Model.Proposal.Guid), "Add Investigator", new { @class = "button" })%>
    </p>

<% Html.Grid(Model.Proposal.Investigators) 
            .Name("List")
            .PrefixUrlParameters(true) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%--<%: Html.ActionLink("Edit", "Edit", new { id = x.Id }) %>--%> 
                <%: Html.ActionLink<InvestigatorController>(a => a.Edit(x.Id, Model.Proposal.Guid), " ", new { @class = "edit_button" })%>          
				<%}).Title("Edit");
                    col.Bound(x => x.IsPrimary).Title("Primary");
			            col.Bound(x => x.Name);
                        col.Bound(x => x.Email); 
                        col.Template(x => { %>                                       
                            <% using (Html.BeginForm("Delete", "Investigator", FormMethod.Post)) { %>
                                <%= Html.AntiForgeryToken() %>
                                <%: Html.Hidden("investigatorId", x.Id) %>    
                                <%: Html.Hidden("proposalId", Model.Proposal.Guid) %>
                                <%= Html.SubmitButton("Submit", " ", new {@class="remove_button"}) %>                                                                           
                            <% } %>                                      
                        <% }).Title("Delete");                       
                        })
            .Pageable()
            .Sortable()
            .Render(); 
        %>
    </fieldset>

    <% using (Html.BeginForm("Edit", "Proposal", FormMethod.Post, new { @enctype = "multipart/form-data"})) {%>
        <%= Html.AntiForgeryToken() %>

        <%: Html.Hidden("id", Model.Proposal.Guid) %>


        <fieldset>
            <legend>Fields</legend>
            <div class="display-label">Proposal Maximum Amount</div>
            <div class="display-field"><%: String.Format("{0:C}", Model.CallForProposal.ProposalMaximum)%></div>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Proposal.RequestedAmount) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Proposal.RequestedAmount, String.Format("{0:F}", Model.Proposal.RequestedAmount))%>
                <%: Html.ValidationMessageFor(model => model.Proposal.RequestedAmount)%>
            </div>

        <% var index = 0;%>
        <%foreach (var question in Model.Proposal.CallForProposal.Questions.OrderBy(a => a.Order)){%>        
        <%
            var answer = "";
             var indexString = string.Format("[{0}]", index);
            index++;  
        %>
        <%if(Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).Any()){%>
            <% answer = Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).Any() ? Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).FirstOrDefault().Answer : " "; %>   
        <%}%>
            <%= Html.Hidden("proposalAnswers" + indexString + ".QuestionId", question.Id, new { @class = StaticValues.Class_indexedControl})%>
            <div class="editor-field">            
            <% switch (question.QuestionType.Name){%>
                <% case "Text Box" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <%= Html.TextBox("proposalAnswers" + indexString + ".Answer", answer, new { @class = "indexedControl " + question.ValidationClasses +" BigWidth" })%>                                     
                <% break; %>
                <% case "Text Area" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <%= Html.TextArea("proposalAnswers" + indexString + ".Answer", answer, new { @class = StaticValues.Class_indexedControl + " BigAnswer" })%>
                <% break; %>
                <% case "Boolean" : %>
                    <%--<%= Html.Encode(Model.Question.Name) %>--%>
                    <div class="editor-label"><%: Html.Encode(" ") %></div>
                    <%
                        var ans = false;
                        if (!Boolean.TryParse(answer, out ans)) {
                            ans = false; } %>
                    <%= Html.CheckBox("proposalAnswers" + indexString + ".Answer", ans, new { @class = "indexedControl " + question.ValidationClasses })%> <%= Html.Encode(question.Name) %>
                <% break; %>
                <% case "Radio Buttons" : %>  
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <% var option = !string.IsNullOrEmpty(answer) ? answer.Trim().ToLower() : string.Empty;%>            
                    <% foreach (var o in question.Options){ %> 
                        <%var isChecked = option == o.Name.Trim().ToLower();%>
                        <%= Html.RadioButton("proposalAnswers" + indexString + ".Answer", o.Name, isChecked, new { @class = StaticValues.Class_indexedControl + " " + question.ValidationClasses })%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Checkbox List" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <% var options = !string.IsNullOrEmpty(answer) ? answer.Split(',') : new string[1]; %>
                    <%--<%= Html.Encode(Model.Answer) %>--%>
     
                    <% foreach (var o in question.Options){%>
                        <% var cblAns = options.Contains(o.Name) ? "checked=\"checked\"" : ""; %>
     
                        <input id="proposalAnswers<%=o.Name%><%=indexString%>.CblAnswer" type="checkbox" <%=cblAns%> value="<%=o.Name%>" name="proposalAnswers<%=indexString%>.CblAnswer" class="indexedControl <%=question.ValidationClasses%>" />                        
                        <%--<%= Html.CheckBox("proposalAnswers" + indexString + ".CblAnswer", cblAns, new { @class = StaticValues.Class_indexedControl + " " + question.ValidationClasses })%>--%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Drop Down" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <%= this.Select("proposalAnswers" + indexString + ".Answer").Options(question.Options.OrderBy(a => a.Name), x => x.Name, x => x.Name).Class("indexedControl " + question.ValidationClasses)
                            .Selected(answer ?? string.Empty)
                            .FirstOption("--Not Selected--")%>
                <% break; %>
                <% case "Date" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <%= Html.TextBox("proposalAnswers" + indexString + ".Answer", answer, new { @class = "dateForm indexedControl " + question.ValidationClasses })%>
                <% break; %>
                <% default: %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>                   
                <%break;%>
            <%}%>
            <%: Html.ValidationMessage(question.Name)%>
            
            </div>
        <%}%>

        <div class="editor-field">
            <%if (Model.Proposal != null && Model.Proposal.File != null && !string.IsNullOrWhiteSpace(Model.Proposal.File.Name))
              {%>
                <%: Html.Encode("Existing File: " + Model.Proposal.File.Name)%> <br />
                <%= this.FileUpload("uploadAttachment").Label("Replace PDF Attachment")%>
            <%}%>
            <%else { %> 
                <%= this.FileUpload("uploadAttachment").Label("Add PDF Attachment")%>
            <%} %>
            
            <%: Html.ValidationMessageFor(model => model.Proposal.File)%>
        </div>

        </fieldset>
        <br />

        <fieldset>
        <legend>Save Options</legend>
            <span id = "SaveOptions">
                <input type="radio" id="SubmitFinalWithValidation" name="SaveOptions" value="<%:StaticValues.RB_SaveOptions_SubmitFinal%>" "<%= Model.SaveOptionChoice == StaticValues.RB_SaveOptions_SubmitFinal ? "checked" : string.Empty%>" /><label for="SubmitFinalWithValidation">Submit Final With Validation</label><br />
                <input type="radio" id="SaveWithValidation" name="SaveOptions" value="<%:StaticValues.RB_SaveOptions_SaveWithValidation%>" "<%= Model.SaveOptionChoice == StaticValues.RB_SaveOptions_SaveWithValidation ? "checked" : string.Empty %>" /><label for="SaveWithValidation">Save Draft With Validation</label><br />
                <input type="radio" id="SaveWithoutValidation" name="SaveOptions" value="<%:StaticValues.RB_SaveOptions_SaveNoValidate%>" "<%= Model.SaveOptionChoice == StaticValues.RB_SaveOptions_SaveNoValidate ? "checked" : string.Empty %>" /><label for="SaveWithoutValidation">Save Draft Without Validation</label><br />
            </span> 
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
            $(".dateForm").each(function (index) {
                $(this).datepicker();
            });
        });

   </script>
</asp:Content>

