var THREE = require('three');
var scene = require('./scene');
var debug = require('./debug');
var config = require('./config');
var objects = require('./objects');
var design = require('./design');

module.exports = {
    render: function (call) {
        // if (config.animate) {
        if (call !== true) requestAnimationFrame(module.exports.render);
        //     config.animate = false;
        // }

        if (Math.abs(_rotationSpeed) > 0) {
            // _phii += _rotationSpeed;
            // _phii += 0.001;
            if (_phi > 6*Math.PI) {
                _phii = 0;
            }
            // scene.rotateLight(_phii);
        }
        zoom();
        // if ( count % 2 === 0 ) {
				// 	  config.envMap = scene.cubeCamera1().renderTarget.texture;
				// 	  scene.cubeCamera2().updateCubeMap( renderer, scene.get() );

			  // } else {
				// 	  config.envMap = scene.cubeCamera2().renderTarget.texture;
				// 	  scene.cubeCamera1().updateCubeMap( renderer, scene.get() );

			  // }
        // if (count === 101) count = 0;
        // count++;
        renderer.render(scene.get(), scene.camera());
    },
    init: function (into, size) {
        renderer.setSize(size.x, size.y);
        renderer.setClearColor(0xffffff);
        renderer.shadowMap.enabled = true;
			  renderer.shadowMap.type = THREE.PCFShadowMap;
        renderer.toneMapping = THREE.LinearToneMapping;
        renderer.gammaInput = true;
        renderer.gammaOutput = true;
        into.append(renderer.domElement);
    },
    radius: function (r) {
        if (r !== undefined) {
            _radius = r;
        } else {
            return _radius;
        }
        _radius = Math.max(2, Math.min(17.5, _radius));
        zoom();
    },
    rotationSpeed: function (s) {
        if (s !== undefined) {
            _rotationSpeed = s;
        } else {
            return _rotationSpeed;
        }
    },
    renderer: function () {
        return renderer;
    },
    psi: function (p) {
        if (p !== undefined) {
            _psi = Math.max(0.01, Math.min(Math.PI/2, p));
        } else {
            return _psi;
        }
    },
    phi: function (p) {
        if (p !== undefined) {
            if (p >= Math.PI) p = -Math.PI+0.001;
            if (p <= -Math.PI) p = Math.PI-0.001;
            _phi = p;
        } else {
            return _phi;
        }
    },
    cameraPosition: function (pos) {
        staringAt.y = 0.55;
        var peakY = design.peak();
        var halfHeight = peakY-staringAt.y;
        var maxRad = halfHeight/0.17;
        if (objects.objects('plate-0').scale.x < 0.8)
            _radius = 12.0*objects.objects('plate-0').scale.x;
        else
            _radius = 9.0*objects.objects('plate-0').scale.x;
        debug.log(peakY);
        debug.log(maxRad);
        debug.log(_radius);
        if (maxRad > _radius && pos < 4) {
            _radius = maxRad;
        }
        _psi = 0.48;
        var _radius2 = 15.0;
        if (pos === 0) {
            _phi = 0;
        } else if (pos === 1) {
            _phi = Math.PI / 6;
        } else if (pos === 2) {
            _phi = -Math.PI / 6;
        } else if (pos === 3) {
            _phi = Math.PI / 6 * 7;
        } else if (pos === 4) {
			      _radius = _radius2*objects.objects('plate-0').scale.x;
            _phi = 0;
			      _psi = Math.PI/2;
        } else if (pos === 5) {
            _phi = Math.PI / 6;
			      _radius = _radius2*objects.objects('plate-0').scale.x;
			      _psi = Math.PI/2*0.8;
        } else if (pos === 6) {
            _phi = -Math.PI / 6;
			      _radius = _radius2*objects.objects('plate-0').scale.x;
			      _psi = Math.PI/2*0.8;
        } else if (pos === 7) {
            _phi = Math.PI;
			      _radius = _radius2*objects.objects('plate-0').scale.x;
			      _psi = Math.PI/2*0.8;0
        }
    }
}

var count = 0;
var _phii = 0;
var _phi = 0, _psi = /*0.41152*/0.6, _radius = 10.0, _rotationSpeed = 0.0000;
var xOffset = 0;
var staringAt = new THREE.Vector3(-xOffset, 0.7, 0);
var renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true, preserveDrawingBuffer: true });

function zoom () {
    scene.camera().position.z = _radius*Math.cos(_phi)*Math.cos(_psi);
    scene.camera().position.x = _radius*Math.sin(_phi)*Math.cos(_psi)-xOffset;
    scene.camera().position.y = _radius*Math.sin(_psi);
    scene.camera().lookAt(staringAt);
    scene.camera().updateMatrix();
}