$(document).ready(function () {
/*
    Client side script to allow responsive filters with dynamic binding and autocomplete
*/


    //Dropdown list data source used for all drop down lists as it will not change per field
    var filterChoiceData = new kendo.data.DataSource({
        dataType: 'json',
        cache: false,
        transport: {
            read: function (options) {
                $.ajax({
                    type: "POST",
                    cache: false,
                    dataType: 'json',
                    url: 'DataService.asmx/FilterListEnumerator',   //Connect to filter list datasource
                    contentType: "application/json; charset=utf-8", 
                    success: function (result) {
                        options.success(result.d);
                    }
                });
            }
        }
    });

   //bind on click event to the AddFilter button on the ASPX page
    $("#AddFilter").click(function () {
        var numFilters = $("#Filters > tbody > tr").length;  //check how many filters are in the table. this is used to assign each new input a unique id (not that it is really critical)
        //unhide filter control buttons
        $("#ApplyFilters").removeClass("notvisible");
        $("#ClearFilters").removeClass("notvisible");
        //HTML for injection into filters tbody section to add a new filter row
        var newRow = '<tr id="Filter' + numFilters + '"><td><input id="FilterField' + numFilters + '" class="k-dropdown field"/></td><td><input id="FilterOperand' + numFilters + '" class="k-dropdown operand"/></td><td><input id="FilterValue' + numFilters + '" class="k-input value"/></td><td>'
        if ($('#Filters > tbody > tr').length != 0) {
            //only add remove button if the table has more than one row...
            newRow = newRow + '<input id="RemoveFilter' + numFilters + '" type="button" value="Remove Filter" class="k-button" onClick="$(this).closest(\'tr\').remove();"></input></td></tr>';
        }
        else {
            newRow = newRow + '</td></tr>';
        };
        
            
            
            $('#Filters tbody').append(newRow); //append row to tbody

        //declare a new datasource to dynamically bind to the autoComplete control on the new row. this ensures that each autocomplete is based on the value in the filter field on the same row      
        var newDS = new kendo.data.DataSource({
            dataType: 'json',
            cache: false,
            serverFiltering: true,
            transport: {
                read: function (options) {
                    $.ajax({
                        type: "POST",
                        cache: false,
                        dataType: 'json',
                        data: JSON.stringify({ "FilterValue": $('#FilterField' + numFilters).val(), "items": 50, "SuggestFilter": $('#FilterValue' + numFilters).val()}), //Send FilterField, Items and SuggestFilter parameters to webservice
                        url: 'DataService.asmx/ValueListEnumerator',    //call the ValueListEnumerator web service
                        contentType: "application/json; charset=utf-8",
                        success: function (result) {
                            options.success(result.d);
                        }
                    });
                },
                schema: {
                    model: { id: "id" } //bind the id column on the model
                },
            }
        });

        //Create the autoComplete control on the filtervalue input 
        $('#FilterValue' + numFilters).kendoAutoComplete({
            dataSource: newDS,          //bind to above created datasource
            dataTextField: "value",     //bind textfield
            placeholder: "Start typing a value...",
            delay: 100,                 //delay suggest by 100ms
            suggest: true,              //set suggest to true
            minLength: 1,               //minimum length value before suggest
            filter: "contains"          //set to contains type filter, i.e. will search anywhere within the string
        });

        //Create the drop down list control on the filter field input
        $('#FilterField' + numFilters).kendoDropDownList({
            dataTextField: "AttributeDisplayName",      //bind text field (display) to display name
            dataValueField: "SPFAttribute",             //bind SPFAttribute to the value, this is passed to the web service and used to generate the web service
            dataSource: filterChoiceData,               //bind to the filterChoiceData datasource
            index: 0
        });

        var operands = ["Contains", "Equals", "Not Equal to", "Less than", "Greater than" ,"In"];     //define the operands dataset

        //Create the drop down list control on the operand field input
        $('#FilterOperand' + numFilters).kendoDropDownList({
            dataSource: operands,       //bind to the operands datasource defined above
            index: 0
        });


    });

    $("#AddFilter").click();

 

});
