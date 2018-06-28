var THREE = require('three');
var init = require('./init');
var controls = require('./controls');
var collision = require('./collision');
var view = require('./view');
var config = require('./config');
var command = require('./command');
var debug = require('./debug');
var design = require('./design');
var objects = require('./objects');
var scene = require('./scene');
var textObject = require('./text');
var decalObjects = require('./decal_objects');

module.exports = {
    /// @function main.reloadEvents
    /// Deprecated
    reloadEvents: function () {
        init.eventLoader();
    },
    /// @function main.init
    /// Initializes the WebGL editor
    /// @param conf Configuration parameters, conf = { sizerId: div.id, containerId: div.id }
    init: function (conf) {
        init.init(conf);
    },
    /// @function main.addObject
    /// Adds a 3D object to the scene
    /// @param {String} type Type of object, <cake|plate|object>
    /// @param {Object} objArg Some properties of the new object (rotation, pos, etc.)
    addObject: function (objArg, type) {
        obj = parseObj(objArg);
        if (type === undefined) {
            type = obj.name.split("-")[0];
        }
        if (type === "cake") {
            controls.addCake(obj);
        } else if (type === "plate") {
            controls.addPlate(obj);
        } else if (type === "object") {
            controls.addObject(obj);
        }
    },
    changePlate: function (option) {
        controls.changePlate(option);
    },
    pinch: function (type, ev) {
        controls.pinch(type, ev);
    },
    pan: function(velocityX, velocityY){
        controls.pan(velocityX, velocityY);
    },
    removeObject: function (oname) {
        return controls.removeObject(oname);
    },
    addTexture: function (tname, oname, url) {
        controls.addTexture(tname, oname, url);
    },
    colorizeObject: function (name, url, colors) {
        controls.colorizeObject(name, url, colors);
    },
    setProperties: function (oname, properties) {
        controls.setGraphicalProperties(oname, properties);
    },
    previewChanges: function (oname, properties) {
        controls.previewChanges(oname, properties);
    },
    setPropertiesDecal: function (oname, properties, source) {
        controls.setGraphicalPropertiesDecal(oname, properties, source);
    },
    getMouseCoordinates: function () {
        return collision.getMouseCoordinates();
    },
    changePlaneTexture: function (url) {
        return command.changePlaneTexture(url);
    },
    rendererDomElement: function () {
        return view.renderer().domElement;
    },
    leftClickEvent: function (pos, name) {
        controls.leftClickEvent(pos, name);
    },
    addRandomObject: function (name, source, onTo, cnt, properties) {
        debug.log("main.addRandomObject");
        debug.log(properties);
        controls.addRandomObject2(name, source, onTo, cnt, properties);
    },
    save: function () {
        command.fadeRoom();
        design.saveMode();
        return design.rootGroup().findElement("main-0").toStringWrapper();
    },
    afterSave: function () {
        command.resetRoom();
        scene.camera().lookAt(new THREE.Vector3(0, 0.7, 0));
    },
    load: function (jStr) {
        design.rootGroup().findElement("main-0").fromString(jStr);
    },
    getPositionFromCake: function (name, sel, up) {
        return objects.getPositionFromCake(name, sel, up);
    },
    undo: function () {
        controls.undo();
    },
    redo: function () {
        controls.redo();
    },
    cameraPosition: function (pos) {
        view.cameraPosition(pos);

        // Render function
        // view.render();
    },
    setPlateSizeRange: function (min, max) {
        config.plateSize.min = min;
        if (typeof max !== "number") config.plateSize.max = 1.5*min;
        else config.plateSize.max = max;
        view.radius(15*min);
    },
    reset: function () {
        scene.reset();
        collision.reset();
        command.reset();
        design.reset();
        textObject.reset();
        decalObjects.reset();
        objects.reset();
        command.addDesignCommand().execute(0);
        controls.addPlate();
        view.renderer(true);
        debug.log(scene.get());
    },
	  plateScale: function () {
		    return objects.objects('plate-0').scale.x;
	  }
}

function parseObj (objArg) {
    var obj = {
        name: "",
        text: "",
        copy: undefined,
        properties: {
            color: 0xFFFFFF,
            rotationY: 0,
            scale: 1.0,
            texture: 0,
            transform: {}
        },
        screenpos: undefined,
        isText3D: false,
        isSideDecoration: false,
        onTo: undefined,
        pieces: undefined,
        Font: undefined,
        coordSource: undefined,
        allowChildren: undefined,
        alignNormal: undefined,
        Object2D: undefined,
        sideDecorDelta: null
    };
    $.each(objArg, function (key, value) {
        if (key === "Name") {
            obj.name = value;
        } else if (key === "Source") {
            obj.text = value;
        } else if (key === "CoordSource") {
            obj.coordSource = value;
        } else if (key === "Properties") {
            $.each(value, function (key2, valueInner) {
                debug.log(key2 +" "+ valueInner);
                obj.properties[key2] = valueInner;
            });
            debug.log(obj);
        } else if (key === "Copy") {
            obj["copy"] = value;
        } else if (key === "Decal") {
            obj["decal"] = value;
        } else if (key === "ScreenPos") {
            obj["screenpos"] = value;
        } else if (key === "isText3D") {
            obj["isText3D"] = value;
        } else if (key === "isSideDecoration") {
            obj["isSideDecoration"] = value;
        } else if (key === "onTo") {
            obj["onTo"] = value;
        } else if (key === "pieces") {
            obj["pieces"] = value;
        } else if (key === "Font") {
            obj["font"] = value;
        } else if (key === "AllowChildren") {
            obj["allowChildren"] = value;
        } else if (key === "AlignNormal") {
            obj["alignNormal"] = value;
        } else if (key === "Object2D") {
            obj["object2D"] = value;
        } else if (key === "TextStyle") {
            obj["textStyle"] = value;
        } else if (key === "SideDecorDelta") {
            obj["sideDecorDelta"] = value;
        } else if (key === "Done") {
            obj["done"] = value;
        }
    });
    // obj.properties.scale = objArg.Properties.scale;

    if (obj.allowChildren === undefined) {
        var objNameInfo = obj.name.split("-");
        if (objNameInfo[0] === "cake")
            obj.allowChildren = true;
        else
            obj.allowChildren = false;
    }

    if (obj.object2D === true) obj.alignNormal = true;
    if (obj.alignNormal === true) {
        obj.properties.transform.rotation = { x: -Math.PI/2, y: 0, z: 0 };
    }

    if (obj.screenpos !== undefined) {
		    // TODO: adjust 200
		    if (obj.screenpos.x > 200)
			      obj.properties["transform"] = {
				        position: { x: -10, y: 0, z: 0 }
			      };
    } else if (obj.properties.transform.position === undefined) {
        obj.properties["transform"] = {
            position: { x: 0, y: 0, z: 0 }
        };
    }
    obj.properties["decal"] = obj.decal;
    // if (config.touchScreen) {
    //     obj.properties.transform.position.x = -2.5;
    //     obj.properties.transform.position.y = 0;
    //     obj.properties.transform.position.z = -2.5;
    // }

    if (obj.isText3D) obj.name += "-3dtext"+ Date.now();

    return obj;
}

function size2Scale (size) {
    return 1/size;
}