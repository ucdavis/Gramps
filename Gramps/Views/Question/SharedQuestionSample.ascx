<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Gramps.Controllers.ViewModels.QuestionViewModel>" %>

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