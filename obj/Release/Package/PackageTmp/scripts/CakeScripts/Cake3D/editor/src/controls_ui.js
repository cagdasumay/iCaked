var THREE = require('three');
var objects = require('./objects');
var debug = require('./debug');

module.exports = {
    rotationArrow: function (oname) {
        return;
    var uiEl = new UIElementRotationArrow(objects.objects(oname));
    return uiEl;
  },
    collisionIcon: function (oname) {
        return;
    var object = objects.objects(oname);
    var uiExists = false;
    object.traverse(function (child) {
      if (child instanceof THREE.Mesh) {
        if (child.parent.name === "ui-collision_"+oname) uiExists = true;
      }
    });
    if (uiExists) return null;
    var uiEl = new UIElementCollisionIcon(objects.objects(oname));
    return uiEl;
  }
}

var UIElement = function (pos) {
  this.parent = undefined;
  this.element = undefined;
}

UIElement.prototype = {
  update: function (properties) {
  }
}

var UIElementRotationArrow = function (object) {
  this.parent = object;
  var texture, material, plane;
  var textureSize = 120;
  var planeSize = 0.1;
  texture = THREE.ImageUtils.loadTexture("/Images/rotate_transparent.png");
  // assuming you want the texture to repeat in both directions:
  texture.wrapS = THREE.ClampToEdgeWrapping;
  texture.wrapT = THREE.RepeatWrapping;
  repeatX = 1;
  repeatY = 1;
  // how many times to repeat in each direction; the default is (1,1),
  // which is probably why your example wasn't working
  texture.repeat.set(repeatX, repeatY);
  texture.offset.x = (repeatX-1)/2*-1;

  material = new THREE.MeshBasicMaterial({ map: texture, opacity: 0., transparent: true });
  plane = new THREE.Mesh(new THREE.PlaneGeometry(planeSize, planeSize), material);
  plane.material.side = THREE.DoubleSide;

  var pos = {
    x: 0,
    y: 0,
    z: 0
  };
  var box = new THREE.Box3().setFromObject(this.parent);
  pos.x = box.size().x/this.parent.scale.x*0.25;
  pos.y = box.size().y/this.parent.scale.y*0.5;
  pos.z = box.size().z/this.parent.scale.z*0.25;

  var maxDim = Math.max(box.size().x, Math.max(box.size().y, box.size().z));

  this.element = new THREE.Object3D();
  this.element.add(plane);
  this.element.rotation.x = Math.PI/2;
  debug.log(pos);
  this.element.position.x = pos.x;
  this.element.position.y = pos.y;
  this.element.position.z = pos.z;
  this.element.scale.x = 1/this.parent.scale.x; // maxDim*4;
  this.element.scale.y = 1/this.parent.scale.y; // maxDim*4;
  this.element.name = "ui-rotate";
  // this.parent.add(this.element);
}

UIElementRotationArrow.prototype = Object.create(UIElement.prototype);

var UIElementCollisionIcon = function (object) {
  this.parent = object;
  var texture, material, plane;
  var textureSize = 120;
  var planeSize = 0.3;
  texture = THREE.ImageUtils.loadTexture("/Images/collision_icon.png");
  // assuming you want the texture to repeat in both directions:
  texture.wrapS = THREE.ClampToEdgeWrapping;
  texture.wrapT = THREE.RepeatWrapping;
  repeatX = 1;
  repeatY = 1;
  // how many times to repeat in each direction; the default is (1,1),
  // which is probably why your example wasn't working
  texture.repeat.set(repeatX, repeatY);
  texture.offset.x = (repeatX-1)/2*-1;

  material = new THREE.MeshBasicMaterial({ map: texture, opacity: 0.9, transparent: true });
  plane = new THREE.Mesh(new THREE.PlaneGeometry(planeSize, planeSize), material);
  plane.material.side = THREE.DoubleSide;

  var pos = {
    x: 0,
    y: 0,
    z: 0
  };
  var box = new THREE.Box3().setFromObject(this.parent);
  // pos.x = box.size().x/this.parent.scale.x*0.25;
  pos.y = box.size().y/this.parent.scale.y;
  // pos.z = box.size().z/this.parent.scale.z*0.25;

  var maxDim = Math.max(box.size().x, Math.max(box.size().y, box.size().z));

  this.element = new THREE.Object3D();
  this.element.add(plane);
  this.element.rotation.x = Math.PI/2;
  debug.log(pos);
  this.element.position.x = pos.x;
  this.element.position.y = pos.y;
  this.element.position.z = pos.z;
  this.element.scale.x = 1/this.parent.scale.x;
  this.element.scale.y = 1/this.parent.scale.y;
  this.element.name = "ui-collision_"+ object.name;
  // this.parent.add(this.element);
}

UIElementCollisionIcon.prototype = Object.create(UIElement.prototype);
