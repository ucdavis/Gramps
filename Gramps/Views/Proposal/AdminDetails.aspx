<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposal Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Admin Proposal Detail Number: <%:Model.Proposal.Sequence %></h2>

    <fieldset>
        <legend>Fields</legend>
        
        <div class="display-label">Guid</div>
        <div class="display-field"><%: Model.Proposal.Guid %></div>
        
        <div class="display-label">Email</div>
        <div class="display-field"><%: Model.Proposal.Email%></div>
        
        <div class="display-label">IsApproved</div>
        <div class="display-field"><%: Model.Proposal.IsApproved%></div>
        
        <div class="display-label">IsDenied</div>
        <div class="display-field"><%: Model.Proposal.IsDenied%></div>
        
        <div class="display-label">IsNotified</div>
        <div class="display-field"><%: Model.Proposal.IsNotified%></div>
        
        <div class="display-label">IsSubmitted</div>
        <div class="display-field"><%: Model.Proposal.IsSubmitted%></div>
        
        <div class="display-label">RequestedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.RequestedAmount)%></div>
        
        <div class="display-label">ApprovedAmount</div>
        <div class="display-field"><%: String.Format("{0:F}", Model.Proposal.ApprovedAmount)%></div>
        
        <div class="display-label">CreatedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.CreatedDate)%></div>
        
        <div class="display-label">SubmittedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.SubmittedDate)%></div>
        
        <div class="display-label">NotifiedDate</div>
        <div class="display-field"><%: String.Format("{0:g}", Model.Proposal.NotifiedDate)%></div>

        <h1>Answers</h1>
        <%foreach (var question in Model.Proposal.CallForProposal.Questions.OrderBy(a => a.Order)){%>
        <div class="display-label"><%: Html.Encode(question.Name) %></div>
        <%
            var answer = "";
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
                    <%= Html.CheckBox(".Answer", ans, new { @disabled = "True" })%> <%= Html.Encode(question.Name) %>
                <% break; %>
                <% case "Radio Buttons" : %>  
                    <%: Html.HtmlEncode(answer)%>
                    <% var option = !string.IsNullOrEmpty(answer) ? answer.Trim().ToLower() : string.Empty;%>            
                    <% foreach (var o in question.Options){ %> 
                        <%var isChecked = option == o.Name.Trim().ToLower();%>
                        <%= Html.RadioButton(".Answer", o.Name, isChecked, new { @disabled = "True" })%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Checkbox List" : %>
                    <%: Html.HtmlEncode(answer)%>
                    <% var options = !string.IsNullOrEmpty(answer) ? answer.Split(',') : new string[1]; %>
                    <%--<%= Html.Encode(Model.Answer) %>--%>
                    <% foreach (var o in question.Options){%>
                        <%var cblAns = options.Contains(o.Name); %>
                        
                        <%= Html.CheckBox(".CblAnswer", cblAns, new { @disabled = "True" })%>
                        <%= Html.Encode(o.Name) %>
                    <% } %>
                <% break; %>
                <% case "Drop Down" : %>
                    <%= this.Select(".Answer").Options(question.Options.OrderBy(a => a.Name), x => x.Name, x => x.Name).Class("indexedControl " + question.ValidationClasses)
                            .Selected(answer ?? string.Empty)
                                                    .FirstOption("--Not Selected--").Disabled(true)%>
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


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

