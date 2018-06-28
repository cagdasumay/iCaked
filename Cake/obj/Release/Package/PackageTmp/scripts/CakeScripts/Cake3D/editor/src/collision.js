var THREE = require('three');
var config = require('./config');
var scene = require('./scene');
var objects = require('./objects');
var design = require('./design');
var debug = require('./debug');
var controls_ui = require('./controls_ui');

module.exports = {
    clicked: function (type, pos, selectedName) {
        var mouse = new THREE.Vector2();
        mouse.x = pos.x/config.size.x*2-1;
        mouse.y = -pos.y/config.size.y*2+1;
        return detectMouseIntersection(type, mouse, selectedName);
    },
    collidesWithObjectAt: function (obj, x, y, z) {
        return collidesWithXAt(obj, x, y, z, "object");
    },
    collidesWithCakeAt: function (obj, x, y, z) {
        return collidesWithXAt(obj, x, y, z, "cake");
    },
    collidesWithPlateAt: function (obj, x, y, z) {
        return collidesWithXAt(obj, x, y, z, "plate");
    },
    getMouseCoordinates: function () {
        var mouse = new THREE.Vector2();
        mouse.x = pos.x / config.size.x * 2 - 1;
        mouse.y = -pos.y / config.size.y * 2 + 1;
        return getMouseCoordinates(mouse);
    },
    mouseRelease: function () {
        for (k = 0; k < collidedList.length; k++) {
            var names = collidedList[k].split('*');
            removeFromCollidedList(names[0], names[1]);
            if (posBeforeCollide[names[0]] !== undefined) {
                objects.objects(names[0]).position.copy(
                    posBeforeCollide[names[0]]
                );
                posBeforeCollide[name[0]] = undefined;
            }
            if (posBeforeCollide[names[1]] !== undefined) {
                objects.objects(names[1]).position.copy(
                    posBeforeCollide[names[1]]
                );
                posBeforeCollide[name[1]] = undefined;
            }
        }
    },
    reset: function () {
        reset();
    }
}

var collidedList = [];
var posBeforeCollide = {};
var preRedColor = {};
var preRedColorSet = {};
var _posBeforCollide1;
var _posBeforCollide2;

function reset() {
    collidedList = [];
    posBeforeCollide = {};
    preRedColor = {};
    preRedColorSet = {};
    _posBeforCollide1;
    _posBeforCollide2;
}

function collidesWithXAt (obj, x, y, z, type) {
    _posBeforCollide1 = new THREE.Vector3().copy(obj.position);
    obj.position.x = x;
    obj.position.y = y;
    obj.position.z = z;
    var selectedBB = new THREE.Box3().setFromObject(obj);
    var otherBB;
    var selectedSizeX = selectedBB.size().x;
    var selectedSizeY = selectedBB.size().y;
    var selectedSizeZ = selectedBB.size().z;
    var safety = new THREE.Vector3(
        -selectedSizeX*0.1, selectedSizeY*0.1, -selectedSizeZ*0.1
    );
    selectedBB.expandByVector(safety);
    var designObjects = scene.designObjects();
    for (var k = 0; k < designObjects.length; k++) {
        otherBB = new THREE.Box3().setFromObject(designObjects[k]);
        var objType = designObjects[k].name.split("-")[0];
        var otherSizeX = otherBB.size().x;
        var otherSizeY = otherBB.size().y;
        var otherSizeZ = otherBB.size().z;
        safety = new THREE.Vector3(
            -otherSizeX*0.2, -otherSizeY*0.2, -otherSizeZ*0.2
        );
        otherBB.expandByVector(safety);

        if (
            collisionCondition(selectedBB, otherBB, obj.name, designObjects[k].name, type)
        ) {
            if (otherSizeX > selectedSizeX
                && otherSizeZ > selectedSizeZ && type === "cake") {
                continue;
            }
            debug.log("Collision detected!");
            // if (posBeforeCollide[obj.name] === undefined) {
            _posBeforCollide2 = new THREE.Vector3().copy(designObjects[k].position);
            addToCollidedList(obj.name, designObjects[k].name);
            // }
            return true;
        } else if (designObjects[k].name !== obj.name) {
            // if (posBeforeCollide[obj.name] !== undefined) {
            // posBeforeCollide[obj.name] = undefined;
            removeFromCollidedList(obj.name, designObjects[k].name);
            // }
        }
    }

    return false;
}

function collisionCondition (selectedBB, otherBB, selName, otherName, type) {
    var otherType = otherName.split("-")[0];
    var element = design.rootGroup().findElement(otherName);
    var selectedElement = design.rootGroup().findElement(selName);
    if (selectedElement.isText3D) return false;
    if (
        selectedBB.intersectsBox(otherBB)
            && selName !== otherName
            && !(element.hasElder(selName))
            && otherType === type
            && !element.object.pastaElement.allowChildren
    ) {
        return collisionConditionInner(selectedElement, selectedBB, element);
        return true;
    } else {
        return false;
    }
}

function collisionConditionInner (selectedElement, selectedBB, otherElement) {
    var selectedObject = selectedElement.object;
    var otherObject = otherElement.object;
    var selectedObjectPos = new THREE.Vector3().copy(selectedObject.position);
    // selectedObjectPos.y += 0.05;
    var rays = new Array();
    var rayCount = 8;
    var raycaster = new THREE.Raycaster();
    var sizeX = selectedBB.size().x;
    var sizeZ = selectedBB.size().z;
    var sizeY = selectedBB.size().y;
    var selectedRadius = 0.5*Math.sqrt(sizeX*sizeX+sizeZ*sizeZ);
    // var intersects = new Array();

    for (var l = 0; l < 3; l++) {
        selectedObjectPos.y = l*sizeY/2+0.05;
        for (var k = 0; k < rayCount; k++) {
            var angle = k/rayCount*2*Math.PI;
            ray = new THREE.Vector3(Math.cos(angle), 0, Math.sin(angle)).normalize();
            raycaster.set(selectedObjectPos, ray);

            var intersects = raycaster.intersectObject(otherObject, true);
            debug.log("collisionConditionInner");
            debug.log(selectedRadius);
            debug.log(intersects);
            if (intersects.length > 0 && intersects[0].distance < selectedRadius) {
                return true;
            }
        }
    }

    return false;
}

function isInCollidedList (name1, name2) {
    var index = collidedList.indexOf(name1 +"*"+ name2)
        +collidedList.indexOf(name2 +"*"+ name1)+1;

    return index;
}

function removeFromCollidedList (name1, name2) {
    debug.log("removeFromCollidedList: "+ name1 +" "+ name2);
    var index = isInCollidedList(name1, name2);
    debug.log("removeFromCollidedList index: "+ index);
    if (index > -1) {
        collidedList.splice(index, 1);
        var object1 = objects.objects(name1);
        var object2 = objects.objects(name2);
        object1.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                if (preRedColorSet[name1]) {
                    child.material.color = preRedColor[name1].clone();
                    preRedColorSet[name1] = false;
                }
            }
        });
        object2.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                if (preRedColorSet[name2]) {
                    child.material.color = preRedColor[name2].clone();
                    preRedColorSet[name2] = false;
                }
            }
        });
    }
}

function addToCollidedList (name1, name2) {
    debug.log("addToCollidedList: "+ name1 +" "+ name2);
    if ((isInCollidedList(name1, name2)) < 0) {
        collidedList.push(name1 +"*"+ name2);
        var object1 = objects.objects(name1);
        var object2 = objects.objects(name2);
        object1.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                if (preRedColorSet[name1] === false || preRedColorSet[name1] === undefined) {
                    preRedColor[name1] = child.material.color.clone();
                    preRedColorSet[name1] = true;
                    posBeforeCollide[object1.name] = new THREE.Vector3().copy(_posBeforCollide1);
                    child.material.color.r = 240;
                }
            }
        });
        object2.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                if (preRedColorSet[name2] === false || preRedColorSet[name2] === undefined) {
                    preRedColor[name2] = child.material.color.clone();
                    preRedColorSet[name2] = true;
                    posBeforeCollide[object2.name] = new THREE.Vector3().copy(_posBeforCollide2);
                    child.material.color.r = 240;
                }
            }
        });
    }
}

function getMouseCoordinates(mouse) {
    var raycaster = new THREE.Raycaster();
    var basePlane = scene.designs(scene.designIndex()).basePlane();
    raycaster.setFromCamera(mouse, scene.camera());
    var intersects = raycaster.intersectObject(basePlane);
    if (intersects[0] != undefined) {
        debug.log("X: " + intersects[0].point.x + " z: " + intersects[0].point.z);
        return intersects[0].point;
    }
}

function detectMouseIntersection(type, mouse, selectedName) {
    var raycaster = new THREE.Raycaster();
    var intersects;
    raycaster.setFromCamera(mouse, scene.camera());
    if (type === "cake" || type === "object" || type === "all" || type === "plate") {
        intersects = raycaster.intersectObjects(scene.designObjects(), true);
        debug.log(intersects);
        debug.log(type);
        debug.log("Intersected objects count: "+ intersects.length);
        if (type === "all") {
            return intersects[0];
        }
        var element = design.rootGroup().findElement(selectedName);
        for (k = 0; k < intersects.length; k++) {
            if (intersects[k].object.parent) {
                if (intersects[k].object.parent.name.split("-")[0] === type ||
                    intersects[k].object.parent.pastaElement.allowChildren) {
                    if (type === "cake" && intersects[k].object.parent.pastaElement.decal)
                        continue;
                    if (
                        (selectedName === undefined ||
                         selectedName !== intersects[k].object.parent.name) &&
                            !(element.isElder(intersects[k].object.parent.name))
                    ) {
                        if (Math.abs(intersects[k].face.normal.y+1) > 1e-2)
                            return intersects[k];
                    }
                }
            }
        }
    } else if (type === "plane") {
        debug.log("PLANE");
        var basePlane = scene.designs(scene.designIndex()).basePlane();
        intersects = raycaster.intersectObject(basePlane);
        debug.log("Intersected plane count: "+ intersects.length);
        if (intersects.length > 0) {
            return intersects[0];
        }
    } else if (type === "ui") {
        intersects = raycaster.intersectObjects(scene.uiElementMeshes());
        if (intersects.length > 0) {
            debug.log(intersects[0]);
            debug.log("Intersected objects count: "+ intersects.length);
            debug.log("Intersected objects count: "+ intersects[0].object.parent.name);
            return intersects[0];
        }
    }
}
