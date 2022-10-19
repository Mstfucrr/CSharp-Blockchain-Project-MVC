﻿$("#cardViewer").click(function () {
    if ($("#card.show")[0] == undefined) {
        $("#card").css({ "scale": "1", "opacity": "1" }).addClass("show");
    }
    else {
        $("#card").css({ "scale": "0", "opacity": "0" }).removeClass("show");
    }

});
var products = [];
$(".product").click(function () {

    var name = this.parentElement.parentElement.dataset["name"];
    var carbon = this.parentElement.parentElement.dataset["carbon"];
    var quantity = this.parentElement.parentElement.children[1].children[0].value;
    var product = {
        'name': name,
        'carbon': carbon,
        'quantity': quantity
    };
    products.push(product);
    const element = `<tr style="background-color: #F42951;font-size: 15px;"><td>${name}</td>
    <td>${quantity}</td>
    <td>${carbon * quantity}</td>
    <td><button class="btn btn-primary deleteRow" type="button"><svg xmlns="http://www.w3.org/2000/svg" viewBox="-32 0 512 512" width="1em" height="1em" fill="currentColor">
                <!--! Font Awesome Free 6.1.1 by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free (Icons: CC BY 4.0, Fonts: SIL OFL 1.1, Code: MIT License) Copyright 2022 Fonticons, Inc. -->
                <path d="M160 400C160 408.8 152.8 416 144 416C135.2 416 128 408.8 128 400V192C128 183.2 135.2 176 144 176C152.8 176 160 183.2 160 192V400zM240 400C240 408.8 232.8 416 224 416C215.2 416 208 408.8 208 400V192C208 183.2 215.2 176 224 176C232.8 176 240 183.2 240 192V400zM320 400C320 408.8 312.8 416 304 416C295.2 416 288 408.8 288 400V192C288 183.2 295.2 176 304 176C312.8 176 320 183.2 320 192V400zM317.5 24.94L354.2 80H424C437.3 80 448 90.75 448 104C448 117.3 437.3 128 424 128H416V432C416 476.2 380.2 512 336 512H112C67.82 512 32 476.2 32 432V128H24C10.75 128 0 117.3 0 104C0 90.75 10.75 80 24 80H93.82L130.5 24.94C140.9 9.357 158.4 0 177.1 0H270.9C289.6 0 307.1 9.358 317.5 24.94H317.5zM151.5 80H296.5L277.5 51.56C276 49.34 273.5 48 270.9 48H177.1C174.5 48 171.1 49.34 170.5 51.56L151.5 80zM80 432C80 449.7 94.33 464 112 464H336C353.7 464 368 449.7 368 432V128H80V432z"></path>
            </svg></button></td>
</tr>`;
    $("#cartData").append(element);

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
        'totalCarbon': totalCarbon

    };
    console.log(addCarbonToUserObject);
});


