var debug = require('./debug');


module.exports = {
    randomInt: function (s, e) {
        // randomInt (1, 3) returns 1 or 2
        return Math.floor(Math.random()*(e-s))+s;
    },
    randomReal: function (s, e) {
        debug.log("Rand interval: "+ e +" "+ s);
        return Math.random()*(e-s)+s;
    },
    shuffle: function (array) {
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
    },
    seqArray: function (c) {
        var array = new Array(c);
        for (var l = 0; l < c; l++) {
            array[l] = l;
        }

        return array;
    },
    cartesian: function () {
        var r = [], arg = arguments, max = arg.length-1;
        function helper(arr, i) {
            for (var j=0, l=arg[i].length; j<l; j++) {
                var a = arr.slice(0); // clone arr
                a.push(arg[i][j]);
                if (i==max)
                    r.push(a);
                else
                    helper(a, i+1);
            }
        }
        helper([], 0);
        return r;
    }
}

