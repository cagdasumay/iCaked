var THREE = require('three');
var scene = require('./scene');
var controls = require('./controls');
var view = require('./view');
var config = require('./config');
var debug = require('./debug');
var design = require('./design');

module.exports = {
    init: function (conf) {
        var container = $("#"+ conf.containerId);
        config.container = container;
        config.mobile = conf.mobile || false;
        config.sizer = $("#"+ conf.sizerId);
        config.vertexShader = conf.vertexShader;
        config.fragmentShader = conf.fragmentShader;
        config.designId = conf.designId;
        document.getElementById(conf.containerId).oncontextmenu = function () { return false; };
        var width = conf.width || config.sizer.width();
        var heigth = conf.height || config.sizer.height();
        debug.log(width +" "+ heigth);
        config.size = { x: width, y: heigth };
        container.width(config.size.x);
        container.height(config.size.y);
        scene.init(config.size);
        design.setSceneAdd(scene.add);
        view.init(container, config.size);
        view.render();
        // setInterval(function () {
        //     view.render();
        // }, 500);
        controls.init($(view.renderer().domElement));
        design.setView(view);
        scene.setControlMethods(controls.addTexture);
    },
    eventLoader: function () {
        controls.eventLoader();
    }
}