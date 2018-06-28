var THREE = require('three');
var scene = require('./scene');
var view = require('./view');
var objects = require('./objects');
var command = require('./command');
var design = require('./design');
var config = require('./config');
var collision = require('./collision');
var controls_ui = require('./controls_ui');
var debug = require('./debug');
var utils = require('./utils');

module.exports = {
    init: function (jqElement) {
        jqElement.on('mousedown', function (event) {
            currentRotationSpeed = view.rotationSpeed();
            view.rotationSpeed(0);
			      coords = getCursorPosition(view.renderer().domElement, event);
            if (event.button == 2 || event.button == 1) {
                var offset = $(this).parent().offset();
                firstClickPosRight = coords;
                rightClickDown = 1;
            } else if (event.button == 0) {
                var offset = $(this).parent().offset();
                firstClickPosLeft = coords;
                leftClickDown = 1;
                selectObject(event);
            }
        });
        jqElement.on('mouseup', function (event) {
            view.rotationSpeed(currentRotationSpeed);
            if (event.button === 2 || event.button == 1) {
                rightClickDown = 0;
            } else if (event.button === 0) {
                if (addCakeCommandGlobal !== null) {
                    design.deselect();
                    addCakeCommandGlobal = null;
                }

                if (addObjectCommandGlobal !== null) {
                    design.deselect();
                    addObjectCommandGlobal = null;
                }
                leftClickDown = 0;
                collision.mouseRelease();
                if (dragStart === 1) {
                    dragStart = 0;
                    replaceObjects();
                }
                if (rotateObjectStart === 1) {
                    rotateObjectStart = 0;
                    rotateObjectMove = 0;
                    rotateObjects();
                }
            }
        });
        jqElement.on('mousemove', function (event) {
            var offset = $(this).parent().offset();
            pos = getCursorPosition(view.renderer().domElement, event);
            if (rightClickDown) {
                rightClickEvent(pos);
            } else if (leftClickDown) {
                leftClickEvent(pos);
                // view.render();
            } else {
                mouseMoveEvent(pos);
            }
        });
        jqElement[0].addEventListener("touchstart", function (e) {
            config.touchScreen = true;
            mousePos = getTouchPos(jqElement[0], e);
            var touch = e.touches[0];
            var mouseEvent = new MouseEvent("mousedown", {
                clientX: touch.clientX,
                clientY: touch.clientY
            });
            jqElement[0].dispatchEvent(mouseEvent);
        }, false);
        jqElement[0].addEventListener("touchend", function (e) {
            var mouseEvent = new MouseEvent("mouseup", {});
            jqElement[0].dispatchEvent(mouseEvent);
        }, false);
        jqElement[0].addEventListener("touchmove", function (e) {
            var touch = e.touches[0];
            var mouseEvent = new MouseEvent("mousemove", {
                clientX: touch.clientX,
                clientY: touch.clientY
            });
            jqElement[0].dispatchEvent(mouseEvent);
        }, false);
        jqElement.mousewheel(function (event) {
            var d = event.deltaY;
            if (isNaN(d) || !isFinite(d)) return;
            radiusDelta = (d < 0) ? -0.4 : 0.4;
            view.radius(view.radius()-radiusDelta);
        });
        jqElement.mouseleave(function (event) {
            leftClickDown = 0;
            rightClickDown = 0;
        });
        $(window).resize(onContainerResize);

        function isMobile() {
            try { document.createEvent("TouchEvent"); return true; }
            catch (e) { return false; }
        }

        if (isMobile() == true) {
            $("#zoom-in").on('touchstart', function () { zoomInClick(true); });
            $("#zoom-out").on('touchstart', function () { zoomOutClick(true); });
            $("#rotate-y-1").on('touchstart', function () { rotateYClick1(true) });
            $("#rotate-y-2").on('touchstart', function () { rotateYClick2(true) });
            $("#rotate-x-1").on('touchstart', function () { rotateXClick1(true); });
            $("#rotate-x-2").on('touchstart', function () { rotateXClick2(true) });
        }
        else {
            $("#home-button").on('mousedown', homeButtonClick);
            $("#zoom-in").on('mousedown', function () { zoomInClick(false); });
            $("#zoom-out").on('mousedown', function () { zoomOutClick(false); });
            $("#rotate-y-1").on('mousedown', function () { rotateYClick1(false) });
            $("#rotate-y-2").on('mousedown', function () { rotateYClick2(false) });
            $("#rotate-x-1").on('mousedown', function () { rotateXClick1(false); });
            $("#rotate-x-2").on('mousedown', function () { rotateXClick2(false) });

            $("#zoom-in").on('mouseup mouseout', function (event) {
                clearInterval(zoomInDownTmr);
            });
            $("#zoom-out").on('mouseup mouseout', function (event) {
                clearInterval(zoomOutDownTmr);
            });
            $("#rotate-y-1").on('mouseup mouseout', function (event) {
                clearInterval(rotateY1DownTmr);
            });
            $("#rotate-y-2").on('mouseup mouseout', function (event) {
                clearInterval(rotateY2DownTmr);
            });
            $("#rotate-x-1").on('mouseup mouseout', function (event) {
                clearInterval(rotateX1DownTmr);
            });
            $("#rotate-x-2").on('mouseup mouseout', function (event) {
                clearInterval(rotateX2DownTmr);
            });
        }

        $("#home-button").on('mousedown', homeButtonClick);
        $("#undo").click(undoClick);
        $("#redo").click(redoClick);

        $("#rotation-toogle").on('click', function (event) {
            view.rotationSpeed(Math.abs(view.rotationSpeed()-0.001));
        });

        command.addDesignCommand().execute(0);
        addPlate();
        // $("#add-side-decor").click(function () {
        //     var name = "";
        //     if (design.selected()) name = design.selected().name;
        //     objects.addSideDecoration(name, "object-898-side", [ 0, 1, 2 ], function () {
        //         scene.add("object", { name: "object-898-side", properties: { }, isSideDecoration: true });
        //         design.rootGroup().swapElement("object-898-side", name);
        //         design.rootGroup().findElement("object-898-side").update();
        //     });
        // });
    },
    pinch: function (type, ev) {
        pinch(type, ev);
    },
    pan: function (velocityX, velocityY) {
        pan(velocityX, velocityY);
    },
    eventLoader: function () {
        eventLoader();
    },
    addCake: function (obj) {
        if (obj.properties.decal) {
            var selected = design.selected();
            if (selected === null || selected === undefined) {
                var topCake = null;
                var maxPosY = 0;
                design.rootGroup().findElement('plate-0').traverseCakes(function (cake) {
                    if (cake.object.position.y > maxPosY && !cake.decal) {
                        topCake = cake;
                        maxPosY = cake.object.position.y;
                    }
                });
                if (topCake) {
                    for (var child in topCake.children) {
                        var childElement = topCake.children[child];
                        if (childElement.decal) {
                            removeObject(childElement.name);
                        }
                    }
                }
            }
        }
        addCakeClickParser(obj);

        // Render function
        // view.render()
    },
    addPlate: function () {
        addPlate();
    },
    changePlate: function (option) {
        objects.changePlate(option);
        var plateElement = design.rootGroup().findElement("plate-0");
        plateElement.properties.shape = option;
    },
    addObject: function (obj) {
        if (obj.isSideDecoration) {
            var onToInfo = obj.onTo.split("-");
            if (onToInfo.length > 1 && onToInfo[0] === "cake") {
                var onToElement = design.rootGroup().findElement(obj.onTo);
                for (var key in onToElement.children) {
                    if (onToElement.children[key].isSideDecoration) {
                        removeObject(onToElement.children[key].name);
                    }
                }
                obj.selectParent = true;
                addObjectClickParser(obj);
            } else if (onToInfo.length === 1 && onToInfo[0] === "all") {
                var objInfo = obj.name.split("-");
                var objIndex = parseInt(objInfo[1]);
                design.rootGroup().findElement("plate-0").traverseCakes(function (cake) {
                    if (!cake) return;
                    debug.log("obj.isSideDecoration: all");
                    debug.log(cake.name);
                    for (var key in cake.children) {
                        if (cake.children[key].isSideDecoration) {
                            removeObject(cake.children[key].name);
                        }
                    }
                    objInfo[1] = ""+ objIndex++;
                    var objDumm = $.extend(true, {}, obj);
                    objDumm.name = objInfo.join("-");
                    objDumm.onTo = cake.name;
                    objDumm.selectParent = false;
                    addObjectClickParser(objDumm);
                });
            }
        } else {
            addObjectClickParser(obj);
        }
    },
    addTexture: function (tname, oname, url) {
	      var designElement = design.rootGroup().findElement(oname);
        designElement.textureURL = url || "";
        objects.objects(oname).textureURL = url || "";
        if (url !== undefined) {
            objects.loadTexture(url, tname, function () {
                // objects.setTexture(oname, tname);
                command.changePropertiesCommand({
                    name: oname,
                    properties: { texture: tname }
                }).execute(0);
            });
        } else {
            command.changePropertiesCommand({
                name: oname,
                properties: { texture: tname }
            }).execute(0);
        }
        // callObjectGetter(oname);
        // Render function
        // view.render()
    },
    setGraphicalProperties: function (oname, properties) {
        if (oname === "plate-0") return;
        if (objects.objects(oname) === undefined) return -1;
        var element = design.rootGroup().findElement(oname);
        if (properties.all === "side") {
            design.rootGroup().findElement("plate-0").traverseCakes(function (cake) {
                if (!cake) return;
                debug.log("obj.isSideDecoration: all");
                debug.log(cake.name);
                for (var key in cake.children) {
                    if (cake.children[key].isSideDecoration) {
                        command.changePropertiesCommand({
                            name: key, properties: properties
                        }).execute(0);
                    }
                }
            });
        } else {
            if (element.isRandom) {
                var changeCount = 0;
                design.rootGroup().traverseChildren(function (child) {
                    if (child.isRandom && child.randomObjectTeam === element.randomObjectTeam) {
                        properties.rotationY = child.properties.rotationY;
                        command.changePropertiesCommand({ name: child.name, properties: properties })
                            .execute(0);
                        changeCount++;
                    }
                });
                command.commandStepsAdjust(changeCount);
            } else if (element.isText3D || element.alignNormal) {
                if (properties.rotationY === undefined) {
                    properties.rotationY = element.pastaRotateValue;
                }
                command.changePropertiesCommand({ name: oname, properties: properties })
                    .execute(0);
            } else {
                command.changePropertiesCommand({ name: oname, properties: properties })
                    .execute(0);
            }
        }
        // Render function
        // view.render();
    },
    setGraphicalPropertiesDecal: function (oname, properties, source) {
        if (objects.objectsDecal(oname) === undefined) return -1;
        command.changePropertiesCommand({ name: oname, properties: properties, decal: true, source: source })
            .execute(0);
        // Render function
        // view.render();
    },
    removeObject: function (oname) {
        return removeObject(oname);
        // Render function
        // view.render();
    },
    previewChanges: function (oname, properties) {
        if (oname !== "plate-0") {
            var designElement = design.rootGroup().findElement(oname);
            properties.texture = designElement.properties.texture;
            objects.setGraphicalProperties(oname, properties);
            design.rootGroup().findElement(oname).updateScaleAndRotation(properties);
        } else {
            objects.setPlateGraphicalProperties(properties);
        }
        // Render function
        // view.render();
    },
    leftClickEvent: function (pos, name) {
        if (name == undefined) {
            design.deselect();
            return false;
        }
        var type = name.split("-")[0];
        if (design.selected() !== undefined) { design.select(type, name); }
        leftClickEvent(pos);
    },
    addRandomObject: function (source, onTo, cnt) {
        command.addRandomObjectCommand ({ source: source, onTo: onTo, count: cnt })
            .execute(0, function () { });
    },
    addRandomObject2: function (name, source, onTo, count, properties) {
        if (onTo === "all") {
            var objInfo = name.split("-");
            var objIndex = parseInt(objInfo[1]);
            design.rootGroup().findElement("plate-0").traverseCakes(function (cake) {
                if (!cake) return;
                objInfo[1] = ""+ objIndex;
                objIndex += count;
                name = objInfo.join("-");
                removeRandomObjects(cake.name);
                addRandomObject2(name, source, cake.name, count, $.extend(true, {}, properties), true);
            });
        } else {
            var designElement = design.rootGroup().findElement(onTo);
            if (!designElement.isCake) {
                debug.log("Cant add randart, no cake selected.")
                return;
            }
            removeRandomObjects(onTo);
            addRandomObject2(name, source, onTo, count, properties);
        }
        // Render function
        // view.render();
    },
    colorizeObject: function (name, imgURL, colors) {
        var designElement = design.rootGroup().findElement(name);
        var _colors = hex2rgba(colors);
        for (var key in designElement.colorize) {
            designElement.colorize[key] = colors[key];
        }
        designElement.textureAlphaURL = imgURL;
        objects.colorizeObject(name, imgURL, designElement.coordSource, _colors, function () {
            command.changePropertiesCommand({ name: name, properties: {
                texture: objects.getColorizedTexture(name)
            }}).execute(0);

            // Render function
            // view.render();
        });
    },
    undo: function () {
        undoClick();

        // Render function
        // view.render();
    },
    redo: function () {
        redoClick();

        // Render function
        // view.render();
    }
}

function getCursorPosition(canvas, event) {
    var rect = canvas.getBoundingClientRect();
    var x = event.clientX - rect.left;
    var y = event.clientY - rect.top;
    return { x: x, y: y };
}

var leftClickDown = 0, rightClickDown = 0;
var firstClickPosRight, firstClickPosLeft;
var rotateYDirection = 0, rotateYDirectionPre = 0;
var rotateXDirection = 0, rotateXDirectionPre = 0;
var posPre = { x: 0, y: 0 };
var cakeId = 1, objectId = 1;
var zoomInDownTmr, zoomOutDownTmr, rotateX1DownTmr, rotateX2DownTmr;
var rotateY1DownTmr, rotateY2DownTmr;
var dragStart = 0;
var currentRotationSpeed = 0;
var rotateObjectStart = 0, rotateObjectMove = 0;
var firstIntersectionPoint;
var firstRotationY = 0;
var addCakeCommandGlobal = null;
var addObjectCommandGlobal = null;
var preLookAtNormalY = 0;

var oldDist = 0;

function hex2rgba (colors) {
    var rgbaList = {};
    var c = 0;
    for (var item in colors) {
        c = colors[item];
        rgbaList[item] = 'rgba('+[(c>>16)&255, (c>>8)&255, c&255].join(',')+',1)';
    }

    return rgbaList;
}

function pinch(type, ev) {
    var temp = 0;
    if (ev.scale < 1) { temp = Math.abs(ev.scale); } else { temp = -Math.abs(ev.scale); }
    if (isNaN(temp) || !isFinite(temp)) return;
    view.radius(view.radius() + temp/10);
}

function pan(velocityX, velocityY) {
    if (isNaN(velocityX) || !isFinite(velocityX)) return;
    view.phi(view.phi() - velocityX/10);
    if (isNaN(velocityY) || !isFinite(velocityY)) return;
    view.psi(view.psi() + velocityY/10);
}

function getTouchPos(canvasDom, touchEvent) {
    var rect = canvasDom.getBoundingClientRect();
    return {
        x: touchEvent.touches[0].clientX - rect.left,
        y: touchEvent.touches[0].clientY - rect.top
    };
}

function eventLoader () {
    $(".add-cake").unbind('click', addCakeClickLocal);
    $(".add-cake").click(addCakeClickLocal);
}

var timestamp = null;
var lastMouseX = null;
var lastMouseY = null;
var phiTmrMove = null;

function rightClickEvent(pos) {
    //var e = event;
    //if (timestamp === null) {
    //    timestamp = Date.now();
    //    lastMouseX = e.screenX;
    //    lastMouseY = e.screenY;
    //    return;
    //}

    //var now = Date.now();
    //var dt = now - timestamp;
    //var dx = e.screenX - lastMouseX;
    //var dy = e.screenY - lastMouseY;
    //var speedX = Math.round(dx / dt * 100);
    //var speedY = Math.round(dy / dt * 100);

    //timestamp = now;
    //lastMouseX = e.screenX;
    //lastMouseY = e.screenY;

    //if (!isNaN(view.phi() - speedX / 10000)) {
    //    view.phi(view.phi() - speedX / 10000);
    //}
    //if (!isNaN(view.psi() + speedY / 10000)) {
    //    view.psi(view.psi() + speedY / 10000);
    //}

    //var counter = 0;
    //var phiTmrMove = setInterval(function () {
    //    if (!isNaN(view.phi() - speedX / 25000)) {
    //        view.phi(view.phi() - speedX / 25000);
    //    }
    //    if (!isNaN(view.psi() + speedY / 25000)) {
    //        view.psi(view.psi() + speedY / 25000);
    //    }
    //    counter++;
    //    if (counter == 5) { clearInterval(phiTmrMove); }
    //}, 50);

    deltaX = pos.x-firstClickPosRight.x;
    deltaY = pos.y-firstClickPosRight.y;

    if (deltaX === 0) { deltaX = 1; }
    if (deltaY === 0) { deltaY = 1; }

    if (posPre.y !== pos.y) {
        if (pos.y-posPre.y < 0) {
            rotateYDirection = -1;
        } else {
            rotateYDirection = 1;
        }
        posPre.y = pos.y;
    }

    if (posPre.x !== pos.x) {
        if (pos.x-posPre.x < 0) {
            rotateYDirection = -1;
        } else {
            rotateYDirection = 1;
        }
        posPre.x = pos.x;
    }

    if (rotateYDirectionPre !== rotateYDirection) {
        firstClickPosRight = { x: pos.x, y: pos.y };
        rotateYDirectionPre = rotateYDirection;
    }

    if (rotateXDirectionPre !== rotateXDirection) {
        firstClickPosRight = { x: pos.x, y: pos.y };
        rotateXDirectionPre = rotateXDirection;
    }
    if (isNaN(deltaY) || !isFinite(deltaY)) return;
    var psi = view.psi()+0.012*(deltaY/Math.abs(deltaY));
    psi = Math.max(-Math.PI, Math.min(Math.PI, psi));
    if (isNaN(deltaX) || !isFinite(deltaX)) return;
    var phi = view.phi()-0.014*(deltaX/Math.abs(deltaX));

    view.psi(psi);
    view.phi(phi);
    // view.render();
}

function selectObject (event) {
    event.preventDefault();
    objectCloser();
	  debug.log("selectObject event");
	  debug.log(design.selected());
    design.deselect();
    var intersects = collision.clicked("ui", firstClickPosLeft);
    // if (intersects !== undefined) debug.log("LLLLL: "+ intersects.object.parent.id);
    if (intersects === undefined) {
        intersects = collision.clicked("all", firstClickPosLeft);
    }
    if (intersects === undefined) {
        // scene.removeUI();
        return -1;
    }
    config.tmpIntersect = intersects;
    var mesh = intersects.object;
    var object = mesh.parent;
    //if (object.name.includes("plate")) { return; }
    // if (object === undefined) object = mesh;
    var tmp = object.name.split("-");
    var type = undefined;
    if (tmp.length > 1) {
        type = tmp[0];
    }
    debug.log("Selected: "+ object.name);
    if (type !== "ui") {
        // scene.removeUI();
    } else {
        // Get parent element of UI object
        // var parent = scene.getUIParent(object.name);
        // parentType = parent.name.split("-");
        // if (parentType.length > 1) {
        //     design.select(parentType[0], parent.name);
        // } else {
        //     return -1;
        // }
        // if (tmp[1] === "rotate") {
        //     rotateObjectStart = 1;
        // }
        // return 0;
    }
    design.select(type, object.name);
    var designObject = scene.getObject(object.name);
    // design.element
    var designElement = design.rootGroup().findElement(object.name);
    // console.log(designElement);
    if (designElement.decal !== true && designElement.name !== "plate") {
        var arrow = controls_ui.rotationArrow(object.name);
        // scene.addUI(arrow);
    }
    debug.log("objectGetter");
    // TODO: design.element
	  debug.log(designElement);
    callObjectGetter(object.name);
    // Render function
    // view.render();
}

function callObjectGetter (name) {
    debug.log("callObjectGetter run.");
    var designElement = design.rootGroup().findElement(name);
    var object = objects.objects(name);
    if (name === "plate-0") {
        var colorTmp = 0;
        for (var k = 0; k < object.children[0].material.materials.length; k++) {
            var m = object.children[0].material.materials[k];
            if (m.pastaColorable) {
                colorTmp = m.color.getHex();
            }
        }
    }

    if (designElement.isText3D || designElement.alignNormal) {
        designElement.properties.rotationY = designElement.pastaRotateValue;
    }

    var getterObject = {
        properties: $.extend(true, {}, designElement.properties),
        source: designElement.source,
        decal: designElement.decal,
        colorize: $.extend(true, {}, designElement.colorize),
        textureURL: designElement.textureURL
    };

    if (getterObject.properties.minSize === undefined) {
        if (name.indexOf("cake") < 0 || getterObject.decal) {
            getterObject.properties.minSize = "0.3";
        } else {
            getterObject.properties.minSize = "2";
        }
    }
    if (getterObject.properties.maxSize === undefined) {
        if (name.indexOf("cake") < 0 || getterObject.decal) {
            getterObject.properties.maxSize = "2.0";
        } else {
            getterObject.properties.maxSize = "70";
        }
    }
    if (getterObject.properties.stepSize === undefined) {
        if (name.indexOf("cake") < 0 || getterObject.decal) {
            getterObject.properties.stepSize = "0.1";
        } else {
            getterObject.properties.stepSize = "2";
        }
    }
    if (getterObject.properties.cakeSize === undefined) {
        if (name.indexOf("cake") < 0 && !getterObject.decal) {
        } else {
            getterObject.properties.cakeSize = "2";
        }
    }

    objectGetter(name, getterObject);
}

function propertiesPopup () {
    var selected = design.selected();
    var object = objects.objects(selected.name);
    var box = new THREE.Box3().setFromObject(object);

    // points that define shape
    var pts1 = [];
    var pts2 = [];
    var pts3 = [];
    var numPoints = 18;
    var radius = 0.5;

    for ( i = 0; i < numPoints+1; i ++ ) {
        if (i <= 6) {
            var a = Math.PI * i / numPoints;
            pts1.push( new THREE.Vector2 ( Math.cos( a ) * radius, Math.sin( a ) * radius ) );
            debug.log("pts1");
        } else if (i <= 12) {
            var a = Math.PI * i / numPoints;
            pts2.push( new THREE.Vector2 ( Math.cos( a ) * radius, Math.sin( a ) * radius ) );
            if (i === 7) pts1.push( new THREE.Vector2 ( Math.cos( a ) * radius, Math.sin( a ) * radius ) );
            debug.log("pts2");
        } else if (i <= 18) {
            var a = Math.PI * i / numPoints;
            pts3.push( new THREE.Vector2 ( Math.cos( a ) * radius, Math.sin( a ) * radius ) );
            if (i === 13) pts2.push( new THREE.Vector2 ( Math.cos( a ) * radius, Math.sin( a ) * radius ) );
            debug.log("pts3");
        }
    }
    pts1.push(new THREE.Vector2(0, 0));
    pts2.push(new THREE.Vector2(0, 0));
    pts3.push(new THREE.Vector2(0, 0));
    var pts = [pts1, pts2, pts3];
    var texture = new Array();
    var meshes = new Array();
    // shape to extrude
    for (k = 0; k < 3; k++) {
        var shape = new THREE.Shape(pts[k]);
        // extrude options
        var options = {
            amount: 0.05,              // default 100, only used when path is null
            bevelEnabled: false,
            bevelSegments: 2,
            steps: 1,                // default 1, try 3 if path defined
            extrudePath: null        // or path
        };

        // geometry
        var geometry = new THREE.ExtrudeGeometry( shape, options );
        texture.push(THREE.ImageUtils.loadTexture('/editor/objects/cake-icon.png'));
        texture[k].repeat.x = 3;
        texture[k].repeat.y = 3;
        if (k === 0) {
            texture[k].offset.x = 0.3;
            texture[k].offset.y = -0.1;
        } else if (k === 11) {
            // texture[k].offset.x = 0.5;
            // texture[k].offset.y = -0.2;
        }
        // mesh
        var mesh = new THREE.Mesh(
            geometry,
            new THREE.MeshBasicMaterial( { map: texture[k] } )
        );
        mesh.position.y = object.position.y+1;
        mesh.position.x = object.position.x;
        mesh.position.z = object.position.z;
        meshes.push(mesh);
        scene.get().add( mesh );
    }
}

function mouseMoveEvent (pos) {
}

/// Runs when moving mouse with left clicked
/// @param pos mouse position on canvas
function leftClickEvent (pos) {
    // A priority is defined for mouse intersectionPoint
    // most important is intersection with a cake
    // least important one is for base plane
    var selected = design.selected();
    var selectLevel = 0;
    if (selected === undefined || selected === null) { design.deselect(); return; }
    if (selected.name === "plate-0") { return; }
    var selectedObject = objects.objects(selected.name);
    if (selectedObject.pastaElement.allowChildren) {
        selectLevel = 2;
    } else {
        // Selecteds are objects
        selectLevel = 1;
    }

    if (rotateObjectStart === 1) {
        selectLevel = 3; // UI element selected
    }

    var intersects = undefined;
    var height = 0;
    var collisionLevel = 0;

    mouseIsOn = mouseHover(pos, selectLevel, selected.name);
    intersects = mouseIsOn.intersects;
    config.tmpIntersect = intersects;
    collisionLevel = mouseIsOn.collisionLevel || 0;
    // debug.log("collisionLevel: "+ collisionLevel);

    if (intersects === undefined) {
        if (addCakeCommandGlobal !== null) {
            var name = selected.name;
            var object = objects.objects(name);
            var camera = scene.camera();
            var mouse = new THREE.Vector2();
            mouse.x = pos.x / config.size.x * 2 - 1;
            mouse.y = -pos.y / config.size.y * 2 + 1;
            var vector = new THREE.Vector3(mouse.x, mouse.y, 0.5);
            vector.unproject(camera);
            var dir = vector.sub(camera.position).normalize();
            var distance = -camera.position.z / dir.z;
            var objPos = camera.position.clone().add(dir.multiplyScalar(distance));
            // debug.log("Positions");
            // debug.log(objPos);
            object.position.copy(objPos);
        }
        return -1;
    } else if (selectLevel === 1 || selectLevel === 2) {
        var mesh = intersects.object;
        var intersectedObject = mesh.parent;
        // var box = new THREE.Box3().setFromObject(intersectedObject);
        // var boxSelected = new THREE.Box3().setFromObject(
        //     objects.objects(selected.name)
        // );
        var name = selected.name;
        var selectedInfo = name.split("-");
        if (dragStart === 0) {
            var cmd = { objects: [] };
            if (selectedInfo.length === 3 && selectedInfo[2] === "side") return -1;
            var designElement = design.rootGroup().findElement(name);
            // TODO: design.element
            if (designElement.decal) {
                cmd.objects.push({
                    name: name,
                    prePosition: {
                        x: objects.objects(name).position.x,
                        y: objects.objects(name).position.y,
                        z: objects.objects(name).position.z
                    },
                    currentPosition: {
                        x: objects.objects(name).position.x,
                        y: objects.objects(name).position.y,
                        z: objects.objects(name).position.z
                    },
                    // TODO: design.element
                    decal: designElement.decal,
                    intersects: intersects
                });
            } else {
                cmd.objects.push({
                    name: name,
                    prePosition: {
                        x: objects.objects(name).position.x,
                        y: objects.objects(name).position.y,
                        z: objects.objects(name).position.z
                    },
                    currentPosition: {
                        x: objects.objects(name).position.x,
                        y: objects.objects(name).position.y,
                        z: objects.objects(name).position.z
                    },
                    prevParentName: designElement.parentName,
                    parentName: ""
                });
                if (designElement.alignNormal) {
                    cmd.objects[cmd.objects.length-1].preOrientation = {
                        x: designElement.object.rotation.x,
                        y: designElement.object.rotation.y,
                        z: designElement.object.rotation.z
                    };
                }
            }
            var moveObjectCommand = new command.moveObjectCommand(cmd);
            dragStart = 1;
        } else if (dragStart === 1) {
            var element = design.rootGroup().findElement(name);
            var swapFlag = 0;
            if (collisionLevel === 1) {
                // Intersects with plane
                height = 0;
                // debug.log("lala "+ name);
                if (!(element.hasParent("null-0"))) {
                    design.rootGroup().swapElement(name, "null-0");
                    debug.log("Parent change: null-0");
                }
            } else if (collisionLevel === 2 || collisionLevel === 3) {
                // Intersects with cake or plate
                height = intersects.point.y;
                if (!(element.hasParent(intersectedObject.name)) &&
                    !intersectedObject.pastaElement.decal) {
                    swapFlag = design.rootGroup().swapElement(name, intersects.object.parent.name);
                    debug.log("Parent change: "+ intersectedObject.name);
                } else {
                }
            }
            var object = objects.objects(name);
            var designObject = scene.getObject(object.name);
            // design.element
            var designElement = design.rootGroup().findElement(object.name);
            var collides = undefined;
            if (selectLevel === 1) { // selected is an object
                collides = collision.collidesWithObjectAt(
                    object,
                    intersects.point.x, height, intersects.point.z
                );
            } else if (selectLevel === 2) { // selected is a cake
                // debug.log("lalalala");
                debug.log(intersects);
                // TODO: design.element
                if (designElement.decal === true) {
                    var newDecal = objects.moveDecalTo(object, intersects);
                    scene.updateDesignObject(newDecal);
                } else {
                    collides = collision.collidesWithCakeAt(
                        object,
                        intersects.point.x, height, intersects.point.z
                    );
                }
            }
            element.update();
            // TODO: design.element
            if (collides || designElement.decal || swapFlag === -3) { return -1; }
            if (designElement.isText3D) {
                // TODO: Text3D move
                objects.moveText3D(object, intersects);
            } else {
                object.position.x = intersects.point.x;
                object.position.z = intersects.point.z;
                object.position.y = height;
                if (selectedInfo.length ===3 && selectedInfo[2] === "randart") {
                    var mY = new THREE.Matrix4().makeRotationY(intersectedObject.rotation.y);
                    var newNormal = intersects.face.normal.clone().applyMatrix4(mY);
                    var oldNormal = intersects.face.normal.clone();
                    var lookAtVec = intersects.point.clone().add(newNormal);
                    // var lookAtVecLocal = object.worldToLocal(lookAtVec);
                    if (Math.abs(newNormal.y-1) > 1e-2
                        || Math.abs(object.rotation.y) > 1e-2) {
                        object.lookAt(lookAtVec);
                        designElement.lookAtData.normal.x = oldNormal.x;
                        designElement.lookAtData.normal.y = oldNormal.y;
                        designElement.lookAtData.normal.z = oldNormal.z;
                    }
                    designElement.properties.transform.rotation = {
                        x: object.rotation.x,
                        y: object.rotation.y,
                        z: object.rotation.z
                    };
                    // designElement.properties.rotationY = object.rotation.y;
                    designElement.lookAtData.point.x = intersects.point.x;
                    designElement.lookAtData.point.y = intersects.point.y;
                    designElement.lookAtData.point.z = intersects.point.z;

                    designElement.lookAtData.dragged = true;
                } else if (object.pastaElement.alignNormal) {
                    var mY = new THREE.Matrix4().makeRotationY(intersectedObject.rotation.y);
                    var oldNormal = intersects.face.normal.clone();
                    var lookAtNormal = new THREE.Vector3().copy(
                        intersects.face.normal.clone().applyMatrix4(mY)
                    );
                    var lookAtVec = new THREE.Vector3().copy(lookAtNormal).add(intersects.point);
                    if (Math.abs(lookAtNormal.y-1) > 1e-2) {
                        object.lookAt(lookAtVec);
                        //var rotYTmp = object.rotation.y;
                        //var rotZTmp = object.rotation.x;
                        //var angleTmp = Math.atan2(rotYTmp, rotZTmp);
                        //if (rotYTmp < 0) angleTmp += Math.PI;
                        //else if (rotZTmp < 0) angleTmp += 2 * Math.PI;
                        //debug.log("atan2 1: " + angleTmp);
                        designElement.lookAtData.normal.x = oldNormal.x;
                        designElement.lookAtData.normal.y = oldNormal.y;
                        designElement.lookAtData.normal.z = oldNormal.z;
                        // object.pastaElement.pastaRotateValue = Math.atan2(
                        //     lookAtNormal.x, lookAtNormal.z
                        // );
                        // object.pastaElement.properties.rotationY = object.pastaElement.pastaRotateValue;
                        var rotTmp = object.worldToLocal(lookAtVec.clone()).normalize();

                        object.pastaElement.pastaRotationAxis.x = rotTmp.x;
                        object.pastaElement.pastaRotationAxis.y = rotTmp.y;
                        object.pastaElement.pastaRotationAxis.z = rotTmp.z;
                        // preLookAtNormalY = lookAtNormal.y;
                    } else {
                        lookAtNormal.y = 1.0;
                        lookAtNormal.x = 0;
                        lookAtNormal.z = 0;
                        lookAtVec = new THREE.Vector3().copy(lookAtNormal).add(intersects.point);
                        object.lookAt(lookAtVec);
                        //var rotYTmp = object.rotation.y;
                        //var rotZTmp = object.rotation.x;
                        //var angleTmp = Math.atan2(rotYTmp, rotZTmp);
                        //if (rotYTmp < 0) angleTmp += Math.PI;
                        //else if (rotZTmp < 0) angleTmp += 2 * Math.PI;
                        //debug.log("atan2 1: " + angleTmp);
                        designElement.lookAtData.normal.x = oldNormal.x;
                        designElement.lookAtData.normal.y = oldNormal.y;
                        designElement.lookAtData.normal.z = oldNormal.z;
                        // object.pastaElement.pastaRotateValue = Math.atan2(
                        //     lookAtNormal.x, lookAtNormal.z
                        // );
                        // object.pastaElement.properties.rotationY = object.pastaElement.pastaRotateValue;
                        var rotTmp = object.worldToLocal(lookAtVec.clone()).normalize();

                        object.pastaElement.pastaRotationAxis.x = rotTmp.x;
                        object.pastaElement.pastaRotationAxis.y = rotTmp.y;
                        object.pastaElement.pastaRotationAxis.z = rotTmp.z;
                        // preLookAtNormalY = lookAtNormal.y;
                    }
                    object.pastaElement.lookAtData.point.x = intersects.point.x;
                    object.pastaElement.lookAtData.point.y = intersects.point.y;
                    object.pastaElement.lookAtData.point.z = intersects.point.z;

                    if (object.pastaElement.pastaRotateValue < 0) {
                        object.pastaElement.pastaRotateValue += Math.PI*2;
                    }
                    object.pastaElement.properties.rotationY =
                        object.pastaElement.pastaRotateValue;
                }
            }
            if (collisionLevel === 2 || collisionLevel === 3) {
                // if (!(element.hasParent(intersectedObject.name))) {
                //   design.rootGroup().swapElement(name, intersects.object.parent.name);
                //   debug.log("Parent change: "+ intersectedObject.name);
                //   debug.log(design.rootGroup().findElement(name));
                // } else {
                // }
            } else if (collisionLevel === 1) {
            }
            // scene.updateUI();

            return 0;
        }
    } else if (selectLevel === 3 && rotateObjectStart === 1) {
        // UI element clicked and dragged
        if (rotateObjectMove === 0) {
            rotateObjectMove = 1;
            firstIntersectionPoint = {
                x: intersects.point.x,
                z: intersects.point.z
            };
            var name = selected.name;
            firstRotationY = objects.objects(name).rotation.y;
        } else if (rotateObjectMove === 1) {
            var name = selected.name;
            var object = objects.objects(name);
            var x1 = intersects.point.x-object.position.x;
            var x2 = firstIntersectionPoint.x-object.position.x;
            var z1 = intersects.point.z-object.position.z;
            var z2 = firstIntersectionPoint.z-object.position.z;
            var dot = x1*x2+z1*z2;
            var det = x1*z2-z1*x2;

            var angle = Math.atan2(det, dot);

            object.rotation.y = firstRotationY+angle;
        }
    }
}

function mouseHover (pos, selectLevel, selectedName) {
    var _intersects = undefined;
    var _collisionLevel = 0;

    if (selectLevel === 1 || selectLevel === 2) {
        _intersects = collision.clicked("cake", pos, selectedName); // checks allowChildren also
        _collisionLevel = 2;
        if (_intersects === undefined) {
            _intersects = collision.clicked("plate", pos, selectedName);
            _collisionLevel = 3;
            if (_intersects === undefined) {
                _intersects = collision.clicked("plane", pos, selectedName);
                _collisionLevel = 1;
            }
        }
    } else if (selectLevel === 3) {
        _intersects = collision.clicked("all", pos);
        _collisionLevel = 2;
        if (_intersects === undefined) {
            _intersects = collision.clicked("plane", pos);
            _collisionLevel = 1;
        }
        // _intersects = collision.clicked("plane", pos);
        // _collisionLevel = 1;
    }

    debug.log(selectLevel +" "+ selectedName +" "+ _collisionLevel);
    debug.log(_intersects);

    return { intersects: _intersects, collisionLevel: _collisionLevel }
}

function isSticky (selected, intersects) {
    return true;
    var sticks = scene.designs(scene.designIndex()).findByName(selected.name);
    debug.log("Sticks: "+ sticks.properties.Stick)
    if (sticks.properties.Stick === "z") {
        var face = intersects.face.normal;
        if (Math.abs(face.z-1) < 1e-3 || Math.abs(face.z+1) < 1e-3) {
            return true;
        } else {
            return false;
        }
    } else if (sticks.properties.Stick === "xy") {
        var face = intersects.face.normal;
        if (Math.abs(face.z) < 1e-3) {
            return true;
        } else {
            return false;
        }
    }
}

function replaceObjects () {
    var lastMoveObjectCommand = command.command();
    // var selected = design.selected();
    lastMoveObjectCommand.execute(0);
}

function rotateObjects () {
    var selected = design.selected();
    var cmd = { };
    var name = selected.name;
    cmd["name"] = name;
    cmd["properties"] = { rotationY: objects.objects(name).rotation.y };
    var changePropertiesCommand = new command.changePropertiesCommand(cmd);
    changePropertiesCommand.execute(0);
}

function removeObject (oname) {
    if (oname === "plate-0") return -1;
    var designObject = scene.designs(scene.designIndex()).findByName(oname);
    var designElement = design.rootGroup().findElement(oname);
    for (var child in designElement.children) {
        return -1;
    }
    if (designObject === undefined) return -1;
    command.removeObjectCommand({
        name: oname,
        prevParentName: designElement.parentName,
        parentName: "null-0"
    }).execute(0);
    // command.removeObjectCommand($.extend(true, {}, designElement)).execute(0);
    return 0;
}

function homeButtonClick() {
    var psiphiTmr; var radTmr;
    // radı ayrı yazdım çünkü psi phi bitiyor rad uzun süre devam ediyor. // üstteki ifler titreme olmasın diye ama silinse de çalışır.
    var phiLim = 0.01;
    var psiLim = 0.48;
    psiphiTmr = setInterval(function () {
        if (Math.abs(view.psi() - psiLim) > 0.01) {
            if (view.psi() - psiLim < 0) { var psi = view.psi() + 0.01; } else { var psi = view.psi() - 0.01; }
        }

        if (Math.abs(view.phi()) > 0.01) {
            if (view.phi() < 0) { var phi = view.phi() + 0.01; } else { var phi = view.phi() - 0.01; }
        }

        view.psi(psi); view.phi(phi);

        if (Math.abs(view.psi() - psiLim) < 0.01 & Math.abs(view.phi()) < 0.01) {
            clearInterval(psiphiTmr);
        }
    }, 4);

    radTmr = setInterval(function () {
        if (view.radius() - 9 < 0) { var rad = view.radius() + 0.05; } else { var rad = view.radius() - 0.05; }

        view.radius(rad);

        if (Math.abs(view.radius() - 9) < 0.05) {
            clearInterval(radTmr);
        }
    }, 1);
}

var isRotateY1TmrWorking = false;

function rotateYClick1(isMobile) {
    if (isMobile == false) {
        rotateY1DownTmr = setInterval(function () {
            view.psi(view.psi() + 0.005);
        }, 1);
    }
    else {
        if (isRotateY1TmrWorking == false) {
            var values = [0.0493, 0.0393, 0.0393, 0.0393, 0.0393, 0.0293, 0.0193, 0.0093, 0, 0];
            isRotateY1TmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.psi(view.psi() + values[t]);
                t += 1;
                if (t == 10) { isRotateY1TmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

var isRotateY2TmrWorking = false;

function rotateYClick2(isMobile) {
    if (isMobile == false) {
        rotateY2DownTmr = setInterval(function () {
            view.psi(view.psi() - 0.005);
        }, 1);
    }
    else {
        if (isRotateY2TmrWorking == false) {
            var values = [0.0493, 0.0393, 0.0393, 0.0393, 0.0393, 0.0293, 0.0193, 0.0093, 0, 0];
            isRotateY2TmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.psi(view.psi() - values[t]);
                t += 1;
                if (t == 10) { isRotateY2TmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

var isRotateX2TmrWorking = false;

function rotateXClick2(isMobile) {
    if (isMobile == false) {
        rotateX2DownTmr = setInterval(function () {
            view.phi(view.phi() + 0.005);
        }, 1);
    }
    else {
        if (isRotateX2TmrWorking == false) {
            var values = [0.0693, 0.0593, 0.0493, 0.0393, 0.0393, 0.0393, 0.0393, 0.0293, 0.0193, 0.0093];
            isRotateX2TmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.phi(view.phi() + values[t]);
                t += 1;
                if (t == 10) { isRotateX2TmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

var isRotateX1TmrWorking = false;

function rotateXClick1(isMobile) {
    if (isMobile == false) {
        rotateX1DownTmr = setInterval(function () {
            view.phi(view.phi() - 0.005);
        }, 1);
    }
    else {
        if (isRotateX1TmrWorking == false) {
            var values = [0.0693, 0.0593, 0.0493, 0.0393, 0.0393, 0.0393, 0.0393, 0.0293, 0.0193, 0.0093];
            isRotateX1TmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.phi(view.phi() - values[t]);
                t += 1;
                if (t == 10) { isRotateX1TmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

var isZoomOutTmrWorking = false;

function zoomOutClick(isMobile) {
    if (isMobile == false) {
        zoomOutDownTmr = setInterval(function () {
            view.radius(view.radius() + 0.02);
        }, 1);
    }
    else {
        if (isZoomOutTmrWorking == false) {
            var values = [0.14, 0.13, 0.10, 0.09, 0.1, 0.09, 0.08, 0.07, 0.06, 0.05, 0.04, 0.03];
            isZoomOutTmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.radius(view.radius() + values[t]);
                t += 1;
                if (t == 11) { isZoomOutTmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

var isZoomInTmrWorking = false;

function zoomInClick(isMobile) {
    if (isMobile == false) {
        zoomInDownTmr = setInterval(function () {
            view.radius(view.radius() - 0.02);
        }, 1);
    }
    else {
        if (isZoomInTmrWorking == false) {
            var values = [0.14, 0.13, 0.10, 0.09, 0.1, 0.09, 0.08, 0.07, 0.06, 0.05, 0.04, 0.03];
            isZoomInTmrWorking = true;
            var t = 0;
            tempTmr = setInterval(function () {
                view.radius(view.radius() - values[t]);
                t += 1;
                if (t == 11) { isZoomInTmrWorking = false; clearInterval(tempTmr); }
            }, 10);
        }
    }
}

function addTextureClick () {
}

// function addPlate () {
//     var addPlateCommand = command.addPlateCommand({
//         name: "plate-1",
//         url: "/editor/objects/dinner-plate.obj?dada"
//     });
//     addPlateCommand.execute(0, function () {
//         scene.add("plate", "plate-1");
//     });
// }

function addCakeClickParser(obj) {
    debug.log(obj);
    obj.isCake = true;
    debug.log(obj.properties.scale);
    if (obj.properties.decal === undefined) {
        obj["decal"] = false;
        design.deselect();
    }
    var plateElement = design.rootGroup().findElement('plate-0');
    var noCake = true;
    for (var child in plateElement.children) {
        if (plateElement.children[child].isCake) {
            noCake = false;
            break;
        }
    }
    if (noCake && obj.properties.transform.position.x > -9) {
        obj.properties.transform.position.x = 0;
        obj.properties.transform.position.z = 0;
        obj.properties.transform.position.y = 0.15;
        obj.onTo = 'plate-0';
    }
    addCakeCommandGlobal = command.addCakeCommand({
        name: obj.name,
        text: obj.text,
        copy: obj.copy,
        decal: obj.properties.decal,
        transform: {
            rotation: { x: 0, y: obj.properties.rotationY, z: 0 },
            scale: {
                x: obj.properties.scale,
                y: obj.properties.scale,
                z: obj.properties.scale
            },
            position: obj.properties.transform.position
        },
        prevParentName: obj.prevParentName || "null-0",
        parentName: obj.parentName || "null-0",
        onTo: obj.onTo,
        isCake: obj.isCake
    });
    addCakeCommandGlobal.execute(0, function (id) {
        if (typeof obj.copy === "string") {
            var desingObject = scene.getObject(obj.copy);
            // design.element
            var designElement = design.rootGroup().findElement(obj.copy);
            obj.properties = $.extend(true, {}, designElement.properties);
            obj.coordSource = designElement.coordSource;
            obj.isRandom = designElement.isRandom;
            obj.allowChildren = designElement.allowChildren;
        } else {
            if (obj.properties.rotationY === undefined) {
                obj.properties["rotationY"] = 0.0;
            }
            if (obj.properties.scale === undefined) {
                obj.properties["scale"] = 1.0;
            }
        }
        scene.add("cake", obj);

        if (typeof obj.onTo === "string") {
            design.rootGroup().swapElement(obj.name, obj.onTo);
        }

        // needed to overcome a bug occurs at copy object
        if (!obj.properties.decal) {
            objects.setGraphicalProperties(obj.name, obj.properties);
        } else if (obj.properties.decal === true) {
            var newDecal = objects.setGraphicalProperties(obj.name, obj.properties);
            scene.updateDesignObject(newDecal);
            design.rootGroup().swapElement(obj.name, design.selected().name);
            design.deselect();
        }
        // callObjectGetter(obj.name);
    });
    if (obj.screenpos !== undefined) {
        leftClickDown = 1;
        design.select("cake", obj.name);
    }
}

function addPlate () {
    objects.addPlate();
    scene.add("plate", { name: "plate-0", properties: { color: 0xcccccc, scale: 1.0, shape: 1 } });
    objects.setPlateGraphicalProperties({ color: 0xeeeeee, scale: 1.0 });
}

function addObjectClickParser (obj) {
    design.deselect();
    addObjectCommandGlobal = command.addObjectCommand({
        name: obj.name,
        text: obj.text,
        copy: obj.copy,
        transform: {
            rotation: obj.properties.transform.rotation || { x: 0, y: 0, z: 0 },
            position: obj.properties.transform.position,
            scale: {
                x: obj.properties.scale,
                y: obj.properties.scale,
                z: obj.properties.scale
            }
        },
        isText3D: obj.isText3D,
        isSideDecoration: obj.isSideDecoration,
        pieces: obj.pieces,
        onTo: obj.onTo,
        font: obj.font,
        textStyle: obj.textStyle || 0,
        prevParentName: obj.prevParentName || "null-0",
        parentName: obj.parentName || "null-0",
        coordSource: obj.coordSource || "",
        alignNormal: obj.alignNormal || false,
        normalScale: obj.properties.normalScale || 0.0,
        sideDecorDelta: obj.sideDecorDelta
    }).execute(0, function (id) {
        if (typeof obj.copy === "string") {
            var desingObject = scene.getObject(obj.copy);
            // design.element
            var designElement = design.rootGroup().findElement(obj.copy);
            debug.log("Copy object.");
            for (var key in designElement) {
                // debug.log(key);
                // debug.log(designElement[key]);
                if (key === "object") continue;
                if (key === "parent") continue;
                if (key === "name") continue;
                if (typeof designElement[key] === "function") continue;
                if (typeof designElement[key] === "object")
                    obj[key] = $.extend(true, {}, designElement[key]);
                else
                    obj[key] = designElement[key];
            }
            obj.allowChildren = designElement.allowChildren;
        } else {
            if (obj.properties.rotationY === undefined) {
                obj.properties["rotationY"] = 0.0;
            }
            if (obj.properties.scale === undefined) {
                obj.properties["scale"] = 1.0;
            }
        }
        debug.log(obj);
        scene.add("object", obj);

        if (obj.isSideDecoration || obj.isText3D) {
            design.rootGroup().swapElement(obj.name, obj.onTo);
            // TODO: assadaki gerekebilir
            // design.rootGroup().findElement(obj.name).update();
        } else if (typeof obj.onTo === "string") {
            design.rootGroup().swapElement(obj.name, obj.onTo);
        }

        objects.setGraphicalProperties(obj.name, obj.properties);
        // callObjectGetter(obj.name);
    });
    if (obj.screenpos !== undefined) {
        leftClickDown = 1;
        if (obj.isSideDecoration && obj.selectParent === true) {
            design.select("cake", obj.onTo);
            // callObjectGetter(obj.onTo);
        } else {
            design.select("object", obj.name);
        }
    } else {
        if (obj.isSideDecoration && obj.selectParent === true) {
            design.select("cake", obj.onTo);
            // callObjectGetter(obj.onTo);
        }
    }
}

function addCakeClickLocal (event) {
    cakeUUID = event.target.id.replace(/-/g, '.');
    debug.log(cakeUUID);

    id = "cake-"+ cakeId;
    cakeId++;
    addCakeCommand = command.addCakeCommand({
        id: id,
        url: config.OBJ_PATH +"/"+ cakeUUID +".obj"
    });
    addCakeCommand.execute(0, function () {
        scene.add("cake", id);
    });
}

function undoClick () {
    command.backward();
}

function redoClick() {
    command.forward();
}

function onContainerResize () {
	  config.size.x = config.sizer.width();
	  config.size.y = config.sizer.height();
	  config.xyRatio = config.size.y/config.size.x;
	  config.container.width(config.size.x);
	  config.container.height(config.size.y);
	  scene.camera().aspect = 1/config.xyRatio;
	  scene.camera().updateProjectionMatrix();
	  if (!config.mobile)
		    view.renderer().setSize(config.size.x, config.size.y);
}

function timerStop (tmr) {
    clearInterval(tmr);
}

function removeRandomObjects (onToName) {
    var onToObject = design.rootGroup().findElement(onToName);
    var k = 0;
    for (var child in onToObject.children) {
        var _c = onToObject.children[child];
        if (_c.isRandom) {
            removeObject(_c.name);
            k++;
        }
    }
    command.commandStepsAdjust(k);
}

function addRandomObject2(name, source, onToName, count, properties, v2) {
    // debug.log("firstId: "+ name);
    var firstId = parseInt(name.split("-")[1]);
    var onToObject = objects.objects(onToName);
    if (onToObject === undefined) return -1;
    var onToObjectPos = new THREE.Vector3().copy(onToObject.position);
    var bbOnToObject = new THREE.Box3().setFromObject(onToObject);
    onToObjectPos.y += bbOnToObject.size().y/2;
    var alpha = 0.0, theta = 0.0;
    var raycaster = new THREE.Raycaster();
    var direction = new THREE.Vector3(0, 1, 0);
    var randObjectPos = new THREE.Vector3();
    var meshDummy;
    var intersectsCount = 0;
    var psi = 0;
    var mod = 0;
    var typeProperty = properties.type;
    v2 = true;
	  var mod2 = 1;
    // Increas by cake scale
    count = Math.min(parseInt(count*onToObject.scale.x), count);
    count = Math.max(count, 1);
	  if (v2 === true) {
		    mod = count;
	  } else {
		    mod = parseInt((1+Math.sqrt(1+4*count))/2);
		    mod2 = mod;
	  }
    var permuteIndex1 = new Array(mod2);
    var permuteIndex2 = new Array(mod);
    // mod1 = 4;
    for (var l = 1; l < mod2+1; l++) {
        permuteIndex1[l-1] = l;
    }
    // mod = count/mod1;
    for (var l = 0; l < mod; l++) {
        permuteIndex2[l] = l;
    }
    var cart = utils.cartesian(permuteIndex1, permuteIndex2);
    // count = cart.length;
    debug.log(cart);
    permuteIndex1 = utils.shuffle(permuteIndex1);
    permuteIndex2 = utils.shuffle(permuteIndex2);
    debug.log("Permute index");
    debug.log(permuteIndex1);
    debug.log(permuteIndex2);
    mod -= 1;
    var perm0 = cart[0][0];
    var centerPsiOffset = 0;
    var addedCount = 0;
    for (var k = 0; k < count; k++) {
        var perm = cart[k];
        var randOffset = utils.randomReal(0.1, 0.3);
        var centerYMag = 0.3;
        if (!v2) {
            centerYMag = 1.25;
        }
        var centerY = 0; // centerYMag*perm[0]/mod2+randOffset;
        var halfMod = parseInt(mod/2);
        var centerPsi = perm[1]/(mod+1)*Math.PI*2;
        debug.log("centerPsi: "+ centerPsi);
        // direction.y =  randOffset*randomReal(permuteIndex1[k]/count, (permuteIndex1[k]+1)/count);
        // TODO: select coeff below
        direction.y = Math.max(0.0, utils.randomReal(centerY-15e-2, centerY+15e-2)*0.5);
        // psi = randomReal(permuteIndex2[k]/count*Math.PI*2, (permuteIndex2[k]+1)/count*Math.PI*2)+randOffset;
        // direction.x = randomReal(2*permuteIndex2[k]/count-1, 2*(permuteIndex2[k]+1)/count-1);
        // direction.z = randomReal(2*permuteIndex3[k]/count-1, 2*(permuteIndex3[k]+1)/count-1);
        var centerPsiRand = 0;
        if (v2 === true) {
            centerPsiRand = 0;
        } else {
            centerPsiRand = 0; // utils.randomReal(-Math.PI/4, Math.PI/4);
        }
        if (perm[0] !== perm0) {
            centerPsiOffset = utils.randomReal(Math.PI/2, 3*Math.PI/2);
            perm0 = perm[0];
            debug.log("centerPsiOffset: "+ centerPsiOffset);
        }
        // centerPsiRandOffset = 0;
        psi = centerPsi+centerPsiOffset+centerPsiRand;
        direction.x = Math.cos(psi);
        direction.z = Math.sin(psi);
        // direction.x = Math.cos(psi);
        // direction.z = Math.sin(psi);
        debug.log("Random dir: "+ centerY +" "+ centerPsi);
        raycaster.set(onToObjectPos, direction.normalize());
        var intersects = raycaster.intersectObject(onToObject, true);
        intersectsCount = intersects.length;
        if (intersectsCount > 0) {
            var mY, newNormal, surfaceAngle, oldNormal, lookAtVec;

            if (typeProperty === 2) {
                var newDir = new THREE.Vector3(0, 1, 0);
                newDir.add(direction.clone().multiplyScalar(-1)).normalize();
                raycaster.set(intersects[intersectsCount-1].point, newDir);
                intersects = raycaster.intersectObject(onToObject, true);
                intersectsCount = intersects.length;
            }

            mY = new THREE.Matrix4().makeRotationY(onToObject.rotation.y);
            newNormal = intersects[intersectsCount-1].face.normal.clone().applyMatrix4(mY);
            surfaceAngle = newNormal.angleTo(new THREE.Vector3(
                newNormal.x, 0, newNormal.z
            ));
            if (surfaceAngle > Math.PI) continue;
            oldNormal = intersects[intersectsCount-1].face.normal.clone();
            lookAtVec = intersects[intersectsCount-1].point.clone().add(newNormal);
            var randArtName = "object-"+ (firstId+k) +"-randart";
            addObjectClickParser({
                name: randArtName,
                text: source,
                isRandom: true,
                properties: {
                    color: properties.color || 0xffffff,
                    transform: {
                        position: {
                            x: intersects[intersectsCount-1].point.x,
                            y: intersects[intersectsCount-1].point.y,
                            z: intersects[intersectsCount-1].point.z
                        },
                        rotation: {
                            x: 0, //meshDummy.rotation.x,
                            y: 0, //meshDummy.rotation.y,
                            z: 0 //meshDummy.rotation.z
                        }
                    },
                    scale: 1.0
                },
                prevParentName: "null-0",
                parentName: onToName
            });
            addedCount++;
            var randArtObject = objects.objects(randArtName);
            randArtObject.children[0].geometry.rotateX(Math.PI/2);
            // debug.log("addRandomObject2: up");
            // debug.log(randArtObject.up);
            // randArtObject.up.set(1, 0, 0);
            // randArtObject.updateMatrix();
            // randArtObject.updateMatrixWorld();
            // debug.log(randArtObject.up);
            // randArtObject.children[0].rotation.x = Math.PI/2;
            debug.log("randart-"+ k);
            debug.log(intersects[intersectsCount-1].point);
            debug.log(intersects[intersectsCount-1].face.normal);
            randArtObject.lookAt(lookAtVec);
            design.rootGroup().swapElement(randArtName, onToName);
            var element = design.rootGroup().findElement(randArtName);
            element.properties.transform.rotation = {
                x: randArtObject.rotation.x,
                y: randArtObject.rotation.y,
                z: randArtObject.rotation.z
            };
            element.lookAtData = {
                point: {
                    x: intersects[intersectsCount-1].point.x,
                    y: intersects[intersectsCount-1].point.y,
                    z: intersects[intersectsCount-1].point.z
                },
                normal: {
                    x: oldNormal.x,
                    y: oldNormal.y,
                    z: oldNormal.z
                },
                dragged: false
            };
            element.properties.rotationY = randArtObject.rotation.y;
            element.randomObjectTeam = config.randomObjectTeam;
        }
    }
    config.randomObjectTeam++;
    command.commandStepsAdjust(addedCount);
}