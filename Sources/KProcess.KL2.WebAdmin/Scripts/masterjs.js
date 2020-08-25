$(function () {
    var url = window.location;
    var UrlWithParameter = window.location.href.split("&")[0];
    var UrlWithDetail = url.toString().includes("/Detail/") ? url.toString().split("/Detail/")[0] : "";
    var UrlPublishModeFilter = url.toString().includes("PublishModeFilter=") ? url.toString().split("PublishModeFilter=")[1].substring(0,1) : "";
    var UrlRefId = url.toString().includes("refId=") ? url.toString().split("refId=")[1] : "";
    var UrlTo = url.toString().includes("to=") ? url.toString().split("to=")[1] : "";
    var UrlWithRef = url.toString().includes("refId=") ? url.toString().split("?refId=")[0] : "";
    var UrlDocumentation = url.toString().includes("Documentation?") ? url.toString().split("?")[0] : "";

    // for sidebar menu entirely but not cover treeview
    $('ul.sidebar-menu a').filter(function () {
        return this.href != url;
    }).parent().removeClass('active');

    // for sidebar menu entirely but not cover treeview
    $('ul.sidebar-menu a').filter(function () {
        return this.href == url;
    }).parent().addClass('active');

    // for treeview
    $('ul.treeview-menu a').filter(function () {
        var currentSection = this.href;
        if (UrlPublishModeFilter != "" && currentSection.includes("PublishModeFilter=")) {
            var PublishModeFilter = currentSection.split("PublishModeFilter=")[1].substring(0, 1);
            return PublishModeFilter == UrlPublishModeFilter;
        }
        if (UrlWithDetail != "")
            return this.href == UrlWithDetail;

        if (UrlWithRef != "")
            return this.href == UrlWithRef;

        if (UrlDocumentation != "")
            return this.href == UrlDocumentation;

        return this.href.split("&")[0] == UrlWithParameter;
    }).parentsUntil(".sidebar-menu > .treeview-menu").addClass('active');

    $('li.refTabs a').filter(function () {
        var currentSection = this.href;
        if (UrlRefId != "" && currentSection.includes("refId=")) {
            var refId = currentSection.split("refId=")[1];
            return refId == UrlRefId;
        }
    }).parent().addClass('active');
})

function popUpImage(img) {
    var imgPop = document.getElementById('imgPop');
    var imageDialog = document.getElementById("imageDialog").ej2_instances[0];
    imgPop.src = img.src;
    imageDialog.height = "auto";
    imageDialog.width = "auto";
    imageDialog.show();
}

var span = document.getElementsByClassName("close")[0];
if (span) {
    span.onclick = function () {
        $("#imageModal").modal("hide");
    }
}

function treeNodeSelected(args) {
    if (args.nodeData.id.includes('p')) {
        var aElement = args.node.querySelectorAll("a");
        aElement[0].setAttribute('onclick', aElement[0].href);
        aElement[0].onclick.call(aElement[0]);
    }
}