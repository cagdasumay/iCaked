
/******************** Global Variables ********************/

var selectedObjName; var selectedObjEditorName; var selectedObjProperties; var decal_idx = 1000; var text_idx = 1000; var relief_idx = 1000;
var design; var objectsArray = []; var objectImages = []; var colors = []; var objectSizes = []; var screenScale = 0.8; var editorLang = "tr"; var addToCart = false;
var randomDesignCakeName = ""; var randomDesignUndoCnt = 0; var randomObjectNames = new Array(); var globalDesignObject;
var selectedColorInput = null; var cakeSizes = []; var entryCakeSize = 10; var globalDesignId = null;

/******************** Preload Functions ********************/

function preload(arrayOfImages) {
    $(arrayOfImages).each(function () {
        $('<img/>')[0].src = this;
    });
}

// Usage:

preload([
    '/Mutfak/table_texture.jpg',
    '/Mutfak/set_texture.jpg',
    '/Mutfak/set_normal.jpg',
    '/Mutfak/seat_texture.jpg',
    '/Mutfak/floor_texture.jpg',
    '/Mutfak/client_oven_texture.jpg',
    '/Mutfak/562445491.jpg'
]);

if (isMobile()) { screenScale = 0.9; } else { screenScale = 0.8; }

/******************** Toggling Functions ********************/

function toggleOtherDropdowns(elementId, speed) {
    $(".objectbar-dropdown[id!='" + elementId + "']").slideUp(speed);
}

function toggleElement(elementId, speed) {
    $("#" + elementId).slideToggle(speed, "easeOutQuint");
}

function toggleElement2(elementId, speed) {
    if ($(elementId).is(":visible")) { $(elementId).fadeOut(speed); }
    else { $(elementId).fadeIn(speed); }
}

function closeElement(elementId, speed) {
    $(elementId).slideUp(speed);
}

var timestamp = null;
var lastMouseX = null;
var lastMouseY = null;

function smoothDrag() {
    $.fn.draggableXYE = function (options) {
        var defaultOptions = {
            distance: 5,
            dynamic: false
        };
        options = $.extend(defaultOptions, options);

        // ADDED: Store startPosition for reverting
        $(this).position({
            my: "center",
            at: "center",
            of: "#dragBall-container"
        });

        var startPosition = $(this).position();

        // ADDED: Function to apply easing to passed element
        function AnimateElement(element, newpos) {
            $(element).stop().animate({
                top: newpos.top,
                left: newpos.left
            }, 0, 'easeOutQuart');
        }
        this.draggable({
            containment: $('#dragBall-container'),
            distance: options.distance,
            start: function (event, ui) {
                ui.helper.data('draggableXY.originalPosition', ui.position || {
                    top: 0,
                    left: 0
                });
                ui.helper.data('draggableXY.newDrag', true);
            },
            drag: function (event, ui) {
                var originalPosition = ui.helper.data('draggableXY.originalPosition');

                var e = event;
                if (timestamp === null) {
                    timestamp = Date.now();
                    lastMouseX = e.screenX;
                    lastMouseY = e.screenY;
                    return;
                }

                var now = Date.now();
                var dt = now - timestamp;
                var dx = e.screenX - lastMouseX;
                var dy = e.screenY - lastMouseY;
                var speedX = Math.round(dx / dt * 100);
                var speedY = Math.round(dy / dt * 100);

                timestamp = now;
                lastMouseX = e.screenX;
                lastMouseY = e.screenY;

                PanSmoothing(speedX, speedY);

                var deltaX = Math.abs(originalPosition.left - ui.position.left);
                var deltaY = Math.abs(originalPosition.top - ui.position.top);

                var newDrag = options.dynamic || ui.helper.data('draggableXY.newDrag');
                ui.helper.data('draggableXY.newDrag', false);

                var newPosition = ui.position;
                AnimateElement(this, newPosition);

                return newPosition;
            },
            stop: function () {
                if (options.revert) {
                    AnimateElement(this, startPosition);
                }
            }
        });
    };

    $('.dragBall').draggableXYE({
        revert: true,
        dynamic: true
    });
}

function PanSmoothing(speedX, speedY) {
    for (i = 10000; i < 10300; i++) {
        setTimeout(function () { pasta.pan(speedX / i, speedY / (i * 2)); }, i - 9995);
    }
}

/******************** Add/Copy/Delete Functions ********************/

function addObj(elementName, pos, posScreen, is2D, allowChildren) {
    $("body").css("cursor", "progress");
    setTimeout(function () { $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link"); $("body").css("cursor", "default"); }, 10000);
    $(".add-obj,.add-cake").addClass("disabled-link");
    $.ajax({
        url: "/Object/AddObject?ObjectName=" + elementName + "&count=0",
        type: 'POST',
        cache: false,
        success: function (result) {
            console.log("Eklenen Obje: " + result.Name + " " + result.EditorName + " MinSize: " + result.MinSize + " MaxSize: " + result.MaxSize + " StepSize: " + result.StepSize);
            console.log(result);
            var posZ = 1.4;
            if (typeof pasta.plateScale === "function") posZ *= pasta.plateScale();
            if (posScreen !== undefined) {
                pasta.addObject({ Name: result.EditorName, Source: result.Source, Object2D: is2D, AllowChildren: allowChildren, CoordSource: result.Properties.styles.coords, Properties: { cakeSize: result.CakeSize, minSize: result.MinSize, maxSize: result.MaxSize, stepSize: result.StepSize, scale: result.Properties.styles.scale, normalScale: result.NormalScale, transform: { position: { x: posZ, y: 0, z: posZ } } }, ScreenPos: posScreen }, result.Type);
            }
            else if (pos == null) {
                if (result.EditorName.indexOf("cake") > -1) {
                    pasta.addObject({ Name: result.EditorName, Source: result.Source, Object2D: is2D, AllowChildren: allowChildren, CoordSource: result.Properties.styles.coords, Properties: { cakeSize: result.CakeSize, minSize: result.MinSize, maxSize: result.MaxSize, stepSize: result.StepSize, scale: result.Properties.styles.scale, normalScale: result.NormalScale, transform: { position: { x: posZ, y: 0., z: posZ } } } }, result.Type);
                }
                else {
                    pasta.addObject({ Name: result.EditorName, Source: result.Source, Object2D: is2D, AllowChildren: allowChildren, CoordSource: result.Properties.styles.coords, Properties: { cakeSize: result.CakeSize, minSize: result.MinSize, maxSize: result.MaxSize, stepSize: result.StepSize, scale: result.Properties.styles.scale, normalScale: result.NormalScale, transform: { position: { x: posZ, y: 0.15, z: posZ } } } }, result.Type);
                }
            }
            else {
                pasta.addObject({ Name: result.EditorName, Source: result.Source, Object2D: is2D, AllowChildren: allowChildren, CoordSource: result.Properties.styles.coords, Properties: { cakeSize: result.CakeSize, minSize: result.MinSize, maxSize: result.MaxSize, stepSize: result.StepSize, scale: result.Properties.styles.scale, normalScale: result.NormalScale, transform: { position: { x: pos.x, y: pos.y, z: pos.z } } } }, result.Type);
            }

            selectedObjProperties = null;
            if (result.TexturePath != "") { pasta.addTexture(result.EditorName + "-texture", result.EditorName, "/Objects/Textures/" + result.TexturePath); } else { pasta.addTexture(undefined, result.EditorName, undefined); }

            if (result.Properties.styles.color != null) { pasta.setProperties(result.EditorName, { color: parseInt(result.Properties.styles.color, 16) }); }
            selectedObjName = result.EditorName;
            selectedObjEditorName = result.Name;

            addObjectHelper(result);
        }
    });
}

function addObjectHelper(result) {
    objectsArray.push(selectedObjName);
    objectImages.push(result.ImagePath);

    $("body").css("cursor", "default"); $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link");
    $("#save_temp,#order-design").removeClass("disabled-link");

    var imgPath = "/Objects/Images/" + result.ImagePath;
    console.log(imgPath);
    $("#mainObjectImg").attr("src", imgPath); $("#mainObjectImg").show();
    if (result.TexturePath != "") { $("#mainObjectColor").hide(); } else { $("#mainObjectColor").show(); }

    if (result.Type == "cake") {
        cakeSizes.push(parseInt(result.CakeSize)); openCloseCakes(totalCakeSize()); objectSizes.push("c");
    }
    else {
        //cakeSizes.push(0);
        objectSizes.push("o");
    }
}

function addRandomObj(type, count) {
    var _count = count || 0;
    var _type = type;
    if (type === "3dtext") _type = "";
    var _texts = [
        'İyi  ki  doğdun',
        'Mutlu  yıllar',
        'Seni  seviyorum'
    ];
    var _colors = [
        0x5a611e,
0xff8b00,
0xf8ca7d,
0xffaa42,
0xfc9301,
0xe86610,
0xf57f99,
0xeeb5c8,
0xf57fb4,
0xe7cbc8,
0xd46ba6,
0x9f4296,
0xa992d2,
0x8971c0,
0xc56ab9,
0xdc2f2f,
0xf03e3e,
0xc83639,
0xd72a33,
0x00b0ad,
0x40bfc2,
0xf3c000,
0xe5db62,
0xf5dc00,
0xeecc12,
0xefcc00,
0xefba39,
0xfefefe
    ];

    $.ajax({
        url: "/Object/GetRandomObject?type=" + _type + "&count=" + _count,
        type: 'POST',
        cache: false,
        success: function (result) {
            var colorSelect = Math.floor(Math.random() * _colors.length);
            console.log("colorSelect: " + colorSelect);
            console.log(_colors[colorSelect]);
            if (type === "cake") {
                pasta.addObject({ Name: result.EditorName, Source: result.Source, CoordSource: result.Properties.styles.coords, Properties: { color: _colors[colorSelect], scale: result.Properties.styles.scale, transform: { position: { x: 0, y: 0.15, z: 0 } } }, onTo: "plate-0" }, result.Type);
                randomDesignCakeName = result.EditorName;
            } else if (type === "object") {
                var randomDesignCakeInfo = randomDesignCakeName.split("-");
                if (randomDesignCakeInfo.length > 0 & randomDesignCakeInfo[0] === "cake") {
                    pasta.addObject({ Name: result.EditorName, Source: result.Source, CoordSource: result.Properties.styles.coords, Properties: { scale: result.Properties.styles.scale, transform: { position: pasta.getPositionFromCake(randomDesignCakeName, 6) } }, onTo: randomDesignCakeName }, result.Type);
                }
            } else if (type === "side") {
                var randomDesignCakeInfo = randomDesignCakeName.split("-");
                if (randomDesignCakeInfo.length > 0 & randomDesignCakeInfo[0] === "cake") {
                    var color = 0xfaa719;
                    if (typeof result.Properties.styles.color === "string")
                        color = parseInt(result.Properties.styles.color, 16);
                    pasta.addObject({ Name: result.EditorName, onTo: randomDesignCakeName, Properties: { color: _colors[colorSelect] }, isSideDecoration: true, pieces: [result.Source] });
                }
            } else if (type === "random") {
                var randomDesignCakeInfo = randomDesignCakeName.split("-");
                if (randomDesignCakeInfo.length > 0 & randomDesignCakeInfo[0] === "cake") {
                    var color = 0xffffff;
                    if (typeof result.Properties.styles.color === "string")
                        color = parseInt(result.Properties.styles.color, 16);
                    pasta.addRandomObject(result.EditorName, result.Source, randomDesignCakeName, count, { color: _colors[colorSelect] });
                }
            } else if (type === "3dtext") {
                pasta.addObject({ Name: result.EditorName, Properties: { color: 0x8f0a97, transform: { position: { x: 0, y: 0.15, z: 1.3 } } }, isText3D: true, Font: Math.floor(Math.random() * 4.0), Source: _texts[Math.floor(Math.random() * _texts.length)], onTo: "plate-0" });
            }
            randomObjectNames.push(result.EditorName);
            randomDesignUndoCnt = 1;
            if (type === "random" || type === "side") return;
            if (result.TexturePath != "") {
                pasta.addTexture(result.EditorName + "-texture", result.EditorName, "/Objects/Textures/" + result.TexturePath);
                randomDesignUndoCnt++;
            }
            if (type === "cake") return;
            if (result.Properties.styles.color != null) {
                pasta.setProperties(result.EditorName, { color: parseInt(result.Properties.styles.color, 16) });
                randomDesignUndoCnt++;
            }
        },
        error: function () {
            randomObjectNames.push("");
        }
    });
}

function copyObj(elementName) {
    $.ajax({
        url: "/Object/CopyObject?ObjectName=" + elementName + "",
        type: 'GET',
        cache: false,
        success: function (result) {
            pasta.addObject({ Name: result.EditorName, Copy: selectedObjName, CoordSource: result.Properties.styles.coords, Properties: { cakeSize: result.CakeSize, minSize: result.MinSize, maxSize: result.MaxSize, stepSize: result.StepSize, scale: selectedObjProperties.scale, initialScale: selectedObjProperties.initialScale, rotationY: selectedObjProperties.rotationY, transform: { position: { x: 1.5, y: 0, z: 1.5 } } } });

            if (selectedObjName.indexOf('side') | selectedObjName.indexOf('rand') | selectedObjName.indexOf('text')) { } else { console.log(result); objectImages.push(result.ImagePath); }

            objectsArray.push(elementName); objectSizes.push(objectSizes[objectsArray.indexOf(elementName)]);

            if (result.Type == "cake") {
                var copiedCakeSize = cakeSizes[objectsArray.indexOf(elementName)];
                cakeSizes.push(parseInt(copiedCakeSize)); openCloseCakes(totalCakeSize());
            } //else { cakeSizes.push(0); }
        }
    });
    $("#editorObjectPopup").fadeOut(100);
}

function deleteObj(elementName) {
    $("body").css("cursor", "progress");
    setTimeout(function () { $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link"); $("body").css("cursor", "default"); }, 10000);

    $.ajax({
        url: "/Object/DeleteObject?ObjectName=" + elementName + "",
        type: 'GET',
        cache: false,
        success: function (result) {
            if (pasta.removeObject(elementName) == -1) return;
            if (elementName.indexOf('side') > -1 | elementName.indexOf('rand') > -1 | elementName.indexOf('text') > -1) { } else { objectImages.splice(objectsArray.indexOf(elementName), 1); }
            objectSizes.splice(objectsArray.indexOf(elementName), 1);

            objectsArray = jQuery.grep(objectsArray, function (value) {
                return value != elementName;
            });

            if (elementName.indexOf("cake") > -1) { cakeSizes.pop(objectsArray.indexOf(elementName)); }

            if (objectsArray.length == 0) { $("#save_temp,#order-design").addClass("disabled-link"); } else { $("#save_temp,#order-design").removeClass("disabled-link"); }
            openCloseCakes(totalCakeSize());
            selectedObjName = undefined;
            selectedObjProperties = undefined;
            $("body").css("cursor", "default"); $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link");
        }
    });

    $("#editorObjectPopup").fadeOut(100);
}

function deleteDesign() {
    if (confirm("Tasarımınız tümüyle silinecektir. Devam etmek istiyor musunuz?")) {
        // burda istediğin metodu çağırabilirsin okan
        pasta.reset();
    }
}

function triggerAdd() {
    $('#editor_delete_warning').fadeOut(300);
    var addLabel = $("#addRandom").text();

    if (addLabel == "Taban Ekle") {
        if (randomCakeExists) {
            pasta.removeObject(randomObjectNames.pop() + "-3dtext");
            var removeElementInfo = randomObjectNames.pop().split("-");
            var indexRandart = parseInt(removeElementInfo[1]);
            for (var k = indexRandart; k < indexRandart + 15; k++) {
                pasta.removeObject("object-" + k + "-randart");
            }
            pasta.removeObject(randomObjectNames.pop());
            pasta.removeObject(randomObjectNames.pop());
            pasta.removeObject(randomObjectNames.pop());
            duration = 1000;
        }

        addRandomObj("cake");
        $("#addRandom").text("Yeni Obje Ekle"); $("#changeRandom").text("Tabanı Değiştir"); $("#changeRandom").removeClass("disabled-link2");
    }
    else if (addLabel == "Yeni Obje Ekle") {
        addRandomObj("object");
        $("#addRandom").text("Yan Süs Ekle"); $("#changeRandom").text("Objeyi Değiştir");
    }
    else if (addLabel == "Yan Süs Ekle") {
        addRandomObj("side");
        $("#addRandom").text("Rastgele Obje Ekle"); $("#changeRandom").text("Yan Süsü Değiştir");
    }
    else if (addLabel == "Rastgele Obje Ekle") {
        addRandomObj("random", 15);
        $("#addRandom").text("Rastgele Yazı Ekle"); $("#changeRandom").text("Objeleri Değiştir");
    }
    else if (addLabel == "Rastgele Yazı Ekle") {
        addRandomObj("3dtext");
        $("#addRandom").text("Sil ve Başa Dön"); $("#changeRandom").text("Yazıyı Değiştir");
    }
    else if (addLabel == "Sil ve Başa Dön") {
        pasta.removeObject(randomObjectNames.pop() + "-3dtext");
        var removeElementInfo = randomObjectNames.pop().split("-");
        var indexRandart = parseInt(removeElementInfo[1]);
        for (var k = indexRandart; k < indexRandart + 15; k++) {
            pasta.removeObject("object-" + k + "-randart");
        }
        pasta.removeObject(randomObjectNames.pop());
        pasta.removeObject(randomObjectNames.pop());
        pasta.removeObject(randomObjectNames.pop());
        $("#addRandom").text("Taban Ekle"); $("#changeRandom").text("Tabanı Değiştir"); $("#changeRandom").addClass("disabled-link2");
    }
    $("#save_temp,#order-design").removeClass("disabled-link");
}

function triggerChange() {
    var changeLabel = $("#changeRandom").text();
    var removeElementName = randomObjectNames.pop();

    if (changeLabel == "Tabanı Değiştir") {
        pasta.removeObject(removeElementName);
        addRandomObj("cake");
    }
    else if (changeLabel == "Objeyi Değiştir") {
        pasta.removeObject(removeElementName);
        addRandomObj("object");
    }
    else if (changeLabel == "Yan Süsü Değiştir") {
        pasta.removeObject(removeElementName);
        addRandomObj("side");
    }
    else if (changeLabel == "Objeleri Değiştir") {
        var removeElementInfo = removeElementName.split("-");
        var indexRandart = parseInt(removeElementInfo[1]);
        for (var k = indexRandart; k < indexRandart + 15; k++) {
            pasta.removeObject("object-" + k + "-randart");
        }
        addRandomObj("random", 10);
    }
    else if (changeLabel == "Yazıyı Değiştir") {
        pasta.removeObject(removeElementName + "-3dtext");
        addRandomObj("3dtext");
    }
}

var randomCakeExists = false;

function triggerMakeCake() {
    $('#editor_delete_warning').fadeOut(300);
    var duration = 0;
    if (randomCakeExists) {
        pasta.removeObject(randomObjectNames.pop() + "-3dtext");
        var removeElementInfo = randomObjectNames.pop().split("-");
        var indexRandart = parseInt(removeElementInfo[1]);
        for (var k = indexRandart; k < indexRandart + 15; k++) {
            pasta.removeObject("object-" + k + "-randart");
        }
        pasta.removeObject(randomObjectNames.pop());
        pasta.removeObject(randomObjectNames.pop());
        pasta.removeObject(randomObjectNames.pop());
        duration = 1000;
    }

    setTimeout(function () { addRandomObj("cake"); }, duration + 200);
    setTimeout(function () { addRandomObj("object"); }, duration + 400);
    setTimeout(function () { addRandomObj("side"); }, duration + 600);
    setTimeout(function () { addRandomObj("random", 10); }, duration + 800);
    setTimeout(function () { addRandomObj("3dtext"); }, duration + 1000);

    randomCakeExists = true;
    $("#save_temp,#order-design").removeClass("disabled-link");
}

/******************** Save/Load Functions ********************/

function checkLogin(csLogin) {
    if (csLogin == "False" & isLogin == false) {
        $(".overall-mask").fadeIn(300);
        $("#account-floating-div").fadeIn(300);
        event.preventDefault();
        return false;
    }
    else {
        return true;
    }
}

function LoadDesign(id) {
    $.ajax({
        url: "/Object/LoadDesign?id=" + id,
        type: 'POST',
        cache: false,
        success: function (result) {
            design = result;
            console.log(result);
            for (i = 0; i < design.length; i++) {
                objectsArray.push(design[i].EditorName);
                if (design[i].SavedSize != "n") { objectSizes.push(design[i].SavedSize); }
                if (design[i].Type == "cake") { cakeSizes.push(design[i].CakeSize); }
                objectImages.push(design[i].ImagePath);
                //console.log(design[i]);
            }
            openCloseCakes(totalCakeSize());
            $.when(pasta.load(design[0].JsonStr)).then(function () { $("#canvas-loader").fadeOut(300); });
            var _cakeSize = parseInt(design[0].CakeSize || 12);
            if (_cakeSize < 1) _cakeSize = 12;
            pasta.setPlateSizeRange(getCakeDimension(_cakeSize));
            $("#save_temp,#order-design").removeClass("disabled-link");
            globalDesignId = id;
            $("#canvas-cake-step,#canvas-left-lower").show();
            $("#canvas-upper").removeClass("disabled-link3"); $("#canvas-sizer").unbind("click");
            // addOGMetaTag(id);
            $("#editor-description-div").show();
            $("#editor-sub-wrapper").hide();
        }
    });
}

function getObjectNamesFromSaveString(str) {
    var objNames = ":"; console.log(str);
    var jObj = JSON.parse(str);
    function traverseChildren(children) {
        for (var child in children) {
            if (typeof child === "string" && (objNames.indexOf(child + ',') < 0 && objNames.indexOf(child + '-') < 0)) {
                var suffix = "";
                if (children[child].alignNormal) suffix = "-2d";
                else if (children[child].decal) suffix = "-decal";
                if (children[child].decal) {
                    objNames += child + suffix + "," + children[child].properties.scale.toFixed(4) + "," + children[child].decalTextureURL + ":";
                } else if (children[child].isText3D) {
                    objNames += child + suffix + "," + children[child].properties.scale.toFixed(4) + "," + children[child].text3D + ":";
                } else {
                    objNames += child + suffix + "," + children[child].properties.scale.toFixed(4) + ":";
                }
                traverseChildren(children[child].children);
            }
        }
    }
    traverseChildren(jObj.children);
    console.log(objNames);
    return objNames;
}

function AutoSaveCake() {
    var jsonstr = pasta.save({ isAuto: true });
    var objNames = getObjectNamesFromSaveString(jsonstr);

    var cakesize = "";
    for (i = 0; i < cakeSizes.length; i++) { cakesize = cakesize + cakeSizes[i] + ","; } cakesize = cakesize.substr(0, cakesize.length - 1);

    var profileVisible = "no";

    var content = editorDesignSelect + "??" + cakesize + "??" + jsonstr + "??" + objNames;

    //var formData = JSON.parse(JSON.stringify(content.serializeArray()));
    // console.log(formData);

    $.ajax({
        url: "/Object/AutoSaveCake",
        type: 'POST',
        data: { content: content },
        cache: false,
        success: function (result) {
            console.log("autosave");
        }
    });
}

function GetDesignPrice() {
    if ($("#editorDesignPrice").is(":visible") == false) { $("#design-price-loading").slideDown(200); }

    AutoSaveCake();

    var cakesize = "";
    for (i = 0; i < cakeSizes.length; i++) { cakesize = cakesize + cakeSizes[i] + ","; } cakesize = cakesize.substr(0, cakesize.length - 1);

    $.ajax({
        url: "/Object/GetDesignPrice?bakeryID=" + selectedBakeryID,
        type: 'POST',
        cache: false,
        success: function (result) {
            $("#editorDesignPrice").text("Fiyat : " + result + " TL");
            $("#design-price-loading").slideUp();
            $('#order-design,#editorDesignPrice').slideDown(200);
        }
    });
}

function saveCakeContinue() {
    $("#editor-name-dialog").fadeOut(300);
    $("#saving-div").fadeIn(300);
    var jsonstr = pasta.save();
    var objNames = getObjectNamesFromSaveString(jsonstr);
    var imgstr = new Array();
    var html;
    pasta.cameraPosition(0);
    setTimeout(function () {
        imgstr.push(capture());
        pasta.cameraPosition(1);
        setTimeout(function () {
            imgstr.push(capture());
            pasta.cameraPosition(2);
            setTimeout(function () {
                imgstr.push(capture());
                pasta.cameraPosition(3);
                setTimeout(function () {
                    imgstr.push(capture());
                    pasta.cameraPosition(4);
                    setTimeout(function () {
                        imgstr.push(capture());
                        pasta.cameraPosition(5);
                        setTimeout(function () {
                            imgstr.push(capture());
                            pasta.cameraPosition(6);
                            setTimeout(function () {
                                imgstr.push(capture());
                                pasta.cameraPosition(7);
                                setTimeout(function () {
                                    imgstr.push(capture());
                                    sendSaveData();
                                    pasta.afterSave();
                                }, 600);
                            }, 600);
                        }, 600);
                    }, 600);
                }, 600);
            }, 600);
        }, 600);
    }, 600);

    var cakesize = "";
    for (i = 0; i < cakeSizes.length; i++) { cakesize = cakesize + cakeSizes[i] + ","; } cakesize = cakesize.substr(0, cakesize.length - 1);

    var profileVisible = "no";
    if ($("#designVisible").prop("checked") == true) { profileVisible = "yes"; }

    function sendSaveData() {
        if (addToCart == false) {
            var content = "<form id='canvasImageForm' action='/Object/SaveCake' method='post' style='display:none;'>" +
                "<input type='hidden' name='whichEditor' value='editor2' />" +
                "<input type='hidden' name='BakeryID' value='" + selectedBakeryID + "' />" +
                "<input type='hidden' name='ContentID' value='" + $("#editor-bakery-content").val() + "' />" +
                "<input type='hidden' name='designNotes' value='" + $("#design-note-input").val() + "' />" +
                "<input type='hidden' name='design_visible' value='" + profileVisible + "' />" +
                "<input type='hidden' name='design_type' value='" + editorDesignSelect + "' />" +
                "<input type='hidden' name='addToCart' value='no' />" +
                "<input type='hidden' name='cakeSizes' value='" + cakesize + "' />" +
                "<input type='hidden' name='designName' value='" + $("#design-name-input").val() + ' ' + $("#design-name-prefix").text() + "' />" +
                "<input type='hidden' name='designCategory' value='" + $("#design-category-input").val() + "' />" +
                "<input type='hidden' name='CanvasImgStr1' id='contentStr1' value='" + imgstr[0] + "'/>" +
                "<input type='hidden' name='CanvasImgStr2' id='contentStr2' value='" + imgstr[1] + "'/>" +
                "<input type='hidden' name='CanvasImgStr3' id='contentStr3' value='" + imgstr[2] + "'/>" +
                "<input type='hidden' name='CanvasImgStr4' id='contentStr4' value='" + imgstr[3] + "'/>" +
                "<input type='hidden' name='CanvasImgStr5' id='contentStr5' value='" + imgstr[4] + "'/>" +
                "<input type='hidden' name='CanvasImgStr6' id='contentStr6' value='" + imgstr[5] + "'/>" +
                "<input type='hidden' name='CanvasImgStr7' id='contentStr7' value='" + imgstr[6] + "'/>" +
                "<input type='hidden' name='CanvasImgStr8' id='contentStr8' value='" + imgstr[7] + "'/>" +
                //"<input type='hidden' name='CanvasImgStr5' id='contentStr5' value='" + imgstr[4] + "'/>" +
                "<input type='hidden' name='JsonStr' value='" + jsonstr + "' />" +
            "<input type='hidden' name='GlobalObjNames' value='" + objNames + "' /></form>";
            html = $(content);
        }
        else {
            var content = "<form id='canvasImageForm' action='/Object/SaveCake' method='post' style='display:none;'>" +
            "<input type='hidden' name='whichEditor' value='editor2' />" +
            "<input type='hidden' name='BakeryID' value='" + selectedBakeryID + "' />" +
            "<input type='hidden' name='ContentID' value='" + $("#editor-bakery-content").val() + "' />" +
            "<input type='hidden' name='designNotes' value='" + $("#design-note-input").val() + "' />" +
            "<input type='hidden' name='design_visible' value='" + profileVisible + "' />" +
            "<input type='hidden' name='design_type' value='" + editorDesignSelect + "' />" +
            "<input type='hidden' name='addToCart' value='yes' />" +
            "<input type='hidden' name='cakeSizes' value='" + cakesize + "' />" +
            "<input type='hidden' name='designName' value='" + $("#design-name-input").val() + ' ' + $("#design-name-prefix").text() + "' />" +
            "<input type='hidden' name='designCategory' value='" + $("#design-category-input").val() + "' />" +
            "<input type='hidden' name='CanvasImgStr1' id='contentStr1' value='" + imgstr[0] + "'/>" +
            "<input type='hidden' name='CanvasImgStr2' id='contentStr2' value='" + imgstr[1] + "'/>" +
            "<input type='hidden' name='CanvasImgStr3' id='contentStr3' value='" + imgstr[2] + "'/>" +
            "<input type='hidden' name='CanvasImgStr4' id='contentStr4' value='" + imgstr[3] + "'/>" +
            "<input type='hidden' name='CanvasImgStr5' id='contentStr5' value='" + imgstr[4] + "'/>" +
            "<input type='hidden' name='CanvasImgStr6' id='contentStr6' value='" + imgstr[5] + "'/>" +
            "<input type='hidden' name='CanvasImgStr7' id='contentStr7' value='" + imgstr[6] + "'/>" +
            "<input type='hidden' name='CanvasImgStr8' id='contentStr8' value='" + imgstr[7] + "'/>" +
            //"<input type='hidden' name='CanvasImgStr5' id='contentStr5' value='" + imgstr[4] + "'/>" +
            "<input type='hidden' name='JsonStr' value='" + jsonstr + "' />" +
            "<input type='hidden' name='GlobalObjNames' value='" + objNames + "' /></form>";
            html = $(content);
        }

        $("#editor-media-div").append(html);
        $(window).off('beforeunload');
        $("#canvasImageForm").submit();
    }
}

function uploadDesignImages() {
    var formData = new FormData();
    var totalFiles = document.getElementById("design-photos-input").files.length;
    for (var i = 0; i < totalFiles; i++) {
        var file = document.getElementById("design-photos-input").files[i];
        formData.append("imageUploadForm", file);
    }
    $.ajax({
        url: "/Object/SavePastaResmi",
        type: "POST",
        dataType: 'json',
        processData: false,
        contentType: false,
        data: formData,
        success: function (response) {
            saveCakeContinue();
        }
    });
}

var selectedBakeryID;

function saveCake(csLogin, type) {
    if (type == "save") {
        if (checkLogin(csLogin)) {
            setDesignSize();
            //setObjSizes();
            uploadDesignImages();
        }
    }
    else {
        setDesignSize();
        //setObjSizes();
        uploadDesignImages();
    }
}

function setDesignSize() {
    var totalSize = 0;
    for (i = 0; i < cakeSizes.length; i++) { totalSize = totalSize + cakeSizes[i]; }

    $.ajax({
        url: "/Object/EditorCakeSelect?size=" + totalSize,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
        }
    });
}

function globalLoadSource(objName, designID) {
    if (objName == "plate-0") {
        return "";
    }
    else {
        for (i = 0; i < design.length; i++) {
            if (design[i].EditorName == objName) {
                if (design[i].EditorName.indexOf("side") > -1) { var ret = [design[i].Source]; return ret; }
                else { return design[i].Source; }
            }
        }
    }
}

function globalLoadTextureURL(textureName, designID) {
    if (textureName == "plate-0-texture") {
        return "";
    }
    else {
        for (i = 0; i < design.length; i++) {
            if (design[i].EditorName + "-texture" == textureName) { return "/Objects/Textures/" + design[i].TexturePath; }
        }
    }
}

function designSaveOption(option) {
    if (option == 'saveas') {
        $("#editor-name-dialog").css("height", "290px");
        $('#design-name-div').slideDown(200);
    }
    else if (option == 'overwrite') {
        $("#editor-name-dialog").css("height", "250px");
        $('#design-name-div').slideUp(200);
    }

    $.ajax({
        url: "/Object/SaveType?type=" + option,
        type: 'POST',
        cache: false,
        success: function (result) {
        }
    });
}

function saveObjSize(objname, size) {
    objectSizes[objectsArray.indexOf(objname)] = size;
}

function capture() {
    imgData = pasta.rendererDomElement().toDataURL("image/jpeg");
    return imgData;
}

/******************** Filter Functions ********************/

function filterMainEditor() {
    if ($("#editor-main-search-input").val().length > 0) {
        $(".editor-objectbar").hide();
        var elem = "#editor-main-search-input";
        var inputKeywords = $(elem).val().toLowerCase().split(" ");
        for (i = 0; i < inputKeywords.length; i++) {
            if (inputKeywords[i].toLowerCase() == "" | inputKeywords[i].toLowerCase() == " ") { inputKeywords.splice(i, 1); }
        }
        var divs = $(elem).parent().parent().find(".filterObj");
        $(divs).hide();
        for (i = 0; i < divs.length; i++) {
            var keywords = $(divs[i]).attr("data-keywords").split(","); keywords.push($(divs[i]).attr("data-obj").toLowerCase());
            for (i2 = 0; i2 < keywords.length; i2++) {
                var keywordsMatch = 0;
                for (i3 = 0; i3 < inputKeywords.length; i3++) {
                    if (keywords[i2].toLowerCase().indexOf(inputKeywords[i3]) > -1) { keywordsMatch++; }
                }
                if (keywordsMatch == inputKeywords.length) { $(divs[i]).show(); }
            }
        }
        $("#editor-main-search-dropdown").slideDown(200);
    }
    else {
        $("#editor-main-search-dropdown").hide();
        $(".editor-objectbar").slideDown(200);
    }
}

function filterCakes(elem) {
    var inputKeywords = $(elem).val().toLowerCase().split(" ");
    for (i = 0; i < inputKeywords.length; i++) {
        if (inputKeywords[i].toLowerCase() == "" | inputKeywords[i].toLowerCase() == " ") { inputKeywords.splice(i, 1); }
    }
    var divs = $(elem).parent().parent().find(".add-cake");
    $(divs).hide();
    for (i = 0; i < divs.length; i++) {
        var keywords = $(divs[i]).attr("data-keywords").split(","); keywords.push($(divs[i]).attr("data-obj").toLowerCase());
        for (i2 = 0; i2 < keywords.length; i2++) {
            var keywordsMatch = 0;
            for (i3 = 0; i3 < inputKeywords.length; i3++) {
                if (keywords[i2].toLowerCase().indexOf(inputKeywords[i3]) > -1) { keywordsMatch++; }
            }
            if (keywordsMatch == inputKeywords.length) { $(divs[i]).show(); }
        }
    }
}

function filterObjects(elem, where) {
    var inputKeywords = $(elem).val().toLowerCase().split(" ");
    for (i = 0; i < inputKeywords.length; i++) {
        if (inputKeywords[i].toLowerCase() == "" | inputKeywords[i].toLowerCase() == " ") { inputKeywords.splice(i, 1); }
    }
    var divs = $("#object-categories-wrapper").find(".add-obj");
    $(divs).hide();
    for (i = 0; i < divs.length; i++) {
        var keywords = $(divs[i]).attr("data-keywords").split(","); keywords.push($(divs[i]).attr("data-obj").toLowerCase());
        for (i2 = 0; i2 < keywords.length; i2++) {
            var keywordsMatch = 0;
            for (i3 = 0; i3 < inputKeywords.length; i3++) {
                if (keywords[i2].toLowerCase().indexOf(inputKeywords[i3]) > -1) { keywordsMatch++; }
            }
            if (keywordsMatch == inputKeywords.length) { $(divs[i]).show(); }
        }
    }

    if (where == 'multi') {
        filter2DObjects(elem);
    }
}

function filter2DObjects(elem) {
    var inputKeywords = $(elem).val().toLowerCase().split(" ");
    for (i = 0; i < inputKeywords.length; i++) {
        if (inputKeywords[i].toLowerCase() == "" | inputKeywords[i].toLowerCase() == " ") { inputKeywords.splice(i, 1); }
    }
    var divs = $("#object2d-categories-wrapper").find(".add-obj[data-is2d='true']");
    $(divs).hide();
    for (i = 0; i < divs.length; i++) {
        var keywords = $(divs[i]).attr("data-keywords").split(","); keywords.push($(divs[i]).attr("data-obj").toLowerCase());
        for (i2 = 0; i2 < keywords.length; i2++) {
            var keywordsMatch = 0;
            for (i3 = 0; i3 < inputKeywords.length; i3++) {
                if (keywords[i2].toLowerCase().indexOf(inputKeywords[i3]) > -1) { keywordsMatch++; }
            }
            if (keywordsMatch == inputKeywords.length) { $(divs[i]).show(); }
        }
    }
}

function filterObjectsByBakery(val) {
    var filterDivs = $(".add-cake, .add-obj");
    filterDivs.hide();
    for (i = 0; i < filterDivs.length; i++) {
        if ($(filterDivs[i]).attr("data-bakery").indexOf(val)) { $(filterDivs[i]).show(); }
    }
}

/******************** Object Functions ********************/

function objectCloser() {
    $('body').unbind('touchmove');
    $("#paste,#delete").addClass("disabled-link");
    selectedObjName = null; selectedObjProperties = null;

    deactivateSideRandomImage();
}

function objGetterMenu(name, designObject) {
    var type = detectType(name);
    $(".editor-objectbar-utilitydiv").slideDown(400);

    var imgPath = "/objects/Images/" + objectImages[objectsArray.indexOf(name)];

    if (imgPath.indexOf("undefined") > -1) { $("#mainObjectImg").hide(); } else { $("#mainObjectImg").attr("src", imgPath); $("#mainObjectImg").show(); }
    if (designObject.textureURL != "") {
        $("#mainObjectColor").hide();
    }
    else { $("#mainObjectColor").show(); }
}

function setObjSizes() {
    $.ajax({
        url: "/Object/SetObjectSizes",
        type: 'POST',
        data: { sizes: objectSizes },
        cache: false,
        success: function (result) {
        }
    });
}

function changePlate(option) {
    pasta.changePlate(option);
}

/******************** Tutorial Functions ********************/

function changeTutorialVideo(idx, elem) {
    $(".tutorial_buttons").removeClass("tutorial-active");
    $(elem).addClass("tutorial-active");
    $(".tutorial_video_wrapper").fadeOut(100);
    var videos = $('.tutorial_video_wrapper');
    for (i = 0; i < videos.length; i++) { $('.tutorial_video_wrapper')[i].pause(); }
    setTimeout(function () { $(".tutorial_video_wrapper:nth-child(" + idx + ")").fadeIn(300); $('.tutorial_video_wrapper')[idx - 1].play(); }, 210);
}

function dontShowTutorial(elem) {
    if ($(elem).is(":checked")) {
        $.ajax({
            url: "/Object/DontShowTutorial",
            type: 'POST',
            cache: false,
            success: function (result) {
            }
        });
    }
}

function objectGetter(name, designObject) {
    console.log(name);
    console.log(designObject);

    objGetterMenu(name, designObject);
    $("#text-extra-utilities-div,#extra-utilities-div").hide(); $("#main-utilities-div").fadeIn(200);

    var isDecal = designObject.decal;
    var isText = designObject.properties.text != undefined;
    if (isDecal) {
        designObject.properties.isDecal = true; designObject.properties.isText = false;
        designObject.properties.source = designObject.source;
        designObject.properties.color = (designObject.source.split(":"))[3];
        $("#extra-utilities-header").hide(); $("#main-utilities-header").css("width", "100%"); $(".editor-platebar").hide(); $("#rotation-slider-row").show();
        $("#color-wrapper").hide();
    }
    else {
        $("#color-wrapper").show();
        designObject.properties.isDecal = false; designObject.properties.isText = false;

        $("#size-slider-row,#rotation-slider-row,.editor-platebar").show();

        if (name.indexOf("cake") >= 0) { activateSideRandomImage(); $(".editor-platebar").hide(); $("#rotation-slider-row").show(); }
        else if (name.indexOf("plate") >= 0) { deactivateSideRandomImage(); $(".editor-platebar").show(); $("#rotation-slider-row").hide(); $("#mainObjectImg").show(); $("#mainObjectImg").prop("src", "/Images/Site/altlik_image.png"); }
        else if (name.indexOf("side") > 0) { $("#size-slider-row,#rotation-slider-row,.editor-platebar").hide(); }
        else { deactivateSideRandomImage(); $(".editor-platebar").hide(); $("#rotation-slider-row").show(); }

        if (name.indexOf("side") > 0 | name.indexOf("randart") > 0) { $("#paste").hide(); } else { $("#paste").show(); }
    }

    var properties = designObject.properties;
    globalDesignObject = designObject;
    $("#paste,#delete").removeClass("disabled-link");

    if (properties.initialScale == null) {
        if (selectedObjProperties != null) { properties.initialScale = selectedObjProperties.initialScale; }
        else { properties.initialScale = 1; }
    }

    if (properties.rotationY == null) {
        if (selectedObjProperties != null) { properties.rotationY = selectedObjProperties.rotationY; }
        else { properties.rotationY = 0; }
    }

    updateSizeSlider(name, properties, isDecal, designObject);
    updateRotationSlider(name, properties, isDecal, designObject);

    selectedObjName = name; selectedObjProperties = properties;

    if (!isDecal) {
        $("#object-color-input").css("background-color", "#" + parseInt(properties.color, 16).toString().toUpperCase());
        $("#object-color-input").css("background-color", "#" + properties.color.toString(16).toUpperCase());
    }

    $('body').bind('touchmove', function (e) { e.preventDefault() });
    if (Object.keys(designObject.colorize).length > 0) {
        $("#object-color-input").hide(); $("#object-color-inputs").show();
        prepareColors(designObject);
    }
    else if (designObject.textureURL == undefined) {
        $("#object-color-inputs").hide(); $("#object-color-input").show();
    }
    else {
        $("#object-color-inputs").hide(); $("#object-color-input").show();
    }

    if (designObject.textureURL == "" | Object.keys(designObject.colorize).length != 0) { $("#color-picker-div").slideDown(200); }
    else if (designObject.textureURL == undefined) { $("#color-picker-div").slideDown(200); }
    else { $("#color-picker-div").slideUp(200); }
}

/******************** Slider/Color/Image Functions ********************/

function initiateRotationSlider() {
    var slider = document.getElementById('rotation-slider');

    noUiSlider.create(slider, {
        start: 0,
        step: 15,
        range: {
            'min': [-180],
            'max': [180]
        },
    });
}

function updateRotationSlider(name, properties, isDecal, designObject) {
    var slider = document.getElementById('rotation-slider');
    slider.noUiSlider.destroy();

    noUiSlider.create(slider, {
        start: (properties.rotationY / 3.14 * 180),
        step: 15,
        range: {
            'min': -180,
            'max': 180
        }
    });

    $("#rotation-slider-label").text(slider.noUiSlider.get());

    slider.noUiSlider.on('slide', function () {
        var sliderVal = (slider.noUiSlider.get() * 6.28) / 360;
        properties.rotationY = sliderVal;
        $("#rotation-slider-label").text(slider.noUiSlider.get());

        if (isDecal) { pasta.setPropertiesDecal(name, { rotationY: sliderVal }); }
        else { pasta.previewChanges(name, { rotationY: properties.rotationY, color: properties.color, scale: properties.scale }); }
    });

    slider.noUiSlider.on('end', function () {
        var sliderVal = (slider.noUiSlider.get() * 6.28) / 360;

        if (isDecal) { pasta.setPropertiesDecal(name, { rotationY: sliderVal }); }
        else { pasta.setProperties(name, { rotationY: properties.rotationY }); }
    });
}

function initiateSizeSlider() {
    var slider = document.getElementById('size-slider');

    noUiSlider.create(slider, {
        start: 1,
        step: 1,
        range: {
            'min': [1],
            'max': [5]
        },
    });
}

function updateSizeSlider(name, properties, isDecal, designObject) {
    var slider = document.getElementById('size-slider');

    if (slider.noUiSlider !== undefined)
        slider.noUiSlider.destroy();

    var isCake = name.indexOf("cake") >= 0;
    var cakeCurrentSize = 0;

    console.log(properties);

    if (isCake && !isDecal) {
        var currentSize = getCakeSize(parseFloat(properties.scale / properties.initialScale));
        cakeCurrentSize = currentSize;
        noUiSlider.create(slider, {
            start: currentSize,
            step: parseFloat(properties.stepSize),
            range: {
                'min': parseFloat(properties.minSize),
                'max': 62
            }
        });
        $("#size-slider-label").text(" For " + currentSize + " People");
    }
    else if (name == "plate-0") {
        noUiSlider.create(slider, {
            start: properties.scale,
            step: 0.1,
            range: {
                'min': 0,
                'max': 2
            }
        });
        $("#size-slider-label").text("x " + parseFloat(properties.scale / properties.initialScale).toFixed(2));
    }
    else if (name.indexOf("3dtext") > -1) {
        noUiSlider.create(slider, {
            start: properties.scale,
            step: 0.1,
            range: {
                'min': 0.3,
                'max': 3
            }
        });
        $("#size-slider-label").text("x " + parseFloat(properties.scale / properties.initialScale).toFixed(2));
    }
    else {
        var currentSize = parseFloat(properties.scale / properties.initialScale);
        var isObj = properties.stepSize != undefined;
        var max = "";

        if (isObj || isDecal) {
            noUiSlider.create(slider, {
                start: currentSize,
                //step: parseFloat(properties.stepSize),
                range: {
                    'min': parseFloat(properties.minSize),
                    'max': parseFloat(properties.maxSize)
                }
            });
            max = parseFloat(properties.maxSize);
        }
        else {
            noUiSlider.create(slider, {
                start: 1,
                range: {
                    'min': 0.5,
                    'max': 2
                }
            });
            max = parseFloat(properties.maxSize);
        }

        var sizetemp = (parseFloat(properties.maxSize) - parseFloat(properties.minSize)) / 3;

        var objSizeLabel = ""; var synoLabel = "";
        var minsize = parseFloat(properties.minSize);

        if (currentSize <= parseFloat(minsize + sizetemp)) { objSizeLabel = "Küçük"; synoLabel = "k"; }
        else if (currentSize >= parseFloat(minsize + sizetemp) & currentSize <= parseFloat(minsize + sizetemp * 2)) { objSizeLabel = "Orta"; synoLabel = "o"; }
        else if (currentSize >= parseFloat(minsize + sizetemp * 2)) { objSizeLabel = "Büyük"; synoLabel = "b"; }

        $("#size-slider-label").text(objSizeLabel);
    }

    slider.noUiSlider.on('slide', function () {
        var sliderVal = slider.noUiSlider.get(); var obj_scale = properties.initialScale * sliderVal;

        if (isCake && !isDecal) {
            $("#size-slider-label").text(" For " + sliderVal.toString().replace(".00", "") + " People");
            cakeCurrentSize = parseInt(sliderVal.toString().replace(".00", ""));

            var idx = objectsArray.indexOf(name);
            cakeSizes[idx] = parseInt(sliderVal.toString().replace(".00", ""));

            openCloseCakes(totalCakeSize());
        }
        else if (name == "plate-0") { $("#size-slider-label").text("x " + sliderVal); }
        else {
            var objSizeLabel = ""; var currentSize = parseFloat(properties.scale / properties.initialScale); console.log(currentSize);
            var sizetemp = (parseFloat(properties.maxSize) - parseFloat(properties.minSize)) / 3;

            // console.log("sizeTemp : " + sizetemp + " küçük : " + parseFloat(parseFloat(properties.minSize) + sizetemp) + " orta : " + parseFloat(parseFloat(properties.minSize) + sizetemp) + " - " + parseFloat(parseFloat(properties.minSize) + sizetemp * 2) + " büyük : " + parseFloat(parseFloat(properties.minSize) + sizetemp * 2));

            var minsize = parseFloat(properties.minSize);

            if (currentSize <= parseFloat(minsize + sizetemp)) { objSizeLabel = "Küçük"; synoLabel = "k"; }
            else if (currentSize >= parseFloat(minsize + sizetemp) & currentSize <= parseFloat(minsize + sizetemp * 2)) { objSizeLabel = "Orta"; synoLabel = "o"; }
            else if (currentSize >= parseFloat(minsize + sizetemp * 2)) { objSizeLabel = "Büyük"; synoLabel = "b"; }
            $("#size-slider-label").text(objSizeLabel);
            saveObjSize(name, synoLabel);
        }

        if (isDecal) { pasta.setPropertiesDecal(name, { scale: obj_scale }); properties.scale = obj_scale; }
        else if (isCake) {
            var diyez = selectedObjProperties.color; if (typeof diyez == "string") { diyez = parseInt(diyez.replace("#", ""), 16); }
            pasta.previewChanges(name, { color: diyez, scale: getCakeDimension(sliderVal) });
            properties.scale = getCakeDimension(sliderVal);
        }
        else {
            var diyez = selectedObjProperties.color; if (typeof diyez == "string") { diyez = parseInt(diyez.replace("#", ""), 16); }
            pasta.previewChanges(name, { color: diyez, scale: properties.scale });
            properties.scale = obj_scale;
        }
    });

    slider.noUiSlider.on('end', function () {
        var sliderVal = slider.noUiSlider.get(); var obj_scale = properties.initialScale * sliderVal;
        if (selectedObjName != "plate-0") {
            if (isDecal) { pasta.setPropertiesDecal(name, { scale: obj_scale }); }
            else if (isCake) {
                pasta.setProperties(name, { scale: getCakeDimension(sliderVal) });
            }
            else { pasta.setProperties(name, { scale: properties.scale }); }
        }
    });
}

function prepareColors(designObject) {
    var content = "<label style='width:100%; text-align:center; font-size:12px; margin-bottom:5px;'>Renkler</label>"; var lol = 100 / Object.keys(designObject.colorize).length; var i = 0;
    for (var key in designObject.colorize) {
        var color = d2h(designObject.colorize[key]);
        content = content + '<input onclick="openColorDiv(' + i + ')" type="button" data-color=' + designObject.colorize[key] + ' class="object-color-inputs" style="height:30px; background:#' + color + '; border:1px solid #e6e6e6; width:' + lol * 0.8 + '%; margin:' + lol * 0.1 + '%; border-radius:3px;" />';
        i++;
    }
    $("#object-color-inputs").empty();
    $("#object-color-inputs").append(content);
    $(".object-color-inputs:nth-of-type(1)").trigger("click");
}

function openColorDiv(idx) {
    selectedColorInput = $(".object-color-inputs")[idx];
    $(".object-color-inputs").css("box-shadow", "none");
    $(selectedColorInput).css("box-shadow", "0px 0px 3px #268ECE");
}

function colorizeTexture() {
    var textureURLInfo = globalDesignObject.textureURL.split('.');
    if (textureURLInfo.length < 2) {
        console.log("Texture URL is not valid: " + globalDesignObject.textureURL);
        return;
    }
    textureURLInfo[textureURLInfo.length - 2] += "_alpha";
    var textureAlphaURL = textureURLInfo.join('.')
    console.log(textureAlphaURL);
    var colors = {}; var i = 0;
    for (var key in globalDesignObject.colorize) {
        //colors[key] = Math.floor(Math.random() * 0x1000000);
        colors[key] = $($(".object-color-inputs")[i]).attr('data-color'); i++;
    } console.log(colors);
    console.log("colorizeTexture: " + selectedObjName + " " + textureAlphaURL);
    console.log(colors);
    pasta.colorizeObject(selectedObjName, textureAlphaURL, colors);
}

function objColorUpdate(color, what) {
    if ($(".object-color-inputs").is(":visible")) {
        $(selectedColorInput).attr("data-color", parseInt(color.replace("#", ""), 16));
        $(selectedColorInput).css("background", color);
        colorizeTexture();
    }
    else {
        $("#object-color-input").css("background", color).css("background", color);
        selectedObjProperties.color = color;
        if (selectedObjProperties.isDecal) {
            var temp = selectedObjProperties.source.split(":"); temp[3] = color;
            var temp2 = temp[0] + ":" + temp[1] + ":" + temp[2] + ":" + temp[3] + ":" + temp[4];
            pasta.setPropertiesDecal(selectedObjName, { color: 0xff0000 }, temp2);
        }
        else if (selectedObjName == "plate-0") { console.log(color); pasta.previewChanges("plate-0", { color: parseInt(color.replace("#", ""), 16) }); }
        else {
            if (selectedObjName.indexOf("side") > -1) {
                pasta.setProperties(selectedObjName, {
                    color: parseInt(color.replace("#", ""), 16), all: "side"
                });
            }
            else {
                pasta.setProperties(selectedObjName, {
                    color: parseInt(color.replace("#", ""), 16)
                });
            }
        }
    }

    if (what == undefined) { arrangeLastUsedColors(color); }

    QuestionProcess(3);
}

function arrangeLastUsedColors(color) {
    var divs = $("#last-used-colors div");
    if (colors[colors.length - 1] != color) {
        colors.push(color);
        for (i = 0; i < 8; i++) { if (colors[colors.length - i - 1] != undefined) { $(divs[i]).css("background", colors[colors.length - i - 1]); $(divs[i]).attr("data-bg", colors[colors.length - i - 1]); } }
    }
}

function addTexture(selectedObjName, src) {
    $.ajax({
        url: "/Object/ChangeObjectTexture?ObjName=" + selectedObjName + "&Source=" + src + "",
        type: 'POST',
        cache: false,
        success: function (result) {
            pasta.addTexture(selectedObjName + "-texture", selectedObjName, src);
        }
    });
}

function uploadUserDecal() {
    var postdata = $('#userDecalForm').serialize();
    var file = document.getElementById('userDecal').files[0];
    var fd = new FormData();
    fd.append("decalCopy", file);
    var xhr = new XMLHttpRequest();
    xhr.open("POST", "/Object/UserDecal", false);
    xhr.send(fd);
    $("#userDecalWrapper .objectTextures").attr("src", "/Images/TempDecals/" + file.name);
}

/******************** Editor Functions ********************/

function editorLocationSelect(val) {
    $('#editor-start-button').slideDown(300);
    filterObjectsByBakery(val);
    //$("#editor-cakeSize-1").empty();

    $.ajax({
        url: "/Object/EditorLocationSelect?location=" + val,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
        }
    });

    //if (val == "76") {
    //    for (i = 2; i < 62; i = i + 2) { $("#editor-cakeSize-1").append("<option value='" + i + "'>" + i + "</option>"); }
    //}
    //else if (val == "48") {
    //    for (i = 10; i < 62; i = i + 5) { $("#editor-cakeSize-1").append("<option value='" + i + "'>" + i + "</option>"); }
    //}

    $('#editor-gramaj').text('Approximately weighs ' + parseInt($("#editor-cakeSize-1").val()) * 175 + ' gr - ' + parseInt($("#editor-cakeSize-1").val()) * 200 + ' gr.');
    $("#selectCakeSizeDiv").slideDown(300);
}

function activateSideRandomImage() {
    $("#add-side,#add-random,#add-image").removeClass("disabled-link2");
    $("#add-side,#add-random,#add-image").attr("data-original-title", "");
}

function deactivateSideRandomImage() {
    $("#add-side,#add-random,#add-image").addClass("disabled-link2");
    $("#add-side").attr("data-original-title", "Yan süs eklemek için önce taban eklemeniz gerekmektedir.");
    $("#add-random").attr("data-original-title", "Orta süsü eklemek için önce taban eklemeniz gerekmektedir.");
    $("#add-image").attr("data-original-title", "Resim eklemek için önce taban eklemeniz gerekmektedir.");
    if ($("#add-side-dropdown").is(":visible")) { $("#add-side-dropdown").slideUp(200); $(".editor-objectbar,#editor-main-search").slideDown(200); }
    if ($("#add-random-dropdown").is(":visible")) { $("#add-random-dropdown").slideUp(200); $(".editor-objectbar,#editor-main-search").slideDown(200); }
    if ($("#add-image-dropdown").is(":visible")) { $("#add-image-dropdown").slideUp(200); $(".editor-objectbar,#editor-main-search").slideDown(200); }
}

function getCakeDimension(kackisilik) {
    var newScale;
    if (kackisilik == 2) { newScale = 0.3333; }
    else if (kackisilik == 4) { newScale = 0.5333; }
    else if (kackisilik == 5) { newScale = 0.5833; }
    else if (kackisilik == 6) { newScale = 0.6333; }
    else if (kackisilik == 8) { newScale = 0.8; }
    else if (kackisilik == 10) { newScale = 0.8666; }
    else if (kackisilik == 12) { newScale = 0.9333; }
    else if (kackisilik == 14) { newScale = 1; }
    else if (kackisilik == 15) { newScale = 1.0333; }
    else if (kackisilik == 16) { newScale = 1.0666; }
    else if (kackisilik == 18) { newScale = 1.1333; }
    else if (kackisilik == 20) { newScale = 1.2; }
    else if (kackisilik == 22) { newScale = 1.2666; }
    else if (kackisilik == 24) { newScale = 1.3333; }
    else if (kackisilik == 25) { newScale = 1.3777; }
    else if (kackisilik == 26) { newScale = 1.4; }
    else if (kackisilik == 28) { newScale = 1.4666; }
    else if (kackisilik == 30) { newScale = 1.5333; }
    else if (kackisilik == 32) { newScale = 1.6; }
    else if (kackisilik == 34) { newScale = 1.6666; }
    else if (kackisilik == 35) { newScale = 1.7; }
    else if (kackisilik == 36) { newScale = 1.7333; }
    else if (kackisilik == 38) { newScale = 1.8; }
    else if (kackisilik == 40) { newScale = 1.8666; }
    else if (kackisilik == 42) { newScale = 1.9333; }
    else if (kackisilik == 44) { newScale = 2; }
    else if (kackisilik == 45) { newScale = 2.0333; }
    else if (kackisilik == 46) { newScale = 2.0666; }
    else if (kackisilik == 48) { newScale = 2.1333; }
    else if (kackisilik == 50) { newScale = 2.2; }
    else if (kackisilik == 52) { newScale = 2.2666; }
    else if (kackisilik == 54) { newScale = 2.3333; }
    else if (kackisilik == 55) { newScale = 2.3777; }
    else if (kackisilik == 56) { newScale = 2.4; }
    else if (kackisilik == 58) { newScale = 2.4666; }
    else if (kackisilik == 60) { newScale = 2.5333; }
    else if (kackisilik == 62) { newScale = 2.6; }
    return newScale;
}

function getCakeSize(scale) {       // interval veriyoz çünkü kafasına göre 0.86666667 'yı 0.8667 'ya dönüştürüyor.
    var kackisilik;
    if (scale >= 0.3333 & scale < 0.5333) { kackisilik = 2; }
    else if (scale >= 0.5333 & scale < 0.5833) { kackisilik = 4; }
    else if (scale >= 0.5833 & scale < 0.6333) { kackisilik = 5; }
    else if (scale >= 0.6333 & scale < 0.8) { kackisilik = 6; }
    else if (scale >= 0.8 & scale < 0.8666) { kackisilik = 8; }
    else if (scale >= 0.8666 & scale < 0.9333) { kackisilik = 10; }
    else if (scale >= 0.9333 & scale < 1) { kackisilik = 12; }
    else if (scale >= 1 & scale < 1.0333) { kackisilik = 14; }
    else if (scale >= 1.0333 & scale < 1.0666) { kackisilik = 15; }
    else if (scale >= 1.0666 & scale < 1.1333) { kackisilik = 16; }
    else if (scale >= 1.1333 & scale < 1.2) { kackisilik = 18; }
    else if (scale >= 1.2 & scale < 1.2666) { kackisilik = 20; }
    else if (scale >= 1.2666 & scale < 1.3333) { kackisilik = 22; }
    else if (scale >= 1.3333 & scale < 1.3777) { kackisilik = 24; }
    else if (scale >= 1.3777 & scale < 1.4) { kackisilik = 25; }
    else if (scale >= 1.4 & scale < 1.4666) { kackisilik = 26; }
    else if (scale >= 1.4666 & scale < 1.5333) { kackisilik = 28; }
    else if (scale >= 1.5333 & scale < 1.6) { kackisilik = 30; }
    else if (scale >= 1.6 & scale < 1.6666) { kackisilik = 32; }
    else if (scale >= 1.6666 & scale < 1.7) { kackisilik = 34; }
    else if (scale >= 1.7 & scale < 1.7333) { kackisilik = 35; }
    else if (scale >= 1.7333 & scale < 1.8) { kackisilik = 36; }
    else if (scale >= 1.8 & scale < 1.8666) { kackisilik = 38; }
    else if (scale >= 1.8666 & scale < 1.9333) { kackisilik = 40; }
    else if (scale >= 1.9333 & scale < 2) { kackisilik = 42; }
    else if (scale >= 2 & scale < 2.0333) { kackisilik = 44; }
    else if (scale >= 2.0333 & scale < 2.0666) { kackisilik = 45; }
    else if (scale >= 2.0666 & scale < 2.1333) { kackisilik = 46; }
    else if (scale >= 2.1333 & scale < 2.2) { kackisilik = 48; }
    else if (scale >= 2.2 & scale < 2.2666) { kackisilik = 50; }
    else if (scale >= 2.2666 & scale < 2.3333) { kackisilik = 52; }
    else if (scale >= 2.3333 & scale < 2.3777) { kackisilik = 54; }
    else if (scale >= 2.3777 & scale < 2.4) { kackisilik = 55; }
    else if (scale >= 2.4 & scale < 2.4666) { kackisilik = 56; }
    else if (scale >= 2.4666 & scale < 2.5333) { kackisilik = 58; }
    else if (scale >= 2.5333 & scale < 2.6) { kackisilik = 60; }
    else if (scale >= 2.6) { kackisilik = 62; }

    return kackisilik;
}

function d2h(d) { return (+d).toString(16); }

function openCloseCakes(selectedSize) {
    console.log(selectedSize);
    var cakedivs = $(".add-cake");
    for (i = 0; i < cakedivs.length; i++) {
        var minSize = $(cakedivs[i]).attr("data-minsize"); var maxSize = $(cakedivs[i]).attr("data-maxsize");
        if (editorModeSelect == "order") {
            if (selectedSize < minSize | selectedSize > maxSize) { $(cakedivs[i]).addClass("editor-disabled-item"); } else { $(cakedivs[i]).removeClass("editor-disabled-item"); }
        }
    }
}

function totalCakeSize() {
    var totalsize = 0;
    if (cakeSizes.length == 0) {
        totalsize = entryCakeSize;
    } else {
        for (i = 0; i < cakeSizes.length; i++) {
            totalsize = totalsize + parseInt(cakeSizes[i]);
        }
    }
    $("#cakeSize-label").text(" For " + totalsize + " People");
    return totalsize;
}

function fillCategories() {
    $.ajax({
        url: "/Object/GetDesignCategories",
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            for (i = 0; i < result.length; i++) {
                if (result[i].indexOf('Yarışma') == -1) {

                    var catStr = result[i];

                    if (catStr == "Doğum Günü Pastaları") { catStr = "Birthday Cakes"; }
                    else if (catStr == "1 Yaş Pastaları") { catStr = "Age 1 Cakes"; }
                    else if (catStr == "Bebek Pastaları") { catStr = "Baby Cakes"; }
                    else if (catStr == "Çizgi Film Pastaları") { catStr = "Cartoon Cakes"; }
                    else if (catStr == "Sevgili Pastaları") { catStr = "Valentines Cakes"; }
                    else if (catStr == "Kutlama Pastaları") { catStr = "Celebration Cakes"; }

                    $("#design-category-input").append("<option value='" + result[i] + "'>" + catStr + "</option>");
                }
            }
        }
    });
}

var editorModeSelect = ""; var editorDesignSelect = "";

function editorPage2(type) {
    editorModeSelect = type;
    if (editorModeSelect == "free") { editorCakeSelect2(10); }
    $.ajax({
        url: "/Object/EditorModeSelect?type=" + type,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            $("#editor-page1").fadeOut(200);
            setTimeout(function () { $("#editor-page2").fadeIn(200); }, 250);
        }
    });
}

function editorModeSelectTemp(type) {
    editorModeSelect = type;

    $.ajax({
        url: "/Object/EditorModeSelect?type=" + type,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
        }
    });
}

function editorDesignTypeTemp(type) {
    editorDesignSelect = type;
    $.ajax({
        url: "/Object/EditorDesignSelect?type=" + type,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            $("#page3-cake").fadeIn();
        }
    });
}

function editorDesignType(type) {
    editorDesignSelect = type;
    $.ajax({
        url: "/Object/EditorDesignSelect?type=" + type,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            $("#editor-page2").fadeOut(200);
            if (editorModeSelect == 'free') {
                setTimeout(function () { $("#editor-thanks-div").fadeIn(200); }, 250);
                setTimeout(function () { $("#editor-sub-wrapper").fadeOut(200); }, 1000);
                $("#order-design").remove();
            }
            else if (editorModeSelect == 'order') {
                setTimeout(function () {
                    if (editorDesignSelect == 'cake') { $("#page3-cake").fadeIn(); }
                    else if (editorDesignSelect == 'cookie') { $("#page3-cookie").fadeIn(); }
                    else if (editorDesignSelect == 'cupcake') { $("#page3-cupcake").fadeIn(); }
                    $("#editor-page3").fadeIn(200);
                }, 250);
            }
        }
    });
}

function backToEditorPage(page) {
    if (page == 1) {
        $("#editor-page2").fadeOut(300);
        setTimeout(function () { $("#editor-page1").fadeIn(200); }, 250);
    }
    else if (page == 2) {
        $("#editor-page3,#page3-cake,#page3-cookie,#page3-cupcake").fadeOut(200);
        setTimeout(function () { $("#editor-page2").fadeIn(200); }, 250);
    }
}

function editorCakeSelect2(val) {
    var size = 0;
    if (val != undefined) { size = val; } else { size = $("#editor-cakeSize-1").val(); }

    $.ajax({
        url: "/Object/EditorCakeSelect?size=" + size,
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            openCloseCakes(totalCakeSize());
        }
    });
}

function editorCakeSelect() {
    $.ajax({
        url: "/Object/EditorCakeSelect?size=" + $("#editor-cakeSize-1").val(),
        type: 'GET',
        async: false,
        cache: false,
        success: function (result) {
            $("#editor-page3").fadeOut(200);
            setTimeout(function () { $("#editor-thanks-div").fadeIn(200); }, 250);
            setTimeout(function () {
                $("#editor-sub-wrapper").fadeOut(200);
            }, 1000);
            setTimeout(function () { $("#canvas-cake-step,#canvas-left-lower").fadeIn(200); }, 1210);

            entryCakeSize = parseFloat($("#editor-cakeSize-1").val());
            pasta.previewChanges('plate-0', { scale: getCakeDimension(entryCakeSize) });
            pasta.setPlateSizeRange(getCakeDimension(entryCakeSize));
            openCloseCakes(totalCakeSize());

            $("#canvas-upper,#canvas-container2").removeClass("disabled-link3"); $("#canvas-sizer").unbind("click");
        }
    });
}

function detectType(name) {
    if (name.indexOf("cake") > -1) { return "cake"; }
    if (name.indexOf("object") > -1) {
        if (name.substr(7, 4) >= 1000) { return "text"; }
        else { return "object"; }
    }
}

function getDesignID() {
    $.ajax({
        url: "/Object/GetDesignID",
        type: 'POST',
        cache: false,
        success: function (result) {
            return result;
        }
    });
}

function changeEditorLanguage(langCode, elem) {
    editorLang = langCode;
    var labels = $("#canvas-sizer label");
    var inputs = $("#canvas-sizer input[type=text]");
    var buttons = $("#canvas-sizer button");
    var h1s = $("#canvas-sizer h1");
    var h2s = $("#canvas-sizer h2");
    var h3s = $("#canvas-sizer h3");
    var textareas = $("#canvas-sizer textarea");
    var options = $("#canvas-sizer option");
    for (i = 0; i < labels.length; i++) { $(labels[i]).text($(labels[i]).attr("data-" + langCode)); }
    for (i = 0; i < inputs.length; i++) { $(inputs[i]).attr("placeholder", $(inputs[i]).attr("data-" + langCode)); }
    for (i = 0; i < buttons.length; i++) { $(buttons[i]).html($(buttons[i]).attr("data-" + langCode)); }
    for (i = 0; i < h1s.length; i++) { $(h1s[i]).text($(h1s[i]).attr("data-" + langCode)); }
    for (i = 0; i < h2s.length; i++) { $(h2s[i]).text($(h2s[i]).attr("data-" + langCode)); }
    for (i = 0; i < h3s.length; i++) { $(h3s[i]).text($(h3s[i]).attr("data-" + langCode)); }
    for (i = 0; i < textareas.length; i++) { $(textareas[i]).text($(textareas[i]).attr("data-" + langCode)); }
    for (i = 0; i < options.length; i++) { $(options[i]).text($(options[i]).attr("data-" + langCode)); }
    // $("#lang_main_flag").attr("src", $(elem).attr("src"));
}

var dontShowCakeWarning = false;

function makeCakeWarning(what) {
    if (what == 'makeCake') {
        if (randomObjectNames.length != 0 & (dontShowCakeWarning == false)) {
            $('#editor_delete_warning').fadeIn(300);
            $("#okWarning").attr("onclick", "triggerMakeCake()");
        }
        else { triggerMakeCake(); }
    }
    else if (what == 'addCake') {
        if (randomObjectNames.length != 0 & (dontShowCakeWarning == false)) {
            $('#editor_delete_warning').fadeIn(300);
            $("#okWarning").attr("onclick", "triggerAdd()");
        }
        else { triggerAdd(); }
    }
}

function figureHeaderClick(what) {
    if (what == 'object') {
        $('#object2d-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#object-categories-wrapper').fadeIn(100); }, 110);
    }
    else if (what == 'object2d') {
        $('#object-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#object2d-categories-wrapper').fadeIn(100); }, 110);
    }
}

function decorationHeaderClick(what) {
    if (what == 'side') {
        $('#random-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#side-categories-wrapper').fadeIn(100); }, 110);
    }
    else if (what == 'random') {
        $('#side-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#random-categories-wrapper').fadeIn(100); }, 110);
    }
}

function reliefHeaderClick(what) {
    if (what == 'relief') {
        $('#decal-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#relief-categories-wrapper').fadeIn(100); }, 110);
    }
    else if (what == 'decal') {
        $('#relief-categories-wrapper').fadeOut(100);
        setTimeout(function () { $('#decal-categories-wrapper').fadeIn(100); }, 110);
    }
}

var currentStep = 1;

function editorStep(num) {
    if (cakeExists() == false & num != 1) {
    }
    else {
        $(".editor-objectbar-utilitydiv").slideUp(300);

        if (num != null) { currentStep = num; }

        $(".editorStep").hide();
        $("#canvas-upper-img1").attr("src", "Images/Site/canvas-upper1.png");
        $("#canvas-upper-img2").attr("src", "Images/Site/canvas-upper2.png");
        $("#canvas-upper-img3").attr("src", "Images/Site/canvas-upper3.png");
        $("#canvas-upper-img4").attr("src", "Images/Site/canvas-upper4.png");
        $("#canvas-upper-img5").attr("src", "Images/Site/canvas-upper5.png");

        $(".canvas-upper-label label,.canvas-upper-label h3").removeClass("editor-active-step");
        $(".canvas-upper-label:eq(" + (currentStep - 1) + ") label, .canvas-upper-label:eq(" + (currentStep - 1) + ") h3").addClass("editor-active-step");

        if (currentStep != 5) {
            $("#filter2D3D").hide(); $("#canvasNextStep").fadeIn(200); $("canvas,#canvas-container2").removeClass("disabled-link3");
        }

        if (currentStep == 1) {
            $("#canvas-upper-img1").attr("src", "Images/Site/canvas-upper1-active.png");
            $("#cake-categories-wrapper").fadeIn(200);
            TutorialManualCall('S3');
        }
        else if (currentStep == 2) {
            $("#canvas-upper-img2").attr("src", "Images/Site/canvas-upper2-active.png");
            $("#object-categories-wrapper,#filter2D3D").fadeIn(200);
            TutorialManualCall('S4');
        }
        else if (currentStep == 3) {
            $("#canvas-upper-img4").attr("src", "Images/Site/canvas-upper4-active.png");
            $("#side-categories-wrapper").fadeIn(200);
            TutorialManualCall('S6');
        }
        else if (currentStep == 4) {
            $("#canvas-upper-img3").attr("src", "Images/Site/canvas-upper3-active.png");
            $("#relief-categories-wrapper").fadeIn(200);
            TutorialManualCall('S7');
        }
        else if (currentStep == 5) {
            $("#canvas-upper-img5").attr("src", "Images/Site/canvas-upper5-active.png");
            $("#editor-order-wrapper").fadeIn(200);
            $("#canvasNextStep,#filter2D3D").hide();
            TutorialManualCall('S9');

            $("#pasta-notlari-dropdown").show(); $("#pastane-sec-dropdown").hide(); backToProvinceSec(); $("#design-name-input").val("");
            $("#pastaneSecimi").addClass("disabled-link2");
            $("#canvas-container2").addClass("disabled-link3");
            canvasNoClick();
            pasta.save({ isAuto: true });
        }

        if (currentStep != 5) { AutoSaveCake(); canvasAllowClick(); $("#pastaneSecWarning").slideUp(300); }
    }
}

function canvasAllowClick() {
    $("#canvas-mask").fadeOut(300);
    $("#canvas-mask").unbind("click");
}

function pastaneSecimiAnim() {
    $("#pastaneSecimi").removeClass("demo");
    setTimeout(function () {
        $("#pastaneSecimi").addClass("demo");
    }, 1);
    $("#pastaneSecimi").removeClass("demo");
    setTimeout(pastaneSecimiAnim, 2000)
}

function canvasNoClick() {
    $("#canvas-mask").fadeIn(300);
    $("#canvas-mask").click(function () {
        $("#canvasClickWarning").slideDown(200);
        setTimeout(function () { $("#canvasClickWarning").slideUp(200); }, 3000);
    });
}

function editorStepNext() {
    if (currentStep != 5) {
        if (currentStep == 1 & cakeExists() == false) {
        }
        else {
            currentStep = currentStep + 1;
            editorStep();
        }
    }
}

function editorStepPrev() {
    if (currentStep != 1) {
        currentStep = currentStep - 1;
        editorStep();
    }
}

function mobileArrangement() {
    // $(".draggable-div").css("max-height", $("#canvas-sizer").height() * 0.45 + "px");
    $(".cake-draggable, .draggable-div").css("max-height", ($("#canvas-sizer").height() - 150).toString() + "px");
    $(".draggable-div2").css("max-height", $("#canvas-sizer").height() * 0.50 + "px");
    $("#canvas-next button").text("Sonraki"); $("#canvas-prev button").text("Önceki");
    //  $(".draggable-div2").css("max-height", $("#canvas-sizer").height() - 500);
    $("#editor-sub-main").css("margin-top", "20%");
    $(".editor-objectbar-utilitydiv").css("max-height", $("#canvas-sizer").height() - 100);
    $("#designVisible-label").text("profilimde gözüksün");
    $("#utilitydiv-wrapper").css("max-height", ($("#canvas-left-side").height() - 55).toString() + "px");
}

function desktopArrangement() {
    $(".draggable-div").css("max-height", $("#canvas-sizer").height() - 250);
    $(".cake-draggable").css("max-height", $("#canvas-sizer").height() - 200);
    $(".draggable-div2").css("max-height", $("#canvas-sizer").height() - 220);
    $("#editor-sub-main").css("margin-top", "20%");
    $(".editor-objectbar-utilitydiv").css("max-height", ($("#canvas-left-side").height() - 50).toString() + "px");
    $("#utilitydiv-wrapper").css("max-height", ($("#canvas-left-side").height() - 150).toString() + "px");
}

function cakeExists() {
    var cakeExist = false;
    for (i = 0; i < objectsArray.length; i++) {
        if (objectsArray[i].indexOf("cake") > -1) { cakeExist = true; break; }
    }
    return cakeExist;
}

function orderDropdowns(step) {
    if (step == 1) {
        $("#pastane-sec-dropdown").slideUp(300);
        $("#pasta-notlari-dropdown").slideToggle(300);
    }
    else if (step == 2) {
        var dropdownValidated = true;
        if ($("#design-name-input").val().length == 0) { dropdownValidated = false; }
        if ($("#design-category-input").val() == null) { dropdownValidated = false; }
        else {
            if ($("#design-category-input").val().length == 0) { dropdownValidated = false; }
        }
        if (dropdownValidated == true) {
            $("#pasta-notlari-dropdown").slideUp(300);
            $("#pastane-sec-dropdown").slideToggle(300);
            $("#pastaneSecWarning").slideUp(300);
            TutorialManualCall('S11');
            $("#design-name-input,#design-category-input").css("border-color", "#e6e6e6");
        }
        else {
            $("#pastaneSecWarning").slideDown(300);
            if ($("#design-name-input").val().length == 0) { $("#design-name-input").css("border-color", "red"); }
            if ($("#design-category-input").val() == null) { $("#design-category-input").css("border-color", "red"); }
        }
    }
}

function editorBakeryAppend(provinceCode, provinceName) {
    console.log(provinceCode + " " + provinceName);
    $.ajax({
        url: "/Object/EditorBakeryAppend?ProvinceCode=" + provinceCode,
        type: 'POST',
        cache: false,
        success: function (result) {
            var result_json = JSON.parse(result);

            $(".pastane-sec-appended").empty();

            for (i = 0; i < result_json.length; i++) {
                var img = "Images/Bakery/" + result_json[i].Thumbnail;
                var onclick = "'" + result_json[i].BakeryID + "','" + result_json[i].Name + "'"; var S12 = "S12";
                $("#pastane-sec-step2").append('<div class="pastane-sec-appended" style="float:left; width:50%;"><img src="' + img + '" class="editorBakeryAppend" onclick="editorContentAppend(' + onclick + ');" /></div>');
            }

            $(".pastaneAppend:nth-child(1) label:nth-child(2)").text(provinceName);
            $(".pastaneAppend:nth-child(1)").slideDown(200);

            $("#pastane-sec-step1").slideUp(200);
            $("#pastane-sec-step2").slideDown(200);
        }
    });
}

function editorContentAppend(bakeryID, bakeryName) {
    selectedBakeryID = bakeryID;
    $.ajax({
        url: "/Object/EditorContentAppend?BakeryID=" + bakeryID,
        type: 'POST',
        cache: false,
        success: function (result) {
            var result_json = JSON.parse(result);

            $(".pastane-content-appended").empty();

            $("#pastane-sec-step3 select").append('<option class="pastane-content-appended" selected disabled>Seçiniz</option>');

            for (i = 0; i < result_json.length; i++) {
                $("#pastane-sec-step3 select").append('<option value=' + result_json[i].ID + ' class="pastane-content-appended">' + result_json[i].Inside + '</option>');
            }

            $(".pastaneAppend:nth-child(2) label:nth-child(2)").text(bakeryName);
            $(".pastaneAppend:nth-child(2)").slideDown(200);

            $("#pastane-sec-step2").slideUp(200);
            $("#pastane-sec-step3").slideDown(200);

            TutorialManualCall('S12');
        }
    });
}

function backToProvinceSec() {
    $(".pastaneAppend,#order-design,#editorDesignPrice").slideUp(200);
    $("#pastane-sec-step2,#pastane-sec-step3").slideUp(200);
    $("#pastane-sec-step1").slideDown(200);
    $(".pastane-sec-appended,#pastane-sec-step3 select").empty();
}

function backToPastaneSec() {
    $(".pastaneAppend:nth-child(2),#order-design,#editorDesignPrice").slideUp(200);
    $("#pastane-sec-step3").slideUp(200);
    $("#pastane-sec-step2").slideDown(200);
    $("#pastane-sec-step3 select").empty();
}

function validatePastaNotlari() {
    var pastaNotlariValidated = true;
    if ($("#design-name-input").val().length == 0) { pastaNotlariValidated = false; } else { $("#design-name-input").css("border-color", "#e6e6e6"); }
    if ($("#design-category-input").val() == null) { pastaNotlariValidated = false; }
    else {
        if ($("#design-category-input").val().length == 0) { pastaNotlariValidated = false; }
    }
    if (pastaNotlariValidated == true) {
        $("#design-name-input,#design-category-input").css("border-color", "#e6e6e6");
        $("#pastaneSecWarning").slideUp(300); $("#pastaneSecimi").removeClass("disabled-link2");
        if (designLoaded == true) { }
        else { editorBakeryAppend($('#editorCitySelect').val(), $('#editorCitySelect option:selected').text()); }
    } else { $("#pastaneSecimi").addClass("disabled-link2"); }
    //arrangeCompetititon();
}

function arrangeCompetititon() {
    if ($("#design-category-input").val() == "Yarışma Pastaları") {
        $("#design-note-input, #design-photos-label, #pastaneSecimi, #odemeAlinmazLabel").slideUp(200);
        $("#saveToCompetition").slideDown(200);
    }
    else {
        $("#design-note-input, #design-photos-label, #pastaneSecimi, #odemeAlinmazLabel").slideDown(200);
        $("#saveToCompetition").slideUp(200);
    }
}

/******************** General Functions ********************/

function isMobile() {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        return true;
    }
    else { return false; }
}

function orientationAlert() {
    var elem = "";
    if (window.orientation != undefined) { elem = window.orientation.toString(); } else if (window.screen.orientation !== undefined) { elem = window.screen.orientation.type; }

    if (elem.toLowerCase().indexOf("portrait") > -1 | elem == "0") {
        $(".loading").show();
        $(".loading").css("background", "rgba(0,0,0,1)");
        $("#loading-img").attr("src", "/Images/Site/orientation.png");
        $("#loading-img").css("width", "250px");
    }
    else {
        $(".loading").hide();
        $(".loading").css("background", "rgba(0,0,0,0.4)");
        $("#loading-img").attr("src", "/Images/Site/loadmore.gif");
        $("#loading-img").css("width", "40px");
    }
}

function opacityAlert(elem) {
    var el = $(elem);
    el.css("transition", "all 0.1s");
    setTimeout(function () { el.css("opacity", "0.5"); }, 0);
    setTimeout(function () { el.css("opacity", "1.0"); }, 100);
    setTimeout(function () { el.css("opacity", "0.5"); }, 200);
    setTimeout(function () { el.css("opacity", "1.0"); }, 300);
    setTimeout(function () { el.css("opacity", "0.5"); }, 400);
    setTimeout(function () { el.css("opacity", "1.0"); }, 500);
}

/******************** Social Functions ********************/

function shareSocial(csLogin, type) {
    if (checkLogin(csLogin)) {
        if (type == "facebook") {
            FB.ui({
                method: 'share',
                href: 'https://www.icaked.com/Editor?designID=' + globalDesignId.toLowerCase(),
            }, function (response) { });
            //window.open('https://www.facebook.com/sharer/sharer.php?s=100&p[url]='
            //    + encodeURIComponent('https://www.icaked.com/Editor?designID=' + globalDesignId.toLowerCase())
            //    + '&p[images][0]=' + encodeURIComponent('https://www.icaked.com/Images/MadeCakes/' + globalDesignId.toLowerCase()) + encodeURIComponent('/designImg_1.png')
            //    + '', 'mywin', 'left=' + ($(window).width() - 500) / 2 + ',top=' + ($(window).height() - 250) / 2 + ',width=500,height=250,toolbar=1,resizable=0'); return false;
        }
        else if (type == "twitter") { window.open('https://twitter.com/intent/tweet', 'mywin', 'left=' + ($(window).width() - 500) / 2 + ',top=' + ($(window).height() - 250) / 2 + ',width=500,height=250,toolbar=1,resizable=0'); return false; }
        else if (type == "googleplus") { }
        else if (type == "pinterest") { }
    }
}

function addOGMetaTag(id) {
    $('head').append('<meta property="og:image" content="http://www.icaked.com' + thumbURLFromID(id) + '" />');
    $('head').append('<meta property="og:image:secure_url" content="https://www.icaked.com' + thumbURLFromID(id) + '" />')
}

function thumbURLFromID(id) {
    return '/Images/MadeCakes/' + id.toLowerCase() + '/designImg_1.png';
}

/******************** Resimli Pasta Functions ********************/

/******************** Document/Window Functions ********************/

$(window).resize(function () {
    if (isMobile()) { screenScale = 0.9; } else { screenScale = 0.8; }
    $("#canvas-sizer").height($(window).height() * screenScale);

    if (isMobile() == true) {
        $("#canvas-sizer").css("width", "667px");
        mobileArrangement();
    }
    else {
        desktopArrangement();
    }
});

$(document).keydown(function (event) {
    //if (event.which === 46) {
    //    if (selectedObjName != undefined) {
    //        deleteObj(selectedObjName); $(".editor-objectbar-utilitydiv").fadeOut(0);
    //    }
    //}
    //if (event.ctrlKey && event.keyCode == 86) {
    //    copyObj(selectedObjName);
    //}
    //if (event.keyCode == 90 && event.ctrlKey) { $("#undo").trigger("click"); };
    //if (event.keyCode == 89 && event.ctrlKey) { $("#redo").trigger("click"); };
});

function canvasWidthHeight() {
    var width = $(window).width();
    var height = $(window).height();

    return (width > height) ? [width, height] : [height, width];
}

var mouseX; var mouseY;

$(document).ready(function () {
    view = "editor";

    var guid = getDesignID();
    var cWH = canvasWidthHeight();

    $(".editor-objectbar").click(function () {
        if ($(this).hasClass("disabled-link2") == false) {
            $(".editor-objectbar").not(this).slideToggle(300);
            toggleElement($(this).attr("data-category"), 300);
            $("." + $(this).attr("data-bars")).slideUp(200);
            $("#editor-main-search").slideToggle(300);
        }
    });

    $("#add-cake-category label").click(function () {
        $(".cake-dropdown").hide();
        $("#add-cake-category label").css("color", "#888888"); $("#add-cake-category label").css("border-color", "transparent");
        $(this).css("color", "black"); $(this).css("border-color", "gray");
        $("#" + $(this).attr("data-dropdown")).fadeIn(200);
    });

    $("#add-object-category label").click(function () {
        $(".object-dropdown").hide();
        $("#add-object-category label").css("color", "#888888"); $("#add-object-category label").css("border-bottom", "1px solid transparent");
        $(this).css("color", "black"); $(this).css("border-bottom", "1px solid gray");
        $("#" + $(this).attr("data-dropdown")).fadeIn(200);
    });

    $("#add-object2d-category label").click(function () {
        $(".object2d-dropdown").hide();
        $("#add-object2d-category label").css("color", "#888888"); $("#add-object2d-category label").css("border-bottom", "1px solid transparent");
        $(this).css("color", "black"); $(this).css("border-bottom", "1px solid gray");
        $("#" + $(this).attr("data-dropdown")).fadeIn(200);
    });

    $("canvas,#canvas-container,#canvas-container2").click(function () { if (selectedObjName == null) { $(".editor-objectbar-sub").hide(200); $(".editor-objectbar-utilitydiv,.editor-platebar").slideUp(400); } });

    $(function () {
        $('input').blur();
    });

    $("#size-slider-label").text("");
    initiateSizeSlider();

    $("#rotation-slider-label").text("0");
    initiateRotationSlider();

    $(".objectTextures").click(function () {
        var textureUrl = $(this).attr("src").replace("~", "");
        decal_idx++;
        pasta.addObject({ Name: "cake-" + (decal_idx), Properties: { scale: 1.0, transform: { position: { x: 0, y: 1.0, z: 0 } } }, Decal: true, Source: 'url:' + textureUrl + '' });
    });

    $("#paste").click(function () { copyObj(selectedObjName); });
    $("#delete").click(function () {
        if (selectedObjName != undefined) {
            deleteObj(selectedObjName); $(".editor-objectbar-utilitydiv").fadeOut(0);
        }
    });

    if (isMobile() == false) {
        //$("#canvas-sizer").on("mouseover", function () {
        //    $("html,body").addClass("noScroll");
        //});

        //$("#canvas-sizer").on("mouseleave", function () {
        //    $("html,body").removeClass("noScroll");
        //});
    }
    else {
        $("#editor-main-container").css("padding", "0px");
        //$("#canvas-sizer,canvas,#canvas-container").css("width", "100%");
        //$("#canvas-sizer,canvas,#canvas-container").css("height", "100%");
    }

    var drag_point; var isObjAdded = false;

    if (isMobile() == false) {
        $(".add-obj,.add-cake").draggable({
            opacity: 0.01,
            helper: "clone",
            start: function (event, ui) {
                $(ui.helper).width(0); selectedObjName = undefined;
                var offset = $(pasta.rendererDomElement()).parent().offset();
                pasta.leftClickEvent({ x: (event.pageX - offset.left), y: (event.pageY - offset.top) });
            },
            drag: function (event) {
                var pos = $(this).position(); var is2Dobj = false; var isMixed = false;
                var offset = $(pasta.rendererDomElement()).parent().offset();
                pasta.leftClickEvent({ x: (event.pageX - offset.left), y: (event.pageY - offset.top) }, selectedObjName);
                if (isObjAdded == false) {
                    if ($(this).attr("data-is2D") == "true") { is2Dobj = true; }
                    if ($(this).attr("data-ismixed") == "true") { isMixed = true; }
                    addObj($(this).attr("data-obj"), drag_point, { x: pos.left, y: pos.top }, is2Dobj, isMixed); isObjAdded = true;
                }
            },
            stop: function (event) {
                isObjAdded = false; drag_point = null;
            }
        });
    }

    $("#userDecal").change(function () {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#textureCarouselWrapper").hide();
            $("#userDecalWrapper").fadeIn(200);
        };
        reader.readAsDataURL(this.files[0]);
        uploadUserDecal();
    });

    $("#decal-add").click(function (event) {
        pasta.addObject({ Name: "cake-" + (text_idx++), Properties: { scale: 1.0, transform: { position: { x: 0, y: 1.0, z: 0 } } }, Decal: true, Source: 'url:/Images/02-very-happy-face.jpg' });
    });

    var isFullScreen = false;

    $("#fullscreen_icon").click(function () {
        isFullScreen = !isFullScreen;
        $("#canvas-sizer").toggleClass("fullScreen"); $("#canvas-sizer").resize();
        toggleFullScreen(document.body);
    });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            e.preventDefault();
            if (isFullScreen) { $("#canvas-sizer").toggleClass("fullScreen"); $("#canvas-sizer").resize(); isFullScreen = false; }
        }
    });

    window.addEventListener("load", function () {
        setTimeout(function () {
            window.scrollTo(0, 1);
        }, 0);
    });

    if (isMobile()) {
        $("html, body").animate({ scrollTop: $('#canvas-sizer').offset().top }, 100);
        $("#fullscreen_icon").trigger("click");
    }

    orientationAlert();

    $(window).on("orientationchange", function (event) {
        orientationAlert();
    })

    $(".add-cake,.add-obj").click(function () {
        var is2D = false; var allowChildren = false;
        if ($(this).attr("data-is2D") == "true") { is2D = true; }
        if ($(this).attr("data-isMixed") == "true") { allowChildren = true; }
        addObj($(this).attr("data-obj"), undefined, undefined, is2D, allowChildren);

        if (cakeExists) {
            if ($(this).hasClass("add-cake")) { QuestionProcess(2); }           
        }
    });

    $(".mobile-cake-up-btn").click(function () {
        $(this).parent().find(".cake-draggable").animate({ scrollTop: $('.cake-draggable').scrollTop() - 95 }, 300, "easeOutQuart");
    });

    $(".mobile-cake-down-btn").click(function () {
        $(this).parent().find(".cake-draggable").animate({ scrollTop: $('.cake-draggable').scrollTop() + 95 }, 300, "easeOutQuart");
    });

    $(".mobile-obj-up-btn").click(function () {
        $(this).parent().find(".obj-draggable").animate({ scrollTop: $('.obj-draggable').scrollTop() - 95 }, 300, "easeOutQuart");
    });

    $(".mobile-obj-down-btn").click(function () {
        $(this).parent().find(".obj-draggable").animate({ scrollTop: $('.obj-draggable').scrollTop() + 95 }, 300, "easeOutQuart");
    });

    $("#add-relief-input").keyup(function () {
        if ($("#add-relief-input").val().length == 0) { $("#add-relief-input").parent().css("border-color", "red"); }
        else { $("#add-relief-input").parent().css("border-color", "#e6e6e6"); }
    });

    $("#add-relief-button").click(function () {
        $("body").css("cursor", "progress");
        setTimeout(function () { $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link"); $("body").css("cursor", "default"); }, 10000);
        var that = this;
        if ($("#add-relief-input").val().length != 0) {
            $("#add-relief-input").parent().css("border-color", "#e6e6e6");
            $.ajax({
                url: "/Object/AddObject?ObjectName=&count=0",
                type: 'POST',
                cache: false,
                success: function (result) {
                    console.log("aaaa");
                    var posZ = 1.3 * pasta.plateScale();
                    pasta.addObject({ Name: result.EditorName, onTo: 'plate-0', Properties: { color: 0x84442b, transform: { position: { x: 0, y: 0.15, z: posZ } } }, isText3D: true, Font: parseInt($("#add-relief-font").val()), Source: $("#add-relief-input").val(), onTo: "plate-0", TextStyle: parseInt($("#add-relief-style").val()) });
                    $("body").css("cursor", "default"); $(".add-obj,.add-cake,.add-side,.add-random").removeClass("disabled-link");
                }
            });
        }
        else {
            $("#add-relief-input").parent().css("border-color", "red");
        }
    });

    $(".add-side").click(function (event) {
        $("body").toggleClass("wait");
        $.ajax({
            url: "/Object/AddObject?ObjectName=" + $(this).attr("data-obj") + "&count=0",
            type: 'POST',
            cache: false,
            success: function (result) {
                console.log(result);
                var color = 0xfaa719;
                if (typeof result.Properties.styles.color === "string")
                    color = parseInt(result.Properties.styles.color, 16);
                $.when(pasta.addObject({
                    Name: result.EditorName, onTo: "all", Properties: { color: color }, isSideDecoration: true, SideDecorDelta: 0.130, pieces: [result.Source], CoordSource: result.Properties.styles.coords, Done: function () {
                        console.log("Dona dona");
                        //pasta.addTexture("texture-19", result.EditorName, "/Images/3_lu_yan_sus_1_texture.png");
                    }
                })).then(function () { $("body").toggleClass("wait"); });

                if (result.TexturePath != "") { pasta.addTexture(result.EditorName + "-texture", result.EditorName, "/Objects/Textures/" + result.TexturePath); } else { pasta.addTexture(undefined, result.EditorName, undefined); }
                //$.when(pasta.addObject({ Name: result.EditorName, onTo: "all", Properties: { color: color }, isSideDecoration: true, pieces: [result.Source] })).then(function () { $("body").toggleClass("wait"); });
                QuestionProcess(5);
            }
        });
    });

    $(".add-random").click(function (event) {
        $("body").toggleClass("wait");
        var numOfRandomObjects = 10 * cakeSizes.length;
        $.ajax({
            url: "/Object/AddObject?ObjectName=" + $(this).attr("data-obj") + "&count=" + numOfRandomObjects,
            type: 'POST',
            cache: false,
            success: function (result) {
                var color = 0xffffff;
                if (typeof result.Properties.styles.color === "string")
                    color = parseInt(result.Properties.styles.color, 16);
                $.when(pasta.addRandomObject(result.EditorName, result.Source, "all", 10, { color: color, type: 2 })).then(function () { $("body").toggleClass("wait"); });
            }
        });
    });

    if ($("#design-saved-div").is(":visible")) {
        setTimeout(function () { $("#design-saved-div").fadeOut(500) }, 2000);
    }

    //$("#textureCarousel").owlCarousel({
    //    items: 1,
    //    pagination: false,
    //});

    $("#textureNext").click(function () {
        $("#textureCarousel").trigger('owl.next');
    });
    $("#texturePrev").click(function () {
        $("#textureCarousel").trigger('owl.prev');
    });

    //$("#colorsCarousel").owlCarousel({
    //    items: 1,
    //    pagination: false
    //});

    $("#colorNext").click(function () {
        $("#colorsCarousel").trigger('owl.next');
    });
    $("#colorPrev").click(function () {
        $("#colorsCarousel").trigger('owl.prev');
    });

    // $(window).resize();

    //$(".owl-carousel").data("owlCarousel").reinit();

    $("#add-side,#add-random,#add-image,.add-cake").tooltip();

    openCloseCakes(totalCakeSize());

    $("#editor-tutorial-wrapper *").addClass("tutorial");

    editorModeSelectTemp("order");  // daha sonra kaldırılabilir
    editorDesignTypeTemp("cake");

    $("#canvas-container2,#canvas-container").css("height", $(window).height() * screenScale + "px");

    if (isMobile() == true) { mobileArrangement(); }
    else { desktopArrangement(); }

    if (isMobile()) {
        if ($(window).width() < $(window).height()) {
            $("#canvas-container2").width(cWH[0] * 0.75);
        }
        $("#canvas-container2").height(cWH[1]);
        $("#canvas-container").width(cWH[0]);
        $("#canvas-container").height(cWH[1]);
        $("#canvas-container canvas").width(cWH[0]);
        $("#canvas-container canvas").height(cWH[1]);
    }

    pasta.init({ sizerId: "canvas-container2", containerId: "canvas-container", designId: guid, mobile: isMobile() });

    if (isMobile() == true) {
        var hammertime = new Hammer(document.getElementsByTagName("canvas")[0]);

        hammertime.get('pinch').set({ enable: true, direction: Hammer.DIRECTION_ALL });

        hammertime.on('pinch', function (ev) {
            if (selectedObjName == null) {
                pasta.pinch("pinch", ev);
            }
        });

        hammertime.on('pan', function (ev) {
            if (selectedObjName == null) {
                pasta.pan(ev.velocityX, ev.velocityY);
            }
        });
    }

    fillCategories();

    $("#editor-order-wrapper").css("max-height", $("#canvas-left-side").height() - 50 + "px");

    $("#design-photos-input").change(function () {
        $("#design-photos-thanks").text($(this)[0].files.length + " adet resim yüklendi.");
        if ($(this)[0].files.length > 0) { $("#design-photos-thanks").slideDown(200); }
        else { $("#design-photos-thanks").slideUp(200); }
    });

    smoothDrag();

    function changeTutorialVideo(idx, elem) {
        $(".tutorial_buttons").removeClass("tutorial-active");
        $(elem).addClass("tutorial-active");
        $(".tutorial_video_wrapper").fadeOut(100);
        var videos = $('.tutorial_video_wrapper');
        for (i = 0; i < videos.length; i++) { $('.tutorial_video_wrapper')[i].pause(); }
        setTimeout(function () { $(".tutorial_video_wrapper:nth-child(" + idx + ")").fadeIn(300); $('.tutorial_video_wrapper')[idx - 1].play(); }, 210);
    }

    function dontShowTutorial(elem) {
        if ($(elem).is(":checked")) {
            $.ajax({
                url: "/Object/DontShowTutorial",
                type: 'POST',
                cache: false,
                success: function (result) {
                }
            });
        }
    }

});

$(document).ready(function () {

    $("#editor-start-button").click(function () {
        QuestionProcess(1);
    });

});

function QuestionProcess(step) {
    if (step == 1) {
        changeEqLabel("Now please click on the cake base you want.");
    }
    else if (step == 2) {
        setTimeout(function () {
            changeEqLabel("Now click on your cake base and select color.");
        }, 1000);
    }
    else if (step == 3) {
        editorStepNext();
        $('.editor-objectbar-utilitydiv').slideUp(400);
        $("#eq_label").text("");
        var text = "Now click on the objects you want on your cake. When you are done click <b onclick='QuestionProcess(4)' style='cursor:pointer;'>here.</b>";
        $("#eq_label").fadeOut(250);
        setTimeout(function () { $("#eq_label").append(text); }, 251);
        setTimeout(function () { $("#eq_label").fadeIn(250); }, 252);
    }
    else if (step == 4) {
        $("#eq_label").empty();
        editorStepNext();
        changeEqLabel("Now click on the decoration you wish for your cake.");
    }
    else if (step == 5) {
        editorStepNext();
        $("#eq_label").text("");
        var text = "Here you can add text or image to your cake. When you are done click <b onclick='QuestionProcess(6)' style='cursor:pointer;'>here.</b>";
        $("#eq_label").fadeOut(250);
        setTimeout(function () { $("#eq_label").append(text); }, 251);
        setTimeout(function () { $("#eq_label").fadeIn(250); }, 252);
    }
    else if (step == 6) {
        $("#eq_label").empty();
        editorStepNext();
        changeEqLabel("This is the last step. After filling the forms choose bakery and content of your cake and your cake is ready !");
    }
    else if (step == 7) {

    }
    else if (step == 8) {


    }
}

function simulateClick(x, y) {
    var ev = document.createEvent("MouseEvent");
    var el = document.elementFromPoint(x, y);
    ev.initMouseEvent(
        "click",
        true /* bubble */, true /* cancelable */,
        window, null,
        x, y, 0, 0, /* coordinates */
        false, false, false, false, /* modifier keys */
        0 /*left*/, null
    );
    el.dispatchEvent(ev);
}

function changeEqLabel(text) {
    $("#eq_label").fadeOut(250);
    setTimeout(function () { $("#eq_label").text(text); }, 251);
    setTimeout(function () { $("#eq_label").fadeIn(250); }, 252);
}