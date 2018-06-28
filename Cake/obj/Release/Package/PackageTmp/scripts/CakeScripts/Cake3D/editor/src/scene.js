var THREE = require('three');
var OBJLoader = require('three-obj-loader');
OBJLoader(THREE);
var objects = require('./objects');
var design = require('./design');
var debug = require('./debug');
var config = require('./config');

module.exports = {
    init: function (size) {
        initScene(size);
        initLight();
    },
    get: function () {
        return scene;
    },
    camera: function () {
        return camera;
    },
    add: function (type, id) {
        return add(type, id);
    },
    addUI: function (uiEl) {
        // scene.add(uiEl.element);
        design.uiElements().push(uiEl);
        uiEl.element.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                design.uiElementMeshes().push(child);
            }
        });
        uiEl.update();
    },
    remove: function (type, id) {
        return remove(type, id);
    },
    removeUI: function () {
        for (k = 0; k < design.uiElements().length; k++) {
            if (design.uiElements()[k].element.name.split("_")[0] !== "ui-collision")
                design.uiElements()[k].parent.remove(design.uiElements()[k].element);
        }
        design.resetUIElements();
    },
    updateUI: function () {
        for (k = 0; k < design.uiElements().length; k++) {
            design.uiElements()[k].update();
        }
    },
    getUIParent: function (name) {
        for (k = 0; k < design.uiElements().length; k++) {
            if (design.uiElements()[k].element.name === name) {
                return design.uiElements()[k].parent;
            }
        }
    },
    designs: function (p) {
        return design.designs(p);
    },
    designIndex: function (k) {
        return design.designIndex(k);
    },
    designObjects: function () {
        return design.designObjects();
    },
    designMeshes: function () {
        return design.designMeshes();
    },
    uiElementMeshes: function () {
        return design.uiElementMeshes();
    },
    updateObject: function (oname) {
        objects.setGraphicalProperties(
            oname,
            design.designs(design.designIndex()).findByName(oname).properties
        );
    },
    getObject: function (oname) {
        return design.designs(design.designIndex()).findByName(oname);
    },
    updateDesignObject: function (object) {
        design.updateDesignObject(object);
    },
    rotateLight: function (angle) {
        directionalLight.position.set(
            5*Math.cos(angle), 12, 5*Math.sin(angle)
        );
    },
    cubeCamera1: function () {
        return cubeCamera1;
    },
    cubeCamera2: function () {
        return cubeCamera2;
    },
    reset: function () {
        reset();
    },
    setControlMethods: function (t) {
        addTexture = t;
    }
}

var SHADOW_MAP_WIDTH = 1024, SHADOW_MAP_HEIGHT = 1024;
var scene, camera;
var ambientLight, directionalLight;
var directionalLightRoom1, directionalLightRoom2;
var _designIndex = -1;
var _designs = new Array();
var _designObjects = new Array();
var _designMeshes = new Array();
var _uiElements = new Array();
var _uiElementMeshes = new Array();
var cubeCamera1, cubeCamera2;
var addTexture;

function reset() {
    for (k = scene.children.length-1; k >= 0; k--) {
        obj = scene.children[k];
        if (obj instanceof THREE.AmbientLight || obj instanceof THREE.DirectionalLight)
            continue;
        scene.remove(obj);
    }
    _designs = new Array();
    _designObjects = new Array();
    _designMeshes = new Array();
    _uiElements = new Array();
    _uiElementMeshes = new Array();
}

function initScene (size) {
    scene = new THREE.Scene();
    camera = new THREE.PerspectiveCamera(23, size.x / size.y, 0.001, 100 );
    // cubeCamera1 = new THREE.CubeCamera( 0.001, 100, 256 );
		// cubeCamera1.renderTarget.texture.minFilter = THREE.LinearMipMapLinearFilter;
		// scene.add( cubeCamera1 );
    // cubeCamera2 = new THREE.CubeCamera( 0.001, 100, 256 );
		// cubeCamera2.renderTarget.texture.minFilter = THREE.LinearMipMapLinearFilter;
		// scene.add( cubeCamera2 );
    // config.envMap = cubeCamera1.renderTarget.texture;
    camera.position.z = 5;
    camera.position.y = 2;
    camera.position.x = 5;
    camera.lookAt(new THREE.Vector3(0, 0, 0));
    camera.target = new THREE.Vector3();
}

function initLight () {
    ambientLight = new THREE.AmbientLight(0xFFFFFF, 0.50); // 0.7
    directionalLight1 = new THREE.DirectionalLight(0xFDB813, 0.15); // 0.15
    directionalLight1.position.set(-12, 12, 5); // (-3, 17, 5);
    directionalLight1.target.position.set(0, 0, 0);
    if (!config.mobile) {
        directionalLight1.castShadow = true;
        directionalLight1.shadow = new THREE.LightShadow(
            new THREE.PerspectiveCamera( 23, 1, 0.001, 100 )
        );
		    directionalLight1.shadow.mapSize.width = SHADOW_MAP_WIDTH;
		    directionalLight1.shadow.mapSize.height = SHADOW_MAP_HEIGHT;
    }
		// directionalLight.shadow.bias = 0;

    directionalLight2 = new THREE.DirectionalLight(0xFFFFFF, 0.1); // 0.75
    directionalLight2.position.set(20, 10, 0); // (0, 17, 5);
    directionalLight2.target.position.set(0, 0, 0);
    if (!config.mobile) {
        directionalLight2.castShadow = true;
        directionalLight2.shadow = new THREE.LightShadow(
            new THREE.PerspectiveCamera( 43, 1, 0.001, 100 )
        );
		    directionalLight2.shadow.mapSize.width = SHADOW_MAP_WIDTH;
		    directionalLight2.shadow.mapSize.height = SHADOW_MAP_HEIGHT;
    }
		// directionalLight.shadow.bias = 0;

    directionalLight3 = new THREE.DirectionalLight(0xFFFFFF, 0.35); // 0.75
    directionalLight3.position.set(-5, 10, 20); // (0, 17, 5);
    directionalLight3.target.position.set(0, 0, 0);

    ambientLight.name = "ambient-light";
    directionalLight1.name = "directional-light-1";
    directionalLight2.name = "directional-light-2";
    directionalLight3.name = "directional-light-3";

    scene.add(ambientLight);
    scene.add(directionalLight1);
    scene.add(directionalLight2);
    scene.add(directionalLight3);
}

// function initRoomLights () {
//     directionalLightRoom1 = new THREE.DirectionalLight
// }

function adjustCamera (e, vals) {
    if (e === 'zoom') {
    }
}

function updateDesignObject (object) {
    for (k = 0; k < _designObjects.length; k++) {
        if (_designObjects[k].name === object.name) {
            _designObjects.splice(k, 1);
            _designMeshes.splice(k, 1);
        }
    }
    _designObjects.push(object);
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                _designMeshes.push(child);
        }
    });
}

function add (type, obj) {
    var object = objects.objects(obj.name) || objects.objectsDecal(obj.name);
    if (object === undefined) return -1;
    debug.log("Scene: "+ obj.name +" "+ type);
    design.designObjects().push(object);
    // THREE.JS function
    scene.add(object);
    // =================
    if (obj.properties.color === undefined) obj.properties["color"] = 0xFFFFFF;
    if (obj.properties.scale === undefined) obj.properties["scale"] = object.scale.x;
    if (type === "cake") {
        design.designs(design.designIndex()).cakes({
            name: obj.name, // no need to hold source data
            properties: obj.properties, // only name and properties
            decal: obj.decal
        });
    } else if (type === "plate") {
        design.designs(design.designIndex()).plate({
            name: obj.name
        });
    } else if (type === "object") {
        design.designs(design.designIndex()).objects({
            name: obj.name,
            properties: obj.properties
        });
    }
    var designElement;
    if (type === "plate") {
        design.rootGroup().findElement("main-0").addObject(object);
        designElement = design.rootGroup().findElement(obj.name);
        designElement.properties = $.extend(true, {}, obj.properties);
    } else {
        var nullEl = design.rootGroup().findElement("null-0");
        debug.log(nullEl.child(obj.name));
        debug.log(obj.name);
        if (nullEl.child(obj.name) === undefined) {
            nullEl.addObject(object);
        }
        designElement = design.rootGroup().findElement(obj.name);
        designElement.properties = $.extend(true, {}, obj.properties);
        designElement.isText3D = obj.isText3D || false;
        if (designElement.isText3D === true) {
            designElement.text3D = obj.text;
        }
        designElement.textStyle = obj.textStyle || 0;
        designElement.alignNormal = obj.alignNormal || false;
        designElement.isCake = obj.isCake || false;
        if (designElement.alignNormal || designElement.isText3D) {
            designElement.pastaRotationAxis = { x: 0, y: 0, z: 1 };
            designElement.pastaRotateValue = 0;
            designElement.lookAtData = obj.lookAtData || {
                point: { x:0, y: 0, z: 0 },
                normal: { x:0, y: 1, z: 0}
            };
            if (designElement.alignNormal && !(typeof obj.copy === "string")) {
                designElement.object.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        child.geometry.rotateX(Math.PI/2);
                    }
                });
                designElement.object.lookAt(new THREE.Vector3(0, 1, 0).
                                            add(designElement.object.position));
            } else if (designElement.alignNormal && typeof obj.copy === "string") {
                designElement.object.lookAt(new THREE.Vector3(
                    designElement.lookAtData.normal.x,
                    designElement.lookAtData.normal.y,
                    designElement.lookAtData.normal.z
                ).add(designElement.object.position));
            }
        }
        designElement.isSideDecoration = obj.isSideDecoration || false;
        designElement.sideDecorDelta = obj.sideDecorDelta;
        designElement.isRandom = obj.isRandom || false;
        if (typeof obj.coordSource === "object") {
            designElement.coordSource = obj.coordSource;
        } else {
            designElement.coordSource = objects.parseCoordSource(obj.coordSource || "");
        }
        designElement.lookAtData = obj.lookAtData || {
            point: { x:0, y: 0, z: 0 },
            normal: { x:0, y: 1, z: 0}
        };
        for (var key in obj.colorize) {
            debug.log("addScene: Colorize"+ obj.colorize[key]);
        }

        designElement.colorize = {};

        for (var key in designElement.coordSource) {
            if (typeof obj.colorize === "object" && obj.colorize[key]) {
                designElement.colorize[key] = obj.colorize[key];
            } else {
                designElement.colorize[key] = 0xffffff;
            }
        }
        designElement.allowChildren = obj.allowChildren || false;
        designElement.textureURL = designElement.object.textureURL || "";
        if (typeof obj.copy === "string") {
            designElement.textureURL = obj.textureURL;
        }
        designElement.textureAlphaURL = obj.textureAlphaURL || "";

        if (obj.decal !== undefined) {
	          designElement.decal = obj.decal;
            designElement.autoAdd = obj.autoAdd || false;
        	  if (designElement.decal === true) {
				        designElement.text = obj.text || ":";
                designElement.decalTextureURL = parseDecalText(designElement.text).url;
			      }
		    }

        if (typeof obj.done === "function") {
            obj.done();
            // designElement.doneCallback = obj.done || "";
            // doneCallback(obj.name, obj.done);
        }
    }

    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            debug.log("object.travers mesh.");
            design.designMeshes().push(child);
        }
    });
    return 0;
}

function remove (type, name) {
    var object = objects.objects(name) || objects.objectsDecal(obj.name);
    if (object === undefined) return -1;
    // THREE.JS remove function
    scene.remove(object);
    // ========================
    for (var k = 0; k < design.designObjects().length; k++) {
        if (design.designObjects()[k].name === name) {
            design.designObjects().splice(k, 1);
        }
    }
    for (var k = design.designMeshes().length-1; k >= 0 ; k--) {
        var mesh = design.designMeshes()[k];
        if (mesh.parent.name === name) {
            design.designMeshes().splice(k, 1);
        }
    }
    design.rootGroup().swapElement(name, "null-0");
    if (type === "cake") {
        design.designs(design.designIndex()).removeCake(name);
        return 0;
    } else if (type === "object") {
        design.designs(design.designIndex()).removeObject(name);
        return 0;
    } else if (type === "plate") {
        if (design.designs(design.designIndex()).plate() !== undefined) {
            design.designs(design.designIndex()).deletePlate();
            return 0;
        } else {
            return -1;
        }
    } else if (type === "ui") {
        return 0
    }
}

function parseDecalText (data) {
    var dataArr = data.split(/:/g, 2);
    debug.log(dataArr);
    if (dataArr[0] === "url") {
        return { url: dataArr[1] }
    } else if (dataArr[0] === "text") {
        dataArr = data.split(/:/g, 4);
        return { font: dataArr[1], curveType: dataArr[2], color: dataArr[3], text: data.substring(dataArr[1].length+dataArr[2].length+dataArr[3].length+8) };
    } else {
		    return { url: "" };
	  }
}

function doneCallback (name, cmd) {
    if (typeof cmd !== "string") {
        debug.log("Scene doneCallback `cmd` is not a string.");
        return -1;
    }
    debug.log("Scene doneCallback run for: "+ name +" "+ cmd);
    var cmdArgs = cmd.split(/:/);

    if (cmdArgs[0] === "addTexture" && cmdArgs.length === 4) {
        addTexture(cmdArgs[1], cmdArgs[2], cmdArgs[3]);
    }
}