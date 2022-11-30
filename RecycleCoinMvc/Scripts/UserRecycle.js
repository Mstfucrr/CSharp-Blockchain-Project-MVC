function arrowClick(filterName) {
    if (!$("#" + filterName + "-arrow i.orderBy")[0]) {
        $("#" + filterName + "-arrow i").addClass("orderBy").removeClass("notOrderBy").removeClass("orderBydesc");
        $("#userRecycleItems").load(`/Admin/ShowUsers/OrderBy?filter=${filterName}`);
    }
    else {
        $("#" + filterName + "-arrow i").addClass("orderBydesc").removeClass("orderBy");
        $("#userRecycleItems").load(`/Admin/ShowUsers/OrderByDesc?filter=${filterName}`);

    }
    $("i").not($("#" + filterName + " i")).addClass("notOrderBy").removeClass("orderBy").removeClass("orderBydesc");
}