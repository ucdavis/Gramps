<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Edit Proposal
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Edit Proposal</h2>

	<%= Html.ClientSideValidation<Proposal>() %>

    <%: Html.ValidationSummary("There were validation Errors.") %>

    <fieldset>
    <legend>Non Editable</legend>
        <div class="display-label">Proposal Id</div>
        <div class="display-field"><%: Model.Proposal.Guid %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%: Model.Proposal.Email%></div>

        <div class="display-label">IsApproved</div>
        <div class="display-field"><%: Model.Proposal.IsApproved%></div>
        
        <div class="display-label">IsDenied</div>
        <div class="display-field"><%: Model.Proposal.IsDenied%></div>        
        
        <div class="display-label">IsSubmitted</div>
        <div class="display-field"><%: Model.Proposal.IsSubmitted%></div>
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
                        })
            .Pageable()
            .Sortable()
            .Render(); 
        %>
    </fieldset>

    <% using (Html.BeginForm()) {%>
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
                    <%= Html.TextBox("proposalAnswers" + indexString + ".Answer", answer, new { @class = "indexedControl " + question.ValidationClasses })%>                 
                <% break; %>
                <% case "Text Area" : %>
                    <div class="editor-label"><%: Html.Encode(question.Name) %></div>
                    <%= Html.TextArea("proposalAnswers" + indexString + ".Answer", answer, new { @class = StaticValues.Class_indexedControl + " TinyMce" })%>
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
            </div>
        <%}%>

        </fieldset>
        <br />
            <p>
                <%: Html.CheckBoxFor(a => a.Proposal.IsSubmitted) %> <%: Html.Encode("Submit Final") %>
                <input type="submit" value="Save" />
            </p>
    <% } %>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script src="<%= Url.Content("~/Scripts/tiny_mce/jquery.tinymce.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/Scripts/jquery.enableTinyMce.js") %>" type="text/javascript"></script>

    <script type="text/javascript">

        var templatecodes = [];

        $(document).ready(function () {
            $(".TinyMce").each(function (index) {
                $(this).enableTinyMce({ script_location: '<%= Url.Content("~/Scripts/tiny_mce/tiny_mce.js") %>', overrideWidth: "700" });
            });
            $(".dateForm").each(function (index) {
                $(this).datepicker();
            });
        });

   </script>
</asp:Content>

