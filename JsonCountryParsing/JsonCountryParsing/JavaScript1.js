/// <reference path="e:\working\developers organism\code examples\countryjosn parsing\jsoncountryparsing\jsoncountryparsing\jquery-2.1.4.js" />

var url = "//";
var jsonData = { data: "value" };
var isInTestingMode = true;
jQuery.ajax({
    method: "Get", // by default "GET"
    url: url,
    data: jsonData, // PlainObject or String or Array
    dataType: "JSON", //, // "Text" , "HTML", "xml", "script" 
    //processData: true, // false , By default, data passed in to the data option as an object (technically, anything other than a string) will be processed and transformed into a query string, fitting to the default content-type "application/x-www-form-urlencoded". If you want to send a DOMDocument, or other non-processed data, set this option to false.
    //cache:true | false //by default true
    //contents : undefined, // An object of string/regular-expression pairs that determine how jQuery will parse the response, given its content type
    crossDomain: false 
    //async: true | false , // by default true,
    //beforeSend: function( xhr ) {
    //  xhr.overrideMimeType( "text/plain; charset=x-user-defined" );
    //}
}).done(function (response) {
    if (isInTestingMode) {
        console.log(response);
    }
}).fail(function (jqXHR, textStatus, exceptionMessage) {
    console.log("Request failed: " + exceptionMessage);
}).always(function () {
    console.log("complete");
});