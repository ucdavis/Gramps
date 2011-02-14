<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.QuestionViewModel>" %>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Question.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Question.Name, new { style = "width: 700px" })%>
                <%: Html.ValidationMessageFor(model => model.Question.Name)%>
            </div>           

            <div class="editor-label">
                <%: Html.LabelFor(model => model.Validators) %>
            </div>
            <div class="editor-field">
                <%= this.CheckBoxList("Question.Validators").Options(Model.Validators, x => x.Id, x => x.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.QuestionTypes) %>
            </div>
            <div class="editor-field">
                <%= this.Select("Question.QuestionType").Options(Model.QuestionTypes, x => x.Id, x => x.Name).FirstOption("--Select a Type--")
                    .Selected(Model != null && Model.Question != null && Model.Question.QuestionType != null ? Model.Question.QuestionType.Id : 0)%>
            </div>

            
            <p id="Option" style="display:none;">
                <span id="OptionsContainer">
                </span>
                <img id="AddOptions" src="<%= Url.Content("~/Images/plus.png") %>" style="width:24px; height:24px;" />
            </p>
