$("#date").click(function () {
    if (!$("#date-arrow i.orderBy")[0]) {
        $("#date-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc")
        $("#userRecycleItems").load("/Admin/ShowUsers/OrderByDateDesc");
    }
    else {
        $("#date-arrow i").removeClass("orderBy").addClass("orderBydesc")
        $("#userRecycleItems").load("/Admin/ShowUsers/OrderByDate")

    }

    $("i").not($("#date i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc")

})

$("#carbon").click(function () {

    if (!$("#carbon-arrow i.orderBy")[0]) {

        $("#carbon-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc")
    }
    else {
        $("#carbon-arrow i").removeClass("orderBy").addClass("orderBydesc")
    }
    $("i").not($("#carbon i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc")

})

$("#amount").click(function () {

    if (!$("#amount-arrow i.orderBy")[0]) {

        $("#amount-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc")
    }
    else {
        $("#amount-arrow i").removeClass("orderBy").addClass("orderBydesc")
    }
    $("i").not($("#amount i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc")

})

$("#product").click(function () {

    if (!$("#product-arrow i.orderBy")[0]) {

        $("#product-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc")
    }
    else {
        $("#product-arrow i").removeClass("orderBy").addClass("orderBydesc")
    }
    $("i").not($("#product i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc")

})

$("#name").click(function () {

    if (!$("#name-arrow i.orderBy")[0]) {
        $("#name-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc")
    }
    else {
        $("#name-arrow i").removeClass("orderBy").addClass("orderBydesc")
    }
    $("i").not($("#name i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc")
})
