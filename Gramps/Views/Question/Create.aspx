<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.QuestionViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%= Html.AntiForgeryToken() %>
        <%= Html.ValidationSummary("Please correct all errors below") %>
	    <%= Html.ClientSideValidation<Question>("Question")%>
        <%: Html.HiddenFor(a => a.TemplateId) %>
        <%: Html.HiddenFor(a => a.CallForProposalId) %>

        <span style="display:none;" id="TypesWithOptions"><%= string.Join(",", Model.QuestionTypes.Where(a => a.HasOptions).Select(a => a.Id.ToString()).ToArray()) %></span>

        <fieldset>
            <legend>Fields</legend>
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Question.Name) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Question.Name, new { style = "width: 700px" })%>
                <%: Html.ValidationMessageFor(model => model.Question.Name)%>
            </div>
            
            <div class="editor-label">
                <%: Html.LabelFor(model => model.Question.Order)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(model => model.Question.Order)%>
                <%: Html.ValidationMessageFor(model => model.Question.Order)%>
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

            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>
        <br />
        <div class="two_col_float two_col_float_left">
        <div class="QuantityContainer">
            <fieldset>
                <legend>Sample</legend>
                <ul>                
                    <li id="SampleTextBox" style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a TextBox Question?")%>
                        <br />
                        <%= Html.TextBox("TextBoxQuestion", "") %>
                    </li>
                    <li id="SampleTextArea"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a TextArea Question?")%>
                        <br />
                        <%= Html.TextArea(".Answer", "") %>
                    </li>
                    <li id="SampleBoolean"style="display:none;" class="hideAndShow">
                        <%= Html.CheckBox(".Answer", true, new {@class="BooleanSample"})%> Sample of a Boolean Question?
                    </li>
                    <li id="SampleRadioButtons"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a Radio Button Question?")%>
                        <br />
                        <%= Html.RadioButton(".Answer", "Option 1", false)%>
                        <%= Html.Encode("Red")%>
                        <%= Html.RadioButton(".Answer", "Option 2", true)%>
                        <%= Html.Encode("Blue")%>
                        <%= Html.RadioButton(".Answer", "Option 3", false)%>
                        <%= Html.Encode("Green")%>
                        <%= Html.RadioButton(".Answer", "Option 4", false)%>
                        <%= Html.Encode("Not any color above but one that cause this to wrap on next line")%>
                    </li>
                    <li id="SampleCheckboxList"style="display:none;" class="hideAndShow">    
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a Checkbox List Question?")%>
                        <br />                              
                        <%= Html.CheckBox(".CblAnswer", false)%>
                        <%= Html.Encode("Checkbox List 1") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 2") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 3") %>
                        <%= Html.CheckBox(".CblAnswer", false)%>
                        <%= Html.Encode("Checkbox List 4") %>
                        <%= Html.CheckBox(".CblAnswer", true)%>
                        <%= Html.Encode("Checkbox List 5") %>
                    </li>
                    <li id="SampleDropDown"style="display:none;" class="hideAndShow">
                        <% var dropDownPick = new Dictionary<int, string>(3) { { 1, "DropDown 1" }, { 2, "DropDown 2" }, { 3, "DropDown 3" } };%>
                         
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a Dropdown List Question?")%>
                        <br />   
                        <%= this.Select("Test").Options(dropDownPick, x=>x.Key, x=>x.Value).FirstOption("--Select a Type--") %>                         
                    </li>
                    <li id="SampleDate"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a Date Question?")%>
                        <br />  
                        <%= Html.TextBox(".Answer", string.Empty, new { @class = "dateForm"})%>
                    </li>
                    <li id="SampleNoAnswer"style="display:none;" class="hideAndShow">
                        <%=Html.Encode(Model.Question != null && !string.IsNullOrWhiteSpace(Model.Question.Name) ? Model.Question.Name : "Sample of a No Answer Question? This can cover several lines up to a maximum of about 200 characters.")%>
                        <br />  
                    </li>
                </ul>    
            </fieldset>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            // attach event handlers
            $("img#AddOptions").click(function () { AddOptionInput(); });
            $("select#Question_QuestionType").change(function (event) { QuestionTypeChange(this); });

            $("#Question_Validators_0").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("required") ? "checked" : ""%>");
            $("#Question_Validators_1").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("email") ? "checked" : ""%>");
            $("#Question_Validators_2").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("url") ? "checked" : ""%>");
            $("#Question_Validators_3").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("date") ? "checked" : ""%>");
            $("#Question_Validators_4").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("phoneUS") ? "checked" : ""%>");
            $("#Question_Validators_5").attr("checked", "<%=Model.Question != null && Model.Question.ValidationClasses.Contains("zipUS") ? "checked" : ""%>");
            <%if(Model.Question != null && Model.Question.Options.Count > 0){%>
                  <%for (int i = 0; i < Model.Question.Options.Count; i++){%>
                    AddOptionInput("<%=Model.Question.Options[i].Name %>");
                  <%}%>
                  $("p#Option").show();
            <%}%>
            $(".questionOptions[0]").attr("value", "test");
        });

        function AddOptionInput(obj) {
            var index = $("span#OptionsContainer").children().length;

            var input = $("<input>");
            var name = "questionOptions[" + index + "]";
            input.attr("id", name);
            input.attr("name", name);
            input.attr("value", obj);
            $("span#OptionsContainer").append(input);
        }

        function QuestionTypeChange(obj) {
            var selectedId = $(obj).find("option:selected").val();
            var typesWithOptions = $("span#TypesWithOptions").html().split(",");

            if ($.inArray(selectedId, typesWithOptions) >= 0) {
                // it's in the array, it has options
                $("p#Option").show();
                //$("span#OptionsContainer").empty();
            }
            else {
                $("p#Option").hide();
            }
            $(".hideAndShow").hide();
            var myText = $(obj).find("option:selected").text().replace(" ", "");
            $("li#Sample" + myText).show();

        }
 
    </script>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="logoContent" runat="server">
</asp:Content>

