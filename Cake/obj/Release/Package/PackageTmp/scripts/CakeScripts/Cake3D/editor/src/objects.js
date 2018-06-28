var THREE = require('three');
var OBJLoader = require('three-obj-loader');
var debug = require('./debug');
var decalObjects = require('./decal_objects');
var scene = require('./scene');
var textObject = require('./text');
var sideDecor = require('./side_decor.js');
var config = require('./config.js');

OBJLoader(THREE);

module.exports = {
    loadObject: function (url, name, done, transform) {
        loadObject(url, name, done, transform);
    },
    objectParser: function (text, obj, done, transform, normalScale) {
        objectParser(text, obj, done, transform, normalScale);
    },
    loadTexture: function (url, name, done) {
        loadTexture(url, name, done);
    },
    objects: function (name) {
        if (name !== undefined) {
            return objects[name] || objectsDecal[name];
        } else {
            return objects;
        }

        return undefined;
    },
    setTexture: function (oname, tname) {
        setTexture(oname, tname);
    },
    setColor: function (oname, color) {
        setColor(oname, color);
    },
    setGraphicalProperties: function (oname, properties, decalSource) {
        return setGraphicalProperties(oname, properties, decalSource);
    },
    setPlateGraphicalProperties: function (properties) {
        return setPlateGraphicalProperties(properties);
    },
    copyObject: function (obj) {
        return copyObject(obj);
    },
    copyObject3D: function (obj, done, transform) {
        copyObject3D(obj, done, transform);
    },
    addObjectDecal: function(text, obj, done, transform, camera, onObject) {
        addObjectDecal(text, obj, done, transform, camera, onObject);
    },
    objectsDecal: function (name) {
        return objectsDecal[name];
    },
    setObjectDecal: function (name, decal) {
        objectsDecal[name] = decal;
    },
    moveDecalTo: function (decal, intersection) {
        return decalObjects.moveTo(decal, intersection);
    },
    moveDecalToXZ: function (decal, point, onTo) {
        return decalObjects.moveToXZ(decal, point, objects[onTo]);
    },
    moveDecalToWrapper: function (decal, decalObjectProps, onToObject) {
        debug.log("moveDecalToWrapper: " + onToObject);
        return decalObjects.moveToWrapper(decal, decalObjectProps, objects[onToObject]);
    },
    moveText3D: function (object, intersects, designMeshes) {
        textObject.moveText3D(object, intersects, designMeshes);
    },
    addPlate: function () {
        addPlate();
    },
    changePlate: function (option) {
        changePlate(option);
    },
    add3DText: function (name, text, font, size, style, transform, done) {
        add3DText(name, text, font, size, style, transform, done);
    },
    addSideDecoration: function (onTo, name, objectArray, delta, done) {
        addSideDecoration (onTo, name, objectArray, delta, done);
    },
    addRandomObject: function (name, onToName, count) {
        // for (var k = 0; k < count; k++) {
        return addRandomObject (name, onToName, count);
        //    setTimeout(parseInt(Math.random()*100), function() {});
        // }
        // return count-1;
    },
    removeRandomObject: function (onToName, lastIndex, count) {
        return removeRandomObject(onToName, lastIndex, count);
    },
    addObjectDecalLoadWrapper: function (name, decalObjectProps, onObject, done) {
        if (typeof onObject === "undefined") {
            onObject = objects['plate-0'];
        }
        var tmpIntersect = {
            point: new THREE.Vector3(
                decalObjectProps.intersectionPoint.x,
                decalObjectProps.intersectionPoint.y,
                decalObjectProps.intersectionPoint.z
            ),
            face: { normal: new THREE.Vector3 (
                decalObjectProps.intersectionNormal.x,
                decalObjectProps.intersectionNormal.y,
                decalObjectProps.intersectionNormal.z
            )},
            object: { parent: onObject }
        };
        var decal = decalObjects.load(name, tmpIntersect, decalObjectProps.sources, onObject, function (d) {
            appendDecalObject(name, d);
        }, done);
    },
    loadAllFonts: function (done) {
        var loader = new THREE.FontLoader();
        loader.load(fontList[0], function ( response ) {
            var loader = new THREE.FontLoader();
        });
    },
    colorizeObject: function (name, imgURL, coordsData, colors, done) {
        colorizeObject(name, imgURL, coordsData, colors, done);
    },
    getColorizedTexture: function (name, index) {
        var _index = index || colorizedTextures[name].length-1;
        return colorizedTextures[name][_index];
    },
    getPositionFromCake: function (name, sel, up) {
        var _up = up || 1.0;
        var cake = objects[name];
        var cakePos = new THREE.Vector3().copy(cake.position);
        var rotDeg = sel*0.25*Math.PI;

        var direction = new THREE.Vector3(Math.cos(rotDeg), _up, Math.sin(rotDeg));
        var raycaster = new THREE.Raycaster(cakePos, direction);

        var intersection = raycaster.intersectObject(cake, true);

        return {
            x: intersection[intersection.length-1].point.x,
            y: intersection[intersection.length-1].point.y,
            z: intersection[intersection.length-1].point.z
        };
    },
    parseCoordSource: function (source) {
        debug.log(source);
        return getCoords(source);
    },
    reset: function () {
        reset();
    }
}

var objects = {};
var textures = {};
var objectsDecal = {};
var colorizedTextureID = 0x00000001 << 24;
var colorizedTextures = {};
var normalScales = {};
var fontList = [
    "/fonts/Grand Hotel_Regular.json",
    "/fonts/Antonio_Regular.json",
    "/fonts/Familiar Pro_Bold.json"
];
var nScaleObject = 1.0;
var nScaleCake = 0.25;
var gNormalMap = new THREE.TextureLoader().load("/Images/normal_map.png");

function reset() {
    objects = {};
    textures = {};
    objectsDecal = {};
    colorizedTextureID = 0x00000001 << 24;
    colorizedTextures = {};
}

var loadedFonts = new Array(fontList.length);

function appendObject (name, object) {
    objects[name] = object;
    debug.log("Object appended to `objects`.");
}

function appendTexture (name, texture) {
    textures[name] = texture;
}

function appendDecalObject (name, mesh) {
    objectsDecal[name] = mesh;
}

function loadTexture (url, name, done) {
    // var imageLoader = new THREE.ImageLoader();
    var _texture = new THREE.TextureLoader().load(url, function(t) {
        // setTimeout(function () {
        appendTexture(name, t);
        if (typeof done === "function") done();
        // }, 10000);
    });
    // appendTexture(name, texture);
    // var texture = new THREE.Texture();
    // imageLoader.load(url, function (image) {
    //   texture.image = image;
    //   texture.needsUpdate = true;
    //   appendTexture(name, texture);
    //     debug.log("loadTexture: "+ name);
    //     debug.log(textures[name]);
    //   if (done !== undefined) {
    //     done();
    //   }
    // });
    // if (typeof done === "function") done();
}

function transformParser (transform) {
    if (transform === undefined) {
        transform = {
            scale: { x: 1., y: 1., z: 1. },
            position: { x: 0., y: 0., z: 0. },
            rotation: { x: 0., y: 0., z: 0. }
        };
    } else {
        if (transform.scale === undefined) {
            transform.scale = { x: 1., y: 1., z: 1. };
        }
        if (transform.position === undefined) {
            transform.position = { x: 0., y: 0., z: 0. };
        }
        if (transform.rotation === undefined) {
            transform.rotation = { x: 0., y: 0., z: 0. };
        }
    }

    return transform;
}

function add3DText (name, text, font, size, style, transform, done) {
    var loader = new THREE.FontLoader();
    loader.load( fontList[font], function ( response ) {
        if (loadedFonts[font] === undefined) loadedFonts[font] = response;
        var obj = textObject.text3d(name, text, response, size, font, style);
        obj.position.set(
            transform.position.x,
            transform.position.y,
            transform.position.z
        );
        appendObject(name, obj);
        debug.log("add3DText");
        debug.log(done);
        if (typeof done === "function") {
            done();
        }
    });
}

function addSideDecoration (onTo, name, objectArray, delta, done) {
    debug.log(onTo);
    if (typeof objectArray === "undefined") {
        objectArray = [];
    }
    var onToObject = objects[onTo];
    if (onToObject === undefined || onTo.split("-")[0] !== "cake") {
        debug.log("addSideDecoration: No cake selected");
        return;
    }
    var decorBase = objectArrayParser(name, objectArray, done);
    var decor;
    if (decorBase.length > 0) {
        decor = sideDecor.createSideDecoration(decorBase, onToObject, delta);
    } else {
        var objectLoader = new THREE.OBJLoader();
        decor = objectLoader.parse(dummyCubeSource());
    }
    decor.name = name;
    debug.log(decor);
    appendObject(name, decor);
    if (typeof done === "function") {
        done();
    }
}

function dummyCubeSource () {
    return "# Blender v2.78 (sub 0) OBJ File: ''\n# www.blender.org\nmtllib dummy_cube.mtl\no Cube\nv 0.000100 -0.000100 -0.000100\nv 0.000100 -0.000100 0.000100\nv -0.000100 -0.000100 0.000100\nv -0.000100 -0.000100 -0.000100\nv 0.000100 0.000100 -0.000100\nv 0.000100 0.000100 0.000100\nv -0.000100 0.000100 0.000100\nv -0.000100 0.000100 -0.000100\nvn 0.0000 -1.0000 -0.0000\nvn 0.0000 1.0000 -0.0000\nvn 1.0000 0.0000 0.0000\nvn -0.0000 -0.0000 1.0000\nvn -1.0000 -0.0000 -0.0000\nvn 0.0000 0.0000 -1.0000\nusemtl None\ns off\nf 1//1 2//1 3//1 4//1\nf 5//2 8//2 7//2 6//2\nf 1//3 5//3 6//3 2//3\nf 2//4 6//4 7//4 3//4\nf 3//5 7//5 8//5 4//5\nf 5//6 1//6 4//6 8//6\n";
}

function objectArrayParser (name, objectArray, done) {
    var objectInfo = name.split("-");
    var objectLoader = new THREE.OBJLoader();
    var meshes = [];
    var objectArr = new Array();
    var objectInner = new THREE.Object3D(); // objectLoader.parse(objectArray[k].text);
    for (var k = 0; k < objectArray.length; k++) {
        /* var meshDummy = new THREE.Mesh(
           new THREE.SphereBufferGeometry(0.075, 16, 16),
           new THREE.MeshPhongMaterial({ color: 0xffffff })
           ); */
        var objectInner;
        /* TODO: return duzelt */
        if (typeof objectArray[k] === "string" && objectArray[k] !== "")
            objectInner = objectLoader.parse(objectArray[k]);
        else
            return; // objectInner = objectLoader.parse(dummyCubeSource());
        objectArr.push(objectInner.children[0]);
    }
    k = 0;
    var posX = 0;
    for (var k = 0; k < objectArr.length; k++) {
        var child = objectArr[k];
        child.name = objectInfo[1] +"-"+ k;
        // // child.position.copy(objectArray[k].properties.transform.position);
        child.geometry = new THREE.Geometry().fromBufferGeometry(child.geometry);
        // child.geometry.computeBoundingBox();
        // child.position.x = posX;
        // posX += child.geometry.boundingBox.size().x;
        // k++;
    }
    if (objectInfo[2] === "side") {
        debug.log(objectArr);
        return objectArr;
    }
}

function randomInt (s, e) {
    return Math.floor((Math.random()*e)+s);
}

function randomReal (s, e) {
    debug.log("Rand interval: "+ e +" "+ s);
    return Math.random()*(e-s)+s;
}

function shuffle(array) {
    var currentIndex = array.length, temporaryValue, randomIndex;

    // While there remain elements to shuffle...
    while (0 !== currentIndex) {
        // Pick a remaining element...
        randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex -= 1;

        // And swap it with the current element.
        temporaryValue = array[currentIndex];
        array[currentIndex] = array[randomIndex];
        array[randomIndex] = temporaryValue;
    }

    return array;
}

function addRandomObject (name, onToName, count) {
    var object = objects[name];
    var onToObject = objects[onToName];
    if (onToObject === undefined) return -1;
    var onToObjectPos = new THREE.Vector3().copy(onToObject.position);
    var alpha = 0.0, theta = 0.0;
    var raycaster = new THREE.Raycaster();
    var direction = new THREE.Vector3(0, 1, 0);
    var randObjectPos = new THREE.Vector3();
    var meshDummy;
    var intersectsCount = 0;
    var psi = 0;
    var permuteIndex1 = new Array(count);
    var permuteIndex2 = new Array(count);
    var permuteIndex3 = new Array(count);
    for (var l = 0; l < count; l++) {
        permuteIndex1[l] = l;
        permuteIndex2[l] = l;
        permuteIndex3[l] = l;
    }
    permuteIndex1 = shuffle(permuteIndex1);
    permuteIndex2 = shuffle(permuteIndex2);
    permuteIndex3 = shuffle(permuteIndex3);
    var randOffset = Math.random()*0+1;
    debug.log("Permute index");
    debug.log(permuteIndex1);
    debug.log(permuteIndex2);
    debug.log(permuteIndex3);
    for (var k = 0; k < count; k++) {
        var centerY = (permuteIndex1[k]+(permuteIndex1[k]+1))/count/2;
        var centerX = (2*permuteIndex2[k]/count-1+2*(permuteIndex2[k]+1)/count-1)/2;
        var centerZ = (2*permuteIndex3[k]/count-1+2*(permuteIndex3[k]+1)/count-1)/2;
        // direction.y =  randOffset*randomReal(permuteIndex1[k]/count, (permuteIndex1[k]+1)/count);
        direction.y = randomReal(centerY-1e-2, centerY+1e-2);
        // psi = randomReal(permuteIndex2[k]/count*Math.PI*2, (permuteIndex2[k]+1)/count*Math.PI*2)+randOffset;
        // direction.x = randomReal(2*permuteIndex2[k]/count-1, 2*(permuteIndex2[k]+1)/count-1);
        // direction.z = randomReal(2*permuteIndex3[k]/count-1, 2*(permuteIndex3[k]+1)/count-1);
        direction.x = randomReal(centerX-1e-2, centerX+1e-2);
        direction.z = randomReal(centerZ-1e-2, centerZ+1e-2);
        // direction.x = Math.cos(psi);
        // direction.z = Math.sin(psi);
        debug.log("Random dir");
        debug.log(direction);
        raycaster.set(onToObjectPos, direction.normalize());
        intersects = raycaster.intersectObject(onToObject, true);
        // debug.log(onToObjectPos);
        // debug.log(direction);
        // debug.log(intersects);
        intersectsCount = intersects.length;
        if (intersectsCount > 0) {
            meshDummy = new THREE.Mesh(
                new THREE.CylinderGeometry(0.1, 0.1, 0.03),
                new THREE.MeshBasicMaterial({ color: 0xfaa719 })
            );
            meshDummy.geometry.rotateX(Math.PI/2);
            meshDummy.position.copy(onToObject.worldToLocal(intersects[intersectsCount-1].point));
            meshDummy.lookAt(onToObject.worldToLocal(intersects[intersectsCount-1].point.clone().add(
                intersects[intersectsCount-1].face.normal
            )));
            meshDummy.name = "randart-"+ k;
            onToObject.add(meshDummy);
        }
    }

    return onToObject.children.length-1;
}

function randomPointOnObject (name, onToName) {
    var object = objects[name];
    var onToObject = objects[onToName];
    if (onToObject === undefined) return -1;
    var onToObjectPos = new THREE.Vector3().copy(onToObject.position);
    var alpha = 0.0, theta = 0.0;
    var raycaster = new THREE.Raycaster();
    var direction = new THREE.Vector3(0, 1, 0);
    var randObjectPos = new THREE.Vector3();
    var meshDummy;
    var intersectsCount = 0;
    var psi = 0;
    var permuteIndex1 = new Array(count);
    var permuteIndex2 = new Array(count);
    var permuteIndex3 = new Array(count);
    for (var l = 0; l < count; l++) {
        permuteIndex1[l] = l;
        permuteIndex2[l] = l;
        permuteIndex3[l] = l;
    }
    permuteIndex1 = shuffle(permuteIndex1);
    permuteIndex2 = shuffle(permuteIndex2);
    permuteIndex3 = shuffle(permuteIndex3);
    var randOffset = Math.random()*0+1;
    debug.log("Permute index");
    debug.log(permuteIndex1);
    debug.log(permuteIndex2);
    debug.log(permuteIndex3);
    for (var k = 0; k < count; k++) {
        var centerY = (permuteIndex1[k]+(permuteIndex1[k]+1))/count/2;
        var centerX = (2*permuteIndex2[k]/count-1+2*(permuteIndex2[k]+1)/count-1)/2;
        var centerZ = (2*permuteIndex3[k]/count-1+2*(permuteIndex3[k]+1)/count-1)/2;
        // direction.y =  randOffset*randomReal(permuteIndex1[k]/count, (permuteIndex1[k]+1)/count);
        direction.y = randomReal(centerY-1e-2, centerY+1e-2);
        // psi = randomReal(permuteIndex2[k]/count*Math.PI*2, (permuteIndex2[k]+1)/count*Math.PI*2)+randOffset;
        // direction.x = randomReal(2*permuteIndex2[k]/count-1, 2*(permuteIndex2[k]+1)/count-1);
        // direction.z = randomReal(2*permuteIndex3[k]/count-1, 2*(permuteIndex3[k]+1)/count-1);
        direction.x = randomReal(centerX-1e-2, centerX+1e-2);
        direction.z = randomReal(centerZ-1e-2, centerZ+1e-2);
        // direction.x = Math.cos(psi);
        // direction.z = Math.sin(psi);
        debug.log("Random dir");
        debug.log(direction);
        raycaster.set(onToObjectPos, direction.normalize());
        intersects = raycaster.intersectObject(onToObject, true);
        // debug.log(onToObjectPos);
        // debug.log(direction);
        // debug.log(intersects);
        intersectsCount = intersects.length;
        if (intersectsCount > 0) {
            meshDummy = new THREE.Mesh(
                new THREE.CylinderGeometry(0.1, 0.1, 0.03),
                new THREE.MeshBasicMaterial({ color: 0xfaa719 })
            );
            meshDummy.geometry.rotateX(Math.PI/2);
            meshDummy.position.copy(onToObject.worldToLocal(intersects[intersectsCount-1].point));
            meshDummy.lookAt(onToObject.worldToLocal(intersects[intersectsCount-1].point.clone().add(
                intersects[intersectsCount-1].face.normal
            )));
            meshDummy.name = "randart-"+ k;
            onToObject.add(meshDummy);
        }
    }

    return onToObject.children.length-1;
}

function removeRandomObject (onToName, lastIndex, count) {
    var firstIndex = lastIndex-count;
    var onToObject = objects[onToName];
    if (onToObject === undefined) return -1;
    for (k = lastIndex; k > firstIndex; k--) {
        onToObject.remove(onToObject.children[k]);
    }

    return 0;
}

function objectParser (text, obj, done, transform, normalScale) {
    // Dont need to define all paramters of transform
    // It is initialized by default values
    obj = parseObjectProperties(obj);
    transform = transformParser(transform);
    debug.log("tttt: "+ transform.scale.x);
    var objectLoader = new THREE.OBJLoader();
    var object;
    if (typeof text === "string" && text !== "")
        object = objectLoader.parse(text);
    else
        object = objectLoader.parse(dummyCubeSource());
    var mesh;
    var nScale = (obj.name.indexOf("cake") === 0 ? nScaleCake : normalScale);
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            // child.material.color.setHex(0xFFFFFF);
            mesh = child;
            // var bmap =  THREE.ImageUtils.loadTexture("/Images/az bump asıl mono.jpg", {}, function(){});
            // var map =  THREE.ImageUtils.loadTexture("/Images/Mickey_Mouse_D.png", {}, function(){});
            // var normalMap = new THREE.TextureLoader().load('/Images/NormalMap.png');
            // var bumpMap = new THREE.TextureLoader().load('/Images/bump_map.png');
            child.material = getMaterial(null, nScale, obj.name);
            // child.material = new THREE.MeshBasicMaterial( {
				    // 	  envMap: config.envMap
				    // } );
            // child.material = new THREE.MeshStandardMaterial( {
				    // 	  map: null,
				    // 	  bumpScale: - 0.05,
				    // 	  color: 0xffffff,
				    // 	  metalness: 1.0,
				    // 	  roughness: 1.0,
				    // 	  shading: THREE.SmoothShading
				    // } );
            // if (config.envMap !== undefined) {
            //     debug.log("env Map added.");
            //     debug.log(config.envMap);
            //     child.material.envMap = config.envMap;
            //     child.material.needsUpdate = true;
            // }

            child.geometry = new THREE.Geometry().fromBufferGeometry(child.geometry);
            child.castShadow = true;
            child.receiveShadow = true;
            child.geometry.mergeVertices();
            child.geometry.computeFaceNormals();
            child.geometry.computeVertexNormals();
        }
    });

    object.scale.x *= transform.scale.x;
    if (obj.name.indexOf("cake") < 0) {
        object.scale.y *= transform.scale.y;
    }
    object.scale.z *= transform.scale.z;
    object.position.y = transform.position.y;
    object.position.x = transform.position.x;
    object.position.z = transform.position.z;
    object.rotation.x = transform.rotation.x;
    object.rotation.y = transform.rotation.y;
    object.rotation.z = transform.rotation.z;
    object.name = obj.name;
    debug.log("Object added with name: "+ object.name +" and id: "+ object.id);
    appendObject(obj.name, object);
    if (done !== undefined) {
        done(object.name);
    }
}

function addObjectDecal (text, obj, done, transform, camera, onObject) {
    debug.log(onObject);
    var sources = parseDecalText(text);
    debug.log("Decal:");
    debug.log(sources);
    decalObjects.load(obj.name, null, sources, onObject, function(d) {
        appendDecalObject(obj.name, d);
    }, done);
}

function parseDecalText (data) {
    var dataArr = data.split(/:/g, 2);
    debug.log(dataArr);
    if (dataArr[0] === "url") {
        return { url: dataArr[1] }
    } else if (dataArr[0] === "text") {
        dataArr = data.split(/:/g, 4);
        return { font: dataArr[1], curveType: dataArr[2], color: dataArr[3], text: data.substring(dataArr[1].length+dataArr[2].length+dataArr[3].length+8) };
    }
}

THREE.Mesh.prototype.clone = function ( object ) {
    var cloneMaterial = this.material.clone();
    cloneMaterial.shading = this.material.shading;
    cloneMaterial.color = new THREE.Color(0xffffff);
    if (this.material.color instanceof THREE.Color) {
        cloneMaterial.color.r = this.material.color.r;
        cloneMaterial.color.g = this.material.color.g;
        cloneMaterial.color.b = this.material.color.b;
    }
    cloneMaterial.needsUpdate = this.material.needsUpdate;
    if ( object === undefined ) object = new THREE.Mesh(this.geometry, cloneMaterial);
    THREE.Object3D.prototype.clone.call( this, object );

    return object;
};

function copyObject3D (obj, done, transform) {
    obj = parseObjectProperties(obj);
    debug.log("Copy.");
    debug.log(obj);
    transform = transformParser(transform);
    var object = objects[obj.copy].clone();

	  var bbCopied = new THREE.Box3().setFromObject(object);
	  object.position.x += 1.5*Math.abs(bbCopied.max.x-bbCopied.min.x);
    // object.position.z += Math.abs(bbCopied.max.z-bbCopied.min.z);
    object.rotation.x = transform.rotation.x;
    object.rotation.y = transform.rotation.y;
    object.rotation.z = transform.rotation.z;
    object.name = obj.name;
    appendObject(obj.name, object);
    if (done !== undefined) {
        done(obj);
    }
}

function parseObjectProperties (obj) {
    if (obj.color === undefined) {
        obj["color"] = 0xffffff;
    }

    return obj;
}

function loadObject (url, name, done, transform) {
    if (transform === undefined) {
        transform = {
            scale: { x: 1., y: 1., z: 1. },
            position: { x: 0., y: 0., z: 0. }
        };
    }
    var objectLoader = new THREE.OBJLoader();
    // var object;
    objectLoader.load(url, function (object) {
        object.traverse(function (child) {
            if (child instanceof THREE.Mesh) {
                child.material = new THREE.MeshPhongMaterial({ color: 0xeeeeee, specular: 0x0, shininess: 1, shading: THREE.SmoothShading });
            }
        });
        var box = new THREE.Box3().setFromObject(object);
        var maxDim = Math.max(box.size().x, Math.max(box.size().y, box.size().z));
        object.scale.x = 1./maxDim*transform.scale.x;
        object.scale.y = 1./maxDim*transform.scale.y;
        object.scale.z = 1./maxDim*transform.scale.z;
        object.position.y = box.size().y/2./maxDim+transform.position.y;
        object.position.x = transform.position.x;
        object.position.z = transform.position.z;
        object.name = name;
        appendObject(name, object);
        if (done !== undefined) {
            done(object.name);
        }
    });
}

function setTexture (oname, tname) {
    objects[oname].traverse(function (child) {
        debug.log(typeof child);
        if (child instanceof THREE.Mesh) {
            var nameArr = child.name.split("-");
            if (nameArr.length > 0 && nameArr[0] === "randart") return;
            if (child.name === "invisible-mesh") return;
            debug.log(textures);
            var nScale = (oname.indexOf("cake") === 0 ? nScaleCake : nScaleObject);
            child.material = getMaterial(null, nScale, oname);
            child.material.map = textures[tname];
            debug.log("setTexture: id: "+ textures[tname].id);
            child.material.map.needsUpdate = true;
            child.material.needsUpdate = true;
            child.geometry.mergeVertices();
            child.geometry.computeFaceNormals();
            child.geometry.computeVertexNormals();
        }
    });
	  if (objects[oname].deselectColor instanceof THREE.Color)
		    objects[oname].deselectColor.setHex(0xFFFFFF);
	  else
		    objects[oname].deselectColor = new THREE.Color(0xFFFFFF);
}

function removeTexture (oname, color) {
    if (objects[oname].deselectColor instanceof THREE.Color) {
        objects[oname].deselectColor.setHex(color);
    } else {
        objects[oname].deselectColor = new THREE.Color(color);
    }
    objects[oname].traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            var nameArr = child.name.split("-");
            if (nameArr.length > 0 && nameArr[0] === "randart") return;
            if (child.name === "invisible-mesh") return;
            var nScale = (oname.indexOf("cake") === 0 ? nScaleCake : nScaleObject);
            if (!(child.material.mat instanceof THREE.Texture)) return;
            child.material = getMaterial(color, nScale, oname);
            child.material.needsUpdate = true;
            child.geometry.mergeVertices();
            child.geometry.computeFaceNormals();
            child.geometry.computeVertexNormals();
        }
    });
}

function setColor (oname, color) {
    objects[oname].traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            var nameArr = child.name.split("-");
            if (nameArr.length > 0 && nameArr[0] === "randart") return;
            if (child.name === "invisible-mesh") return;
            var nScale = (oname.indexOf("cake") === 0 ? nScaleCake : nScaleObject);
            child.material = getMaterial(color, nScale, oname);
            child.material.needsUpdate = true;
        }
    });
}

function getMaterial (color, nScale, oname) {
    var _nScale = nScale || nScaleCake;
    if (nScale === 0.0) {
        _nScale = 0.0;
    }
    if (typeof oname === "string" && typeof normalScales[oname] === "number") {
        _nScale = normalScales[oname];
    } else {
        normalScales[oname] = _nScale;
    }
    var _color = color || 0xffffff;
    var normalMap = (_nScale > 0.0 ? gNormalMap : null);
    debug.log("getMaterial: "+ _nScale +" "+ oname +" "+ nScale);
    return new THREE.MeshPhongMaterial({
        color: _color, specular: 0x0,
        shininess: 1, shading: THREE.SmoothShading, side: THREE.DoubleSide,
        normalMap: normalMap, normalScale: new THREE.Vector2(_nScale, _nScale)
    });
}

function transformObject (name, transform, done) {
}

function setGraphicalProperties(oname, properties, decalSource, loading) {
    var object = objects[oname] || objectsDecal[oname];
    if (oname === "plate-0") return;
    if (properties.texture === undefined &&
        typeof object.pastaElement.properties.texture !== "string") {
        properties.texture = 0;
    }
    var objectInfo = oname.split("-");
    var subType;
    if (objectInfo.length === 3) subType = objectInfo[2];
    var color = 0xFFFFFF;
    var retDecal = object;
    if (properties.color !== undefined) color = properties.color;
    // debug.log("sources.texttt: "+ decalSource);
    if (decalSource !== undefined && decalSource !== null) {
        var sources = parseDecalText(decalSource);
        retDecal = decalObjects.changeTexture(retDecal, sources);
    }
    debug.log("setGraphicalProperties name:"+ oname);
    $.each(properties, function (key, value) {
        debug.log("setGraphicalProperties: "+ key +" "+ value);
        if (key === "color" && !properties.decal) {
            color = value; // assigned color to be used in removeTexture
            object.traverse(function (child) {
                if (child instanceof THREE.Mesh) {
                    child.material.color.setHex(value);
                }
            });

            if (typeof object.pastaElement === "object" && object.pastaElement.isCake && false) {
                for (var _c in object.pastaElement.children) {
                    var child = object.pastaElement.children[_c];
                    if (child.decal) {
                        child.object.traverse(function (_child) {
                           if (_child instanceof THREE.Mesh) {
                               _child.material.color.setHex(value);
                           }
                        });
                    }
                }
            }
        } else if (key === "scale") {
            if (!properties.decal) {
                //    debug.log("lalal scale" + value);
                debug.log("minSize: " + object.pastaElement.properties.minSize);
                object.pastaElement.unlimitedScale = value;
                if (typeof object.pastaElement.properties.minSize === "string") {
                    if (oname.indexOf("cake") < 0) {
                        value = Math.max(value, parseFloat(object.pastaElement.properties.minSize));
                    } else {
                        value = Math.max(value, getCakeDimension(parseInt(object.pastaElement.properties.minSize)));
                    }
                }
                if (typeof object.pastaElement.properties.maxSize === "string") {
                    if (oname.indexOf("cake") < 0) {
                        value = Math.min(value, parseFloat(object.pastaElement.properties.maxSize));
                    } else {
                        var size = getCakeDimension(parseInt(object.pastaElement.properties.maxSize));
                        if (typeof size !== "number")
                            size = value;
                        value = Math.min(value, size);
                    }
                }
                var preValue = object.scale.x;
                if (object.pastaElement.alignNormal) {
                    object.scale.y = value;
                    object.scale.x = value;
                } else {
                    object.scale.x = value;
                    object.scale.z = value;
                    if (oname.indexOf("cake") < 0
                        && oname.indexOf("plate") < 0
                        && oname.indexOf("side") < 0) {
                        object.scale.y = value;
                    } else if (oname.indexOf("cake") >= 0) {
                        if (object.pastaElement.hasParent('plate-0')) {
							              var scaleMul = value/preValue;
							              config.plateSize.min *= scaleMul;
							              config.plateSize.max *= scaleMul;
							              if (config.loading !== true) {
								                setPlateGraphicalProperties({
									                  scale: objects['plate-0'].scale.x*scaleMul
								                });
							              }
                        }
                    }
                }
            } else {
                retDecal = decalObjects.scale(retDecal, value);
            }
            // debug.log("Scale movee: "+ value);
        } else if (key === "texture" && !properties.decal && oname != "plate-0") {
            if (value === 0) {
                removeTexture(oname, color);
            } else if (typeof value === "string") {
                setTexture(oname, value);
            }
        } else if (key === "rotationY" &&
                   !(object.pastaElement.isSideDecoration ||
                     object.pastaElement.isRandom)) {
            if (!properties.decal) {
                if (object.pastaElement.isText3D) {
                    var rValue = value-object.pastaElement.pastaRotateValue;
                    debug.log("setGraphicalProperties: rotAxis "+ value +" "+ object.pastaElement.pastaRotateValue);
                    var rotAxis = new THREE.Vector3(
                        object.pastaElement.pastaRotationAxis.x,
                        object.pastaElement.pastaRotationAxis.y,
                        object.pastaElement.pastaRotationAxis.z
                    );
                    object.rotateOnAxis(rotAxis, rValue);
                    object.pastaElement.pastaRotateValue = value;
                    object.pastaElement.properties.rotationY = value;
                } else if (object.pastaElement.alignNormal) {
                    object.traverse(function (child) {
                        if (child instanceof THREE.Mesh) {
                            child.rotation.z = value;
                        }
                    });
                    object.pastaElement.pastaRotateValue = value;
                    object.pastaElement.properties.rotationY = value;
                } else {
                    object.rotation.y = value;
                }
            } else {
                retDecal = decalObjects.rotate(retDecal, value);
            }
        }
    });

    return retDecal;
}

function copyObject (obj) {
    var newObj = { };
    $.each(obj, function (key, value) {
        newObj[key] = value;
    });

    return newObj;
}

function createPlateMaterial (type) {
    var texture = new THREE.TextureLoader().load("/Images/Site/plate_texture.png");
    texture.wrapS = texture.wrapT = THREE.RepeatWrapping;
    var color = 0xcccccc;
    if (objects["plate-0"]) color = objects["plate-0"].deselectColor.getHex();
    if (type === 1 || type === 4) {
        texture.repeat.set(16, 1);
        var materialTop = new THREE.MeshPhongMaterial({
            color: color, shading: THREE.FlatShading,
            normalMap: gNormalMap,
            normalScale: new THREE.Vector2(0.2, 0.2)
        });
        materialTop.pastaColorable = true;
        var materialSide = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading, map: texture
        });
        var materialBottom = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading
        });
        materialBottom.pastaColorable = true;
        var materialsArray = [];
        materialsArray.push(materialSide); //materialindex = 0
        materialsArray.push(materialTop); // materialindex = 1
        materialsArray.push(materialBottom); // materialindex = 2
        return new THREE.MeshFaceMaterial(materialsArray);
    } else {
        texture.repeat.set(8, 1);
        var materialSideX1 = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading, map: texture
        });
        var materialSideX2 = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading, map: texture
        });
        var materialSideY1 = new THREE.MeshPhongMaterial({
            color: color, shading: THREE.FlatShading,
            normalMap: gNormalMap,
            normalScale: new THREE.Vector2(0.2, 0.2)
        });
        materialSideY1.pastaColorable = true;
        var materialSideY2 = new THREE.MeshPhongMaterial({
            color: 0x000000, shading: THREE.FlatShading
        });
        materialSideY2.pastaColorable = true;
        var materialSideZ1 = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading, map: texture
        });
        var materialSideZ2 = new THREE.MeshPhongMaterial({
            color: 0xffffff, shading: THREE.FlatShading, map: texture
        });
        var materialsArray = [];
        materialsArray.push(materialSideX1); //materialindex = 0
        materialsArray.push(materialSideX2); //materialindex = 0
        materialsArray.push(materialSideY1); //materialindex = 0
        materialsArray.push(materialSideY2); //materialindex = 0
        materialsArray.push(materialSideZ1); //materialindex = 0
        materialsArray.push(materialSideZ2); //materialindex = 0

        return new THREE.MeshFaceMaterial(materialsArray);
    }
}

function addPlate () {
    var materials = createPlateMaterial(1);
    var _scaler = 0.95;
    var geometry = new THREE.CylinderGeometry(1.6*_scaler, 1.6*_scaler, 0.15, 64);
    // geometry = new THREE.BoxGeometry(3, 0.15, 3);
    var cylinder = new THREE.Mesh(geometry, materials);

    cylinder.position.y = 0.075;
    cylinder.castShadow = true;
    cylinder.receiveShadow = true;
    var cylinderObject = new THREE.Object3D();
    cylinderObject.name = "plate-0";
    cylinderObject.add(cylinder);
    for (var k = 0; k < materials.materials.length; k++) {
        var m = materials.materials[k];
        if (m.pastaColorable) {
            debug.log("lorke");
            cylinderObject.deselectColor = m.color.clone();
            cylinderObject.deselectColor.setHex(0xcccccc);
        }
    }
    cylinderObject.position.x = config.center.x;
    appendObject("plate-0", cylinderObject);
    textObject.setPlate0(objects['plate-0']);
}

function changePlate (option) {
    var geometry;
    var _scaler = 0.97;
    if (option == 1) { geometry = new THREE.CylinderGeometry(1.6*_scaler, 1.6*_scaler, 0.15, 64); }  // round
    else if (option == 2) { geometry = new THREE.BoxGeometry(2.9*_scaler, 0.15, 2.9*_scaler); }          // square
    else if (option == 3) { geometry = new THREE.BoxGeometry(3.1*_scaler, 0.15, 2.6*_scaler); }          // rect
    else if (option == 4) { geometry = new THREE.CylinderGeometry(1.6*_scaler, 1.6*_scaler, 0.15, 64); }          // ellipse
    var plateObj = objects["plate-0"];
    var material = createPlateMaterial(option);

    var plate = new THREE.Mesh(geometry, material);
    plate.position.y = 0.075;
    if (option == 4) { plate.scale.z = 0.8; }
    plate.castShadow = true;
    plate.receiveShadow = true;
    plateObj.remove(objects["plate-0"].children[0]);
    plateObj.add(plate);
}

function setPlateGraphicalProperties (properties) {
    var object = objects["plate-0"];
    var child = object.children[0];
    for (var k = 0; k < child.material.materials.length; k++) {
        var m = child.material.materials[k];
        if (m.pastaColorable && properties.color) {
            m.color.setHex(properties.color);
            child.parent.deselectColor.setHex(properties.color);
            object.pastaElement.properties.color = properties.color;
        }
    }
    if (properties.scale) {
        if (typeof config.plateSize.min === "number" &&
            typeof config.plateSize.max === "number") {
            properties.scale = Math.max(
                config.plateSize.min,
                Math.min(config.plateSize.max, properties.scale)
            );
        }
        var scaler = 1.0;
        if (child.parent.scale.z < child.parent.scale.x) {
            scaler = 0.8;
        }
        child.parent.scale.x = properties.scale;
        child.parent.scale.z = properties.scale*scaler;
        object.pastaElement.properties.scale = properties.scale;
        object.pastaElement.updateScaleAndRotation(properties);
    }
    return;
}

function colorizeObject (name, imgURL, coordsData, colors, done) {
    var object = objects[name];

    if (object === undefined) {
        debug.log("colorizeObject: object "+ name +"not found");
        return;
    }

    var canvasElement = document.createElement("canvas");
    var context = canvasElement.getContext('2d');
    var imageObj = new Image();

    colorizedTextureID++;

    imageObj.onload = function () {
        context.drawImage(imageObj, 0, 0, 1024, 1024);
        canvasElement.width = imageObj.width;
        canvasElement.height = imageObj.height;
        var w = imageObj.width;
        var h = imageObj.height;

        var coords = coordsData;

        for (var key in coords) {
            var coordsArr = coords[key];
            for (var index = 0; index < coordsArr.length; index++) {
                var coordList = coordsArr[index];
                context.beginPath();
                var coord = coordList[0].split(",");
                if (coord.length !== 2) continue;
                context.moveTo(parseInt(coord[1]), parseInt(coord[0]));
                for (var k = 1; k < coordList.length; k++) {
                    coord = coordList[k].split(",");
                    if (coord.length !== 2) continue;
                    // console.log(coord);
                    context.lineTo(parseInt(coord[1]), parseInt(coord[0]));
                }
                context.fillStyle = colors[key];
                context.fill();
            }
        }
        context.drawImage(imageObj, 0, 0, imageObj.width, imageObj.height);

        var texture = new THREE.Texture(canvasElement);
        var textureId = "texture-"+ colorizedTextureID;

        appendTexture(textureId, texture);
		    object.pastaElement.properties.texture = textureId;

        if (colorizedTextures[name] === undefined) colorizedTextures[name] = new Array();
        colorizedTextures[name].push(textureId);
        if (typeof done === "function") {
            done();
        }
    };
    imageObj.src = imgURL;

    return colorizedTextureID;
};

function getCoords (coordsText) {
    var areas = coordsText.split(/\r?\n/);
    var areasJSON = {};
    for (var k = 0; k < areas.length; k++) {
        var area = areas[k];
        var tagAndCoord = area.split(":");
        if (tagAndCoord.length !== 2) continue;
        var tag = tagAndCoord[0];
        var cText = tagAndCoord[1];
        if (areasJSON[tag] === undefined) areasJSON[tag] = new Array();
        areasJSON[tag].push(cText.split(";"));
    }

    return areasJSON;
}