<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.EmailQueueListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Email Queue Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%Html.RenderPartial("CallNavigationButtons"); %>

    <h2>Email Queue Index</h2><br />


<% Html.Grid(Model.EmailQueues) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				<%: Html.ActionLink<EmailQueueController>(a => a.Edit(x.Id, Model.CallForProposal.Id), " ", new { @class = "edit_button" })%>           
				<%}).Title("Edit");
			col.Template(x => {%>
				<%: Html.ActionLink<EmailQueueController>(a => a.Details(x.Id, Model.CallForProposal.Id), " ", new { @class = "details_button" })%>           
				<%}).Title("Details");
            col.Bound(x => x.Created);
            col.Bound(x => x.EmailAddress);
            col.Bound(x => x.Subject);
            col.Bound(x => x.Pending);
            col.Bound(x => x.SentDateTime);
            col.Template(x => {%>
                <% using (Html.BeginForm<EmailQueueController>(b => b.Delete(x.Id, Model.CallForProposal.Id), FormMethod.Post, new { name = "DeleteEmailForm" })){%>
                    <%:Html.AntiForgeryToken()%>  
                    <%= Html.SubmitButton("Submit", " ", new { @class = "DeleteEmail remove_button" })%>
                <%}%>          
				<%}).Title("Delete");
            })
            .Pageable()
            .Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // assign the submit event to each of the delete links
            $("input.DeleteEmail").click(function (event) { $(this).parents("form[name='DeleteEmailForm']").submit(); });
        });
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

