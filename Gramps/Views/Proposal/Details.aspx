<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/PublicSite.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalViewModel>" %>
<%@ Import Namespace="Gramps.Controllers" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Proposal Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Proposal Details</h2>

    <fieldset>
        <legend><strong>Details</strong></legend>
        
        <div class="display-label">Guid</div>
        <div class="display-field"><%: Model.Proposal.Guid %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%: Model.Proposal.Email%></div>
        
        <span id = "ApprovedSpan">
            <label for="Approved">Decission: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" disabled="true" value="<%:StaticValues.Approved%>" "<%=Model.Proposal.IsApproved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.Denied%>" "<%= Model.Proposal.IsDenied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.NotDecided%>" "<%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %>" /><label for="notDecieded">Not Decided</label>
        </span>  
        
        <div class="display-field">
            <%= Html.CheckBox("IsSubmitted", Model.Proposal.IsSubmitted, new { @disabled = "True" })%> <%: Html.Encode("Submitted") %>
        </div>
        
        <div class="display-label">RequestedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%></div>
        
        <div class="display-label">ApprovedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.ApprovedAmount)%></div>
        
        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.CreatedDate)%></div>
        
        <div class="display-label">SubmittedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%></div>
    </fieldset>

    <fieldset>
        <legend>Investigators</legend>
        <%foreach (var investigator in Model.Proposal.Investigators.Where(a => a.IsPrimary)){%>
            <fieldset>
            <legend><strong>Primary Investigator</strong></legend>
                <%: Html.Encode(investigator.Name) %> <br />
                <%: Html.Encode(investigator.Institution) %> <br />
                <%: Html.Encode(investigator.Address1) %> <br />
                <%if (!string.IsNullOrWhiteSpace(investigator.Address2)) {%>
                    <%: Html.Encode(investigator.Address2) %> <br />
                <%}%>
                <%if (!string.IsNullOrWhiteSpace(investigator.Address3)) {%>
                    <%: Html.Encode(investigator.Address3) %> <br />
                <%}%>
                <%: Html.Encode(string.Format("{0} {1} {2}", investigator.City, investigator.State, investigator.Zip)) %> <br />
                <%: Html.Encode(investigator.Phone) %> <br />
                <%: Html.Encode(investigator.Email) %> <br />
            </fieldset>
        <%}%>
        <%foreach (var investigator in Model.Proposal.Investigators.Where(a => !a.IsPrimary)){%>
            <fieldset>
            <legend>Investigator</legend>
                <%: Html.Encode(investigator.Name) %> <br />
                <%: Html.Encode(investigator.Institution) %> <br />
                <%: Html.Encode(investigator.Address1) %> <br />
                <%if (!string.IsNullOrWhiteSpace(investigator.Address2)) {%>
                    <%: Html.Encode(investigator.Address2) %> <br />
                <%}%>
                <%if (!string.IsNullOrWhiteSpace(investigator.Address3)) {%>
                    <%: Html.Encode(investigator.Address3) %> <br />
                <%}%>
                <%: Html.Encode(string.Format("{0} {1} {2}", investigator.City, investigator.State, investigator.Zip)) %> <br />
                <%: Html.Encode(investigator.Phone) %> <br />
                <%: Html.Encode(investigator.Email) %> <br />
            </fieldset>
        <%}%>
        </fieldset>

    <fieldset>
        <legend><strong>Answers</strong></legend>
        <% var index = 0;%>
        <%foreach (var question in Model.Proposal.CallForProposal.Questions.OrderBy(a => a.Order)){%>
        <div class="display-label"><%: Html.Encode(question.Name) %></div>

        <%
            var answer = "";
            var indexString = string.Format("[{0}]", index);
            index++;  
        %>

        <%if(Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).Any()){%>
            <% answer = Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).Any() ? Model.Proposal.Answers.Where(a => a.Question.Id == question.Id).FirstOrDefault().Answer : " "; %>   
        <%}%>
            <% switch (question.QuestionType.Name){%>
                <% case "Text Box" : %>
                    <fieldset>
                        <%:Html.HtmlEncode(answer)%>
                        <br />
                    </fieldset>                  
                <% break; %>
                <% case "Text Area" : %>
                    <fieldset>
                        <%: Html.HtmlEncode(answer)%>
                        <br />
                    </fieldset> 
                <% break; %>
                <% case "Boolean" : %>
                    <%--<%= Html.Encode(Model.Question.Name) %>--%>
                    <%
                        var ans = false;
                        if (!Boolean.TryParse(answer, out ans)) {
                            ans = false; } %>
                    <%= Html.CheckBox("proposalAnswers" + indexString + ".Answer", ans, new { @class = "indexedControl " + question.ValidationClasses, @disabled = "True" })%> <%= Html.Encode(question.Name) %>
                <% break; %>
                <% case "Radio Buttons" : %>  
                 <% var option = !string.IsNullOrEmpty(answer) ? answer.Trim().ToLower() : string.Empty;%>            
                    <% foreach (var o in question.Options){ %> 
                        <%var isChecked = option == o.Name.Trim().ToLower();%>
                        <%= Html.RadioButton("proposalAnswers" + indexString + ".Answer", o.Name, isChecked, new { @class = StaticValues.Class_indexedControl + " " + question.ValidationClasses, @disabled = "True" })%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Checkbox List" : %>
                    <% var options = !string.IsNullOrEmpty(answer) ? answer.Split(',') : new string[1]; %>
                    <%--<%= Html.Encode(Model.Answer) %>--%>
     
                    <% foreach (var o in question.Options){%>
                        <% var cblAns = options.Contains(o.Name) ? "checked=\"checked\"" : ""; %>
     
                        <input id="proposalAnswers<%=o.Name%><%=indexString%>.CblAnswer" type="checkbox" <%=cblAns%> value="<%=o.Name%>" name="proposalAnswers<%=indexString%>.CblAnswer" class="indexedControl <%=question.ValidationClasses%>" disabled="True" />
                        <%--<%= Html.CheckBox("proposalAnswers" + indexString + ".CblAnswer", cblAns, new { @class = StaticValues.Class_indexedControl + " " + question.ValidationClasses })%>--%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Drop Down" : %>
                    <%= this.Select("proposalAnswers" + indexString + ".Answer").Options(question.Options.OrderBy(a => a.Name), x => x.Name, x => x.Name).Class("indexedControl " + question.ValidationClasses)
                            .Selected(answer ?? string.Empty).Disabled(true)
                            .FirstOption("--Not Selected--")%>
                <% break; %>
                <% case "Date" : %>
                    <fieldset>
                        <%:Html.Encode(answer)%>
                        <br />
                    </fieldset> 
                <% break; %>
            <%}%>
    <%}%>
         
    </fieldset>
    <%if (!Model.Proposal.IsSubmitted){%>
    <p>
        <%:Html.ActionLink<ProposalController>(a => a.Edit(Model.Proposal.Guid), "Edit")%>
    </p>
    <%}%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

