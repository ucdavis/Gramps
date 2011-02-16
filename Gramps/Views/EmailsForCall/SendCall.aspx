<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailsForCallSendViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	SendCall
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Send Call</h2>



<% Html.Grid(Model.EmailsForCallList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
                col.Bound(x => x.Email);
                col.Bound(x => x.HasBeenEmailed);
                col.Bound(x => x.EmailedOnDate);
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

