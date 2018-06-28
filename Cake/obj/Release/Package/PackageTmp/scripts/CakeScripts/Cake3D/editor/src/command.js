var THREE = require('three');
var objects = require('./objects');
var design = require('./design');
var scene = require('./scene');
var debug = require('./debug');
var text = require('./text');
var view = require('./view');
var config = require('./config');
// var MTLLoader = require('three-mtl-loader');
// var _OBJLoader = require('three-obj-loader');
//var hdrLoader = require('./hdr');
var  mtlLoader = require('./mtl_loader');

module.exports = {
    command: function (cmd) {
        if (cmd !== undefined) {
            return new Command(cmd);
        } else {
            return commandList[commandList.length-1];
        }
    },
    addDesignCommand: function () {
        var _cmd = new AddDesignCommand();
        return _cmd;
    },
    selectDesignCommand: function (k) {
        var _cmd = new AddDesignCommand();
        addToCommandList(_cmd);
        return _cmd;
    },
    design: function () {
        if (scene.designIndex() !== -1) {
            return scene.designs(scene.designIndex());
        }
    },
    addPlateCommand: function (cmd) {
        var _cmd = new AddPlateCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    addCakeCommand: function (cmd) {
        var _cmd = new AddCakeCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    addObjectCommand: function (cmd) {
        var _cmd = new AddObjectCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    moveObjectCommand: function (cmd) {
        var _cmd = new MoveObjectCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    addRandomObjectCommand: function (cmd) {
        var _cmd = new AddRandomObjectCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    removeObjectCommand: function (cmd) {
        var _cmd = new RemoveObjectCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    changePropertiesCommand: function (cmd) {
        var _cmd = new ChangePropertiesCommand(cmd);
        addToCommandList(_cmd);
        return _cmd;
    },
    forward: function () {
        debug.log("forward: Command list length: "+ commandList.length +" "+ commandAddress);
        // debug.log(commandList);
        // debug.log(commandSteps);
        var cs = 1;
        commandStepIndex++;
        commandStepIndex = Math.min(commandStepIndex, commandSteps.length-1);
        if (commandStepIndex >= 0) cs = commandSteps[commandStepIndex];
        for (var k = 0; k < cs; k++) {
            if (commandList.length-1 > commandAddress && -1 <= commandAddress) {
                commandAddress++;
                commandList[commandAddress].execute(1);
            }
        }
    },
    backward: function () {
        debug.log("backward: Command list length: "+ commandList.length +" "+ commandAddress);
        // debug.log(commandList);
        // debug.log(commandSteps);
        var cs = 1;
        if (commandStepIndex >= 0) cs = commandSteps[commandStepIndex];
        for (var k = 0; k < cs; k++) {
            if (commandAddress >= 0 && commandList.length >= commandAddress) {
                if (commandList[commandAddress].unexecute() === 0) {
                    commandAddress--;
                }
            }
        }
        commandStepIndex--;
        commandStepIndex = Math.max(commandStepIndex, -1);
    },
    changePlaneTexture: function (url) {
        return changePlaneTexture(url);
    },
    commandStepsAdjust: function (count) {
        debug.log("commandStepsAdjust: "+ commandStepIndex);
        debug.log(commandSteps);
        commandSteps.splice(commandStepIndex-count+1, count-1);
        commandSteps[commandSteps.length-1] = count;
        commandStepIndex -= count-1;
        debug.log(commandStepIndex);
        debug.log(commandSteps);
    },
    fadeRoom: function () {
        setRoomColor(0x1c2a2a);
    },
    resetRoom: function () {
        setRoomColor(0xffffff);
    },
    reset: function () {
        reset();
    },
}

var commandList = new Array();
var commandSteps = new Array();
var commandAddress = -1;
var commandStepIndex = -1;

function reset() {
    commandList = new Array();
    commandSteps = new Array();
    commandAddress = -1;
    commandStepIndex = -1;
    addBasePlane();
    if (!config.mobile) addRoomMtl();
}

function addToCommandList (cmd) {
    commandAddress++;
    if (commandAddress >= 0 && commandAddress < commandList.length) {
        commandList[commandAddress] = cmd;
        // commandSteps[commandStepIndex] = 1;
        commandList.splice(commandAddress+1, commandList.length-(commandAddress+1));
    } else if (commandAddress >= commandList.length) {
        commandList.push(cmd);
        commandSteps.push(1);
    }

    commandStepIndex++;
    if (commandStepIndex >= 0 && commandStepIndex < commandSteps.length) {
        commandSteps[commandStepIndex] = 1;
        commandSteps.splice(commandStepIndex+1, commandSteps.length-(commandStepIndex+1));
    } else if (commandStepIndex >= commandSteps.length) {
        commandSteps.push(1);
    }
}

var Command = function (cmd) {
    this.cmd = cmd;
}

Command.prototype = {
    /// Executes a Command
    /// @param redo executes as a redo operation if it is set to 1, used in some cases
    execute: function (redo) {
        // debug.log("Command: "+ this.cmd);
        return this;
    },
    unexecute: function () {
        return this;
    },
    done: function () { }
}

var AddCakeCommand = function (cmd) {
    this.cmd = cmd;
}

AddCakeCommand.prototype = Object.create(Command.prototype);

AddCakeCommand.prototype.execute = function (redo, done) {
    debug.log("AddCakeCommand");
    if (!redo) {
        if (done !== undefined) { this.done = done; }
        if (this.cmd.copy === undefined) {
            if (!this.cmd.decal) {
                debug.log(this.cmd.transform.scale);
                objects.objectParser(this.cmd.text, this.cmd, this.done, this.cmd.transform);
            } else {
                var selected = null;
                if (redo === 0) {
                    selected = design.selected();
                    this.cmd["selected"] = selected;
                } else {
                    selected = this.cmd.selected;
                }
                if (selected !== null && selected.type === "cake") {
                    objects.addObjectDecal(this.cmd.text, this.cmd, this.done, this.cmd.transform, scene.camera(), objects.objects(selected.name));
                } else {
                    var topCake = "";
                    var maxPosY = 0;
                    design.rootGroup().findElement('plate-0').traverseCakes(function (cake) {
                        if (cake.object.position.y > maxPosY && !cake.decal) {
                            topCake = cake.name;
                            maxPosY = cake.object.position.y;
                        }
                    });
                    if (typeof topCake === "string" && topCake.length > 0) {
                        design.select("cake", topCake, true);
                        objects.addObjectDecal(this.cmd.text, this.cmd, this.done, this.cmd.transform, scene.camera(), objects.objects(topCake));
                    } else {
                        debug.log("Can not add decal, no cake selected.");
                    }
                }
            }
        } else if (typeof this.cmd.copy === "string") {
            objects.copyObject3D(this.cmd, this.done, this.cmd.transform);
        }
    }
    else {
        objects.objects(this.cmd.name).visible = true;
        design.rootGroup().swapElement(this.cmd.name, this.cmd.parentName);
        return 0;
        var object = objects.objects(this.cmd.name);
        var designElement = design.rootGroup().findElement(this.cmd.name);
        resetProperties (
            this.cmd.name,
            designElement.properties
        );
        // TODO: better solution for redo
        designElement.object = object;
        if (this.cmd.deselectColor instanceof THREE.Color) {
            objects.objects(this.cmd.name).deselectColor = this.cmd.deselectColor;
        } else {
            debug.log("AddObjectCommand.execute: no deselectColor in command.");
        }
    }
    return 0;
}

AddCakeCommand.prototype.unexecute = function () {
    debug.log("AddCakeCommand undone.");
    var designElement = design.rootGroup().findElement(this.cmd.name);
    designElement.object.visible = false;
    design.rootGroup().swapElement(this.cmd.name, this.cmd.prevParentName);
    return 0;
    this.cmd.prevParentName = this.cmd.parentName;
    this.cmd.deselectColor = objects.objects(this.cmd.name).deselectColor;
    return removeCake(this.cmd);
}

var AddObjectCommand = function (cmd) {
    this.cmd = cmd;
}

AddObjectCommand.prototype = Object.create(Command.prototype);

AddObjectCommand.prototype.execute = function (redo, done) {
    debug.log("AddObjectCommand");
    if (!redo) {
        if (done !== undefined) { this.done = done; }
        if (this.cmd.copy === undefined) {
            if (this.cmd.isText3D) {
                objects.add3DText(this.cmd.name, this.cmd.text, this.cmd.font, 0.13, this.cmd.textStyle, this.cmd.transform, this.done);
            } else if (this.cmd.isSideDecoration) {
                objects.addSideDecoration(this.cmd.onTo, this.cmd.name, this.cmd.pieces, this.cmd.sideDecorDelta, this.done);
                this.cmd.parentName = this.cmd.onTo;
            } else {
                objects.objectParser(this.cmd.text, this.cmd, this.done, this.cmd.transform, this.cmd.normalScale);
            }
            name = this.cmd.name;
        } else if (typeof this.cmd.copy === "string") {
            objects.copyObject3D(this.cmd, this.done, this.cmd.transform);
        }
        var designElement = design.rootGroup().findElement(this.cmd.name);
    }
    else {
        objects.objects(this.cmd.name).visible = true;
        design.rootGroup().swapElement(this.cmd.name, this.cmd.parentName);
        return 0;
        resetProperties (
            this.cmd.name,
            scene.designs(scene.designIndex()).objects(this.cmd.name).properties
        );
        var object = objects.objects(this.cmd.name);
        var designElement = design.rootGroup().findElement(this.cmd.name);
        // TODO: better solution for redo
        designElement.object = object;
        debug.log(designElement);
        if (designElement.isRandom) {
            debug.log("RotateX");
            designElement.object.children[0].geometry.rotateX(Math.PI/2);
            designElement.object.rotation.x = this.cmd.randomObjectRotation.x;
            designElement.object.rotation.y = this.cmd.randomObjectRotation.y;
            designElement.object.rotation.z = this.cmd.randomObjectRotation.z;
        }
        if (this.cmd.deselectColor instanceof THREE.Color) {
            objects.objects(this.cmd.name).deselectColor = this.cmd.deselectColor;
        } else {
            debug.log("AddObjectCommand.execute: no deselectColor in command.");
        }
        design.rootGroup().swapElement(this.cmd.name, this.cmd.prevParentName);
    }
    return 0;
}

AddObjectCommand.prototype.unexecute = function () {
    debug.log("AddObjectCommand undone.");
    debug.log(this.cmd);
    var designElement = design.rootGroup().findElement(this.cmd.name);
    designElement.object.visible = false;
    design.rootGroup().swapElement(this.cmd.name, this.cmd.prevParentName);
    return 0;
    this.cmd.prevParentName = designElement.parentName;
    this.cmd.deselectColor = objects.objects(this.cmd.name).deselectColor;
    if (designElement.isRandom) {
        this.cmd.randomObjectRotation = {
            x: designElement.object.rotation.x,
            y: designElement.object.rotation.y,
            z: designElement.object.rotation.z
        }
    }
    return removeObject(this.cmd);
}

var RemoveObjectCommand = function (cmd) {
    this.cmd = cmd;
}

RemoveObjectCommand.prototype = Object.create(Command.prototype);

RemoveObjectCommand.prototype.execute = function (redo, done) {
    debug.log("RemoveObjectCommand");
    var designObject = scene.designs(scene.designIndex()).findByName(this.cmd.name);
    // design.element
    var designElement = design.rootGroup().findElement(this.cmd.name);
    designElement.object.visible = false;
    design.rootGroup().swapElement(this.cmd.name, "null-0");
    return 0;
    /* =============== */
    if (!designObject) return -1;
    this.cmd.prevParentName = designElement.parentName;
    if (designObject.properties.texture === undefined) designObject.properties["texture"] = 0;
    // design.element
    if (designElement.properties.texture === undefined) designElement.properties["texture"] = 0;
    if (redo === 0) {
        this.cmd["prevProperties"] = { };
        prevProperties = { };
        // TODO: design.element
        $.each(designElement.properties, function (key, value) {
            debug.log("Old properties: "+ key +" "+ value);
            prevProperties[key] = value;
        });
        this.cmd.prevProperties = prevProperties;
    }
    var type = this.cmd.name.split("-")[0];
    this.cmd.parentName = "null-0";
    if (type === "object")
        return removeObject(this.cmd);
    else if (type === "cake")
        return removeCake(this.cmd);
}

RemoveObjectCommand.prototype.unexecute = function () {
    debug.log("RemoveObjectCommand undone.");
    objects.objects(this.cmd.name).visible = true;
    design.rootGroup().swapElement(this.cmd.name, this.cmd.prevParentName);
    return 0;
    var name = this.cmd.name;
    var properties = { };
    $.each(this.cmd.prevProperties, function (key, value) {
        debug.log("Unexecute prev properties: "+ key +" "+ value);
        properties[key] = value;
    });
    this.cmd["properties"] = properties;
    debug.log(this.cmd);
    // this.cmd.parentName = this.cmd.prevParentName;
    // this.cmd.prevParentName = "null-0";
    result = scene.add(name.split("-")[0], this.cmd);
    design.rootGroup().swapElement(name, this.cmd.prevParentName);
    design.rootGroup().findElement(name).object = objects.objects(name);

    return result;
}

var AddRandomObjectCommand = function (cmd) {
    this.cmd = cmd;
}

AddRandomObjectCommand.prototype = Object.create(Command.prototype);

AddRandomObjectCommand.prototype.execute = function (redo, done) {
    if (done !== undefined) this.done = done;
    this.cmd["lastIndex"] = objects.addRandomObject(
        this.cmd.source, this.cmd.onTo, this.cmd.count
    );

    return 0;
}

AddRandomObjectCommand.prototype.unexecute = function () {
    objects.removeRandomObject(this.cmd.onTo, this.cmd.lastIndex, this.cmd.count);

    return 0;
}

var MoveObjectCommand = function (cmd) {
    this.cmd = cmd;
}

MoveObjectCommand.prototype = Object.create(Command.prototype);

MoveObjectCommand.prototype.execute = function (redo) {
    debug.log("MoveObjectCommand");
    for (k = 0; k < this.cmd.objects.length; k++) {
        var name = this.cmd.objects[k].name;
        var object = objects.objects(name);
        if (redo === 0) {
            this.cmd.objects[k].currentPosition = {
                x: object.position.x,
                y: object.position.y,
                z: object.position.z
            };
            if (object.pastaElement.alignNormal) {
                this.cmd.objects[k].currentOrientation = {
                    x: object.rotation.x,
                    y: object.rotation.y,
                    z: object.rotation.z
                }
            }
        }
        if (!this.cmd.objects[k].decal) {
            debug.log("MoveObjectCommand.execute");
            debug.log(this.cmd.objects[k]);
            replaceObject(object, this.cmd.objects[k].currentPosition);
            if (object.pastaElement.alignNormal) {
                object.rotation.x = this.cmd.objects[k].currentOrientation.x;
                object.rotation.y = this.cmd.objects[k].currentOrientation.y;
                object.rotation.z = this.cmd.objects[k].currentOrientation.z;
            }
            design.rootGroup().swapElement(name, this.cmd.objects[k].parentName);
            design.rootGroup().findElement(name).update();
        } else {
            this.cmd.objects[k].intersects["point"] = new THREE.Vector3(
                this.cmd.objects[k].currentPosition.x,
                this.cmd.objects[k].currentPosition.y,
                this.cmd.objects[k].currentPosition.z
            );
        }
    }
    return 0;
}

MoveObjectCommand.prototype.unexecute = function () {
    debug.log("MoveObjectCommand undone.");
    for (k = 0; k < this.cmd.objects.length; k++) {
        var name = this.cmd.objects[k].name;
        var object = objects.objects(name);
        if (!this.cmd.objects[k].decal) {
            replaceObject(object, this.cmd.objects[k].prePosition);
            var designElement = design.rootGroup().findElement(name);
            if (designElement.alignNormal) {
                designElement.object.rotation.x = this.cmd.objects[k].preOrientation.x;
                designElement.object.rotation.y = this.cmd.objects[k].preOrientation.y;
                designElement.object.rotation.z = this.cmd.objects[k].preOrientation.z;
            }
            this.cmd.objects[k].parentName = designElement.parentName;
            debug.log(name);
            debug.log(this.cmd.objects[k]);
            design.rootGroup().swapElement(name, this.cmd.objects[k].prevParentName);
            designElement.update();
        } else {
            return 0;
            var newDecal = objects.moveDecalTo(object, this.cmd.objects[k].intersects);
            scene.updateDesignObject(newDecal);
        }
    }
    return 0;
}

var ChangePropertiesCommand = function (cmd) {
    this.cmd = cmd;
}

ChangePropertiesCommand.prototype = Object.create(Command.prototype);

ChangePropertiesCommand.prototype.execute = function (redo) {
    debug.log("ChangePropertiesCommand");
    if (!this.cmd.decal) {
        var designObject = scene.designs(scene.designIndex()).findByName(this.cmd.name);
        // design.element
        var designElement = design.rootGroup().findElement(this.cmd.name);
        if (!designObject) return -1;
        // if (designObject.properties.texture === undefined) designObject.properties["texture"] = 0;
        // if (designObject.properties.rotationY === undefined) designObject.properties["rotationY"] = 0;
        if (redo === 0) {
            this.cmd["prevProperties"] = { };
            var prevProperties = { };
            $.each(designObject.properties, function (key, value) {
                debug.log("Old properties: "+ key +" "+ value);
                prevProperties[key] = value;
            });
            this.cmd.prevProperties = prevProperties;
        }
        $.each(this.cmd.properties, function (key, value) {
            debug.log("New properties: "+ key +" "+ value);
            designObject.properties[key] = value;
            // design.element
            designElement.properties[key] = value;
        });
        objects.setGraphicalProperties(this.cmd.name, designElement.properties);
        design.rootGroup().findElement(this.cmd.name).updateScaleAndRotation();
    } else {
        this.cmd.properties["decal"] = true;
        var designObject = scene.designs(scene.designIndex()).findByName(this.cmd.name);
        // design.element
        var designElement = design.rootGroup().findElement(this.cmd.name);
        if (!designObject) return -1;
        if (redo === 0) {
            this.cmd["prevProperties"] = { };
            var prevProperties = { };
            $.each(designObject.properties, function (key, value) {
                debug.log("Old properties: "+ key +" "+ value);
                prevProperties[key] = value;
            });
            this.cmd.prevProperties = prevProperties;
        }
        $.each(this.cmd.properties, function (key, value) {
            debug.log("New properties: "+ key +" "+ value);
            designObject.properties[key] = value;
            // design.element
            designElement.properties[key] = value;
        });
        if (this.cmd.source !== undefined) {
            designObject["source"] = this.cmd.source;
            // design.element
            designElement["source"] = this.cmd.source;
        }
        scene.updateDesignObject(objects.setGraphicalProperties(this.cmd.name, this.cmd.properties, this.cmd.source));
    }
    return 0;
}

ChangePropertiesCommand.prototype.unexecute = function () {
    if (!this.cmd.decal) {
        var designObject = scene.designs(scene.designIndex()).findByName(this.cmd.name);
        // design.element
        var designElement = design.rootGroup().findElement(this.cmd.name);
        $.each(this.cmd.prevProperties, function (key, value) {
            debug.log("Unexecute prev properties: "+ key +" "+ value);
            designObject.properties[key] = value;
            // design.element
            designElement.properties[key] = value;
        });
        objects.setGraphicalProperties(this.cmd.name, this.cmd.prevProperties);
        design.rootGroup().findElement(this.cmd.name).updateScaleAndRotation();
    } else {
    }
    return 0;
}

function replaceObject (object, pos) {
    object.position.x = pos.x;
    object.position.y = pos.y;
    object.position.z = pos.z;
}

var AddPlateCommand = function (cmd) {
    this.cmd = cmd;
}

AddPlateCommand.prototype = Object.create(Command.prototype);

AddPlateCommand.prototype.execute = function (redo, done) {
    debug.log("AddPlateCommand");
    if (done !== undefined) { this.done = done; }
    objects.objectParser(this.cmd.text, this.cmd, this.done);
}

AddPlateCommand.prototype.unexecute = function () {
    debug.log("AddPlateCommand undone.");
    return removePlate(this.cmd);
}

var AddDesignCommand = function () { }

AddDesignCommand.prototype = Object.create(Command.prototype);

AddDesignCommand.prototype.execute = function (redo, done) {
    debug.log("AddDesignCommand");
    if (done !== undefined) { this.done = done; }
    scene.designs().push(new design.Design());
    scene.designIndex(scene.designs().length-1);
    addBasePlane();
    if (!config.mobile) addRoomMtl();
}

var SelectDesignCommand = function () { }

SelectDesignCommand.prototype = Object.create(Command.prototype);

SelectDesignCommand.prototype.execute = function (redo, k) {
    if (k !== undefined) {
        if (k >= 0 && k < scene.designs().length) {
            scene.designIndex(k);
        }
    } else {
        if (scene.designs().length > 0) {
            scene.designIndex(0);
        }
    }
    return module.exports.design();
}

function removeCake (cmd) {
    return scene.remove("cake", cmd.name);
}

function removeObject (cmd) {
    return scene.remove("object", cmd.name);
}

function removePlate (cmd) {
    return scene.remove("plate", cmd.name);
}

var textureUrl = "/Images/PlaneTextures/wood_texture.jpg";

function changePlaneTexture(url) {
    textureUrl = url;
    addBasePlane();
}

function addBasePlane() {
    var geometry = new THREE.BoxBufferGeometry(6.5, 0.01, 6.5);
    var texture = THREE.ImageUtils.loadTexture(textureUrl);
    var texture2 = THREE.ImageUtils.loadTexture(textureUrl);
    var texture3 = THREE.ImageUtils.loadTexture(textureUrl);

    var cubeMaterialArray = [];
    var opacity = 0.0;
    // order to add materials: x+,x-,y+,y-,z+,z-
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));
    cubeMaterialArray.push(new THREE.MeshPhongMaterial({ map: texture2 }));

    var cubeMaterials = new THREE.MeshFaceMaterial(cubeMaterialArray);

    var material = new THREE.MeshPhongMaterial({ color: 0xaaaaaa, transparent: true, opacity: 0.0 });
    if (config.mobile) {
        var plane = new THREE.Mesh(geometry, cubeMaterials);
    } else {
        var plane = new THREE.Mesh(geometry, material);
    }

    texture.wrapS = texture2.wrapS = texture3.wrapS = texture.wrapT = texture2.wrapT = texture3.wrapT = THREE.RepeatWrapping;

    /*sağ sol*/ texture.repeat.set(2, 0.3); /*ön arka*/ texture3.repeat.set(3.9, 0.3); /*üst alt*/ texture2.repeat.set(3.9, 2);

    plane.position.x = config.center.x;
    plane.position.z = config.center.z;
    plane.position.y = -0.05;
    // plane.rotation.x = Math.PI / 2;

    // plane.castShadow = true;
    plane.receiveShadow = false;

    var planeObject = new THREE.Object3D();
    planeObject.name = "base-plane";
    planeObject.add(plane);

    scene.designs(scene.designIndex()).basePlane(plane);
    scene.get().add(planeObject);
}

function addRoom_back () {
		var textureLoader = new THREE.TextureLoader();
		textureLoader.load( '/Images/cube/cube_map.jpg', function ( texture ) {
				texture.mapping = THREE.UVMapping;
        var mesh = new THREE.Mesh( new THREE.SphereGeometry( 80, 32, 16 ), new THREE.MeshBasicMaterial( { map: texture } ) );
		    mesh.scale.x = -1;
        mesh.position.y = 20.0;
        var floor = new THREE.Mesh(new THREE.CylinderGeometry(40, 40, 1, 32) , new THREE.MeshBasicMaterial( {color: 0xffffff} ));
        floor.position.y = -1;
		    scene.get().add( mesh );
        // scene.get().add(floor);
        config.envMap = scene.cubeCamera1().renderTarget.texture;
		} );
}

function addRoom_hdr () {
    var genCubeUrls = function( prefix, postfix ) {
				return [
						prefix + 'px' + postfix, prefix + 'nx' + postfix,
						prefix + 'py' + postfix, prefix + 'ny' + postfix,
						prefix + 'pz' + postfix, prefix + 'nz' + postfix
				];
		};
		var hdrUrls = genCubeUrls( "/Images/cube/", ".hdr" );
		new THREE.HDRCubeTextureLoader().load( THREE.UnsignedByteType, hdrUrls,
                                           function ( hdrCubeMap ) {
				                                       var pmremGenerator = new THREE.PMREMGenerator( hdrCubeMap );
				                                       pmremGenerator.update( view.renderer() );
				                                       var pmremCubeUVPacker = new THREE.PMREMCubeUVPacker( pmremGenerator.cubeLods );
				                                       pmremCubeUVPacker.update( view.renderer() );
				                                       var hdrCubeRenderTarget = pmremCubeUVPacker.CubeUVRenderTarget;
                                               config.envMap = hdrCubeRenderTarget.texture;
		                                       } );
}

function addRoom () {
    var loader = new THREE.OBJLoader();

    // load a resource
    loader.load(
	      // resource URL
	      '/Mutfak/kitchen_6.obj',
	      // Function when resource is loaded
	      function ( object ) {
            object.traverse(function (child) {
                if (child instanceof THREE.Mesh) {
                    var textureLoader = new THREE.TextureLoader();
                    var map = textureLoader.load('/Mutfak/atlas_texture.jpg');
                    var nMap = textureLoader.load('/Mutfak/atlas_normals.jpg');
                    child.material =  new THREE.MeshPhongMaterial({ color: 0xffffff, specular: 0xffffff, map: map, normalMap: nMap, shininess: 1, shading: THREE.SmoothShading });
                    child.geometry = new THREE.Geometry().fromBufferGeometry(child.geometry);
                    child.geometry.mergeVertices();
                    child.geometry.computeFaceNormals();
                    child.geometry.computeVertexNormals();
                }
            });
		        scene.get().add( object );
	      }
    );
}

function addRoomMtl () {
    var mtlLoader = new THREE.MTLLoader();
    mtlLoader.setBaseUrl('/Mutfak/');
		mtlLoader.setPath( '/Mutfak/' );
		mtlLoader.load( 'final_kitchen_spec.mtl', function( materials ) {
				materials.preload();
        for (var key in materials.materials) {
            materials.materials[key].shading = THREE.SmoothShading;
            materials.materials[key].transparent = true;
            materials.materials[key].opacity = 1.0;
            materials.materials[key].color.setHex(0xffffff);
            // materials.materials[key].side = THREE.DoubleSide;
        }
        debug.log(materials);
				var objLoader = new THREE.OBJLoader();
	      objLoader.setMaterials( materials );

				objLoader.setPath( '/Mutfak/' );
				objLoader.load( 'final_kitchen.obj', function ( obj ) {
            // obj.children[0].geometry.mergeVertices();
            obj.traverse(function (child) {
                if (child instanceof THREE.Mesh) {
                    child.geometry = new THREE.Geometry().fromBufferGeometry(child.geometry);
                    child.material.needsUpdate = true;
                    child.geometry.mergeVertices();
                    child.geometry.computeFaceNormals();
                    child.geometry.computeVertexNormals();
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            // obj.children[0].geometry.computeFaceNormals();
            // obj.children[0].geometry.computeVertexNormals();
            obj.position.y = 0.02;
			      obj.position.x = config.center.x;
            obj.scale.y = 1.5
            obj.scale.x = 2.0;
            obj.scale.z = 2.0;
            obj.name = "kitchen-0";
            roomObj = obj;
						scene.get().add( roomObj );
            // Render function
            // view.render();
				}, function () { }, function () { } );
		});
}

function addTableMtl () {
    var mtlLoader = new THREE.MTLLoader();
    mtlLoader.setBaseUrl('/Mutfak/');
		mtlLoader.setPath( '/Mutfak/' );
		mtlLoader.load( 'final_kitchen_table.mtl', function( materials ) {
				materials.preload();
        for (var key in materials.materials) {
            materials.materials[key].shading = THREE.SmoothShading;
            materials.materials[key].transparent = true;
            materials.materials[key].opacity = 1.0;
            materials.materials[key].color.setHex(0xffffff);
            // materials.materials[key].side = THREE.DoubleSide;
        }
        debug.log(materials);
				var objLoader = new THREE.OBJLoader();
	      objLoader.setMaterials( materials );

				objLoader.setPath( '/Mutfak/' );
				objLoader.load( 'final_kitchen_table.obj', function ( obj ) {
            // obj.children[0].geometry.mergeVertices();
            obj.traverse(function (child) {
                if (child instanceof THREE.Mesh) {
                    child.geometry = new THREE.Geometry().fromBufferGeometry(child.geometry);
                    child.material.needsUpdate = true;
                    child.geometry.mergeVertices();
                    child.geometry.computeFaceNormals();
                    child.geometry.computeVertexNormals();
                    child.castShadow = true;
                    child.receiveShadow = true;
                }
            });
            // obj.children[0].geometry.computeFaceNormals();
            // obj.children[0].geometry.computeVertexNormals();
            obj.position.y = 0.02;
            obj.scale.y = 1.5
            obj.scale.x = 2.0;
            obj.scale.z = 2.0;
            obj.name = "kitchen-0";
            roomObj = obj;
						scene.get().add( roomObj );
            // Render function
            // view.render();
				}, function () { }, function () { } );
		});
}

function setRoomColor (color)
{
    roomObj.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            child.material.color.setHex(color);
        }
    });
}

function resetProperties (name, properties) {
    $.each(properties, function (key, value) {
        if (key === "color") {
            objects.setColor(name, value);
        } else if (key === "texture" && typeof value === "string") {
            objects.setTexture(name, value);
        }
    });
}

var roomObj = null;