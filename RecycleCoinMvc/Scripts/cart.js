$("#cardViewer").click(function () {
    if ($("#card.show")[0] == undefined) {
        $("#card").css({ "scale": "1", "opacity": "1" }).addClass("show");
    }
    else {
        $("#card").css({ "scale": "0", "opacity": "0" }).removeClass("show");
    }

});
var products = [];
$(".product").click(function () {
    const productId = parseInt(this.parentElement.parentElement.dataset["productid"]);
    const quantity = parseInt(this.parentElement.parentElement.children[1].children[0].value);



    $.ajax({
        type: "POST",
        url: "/Admin/RecycleCenter/AddToCart",
        data: { productId: productId, quantity: quantity },
        success: function (response) {
            console.log(response);
            // response ile gelen verileri kullanarak update işlemi gerçekleştirilecek
        }
    });
    location.reload();
    
});

$("#cartData").on("click", ".deleteRow", function () {
    console.dir(this.parentElement.parentElement.remove());

});
var addCarbonToUserObject = {};
$("#recycleSubmit").click(function () {
    var toAddress = $("#toAddress")[0].value;
    var dataRows = $("#cartData tr");
    var totalCarbon = 0;
    for (data of dataRows) {
        totalCarbon += parseInt(data.children[2].innerText);
    }
    addCarbonToUserObject = {
        'toAddress': toAddress,
        'totalCarbon': parseInt(totalCarbon)

    };

    $.ajax({
        type: "GET",
        url: "/Admin/RecycleCenter/RecyleItems",
        data: { toAddress: toAddress, totalCarbon: totalCarbon },
        success: function (response) {
            console.log(response);
            // response ile gelen verileri kullanarak update işlemi gerçekleştirilecek
        }
    });


    console.log(addCarbonToUserObject);
});


