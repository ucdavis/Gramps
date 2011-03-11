<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Gramps.Core.Resources" %>
        <fieldset class="indexedControlContainer">
        <legend>Properties</legend>
        <br />
            <span id="property<%: Html.Encode(StaticValues.Report_Sequence)%>" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%: Html.Encode(StaticValues.Report_Sequence)%></label>
                <input id="_Property" class="indexedControl" type="hidden" value="True" name="_Property" />
                <input id="_PropertyName" class="indexedControl" type="hidden" value="<%: StaticValues.Report_Sequence %>" name="_PropertyName" />                            
            </span> 
            <span id="property<%: Html.Encode(StaticValues.Report_Submitted)%>" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%: Html.Encode(StaticValues.Report_Submitted)%></label>
                <input id="_Property" class="indexedControl" type="hidden" value="True" name="_Property" />
                <input id="_PropertyName" class="indexedControl" type="hidden" value="<%: StaticValues.Report_Submitted %>" name="_PropertyName" />             
            </span> 
            <span id="property<%: Html.Encode(StaticValues.Report_Approved)%>" class="property">
                <%= Html.CheckBox("_Selected", new { @class = StaticValues.Class_indexedControl })%>  
                <label for="Selected" class="indexedControl"><%: Html.Encode(StaticValues.Report_Approved)%></label>
                <input id="_Property" class="indexedControl" type="hidden" value="True" name="_Property" />
                <input id="_PropertyName" class="indexedControl" type="hidden" value="<%: StaticValues.Report_Approved %>" name="_PropertyName" />                            
            </span> 
        </fieldset>