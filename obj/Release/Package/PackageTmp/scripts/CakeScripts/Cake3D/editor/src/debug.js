module.exports = {
    level: function (l) {
        if (l !== undefined) {
            level = l;
        } else {
            return level;
        }
    },
    log: function (msg) {
        if (level > 0) console.log(msg);
    },
    lor: function (msg) {
        if (level < 0) console.log(msg);
    }
};

var level = 1;
