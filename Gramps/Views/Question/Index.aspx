<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.QuestionListViewModel>" %>
<%@ Import Namespace="Gramps.Helpers" %>
<%@ Import Namespace="Gramps.Controllers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Question Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>

    <h2>Index</h2><br/>


<p>
    <%: Html.ActionLink<QuestionController>(a => a.Create(Model.TemplateId, Model.CallForProposalId), "Create", new {@class="button"}) %>
</p>

<% Html.Grid(Model.QuestionList) 
            .Name("List")
            .PrefixUrlParameters(false) //True if >0 sortable/pageable grids
            .Columns(col => {
            col.Template(x => {%>
				    <%:Html.ActionLink<QuestionController>(a => a.Edit(x.Id, Model.TemplateId, Model.CallForProposalId), "Edit")%>
				<%}).Title("Edit");
			            col.Template(x => {%>
				<% using (Html.BeginForm<QuestionController>(b => b.MoveUp(x.Id, Model.TemplateId, Model.CallForProposalId), FormMethod.Post, new { name = "MoveUpQuestionForm" })){%>
                    <%:Html.AntiForgeryToken()%>  
                    <%= Html.SubmitButton("Submit", " ", new { @class = "MoveUpQuestion moveUp_button" })%>
                <%}%> 
                <% using (Html.BeginForm<QuestionController>(b => b.MoveDown(x.Id, Model.TemplateId, Model.CallForProposalId), FormMethod.Post, new { name = "MoveDownQuestionForm" })){%>
                    <%:Html.AntiForgeryToken()%>  
                    <%= Html.SubmitButton("Submit", " ", new { @class = "MoveDownQuestion moveDown_button" })%>
                <%}%>  
                <%}).Title("Move");                                 
                                col.Bound(x => x.Name);
                                col.Bound(x => x.QuestionType.Name).Title("Type");
                                col.Bound(x => x.OptionChoices).Title("Options");
                                col.Bound(x => x.ValidationClasses).Title("Validators");
			            col.Template(x => {%>
				<% using (Html.BeginForm<QuestionController>(b => b.Delete(x.Id, Model.TemplateId, Model.CallForProposalId), FormMethod.Post, new { name = "DeleteQuestionForm" })){%>
                    <%:Html.AntiForgeryToken()%>  
                    <%= Html.SubmitButton("Submit", " ", new { @class = "DeleteQuestion remove_button" })%>
                <%}%>          
				<%}).Title("Delete");
            })
            //.Pageable()
            //.Sortable()
            .Render(); 
        %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // assign the submit event to each of the delete links
            $("input.DeleteQuestion").click(function (event) { $(this).parents("form[name='DeleteQuestionForm']").submit(); });
            $("input.MoveUpQuestion").click(function (event) { $(this).parents("form[name='MoveUpQuestionForm']").submit(); });
            $("input.MoveDownQuestion").click(function (event) { $(this).parents("form[name='MoveDownQuestionForm']").submit(); });
            //$("input.EditQuestion").click(function (event) { $(this).parents("form[name='EditQuestionForm']").submit(); });
        });
    </script>


        <style type="text/css">
        .moveUp_button
        {
            background: url("../Images/arrow_fat_up.gif") no-repeat scroll 0 0 transparent;
            border:0;
            cursor:pointer;
        }
        .moveUp_button:hover
        {
            color:#0D548A;
        }
        
        .moveDown_button
        {
            background: url("../Images/arrow_fat_down.gif") no-repeat scroll 0 0 transparent;
            border:0;
            cursor:pointer;
        }
        .moveDown_button:hover
        {
            color:#0D548A;
        }

        .edit_button
        {
            background: url("../Content/Common/Icons/edit.png") no-repeat scroll 0 0 transparent;
            border:0;
            width: 15px;
            cursor:pointer;
        }
        .edit_button:hover
        {
            color:#0D548A;
        }
        
        .remove_button
        {
            background: url("../Images/cancled_transparent.png") no-repeat scroll 0 0 transparent;
            border:0;
            float: left;
            cursor:pointer;
            width: 22px;
            height: 22px
        }
        .remove_button:hover
        {
            color:#0D548A;
        }
    </style>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

