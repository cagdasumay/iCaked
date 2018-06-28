/****************** Follow Functions ******************/

function toggleFollowers(what) {
    $("#followers-label,#following-label").toggleClass("follow-active");

    if (what == "openFollowers") {
        $("#followingWrapper").fadeOut(200);
        setTimeout(function () { $("#followersWrapper").fadeIn(200); }, 210);
    }
    else if (what == "openFollowings") {
        $("#followersWrapper").fadeOut(200);
        setTimeout(function () { $("#followingWrapper").fadeIn(200); }, 210);
    }
}

function followUser(userId) {
    $.ajax({
        url: "/Account/FollowUser?userId=" + userId,
        type: 'GET',
        cache: false,
        success: function (result) {
            location.reload();
        }
    });
}

function stopFollowingUser(userId) {
    if (confirm("Bu kullanıcı takipten çıkarılacaktır onaylıyor musunuz?")) {
        $.ajax({
            url: "/Account/StopFollowingUser?userId=" + userId,
            type: 'GET',
            cache: false,
            success: function (result) {
                location.reload();
            }
        });
    }
}

function setIntroCity(city) {
    $.ajax({
        url: "/Home/SetIntroCity?city=" + city,
        type: 'GET',
        cache: false,
        success: function (result) {
            location.reload();
        }
    });
}

function changeImage(n, t) { t == "over" ? $(n).find(".ilanChildImg").css("opacity") == 1 && $(n).find(".ilanChildImg").fadeOut(300) : t == "out" && $(n).find(".ilanChildImg").css("opacity") < 1 | $(n).find(".ilanChildImg").is(":visible") == !1 && $(n).find(".ilanChildImg").fadeIn(300) }


/****************** Fav Functions ******************/

function FaveProductJson(elem) {
    $.ajax({
        url: "/Order/FaveProductJson?ProductID=" + $(elem).attr("data-productID") + "&BakeryID=" + $(elem).attr("data-bakeryID"),
        type: 'GET',
        cache: false,
        success: function (result) {
            location.reload();
        }
    });
}

function UnFaveProductJson(elem) {
    $.ajax({
        url: "/Order/UnfaveProductJson?ProductID=" + $(elem).attr("data-productID") + "&BakeryID=" + $(elem).attr("data-bakeryID"),
        type: 'GET',
        cache: false,
        success: function (result) {
            location.reload();
        }
    });
}

/****************** Product Functions ******************/

function arrangeProductCategories() {
    var cat = $("#bakery_product_category").val();
    $("#numberGramSelector input[type=radio]").attr("checked", false);
    if (cat.indexOf("Pasta") > -1) {
        $("#productGram_div,#productNumber_div,#numberGramSelector").slideUp(300);
        $("#cake_size_div,#cakeSizeAppend").slideDown(300); $("#gramAppend").empty();
        gramCounter = 1;
    }
    else if (cat == "Çikolata" | cat == "Tatlı") {
        $("#cake_size_div,#cakeSizeAppend,#productNumber_div,#productGram_div,#party_size_div").slideUp(300); $("#numberGramSelector").slideDown(300);
        $("#cakeSizeAppend,#partyAppend").empty(); sizeCounter = 1;
    }
    else if (cat == "Cupcake" | cat == "Kek/Kurabiye") {
        $("#cake_size_div,#cakeSizeAppend,#productGram_div,#numberGramSelector,#party_size_div").slideUp(300); $("#productNumber_div").slideDown(300);
        $("#cakeSizeAppend,#gramAppend,#partyAppend").empty(); sizeCounter = 1; gramCounter = 1;
    }
    else if (cat == "Parti Malzemeleri") {
        $("#cake_size_div,#cakeSizeAppend,#productGram_div,#numberGramSelector").slideUp(300); $("#party_size_div").slideDown(300);
        $("#cakeSizeAppend,#gramAppend,#partyAppend").empty(); sizeCounter = 1; gramCounter = 1; partyCounter = 1;
    }
}

function findAndReplace(string, target, replacement) {
    var i = 0, length = string.length;
    for (i; i < length; i++) { string = string.replace(target, replacement); }
    return string;
}

function fillBakeryProduct(elem, name, description, price, category, stock, thumbnail, productID, size, gram, number, bakery, approved, designProduct) {
    if (bakery != null) { $("#bakery_name").val(bakery); }
    gramCounter = 1; sizeCounter = 1;
    $("#bakery_product_price").val("");
    $("#cakeSizeAppend").empty();
    $(".bpc-open").show(); $(".bpc-close").hide(); $(elem).hide(); $(elem).parent().children(".bpc-close").show();
    $("#bakery_product_name").val(name);
    $("#bakery_product_description").val(description);
    $("#bakery_product_category").val(category);
    $("#bakery_product_category").addClass("disabled-link");
    $("#bakery_product_stockAvailable").val(stock);
    if (approved != null) { if (approved == "yes") { $("#bakery_product_approved").prop("checked", true); } else { $("#bakery_product_approved").prop("checked", false); } } console.log(designProduct);
    if (designProduct != null) { if (designProduct == "yes") { $("#bakery_product_designProduct").prop("checked", true); } else { $("#bakery_product_designProduct").prop("checked", false); } }
    var images = thumbnail.split("  ");
    $("#bakery_product_thumbnail_div .bakery_product_thumbnail_img").attr("src", "/Images/Site/no_image.png");
    var counter = 1;
    for (i = 0; i < images.length - 1; i++) {
        if (counter < 4) {
            $("#bakery_product_thumbnail_div .bakery_product_thumbnail_img:nth-child(" + counter + ")").attr("src", "/Images/Products/" + productID + "/" + images[i]);
        }
        counter++;
    }
    $("#bakery_product_submit").val("Kaydet"); $("#product-id-hidden").val(productID); $("#bakery_product_delete").show().attr("data-productName", name).attr("href", "/Bakery/DeleteBakeryProduct?productID=" + productID);
    $("#bakery-product-form").attr("action", "/Bakery/EditBakeryProduct");

    if (size != "") {
        if (category == "Parti Malzemeleri") {
            $("#bakery_product_description").val(" ");
            $("#productNumber_div,#productGram_div,#numberGramSelector,#cake_size_div").hide(); $("#party_size_div").slideDown(300);
            var sizes = size.split(" "); var prices = price.split(" "); var descriptions = description.split("~"); $("#cakeSizeAppend,#gramAppend,#partySizeAppend").empty();
            for (i = 0; i < sizes.length; i++) {
                console.log(descriptions[i]);
                var div = "<div style='margin-bottom:5px; width:100%; float:left;'><input readonly name='partySize" + partyCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + sizes[i] + " kişilik " + prices[i] + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); sizeCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /><textarea readonly name='partyDescription" + partyCounter + "' style='width:100%; float:left; padding:5px; resize:none; border:1px solid #cccccc; border-radius:3px;'>" + findAndReplace(descriptions[i], "<br />", "\r\n") + "</textarea></div>";
                $("#partySizeAppend").append(div);
                partyCounter++;
            }
        }
        else {
            $("#productNumber_div,#productGram_div,#numberGramSelector,#party_size_div").hide(); $("#cake_size_div").slideDown(300);
            var sizes = size.split(" "); var prices = price.split(" "); $("#gramAppend,#partySizeAppend").empty();
            for (i = 0; i < sizes.length; i++) {
                var div = "<div><input readonly name='cakeSize" + sizeCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + sizes[i] + " " + prices[i] + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); sizeCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div>";
                $("#cakeSizeAppend").append(div);
                sizeCounter++;
            }
        }
    }
    else if (gram != "") {
        $("#productNumber_div,#cake_size_div,#party_size_div").hide(); $("#productGram_div").slideDown(300);
        $("#cakeSizeAppend,#partySizeAppend").empty();
        var grams = gram.split(" "); var prices = price.split(" "); $("#cakeSizeAppend,#gramAppend,#partySizeAppend").empty();
        for (i = 0; i < grams.length; i++) {
            var div = "<div><input readonly name='gram" + gramCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + grams[i] + " " + prices[i] + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); gramCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div>";
            $("#gramAppend").append(div);
            gramCounter++;
        }
    }
    else if (number != "") {
        $("#productGram_div,#cake_size_div,#party_size_div").hide(); $("#productNumber_div").slideDown(300);
        $("#bakery_product_number").val(number); $("#cakeSizeAppend,#gramAppend,#partySizeAppend").empty();
        $("#bakery_product_price").val(price + " TL");
    }
}

function toggleGramNumber(what) {
    if (what == "gram") {
        $("#productNumber_div").hide(); $("#productGram_div").slideDown(300);
    }
    else if (what == "number") {
        $("#gramAppend").empty();
        $("#productGram_div").hide(); $("#productNumber_div").slideDown(300);
    }
}

function closeBakeryProduct(elem) {
    $(elem).hide(); $(elem).parent().children(".bpc-open").show();
    $("#bakery_product_name,#bakery_product_description,#bakery_product_price,#bakery_product_gram,#bakery_product_number,#bakery_product_stockAvailable").val("");
    $("#bakery_product_approved").attr("checked", false);
    $("#bakery_product_category").val("Pasta");
    $("#bakery_product_thumbnail_div .bakery_product_thumbnail_img").attr("src", "/Images/Site/no_image.png");
    $("#bakery_product_submit").val("Ekle");
    $("#bakery-product-form").attr("action", "/Bakery/AddBakeryProduct");
    $("#bakery_product_delete").hide().attr("href", "");
    $("#cakeSizeAppend,#gramAppend").empty();
    $("#bakery_product_category").removeClass("disabled-link");
    $("#productNumber_div,#productGram_div,#numberGramSelector").slideUp(); $("#cake_size_div").slideDown();
    gramCounter = 1; sizeCounter = 1;
}

function AddProductChecker(productID, addAnyway) {
    $(".loading-child").hide();
    $(".loading").fadeIn(300);
    $("#cart-warning-dialog").center();
    $('#cart-warning-dialog,#designsConflict,#productsConflict,#designProductConflict,#productDesignConflict').hide();
    if (addAnyway == null | addAnyway == undefined) { addAnyway = false; }
    $.ajax({
        url: "/Order/AddProductChecker?ProductID=" + productID + "&addAnyway=" + addAnyway,
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result == "ok") { addDeleteLoading(true); window.location = "/Sepetim"; }
            else if (result == "productsConflict") {
                $("#cart-warning-dialog,#productsConflict").fadeIn(300); $("#productsConflict").children().children(".black-gradient").attr("onclick", "AddProductChecker('" + productID + "',true)");
            }
            else if (result == "productDesignConflict") {
                //      $("#cart-warning-dialog").css("margin-top", parseInt($(window).scrollTop() + parseInt($(window).height()) / 2 - 85) + "px");
                $("#cart-warning-dialog,#productDesignConflict").fadeIn(300); $("#productDesignConflict").children().children(".black-gradient").attr("onclick", "AddProductChecker('" + productID + "',true)");
            }
        }
    });
}

function DeleteChecker(idx, deleteAnyway) {
    $(".loading-child").hide();
    $(".loading").fadeIn(300);
    $("#cart-warning-dialog").center();
    $('#cart-warning-dialog,#designsConflict,#productsConflict,#designProductConflict,#productDesignConflict,#deleteBakeryConflict').hide();
    if (deleteAnyway == null | deleteAnyway == undefined) { deleteAnyway = false; }
    $.ajax({
        url: "/Order/DeleteProductCheck?idx=" + idx + "&deleteAnyway=" + deleteAnyway,
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result == "ok") { addDeleteLoading(false); location.reload(); }
            else if (result == "index") { addDeleteLoading(false); window.location = "/"; }
            else if (result == "deleteBakeryConflict") {
                $("#cart-warning-dialog,#deleteBakeryConflict").fadeIn(300); $("#deleteBakeryConflict").children().children(".black-gradient").attr("onclick", "DeleteChecker('" + idx + "',true)");
            }
        }
    });
}

function resetProductFilters() {
    $(".product-div-wrapper").show();
    $("#product_category_filter option:eq(0),#product_bakery_filter option:eq(0),#product-sort option:eq(0)").prop("selected", true);
    $(".remove-filters").fadeOut(200);
}

function filterAdminProducts(source) {
    var category = $("#product_category_filter").find(':selected').attr('data-sort');
    var bakery = $("#product_bakery_filter").find(':selected').attr('data-sort');
    $(".product-div-wrapper").hide();
    if (bakery != "All" & category == "All") { $(".product-div-wrapper[data-bakery*='" + bakery + "']").show(); }
    if (bakery == "All" & category != "All") { $(".product-div-wrapper[data-category*='" + category + "']").show(); }
    if (category != "All" & bakery != "All") { $(".product-div-wrapper[data-category*='" + category + "'][data-bakery*='" + bakery + "']").show(); }
    $(".remove-filters").fadeIn(200);
}

function sortProducts(source, appendTo) {
    var divList = $(appendTo + " .product-div-wrapper");
    divList.sort(function (a, b) {
        if (source == "Rating desc") { return $(b).attr("data-rating") - $(a).attr("data-rating"); }
        else if (source == "Price desc") { return parseFloat($(b).attr("data-price")) - parseFloat($(a).attr("data-price")); }
        else if (source == "Price asc") { return parseFloat($(a).attr("data-price")) - parseFloat($(b).attr("data-price")); }
        else if (source == "AddDate desc") { return $(a).attr("data-date") - $(b).attr("data-date"); }
        else if (source == "AddDate asc") { return $(b).attr("data-date") - $(a).attr("data-date"); }
    });
    $(appendTo).append(divList);
}

var sbpDivs;

function searchBakeryProducts(elem) {
    var key = $(elem).val().replace("İ", "i").replace("ı", "i").toLowerCase();
    var filterDivs = $(".sortFilterBakeryProduct:visible");
    if (sbpDivs == undefined) { sbpDivs = filterDivs; }
    else { filterDivs = sbpDivs; }

    filterDivs.hide();
    for (i = 0; i < filterDivs.length; i++) {
        if ($(filterDivs[i]).attr("data-name").replace("İ", "i").replace("ı", "i").toLowerCase().indexOf(key) > -1) { $(filterDivs[i]).show(); };
    }

    sbpDivs = filterDivs;
}

var sppDivs;

function searchPartyProducts(elem) {
    var key = $(elem).val().toLowerCase();
    var filterDivs = $(".sortFilterPartyProduct:visible");
    if (sppDivs == undefined) { sppDivs = filterDivs; }
    else { filterDivs = sppDivs; }

    filterDivs.hide();
    for (i = 0; i < filterDivs.length; i++) {
        var nameKeys = $(filterDivs[i]).attr("data-name").split(' ');
        for (i2 = 0; i2 < nameKeys.length; i2++) {
            if (nameKeys[i2].toLowerCase().indexOf(key) > -1) { $(filterDivs[i]).show(); }
        }
    }

    sppDivs = filterDivs;
}

function bakeryCheckFilter(elem) {
    var txt = $(elem).siblings("label").text();

    if (txt == "Boutique Cake") { txt = "Butik Pasta"; }
    else if (txt == "Birthday Cakes") { txt = "Doğum Günü Pastaları"; }
    else if (txt == "Age 1 Cakes") { txt = "1 Yaş Pastaları"; }
    else if (txt == "Baby Cakes") { txt = "Bebek Pastaları"; }
    else if (txt == "Cartoon Cakes") { txt = "Çizgi Film Pastaları"; }
    else if (txt == "Valentines Cakes") { txt = "Sevgili Pastaları"; }
    else if (txt == "Celebration Cakes") { txt = "Kutlama Pastaları"; }
    else if (txt == "Cakes") { txt = "Yaş Pasta"; }
    else if (txt == "Chocolate") { txt = "Çikolata"; }
    else if (txt == "Dessert") { txt = "Tatlı"; }

    var isChecked = $(elem).is(":checked");
    var filterDivs = $(".sortFilterBakeryProduct");

    for (i = 0; i < filterDivs.length; i++) {
        if ($(filterDivs[i]).attr("data-category") == txt) {
            if (isChecked == true) { $(filterDivs[i]).show(); }
            else { $(filterDivs[i]).hide(); }
        }
    }

    sbpDivs = undefined;
}

function designsCheckFilter(elem) {
    var txt = $(elem).siblings("label").text();
    var isChecked = $(elem).is(":checked");
    var filterDivs = $(".filter-design");

    for (i = 0; i < filterDivs.length; i++) {
        if ($(filterDivs[i]).attr("data-category") == txt) {
            if (isChecked == true) { $(filterDivs[i]).show(); }
            else { $(filterDivs[i]).hide(); }
        }
    }

    sidDivs = undefined;
}

function filterBakeryProducts() {
    var filterDivs = $(".sortFilterBakeryProduct");
    var val = $("#bakeryProductFilter").val();
    if (val != "all") {
        filterDivs.hide();
        for (i = 0; i < filterDivs.length; i++) {
            if (val == $(filterDivs[i]).attr("data-category")) { $(filterDivs[i]).show(); }
        }
    }
    else { filterDivs.show(); }
}

function sortBakeryProducts() {
    var condition = $("#sortProductFilter").val();
    var sortDivs = $(".sortFilterBakeryProduct");
    sortDivs.sort(function (a, b) {
        if (condition == "newest") { return new Date($(b).attr("data-date")).getTime() - new Date($(a).attr("data-date")).getTime(); }
        else if (condition == "popular") { return parseInt($(b).attr("data-favs")) - parseInt($(a).attr("data-favs")); }
        else if (condition == "priceAsc") { return parseInt($(a).attr("data-price")) - parseInt($(b).attr("data-price")); }
        else if (condition == "priceDesc") { return parseInt($(b).attr("data-price")) - parseInt($(a).attr("data-price")); }
    });
    $("#sortFilterBakeryProductWrapper").empty();
    $("#sortFilterBakeryProductWrapper").append(sortDivs);
}

function sortPartyProducts() {
    var condition = $("#sortPartyFilter").val();
    var sortDivs = $(".sortFilterPartyProduct");
    sortDivs.sort(function (a, b) {
        if (condition == "newest") { return new Date($(b).attr("data-date")).getTime() - new Date($(a).attr("data-date")).getTime(); }
        else if (condition == "popular") { return parseInt($(b).attr("data-favs")) - parseInt($(a).attr("data-favs")); }
        else if (condition == "priceAsc") { return parseInt($(a).attr("data-price")) - parseInt($(b).attr("data-price")); }
        else if (condition == "priceDesc") { return parseInt($(b).attr("data-price")) - parseInt($(a).attr("data-price")); }
    });
    $("#sortFilterPartyProductWrapper").empty();
    $("#sortFilterPartyProductWrapper").append(sortDivs);
}

function filterAllProducts(cat) {
    $("#allProductsProvinceFilter option:eq(0)").prop("selected", true);
    $("#searchAllProductsFilter").val("");
    var filterDivs = $(".sortAllProduct");
    var val;
    if (cat == null) { val = $("#allProductsFilter").val(); } else { val = cat; }
    if (val != "all") {
        filterDivs.hide();
        if (val == "Tatli") { val = "Tatlı"; }
        for (i = 0; i < filterDivs.length; i++) {
            if (val == $(filterDivs[i]).attr("data-category")) { $(filterDivs[i]).show(); }
        }
        if (val == "Cikolata") { $("#urunlerh1").text("Çikolata Ürünleri"); }
        else if (val == "butik-pasta") { $("#urunlerh1").text("Butik Pasta Ürünleri"); }
        else if (val == "Pasta") { $("#urunlerh1").text("Pasta Ürünleri"); }
        else if (val == "Tatli") { $("#urunlerh1").text("Tatlı Ürünleri"); }
    }
    else { $("#urunlerh1").text(" Ürünler"); filterDivs.show(); }
}

function filterAllProductsByProvince() {
    $("#allProductsFilter option:eq(0)").prop("selected", true);
    $("#searchAllProductsFilter").val(""); $("#urunlerh1").text(" Products");
    var filterDivs = $(".sortAllProduct");
    var val = $("#allProductsProvinceFilter").val();

    if (val != "all") {
        filterDivs.hide();
        for (i = 0; i < filterDivs.length; i++) {
            if (val == $(filterDivs[i]).attr("data-province")) { $(filterDivs[i]).show(); }
        }
    }
    else { filterDivs.show(); }
}

function sortAllProducts() {
    var condition = $("#sortAllFilter").val();
    var sortDivs = $(".sortAllProduct");
    sortDivs.sort(function (a, b) {
        if (condition == "newest") { return new Date($(b).attr("data-date")).getTime() - new Date($(a).attr("data-date")).getTime(); }
        else if (condition == "popular") { return parseInt($(b).attr("data-favs")) - parseInt($(a).attr("data-favs")); }
        else if (condition == "priceAsc") { return parseInt($(a).attr("data-price")) - parseInt($(b).attr("data-price")); }
        else if (condition == "priceDesc") { return parseInt($(b).attr("data-price")) - parseInt($(a).attr("data-price")); }
    });
    $("#sortAllWrapper").empty();
    $("#sortAllWrapper").append(sortDivs);
}

/****************** Design Functions ******************/

function AddDesignChecker(madeID, addAnyway) {
    $(".loading-child").hide();
    $(".loading").fadeIn(300);
    $("#cart-warning-dialog").center();
    $('#cart-warning-dialog,#designsConflict,#productsConflict,#designProductConflict,#productDesignConflict,#deleteBakeryConflict').hide();
    if (addAnyway == null | addAnyway == undefined) { addAnyway = false; }
    $.ajax({
        url: "/Order/AddDesignChecker?MadeID=" + madeID + "&addAnyway=" + addAnyway + "&sizeOption=" + $("#select-design-size").val(),
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result == "ok") { addDeleteLoading(true); window.location = "/Sepetim"; }
            else if (result == "designsConflict") {
                $("#cart-warning-dialog,#designsConflict").fadeIn(300); $("#designsConflict").children().children(".black-gradient").attr("onclick", "AddDesignChecker('" + madeID + "',true)");
            }
            else if (result == "designProductConflict") {
                $("#cart-warning-dialog,#designProductConflict").fadeIn(300); $("#designProductConflict").children().children(".black-gradient").attr("onclick", "AddDesignChecker('" + madeID + "',true)");
            }
        }
    });
}

function changeDesignName(elem, id, newName) {
    $(elem).parent().hide();
    $(elem).parent().parent().find('.cakeNameDiv').fadeIn(200);
    $(elem).parent().parent().find('.cakeNameDiv label').text($(elem).parent().find('.design-name-input').val());
    $.ajax({
        url: "/Object/ChangeDesignName?designID=" + id + "&newName=" + newName,
        type: 'GET',
        cache: false,
        success: function (result) {
        }
    });
}

var sidDivs;

function searchIncomingDesigns(elem) {
    var key = $(elem).val().toLowerCase();
    var designDivs = $(".filter-design:visible");
    if (sidDivs == undefined) { sidDivs = designDivs; }
    else { designDivs = sidDivs; }

    designDivs.hide();
    for (i = 0; i < designDivs.length; i++) {
        var nameKeys = $(designDivs[i]).attr("data-name").split(' ');
        for (i2 = 0; i2 < nameKeys.length; i2++) {
            if (nameKeys[i2].toLowerCase().indexOf(key) > -1) { $(designDivs[i]).show(); }
        }
    }

    sidDivs = designDivs;
}

function searchAllProducts(elem) {
    $("#allProductsProvinceFilter option:eq(0),#allProductsFilter  option:eq(0)").prop("selected", true); $("#urunlerh1").text(" Products");
    var key = $(elem).val().toLowerCase();
    var designDivs = $(".sortAllProduct");

    designDivs.hide();
    for (i = 0; i < designDivs.length; i++) {
        var nameKeys = $(designDivs[i]).attr("data-name").split(' ');
        for (i2 = 0; i2 < nameKeys.length; i2++) {
            if (nameKeys[i2].toLowerCase().indexOf(key) > -1) { $(designDivs[i]).show(); }
        }
    }
}

function searchValentines(elem) {
    var key = $(elem).val().toLowerCase(); console.log(key);
    var designDivs = $(".filter-design, .sortAllProduct");

    designDivs.hide();
    for (i = 0; i < designDivs.length; i++) {
        var nameKeys = $(designDivs[i]).attr("data-name").split(' ');
        for (i2 = 0; i2 < nameKeys.length; i2++) {
            if (nameKeys[i2].toLowerCase().indexOf(key) > -1) { $(designDivs[i]).show(); }
        }
    }
}

function sortIncomingDesigns(elem) {
    var condition = $(elem).val();
    var designDivs = $(".filter-design");
    designDivs.sort(function (a, b) {
        if (condition == "newest") { return new Date($(b).attr("data-date")).getTime() - new Date($(a).attr("data-date")).getTime(); }
        else if (condition == "popular") { return $(b).attr("data-likes") - $(a).attr("data-likes"); }
        else if (condition == "priceAsc") { return $(b).attr("data-price") - $(a).attr("data-price"); }
        else if (condition == "priceDesc") { return $(a).attr("data-price") - $(b).attr("data-price"); }
    });
    $("#filter-design-wrapper").append(designDivs);
}

function sortValentines(elem) {
    var condition = $(elem).val();
    var designDivs = $(".filter-design, .sortAllProduct");
    designDivs.sort(function (a, b) {
        if (condition == "newest") { return new Date($(b).attr("data-date")).getTime() - new Date($(a).attr("data-date")).getTime(); }
        else if (condition == "popular") { return $(b).attr("data-likes") - $(a).attr("data-likes"); }
        else if (condition == "priceAsc") { return $(b).attr("data-price") - $(a).attr("data-price"); }
        else if (condition == "priceDesc") { return $(a).attr("data-price") - $(b).attr("data-price"); }
    });
    $("#filter-design-wrapper").append(designDivs);
}

function filterValentines(elem) {
    var key = $(elem).val().toLowerCase();
    var designDivs = $(".filter-design, .sortAllProduct");

    if (key == "all") { designDivs.show(); }
    else {
        designDivs.hide();
        for (i = 0; i < designDivs.length; i++) {
            if (key.toString() == $(designDivs[i]).attr("data-type").toString()) { $(designDivs[i]).show(); }
        }
    }
}

function filterIncomingDesignsSize(elem) {
    var key = $(elem).val().toLowerCase();
    var designDivs = $(".filter-design");

    if (key == "all") { designDivs.show(); }
    else {
        designDivs.hide();
        for (i = 0; i < designDivs.length; i++) {
            if (key.toString() == $(designDivs[i]).attr("data-size").toString()) { $(designDivs[i]).show(); }
        }
    }
}

function filterIncomingDesigns(elem, value) {
    var condition;
    if (value == null) { condition = $(elem).val(); } else { condition = value; }
    var designDivs = $(".filter-design");
    if (condition == "all") { $("#tasarimlarh1").text("Tasarım Pastalar"); designDivs.show(); }
    else {
        designDivs.hide();
        for (i = 0; i < designDivs.length; i++) {
            if (condition == $(designDivs[i]).attr("data-category")) { $(designDivs[i]).show(); }
        }
        $("#tasarimlarh1").text(condition);

        if (condition == "Doğum Günü Pastaları") {
            desc = "Doğum günü pastaları için tasarlanan en güzel pastaları burada bulabilir, anında butik pasta siparişi verip sevdiklerinize güzel bir sürpriz yapabilirsiniz.";
        }
        else if (condition == "Bebek Pastaları") {
            desc = "iCaked kullanıcıları tarafından 3 boyutlu pasta tasarım editörü kullanılarak tasarlanmış birbirinden güzel yüzlerce bebek pastası burada. ";
        }
        else if (condition == "1 Yaş Pastaları") {
            desc = "Bebeğinizin 1. yaş gününü unutulmaz kılın, kullanıcılar tarafından tasarlanan en güzel 1 yaş butik pastalar arasından en beğendiğinizi anında sipariş verin";
        }
        else if (condition == "Çizgi Film Pastaları") {
            desc = "Çocuğunuz için unutulmaz bir doğum günü partisi hazırlamak ister misiniz? Ona en sevdiği çizgi film karakterlerinden oluşan bir butik pasta siparişi verin.";
        }
        else if (condition == "Sevgili Pastaları") {
            desc = "Sevgilinize özel bir pasta yaptırarak ona unutmayacağı bir sürpriz yaşamak ister misiniz? En güzel sevgili pastasını seçin ve hemen sipariş verin.";
        }
        else if (condition == "Kutlama Pastaları") {
            desc = "Arkadaşınıza işe giriş hediyesi veya bir yıldönümü mü kutlayacaksınız? Tam ağzınıza layık kutlama pastalarına bir göz atın…";
        }
        else if (condition == "Yarışma Pastaları") {
            desc = "Yarışmaya katılın ve en güzel butik pasta ve tasarım pastalardan kazanma şansı yakalayın…";
        }
        else { desc = ""; }

        $("#tasarimlar-upper-description").text(desc);
    }
}

/****************** Login / Register Functions ******************/

function Login() {
    $("#login-button").fadeOut(0);
    $.ajax({
        url: "/Account/Login?email_login=" + $("#email").val() + "&password_login=" + $("#password_login").val() + "&rememberMe=" + $("#rememberMe").is(":checked") + "",
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result == true) {
                if (view == "editor") {
                    $("#account-floating-div,#login-hidden,#login-register-div").fadeToggle(200); $(".overall-mask").fadeOut(200); isLogin = true;
                }
                else {
                    window.location.href = '/Profil';
                }
            }
            else {
                $("#login-loading").hide();
                $("#login-alert-label").slideDown(100);
                $("#login-alert-label").text("Giriş başarısız. Lütfen tekrar deneyin.");
                $("#login-button").fadeIn(0);
            }
        }
    });
}

function Logout() {
    if (confirm("Çıkış yapılacaktır. Onaylıyor musunuz?")) {
        isLogin = false;
        //if (isfblogin) { facebookLogout(); }
        setTimeout(function () { window.location = "/Account/Logout"; }, 500);
    }
}

var emailCheck;

function checkEmail() {
    $.ajax({
        url: "/Account/CheckEmail?email=" + $("#email_register").val() + "",
        type: 'GET',
        cache: false,
        success: function (result) {
            emailCheck = result;
        }
    });
}

function Register() {
    $("#register-button").fadeOut(0);
    $.ajax({
        url: "/Account/Register?email_register=" + $("#email_register").val() + "&name_register=" + $("#name_register").val() + "&surname_register=" + $("#surname_register").val() + "&password_register=" + $("#password_register").val() + "",
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result == true) {
                if (view == "editor") {
                    $("#account-floating-div,#login-hidden,#login-register-div").fadeToggle(200); $(".overall-mask").fadeOut(200); isLogin = true;
                }
                else {
                    window.location.href = '/Profil';
                }
            }
            else {
                $("#register-button").fadeIn(0);
            }
        }
    });
}

function Forgot() {
    $("#forgot-button").fadeOut(0);
    $.ajax({
        url: "/Account/Forgot?email=" + $("#forgot-email").val(),
        type: 'GET',
        cache: false,
        success: function (result) {
            $("#forgot-button").fadeIn(0);
            if (result == true) { $("#forgot-loading").hide(); $("#forgot-alert-label").slideDown(100); $("#forgot-alert-label").css("color", "green"); $("#forgot-alert-label").text("Yeni şifreniz e-posta adresinize gönderilmiştir."); }
            else {
                $("#forgot-loading").hide();
                $("#forgot-alert-label").slideDown(100);
                $("#forgot-alert-label").css("color", "red");
                $("#forgot-alert-label").text("Bu e-posta ile kayıtlı bir kullanıcı bulunmamaktadır.");
            }
        }
    });
}

function DeleteAccount() {
    if (confirm("Hesabınız silinecektir. Bilgileriniz geri döndürülemez. Onaylıyor musunuz?") == false) { event.preventDefault(); }
}

/****************** Validation Functions ******************/

function validateLogin() {
    var emailValidated = ($("#email").val().length > 0); var passwordValidated = ($("#password_login").val().length > 0);

    if (emailValidated & passwordValidated) { $("#login-loading").fadeIn(100); $("#login-alert-label").slideUp(100); Login(); }
    else {
        $("#login-alert-label").slideDown(100); $("#login-alert-label").text("All fields must be filled.");
        if (emailValidated == false) { $("#email-login-div").css("border-color", "red"); }
        if (passwordValidated == false) { $("#password-login-div").css("border-color", "red"); }
    }
}

function validateRegister() {
    $("#register-loading").fadeIn(100);
    checkEmail();

    var nullCheck = ($("#email_register").val().length > 0) & ($("#name_register").val().length > 0) & ($("#surname_register").val().length > 0) & ($("#password_register").val().length > 0) & ($("#password_confirm_register").val().length > 0);
    var passwordCheck = ($("#password_register").val() == $("#password_confirm_register").val());
    var emailSyntaxCheck = ($("#email_register").val().indexOf("@") > -1 & $("#email_register").val().indexOf(".") > -1);
    var contractCheck = $("#acceptUserContract").prop("checked");

    setTimeout(function () {
        if (nullCheck & passwordCheck & emailCheck & emailSyntaxCheck & contractCheck) { $("#register-alert-label").slideUp(100); Register(); }
        else {
            $("#register-loading").hide(); $("#register-alert-label").slideDown(100);

            if (nullCheck == false) {
                $("#register-floating-div input").filter(function () { if ($(this).val().length == 0) { $(this).parent().css("border-color", "red"); } })
                $("#register-floating-div select").filter(function () { if ($(this).val() == null) { $(this).parent().css("border-color", "red"); } })
                $("#register-alert-label").text("All fields must be filled.");
            }

            if (passwordCheck == false) { $("#password-confirm-register-div,#password-register-div").css("border-color", "red"); $("#register-alert-label").text("Passwords must keep one another."); }
            if ($("#email_register").val().length > 0 & emailSyntaxCheck == false) { $("#email-register-div").css("border-color", "red"); $("#register-alert-label").text("Please enter a valid email."); }
            if (emailCheck == false) { $("#email-register-div").css("border-color", "red"); $("#register-alert-label").text("This e-mail is registered on the system."); }
            if (contractCheck == false) { $("#acceptUserContract").css("box-shadow", "0px 0px 6px 0px red"); $("#register-alert-label").text("You have to accept the contract."); }
        }
    }, 1000);
}

function validateForgot() {
    $("#forgot-alert-label").css("color", "red");
    if ($("#forgot-email").val().length > 0) {
        if ($("#forgot-email").val().indexOf("@") > -1 & $("#forgot-email").val().indexOf(".") > -1) { $("#forgot-loading").fadeIn(100); $("#forgot-alert-label").slideUp(100); Forgot(); }
        else { $("#forgot-alert-label").slideDown(100); $("#forgot-email-div").css("border-color", "red"); $("#forgot-alert-label").text("Please enter a valid email."); }
    }
    else { $("#forgot-alert-label").slideDown(100); $("#forgot-alert-label").text("Enter your e-mail."); }
}

function validateDeleteProduct() {
    var name = $("#bakery_product_delete").attr("data-productName");
    if (confirm(name + " named product will be deleted. Do you confirm?")) { }
    else { event.preventDefault(); }
}

function validateEditBlog(elem) {
    var inputs = $(".pop-input"); var validated = true;
    for (i = 0; i < inputs.length; i++) {
        if ($($(inputs)[i]).val().length == 0) { $($(inputs)[i]).css("border-color", "red"); validated = false; }
    }
    if (validated == false) { event.preventDefault(); }
}

function validateAddBlog() {
    event.preventDefault();
    var validated = true;
    if ($("#addBlogTitle").val().length == 0) { $("#addBlogTitle").css("border-color", "red"); validated = false; }
    if ($("#addBlogComment").val().length == 0) { $("#addBlogComment").css("border-color", "red"); validated = false; }
    if (validated == true) {
        $("#write-blog-div").hide(300);
        setTimeout(function () { $("#blog-thanks-div").show(300); }, 310);
        setTimeout(function () { $("#addBlogForm").submit(); }, 1000);
    }
}

function validateBakeryInfo() {
    var validated = true;
    $("#bakery_weekdays").val("");
    var days = $(".bakery_weekdays");
    for (i = 0; i < days.length; i++) { if ($(days[i]).hasClass("bw_active")) { $("#bakery_weekdays").val($("#bakery_weekdays").val() + "1"); } else { $("#bakery_weekdays").val($("#bakery_weekdays").val() + "0"); } }

    var inputs = $(".bakeryInfo input[type=text], .bakeryInfo textarea");
    for (i = 0; i < inputs.length; i++) { if ($(inputs[i]).val().length == 0) { $(inputs[i]).css("border-color", "red"); validated = false; } }
    if ($("#bakery_phone").val() == "(___) ___-__-__") { validated = false; }

    if ($("#bakeryTime1").val() == null) { $("#bakeryTime1").css("border-color", "red"); validated = false; }
    if ($("#bakeryTime2").val() == null) { $("#bakeryTime2").css("border-color", "red"); validated = false; }

    if (validated == false) { event.preventDefault(); }
}

function validatePopInput(elem) {
    if ($(elem).val().length == 0) { $(elem).css("border-color", "red"); }
    else { $(elem).css("border-color", "#cccccc"); }
}

function validateEditComment(elem) {
    var inputs = $(".pop-input"); var validated = true;
    for (i = 1; i < inputs.length; i++) {
        if ($($(inputs)[i]).val().length == 0) { $($(inputs)[i]).css("border-color", "red"); validated = false; }
    }
    if (validated == false) { event.preventDefault(); }
}

function validateProductComment() {
    var addCommentValidated = true;
    var commentElems = ["#product-comment-name", "#product-comment-content"];
    for (i = 0; i < commentElems.length; i++) {
        if ($(commentElems[i]).val().length == 0) { $(commentElems[i]).css("border-color", "red"); addCommentValidated = false; }
    }
    if (addCommentValidated == false) { event.preventDefault(); }
}

function validateDesignComment() {
    var addCommentValidated = true;
    var commentElems = ["#design-comment-name", "#design-comment-content"];
    for (i = 0; i < commentElems.length; i++) {
        if ($(commentElems[i]).val().length == 0) { $(commentElems[i]).css("border-color", "red"); addCommentValidated = false; }
    }
    if (addCommentValidated == false) { event.preventDefault(); }
}

function validateAdminObjUpload(event) {
    var objUploadValidated = true;
    var uploadElem = ["#admin-object-keywords", "#admin-object-minsize", "#admin-object-maxsize", "#admin-object-stepsize"];

    for (i = 0; i < uploadElem.length; i++) {
        if ($(uploadElem[i]).val().length == 0) { $(uploadElem[i]).css("border-color", "red"); objUploadValidated = false; } else { $(uploadElem[i]).css("border-color", "#cccccc"); }
    }

    if (objUploadValidated == false) { event.preventDefault(); }
}

function validateAddPrice(elem) {
    var addPriceValidated = true;
    if ($(elem).find("input[name=size]").val() == 0) { $(elem).find("input[name=size]").css("border-color", "red"); addPriceValidated = false; }
    if ($(elem).find("input[name=price]").val().length == 0) { $(elem).find("input[name=price]").css("border-color", "red"); addPriceValidated = false; }
    if (addPriceValidated == false) { event.preventDefault(); }
}

function validateAddContent(elem) {
    var addContentValidated = true;
    if ($(elem).find("input[name=content]").val().length == 0) { $(elem).find("input[name=content]").css("border-color", "red"); addContentValidated = false; }
    if (addContentValidated == false) { event.preventDefault(); }
}

function validateAnonAddress() {
    event.preventDefault();
    var anonAddressValidated = true;
    if ($("#anonProvinceAddress").val() == null) { $("#anonProvinceAddress").css("border-color", "red"); anonAddressValidated = false; }
    if ($("#anonDistrictAddress").val() == null) { $("#anonDistrictAddress").css("border-color", "red"); anonAddressValidated = false; }
    if ($("#anonNeighborhoodAddress").val() == null) { $("#anonNeighborhoodAddress").css("border-color", "red"); anonAddressValidated = false; }
    if ($("#anonOpenAddress").val().length == 0) { $("#anonOpenAddress").css("border-color", "red"); anonAddressValidated = false; }
    // if ($("#anonAddressDescription").val().length == 0) { $("#anonAddressDescription").css("border-color", "red"); anonAddressValidated = false; }

    if (anonAddressValidated == true) {
        $("#anonAddressForm").submit();
    }
}

function validateKeyup(elem) {
    if ($(elem).val().length == 0) { $(elem).css("border-color", "red"); } else { $(elem).css("border-color", "#cccccc"); }
}

function validateChange(elem) {
    console.log($(elem).val().length);
    if ($(elem).val().length == 0) { $(elem).css("border-color", "red"); } else { $(elem).css("border-color", "#cccccc"); }
}

function validateFatura2() {
    // amk
    var faturaValidated2 = true;

    if ($("#bireyselFatura").is(":checked") == false & $("#kurumsalFatura").is(":checked") == false) {
        $($("#bireyselFatura").parent().find("label")).css("color", "red");
        $($("#kurumsalFatura").parent().find("label")).css("color", "red");
        faturaValidated2 = false;
    }
    else {
        $($("#bireyselFatura").parent().find("label")).css("color", "black");
        $($("#kurumsalFatura").parent().find("label")).css("color", "black");

        if ($("#faturaName").val().length == 0) { faturaValidated2 = false; $("#faturaName").css("border-color", "red"); }
        if ($("#faturaAdres").val().length == 0) { faturaValidated2 = false; $("#faturaAdres").css("border-color", "red"); }

        if ($("#kurumsalFatura").is(":checked")) {
            if ($("#faturaID").val().length == 0) { faturaValidated2 = false; $("#faturaID").css("border-color", "red"); }
            if ($("#faturaDaire").val().length == 0) { faturaValidated2 = false; $("#faturaDaire").css("border-color", "red"); }
        }
    }

    return faturaValidated2;
}

function validateFatura() {
    var faturaValidated = true;
    if ($("#faturaAdresSame").length == 0) {
        if (validateFatura2() == false) { faturaValidated = false; }
    }
    else {
        if ($("#faturaAdresSame").is(":checked") == false) {
            if (validateFatura2() == false) { faturaValidated = false; }
        }
    }

    if (faturaValidated) { exportPdf(false); }

    return faturaValidated;
}

function validateInfo() {
    event.preventDefault();
    var infoValidated = true;

    var elems = $("#contactDiv input[type=text]");

    for (i = 0; i < elems.length; i++) {
        if ($(elems[i]).val().length == 0) { changeTooltip(elems[i], "Bu alan boş bırakılamaz."); $(elems[i]).css("border-color", "red"); infoValidated = false; }
        else { changeTooltip(elems[i], ""); $(elems[i]).css("border-color", "#d6d6d6"); }
    }

    if ($("#contactTel").val() == "(___) ___-__-__" | $("#contactTel").val().length == 0) { $("#contactTel").css("border-color", "red"); infoValidated = false; }
    else { $("#contactTel").css("border-color", "#d6d6d6"); }

    if ($("#contactMail").val().indexOf('@') < 0 | $("#contactMail").val().indexOf('.') < 0) { changeTooltip("#contactMail", "Lütfen geçerli bir e-posta adresi giriniz."); $("#contactMail").css("border-color", "red"); infoValidated = false; }
    else { $("#contactMail").css("border-color", "#d6d6d6"); }

    if (infoValidated == false) {
        event.preventDefault();
    }
    else {
        carryHiddens(); $("#deliveryInfoForm").submit();
    }
}

function validatePayment() {
    var paymentValidated = true;

    if (!$("#sozlesmeCheck").is(":checked")) { changeTooltip("#sozlesmeLabel", "Devam etmek için sözleşmeyi kabul etmeniz gerekmektedir."); $("#sozlesmeLabel").css("color", "red"); paymentValidated = false; }
    else { changeTooltip("#sozlesmeLabel", ""); $("#sozlesmeLabel").css("color", "black"); }

    return paymentValidated;
}

function validatePaymentPos() {
    event.preventDefault();
    var paymentValidated = true;

    //if (subedeOdeme == false) { paymentValidated = validateFatura(); }

    var elems = $("#paymentForm input[type=text]:visible:not(#promCodeInput)");

    var cardnums = $("#paymentForm .cardNum");

    for (i = 0; i < elems.length; i++) {
        if ($(elems[i]).val().length == 0) { changeTooltip(elems[i], "Bu alan boş bırakılamaz."); $(elems[i]).css("border-color", "red"); paymentValidated = false; }
        else { changeTooltip(elems[i], ""); $(elems[i]).css("border-color", "#d6d6d6"); }
    }

    for (i = 0; i < cardnums.length; i++) {
        if ($(cardnums[i]).val().length < 4) { changeTooltip(cardnums[i], "Min. 4 sayı girilmelidir."); $(cardnums[i]).css("border-color", "red"); paymentValidated = false; }
        else { changeTooltip(cardnums[i], ""); $(cardnums[i]).css("border-color", "#d6d6d6"); }
    }

    paymentValidated = validatePayment();

    if (paymentValidated == false) { event.preventDefault(); }
    else { loadPayment(); $("#paymentForm").submit(); }
}

function validatePaymentDirect() {
    carryHiddens();
    event.preventDefault();
    var paymentValidated = true;

    //  if (subedeOdeme == false) { paymentValidated = validateFatura(); }

    paymentValidated = validatePayment();

    if (!paymentValidated) { event.preventDefault(); }
    else {
        loadPayment();
        $("#paymentForm").submit();
    }
}

function validateFeedbackForm() {
    var feedbackFormValidated = true;
    var elements = ["#feedbackFormName", "#feedbackFormEmail", "#feedbackFormContent"];

    for (i = 0; i < elements.length; i++) {
        if ($(elements[i]).val().length == 0) { $(elements[i]).css("border-color", "red"); feedbackFormValidated = false; } else { $(elements[i]).css("border-color", "#cccccc"); }
    }

    if (feedbackFormValidated == false) { event.preventDefault(); }
    else {
        $("#feedbackFormSubmit").fadeOut(300);
        setTimeout(function () { $("#feedbackFormLoading").fadeIn(300); }, 310);
    }
}

function validateIstekForm() {
    var istekFormValidated = true;
    var elements = ["#istekTasarimName", "#istekTasarimMail", "#istekTasarimMessage"];

    for (i = 0; i < elements.length; i++) {
        if ($(elements[i]).val().length == 0) { $(elements[i]).css("border-color", "red"); istekFormValidated = false; } else { $(elements[i]).css("border-color", "#cccccc"); }
    }

    if (istekFormValidated == false) { event.preventDefault(); }
    else {
        $("#istekTasarimSubmit").fadeOut(300);
        setTimeout(function () { $("#istekTasarimLoading").fadeIn(300); }, 310);
    }
}

function validateFigurForm() {
    var istekFormValidated = true;
    var elements = ["#istekFigurName", "#istekFigurMail", "#istekFigurMessage"];

    for (i = 0; i < elements.length; i++) {
        if ($(elements[i]).val().length == 0) { $(elements[i]).css("border-color", "red"); istekFormValidated = false; } else { $(elements[i]).css("border-color", "#cccccc"); }
    }

    if (istekFormValidated == false) { event.preventDefault(); }
    else {
        $("#istekFigurSubmit").fadeOut(300);
        setTimeout(function () { $("#istekFigurLoading").fadeIn(300); }, 310);
    }
}

function validateSozlesme() {
    if (!$("#sozlesmeCheck").is(":checked")) { } else { $("#sozlesmeCheck").css("box-shadow", "none"); }
}

function validateBakeryProduct() {
    //var bakeryProductValidated = true;

    //var bakeryDivs = ["bakery_product_name", "bakery_product_description", "bakery_product_stockAvailable", "bakery_product_price"];

    //for(i = 0; i < bakeryDivs.length; i++){
    //    if ($("#" + bakeryDivs[i]).val().length == 0) { $("#" + bakeryDivs[i]).css("border-color", "red"); bakeryProductValidated = false; }
    //}

    //if (bakeryProductValidated == false) { event.preventDefault(); }
}

function validateAddLocation() {
    event.preventDefault();
    var addLocationValidated = true;
    var locationDivs = ["#bakeryProvinceLocation", "#bakeryDistrictLocation", "#bakeryTownLocation"];

    for (i = 0; i < locationDivs.length; i++) {
        if ($(locationDivs[i]).val() != null) {
            if ($(locationDivs[i]).val().length == 0) { $(locationDivs[i]).css("border-color", "red"); addLocationValidated = false; }
        }
        else { $(locationDivs[i]).css("border-color", "red"); addLocationValidated = false; }
    }

    if (addLocationValidated == true) {
        $("#addLocationButton").hide(0);
        $("#addLocationLoadingDiv").fadeIn(200);
        $("#addLocationForm").submit();
    }
}

function validateDeleteLocation() {
    if (confirm("Seçtiğiniz lokasyon bilgisi silinecektir. Onaylıyor musunuz?")) { }
    else { event.preventDefault(); }
}

function validateBakeryReport() {
    var reportValidated = true;
    var reportDivs = ["#orderStart", "#orderEnd", "#deliveryStart", "#deliveryEnd", "#priceStart", "#priceEnd"];

    for (i = 0; i < reportDivs.length; i++) {
        if ($(reportDivs[i]).val().length == 0) { $(reportDivs[i]).css("border-color", "red"); reportValidated = false; } else { $(reportDivs[i]).css("border-color", "#cccccc"); }
    }

    if (reportValidated == true) {
        getDataSet();
    }
}

function validateSiparisTakip() {
    if ($("#OrderNumber").val().length == 0) {
        console.log("aa");
        $("#OrderNumber").css("border-color", "red");
        event.preventDefault();
    }
}

function validateDeleteOrder() {
    if (!confirm("Seçtiğiniz sipariş geri getirilemeyecek şekilde silinecektir. Onaylıyor musunuz?")) { event.preventDefault(); }
}

function validateContactMail() {
    var temp = $("#contactMailWrapper input").val();
    if (temp.length == 0 | temp.indexOf('@') < 0 | temp.indexOf('.') < 0) { event.preventDefault(); $("#contactMailWrapper").css("border-color", "red"); }
    else {
        $.ajax({
            url: "/Home/JoinMail?mail=" + temp,
            type: 'GET',
            cache: false,
            success: function (result) {
                $("#contactMailFail").hide();
                if (result == true) {
                    setTimeout(function () { $("#contactMailSuccess").slideDown(200); }, 210);
                }
                else { $("#contactMailFail").slideDown(200); }
            }
        });
    }
}

function validateAddDesignPhotos() {
    $("#addDesignPhotosThanks").hide();
    var addDesignPhotosValidated = true;
    if ($("#designImages").val().length == 0) {
        addDesignPhotosValidated = false;
    }
    if (addDesignPhotosValidated == false) { $("#addDesignPhotosAlert").slideDown(200); event.preventDefault(); } else { $("#addDesignPhotosAlert").slideUp(200); }
}

/****************** Admin Functions ******************/

function editAdminComment(elem, id) {
    var row = $(elem).parent().parent();
    $(row).find("td").height($(row).find("td").height());
    if ($(row).find("input[type=text]").length != 0) {
        $(row).find("input[type=text]").remove(); $(row).find("input[type=hidden]").remove(); $(row).find("input[type=number]").remove(); $(row).find("input[type=checkbox]").remove(); $(row).find("input[type=image]").hide();
        for (i = 0; i < $(row).children().length; i++) {
            if (i > 1 & i < 7) {
                $($(row).children()[i]).children("label").show();
            }
        }
    }
    else {
        var names = ["id='commentName' name='commentName'", "id='commentTitle' name='commentTitle'", "id='commentComment' name='commentComment'", "id='commentLike' name='commentLike'", "id='commentDislike' name='commentDislike'"];
        $(row).find("input[type=image]").show();
        $($(row).children()[0]).append("<input type='hidden' name='commentID' id='commentID' value='" + id + "' />"); var type = "";
        for (i = 0; i < $(row).children().length; i++) {
            var width = $($(row).children()[i]).width();
            if (i > 1 & i < 7) {
                if (i == 5 | i == 6) { type = "type='number'"; } else { type = "type='text'"; } console.log($(row).children()[i]);
                $($(row).children()[i]).children("label").hide();
                $($(row).children()[i]).append("<input class='pop-input' style='width:" + width + "px;' onkeyup='validatePopInput(this)' autocomplete='off' " + names[i - 2] + type + " value='" + $($(row).children()[i]).children("label").text() + "' />");
            }
            if (i == 7) {
                var checked = "checked"; if ($($(row).children()[i]).children("label").text() == "Onaysız") { checked = ""; }
                $($(row).children()[i]).append("<input class='pop-input' name='admin-comment-approved' id='admin-comment-approved' style='margin-top:3px; margin-right:5px; float:left;' " + names[i - 1] + " type='checkbox' " + checked + " />");
            }
        }
    }
}

function deleteAdminComment() {
    if (confirm("Seçtiğiniz yorum silinecektir. Onaylıyor musunuz?") == false) { event.preventDefault(); }
}

function deleteAdminBlog() {
    if (confirm("Seçtiğiniz blog silinecektir. Onaylıyor musunuz?") == false) { event.preventDefault(); }
}

function adminObjClose(elem) {
    $("#admin-object-name").text("");
    $("#admin-object-category").val("object");
    $("#admin-object-approve").attr("checked", false);
    $("#admin-object-keywords,#admin-object-price").val("");
    $("#admin-object-form").attr("action", "/Object/AdminAddObj");
    $("#admin-object-edit-submit").hide(); $("#admin-object-submit").show();
    $("#admin-object-close").fadeOut(200);
    $("#admin-object-properties").val("");

    $("#object-image-display").attr("src", "");
    $("#object-texture-display").attr("src", "");

    resetFormElement($("#objectFile")); resetFormElement($("#objectImage")); resetFormElement($("#objectTexture"));
}

function adminObjEdit(elem) {
    $("#admin-object-name").text($(elem).attr("data-objname"));

    $("#admin-object-type").val($(elem).attr("data-objtype"));
    $("#admin-object-keywords").val($(elem).attr("data-objkeywords"));
    $("#admin-object-minsize").val($(elem).attr("data-objminsize").replace(",", "."));
    $("#admin-object-maxsize").val($(elem).attr("data-objmaxsize").replace(",", "."));
    $("#admin-object-stepsize").val($(elem).attr("data-objstepsize").replace(",", "."));
    $("#admin-object-difficulty").val($(elem).attr("data-objdifficulty"));
    if ($(elem).attr("data-objapproved") == "yes") { $("#admin-object-approve").attr("checked", true); } else { $("#admin-object-approve").attr("checked", false); }

    $("#admin-object-properties").val($(elem).attr("data-objprop"));

    $("#admin-object-color").val($(elem).attr("data-objcolor")).css("background", "#" + $(elem).attr("data-objcolor"));
    $("#admin-object-form").attr("action", "/Object/AdminEditObj?ObjectID=" + $(elem).attr("data-objid"));
    $("#admin-object-submit").hide(); $("#admin-object-edit-submit").show();
    $("#admin-object-close").fadeIn(200);
    var parent = $(elem).parent().parent();

    $("#object-image-display").attr("src", parent.children("td").children(".img-popup").attr("src"));
    $("#object-texture-display").attr("src", parent.children("td").children(".texture-popup").attr("src"));
    $("#object-alpha-display").attr("src", parent.children("td").children(".alpha-popup").attr("src"));

    $("#admin-category-append").empty();
    var cat_temp = $(elem).attr("data-objcategories");
    var cats = cat_temp.split(',');
    categoryCounter = 1;
    for (i = 0; i < cats.length; i++) {
        if (categoryCounter < 6) {
            var elem = "<div style='width:100%; float:left;'><div style='margin-left:77px; float:left; width:148px; border-bottom:1px solid #e6e6e6;'><input value='" + cats[i] + "' type='text' name='adminCategory" + categoryCounter + "' readonly style='float:left; border:none; width:110px; margin-bottom:0px;' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); categoryCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div></div>";
            categoryCounter++;
            $("#admin-category-append").append(elem);
        }
    }

    resetFormElement($("#objectFile")); resetFormElement($("#objectImage")); resetFormElement($("#objectTexture")); resetFormElement($("#objectAlpha"));
}

function adminObjDelete(elem) {
    var name = $(elem).attr("data-objname");
    if (confirm(name + " isimli obje silinecektir. Onaylıyor musunuz?")) {
        window.location = "/Object/AdminDeleteObj?ObjectID=" + $(elem).attr("data-objid");
    }
    else {
        event.preventDefault();
    }
}

function toggleAdminDivs(source, elem, that) {
    $(".admin-menu-div").hide();
    if (elem == "objects") { $("#" + source).fadeIn(200); }
    else if (elem == "products") { $("#" + source).fadeIn(200); }
    else if (elem == "comments") { $("#" + source).fadeIn(200); }
    else if (elem == "blogs") { $("#" + source).fadeIn(200); }
    else if (elem == "multiple") { $("#" + source).fadeIn(200); }
    else if (elem == "orders") { $("#" + source).fadeIn(200); }
    else if (elem == "reports") { $("#" + source).fadeIn(200); }
    else if (elem == "photos") { $("#" + source).fadeIn(200); }
    else if (elem == "mails") { $("#" + source).fadeIn(200); }
    else if (elem == "bulk") { $("#" + source).fadeIn(200); }
    else if (elem == "stat") { $("#" + source).fadeIn(200); }
    else if (elem == "bakery") { $("#" + source).fadeIn(200); }

    $(".profile-management-header label").removeClass("profile-management-header-active");
    $(that).addClass("profile-management-header-active");
}

function sendDailyMails() {
    $("#sendDailyMails-btn").slideUp(300);
    $("#sendDailyMails-loading-div").slideDown(300);
    $.ajax({
        url: "/Account/DailyMail",
        type: 'GET',
        cache: false,
        success: function (result) {
            $("#sendDailyMails-loading-div").slideUp(300);
            $("#sendDailyMails-ok").slideDown(300);
        }
    });
}

var categoryCounter = 1;

function appendAdminCategory() {
    if ($("#admin-object-category").val() == null) { $("#admin-object-category").css("border-color", "red"); }
    else {
        if (categoryCounter < 6) {
            var elem = "<div style='width:100%; float:left;'><div style='margin-left:77px; float:left; width:148px; border-bottom:1px solid #e6e6e6;'><input value='" + $("#admin-object-category").val() + "' type='text' name='adminCategory" + categoryCounter + "' readonly style='float:left; border:none; margin-bottom:0px; width:110px;' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); categoryCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div></div>";
            categoryCounter++;
            $("#admin-category-append").append(elem);
        }
    }
}

function appendAdminObjectCategories(elem) {
    var type = $(elem).val();
    var cats;

    if (type == "cake") { cats = ["Geometrik", "Sayılar", "Özel", "Hobi"]; }
    else if (type == "object") { cats = ["Hobi", "Doğa", "Kutlama", "Bebek", "Çocuk", "Çizgi Film", "Yiyecek", "Meslek", "İnsan", "Diğer"]; }
    else if (type == "object2D") { cats = ["Hobi", "Doğa", "Kutlama", "Bebek", "Çocuk", "Çizgi Film", "Yiyecek", "Meslek", "İnsan", "Diğer"]; }

    $("#admin-object-category,#admin-category-append").empty();
    categoryCounter = 1;
    $("#admin-object-category").css("border-color", "#cccccc");

    for (i = 0; i < cats.length; i++) {
        $("#admin-object-category").append("<option value=" + cats[i] + ">" + cats[i] + "</option>");
    }
}

/****************** Payment Functions ******************/

function loadPayment() {
    $("#loading-label").text(" Satın alma işlemi gerçekleştiriliyor... ");
    $("#loading-img").attr("src", window.location.protocol + "//" + window.location.host + "/Images/Site/loadmore.gif");
    $(".loading").fadeIn(200);
}

var subedeOdeme = false; var paymentType = "";

function togglePos(what, type) {
    if (what == 'close') { $("#posMain").slideUp(300); }
    else if (what == 'open') { $("#posMain").slideDown(300); $("#kapida_radio").prop("checked", false); $("#otherPaymentOptions").slideUp(300); }
    SelectPayment(type);
    paymentType = type;
    if (type == '1') { $("#checkOutBtn").attr("onclick", "validatePaymentPos()"); } else { $("#checkOutBtn").attr("onclick", "validatePaymentDirect()"); }
    if (type == '2') {
        subedeOdeme = true;
        //$("#faturaDiv").slideUp(200);
    }
    else {
        subedeOdeme = false;
        //$("#faturaDiv").slideDown(200);
    }
}

function SelectPayment(type) {
    $.ajax({
        url: "/Order/SelectPayment?type=" + type,
        type: 'GET',
        cache: false,
        success: function (result) {
        }
    });
}

function checkKapidaOdeme() {
    if ($('#otherPaymentOptions').is(":visible")) { $("#pos_radio").prop("checked", true); $("#posMain").slideDown(200); } else { $("#posMain").slideUp(200); }
    $('#otherPaymentOptions').slideToggle(200);
}

var paymentBank = "";

function paymentFocus(what, name) {
    var elem = "";
    if (name != undefined) { elem = $("#" + name).val(); }

    if ($(what).val().length > 0) { $(what).css("border-color", "#d6d6d6"); }

    if (name == 'cardNum1' & elem.length == 4) { $('#cardNum2').focus(); }
    else if (name == "cardNum2" & elem.length == 4) { $("#cardNum3").focus(); }
    else if (name == "cardNum3" & elem.length == 4) { $("#cardNum4").focus(); }
    else if (name == "contactName") { $(".alici_isim_soyisim").text($("#contactName").val() + " " + $("#contactSurname").val()); }
    else if (name == "contactSurname") { $(".alici_isim_soyisim").text($("#contactName").val() + " " + $("#contactSurname").val()); }
    else if (name == "contactTel") { $(".alici_telefon").text($("#contactTel").val()); }
    else if (name == "contactMail") { $(".alici_email").text($("#contactMail").val()); }
    else if (name == "cardYear") { $("#cardCvc").focus(); }

    if ($("#cardNum1").val().length == 4 & $("#cardNum2").val().length >= 2 & paymentBank == "") { detectBank(); }
    if ($("#contactName").val().length > 0 & $("#contactSurname").val().length > 0 & $("#contactTel").val().length > 0 & $("#contactMail").val().length > 0) { $("#faturaAdresSameDiv").removeClass("disabled-link2"); writeFatura(); }
    else { if (paymentType != "2") { $("#faturaAdresSameDiv").addClass("disabled-link2"); $("#faturaAdresSame").attr("checked", false); $("#faturaDiv").slideDown(300); writeFatura(); } }
}

function detectBank() {
    for (i = 0; i < num.length; i++) {
        var temp = num[i].trim().split(/\s+/);
        var cardNum = temp[0];
        var enteredNum = $("#cardNum1").val().toString() + $("#cardNum2").val().toString().substr(0, 2);
        if (cardNum == enteredNum) { paymentBank = temp[2]; break; }
    }
    $(".paymentBank").text(paymentBank);
}

function faturaDetect(what) {
    if (what == 'bireysel') {
        $("#faturaName").attr("placeholder", "Fatura İsim-Soyisim");
        $("#faturaID,#faturaDaire").slideUp(200);
        $("#faturaID,#faturaDaire").css("border-color", "#cccccc");
    }
    else if (what == 'kurumsal') {
        $("#faturaName").attr("placeholder", "Şirket İsmi");
        $("#faturaID,#faturaDaire").slideDown(200);
    }

    $(".faturaInput").removeClass("fatura_disabled");

    $($("#bireyselFatura").parent().find("label")).css("color", "black");
    $($("#kurumsalFatura").parent().find("label")).css("color", "black");
}

function writeFatura() {
    var content = "";
    var faturaName = ""; var faturaAdres = ""; var faturaID = ""; faturaDaire = "";
    if ($("#faturaAdresSame").length == 0 | $("#faturaAdresSame").is(":checked")) {
        if ($("#faturaAdresSame").length == 0) {
            faturaName = $("#faturaName").val();
            faturaAdres = $("#faturaAdres").val();
            if ($("#bireyselFatura").is(":checked")) { /*do nothing*/ }
            else if ($("#kurumsalFatura").is(":checked")) { faturaDaire = $("#faturaDaire").val(); faturaID = $("#faturaID").val(); }
        }
        else {
            console.log(paymentType);
            if (paymentType != "3") { faturaName = $("#contactName").val() + " " + $("contactSurname").val(); }
            else { faturaName = $("#faturaName").val(); }

            faturaAdres = $("#faturaAppend").html();
            faturaID = ""; faturaDaire = "";
        }
    }
    else {
        faturaName = $("#faturaName").val();
        faturaAdres = $("#faturaAdres").val();
        if ($("#bireyselFatura").is(":checked")) { /*do nothing*/ }
        else if ($("#kurumsalFatura").is(":checked")) { faturaDaire = $("#faturaDaire").val(); faturaID = $("#faturaID").val(); }
    }

    content = '<br /><label style="width:100%; float:left; padding-bottom:5px;">' + faturaName + '</label>' + faturaAdres;

    if (faturaID != "") { content = content + '<label style="width:100%; float:left; padding-bottom:5px;">' + faturaID + '</label>'; }
    if (faturaDaire != "") { content = content + '<label style="width:100%; float:left; padding-bottom:5px;">' + faturaDaire + '</label>'; }

    content = content + "<br />";

    $(".faturaBilgileri").empty();
    $(".faturaBilgileri").append(content);
}

function exportPdf(norefresh) {
    var params = { html: $("#sozlesme_main_wrapper").html().toString() };
    $.ajax({
        url: "/Order/Html2Pdf",
        data: JSON.stringify(params),
        contentType: "application/json; charset=utf-8",
        type: 'POST',
        dataType: "json",
        success: function (result) {
            if (norefresh == undefined) { window.open(location.origin + "/Bills/" + result); }
        }
    });
}

function selectSecondBakeryHour(elem) {
    var newVal = parseInt($(elem).val().replace(':00', ''));
    if (newVal > 24) { newVal = newVal - 24; }

    $('#bakerySecondHour').text(' - ' + (parseInt($(elem).val().replace(':00', '')) + 1).toString() + ':00 arası');
}

function arrangeHourFromDate(elem, isDesignExist) {
    var dateFirst = new Date($(elem).datepicker("getDate"));
    var dateSecond = new Date();

    if (isDesignExist == "yes") { dateSecond.setDate(dateSecond.getDate() + 1); }

    var secondHour;

    if (dateFirst > dateSecond) {
        $("#deliveryTimePicker").hide(); $("#deliveryTimePickerClean").show();
        $("#selectedHour").val("clean");
        secondHour = $("#deliveryTimePickerClean").val();
    }
    else {
        $("#deliveryTimePickerClean").hide(); $("#deliveryTimePicker").show();
        $("#selectedHour").val("normal");
        secondHour = $("#deliveryTimePicker").val();
    }

    secondHour = parseInt(parseInt(secondHour.replace(":00", "")) + 1) + ":00";
    if (secondHour.indexOf("25") > -1) { secondHour.replace("25", "00"); }

    $("#bakerySecondHour").text(" - " + secondHour + " arası");
}

function carryHiddens() {
    if ($("#bakerySecondHour").length != 0) {
        $("#bakerySecondHourCopy").val($("#bakerySecondHour").text());
    }
}

function promotionCode(elem) {
    var val = $(elem).val();
    if (val.length >= 6) {
        $.ajax({
            url: "/Order/MakeDiscount?promotionCode=" + val,
            type: 'GET',
            cache: false,
            success: function (result) {
                $("#toplamAppend").empty();
                if (result.isOk == true) {
                    $("#promotionUnsuccessful,#promotionTying,#toplamOld").hide();
                    $("#indirimLabel").text(result.indirimTutari + " TL"); $("#odenecekTutar").text(result.newCartPrice + " TL");
                    $("#toplamAppend").append(result.append);
                    $("#promotionSuccessful,#indirimTutarı,#toplamAppend").slideDown(200);
                }
                else {
                    $("#promotionSuccessful,#promotionTying,#indirimTutarı,#toplamAppend").hide();
                    $("#indirimLabel").text(""); $("#odenecekTutar").text($("#sepetTutari").text());
                    $("#promotionUnsuccessful,#toplamOld").slideDown(200);
                }
            }
        });
    }
    else {
        $("#toplamAppend").empty();
        $("#indirimLabel").text(""); $("#odenecekTutar").text($("#sepetTutari").text());
        $("#toplamOld").show();
        $("#promotionSuccessful,#indirimTutarı,#toplamAppend").hide();
    }
}

/****************** Open/Close/Toggle/Append/Sort Functions ******************/

var sizeCounter = 1; var gramCounter = 1; var partyCounter = 1;

function appendCakeSize() {
    if (sizeCounter < 6) {
        var div = "<div><input readonly name='cakeSize" + sizeCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + $("#cakeSize-1").val() + "-" + $("#cakeSize-2").val() + " " + $("#bakery_product_price").val() + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); sizeCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div>";
        $("#cakeSizeAppend").append(div);
        sizeCounter++;
    }
}

function appendPartySize() {
    var partySizeValidated = true;
    if ($("#bakery_product_description").val().length == 0) { partySizeValidated = false; $("#bakery_product_description").css("border-color", "red"); }
    if ($("#bakery_product_price").val().length == 0) { partySizeValidated = false; $("#bakery_product_price").css("border-color", "red"); }

    if (partySizeValidated == true) {
        if (partyCounter < 6) {
            var desc = $('#bakery_product_description').val();
            var div = "<div style='margin-bottom:5px; width:100%; float:left;'><input readonly name='partySize" + partyCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + $("#partySize").val() + " kişilik " + $("#bakery_product_price").val() + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); sizeCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /><textarea readonly name='partyDescription" + partyCounter + "' style='width:100%; float:left; padding:5px; border:1px solid #cccccc; border-radius:3px; resize:none;'>" + desc + "</textarea></div>";
            $("#partySizeAppend").append(div);
            partyCounter++;
        }
    }
}

function appendGram() {
    if (gramCounter < 6) {
        var div = "<div><input readonly name='gram" + gramCounter + "' type='text' style='font-size:14px; float:left; font-family:Open Sans !important; width:120px;' value='" + $("#bakery_product_gram").val() + " " + $("#bakery_product_price").val() + "' /><img src='Images/Site/close_icon.png' onclick='$(this).parent().remove(); gramCounter--;' style='width:10px; margin-top:8px; float:right; cursor:pointer;' /></div>";
        $("#gramAppend").append(div);
        gramCounter++;
    }
}

function toggleFullScreen(elem) {
    // ## The below if statement seems to work better ## if ((document.fullScreenElement && document.fullScreenElement !== null) || (document.msfullscreenElement && document.msfullscreenElement !== null) || (!document.mozFullScreen && !document.webkitIsFullScreen)) {
    if ((document.fullScreenElement !== undefined && document.fullScreenElement === null) || (document.msFullscreenElement !== undefined && document.msFullscreenElement === null) || (document.mozFullScreen !== undefined && !document.mozFullScreen) || (document.webkitIsFullScreen !== undefined && !document.webkitIsFullScreen)) {
        if (elem.requestFullScreen) {
            elem.requestFullScreen();
        } else if (elem.mozRequestFullScreen) {
            elem.mozRequestFullScreen();
        } else if (elem.webkitRequestFullScreen) {
            elem.webkitRequestFullScreen(Element.ALLOW_KEYBOARD_INPUT);
        } else if (elem.msRequestFullscreen) {
            elem.msRequestFullscreen();
        }
    } else {
        if (document.cancelFullScreen) {
            document.cancelFullScreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitCancelFullScreen) {
            document.webkitCancelFullScreen();
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        }
    }
}

function openInfoDivs(element) {
    var div = $(element).parent().attr("id");
    var type = $(element).parent().attr("change");
    if ($("#" + div + " input").length > 0 & div.indexOf("password") < 0) { $("#" + div + " input").val($("#" + div + " label").text()); }
    else if (div.indexOf("neighborhood") == 0) {
        $("#province-change-div label,#district-change-div label").hide();
        $("#province-change-div .profile-info-edit-div,#district-change-div .profile-info-edit-div").fadeIn();
    }

    $("#" + div + " img," + "#" + div + " label").hide(); $("#" + div + " .profile-info-edit-div").fadeIn(200);
}

function closeInfoDivs(element) {
    if ($(element).attr("id") == "location-cancel") {
        $("#neighborhood-change-div .profile-info-edit-div,#district-change-div .profile-info-edit-div,#province-change-div .profile-info-edit-div").hide();
        $("#neighborhood-change-div img,#neighborhood-change-div label,#district-change-div img,#district-change-div label,#province-change-div img,#province-change-div label").fadeIn(200);
    }
    if ($(element).attr("id") == "password-cancel") {
        $("#password-change-div .profile-info-edit-div").hide();
        $("#password-change-div img,#password-change-div label").fadeIn(200);
    }
    else {
        var div = $(element).parent().parent().attr("id");
        $("#" + div + " .profile-info-edit-div").hide();
        $("#" + div + " img," + "#" + div + " label").fadeIn(200);
    }
}

function toggleMobileFilter(div, elem) {
    if ($(div).is(":visible")) { $(elem).text("Filtrele"); }
    else { $(elem).text("Tamam"); }
    $(div).slideToggle(300);
}

function toggleProfileDivs(source, elem) {
    $("#design-wrapper,#blogs-wrapper,#favs-wrapper").hide();

    if (elem == "profile") { $("#info-wrapper,#order-wrapper,#design-wrapper,#follow-wrapper,#address-wrapper").hide(); $("#profile-right-wrapper").fadeIn(200); }
    else if (elem == "info") { $("#profile-right-wrapper,#order-wrapper,#design-wrapper,#follow-wrapper,#address-wrapper").hide(); $("#info-wrapper").fadeIn(200); }
    else if (elem == "order") { $("#info-wrapper,#profile-right-wrapper,#design-wrapper,#follow-wrapper,#address-wrapper").hide(); $("#order-wrapper").fadeIn(200); }
    else if (elem == "follow") { $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#design_wrapper,#address-wrapper").hide(); $("#follow-wrapper").fadeIn(200); }
    else if (elem == "address") { $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#design_wrapper,#follow-wrapper").hide(); $("#address-wrapper").fadeIn(200); }

    $(".profile-management-header label").removeClass("profile-management-header-active"); $("#" + source).addClass("profile-management-header-active");
}

function toggleBakeryDivs(source, elem, that) {
    $(".bakery-menu-div").hide();
    if (elem == "market") { $("#" + source).fadeIn(200); }
    else if (elem == "order") { $("#" + source).fadeIn(200); $(window).resize(); }
    else if (elem == "product") { $("#" + source).fadeIn(200); }
    else if (elem == "object") { $("#" + source).fadeIn(200); }
    else if (elem == "info") { $("#" + source).fadeIn(200); }
    else if (elem == "message") { $("#" + source).fadeIn(200); }
    else if (elem == "content") { $("#" + source).fadeIn(200); }
    else if (elem == "network") { $("#" + source).fadeIn(200); }
    else if (elem == "report") { $("#" + source).fadeIn(200); }
}

function sortBakeries(source) {
    var divList;
    if ($("#pastane-div-wrapper-list").is(":visible")) { divList = $("#pastane-div-wrapper-list .pastane-link"); }
    else { divList = $("#pastane-div-wrapper-grid .pastane-link"); }

    divList.sort(function (a, b) {
        if (source == "populerite") { return $(b).attr("data-populerite") - $(a).attr("data-populerite"); }
        if (source == "lezzet") { return $(b).attr("data-lezzet") - $(a).attr("data-lezzet"); }
        if (source == "uygulama") { return $(b).attr("data-uygulama") - $(a).attr("data-uygulama"); }
    });
    if ($("#pastane-div-wrapper-list").is(":visible")) { $("#pastane-div-wrapper-list").html(divList); }
    else { $("#pastane-div-wrapper-grid").html(divList); }
}

function sortDesigns(elem) {
    var divList = $(".profile-design-div");
    source = $(elem).val();
    divList.sort(function (a, b) {
        if (source == "Date desc") { return $(b).attr("data-date") - $(a).attr("data-date"); }
        else if (source == "Date asc") { return $(a).attr("data-date") - $(b).attr("data-date"); }
    });
    $("#design-wrapper-sub").append(divList);
}

function appendDistricts(element, source) {
    $.ajax({
        url: "/Order/GetDistricts?provinceId=" + source,
        type: 'GET',
        cache: false,
        success: function (result) {
            $(element).blur().empty();
            for (i = 0; i < result.length; i++) { $(element).append(result[i]); }
        }
    });
}

function appendNeighborhoods(element, source, check) {
    $.ajax({
        url: "/Order/GetNeighborhoods?districtId=" + source,
        type: 'GET',
        cache: false,
        success: function (result) {
            $(element).blur().empty();
            for (i = 0; i < result.length; i++) { $(element).append(result[i]); }
            if (element == "#anonNeighborhoodAddress") {
                console.log(check);
                if (check == "True") { appendAddressMinPacket(); }
            }
        }
    });
}

function appendAddressMinPacket() {
    $.ajax({
        url: "/Order/GetAddressMinPacket?provinceId=" + $("#anonProvinceAddress").val() + "&districtId=" + $("#anonDistrictAddress").val(),
        type: 'GET',
        cache: false,
        success: function (result) {
            $("#minPacketAddress").text("");
            $("#minPacketAddress").css("color", result.color);
            $("#minPacketAddress").text(result.message);
            if (result.isOk == false) {
                $("#addAnonAddressButton").addClass("disabled-link");
            }
            else { $("#addAnonAddressButton").removeClass("disabled-link"); }
        }
    });
}

function appendBakeryDistricts(element, source) {
    $.ajax({
        url: "/Order/GetBakeryDistricts?provinceId=" + source,
        type: 'GET',
        cache: false,
        success: function (result) {
            $(element).blur().empty();
            for (i = 0; i < result.length; i++) { $(element).append(result[i]); }
        }
    });
}

function appendBakeryTowns(element, source) {
    $.ajax({
        url: "/Order/GetBakeryTowns?districtId=" + source,
        type: 'GET',
        cache: false,
        success: function (result) {
            if (result != true) {
                $(element).blur().empty();
                for (i = 0; i < result.length; i++) { $(element).append(result[i]); }
            }
        }
    });
}

function openProfile(elem) {
    $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#follow-wrapper,#blogs-wrapper,#favs-wrapper,#design-wrapper,#address-wrapper").hide(); $("#profile-right-wrapper").fadeIn(200);
    $(".profileSelect").css("background", "#f2f2f4"); $(elem).parent().css("background", "#dddddd");
}

function openDesigns(elem) {
    $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#follow-wrapper,#blogs-wrapper,#favs-wrapper,#profile-right-wrapper,#address-wrapper").hide(); $("#design-wrapper").fadeIn(200);
    $(".profileSelect").css("background", "#f2f2f4"); $(elem).parent().css("background", "#dddddd");
}

function openFavs(elem) {
    $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#follow-wrapper,#design-wrapper,#blogs-wrapper,#profile-right-wrapper,#address-wrapper").hide(); $("#favs-wrapper").fadeIn(200);
    $(".profileSelect").css("background", "#f2f2f4"); $(elem).parent().css("background", "#dddddd");
}

function openBlogs(elem) {
    $("#info-wrapper,#profile-right-wrapper,#order-wrapper,#follow-wrapper,#design-wrapper,#favs-wrapper,#profile-right-wrapper,#address-wrapper").hide(); $("#blogs-wrapper").fadeIn(200);
    $(".profileSelect").css("background", "#f2f2f4"); $(elem).parent().css("background", "#dddddd");
}

function toggleSelectedBakery(elem) {
    $(".bakeryInfoDiv").hide();
    $(elem).parent().find(".bakeryInfoDiv").fadeToggle(500);
}

function faturaToggle(elem) {
    if ($(elem).is(":checked") == false) { $("#faturaDiv").slideDown(300); }
    else {
        $("#faturaDiv").slideUp(300); writeFatura();
        setTimeout(function () { $(".faturaInput").val(""); }, 310);
    }
}

function toggleTopSellersNewest(what) {
    if (what == 'topSellers') {
        $("#newestSliderWrapper").fadeOut(300);
        setTimeout(function () { $("#topSellersSliderWrapper").fadeIn(); }, 310);
    }
    else {
        $("#topSellersSliderWrapper").fadeOut(300);
        setTimeout(function () { $("#newestSliderWrapper").fadeIn(); }, 310);
    }
}

function toggleCommentDivs(what) {
    if (what == 'product') {
        $("#admin-design-comment-div").fadeOut(200);
        setTimeout(function () { $("#admin-product-comment-div").fadeIn(200); }, 210);
        $("#admin-product-comment-label,#admin-design-comment-label").toggleClass("admin-comment-active");
    }
    else if (what == 'design') {
        $("#admin-product-comment-div").fadeOut(200);
        setTimeout(function () { $("#admin-design-comment-div").fadeIn(200); }, 210);
        $("#admin-product-comment-label,#admin-design-comment-label").toggleClass("admin-comment-active");
    }
}

function openProfileOrderDetail() {
    $("#profile-order-pop").fadeToggle(300);
}

function openBigPic(elem) {
    $('#orderSub-bigPic-mainImg').attr('src', $(elem).attr('src')); $('#orderSub-bigPic').fadeIn(300);
}

var chld = 1;

function changeSellers(num) {
    if (chld != num) {
        $(".index-bakery-topsellers-wrapper").fadeOut(250);
        setTimeout(function () { $(".index-bakery-topsellers-wrapper[data-idx=" + num + "]").fadeIn(250); }, 270);
        chld = num;
    }
}

function bakeryObjClose(elem) {
    $("#bakery-object-name").text("");
    $("#bakery-object-category").val("object");
    $("#bakery-object-keywords,#bakery-object-price").val("");
    $("#bakery-object-form").attr("action", "/Bakery/AddBakeryObject");
    $("#bakery-object-edit-submit").hide(); $("#bakery-object-submit").show();
    $("#bakery-object-close").fadeOut(200);

    $("#object-image-display").attr("src", "");
    $("#object-texture-display").attr("src", "");

    resetFormElement($("#objectFile")); resetFormElement($("#objectImage")); resetFormElement($("#objectTexture"));
}

function changeBakeryView(elem) {
    if (elem == "list") { $(".pastaneler-grid").hide(); $(".pastaneler-list").fadeIn(300); }
    if (elem == "grid") { $(".pastaneler-list").hide(); $(".pastaneler-grid").fadeIn(300); }
}

/****************** Add/Edit/Delete/Save/Update/Clear/Set/Change Functions ******************/

function cancelAdd() {
    $('#cart-warning-dialog,#designsConflict,#productsConflict,#designProductConflict,#productDesignConflict,#deleteBakeryConflict,.loading').fadeOut(300); $(this).parent().children('.black-gradient').removeAttr('onclick');
}

function addDeleteLoading(isAdd) {
    var label = "";
    if (isAdd) { label = "Added to cart."; } else { label = "Remove from cart."; }
    $("#loading-label").text(label);
    $("#loading-img").attr("src", window.location.protocol + "//" + window.location.host + "/Images/Site/tick_icon2.png");
    $(".loading,.loading-child").fadeIn(300);
    setTimeout(function () { $(".loading").fadeOut(300); }, 600);
    setTimeout(function () { $("#loading-label").text(""); $("#loading-img").attr("src", window.location.protocol + "//" + window.location.host + "/Images/Site/loadmore.gif"); }, 950);
}

function updateProductMainImage(elem) {
    if ($("#product-main-image").attr("src") != $(elem).attr("src")) {
        $("#product-main-image").fadeOut(300, function () {
            $("#product-main-image").attr('src', $(elem).attr("src"));
            $("#product-main-image").fadeIn(300);
        });
    }
}

function updateDesignMainImage(elem) {
    if ($("#designMainImage").attr("src") != $(elem).attr("src")) {
        $("#designMainImage").fadeOut(200, function () {
            $("#designMainImage").attr('src', $(elem).attr("src"));
            $("#designMainImage").fadeIn(200);
        });
    }
}

function bakeryObjEdit(elem) {
    $("#bakery-object-name").text($(elem).attr("data-objname"));

    $("#bakery-object-category").val($(elem).attr("data-objtype"));
    $("#bakery-object-price").val($(elem).attr("data-objprice") + " TL");
    $("#bakery-object-keywords").val($(elem).attr("data-objkeywords"));

    $("#bakery-object-color").val($(elem).attr("data-objcolor")).css("background", "#" + $(elem).attr("data-objcolor"));
    $("#bakery-object-form").attr("action", "/Bakery/EditBakeryObject?ObjectID=" + $(elem).attr("data-objid"));
    $("#bakery-object-submit").hide(); $("#bakery-object-edit-submit").show();
    $("#bakery-object-close").fadeIn(200);
    var parent = $(elem).parent().parent();

    $("#object-image-display").attr("src", parent.children("td").children(".img-popup").attr("src"));
    $("#object-texture-display").attr("src", parent.children("td").children(".texture-popup").attr("src"));

    resetFormElement($("#objectFile")); resetFormElement($("#objectImage")); resetFormElement($("#objectTexture"));
}

function bakeryObjDelete(elem) {
    var name = $(elem).attr("data-objname");
    if (confirm(name + " isimli obje silinecektir. Onaylıyor musunuz?")) {
        window.location = "/Bakery/DeleteBakeryObject?ObjectID=" + $(elem).attr("data-objid");
    }
    else {
        event.preventDefault();
    }
}

function deleteAddress() {
    if (!confirm("Seçtiğiniz adres silinecektir. Onaylıyor musunuz?")) { event.preventDefault(); }
}

function editAddress(elem) {
    $("#edit-address-div").fadeToggle(300);
    $("#editOpenAddress").text($(elem).attr("data-openAddress"));
    $("#editAddressDescription").text($(elem).attr("data-description"));
    $("#AddressId").val($(elem).attr("data-addressId"));
}

function clearBakeryPrice(type) {
    if (type == "cake") {
        $('#cakePriceDialog').fadeOut(300);
        $('#addCakePriceForm').find('input[name=what]').val('add');
        $("#addCakePriceForm").find("input[name=size]").val(0);
        $("#addCakePriceForm").find("input[name=price]").val("");
    }
    else if (type == "cupcake") {
        $('#cupcakePriceDialog').fadeOut(300);
        $('#addCupcakePriceForm').find('input[name=what]').val('add');
        $("#addCupcakePriceForm").find("input[name=size]").val(0);
        $("#addCupcakePriceForm").find("input[name=price]").val("");
    }
    else if (type == "cookie") {
        $('#cookiePriceDialog').fadeOut(300);
        $('#addCookiePriceForm').find('input[name=what]').val('add');
        $("#addCookiePriceForm").find("input[name=size]").val(0);
        $("#addCookiePriceForm").find("input[name=price]").val("");
    }
}

function editBakeryPrice(number, price, type, ID) {
    $("#cakePriceDialog,#cupcakePriceDialog,#cookiePriceDialog").hide();

    if (type == "cake") {
        $("#cakePriceDialog").fadeIn(300);
        $("#addCakePriceForm").find("input[name=size]").val(number);
        $("#addCakePriceForm").find("input[name=price]").val(price);
        $("#addCakePriceForm").find("input[name=what]").val("edit");
        $("#addCakePriceForm").find("input[name=ID]").val(ID);
    }
    else if (type == "cupcake") {
        $("#cupcakePriceDialog").fadeIn(300);
        $("#addCupcakePriceForm").find("input[name=size]").val(number);
        $("#addCupcakePriceForm").find("input[name=price]").val(price);
        $("#addCupcakePriceForm").find("input[name=what]").val("edit");
        $("#addCupcakePriceForm").find("input[name=ID]").val(ID);
    }
    else if (type == "cookie") {
        $("#cookiePriceDialog").fadeIn(300);
        $("#addCookiePriceForm").find("input[name=size]").val(number);
        $("#addCookiePriceForm").find("input[name=price]").val(price);
        $("#addCookiePriceForm").find("input[name=what]").val("edit");
        $("#addCookiePriceForm").find("input[name=ID]").val(ID);
    }
}

function clearBakeryContent(type) {
    if (type == "cake") {
        $('#cakeContentDialog').fadeOut(300);
        $('#addCakeContentForm').find('input[name=what]').val('add');
        $("#addCakeContentForm").find("input[name=content]").val("");
    }
    else if (type == "cupcake") {
        $('#cupcakeContentDialog').fadeOut(300);
        $('#addCupcakeContentForm').find('input[name=what]').val('add');
        $("#addCupcakeContentForm").find("input[name=content]").val("");
    }
    else if (type == "cookie") {
        $('#cookieContentDialog').fadeOut(300);
        $('#addCookieContentForm').find('input[name=what]').val('add');
        $("#addCookieContentForm").find("input[name=content]").val("");
    }
}

function editBakeryContent(content, type, ID) {
    $("#cakeContentDialog,#cupcakeContentDialog,#cookieContentDialog").hide();

    if (type == "cake") {
        $("#cakeContentDialog").fadeIn(300);
        $("#addCakeContentForm").find("input[name=content]").val(content);
        $("#addCakeContentForm").find("input[name=what]").val("edit");
        $("#addCakeContentForm").find("input[name=ID]").val(ID);
    }
    else if (type == "cupcake") {
        $("#cupcakeContentDialog").fadeIn(300);
        $("#addCupcakeContentForm").find("input[name=content]").val(content);
        $("#addCupcakeContentForm").find("input[name=what]").val("edit");
        $("#addCupcakeContentForm").find("input[name=ID]").val(ID);
    }
    else if (type == "cookie") {
        $("#cookieContentDialog").fadeIn(300);
        $("#addCookieContentForm").find("input[name=content]").val(content);
        $("#addCookieContentForm").find("input[name=what]").val("edit");
        $("#addCookieContentForm").find("input[name=ID]").val(ID);
    }
}

function confirmDeleteDesign() {
    if (!confirm("Seçtiğiniz dizayn silinecektir. Onaylıyor musunuz?")) { event.preventDefault(); }
}

function saveChanges(event, element) {
    var div = $(element).parent().parent().attr("id");
    if (div.indexOf("password") == 0) {
        var passValidated = true;
        if ($("#new-pass").val().length == 0) { $("#new-pass").css("border-color", "red"); event.preventDefault(); }
        if ($("#old-pass").val().length == 0) { $("#old-pass").css("border-color", "red"); event.preventDefault(); }
        if ($("#new-pass-confirm").val().length == 0) { $("#new-pass-confirm").css("border-color", "red"); event.preventDefault(); }
        if ($("#new-pass-confirm").val() != $("#new-pass").val()) { $("#new-pass-confirm,#newpass").css("border-color", "red"); event.preventDefault(); }
        if ($("#new-pass").val().length == 0 & $("#old-pass").val().length == 0 & $("#new-pass-confirm").val().length == 0 & $("#new-pass-confirm").val() != $("#new-pass").val()) { passValidated = false; } else { passValidated = true; }

        if (passValidated == true) {
            $.ajax({
                url: "/Account/ChangePassword?oldPass=" + $("#old-pass").val() + "&newPass=" + $("#new-pass").val(),
                type: 'GET',
                cache: false,
                success: function (result) {
                    location.reload();
                }
            });
        }
    }
    else {
        var profileInfoValidated = false;

        if ($("#" + div + " input").val().length == 0) { $("#" + div + " input").css("border-color", "red"); event.preventDefault(); profileInfoValidated = false; }
        else { profileInfoValidated = true; }

        if (profileInfoValidated == false) { }
        else {
            if (div.indexOf("email") == 0) {
                var password = prompt("Lütfen şifrenizi giriniz.", "");
                if (password.length > 0) {
                    $.ajax({
                        url: "/Account/ChangeEmail?email=" + $("#" + div + " input").val() + "&password=" + password,
                        type: 'GET',
                        cache: false,
                        success: function (result) {
                            location.reload();
                        }
                    });
                }
                else { $("#" + div + " input").css("border-color", "red"); event.preventDefault(); }
            }
            else {
                var div = $(element).parent().parent().attr("id");
                var data;
                if ($("#" + div + " input").length > 0) { data = $("#" + div + " input").val(); } else { data = $("#" + div + " select").val(); }

                $.ajax({
                    url: "/Account/SaveChanges?parent=" + div + "&data=" + data,
                    type: 'GET',
                    cache: false,
                    success: function (result) {
                        location.reload();
                    }
                });
            }
        }
    }
}

function setDesignVisibility(elem) {
    var id = $(elem).attr("data-id");
    $.ajax({
        url: "/Account/SetDesignVisibility?id=" + id + "&isVisible=" + $(elem).is(":checked"),
        type: 'GET',
        cache: false,
        success: function (result) {
        }
    });
}

function changeOrderStatus(elem, orderID) {
    if (confirm("Siparişin durumu " + $(elem).val() + " olarak değiştirilecektir. Onaylıyor musunuz?")) {
        $.ajax({
            url: "/Bakery/ChangeOrderStatus?status=" + $(elem).val() + "&orderID=" + orderID,
            type: 'GET',
            cache: false,
            success: function (result) {
            }
        });
    }
}

/****************** General Functions ******************/

function isMobile() {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        return true;
    }
    else { return false; }
}

function resetFormElement(e) {
    e.wrap('<form>').parent('form').trigger('reset');
    e.unwrap();
}

jQuery.fn.center = function () {
    this.css("position", "fixed");
    this.css("top", ($(window).height() / 2) - (this.outerHeight() / 2));
    return this;
}

function changeTooltip(elem, newMessage) {
    $(elem).attr('data-original-title', newMessage);
}

function confirmAction(label) {
    if (confirm(label) == false) { event.preventDefault(); }
}

function SetMobileArrangements() {
    $(".hasdd a,.hasbp a").attr("href", "");
}

/****************** Unclassified Functions ******************/

function arrangeOtherCakeSize(size) {
    $("#cakeSize-2").val("");
    $("#cakeSize-2").removeClass("disabled-link");
    var options = $("#cakeSize-2 option");
    for (i = 0; i < options.length; i++) { if (parseInt($(options[i]).val()) <= parseInt(size)) { $(options[i]).attr("disabled", true); } else { $(options[i]).attr("disabled", false); } }
}

function approveBakeryComment(id) {
    $.ajax({
        url: "/Bakery/ApproveComment?CommentID=" + id,
        type: 'GET',
        cache: false,
        success: function (result) {
            location.reload();
        }
    });
}

function checkCart(id) {
    var res;
    $.ajax({
        url: "/Order/CheckCart?bakeryID=" + id,
        async: false,
        type: 'GET',
        cache: false,
        success: function (result) {
            res = result;
        }
    });
    return res;
}

/****************** Facebook login **********************/

//var isfblogin = false;

//function callFBLogin(response, loginType) {
//    $.ajax({
//        url: "/Account/FacebookLogin?fbID=" + response.id + "&nameSurname=" + response.name + "&email=" + response.email,
//        type: 'GET',
//        cache: false,
//        success: function (result) {
//            isfblogin = true;
//            if (result != "logon") { location.reload(); }
//        }
//    });
//}

//function facebookLogout() {
//    FB.getLoginStatus(function (ret) {
//        /// are they currently logged into Facebook?
//        if (ret.authResponse) {
//            //they were authed so do the logout
//            FB.logout(function (response) {
//                console.log(response);
//            });
//        } else {
//            ///do something if they aren't logged in
//            //or just get rid of this if you don't need to do something if they weren't logged in
//        }
//    });
//}

//function facebookLogin() {
//    console.log("facebookLogin.")
//    FB.login(function (response) {
//        if (response.authResponse) {
//            //console.log(response); // dump complete info
//            access_token = response.authResponse.accessToken; //get access token
//            user_id = response.authResponse.userID; //get FB UID

//            FB.api('/me', { fields: 'name, email' }, function (response) {
//                callFBLogin(response);
//                console.log(response);
//            });
//        } else {
//            //user hit cancel button
//            console.log('User cancelled login or did not fully authorize.');
//        }
//    }, {
//        scope: 'public_profile,email'
//    });
//}

//function addFacebookLoginLink() {
//    if ($("#fb-login-link").length < 0) {
//        $("#login-floating-div").append('<div style="border:none;"><a onclick="facebookLogin(); id="fb-login-link" return false;" href="#" class="fb_gradient">Facebook ile giriş</a></div>');
//    }

//    if ($("#fb-register-link").length < 0) {
//        $("#register-floating-div").append('<div style="border:none;"><a onclick="facebookLogin(); id="fb-register-link" return false;" href="#" class="fb_gradient">Facebook ile üye ol</a></div>');
//    }
//}

//function statusChangeCallback(response) {
//    console.log('statusChangeCallback');
//    console.log(response);
//    // The response object is returned with a status field that lets the
//    // app know the current login status of the person.
//    // Full docs on the response object can be found in the documentation
//    // for FB.getLoginStatus().
//    if (response.status === 'connected') {
//        console.log("Logged into your app and Facebook.")
//        FB.api('/me', { fields: 'name, email' },
//            function (response) {
//                console.log(response);
//            }
//        );
//    } else if (response.status === 'not_authorized') {
//        // The person is logged into Facebook, but not your app.
//        console.log("The person is logged into Facebook, but not your app.");
//        addFacebookLoginLink();
//    } else {
//        // The person is not logged into Facebook, so we're not sure if
//        // they are logged into this app or not.
//        console.log("The person is not logged into Facebook, so we're not sure if they are logged into this app or not.");
//        addFacebookLoginLink();
//    }
//}

//function checkLoginState() {
//    $.ajax({
//        url: "/Account/CheckFBLogin",
//        type: 'GET',
//        cache: false,
//        success: function (result) {
//            isfblogin = result;
//            if (result == false) {
//                FB.getLoginStatus(function (response) {
//                    statusChangeCallback(response);
//                });
//            }
//        }
//    });
//}

//ayberk

function changeImgSrc(elem, img) {
    if (img != "") { $(elem).attr('src', img); }
}

/****************** Lending Functions ******************/

/****************** Document Functions ******************/

var view = ""; var isLogin = false;

$(document).on("click", function (event) {
    //$('.collapse').collapse('hide');
    if (event.target.className.indexOf("login-register") < 0 & event.target.className.indexOf("save") < 0 & event.target.className.indexOf("social-img") < 0 & event.target.className.indexOf("order") < 0 & event.target.id.indexOf("oturum-label") < 0 & event.target.className.indexOf("tutorial") < 0 & event.target.className.indexOf("overall-mask") < 0) { $("#account-floating-div,.overall-mask").fadeOut(200); }
    if (event.target.className.indexOf("sepetim-header") < 0) { $("#sepetim-header-div").fadeOut(200); }
});

$(document).keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13' & $("#login-floating-div").is(":visible")) { $("#login-button").trigger("click"); }
    if (keycode == '13' & $("#register-floating-div").is(":visible")) { $("#register-button").trigger("click"); }
});

$(document).ready(function () {

    $("#main-carousel").owlCarousel({
        singleItem: true,
        items: 1,
        itemsTablet: [600, 1],
        itemsMobile: [479, 1],
        pagination: true,
        navigation: false,
        animateIn: 'fadeIn',
        animateOut: 'fadeOut',
        autoPlay: true
    });

    //$(".img_transition").on("mouseover", function () {
    //});

    //$(".img_transition").on("mouseout", function () {
    //    document.getElementById('img_transition')
    //    $(".img_transition").attr('src', imgs2[0]);
    //});

    $("#index-designs-slider,#index-designs-slider2").owlCarousel({
        items: 4,
        itemsTablet: [600, 3],
        itemsMobile: [479, 2],
        pagination: true,
        navigation: false
    });

    $("#index-products-slider,#index-products-slider2").owlCarousel({
        items: 4,
        itemsTablet: [600, 3],
        itemsMobile: [479, 2],
        pagination: true,
        navigation: false
    });

    $("#index-products-slider3,#index-products-slider4,#index-products-slider5,#newestSlider,#topSellersSlider").owlCarousel({
        items: 4,
        itemsTablet: [600, 4],
        itemsMobile: [479, 2],
        pagination: true,
        navigation: false
    });

    $("#tutorial-slider").owlCarousel({
        singleItem: true,
        items: 1,
        pagination: true,
        navigation: true,
        navigationText: ["<img src='/Images/Site/left.png' style='width:20px;'>", "<img src='/Images/Site/right.png' style='width:20px;'>"]
    });

    $("#youDesign-carousel").owlCarousel({
        items: 4,
        itemsTablet: [600, 2],
        itemsMobile: [479, 2]
    });

    $("#index-bakeries-carousel").owlCarousel({
        margin: 10,
        items: 6,
        itemsTablet: [600, 4],
        itemsMobile: [479, 2]
    });

    $("#login-register-div *").addClass("login-register");

    $("#sepetim-header-div *,#sepetim-header *").addClass("sepetim-header");

    $(".login-register-toggle").click(function () {
        $("#account-floating-div").fadeToggle(200);
    });

    $(".account-button").click(function (event) {
        if (event.target.className.indexOf('account-button-active') < 0) { $(".account-button").toggleClass("account-button-active"); }
        if (event.target.id == "login-menu") { $("#register-floating-div").hide(); $("#login-floating-div").fadeIn(200); }
        if (event.target.id == "register-menu") { $("#login-floating-div").hide(); $("#register-floating-div").fadeIn(200); }
    });

    $("#account-floating-div input").keyup(function (event) {
        if ($(this).val().length > 0) { $(this).parent().css("border-color", "#cccccc"); } else { $(this).parent().css("border-color", "red"); }
    });

    $("#account-floating-div select").change(function (event) {
        if ($(this).val() != null) { $(this).parent().css("border-color", "#cccccc"); } else { $(this).parent().css("border-color", "red"); }
    });

    $("#forgot-password-button").click(function () {
        $("#login-sub-div,#login-button").toggle(); $(".forgot").toggle();
    });

    $("#lower-carousel,#bakeryUploadCarousel,#fav-carousel").owlCarousel({ interval: false });

    $("#sehir_pastaneler").on("change", function () {
        appendDistricts("#ilce_pastaneler", $("#sehir_pastaneler").val()); $("#semt_pastaneler").empty();
    });

    $("#ilce_pastaneler").on("change", function () {
        appendNeighborhoods("#semt_pastaneler", $("#ilce_pastaneler").val());
    });



    $("#istekTasarimImg").change(function () {
        var files = this.files;
        $("#istekTasarimThumbnailWrapper").fadeIn(200);

        for (i = 0; i < files.length; i++) {
            var reader = new FileReader();
            reader.onload = function (event) {
                $("#istekTasarimThumbnailWrapper").append('<img alt="tasarımınızı anlatın" src="' + event.target.result + '" id="istekTasarimThumbnail" style="width:200px; margin:5px; padding:5px; border:1px solid #d6d6d6; box-shadow: 0px 1px 5px -3px grey; border-radius:3px;" />');
            }
            reader.readAsDataURL(this.files[i]);
        }
    });

    $("#istekFigurImg").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            console.log(e);
            $("#istekFigurThumbnail").attr("src", e.target.result);
            $("#istekFigurThumbnailWrapper").fadeIn(200);
        };
        reader.readAsDataURL(this.files[0]);
    });

    $("#upload_userImg").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            $(".profile-thumbnail").attr("src", e.target.result);
            $("#upload-ok").fadeIn(200);
        };
        reader.readAsDataURL(this.files[0]);
    });

    $("#profile-info-birth").datepicker({
        yearRange: "-100:+0",
        changeMonth: true,
        changeYear: true,
        dateFormat: 'dd/mm/yy',
        autoSize: true,
        dayNames: ["pazar", "pazartesi", "salı", "çarşamba", "perşembe", "cuma", "cumartesi"],
        dayNamesMin: ["pa", "pzt", "sa", "çar", "per", "cum", "cmt"],
        monthNames: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        monthNamesShort: ["Ocak", "Şubat", "Mart", "Nisan", "Mayıs", "Haziran", "Temmuz", "Ağustos", "Eylül", "Ekim", "Kasım", "Aralık"],
        nextText: "ileri",
        prevText: "geri",
    });

    $(".info-change-input-div img,.info-change-input-div label").click(function (event) { openInfoDivs(this); });
    $(".info-change-input-div .cancel-btn").click(function (event) { closeInfoDivs(this); });
    $(".info-change-input-div .ok-btn").click(function (event) { saveChanges(event, this); });

    $("#profile-info-province").change(function () {
        div = "district-change-div";
        $("#" + div + " img," + "#" + div + " label").hide();
        $("#" + div + " .profile-info-edit-div").fadeIn(200);
        appendDistricts('#profile-info-district', $(this).val())
    });
    $("#profile-info-district").change(function () {
        div = "neighborhood-change-div";
        $("#" + div + " img," + "#" + div + " label").hide();
        $("#" + div + " .profile-info-edit-div").fadeIn(200);
        appendNeighborhoods('#profile-info-neighborhood', $(this).val())
    });

    var bakeryLoginValidated = false;

    $("#bakeryLogin-btn").click(function (event) {
        event.preventDefault();
        $("#bakery-login-validation-div label").hide(); $("#bakery-loading").show();

        if ($("#bakery_username").val().length == 0) { $("#bakery_username").css("border-color", "red"); bakeryLoginValidated = false; }
        if ($("#bakery_password").val().length == 0) { $("#bakery_password").css("border-color", "red"); bakeryLoginValidated = false; }
        if ($("#bakery_password").val().length != 0 & $("#bakery_username").val().length != 0) { bakeryLoginValidated = true; }

        if (bakeryLoginValidated == false) {
            $("#bakery-login-validation-div label").text("All fields must be filled.").show();
            $("#bakery-loading").hide(0);
        }
        else { $('#bakery_login_form').trigger("submit"); }
    });

    $("#bakery_username").keyup(function () { if ($("#bakery_username").val().length > 0) { $("#bakery_username").css("border-color", "#cccccc"); } else { bakeryLoginValidated = false; $("#bakery_username").css("border-color", "red"); } });
    $("#bakery_password").keyup(function () { if ($("#bakery_password").val().length > 0) { $("#bakery_password").css("border-color", "#cccccc"); } else { bakeryLoginValidated = false; $("#bakery_password").css("border-color", "red"); } });

    $("#bakeryProductImages").change(function () {
        var files = event.target.files;

        for (var i = 0; i < files.length; i++) {
            var file = files[i];
            if (!file.type.match('image')) continue;

            var picReader = new FileReader(); var counter = 1;
            $("#bakery_product_thumbnail_div .bakery_product_thumbnail_img").attr("src", "/Images/Site/no_image.png");
            picReader.addEventListener("load", function (event) {
                if (counter < 4) {
                    var picFile = event.target;
                    $("#bakery_product_thumbnail_div .bakery_product_thumbnail_img:nth-child(" + counter + ")").attr("src", picFile.result);
                }
                counter++;
            });
            picReader.readAsDataURL(file);
        }
    });

    $("#bakery_product_submit").click(function () {
        var temp;
        var elems = $(".bakery_product_thumbnail_img").attr("src");
        for (i = 0; i < elems.length; i++) { temp = "  " + temp + elems[i]; }
        $("#bakery_product_images").val(temp);
    });

    $("#bakery-product-explanation-button").click(function () { $("#bakery-product-explanation-div").slideToggle(300); });

    $(".obj-img-thumbnail").mouseover(function () { $(this).parent().children(".img-popup").show(); });
    $(".obj-img-thumbnail").mouseout(function () { $(this).parent().children(".img-popup").hide(); });

    $(".obj-texture-thumbnail").mouseover(function () { $(this).parent().children(".texture-popup").show(); });
    $(".obj-texture-thumbnail").mouseout(function () { $(this).parent().children(".texture-popup").hide(); });

    $(".obj-color-thumbnail").mouseover(function () { $(this).parent().children(".color-popup").show(); });
    $(".obj-color-thumbnail").mouseout(function () { $(this).parent().children(".color-popup").hide(); });

    $(".obj-alpha-thumbnail").mouseover(function () { $(this).parent().children(".alpha-popup").show(); });
    $(".obj-alpha-thumbnail").mouseout(function () { $(this).parent().children(".alpha-popup").hide(); });

    $("#objectImage").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#object-image-display").attr("src", e.target.result);
        };
        reader.readAsDataURL(this.files[0]);
    });

    $("#objectTexture").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#object-texture-display").attr("src", e.target.result);
        };
        reader.readAsDataURL(this.files[0]);
    });

    $(".plus-cart-product-a").click(function (event) {
        var elem = $(this).parent().children(".CartProductCount");
        var num = parseInt($(elem).val()) + 1;
        if (num <= parseInt($(elem).attr("data-max"))) {
            $(elem).val((num).toString());
        }
        else { event.preventDefault(); }
    });

    $(".minus-cart-product-a").click(function (event) {
        var elem = $(this).parent().children(".CartProductCount");
        var num = parseInt($(elem).val()) - 1;
        if (num > 0) {
            $(elem).val((num).toString());
        }
        else { event.preventDefault(); }
    });

    $(".plus-cart").click(function () {
        var num = parseInt($("#adet").val()) + 1;
        if (num <= parseInt($("#adet").attr("data-max"))) { $("#adet").val((num).toString() + " adet"); }
        updateProduct();
    });

    $(".minus-cart").click(function () {
        var num = parseInt($("#adet").val()) - 1;
        if (num > 0) { $("#adet").val((num).toString() + " adet"); }
        updateProduct();
    });

    //$(".product-div").mouseover(function () {
    //    $(this).find(".product-price").hide();
    //    $(this).find(".obj-mask").show();
    //});

    $(".product-div").mouseleave(function () {
        $(this).find(".obj-mask").hide();
        $(this).find(".product-price").show();
    });

    $("#add-to-cart-btn").click(function () { $(".loading").fadeIn(200); $("#loading-img").attr("src", window.location.protocol + "//" + window.location.host + "/Images/Site/loadmore.gif"); $("#loading-label").text("Adding to cart... "); });

    $("input[name=Size],.plus-cart,.minus-cart").click(function () { $(".loading").fadeIn(200); $("#loading-label").text(" Lütfen Bekleyin... "); });

    $("#sepetim-header").click(function () { $("#sepetim-header-div").fadeToggle(300); });

    $("#add-comment-button").click(function () {
        if (!$("#product-comment-div").is(":visible")) { $("#add-comment-button span").text("Kapat"); } else { $("#add-comment-button span").text("Yorum Yap"); }
        $("#product-comment-div").slideToggle(200);
    });

    $("#add-comment-button-design").click(function () {
        if (!$("#design-comment-div").is(":visible")) { $("#add-comment-button-design span").text("Kapat"); } else { $("#add-comment-button-design span").text("Yorum Yap"); }
        $("#design-comment-div").slideToggle(200);
    });

    $("#add-comment").click(function () {
        $('html, body').animate({
            scrollTop: $("#product-comments-div").offset().top
        }, 1000, "easeOutQuart");
    });

    $(".remove-filters").click(function () { resetProductFilters(); });

    $('.pop-input:nth-child(4)').on('keydown', function (e) { -1 !== $.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) || /65|67|86|88/.test(e.keyCode) && (!0 === e.ctrlKey || !0 === e.metaKey) || 35 <= e.keyCode && 40 >= e.keyCode || (e.shiftKey || 48 > e.keyCode || 57 < e.keyCode) && (96 > e.keyCode || 105 < e.keyCode) && e.preventDefault() });

    $("#addBlogImg").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#addBlogImg_display").attr("src", e.target.result);
        };
        reader.readAsDataURL(this.files[0]);
    });

    $("#provinceAddress ,#districtAddress, #neighborhoodAddress, #openAddress, #addressDescription, #editProvinceAddress ,#editDistrictAddress, #editNeighborhoodAddress, #editOpenAddress, #editAddressDescription").on("change keyup", function () { if ($(this).val() != "" | $(this).val() != null) { $(this).css("border-color", "#cccccc"); } else { $(this).css("border-color", "red"); } });

    $("#addAddressForm").submit(function (e) {
        var addressValidated = true;
        var elems = $("#provinceAddress ,#districtAddress, #neighborhoodAddress, #openAddress, #addressDescription");
        for (i = 0; i < elems.length; i++) {
            if ($(elems[i]).val() == null | $(elems[i]).val() == "") { $(elems[i]).css("border-color", "red"); addressValidated = false; } else { $(elems[i]).css("border-color", "#cccccc"); }
        }
        if (addressValidated == false) { e.preventDefault(); }
    });

    $("#editAddressForm").submit(function (e) {
        var addressValidated = true;
        var elems = $("#editProvinceAddress ,#editDistrictAddress, #editNeighborhoodAddress, #editOpenAddress, #editAddressDescription");
        for (i = 0; i < elems.length; i++) {
            if ($(elems[i]).val() == null | $(elems[i]).val() == "") { $(elems[i]).css("border-color", "red"); addressValidated = false; } else { $(elems[i]).css("border-color", "#cccccc"); }
        }
        if (addressValidated == false) { e.preventDefault(); }
    });

    $("#cakePriceDialog,#cupcakePriceDialog,#cookiePriceDialog").find("input[name=size]").on("keyup change", function () {
        if ($(this).val() != 0) { $(this).css("border-color", "#cccccc"); } else { $(this).css("border-color", "red"); }
    });

    $("#cakePriceDialog,#cupcakePriceDialog,#cookiePriceDialog").find("input[name=price]").on("keyup change", function () {
        if ($(this).val().length != 0) { $(this).css("border-color", "#cccccc"); } else { $(this).css("border-color", "red"); }
    });

    $("#cakeContentDialog,#cupcakeContentDialog,#cookieContentDialog").find("input[name=content]").on("keyup", function () {
        if ($(this).val().length != 0) { $(this).css("border-color", "#cccccc"); } else { $(this).css("border-color", "red"); }
    });

    $(".bakeryInfo input[type=text], .bakeryInfo textarea").on("keyup", function () {
        if ($(this).val().length == 0) { $(this).css("border-color", "red"); } else { $(this).css("border-color", "#e6e6e6"); }
    });

    $(function () {
        $('.cardNum, .cardCvc, #OrderNumber').on('keydown', function (e) { -1 !== $.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) || /65|67|86|88/.test(e.keyCode) && (!0 === e.ctrlKey || !0 === e.metaKey) || 35 <= e.keyCode && 40 >= e.keyCode || (e.shiftKey || 48 > e.keyCode || 57 < e.keyCode) && (96 > e.keyCode || 105 < e.keyCode) && e.preventDefault() });
    });

    $('.cardName, .cardSurname').keypress(function (event) {
        var inputValue = event.which;
        if (!(inputValue >= 65 && inputValue <= 90) && !(inputValue >= 97 && inputValue <= 122) && (inputValue != 32 && inputValue != 0) && (inputValue != 304) && (inputValue != 231) && (inputValue != 305) && (inputValue != 199) && (inputValue != 214) && (inputValue != 286) && (inputValue != 246) && (inputValue != 287) && (inputValue != 220) && (inputValue != 350) && (inputValue != 252) && (inputValue != 351)) {
            event.preventDefault();
        }
    });

    $("#contactTel,#istekTasarimTel,#addBakeryTel").mask("(999) 999-99-99");

    var paymentInputs = $("#paymentForm input[type=text]");
    for (i = 0; i < paymentInputs.length; i++) {
        $(paymentInputs[i]).tooltip();
    }

    $("#sozlesmeCheck,.visibleInProfile").tooltip();

    var faturaTimeout = '';
    $('.faturaInput').keyup(function () {
        clearTimeout(faturaTimeout);
        faturaTimeout = setTimeout(function () {
            writeFatura();
        }, 500);
    });

    $("#indexMainSearch").autocomplete({
        delay: 300,
        position: { my: "right+0 top+20", at: "right bottom" },
        source: '/Home/MainAutocomplete',
        data: { term: $(this).val() },
        minLength: 3,
        open: function () { $('ul.ui-autocomplete').addClass('opened') },
        close: function () {
            $('ul.ui-autocomplete')
              .removeClass('opened')
              .css('display', 'block');
        },
        autoFocus: true
        //close: function (event, ui) {
        //    if (!$("ul.ui-autocomplete").is(":visible")) {
        //        $("ul.ui-autocomplete").show();
        //    }
        //}
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        item.value = item.name;
        item.label = item.name;
        var acItem = "<li>" +
                        "<a style='color:initial; text-decoration:none;' href='" + item.link + "'>" +
                            "<div style='width:100%; float:left; border-bottom:1px solid #e6e6e6; padding:5px 0px; font-family:Raleway; cursor:pointer;'>" +
                                "<div style='float:left; width:120px;'>" +
                                    "<img class='acImg' src='" + item.img + "' />" +
                                "</div>" +

                                "<div style='float:left; width:275px; padding-left:15px; padding-top:5px;'>" +
                                    "<label style='width:100%; float:left; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; font-size:16px; font-weight:500;'>" + item.bakeryName + " " + item.name + "</label>" +
                                    "<label style='width:100%; float:left; font-size:12px; color:darkgray; padding-top:5px;'>" + item.desc + "</label>" +
                                    "<label style='float:left; font-family:Open Sans; font-size:20px;'>" + item.price + "</label>" +
                                "</div>" +
                            "</div>" +
                       " </a>" +
                      "</li>";

        return $(acItem)
            .data("item.autocomplete", item)
            .appendTo(ul);
    };

    //checkLoginState();

    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.go-top').fadeIn(500);
        } else {
            $('.go-top').fadeOut(300);
        }
    });

    $(function () {
        $("img.lazy").lazyload({
            failure_limit: 50,
            skip_invisible: true
        });
    });

    $("img.lazy").show().lazyload();

    $('.go-top').click(function (event) {
        event.preventDefault();
        jQuery('html, body').animate({ scrollTop: 0 }, 500, "easeOutQuart");
        return false;
    });

    if (isMobile() == false) {
        $(".dropdown").on("mouseover", function () { $(this).children(".dropdown-menu").slideDown(0); });
        $(".dropdown").on("mouseout", function () { $(this).children(".dropdown-menu").slideUp(0); });
    }
    else {
        SetMobileArrangements();
        $(".dropdown").unbind("mouseover"); $(".dropdown").unbind("mouseout");
    }

    $("#bakeryMarket-check-filter-div input[type=checkbox]").change(function () { bakeryCheckFilter(this); });
    $("#designs-check-filter-div input[type=checkbox]").change(function () { designsCheckFilter(this); });

    $("html").easeScroll({
        frameRate: 60,
        animationTime: 1000,
        stepSize: 120,
        pulseAlgorithm: !0,
        pulseScale: 8,
        pulseNormalize: 1,
        accelerationDelta: 20,
        accelerationMax: 1,
        keyboardSupport: !0,
        arrowScroll: 50
    });
    
});