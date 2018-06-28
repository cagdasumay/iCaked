var THREE = require('three');
var debug = require('./debug');
var DecalGeometry = require('../node_modules/three-decal-geometry/index.js');

DecalGeometry(THREE);

module.exports = {
    texture: function (text, curve, options) {
        if (options === undefined) {
            options = {};
            options["color"] = "rgba(0, 0, 0, 255)",
            options["font"] = "48px Times New Roman",
            options["background"] = "rgba(255, 255, 255, 0)";
        }
        delete canvas;
        delete ctx;
        return new THREE.Texture(startIt(text, curve, options));
    },
    text3d: function (name, text, font, size, fontId, style, done) {
        text3DCurrent[name] = {
            text: text,
            font: fontId,
            size: size,
            style: style
        };
        var obj = createText(text, font, size, fontId, style);
        obj.name = name;
        return obj;
    },
    moveText3D: function (object, intersects, designMeshes) {
        text3DCurrent[object.name].intersectsPoint = {
            x: intersects.point.x,
            y: intersects.point.y,
            z: intersects.point.z
        };
        text3DCurrent[object.name].intersectsNormal = {
            x: intersects.face.normal.x,
            y: intersects.face.normal.y,
            z: intersects.face.normal.z
        };
        text3DCurrent[object.name].intersectsObjectPos = {
            x: intersects.object.parent.position.x,
            y: intersects.object.parent.position.y,
            z: intersects.object.parent.position.z
        };
        text3DCurrent[object.name].onTo = intersects.object.parent.name;
        moveText3D(object,
                   intersects.point.clone(),
                   intersects.face.normal.clone(),
                   intersects.object.parent.position.clone(),
                   intersects.object.parent
                  );
    },
    moveText3DWrapper: function (object, point, normal, onToObject) {
        text3DCurrent[object.name].intersectsPoint = {
            x: point.x,
            y: point.y,
            z: point.z
        };
        text3DCurrent[object.name].intersectsNormal = {
            x: normal.x,
            y: normal.y,
            z: normal.z
        };
        text3DCurrent[object.name].intersectsObjectPos = {
            x: onToObject.position.x,
            y: onToObject.position.y,
            z: onToObject.position.z
        };
        text3DCurrent[object.name].onTo = onToObject.name;
        moveText3D(
            object,
            new THREE.Vector3(point.x, point.y, point.z),
            new THREE.Vector3(normal.x, normal.y, normal.z),
            onToObject.position.clone(),
            onToObject,
            true
        );
    },
    getText3DCurrent: function (name) {
        return text3DCurrent[name];
    },
    reset: function () {
        reset();
    },
    setPlate0: function (p) {
        plate0 = p;
    }
}

var canvas, ctx, points;
var canvasSize = 512;
var curves = [
    "24,437,74,74,437,74,477,437",
    "24,75,74,438,437,438,477,75",
    "0,156,174,356,337,24,480,306"
];
var gMaxW, gMinW, gMaxH, gMinH;

var text3DCurrent = {};
var plate0;

function reset() {
    canvas = null, ctx = null, points = null;
    text3DCurrent = {};
}

function moveText3D (object,
                     intersectsPoint, intersectsNormal,
                     intersectsObjectPos, intersectsParent,
                     fromWrapper
                    ) {
    var raycaster;
    var mY = new THREE.Matrix4().makeRotationY(intersectsParent.rotation.y);
    var meshIntersects = [];
    object.position.copy(intersectsPoint);
	  var lockRotationPre = object.pastaElement.lockRotation;
    var direction = new THREE.Vector3().copy(intersectsNormal).applyMatrix4(mY).normalize();
    var lookAtVec = new THREE.Vector3().copy(direction).add(intersectsPoint);
    if (Math.abs(intersectsNormal.y-1) < 1e-2) {
		    intersectsNormal.y = 1;
		    intersectsNormal = intersectsNormal.normalize();
		    if (object.pastaElement.lockRotation === false) {
			      intersectsNormal.x = 0;
			      intersectsNormal.z = 0;
			      direction = new THREE.Vector3().copy(intersectsNormal).applyMatrix4(mY).normalize();
			      lookAtVec = new THREE.Vector3().copy(direction).add(intersectsPoint);
			      object.lookAt(lookAtVec);
			      object.pastaElement.pastaRotateValue = 0;
			      object.pastaElement.properties.rotationY = 0;
		    }
		    object.pastaElement.lockRotation = true;
    } else {
		    object.pastaElement.lockRotation = false;
		    // lookAtVec = new THREE.Vector3().copy(new THREE.Vector3(0, 1, 0)).add(intersectsPoint);
    }
	  if (object.pastaElement.lockRotation === false)
		    object.lookAt(lookAtVec);
    var rotTmp = object.worldToLocal(lookAtVec.clone()).normalize();
    object.pastaElement.pastaRotationAxis.x = rotTmp.x;
    object.pastaElement.pastaRotationAxis.y = rotTmp.y;
    object.pastaElement.pastaRotationAxis.z = rotTmp.z;
    object.pastaElement.lookAtData.normal.x = intersectsNormal.x;
    object.pastaElement.lookAtData.normal.y = intersectsNormal.y;
    object.pastaElement.lookAtData.normal.z = intersectsNormal.z;
    object.pastaElement.lookAtData.point.x = object.position.x;
    object.pastaElement.lookAtData.point.y = object.position.y;
    object.pastaElement.lookAtData.point.z = object.position.z;
    
    var objectRot = new THREE.Vector3();
    objectRot.copy(object.rotation);
    object.updateMatrix();
    object.updateMatrixWorld();
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh && child.name.split("-")[0] === "letter") {
            var childPos = object.localToWorld(
                child.initialPosition.clone()
            );
            var directionScaled = new THREE.Vector3().copy(direction).multiplyScalar(12);
            // directionScaled.y = direction.y;
            var rayOrigin = childPos.clone().add(directionScaled); // .add(directionScaled);
            debug.log(rayOrigin);
            var delta = intersectsObjectPos.clone().sub(childPos);
            var faceObject, pointObject, meshIntersect;
            raycaster = new THREE.Raycaster(
                rayOrigin, direction.clone().multiplyScalar(-1).normalize()
            );
            meshIntersects = raycaster.intersectObject(intersectsParent, true);
            var preBoundingBoxDelta = child.boundingBoxDeltaX;
            var distanceZ = child.distanceZ;
            if (meshIntersects === undefined || meshIntersects.length === 0) {
                debug.log("No intersects from outside.");
                rayOrigin = intersectsObjectPos.clone();
                delta = childPos.clone().sub(rayOrigin);
                raycaster = new THREE.Raycaster(
                    rayOrigin, delta.normalize()
                );
                meshIntersects = raycaster.intersectObject(intersectsParent, true);
                if (meshIntersects === undefined || meshIntersects.length === 0) {
                    debug.log("No intersects from inside.");
                    return;
                }
            }
            for (k = 0; k < meshIntersects.length; k++) {
                meshIntersect = meshIntersects[k];
                break;
            }
            debug.log(meshIntersect.point);
            faceNObject = meshIntersect.face.normal.clone();
            var m = new THREE.Matrix4();

            var m1 = new THREE.Matrix4();
            var m2 = new THREE.Matrix4();
            var m3 = new THREE.Matrix4();

            m1.makeRotationX( intersectsParent.rotation.x );
            m2.makeRotationY( intersectsParent.rotation.y );
            m3.makeRotationZ( intersectsParent.rotation.z );

            m.multiplyMatrices( m1, m2 );
            m.multiply( m3 );
            faceNObject.applyMatrix4(m);
            pointObject = meshIntersect.point.clone();
            child.position.copy(object.worldToLocal(pointObject.clone()));
            child.lookAt(object.worldToLocal(pointObject.clone().add(faceNObject)));
            var dParent = child.position.clone().sub(child.parent.position);
            object.traverse(function (sel) {
                if (sel instanceof THREE.Mesh && sel.name === "sel-"+ child.name) {
                    sel.position.copy(child.position);
                    sel.rotation.copy(child.rotation);
                }
            });
        } else if (child instanceof THREE.Mesh && child.name === "gofret") {
            if (object.pastaElement.lockRotation === false || true) {
                var newChild = new THREE.Mesh(moveGofret(
                    object.pastaElement.parent.object, intersectsPoint.clone(),
                    intersectsNormal.clone(), s.clone(), check.clone()
                ), child.material);
                newChild.name = "gofret";
                object.remove(child);
                newChild.lookAt(object.worldToLocal(lookAtVec.clone()));
                newChild.position.copy(object.worldToLocal(intersectsPoint.clone()));
                // newChild.rotation.x -= Math.PI/2;
                object.add(newChild);
                // child.position.set(0, 0, 0);
            }
        }
    });
}

function createText(text, font, size, fontId, _style) {
    var style = _style || 0;
    if (fontId === undefined) fontId = -1;
    debug.log("fontId: "+ fontId);
    var object = new THREE.Object3D();

    var charArray = text.split('');

    var preBoundingBoxDeltaX = 0;
    var centerOffset = 0;
    for (k = 0; k < charArray.length; k++) {
        var charOffsetIndex = k-charArray.length/2;
        var dada = k-(charArray.length/2);
        if (charArray[k].replace(/^\s+/, '').replace(/\s+$/, '') === '') {
            centerOffset += preBoundingBoxDeltaX;
            continue;
        }
        var textGeo = new THREE.TextGeometry( charArray[k], {
            font: font,
            size: size,
            height: 0.015,
            curveSegments: 10,
            bevelThickness: 0.0075,
            bevelSize: 0.0075,
            bevelEnabled: true,
            material: 0,
            extrudeMaterial: 1
        });

        if (style === 4) {
            textGeo.translate(0, 0, 0.005);
        }

        var selectorMesh = new THREE.Mesh(
            new THREE.PlaneGeometry(0.15, 0.15, 8).translate(0.07, 0.07, 0.01),
            new THREE.MeshPhongMaterial({ color: 0xffffff, transparent: true, opacity: 0.0 })
        );

        textGeo.computeBoundingBox();
        textGeo.computeVertexNormals();

        var material = new THREE.MeshPhongMaterial( {
            color: 0xffffff, shading: THREE.SmoothShading
        } );

        var boundingBoxDeltaX = textGeo.boundingBox.max.x - textGeo.boundingBox.min.x;
        if (textTinyChars(charArray[k]) && fontId !== 0) {
            boundingBoxDeltaX = 0.0625;
        }
        var boundingBoxDeltaY = textGeo.boundingBox.max.y - textGeo.boundingBox.min.y;
        var boundingBoxDeltaZ = textGeo.boundingBox.max.z - textGeo.boundingBox.min.z;

        if (textTinyChars(charArray[k])) {
            centerOffset += ( preBoundingBoxDeltaX )*1.2;
        } else {
            centerOffset += ( preBoundingBoxDeltaX )*1.2;
        }

        textGeo.applyMatrix(new THREE.Matrix4().makeTranslation(
            -boundingBoxDeltaX/2*0, 0, 0
        ));

        var textMesh1 = new THREE.Mesh( textGeo, material );
        var hover = 0.0;

        textMesh1.position.x = centerOffset;
        textMesh1.position.y = hover;
        textMesh1.position.z = 0;
        selectorMesh.position.x = centerOffset;
        selectorMesh.position.y = hover;
        selectorMesh.position.z = 0;

        textMesh1["boundingBoxDeltaX"] = boundingBoxDeltaX;
        textMesh1.name = "letter-"+ k;
        selectorMesh.name = "sel-letter-"+ k;
        selectorMesh.invisivleMesh = true;
        object.add( textMesh1 );
        object.add(selectorMesh);
        if (textTinyChars(charArray[k]) && false) {
            preBoundingBoxDeltaX = 0.125;
        } else {
            preBoundingBoxDeltaX = boundingBoxDeltaX;
        }
    }
    object.traverse(function (child) {
        if (child instanceof THREE.Mesh && child.name.split("-")[0] === "letter") {
            child.position.x -= centerOffset/2;
            child.position.y = characterY(child.position.x, style);
            child.distanceZ = child.position.y;
            child["initialPosition"] = object.worldToLocal(child.position.clone());
            // var posTmp = child.initialPosition.y;
            // child.initialPosition.y = child.initialPosition.z;
            // child.initialPosition.z = posTmp;
        } else if (child instanceof THREE.Mesh && child.name.split("-")[0] === "sel") {
            child.position.x -= centerOffset/2;
            child.position.y = characterY(child.position.x, style);
        }
    });
    object.lookAt(new THREE.Vector3(0, 1, 0).add(object.position));

    if (style === 4) {
        var gofret = addGofret(
            plate0, object.position.clone(),
            object.rotation.clone(), s.clone(), check.clone()
        );
        gofret.lookAt(object.worldToLocal(new THREE.Vector3(0, 1, 0)));
        object.add(gofret);
        var geometry = new THREE.BoxGeometry( 1, 0.5, 0.25 );
        var materiall = new THREE.MeshBasicMaterial( {color: 0x00ff00} );
        var cube = new THREE.Mesh( geometry, materiall );
        // object.add(cube);
    }

    return object;
}

function textTinyChars (c) {
    if (/[Iİiılj]/g.test(c)) {
        return true;
    } else {
        return false;
    }
}

// var up = new THREE.Vector3(0, 1, 0);
// var r = new THREE.Vector3(0, 0, 0);
var s = new THREE.Vector3(1.25, 0.25, 1.0);
// var p = new THREE.Vector3(0, 1, 0);
var check = new THREE.Vector3(1, 1, 1);

function addGofret (onToObject, p, r, s, check) {
    var mesh;
    onToObject.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate") {
				        mesh = child;
			      }
        }
    });

    var decalMaterial = new THREE.MeshPhongMaterial({
        color: 0xffffff,
        shininess: 1,
				shading: THREE.SmoothShading, side: THREE.DoubleSide,
				transparent: true
    });



    var decalGeometry = new THREE.DecalGeometry(mesh, p, r, s, check, onToObject);

    // decalGeometry.rotateX(-r.x);
    // decalGeometry.translate(0, 0, -0.15);

    var ret = new THREE.Mesh(decalGeometry, decalMaterial);
    ret.name = "gofret";
    return ret;
}

function moveGofret (onToObject, p, r, s, check) {
    var mesh;
    onToObject.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            if (child.parent.name !== "ui-rotate") {
				        mesh = child;
			      }
        }
    });

    var mouseHelper = new THREE.Mesh( new THREE.BoxGeometry( 1, 1, 1 ), new THREE.MeshNormalMaterial() );
    mouseHelper.visible = false;
    mouseHelper.position.copy( p );
    var lookAtPos = new THREE.Vector3();
    lookAtPos.copy(p).add(r);
    mouseHelper.lookAt( lookAtPos );
    r.copy( mouseHelper.rotation );

    var decalGeometry = new THREE.DecalGeometry(mesh, p, r, s, check, onToObject);

    // decalGeometry.rotateX(-r.x);
    decalGeometry.translate(0*-p.x, 0*p.z, 0);
    // decalGeometry.translate(-p.z, -p.z, 0);


    return decalGeometry;
}

function characterY (x, f) {
    if (f === 0) {
        return 0;
    } else if (f === 1) {
        return Math.abs(x);
    } else if (f === 2) {
        return x*x*0.6;
    } else if (f === 3) {
        return x*x*x*1.5;
    } else {
        return 0;
    }
}

function startIt(curveText, curve, options)
{
    canvas = document.createElement('canvas');
    // $(canvas).css('opacity', '0.0');
    // canvas.style.opacity = '0.2';
    canvas.width = canvasSize;
    canvas.height = canvasSize;
    ctx = canvas.getContext('2d');
    // debug.log("Lalalalklkl");
    // debug.log(size);
    ctx = canvas.getContext('2d');
    ctx.fillStyle = options.background;
    ctx.fillRect(0, 0, canvasSize, canvasSize);
    ctx.fillStyle = options.color;
    ctx.font = options.font;
    // ctx.font = "58px Times New Roman";
    // curve = "99.2,177.2,130.02,60.0,300.5,276.2,300.7,176.2";
    // curveText = "testing 1234567890";

    // calculateSize(curveText, curves[parseInt(curve)]);
    changeCurve(curveText, curves[parseInt(curve)]);

    var canvasRet = document.createElement('canvas');
    // $(canvas).css('opacity', '0.0');
    // canvas.style.opacity = '0.2';
    canvasRet.width = gMaxW-gMinW;
    canvasRet.height = gMaxH-gMinH;
    // canvasRet.width = 512;
    // canvasRet.height = 512;
    var ctxRet = canvasRet.getContext('2d');

    ctxRet.drawImage(
        canvas, gMinW, gMinH,
        canvasRet.width, canvasRet.height,
        0, 0, canvasRet.width, canvasRet.height
    );
    // ctxRet.drawImage(canvas, 0, 0, canvasRet.width, canvasRet.height);

    return canvasRet;
}

function calculateSize (curveText, curve) {
    points = curve.split(',');
    if (points.length == 8)
        drawStack(curveText, false);
}

function changeCurve(curveText, curve)
{
    points = curve.split(',');
    if (points.length == 8)
        drawStack(curveText, true);
}

function drawStack(curveText, draw)
{
    Ribbon = {
        maxChar: 50, startX: points[0], startY: points[1],
        control1X: points[2], control1Y: points[3],
        control2X: points[4], control2Y: points[5],
        endX: points[6], endY: points[7]
    };

    if (draw || true) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.save();
        // delete
        // ctx.beginPath();
    }
    ctx.moveTo(Ribbon.startX,Ribbon.startY);

    if (draw || true) {
        // ctx.bezierCurveTo(
        //   Ribbon.control1X,Ribbon.control1Y,
        //   Ribbon.control2X,Ribbon.control2Y,
        //   Ribbon.endX,Ribbon.endY
        // );
        //
        // ctx.stroke();
        // !delete
        ctx.restore();
    }

    return FillRibbon(curveText, Ribbon, draw);
}

function FillRibbon(text, Ribbon, draw)
{

    var textCurve = [];
    var ribbon = text.substring(0,Ribbon.maxChar);
    var curveSample = 1000;
    var maxH = 0, minH = 1e12, maxW = 0, minW = 1e12;
    var bH, bW;

    xDist = 0;
    var i = 0;
    for (i = 0; i < curveSample; i++)
    {
        a = new bezier2(i/curveSample,Ribbon.startX,Ribbon.startY,Ribbon.control1X,Ribbon.control1Y,Ribbon.control2X,Ribbon.control2Y,Ribbon.endX,Ribbon.endY);
        b = new bezier2((i+1)/curveSample,Ribbon.startX,Ribbon.startY,Ribbon.control1X,Ribbon.control1Y,Ribbon.control2X,Ribbon.control2Y,Ribbon.endX,Ribbon.endY);
        c = new bezier(a,b);
        textCurve.push({bezier: a, curve: c.curve});
    }

    letterPadding = ctx.measureText(" ").width / 4;
    w = ribbon.length;
    ww = Math.round(ctx.measureText(ribbon).width);


    totalPadding = (w-1) * letterPadding;
    totalLength = ww + totalPadding;
    p = 0;

    cDist = textCurve[curveSample-1].curve.cDist;

    z = (cDist / 2) - (totalLength / 2);

    for (i=0;i<curveSample;i++)
    {
        if (textCurve[i].curve.cDist >= z)
        {
            p = i;
            break;
        }
    }
    // p = 0;
    for (i = 0; i < w ; i++)
    {
        if (draw || true) ctx.save();
        bH = textCurve[p].bezier.point.y;
        bW = textCurve[p].bezier.point.x;
        debug.log("bW bH: "+ bW +" "+ bH);
        if (bH > maxH) maxH = bH;
        if (bH < minH) minH = bH;
        if (bW > maxW) maxW = bW;
        if (bW < minW) minW = bW;
        if (draw || true) {
            ctx.translate(bW, bH);
            ctx.rotate(textCurve[p].curve.rad);
            ctx.fillText(ribbon[i],0,0);
            ctx.restore();
        }
        x1 = ctx.measureText(ribbon[i]).width + letterPadding ;
        x2 = 0;
        for (j=p;j<curveSample;j++)
        {
            x2 = x2 + textCurve[j].curve.dist;
            if (x2 >= x1)
            {
                p = j;
                break;
            }
        }
    }
    var textSize = ctx.measureText("O");
    // if (!draw) {
    debug.log("max min H: "+ minW +" "+ minH +" "+ maxW +" "+ maxH);
    gMinW = Math.max(0, minW-textSize.width);
    gMinH = Math.max(0, minH-textSize.width);
    gMaxW = Math.min(canvasSize, maxW+textSize.width);
    gMaxH = Math.min(canvasSize, maxH+textSize.width/4);
    // }
} //end FillRibon

function bezier(b1, b2)
{
    //Final stage which takes p, p+1 and calculates the rotation, distance on the path and accumulates the total distance
    this.rad = Math.atan(b1.point.mY/b1.point.mX);
    this.b2 = b2;
    this.b1 = b1;
    dx = (b2.x - b1.x);
    dx2 = (b2.x - b1.x) * (b2.x - b1.x);
    this.dist = Math.sqrt( ((b2.x - b1.x) * (b2.x - b1.x)) + ((b2.y - b1.y) * (b2.y - b1.y)) );
    xDist = xDist + this.dist;
    this.curve = {rad: this.rad, dist: this.dist, cDist: xDist};
}

function bezierT(t,startX, startY,control1X,control1Y,control2X,control2Y,endX,endY)
{
    //calculates the tangent line to a point in the curve; later used to calculate the degrees of rotation at this point.
    this.mx = (3*(1-t)*(1-t) * (control1X - startX)) + ((6 * (1-t) * t) * (control2X - control1X)) + (3 * t * t * (endX - control2X));
    this.my = (3*(1-t)*(1-t) * (control1Y - startY)) + ((6 * (1-t) * t) * (control2Y - control1Y)) + (3 * t * t * (endY - control2Y));
}

function bezier2(t,startX, startY,control1X,control1Y,control2X,control2Y,endX,endY)
{
    //Quadratic bezier curve plotter
    this.Bezier1 = new bezier1(t,startX,startY,control1X,control1Y,control2X,control2Y);
    this.Bezier2 = new bezier1(t,control1X,control1Y,control2X,control2Y,endX,endY);
    this.x = ((1 - t) * this.Bezier1.x) + (t * this.Bezier2.x);
    this.y = ((1 - t) * this.Bezier1.y) + (t * this.Bezier2.y);
    this.slope = new bezierT(t,startX, startY,control1X,control1Y,control2X,control2Y,endX,endY);

    this.point = {t: t, x: this.x, y: this.y, mX: this.slope.mx, mY: this.slope.my};
}

function bezier1(t,startX, startY,control1X,control1Y,control2X,control2Y)
{
    //linear bezier curve plotter; used recursivly in the quadratic bezier curve calculation
    this.x = (( 1 - t) * (1 - t) * startX) + (2 * (1 - t) * t * control1X) + (t * t * control2X);
    this.y = (( 1 - t) * (1 - t) * startY) + (2 * (1 - t) * t * control1Y) + (t * t * control2Y);

}
