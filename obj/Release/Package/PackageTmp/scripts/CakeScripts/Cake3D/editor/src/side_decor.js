var THREE = require('three');
var debug = require('./debug');

module.exports = {
    createSideDecoration: function (decorBase, onToObject, delta) {
        return createSideDecoration(decorBase, onToObject, delta);
    }
}


function createSideDecoration (decorBase, onToObject, delta) {
    var bSphereRad = 0;
    var onToObjectPos = new THREE.Vector3().copy(onToObject.position);
    var decor = new THREE.Object3D();
    decor.position.copy(onToObjectPos);
    decor.updateMatrix();
    decor.updateMatrixWorld();
    var decorBaseBB = new THREE.Box3().setFromObject(decorBase[0]);
    var decorBaseSizeX = decorBaseBB.size().x;
    var angleInc = 0;
    var raycaster = new THREE.Raycaster();
    var rayOrigin = new THREE.Vector3().copy(onToObjectPos);
    rayOrigin.y = onToObject.position.y+0.02; // 0.01
    // rayOrigin.y += 0.05;
    var rayDirection = new THREE.Vector3();
    var intersects;
    var pointObject = new THREE.Vector3();
    var faceNObject = new THREE.Vector3();
    var decorBaseChildrenCnt = decorBase.length;
    var mY = new THREE.Matrix4().makeRotationY(onToObject.rotation.y);
    onToObject.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name === "ui-rotate") return;
            child.geometry.computeBoundingSphere();
            bSphereRad = child.geometry.boundingSphere.radius*onToObject.scale.x*1.01;
        }
    });
    angleInc = (decorBaseSizeX/bSphereRad/decorBaseChildrenCnt)/8;
    var prePointObject = undefined;
    var preBB = new Array(2);
    var preBS = new Array(2);
    var k = 0;
    var meshDummy;
    var firstY = null;
    var maxX = 0;
    for (kf = 0; kf < 2*Math.PI; kf += angleInc) {
        rayOrigin.x = onToObjectPos.x+bSphereRad*Math.cos(kf);
        rayOrigin.z = onToObjectPos.z+bSphereRad*Math.sin(kf);
        rayDirection.copy(onToObjectPos.clone().sub(rayOrigin));
        raycaster.set(rayOrigin, rayDirection.normalize());
        intersects = raycaster.intersectObject(onToObject, true);

        var bsSize;

        for (var l = 0; l < intersects.length; l++) {
            if (l === 2) break;
            if (intersects && intersects.length > 0) {
                pointObject.copy(intersects[l].point);
                if (firstY !== null) {
                    pointObject.y = firstY;
                } else {
                    firstY = pointObject.y;
                }
                faceNObject.copy(intersects[l].face.normal).applyMatrix4(mY);
                meshDummy = decorBase[k%decorBaseChildrenCnt];
                if (meshDummy instanceof THREE.Mesh) {
                    // var decorBaseMeshClone = meshDummy.clone();
                    var decorBaseMeshClone = new THREE.Mesh(
                        meshDummy.geometry.clone(),
                        meshDummy.material.clone()
                    );
                    // decorBaseMeshClone.scale.multiplyScalar(15);
                    // decorBaseMeshClone.geometry.computeBoundingBox();
                    decorBaseMeshClone.geometry.computeBoundingSphere();
                    bsSize = decorBaseMeshClone.geometry.boundingSphere.radius;
                    // var expandBy = new THREE.Vector3(-bbSize.x*0.1, 0, -bbSize.z*0.1);
                    // decorBaseMeshClone.geometry.boundingBox.expandByVector(expandBy);
                    // decorBaseMeshClone.geometry.boundingBox.min.add(pointObject);
                    // decorBaseMeshClone.geometry.boundingBox.max.add(pointObject);
                    // debug.log(decorBaseMeshClone.geometry.boundingSphere);
                    // if (preBB !== undefined) {
                    //     if (decorBaseMeshClone.geometry.boundingBox.intersectsBox(preBB)) {
                    //         debug.log("createSideDecoration: preBB intersects.");
                    //         continue;
                    //     } else {
                    //         debug.log("createSideDecoration: preBB does not intersects.");
                    //     }
                    // }
                    if (preBS[l]) {
                        // debug.log(pointObject.clone().sub(preBS[l]).add(decorBaseMeshClone.geometry.boundingSphere.center).length());
                        debug.log("Side Decor delta: "+ delta);
                        var d_ = decorBaseMeshClone.geometry.boundingSphere.radius*1.5;
                        if (typeof delta === "number") {
                            d_ = delta;
                        }
                        if (
                            pointObject.clone().sub(preBS[l]).length() < d_) {
                            debug.log("createSideDecoration: preBB intersects.");
                            continue;
                        } else {
                            debug.log("createSideDecoration: preBB does not intersects.");
                        }
                    }
                    decor.add(decorBaseMeshClone);
                    pointObject.y -= 0.0375;
                    decorBaseMeshClone.position.copy(decor.worldToLocal(pointObject.clone()));
                    if (decorBaseMeshClone.position.x > maxX) maxX = decorBaseMeshClone.position.x;
                    decorBaseMeshClone.lookAt(decor.worldToLocal(pointObject.clone().add(faceNObject)));
                    // preBB[l] = decorBaseMeshClone.geometry.boundingBox.clone();
                    preBS[l] = pointObject.clone();
                }
            }
            k++;
            // if (k === 2) break;
        }
    }
    plane = new THREE.Mesh(
        new THREE.PlaneBufferGeometry(2*maxX+bsSize, 2*maxX+bsSize, 8, 8),
        new THREE.MeshBasicMaterial( {
            color: 0x248f24, visible: false, alphaTest: 0
        }));
    plane.lookAt(new THREE.Vector3(0, 1, 0));
    plane.position.y += 0.01; // TODO make dynamic
    plane.name = "invisible-mesh";
    decor.add(plane);
    debug.log("K total: "+ k);
    return decor;
}
