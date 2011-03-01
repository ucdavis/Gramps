<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.ProposalAdminViewModel>" %>

    <h2>Proposal Detail Number: <%:Model.Proposal.Sequence %></h2>

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

        <%Html.RenderPartial("ProposalAnswerDetails"); %>
         
    </fieldset>