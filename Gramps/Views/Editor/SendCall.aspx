<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.ReviewersSendViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Send Reviewer Notification
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Send Reviewer Proposals Ready to be Reviewed</h2>



<% Html.Grid(Model.EditorsToNotify) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
                col.Bound(x => x.ReviewerName);
                col.Bound(x => x.ReviewerEmail);
                col.Bound(x => x.HasBeenNotified);
                col.Bound(x => x.NotifiedDate);
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%: Html.HiddenFor(a => a.CallForProposal.Id) %>
        <%: Html.HiddenFor(a => a.Immediate) %>
        <br />
        <p>
            <input type="submit" value="Send unsent emails" />
        </p>

    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

