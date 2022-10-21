if (window.innerWidth < 768) {
    $("[data-bss-disabled-mobile]").removeClass("animated").removeAttr("data-aos data-bss-hover-animate data-bss-parallax-bg data-bss-scroll-zoom");
}

$(document).ready(function () {
    AOS.init();

    $("[data-bss-hover-animate]")
        .mouseenter(function () { var elem = $(this); elem.addClass(`animated ${elem.attr("data-bss-hover-animate")}`) })
        .mouseleave(function () { var elem = $(this); elem.removeClass(`animated ${elem.attr("data-bss-hover-animate")}`) });
});
$(document).ready(function () {
    $("#TransactionViewer").slideUp(1);
});

$(window).click(function () {
    if ($("#TransactionViewer.show")[0] == undefined) {
        $("#TransactionViewer").slideUp(300);
        $("#blocks").css("filter", "blur(0)");
    }
    $("#TransactionViewer.show").removeClass("show");

});
$(".block").click(function () {

    
    $("#blocks").css("filter", "blur(2px)");
    $("#TransactionViewer")
        .slideDown(300)
        .addClass("show");
    const hash = this.dataset["hash"];
    $.ajax({
        type: "POST",
        dataType: "Json",
        data: { hash: hash },
        url: "/Transaction/GetTransactionByBlock"
    });
});

$("#TransactionTable").click(function () {
    $("#TransactionViewer").addClass("show");
});