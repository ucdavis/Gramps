<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Gramps.Controllers.ViewModels.QuestionViewModel>" %>
<%@ Import Namespace="Gramps.Core.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("NavigationButtons"); %>
    <h2>Create</h2>

    <% using (Html.BeginForm()) {%>
        <%Html.RenderPartial("SharedQuestionSetup"); %>

        <fieldset>
            <legend>Fields</legend>
            
            <% Html.RenderPartial("SharedQuestionFields"); %>       
            
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <% Html.RenderPartial("SharedQuestionSample"); %>

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

