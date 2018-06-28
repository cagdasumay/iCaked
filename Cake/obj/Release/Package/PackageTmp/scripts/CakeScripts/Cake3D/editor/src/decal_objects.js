var THREE = require('three');
var DecalGeometry = require('../node_modules/three-decal-geometry/index.js');
var debug = require('./debug');
var config = require('./config');
var text = require('./text');

DecalGeometry(THREE);

module.exports = {
    load: function (name, tmpIntersect, sources, onCake, done1, done2) {
        decalObjectsCurrent[name] = { };
        decalObjectsCurrent[name].sources = sources;
        decalObjectsCurrent[name].onTo = onCake.name;
        if (tmpIntersect !== null && typeof tmpIntersect !== "undefined") {
            config.tmpIntersect = tmpIntersect;
        } else {
            var raycaster = new THREE.Raycaster(
                onCake.position, new THREE.Vector3(0, 1, 0)
            );
            var intersects = raycaster.intersectObject(onCake, true);
            config.tmpIntersect = intersects[intersects.length-1];
        }
        decalObjectsCurrent[name].intersectionPoint = {
            x: config.tmpIntersect.point.x,
            y: config.tmpIntersect.point.y,
            z: config.tmpIntersect.point.z
        };
        decalObjectsCurrent[name].intersectionNormal = {
            x: config.tmpIntersect.face.normal.x,
            y: config.tmpIntersect.face.normal.y,
            z: config.tmpIntersect.face.normal.z
        };
        decalObjectsCurrent[name].scaleFactor = 1.0;
        decalObjectsCurrent[name].rotationFactor = 0.0;
        return load(name, config.tmpIntersect, sources, onCake, done1, done2);
    },
    moveTo: function (decal, intersection) {
        decalObjectsCurrent[decal.name].intersectionPoint = {
            x: intersection.point.x,
            y: intersection.point.y,
            z: intersection.point.z
        };
        decalObjectsCurrent[decal.name].intersectionNormal = {
            x: intersection.face.normal.x,
            y: intersection.face.normal.y,
            z: intersection.face.normal.z
        };
        decalObjectsCurrent[decal.name].onTo = intersection.object.parent.name;
        intersectionList[decal.name].point.copy(intersection.point);
        intersectionList[decal.name].face.normal.copy(intersection.face.normal);
        intersectionList[decal.name].object = intersection.object;
        return moveTo(decal,
                      intersection.point.clone(),
                      intersection.face.normal.clone(),
                      intersection.object.parent);
    },
    moveToWrapper: function (decal, decalObjectProps, onToObject) {
        decalObjectsCurrent[decal.name] = decalObjectProps;
        debug.log("moveToWrapper");
        debug.log(decalObjectsCurrent[decal.name]);
        var point = decalObjectProps.intersectionPoint;
        var normal = decalObjectProps.intersectionNormal;
        return moveTo(decal,
                      new THREE.Vector3(point.x, point.y, point.z),
                      new THREE.Vector3(normal.x, normal.y, normal. z),
                      onToObject);
    },
    moveToXZ: function (decal, point, parent) {
        debug.log("moveToXZ: debug");
        debug.log(decal);
        debug.log(parent);
        debug.log(point);
        debug.log(decalObjectsCurrent[decal.name]);
        return moveToXZ(decal, point, parent);
    },
    scale: function (decal, factor) {
        return scale(decal, factor);
    },
    rotate: function (decal, factor) {
        decalObjectsCurrent[decal.name].rotationFactor = factor;
        return rotate(decal, factor);
    },
    changeTexture: function (decal, sources) {
        return changeTexture(decal, sources);
    },
    getIntersectionPoint: function (name) {
        return intersectionPointList[name];
    },
    getDecalObjectsCurrent: function (name) {
        return decalObjectsCurrent[name];
    },
    reset: function () {
        reset();
    }
}

var up = new THREE.Vector3(0, 1, 0);
var r = new THREE.Vector3(0, 0, 0);
var s = new THREE.Vector3(1.0, 1.0, 1.0);
var p = new THREE.Vector3(0, 1, 0);
var check = new THREE.Vector3(1, 1, 1);
var intersectionList = { };
var scaleList = { };
var rotationList = { };
var intersectionPointList = { };
var decalNormal;
var decalObjectsCurrent = {};

function reset() {
    up = new THREE.Vector3(0, 1, 0);
    r = new THREE.Vector3(0, 0, 0);
    s = new THREE.Vector3(1.0, 1.0, 1.0);
    p = new THREE.Vector3(0, 1, 0);
    check = new THREE.Vector3(1, 1, 1);
    intersectionList = {};
    scaleList = {};
    rotationList = {};
    intersectionPointList = {};
    decalNormal;
    decalObjectsCurrent = {};
}

function getParentObjectColor (object) {
	  if (object.deselectColor instanceof THREE.Color)
		    return object.deselectColor.getHex();
	  else
		    return 0xffffff;
	  mat = object.children[0].material;
	  if (mat instanceof THREE.MultiMaterial)
		    return mat.materials[1].color.getHex();
	  else
		    return mat.color.getHex();
}

function load (name, tmpIntersect, sources, object, done1, done2) {
    var mesh, geometry;
	  var pColor = getParentObjectColor(object);
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate") {
				        mesh = child;
			      }
        }
    });
    var decalMap, decalNormal;

    if (sources.url !== undefined) {
        var textureLoader = new THREE.TextureLoader();
        debug.log("sources.url: "+ sources.url);
        var urlInfo = sources.url.split(".");
        urlInfo[urlInfo.length-2] += '_normal';
        // decalNormal = textureLoader.load(urlInfo.join('.'))
        // debug.log(decalNormal);
        decalMap = textureLoader.load(sources.url, function (decalMap) {
            decalMap.needsUpdate = true;

            var decalMaterial = new THREE.MeshPhongMaterial({
                map: decalMap,
                color: 0xffffff,
                shininess: 1,
				        shading: THREE.SmoothShading, // side: THREE.DoubleSide,
				        transparent: true,
                        opacity: 0.9
            });

            // debug.log(decalMaterial);

            var whRatio = decalMaterial.map.image.width/decalMaterial.map.image.height;
            // var whRatio = 236/309;
            var pointOnObject = new THREE.Vector3();
            pointOnObject.copy(tmpIntersect.point);

            var p_ = new THREE.Vector3().copy(pointOnObject);

            // var material = decalMaterial.clone();
            mouseHelper = new THREE.Mesh(
                new THREE.BoxGeometry( 1, 1, 1 ),
                new THREE.MeshNormalMaterial()
            );
            mouseHelper.visible = false;
            mouseHelper.position.copy( p_ );
            var lookAtPos = new THREE.Vector3();
            lookAtPos.copy(pointOnObject).add(tmpIntersect.face.normal);
            mouseHelper.lookAt( lookAtPos );
            var r_ = new THREE.Vector3().copy( mouseHelper.rotation );
            debug.log(r_);
            debug.log(tmpIntersect.face.normal);
            // material.color.setHex( 0xffffff );
            scaleList[name] = s.clone();
            scaleList[name].x = whRatio;
            // s.copy(scaleList[name]);
            rotationList[name] = r_.z;
            var s_ = new THREE.Vector3().copy(scaleList[name]);
            var decalGeometry = new THREE.DecalGeometry( mesh, p_, r_, s_, check, object );
            debug.log(decalGeometry);
            var m = new THREE.Mesh( decalGeometry, decalMaterial );
            var obj = new THREE.Object3D();
            obj.add(m);
            obj.name = name;
			      obj.deselectColor = new THREE.Color(pColor);
            obj["isDecal"] = true;
            intersectionList[name] = tmpIntersect;
            intersectionPointList[name] = tmpIntersect.point.clone();
            if (typeof done1 === "function") {
                done1(obj);
            }
            if (typeof done2 === "function") {
                done2(name);
            }
        });
        // debug.log(decalNormal);
    } else {
        decalMap = text.texture(
            sources.text,
            sources.curveType,
            { color: sources.color, font: sources.font }
        );
    }
    // decalMap.wrapS = THREE.RepeatWrapping;
    // decalMap.wrapT = THREE.RepeatWrapping;
    // decalMap.repeat.set( 1, 1 );
    // decalNormal.offset.set(0.5, 0.5);

}

function changeTexture (decal, sources) {
    var mesh, decalMesh;
    var intersection = intersectionList[decal.name];
    var object = intersection.object.parent;
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                mesh = child
        }
    });
    decal.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                decalMesh = child
        }
    });
    debug.log(intersection);

    if (sources.url !== undefined) {
        var textureLoader = new THREE.TextureLoader();
        debug.log("sources.url: "+ sources.url);
        decalNormal = textureLoader.load(sources.url);
    } else {
        debug.log("sources.text: "+ sources.text);
        decalNormal = text.texture(
            sources.text,
            sources.curveType,
            { color: sources.color, font: sources.font }
        );
    }
    decalNormal.wrapS = THREE.RepeatWrapping;
    decalNormal.wrapT = THREE.RepeatWrapping;
    decalNormal.repeat.set( 1, 1 );
    // decalNormal.offset.set(0.5, 0.5);
    decalNormal.needsUpdate = true;

    var decalMaterial = new THREE.MeshPhongMaterial({
        map: decalNormal,
        // normalScale: new THREE.Vector2(1, 1),
        color: 0xffffff,
        shininess: 1,
        transparent: true,
        depthTest: true,
        depthWrite: false,
        polygonOffset: true,
        polygonOffsetFactor: -4,
        wireframe: false
    });

    var pointOnObject = new THREE.Vector3();
    pointOnObject.copy(intersection.point);

    p.copy(pointOnObject);

    mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(pointOnObject).add(intersection.face.normal);
    mouseHelper.lookAt( lookAtPos );
    r.copy( mouseHelper.rotation );
    debug.log("Face normal.");
    debug.log(intersection.face.normal);
    // material.color.setHex( 0xffffff );
    r.z = rotationList[decal.name];
    s = scaleList[decal.name].clone();
    var decalGeometry = new THREE.DecalGeometry( mesh, p, r, s, check, object );
    debug.log(decalGeometry);
    var m = new THREE.Mesh( decalGeometry, decalMaterial );
    decal.remove(decalMesh);
    decal.add(m);
    decal.matrixAutoUpdate = true;
    decal.matrixWordlNeedsUpdate = true;

    return decal;
}

function moveTo (decal, intersectionPoint, intersectionNormal, intersectionParent) {
    // return;
    var mesh, decalMesh;
    var object = intersectionParent;
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                mesh = child
        }
    });
    decal.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                decalMesh = child
        }
    });

    var pointOnObject = new THREE.Vector3();
    pointOnObject.copy(intersectionPoint);

    var p_ = new THREE.Vector3().copy(pointOnObject);

    var mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p_ );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(pointOnObject).add(intersectionNormal);
    mouseHelper.lookAt( lookAtPos );
    var r_ = new THREE.Vector3().copy( mouseHelper.rotation );
    debug.log("Face normal.");
    debug.log(intersectionNormal);

    // r.z = rotationList[decal.name] || rf || 0.0;
    // s = scaleList[decal.name].clone();
    r_.z = decalObjectsCurrent[decal.name].rotationFactor || 0.0;
    var scaleFactor = decalObjectsCurrent[decal.name].scaleFactor || 1.0;
    // s = new THREE.Vector3(scaleFactor, scaleFactor, scaleFactor);
    // s.copy(scaleList[decal.name]); // .multiplyScalar(scaleFactor);
    var s_ = new THREE.Vector3().copy(scaleList[decal.name]);
    var decalGeometry = new THREE.DecalGeometry( mesh, p_, r_, s_, check, object );
    debug.log(decalGeometry);

    var material = new THREE.MeshPhongMaterial();
    var m = new THREE.Mesh( decalGeometry, mesh.material );
    m.material = decalMesh.material.clone();
    decal.remove(decalMesh);
    decal.add(m);
    decal.matrixAutoUpdate = true;
    decal.matrixWordlNeedsUpdate = true;
    intersectionPointList[decal.name].copy(intersectionPoint);

    debug.log("moveTo: position");
    debug.log(decal.position);

    return decal;
}

function moveToXZ (decal, point) {
    // return;
    var mesh, decalMesh;
    var intersection = intersectionList[decal.name];
    var intersectionPoint = intersectionPointList[decal.name].clone();
    var object = intersection.object.parent;
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                mesh = child
        }
    });
    decal.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                decalMesh = child
        }
    });
    debug.log(intersection);

    var pointOnObject = new THREE.Vector3();
    pointOnObject.copy(point);
    // pointOnObject.x = point.x;
    // pointOnObject.y = point.y;
    // pointOnObject.z = point.z;
    intersectionPoint.copy(point);

    p.copy(pointOnObject); // .add(intersectionPointDelta[decal.name]);

    mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(pointOnObject).add(intersectionNormal);
    mouseHelper.lookAt( lookAtPos );
    r.copy( mouseHelper.rotation );

    r.z = rotationList[decal.name];
    s = scaleList[decal.name].clone();
    var decalGeometry = new THREE.DecalGeometry( mesh, p, r, s, check, object );
    debug.log(decalGeometry);

    var material = new THREE.MeshPhongMaterial();
    var m = new THREE.Mesh( decalGeometry, mesh.material );
    m.material = decalMesh.material.clone();
    decal.remove(decalMesh);
    decal.add(m);
    decal.matrixAutoUpdate = true;
    decal.matrixWordlNeedsUpdate = true;

    return decal;
}

function scale (decal, factor) {
    var mesh, decalMesh;
    intersection = intersectionList[decal.name];
    var object = intersection.object.parent;
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                mesh = child;
        }
    });
    decal.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                decalMesh = child;
        }
    });
    debug.log(intersection);

    var pointOnObject = new THREE.Vector3();
    pointOnObject.copy(intersectionPointList[decal.name]);

    var p_ = new THREE.Vector3().copy(pointOnObject);

    mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p_ );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(pointOnObject).add(intersection.face.normal);
    mouseHelper.lookAt( lookAtPos );
    var r_ = new THREE.Vector3().copy( mouseHelper.rotation );
    debug.log("Face normal.");
    debug.log(intersection.face.normal);

    // factor *= 0.5;
    // s.set(factor, factor, factor);
    debug.log("decalObjects.scale");
    debug.log(scaleList[decal.name]);
    debug.log(factor);
    debug.log(decalObjectsCurrent[decal.name].scaleFactor);
    var s_ = new THREE.Vector3().
        copy(scaleList[decal.name]).
        multiplyScalar(factor/decalObjectsCurrent[decal.name].scaleFactor);
    decalObjectsCurrent[decal.name].scaleFactor = factor;
    r_.z = rotationList[decal.name];
    scaleList[decal.name].copy(s_);
    var decalGeometry = new THREE.DecalGeometry( mesh, p_, r_, s_, check, object );
    debug.log(decalGeometry);
    var material = new THREE.MeshPhongMaterial();
    var m = new THREE.Mesh( decalGeometry, mesh.material );
    m.material = decalMesh.material.clone();
    decal.remove(decalMesh);
    decal.add(m);
    decal.matrixAutoUpdate = true;
    decal.matrixWordlNeedsUpdate = true;

    return decal;
}

function rotate (decal, factor) {
    var mesh, decalMesh;
    intersection = intersectionList[decal.name];
    var object = intersection.object.parent;
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                mesh = child
        }
    });
    decal.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate")
                decalMesh = child
        }
    });
    debug.log(intersection);

    var pointOnObject = new THREE.Vector3();
    pointOnObject.copy(intersectionPointList[decal.name]);

    var p_ = new THREE.Vector3().copy(pointOnObject);

    mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p_ );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(pointOnObject).add(intersection.face.normal);
    mouseHelper.lookAt( lookAtPos );
    var r_ = new THREE.Vector3().copy( mouseHelper.rotation );
    debug.log("Face normal.");
    debug.log(intersection.face.normal);

    // s.set(factor, factor, factor);
    // r.y += factor;
    r_.z = factor;
    rotationList[decal.name] = factor;
    var s_ = new THREE.Vector3().copy(scaleList[decal.name]);
    var decalGeometry = new THREE.DecalGeometry( mesh, p_, r_, s_, check, object );
    debug.log(decalGeometry);
    var material = new THREE.MeshPhongMaterial();
    var m = new THREE.Mesh( decalGeometry, mesh.material );
    m.material = decalMesh.material.clone();
    decal.remove(decalMesh);
    decal.add(m);
    decal.matrixAutoUpdate = true;
    decal.matrixWordlNeedsUpdate = true;

    return decal;
}
