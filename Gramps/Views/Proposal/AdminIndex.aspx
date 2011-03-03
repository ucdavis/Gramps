<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ProposalAdminListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Admin Proposals Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Admin Proposals Index</h2>



    <% Html.Grid(Model.Proposals) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.AdminEdit(x.Id, Model.CallForProposal.Id), " ", new { @class = "edit_button" })%>           
				<%}).Title("Edit");
			col.Template(x => {%>
				<%: Html.ActionLink<ProposalController>(a => a.AdminDetails(x.Id, Model.CallForProposal.Id), " ", new { @class = "details_button" })%>           
				<%}).Title("Details");
            col.Bound(x => x.Email);
            col.Bound(x => x.LastViewedDate);
            col.Bound(x => x.Approved).Title("Apprv.");
            col.Bound(x => x.Denied);
            col.Bound(x => x.NotifiedOfDecission).Title("Notified");
            col.Bound(x => x.Submitted).Title("Sub.");
            col.Bound(x => x.WarnedOfClosing).Title("Warned");                
            col.Bound(x => x.SubmittedDate);
            col.Bound(x => x.CreatedDate);  
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>
    <br/>
    <div>
    <% using (Html.BeginForm<ProposalController>(b => b.SendCall(Model.CallForProposal.Id, Model.Immediate), FormMethod.Post)){%>
        <%:Html.AntiForgeryToken()%>          
        
        <%= Html.SubmitButton("Submit", "Send Warning of Close")%>        
    <%}%>  
    </div>
    <div>
    <% using (Html.BeginForm<ProposalController>(b => b.SendDecision(Model.CallForProposal.Id, Model.Immediate), FormMethod.Post)){%>
        <%:Html.AntiForgeryToken()%>          
        <%= Html.SubmitButton("Submit", "Send Decision")%>        
    <%}%> 
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

