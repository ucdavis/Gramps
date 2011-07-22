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
        <ul>
        <li>
            <%: Html.Label("Proposal Unique ID:") %>
            <%: Model.Proposal.Guid %>
        </li>
        
        <li>
            <%: Html.Label("Email:") %>
            <%: Model.Proposal.Email%>
        </li>
 <%--       <li>
        <span id = "ApprovedSpan">
            <label for="Approved">Decision: </label>
                <input type="radio" id="IsApproved" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Approved%>" "<%=Model.Proposal.IsApproved ? "checked" : string.Empty%>" /><label for="approved">Approved</label>
                <input type="radio" id="IsDenied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_Denied%>" "<%= Model.Proposal.IsDenied ? "checked" : string.Empty %>" /><label for="denied">Denied</label>
                <input type="radio" id="IsNotDecied" name="ApprovedDenied" disabled="true" value="<%:StaticValues.RB_Decission_NotDecided%>" "<%= !Model.Proposal.IsDenied && !Model.Proposal.IsApproved ? "checked" : string.Empty %>" /><label for="notDecieded">Not Decided</label>
        </span>  
        </li>--%>
        <li>
            <%= Html.CheckBox("IsSubmitted", Model.Proposal.IsSubmitted, new { @disabled = "True" })%> <%: Html.Encode("Submitted") %>
        </li>
        
        <li>
            <%: Html.Label("Requested Amount:")%>
            <%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%>
        </li>
        <%if(Model.Proposal.IsApproved) {%>
        <li>
            <%: Html.Label("Approved Amount:")%>
            <%: String.Format("{0:F}", Model.Proposal.ApprovedAmount)%>
        </li>
        <%}%>
        <li>
            <%: Html.Label("Created Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.CreatedDate)%>
        </li>
        <li>
            <%: Html.Label("Submitted Date:")%>
            <%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%>
        </li>
        </ul>
    </fieldset>

    <ul>
    <li>
    <fieldset>       
        <legend><strong>Investigators</strong></legend>
        <ul>
        <%foreach (var investigator in Model.Proposal.Investigators.Where(a => a.IsPrimary)){%>
            <li>
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
            </li>
        <%}%>
        <%foreach (var investigator in Model.Proposal.Investigators.Where(a => !a.IsPrimary)){%>
            <li>
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
            </li>
        <%}%>
        </ul>
    </fieldset>
    </li>
    </ul>

    <fieldset>
        <legend><strong>Answers</strong></legend>
        <ul>
        <% var index = 0;%>
        <%foreach (var question in Model.Proposal.CallForProposal.Questions.OrderBy(a => a.Order)){%>
        <li>
            <%if (question.QuestionType.Name == "No Answer"){%>
                <%:Html.Encode(question.Name)%>
            <%}%>
            <%else{%>
                <strong><%:Html.Encode(question.Name)%></strong>
            <%}%>        
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
        </li>
    <%}%>
    </ul>     
    </fieldset>
    <%if (!Model.Proposal.IsSubmitted){%>
    <p>
        <%:Html.ActionLink<ProposalController>(a => a.Edit(Model.Proposal.Guid), "Edit")%>
    </p>
    <%}%>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

