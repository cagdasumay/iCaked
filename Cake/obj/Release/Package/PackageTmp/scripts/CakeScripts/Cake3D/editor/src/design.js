var THREE = require('three');
var objects = require('./objects');
var debug = require('./debug');
var decalObjects = require('./decal_objects');
var textObject = require('./text');
var config = require('./config');

module.exports = {
    Design: function () {
        return new Design();
    },
    setSceneAdd: function (s) {
        sceneWrapper = s;
    },
    selected: function () {
        return selected;
    },
    select: function (type, name, verbose) {
        //if (name.split("-")[0] && name.split("-")[0] === "plate") return;
        var object = objects.objects(name);
        var decalCheck = false;
        debug.log(object);
        if (object.isDecal !== undefined && object.isDecal === true) {
            decalCheck = true;
        }
        selected = { type: type, name: name };
        debug.log(rootGroup.findElement(name));
        rootGroup.findElement(name).traverseChildrenAndSelf(function (element) {
            if (name === 'plate-0' && element.name !== name) return;
            debug.log("select element.name"+ element.name);
            element.object.traverse(function (child) {
                if (child instanceof THREE.Mesh) {
                    // if (decalCheck) {
                    //   child.material.transparent = false;
                    //   child.material.opacity = 1.0;
                    // } else {
                    debug.log("select obj name:"+ child.parent.name);
                    if (child.parent.name === "plate-0") {
                        for (var k = 0; k < child.material.materials.length; k++) {
                            var m = child.material.materials[k];
                            debug.log(m.pastaColorable);
                            if (m.pastaColorable) {
                                // m.transparent = true;
                                // m.opacity = 0.7;
                            }
                        }
                        return;
                    }
                    var nameArr = child.name.split("-");
                    if (nameArr.length > 0 && nameArr[0] === "randart") return;
                    if (child.name === "invisible-mesh") return;
                    if (child.invisivleMesh === true) return;
                    if (verbose !== true) {
                        if (element.decal) {
                            child.material.color.setHex(0xA9FFAF); //  = 0.5;
							              console.log(element);
                        } else {
						                // child.material.transparent = true;
                            // child.material.opacity = 0.7;
                        }
                        if (element.object.deselectColor instanceof THREE.Color) {
                            child.material.color.setHex(0xA9FFAF);
                        }
                    }
                    debug.log(element.object.deselectColor);
                }
            });
        });
    },
    deselect: function () {
        if (selected === null) return;
        var object = objects.objects(selected.name);

        rootGroup.findElement(selected.name).traverseChildrenAndSelf(function (element) {
            var decalCheck = false;
            if (element.object.isDecal !== undefined && element.object.isDecal === true) {
                decalCheck = false;
            }

            if (!decalCheck) {
                element.object.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        if (child.parent.name === "plate-0") {
                            for (var k = 0; k < child.material.materials.length; k++) {
                                var m = child.material.materials[k];
                                if (m.pastaColorable) {
                                    // m.transparent = false;
                                    if (object.deselectColor instanceof THREE.Color)
                                        m.color.setHex(object.deselectColor.getHex());
                                    else
                                        m.color.setHex(0xffffff);
                                }
                            }
                            return;
                        }

                        var nameArr = child.name.split("-");
                        if (nameArr.length > 0 && nameArr[0] === "randart") return;
                        if (child.name === "invisible-mesh") return;
                        if (child.invisivleMesh == true) return;
                        debug.log("Element deselect color: "+ element.object.deselectColor);
                        /* if (!element.decal)
                           child.material = new THREE.MeshPhongMaterial({ color: child.material.color, specular: 0x0, shininess: 1, shading: THREE.SmoothShading, map: child.material.map, side: THREE.DoubleSide });
                           //  child.material.color.setHex(element.deselectColor);*/
                        if (element.object.deselectColor instanceof THREE.Color && !element.decal) {
							              // child.material.transparent = false;
							              child.material.opacity = 1.0;
                            if (element.object.deselectColor instanceof THREE.Color)
                                child.material.color.setHex(
                                    element.object.deselectColor.getHex()
                                );
                            else
                                child.material.color.setHex(0xffffff);
                        } else if (element.decal) {
							              child.material.color.setHex(0xffffff);
							              console.log(element)
						            }
                    }
                });
            } else if (false) {
                element.object.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        child.material.transparent = false;
                        // child.material.opacity = 1.0;
                    }
                });
            }
            var decalCheck = false;
            debug.log(object);
            if (object.isDecal !== undefined && object.isDecal === true) {
                decalCheck = true;
            }
        });

        selected = null;
    },
    rootGroup: function () {
        return rootGroup;
    },
    designs: function (p) {
        if (typeof p === "number") {
            return _designs[p];
        } else {
            return _designs;
        }
    },
    designIndex: function (k) {
        if (typeof k === "number") { _designIndex = k; }
        return _designIndex;
    },
    designObjects: function () {
        return _designObjects;
    },
    designMeshes: function () {
        return _designMeshes;
    },
    uiElementMeshes: function () {
        return _uiElementMeshes;
    },
    uiElements: function () {
        return _uiElements;
    },
    resetUIElements: function () {
        _uiElements = new Array();
        _uiElementMeshes = new Array();
    },
    updateObject: function (oname) {
        objects.setGraphicalProperties(
            oname,
            _designs[_designIndex].findByName(oname).properties
        );
    },
    getObject: function (oname) {
        return _designs[_designIndex].findByName(oname);
    },
    updateDesignObject: function (object) {
        updateDesignObject(object);
    },
    saveMode: function () {
        var makeInvisible = function (el) {
            if (el.name !== "null-0")
                el.object.visible = false;
            for (var child in el.children) {
                makeInvisible(el.children[child]);
            }
        }
        makeInvisible(nullGroup);
        rootGroup.findElement('plate-0').calculatePeakPoint();
        debug.log("peakY"+ peakY);
        module.exports.deselect();
    },
    peak: function () {
        return peakY;
    },
    setView: function (v) {
        view = v;
    },
    reset: function () {
        reset();
    }
}

var baseInc = 0;
var selected = null;
var _designIndex = -1;
var _designs = new Array();
var _designObjects = new Array();
var _designMeshes = new Array();
var _uiElements = new Array();
var _uiElementMeshes = new Array();
var reachElement = {};
var peakY = 0.15;

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

var Design = function () {
    this.workspace = {
        basePlane: undefined,
        plate: undefined,
        cakes: [],
        objects: []
    };
}

Design.prototype = {
    basePlane: function (p1) {
        if (typeof p1 === "object") {
            this.workspace.basePlane = p1;
            debug.log("Base grid added to desing.");
        } else {
            return this.workspace.basePlane;
        }
    },
    plate: function (p1, p2) {
        if (typeof p1 === "object") {
            this.workspace.plate = p1;
        } else if (typeof p1 === "string") {
            if (typeof p2 === "string") {
                this.workspace.plate[p1] = p2;
            }
        } else {
            return this.workspace.plate;
        }
    },
    cakes: function (p1, p2, p3) {
        if (typeof p1 === "string" && typeof p2 === "string" && typeof p3 === "string") {
            this.workspace.cakes[p1][p2] = p3;
        } else if (typeof p1 === "object") {
            this.workspace.cakes[p1.name] = p1;
        } else if (typeof p1 === "string") {
            return this.workspace.cakes[p1];
        } else {
            return this.workspace.cakes;
        }
    },
    objects: function (p1, p2, p3) {
        if (typeof p1 === "string" && typeof p2 === "string" && typeof p3 === "string") {
            this.workspace.objects[p1][p2] = p3;
        } else if (typeof p1 === "object") {
            this.workspace.objects[p1.name] = p1;
        } else if (typeof p1 === "string") {
            return this.workspace.objects[p1];
        } else {
            return this.workspace.objects;
        }
    },
    changeParentOfObject: function (oname, pname) {
        var onameArr = oname.split('-');
    },
    removeCake: function (k) {
        delete this.workspace.cakes[name];
    },
    removeObject: function (name) {
        delete this.workspace.objects[name];
    },
    deletePlate: function () {
        if (this.workspace.plate !== undefined) {
            this.workspace.plate = undefined;
        }
    },
    findByName: function (name) {
        nameArr = name.split('-');
        if (nameArr[0] === "cake") {
            return this.workspace.cakes[name];
        } else if (nameArr[0] === "object") {
            return this.workspace.objects[name];
        } else if (nameArr[0] === "plate") {
            return this.workspace.plate;
        }
    }
}

var Element = function (obj) {
    this.object = obj;
    obj.pastaElement = this;
    // obj.parentElement = this;
    this.name = obj.name;
    this.parentName = "null-0";
    this.parent = null;
    this.children = { };
    this.isElement = true;
    this.dParent = new THREE.Vector3();
    this.preScaler = 1.0;
    this.preRotation = 0.0;
    this.properties = { };
    this.decal = false;
    this.source = "";
    this.scaled = 1.0;
    this.rotated = 0.0;
    this.isText3D = false;
    this.textureURL = "";
    this.textureAlphaURL = "";
    this.unlimitedScale = 1.0;
    this.lockRotation = false;
    this.isCake = false;
    this.sideDecorDelta = null;
    this.done = "";
}

Element.prototype = {
    addElement: function (name, el) {
        if (el.isElement) {
            this.children[name] = el;
        }
    },
    addObject: function (obj) {
        var el = new Element(obj);
        var tmp = new THREE.Vector3();
        this.children[obj.name] = el;
        el.parent = this;
        el.parentName = this.name;
        if (!el.decal) {
            el.dParent.copy(tmp.copy(el.object.position).sub(this.object.position));
        } else {
            var decalObjectProps = decalObjects.getDecalObjectsCurrent(el.name);
            var dParentTmp = new THREE.Vector3(
                decalObjectProps.intersectionPoint.x,
                decalObjectProps.intersectionPoint.y,
                decalObjectProps.intersectionPoint.z
            );
            el.dParent.copy(tmp.copy(dParentTmp).sub(this.object.position));
        }
        this.unlimitedScale = obj.scale.x;
    },
    removeElement: function (name) {
        // this.children[name].parent = null;
        delete this.children[name];
    },
    findElement: function (name) {
        var found = undefined;
        for (var key in this.children) {
            if (key === name) {
                found = this.children[name];
                break;
            } else {
                if (typeof this.children[key].findElement === "function") {
                    found = this.children[key].findElement(name);
                    if (found !== undefined) return found;
                }
            }
        }
        return found;
    },
    swapElement: function (name, pname) {
        var el = rootGroup.findElement(name);
        if (el === undefined) return -1;
        var pel = rootGroup.findElement(pname);
        if (pel === undefined) return -2;
        if (pel.parent !== null && pel.parent.name === name) return -3;
        if (el.parent === pel) return 1;
        pel.addElement(name, el);
        el.parent.removeElement(name);
        el.parent = pel;
        el.parentName = pel.name;
        var tmp = new THREE.Vector3();
        if (!el.decal) {
	          el.dParent.copy(tmp.copy(el.object.position).sub(pel.object.position));
        } else {
            var decalObjectProps = decalObjects.getDecalObjectsCurrent(el.name);
            var dParentTmp = new THREE.Vector3(
                decalObjectProps.intersectionPoint.x,
                decalObjectProps.intersectionPoint.y,
                decalObjectProps.intersectionPoint.z
            );
            el.dParent.copy(tmp.copy(dParentTmp).sub(pel.object.position));
        }
        debug.log("swapElement: dParent");
        debug.log(el.name);
        debug.log(el.object.position);
        debug.log(pel.object.position);

        return 0;
    },
    isElder: function (name) {
        if (this.findElement(name) !== undefined) return true;
        else return false;
    },
    isParent: function (name) {
        if (this.children[name] !== undefined) return true;
        else return false;
    },
    hasElder: function (name) {
        if (this.parent === null) return false;
        else {
            if (this.parent.name === name) return true;
            else return this.parent.hasElder(name);
        }
    },
    hasParent: function (name) {
        if (this.parent === null) return false;
        if (this.parent.name === name) return true;
        else return false;
    },
    child: function (name) {
        if (name !== undefined) {
            return this.children[name];
        } else {
            return this.children;
        }
    },
    traverseChildren: function (func) {
        for (child in this.children) {
            var _child = this.children[child];
            func(this.children[child]);
            _child.traverseChildren(func);
        }
    },
    traverseChildrenAndSelf: function (func) {
        var _this = this;
        func(this);
        _this.traverseChildren(func);
    },
    traverseCakes: function (func) {
        for (child in this.children) {
            debug.log("traverseCakes: "+ child);
            debug.log("traverseCakes: isCake: " +this.children[child].isCake);
            if (this.children[child].isCake !== true) continue;
            var _child = this.children[child];
            func(this.children[child]); //.traverseCakes(func);
            // debug.log(this.children[child]);
            _child.traverseCakes(func);
        }
    },
    updateScaleAndRotation: function (properties) {
        var tmp = new THREE.Vector3();
        var scale, rotationY;
        if (properties === undefined || true) {
            scale = this.properties.scale;
            rotationY = this.properties.rotationY;
        } else {
            scale = properties.scale;
            rotationY = properties.rotationY;
        }
        var elderPos = new THREE.Vector3().copy(this.object.position);
	      scale = this.object.scale.x;
        this.dParent.copy(
            tmp.copy(this.object.position).sub(this.parent.object.position).multiplyScalar(scale/this.preScaler)
        );
        this.scaled = scale/this.preScaler;
        this.preScaler = scale;
        this.rotated = rotationY-this.preRotation;
        this.preRotation = rotationY;
        this.properties.scale = scale;
        var _this = this;
        if (this.alignNormal) {
            _this.object.traverse(function (c) {
                if (c instanceof THREE.Mesh) {
                    _this.properties.rotationY = c.rotation.z;
                }
            });
        } else {
            this.properties.rotationY = this.object.rotation.y;
        }

        this.traverseChildren(function (child) {
            if (_this.name === "plate-0") {
                if (child.isCake) return;
                if (child.parent.name !== _this.name) return;
            }
            debug.log("traverseChildren name:"+ child.name);
            var scaleInner; //  = child.parent.scaled*child.object.scale.x;
            var rotationYInner; //  = child.parent.rotated+child.object.rotation.y;
            if (!child.decal) {
                if (child.isText3D) {
                    rotationYInner = child.parent.rotated+child.pastaRotateValue;
                } else {
                    rotationYInner = child.parent.rotated+child.object.rotation.y;
                }
                scaleInner = child.parent.scaled*child.object.scale.x; // important
            } else {
                var decalObjectProps = decalObjects.getDecalObjectsCurrent(child.name);
                scaleInner = child.parent.scaled*decalObjectProps.scaleFactor;
                rotationYInner = child.parent.rotated+decalObjectProps.rotationFactor;
            }
            var posParent = new THREE.Vector3().copy(child.parent.object.position);
            var decal = objects.setGraphicalProperties(child.name, { color: child.properties.color,
                                                                     scale: scaleInner, decal: child.decal
                                                                   });
            // updateDesignObject(decal);
            var rotatedY = undefined;
            var meshIndex = child.object.children.length;
            var k = 0;
            var multiMeshSet = false;
            if (!child.decal) {
                child.object.traverse(function (grandChild) {
                    if (grandChild instanceof THREE.Mesh) {
                        if (rotatedY !== undefined) return;
                        var dGrandParent = new THREE.Vector3().copy(child.object.position)
                            .sub(posParent);
                        if (child.isRandom || child.alignNormal || child.isText3D) {
                            var lookAtPoint = new THREE.Vector3(
                                child.lookAtData.point.x,
                                child.lookAtData.point.y,
                                child.lookAtData.point.z
                            );
                            var lookAtNormal = new THREE.Vector3(
                                child.lookAtData.normal.x,
                                child.lookAtData.normal.y,
                                child.lookAtData.normal.z
                            );

                            var mY = new THREE.Matrix4().makeRotationY(child.parent.object.rotation.y);

                            if (Math.abs(lookAtNormal.y-1) > 1e-2 || child.isRandom) {
                                lookAtNormal.applyMatrix4(mY);

                                child.object.lookAt(lookAtPoint.add(lookAtNormal));
                            }

                            // TODO assasi cok feci

                            if (child.isText3D) {
                                grandChild.geometry.translate(
                                    dGrandParent.x, dGrandParent.y, dGrandParent.z
                                );
                                var rValue = 0;
                                var rotAxis = new THREE.Vector3(
                                    child.pastaRotationAxis.x,
                                    child.pastaRotationAxis.y,
                                    child.pastaRotationAxis.z
                                );
                                debug.log("update: text3d");
                                debug.log(lookAtNormal);
                                debug.log(rotationYInner);
                                if (Math.abs(lookAtNormal.y-1) < 1e-2)
                                    rValue =  rotationYInner-child.pastaRotateValue;

                                debug.log(rValue);
                                child.object.rotateOnAxis(rotAxis, rValue);

                                if (Math.abs(lookAtNormal.y-1) < 1e-2)
                                    child.pastaRotateValue = rotationYInner;

                                grandChild.geometry.translate(
                                    -dGrandParent.x, -dGrandParent.y, -dGrandParent.z
                                );

                                rotatedY = rotationYInner;
                            } else if (child.alignNormal) {
                                // grandChild.geometry.translate(
                                //     dGrandParent.x, dGrandParent.y, dGrandParent.z
                                // );

                                // child.object.rotateOnAxis(rotAxis, rValue);
                                // child.object.traverse(function (c) {
                                //     if (c instanceof THREE.Mesh) {
                                //         if (Math.abs(lookAtNormal.y-1) < 1e-3 && false)
                                //             c.rotation.z = rotationYInner;
                                //     }
                                // });

                                // if (Math.abs(lookAtNormal.y-1) < 1e-3 )
                                //     child.pastaRotateValue = rotationYInner;

                                // // grandChild.geometry.translate(
                                // //         -dGrandParent.x, -dGrandParent.y, -dGrandParent.z
                                // // );

                                rotatedY = rotationYInner;
                            }
                        } else {
                            grandChild.geometry.translate(
                                dGrandParent.x, dGrandParent.y, dGrandParent.z
                            );
                            child.object.rotation.y = rotationYInner;
                            grandChild.geometry.translate(
                                -dGrandParent.x, -dGrandParent.y, -dGrandParent.z
                            );
                            rotatedY = rotationYInner;
                        }
                    }
                });
            } else if (child.decal) {
                var decal = objects.setGraphicalProperties(child.name, {
                    rotationY: rotationYInner, decal: child.decal
                });
                updateDesignObject(decal);
            }

			      /* if (typeof child.properties.minSize === "string") {
               if (child.name.indexOf("cake") < 0) {
               scaleInner = Math.max(scaleInner, parseFloat(child.properties.minSize));
               } else {
               scaleInner = Math.max(scaleInner, getCakeDimension(parseInt(child.properties.minSize)));
               }
               }
			         if (typeof child.properties.maxSize === "string") {
				       if (child.name.indexOf("cake") < 0) {
					     scaleInner = Math.min(scaleInner, parseFloat(child.properties.maxSize));
				       } else {
					     var size = getCakeDimension(parseInt(child.properties.maxSize));
					     if (typeof size !== "number")
						   size = scaleInner;
					     scaleInner = Math.min(scaleInner, size);
				       }
			         } */
            child.properties.scale = child.object.scale.x; // important
            if (!child.alignNormal)
                child.properties.rotationY = rotatedY;
            // child.object.updateMatrix();
            // child.object.updateMatrixWorld()
            var tmpInner = new THREE.Vector3();
            var dParentY = child.dParent.y;
            if (!child.decal) {
                child.dParent.copy(
                    tmpInner.copy(child.object.position).sub(child.parent.object.position).multiplyScalar(child.parent.scaled)
                ).applyMatrix4(
                    new THREE.Matrix4().makeRotationY(child.parent.rotated)
                );
            } else if (child.decal) {
                child.dParent.copy(
                    tmpInner.copy(decalObjects.getIntersectionPoint(child.name)).sub(child.parent.object.position).multiplyScalar(child.properties.scale/child.preScaler)
                ).applyMatrix4(
                    new THREE.Matrix4().makeRotationY(child.parent.rotated)
                );
            }

            // TODO: important
            if (!child.parent.allowChildren) {
                child.dParent.y = dParentY;
            }

            child.scaled = child.object.scale.x/child.preScaler;
            child.preScaler = child.object.scale.x;
            child.rotated = rotationYInner-child.preRotation;
            child.preRotation = rotationYInner;
        });
        this.update(true);
    },
    update: function (s) {
        var scale = s || false;
        var tmp = new THREE.Vector3();
        if (!this.decal) {
            this.dParent.copy(
                tmp.copy(this.object.position).sub(this.parent.object.position)
            );
        } else {
            var decalObjectProps = decalObjects.getDecalObjectsCurrent(this.name);
            var dParentTmp = new THREE.Vector3(
                decalObjectProps.intersectionPoint.x,
                decalObjectProps.intersectionPoint.y,
                decalObjectProps.intersectionPoint.z
            );
            this.dParent.copy(
                tmp.copy(dParentTmp).sub(this.parent.object.position)
            );
        }
        _this = this;
        debug.log("update: this");
        debug.log(_this)
        this.traverseChildren(function (child) {
            debug.log(child);
            var posParent = new THREE.Vector3().copy(child.parent.object.position);
            var newPos = posParent.clone().add(child.dParent);
            if (!child.decal && scale && !child.parent.allowChildren) newPos.y = child.object.position.y;

            if (child.isText3D) {
                var text3DCurrent = textObject.getText3DCurrent(child.name);
                var intersectsPoint = new THREE.Vector3(
                    text3DCurrent.intersectsPoint.x,
                    text3DCurrent.intersectsPoint.y,
                    text3DCurrent.intersectsPoint.z
                );
                var delta = intersectsPoint.clone().sub(child.object.position).
                    add(child.dParent).
                    add(posParent);
                text3DCurrent.intersectsPoint.x = delta.x;
                text3DCurrent.intersectsPoint.y = delta.y;
                text3DCurrent.intersectsPoint.z = delta.z;
                child.lookAtData.point.x = newPos.x;
                child.lookAtData.point.y = newPos.y;
                child.lookAtData.point.z = newPos.z;
                child.object.position.copy(newPos);
            } else if (!child.decal) {
                if (child.isRandom || child.alignNormal) {
                    // var lookAtPoint = new THREE.Vector3(
                    //     child.lookAtData.point.x,
                    //     child.lookAtData.point.y,
                    //     child.lookAtData.point.z
                    // );
                    // var delta = lookAtPoint.clone().sub(child.object.position).
                    //     add(child.dParent).
                    //     add(posParent);
                    // child.lookAtData.point.x = delta.x;
                    // child.lookAtData.point.y = delta.y;
                    // child.lookAtData.point.z = delta.z;
                    child.lookAtData.point.x = newPos.x;
                    child.lookAtData.point.y = newPos.y;
                    child.lookAtData.point.z = newPos.z;
                }
                debug.log("update: traverseChildren.");
                debug.log(child)
                child.object.position.copy(newPos);
                debug.log(child.object.position);
            } else if (child.decal) {
                var decalObjectProps = decalObjects.getDecalObjectsCurrent(child.name);

                decalObjectProps.intersectionPoint.x = newPos.x;
                decalObjectProps.intersectionPoint.y = newPos.y;
                decalObjectProps.intersectionPoint.z = newPos.z;
                var decal = objects.moveDecalToWrapper(child.object, decalObjects.getDecalObjectsCurrent(child.name), _this.name);
                updateDesignObject(decal);
            }
        });
    },
    toString: function () {
        this.properties["transform"] = { };
        this.properties.transform["position"] = {
            x: this.object.position.x,
            y: this.object.position.y,
            z: this.object.position.z
        };
        this.properties.transform["rotation"] = {
            x: this.object.rotation.x,
            y: this.object.rotation.y,
            z: this.object.rotation.z
        };
        this.properties.scale = this.object.scale.x;
        if (this.alignNormal) {
            var _this = this;
            _this.object.traverse(function (c) {
                if (c instanceof THREE.Mesh) {
                    _this.properties.rotationY = c.rotation.z;
                }
            });
        } else {
            this.properties.rotationY = this.object.rotation.y;
        }
        // if (this.pastaRotationAxis !== undefined) {
        //     this.properties.rotateValue = this.pastaRotateValue;
        //     this.properties["rotationAxis"] = {
        //         x: this.pastaRotationAxis.x,
        //         y: this.pastaRotationAxis.y,
        //         z: this.pastaRotationAxis.z
        //     };
        // }
        if (this.isText3D) {
            this.text3DCurrent = textObject.getText3DCurrent(this.name);
        } else if (this.decal) {
            this.decalObjectCurrent = decalObjects.getDecalObjectsCurrent(this.name);
            this.properties.rotationY = this.decalObjectCurrent.rotationFactor;
            this.properties.scale = this.decalObjectCurrent.scaleFactor;
        } else if (this.isSideDecoration) {
        } else if (this.name === "plate-0") {
            if (this.object.deselectColor instanceof THREE.Color)
                this.properties.color = this.object.deselectColor.getHex();
            else
                this.properties.color = 0xffffff;
            this.properties.scaleZ = this.object.scale.z;
            this.properties.scale = this.object.scale.x;
        }
        var jObjectString = "";
        var k = 0;
        for (var key in this) {
            if (typeof this[key] === "function") continue;
            if (key === "object") continue;
            if (key === "parent") continue;
            if (k > 0) jObjectString += ",";
            if (key === "children") {
                jObjectString += '"'+ key +'": {';
                var l = 0;
                for (var child in this[key]) {
                    if (l > 0) jObjectString += ",";
                    jObjectString += '"'+ child +'": {'+ this[key][child].toString() +" }";
                    l++;
                }
                jObjectString += "}";
            } else {
                jObjectString += '"'+ key +'": '+ JSON.stringify(this[key], function(key, value) {
                    if (typeof value === "number") {
                        return parseFloat(value.toFixed(4));
                    } else if (typeof value === "undefined") {
                        value = "";
                    }
                    return value;
                });
                k++;
            }
        }

        return jObjectString;
    },
    toStringWrapper: function () {
        return "{"+ this.toString() +"}";
    },
    fromString: function (jStr) {
		    // config.loading = true;
        debug.log(jStr);
        jObj = JSON.parse(jStr);
        this.loadJObj(jObj);
    },
    loadJObj: function (jObj) {
        var propertiesSet = false;
        var _this = this;

        // Render function
        view.render(true);
        for (var name in jObj.children) {
	          config.loading = false;
            propertiesSet = false;
            debug.log("loadJObj for: "+ name);
            var child = jObj.children[name];
            if (child.name === "plate-0") {
                objects.changePlate(child.properties.shape);
                objects.setPlateGraphicalProperties(child.properties);
                objects.objects(child.name).scale.z = child.properties.scaleZ;
                _this.child(child.name).loadJObj(child);
            } else if (!child.isText3D && !child.decal) {
                debug.log("Load: parent name: "+ jObj.name);
                debug.log("Load: child name: "+ child.name);
                // if (child.name === "plate-0") {
                //     _this.child(child.name).loadJObj(child);
                //     continue;
                // }
                var source;
                if (child.isSideDecoration === true) {
                    var sideDecorName = child.name;
                    source = globalLoadSource(sideDecorName, config.designId); // global function

                    /* Source empty */
                    if (!(typeof source !== "undefined" &&
                          source !== null && typeof source !== "string" &&
                          source.length > 0)) {
                        debug.log("Source for "+ child.name +" could not be found!");
                        continue;
                    }

                    objects.addSideDecoration(
                        jObj.name, child.name, source, child.sideDecorDelta, function () { }
                    );
                    child.properties.scale = 1.0;
                } else {
                    source = globalLoadSource(child.name, config.designId); // global function

                    /* Source empty */
                    if (!(typeof source === "string" && source !== "")) {
                        debug.log("Source for "+ child.name +" could not be found!");
                        continue;
                    }

                    debug.log("continue n worked");

                    objects.objectParser(source, { name: child.name },
                                         function () { }, child.properties.transform,
                                         child.properties.normalScale);
                }
                var childDummy = $.extend(true, {}, child);
                if (typeof child.properties.texture === "string" && child.textureAlphaURL === "") {
                    var textureUrl = globalLoadTextureURL(childDummy.properties.texture, config.designId);
                    var _childDummy = $.extend(true, {}, childDummy);
                    (function (childDummy) {
                        objects.loadTexture(textureUrl, childDummy.properties.texture, function () {
                            // debug.log("loadJObj: texture"+ textureUrl);
                            // debug.log(childDummy);
                            objects.setGraphicalProperties(childDummy.name, { texture: childDummy.properties.texture });
                            // dada = true;
                        });
                    })(_childDummy);
                }
                if (child.isRandom) {
                    var childObj = objects.objects(child.name);
                    if (childObj && childObj.children) {
                        childObj.children[0].geometry.rotateX(Math.PI/2);
                    }
                }
                if (child.alignNormal) {
                    objects.objects(child.name).traverse(function (child) {
                        if (child instanceof THREE.Mesh) {
                            child.geometry.rotateX(Math.PI/2);
                        }
                    });
                }
                // if (!propertiesSet) {
                //    propertiesSet = true;
                // } else {
                //    setTimeout(function () { }, 3000);
                // }
                if (child.textureAlphaURL !== "") {
                    (function (name, url, coords, colors) {
                        var _colors = hex2rgba(colors);
                        objects.colorizeObject(name, url, coords, _colors, function () {
                            objects.setTexture(name, objects.getColorizedTexture(name));
                        });
                    })(child.name, child.textureAlphaURL,
                       $.extend(true, {}, child.coordSource),
                       $.extend(true, {}, child.colorize));
                }
                var type = child.name.split("-")[0];
                debug.log(sceneAdd(type, {
                    name: child.name, properties: child.properties,
                    isSideDecoration: child.isSideDecoration, isRandom: child.isRandom
                }));

                if (type !== "plate")
                    debug.log("swappo "+ child.name  + rootGroup.swapElement(child.name, jObj.name));

                var parentElement = rootGroup.findElement(jObj.name);
                for (var key in child) {
                    if (key === "dParent") continue;
                    if (key === "children") continue;
                    parentElement.child(child.name)[key] = child[key];
                }
				        if (child.name.indexOf("cake") >= 0) {
                    if (_this.name === 'plate-0') {
							          config.loading = true;
                    }
				        }
                var textureTmp = child.properties.texture;
                child.properties.texture = undefined;
                objects.setGraphicalProperties(child.name, child.properties);
                child.properties.texture = textureTmp;
                _this.child(child.name).loadJObj(child);
				        config.loading = false;
            } else if (child.isText3D) {
                debug.log("Load: parent name: "+ jObj.name);
                debug.log("Load: child name: "+ child.name);
                debug.log("Child is text 3d.");
                var _childDummy = $.extend(true, {}, child);
                (function (childDummy) {
                    var rotAxis;
                    if (typeof childDummy.pastaRotationAxis === "object") {
                        rotAxis = {
                            x: childDummy.pastaRotationAxis.x,
                            y: childDummy.pastaRotationAxis.y,
                            z: childDummy.pastaRotationAxis.z
                        };
                    } else {
                        rotAxis = { x: 0, y: 1, z: 0 };
                    }
                    objects.add3DText(
                        childDummy.name,
                        childDummy.text3DCurrent.text,
                        childDummy.text3DCurrent.font,
                        childDummy.text3DCurrent.size,
                        childDummy.text3DCurrent.style,
                        childDummy.properties.transform,
                        function () {
                            var type = childDummy.name.split("-")[0];
                            var object = objects.objects(childDummy.name);
                            debug.log(sceneAdd(type, childDummy )); //{ name: childDummy.name, properties: childDummy.properties, isText3D: true }));
						                var parentElement = rootGroup.findElement(jObj.name);
                            if (type !== "plate")
                                debug.log(rootGroup.swapElement(childDummy.name, jObj.name));
                            debug.log(parentElement.children);
                            for (var key in childDummy) {
                                if (key === "dParent") continue;
                                parentElement.child(childDummy.name)[key] = childDummy[key];
                            }
						                parentElement.child(childDummy.name).lockRotation = false;
						                objects.setGraphicalProperties(childDummy.name, { scale: childDummy.properties.scale });
                            if (childDummy.text3DCurrent.intersectsPoint !== undefined) {
                                textObject.moveText3DWrapper(
                                    objects.objects(childDummy.name),
                                    childDummy.text3DCurrent.intersectsPoint,
                                    childDummy.text3DCurrent.intersectsNormal,
                                    objects.objects(jObj.name)
                                );
                            }
                            childDummy.pastaRotationAxis.x = rotAxis.x;
                            childDummy.pastaRotationAxis.y = rotAxis.y;
                            childDummy.pastaRotationAxis.z = rotAxis.z;
                            childDummy.properties.rotationY = childDummy.pastaRotateValue;
                            childDummy.pastaRotateValue = 0;

                            // childDummy.pastaRotateValue = 0;
                            objects.setGraphicalProperties(childDummy.name, childDummy.properties);
                            _this.child(childDummy.name).loadJObj(childDummy);
                        }
                    );
                })(_childDummy);
            } else if (child.decal) {
                debug.log("Load: parent name: "+ jObj.name);
                debug.log("Load: child name: "+ child.name);
                debug.log("Child is decal.");
                var _childDummy = $.extend(true, {}, child);
                (function (childDummy) {
                    objects.addObjectDecalLoadWrapper(
                        childDummy.name,
                        childDummy.decalObjectCurrent,
                        objects.objects(childDummy.decalObjectCurrent.onTo), function (a) {
                            debug.log("folfol "+ a);
                            var type = childDummy.name.split("-")[0];
                            debug.log(sceneAdd(type, { name: childDummy.name, properties: childDummy.properties, decal: true, text: childDummy.text }));
                            var parentElement = rootGroup.findElement(jObj.name);
                            if (type !== "plate")
                                debug.log(rootGroup.swapElement(childDummy.name, jObj.name));
                            for (var key in childDummy) {
                                if (key === "dParent") continue;
                                parentElement.child(childDummy.name)[key] = childDummy[key];
                            }
                            decalObjects.getDecalObjectsCurrent(childDummy.name).scaleFactor = 1.0;
                            objects.setGraphicalProperties(childDummy.name, childDummy.properties);
                            _this.child(childDummy.name).loadJObj(childDummy);
                            var nameInfo = childDummy.name.split("-");
                            if (nameInfo.length > 1 && parseInt(nameInfo[1]) >= decal_idx) {
                                decal_idx = parseInt(nameInfo[1])+1;
                            }
                        });
                })(_childDummy);
            }
        }
    },
    calculatePeakPoint: function () {
        peakY = 0;
        this.traverseChildren(function (child) {
            var bb = new THREE.Box3().setFromObject(child.object);
            debug.log(child.name);
            debug.log(bb);
            if (bb.max.y > peakY) {
                peakY = bb.max.y;
            }
        });
    }
}

function sceneAdd (type, obj) {
    sceneWrapper(type, obj);
    return;
    var object = objects.objects(obj.name) || objects.objectsDecal(obj.name);
    if (object === undefined) return -1;
    debug.log("Scene: "+ obj.name +" "+ type);
    _designObjects.push(object);
    // THREE.JS function
    sceneWrapper.add(object);
    // =================
    if (obj.properties.color === undefined) obj.properties["color"] = 0xFFFFFF;
    if (obj.properties.scale === undefined) obj.properties["scale"] = object.scale.x;
    if (type === "cake") {
        _designs[_designIndex].cakes({
            name: obj.name, // no need to hold source data
            properties: obj.properties, // only name and properties
            decal: obj.decal
        });
    } else if (type === "plate") {
        _designs[_designIndex].plate({
            name: obj.name
        });
    } else if (type === "object") {
        _designs[_designIndex].objects({
            name: obj.name,
            properties: obj.properties
        });
    }
    if (type === "plate") {
        debug.log(rootGroup.findElement("main-0").children);
        // rootGroup.findElement("main-0").addObject(object);
        debug.log(rootGroup.findElement("main-0").children);
    } else {
        var nullEl = rootGroup.findElement("null-0");
        if (nullEl.child(obj.name) === undefined) {
            nullEl.addObject(object);
        }

        var designElement = rootGroup.findElement(obj.name)
        designElement.properties = $.extend(true, {}, obj.properties);
        designElement.isText3D = obj.isText3D || false;
        designElement.alignNormal = obj.alignNormal || false;
        if (designElement.alignNormal || designElement.isText3D) {
            designElement.pastaRotationAxis = obj.pastaRotationAxis;
            designElement.pastaRotateValue = obj.pastaRotateValue;
            designElement.lookAtData = obj.lookAtData;
            if (designElement.alignNormal) {
                designElement.object.traverse(function (child) {
                    if (child instanceof THREE.Mesh) {
                        child.geometry.rotateX(Math.PI/2);
                    }
                });
            }
        }
        designElement.isSideDecoration = obj.isSideDecoration || false;
        designElement.isRandom = obj.isRandom || false;
        if (typeof obj.coordSource === "object") {
            designElement.coordSource = obj.coordSource;
        } else {
            designElement.coordSource = objects.parseCoordSource(obj.coordSource || "");
        }
        // designElement.lookAtData = obj.lookAtData || { point: {}, normal: {}, dragged: false };
        designElement.colorize = {};
        designElement.allowChildren = obj.allowChildren || false;

        for (var key in designElement.coordSource) {
            designElement.colorize[key] = 0xffffff;
        }

        if (obj.decal !== undefined) designElement.decal = obj.decal;
    }

    object.traverse(function (child) {
        if (child instanceof THREE.Mesh) {
            debug.log("object.travers mesh.")
            _designMeshes.push(child);
        }
    });
    return 0;
}

function hex2rgba (colors) {
    var rgbaList = {};
    var c = 0;
    for (var item in colors) {
        c = colors[item];
        rgbaList[item] = 'rgba('+[(c>>16)&255, (c>>8)&255, c&255].join(',')+',1)';
    }

    return rgbaList;
}

var sceneWrapper;
var rootGroup = new Element(new THREE.Object3D());
rootGroup.name = "root-0";
var nullGroup = new Element(new THREE.Object3D());
nullGroup.name = "null-0";
var mainGroup = new Element(new THREE.Object3D());
mainGroup.name = "main-0";
rootGroup.addElement("null-0", nullGroup);
rootGroup.addElement("main-0", mainGroup);
var view = null;

function reset() {
    baseInc = 0;
    selected = null;
    _designIndex = -1;
    _designs = new Array();
    _designObjects = new Array();
    _designMeshes = new Array();
    _uiElements = new Array();
    _uiElementMeshes = new Array();
    reachElement = {};
    nullGroup = new Element(new THREE.Object3D());
    nullGroup.name = "null-0";
    mainGroup = new Element(new THREE.Object3D());
    mainGroup.name = "main-0";
    rootGroup = new Element(new THREE.Object3D());
    rootGroup.name = "root-0";
    rootGroup.addElement("null-0", nullGroup);
    rootGroup.addElement("main-0", mainGroup);
}