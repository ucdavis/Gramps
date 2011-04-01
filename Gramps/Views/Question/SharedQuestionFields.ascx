<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.QuestionViewModel>" %>
    <ul>
            <li>
                <%: Html.LabelFor(model => model.Question.Name) %>
                <%: Html.TextBoxFor(model => model.Question.Name, new { style = "width: 700px" })%>
                <%: Html.ValidationMessageFor(model => model.Question.Name)%>
            </li>         

            <li>
                <%: Html.LabelFor(model => model.Validators) %>
                <%= this.CheckBoxList("Question.Validators").Options(Model.Validators, x => x.Id, x => x.Name)%>
            </li>
            
            <li>
                <%: Html.LabelFor(model => model.QuestionTypes) %>
                <%= this.Select("Question.QuestionType").Options(Model.QuestionTypes, x => x.Id, x => x.Name).FirstOption("--Select a Type--")
                    .Selected(Model != null && Model.Question != null && Model.Question.QuestionType != null ? Model.Question.QuestionType.Id : 0)%>
            </li>
            <span id=ShowMaxCharacters>
            <li>
                <%: Html.LabelFor(model => model.Question.MaxCharacters) %>

                <%: Html.TextBoxFor(model => model.Question.MaxCharacters)%>
                <%: Html.ValidationMessageFor(model => model.Question.MaxCharacters)%>
            </li>  
            </span>
            
            <p id="Option" style="display:none;">
                <span id="OptionsContainer">
                </span>
                <img id="AddOptions" src="<%= Url.Content("~/Images/plus.png") %>" style="width:24px; height:24px;" />
            </p>

       </ul>
