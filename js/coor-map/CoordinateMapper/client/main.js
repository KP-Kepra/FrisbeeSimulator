import {Template} from 'meteor/templating';
import {ReactiveVar} from 'meteor/reactive-var';

var time = new ReactiveVar(0);
var midpointX = new ReactiveVar(0);
var midpointY = new ReactiveVar(0);
var output = new ReactiveVar("");

import './main.html';

var drawnPath;
var raster;
var pointsArray = [];

Meteor.subscribe("images");

// Template.mainLayout.onCreated(function() {
//     $('container').append('<script src="//cdnjs.cloudflare.com/ajax/libs/paper.js/0.9.23/paper-core.min.js"></script>');
// });

Template.canvas.onRendered(() => {
    let canvas = document.getElementById('framePreview');

    paper.setup(canvas);

    let url = 'http://localhost:3000/cfs/files/images/Andx6nqBdfib2A6tR/photo.jpg';
    // let url = 'http://localhost:3000/cfs/files/images/B6rjXych2ZJGhahKq/Screen%20Shot%202017-01-24%20at%2011.52.49.png'

    generateNewRaster(url);

    drawnPath = new paper.Path();
    drawnPath.strokeColor = 'black';
    drawnPath.strokeWidth = 4;
    paper.view.draw();

    HTTP.get(Meteor.absoluteUrl("/myfile.json"), function(err,result) }
    console.log(result.data);
    });
    let result = JSON.parse(Assets.getText("thomasthecuck.json"));
    Session.set("output", result);
});

Template.canvas.events({
    'click': (event) => {
        let newArrayPoint = {
            x: event.offsetX,
            y: event.offsetY
        };
        pointsArray.push(newArrayPoint);
        let newPoint = new paper.Point(event.offsetX, event.offsetY);
        drawnPath.add(newPoint);
        console.log(pointsArray);
    }
});

Template.home.onRendered(() => {
    Meteor.setInterval(function () {
        Session.set("time", new Date().toLocaleString())
    }, 1000);
});

Template.home.helpers({
    images: () => {
        return Images.find(); // Where Images is an FS.Collection instance
    },
    showTimeString: () => {
        return getDispTime()
    },
    showMidX: () => {
        return getMidX()
    },
    showMidY: () => {
        return getMidY()
    },
    showOutput: () => {
        return getOutput()
    }
});

var getDispTime = function () {
    return Session.get('time');
};

var getMidX = function() {
    return Session.get('midpointX');
};

var getMidY = function() {
    return Session.get('midpointY');
};

var getOutput = function() {
    return Session.get('output');
}


Tracker.autorun(function() {
    var time = Session.get('time');
    var midpointX = Session.get('midpointX');
    var midpointY = Session.get('midpointY');
    var output = Session.get('output');
});

Template.home.events({
    'click #processPath': (event) => {
        if(pointsArray.length === 0) {
            alert("No points selected")
        }
        else{
            console.log("Processing");
            let totalX = 0;
            let totalY = 0;
            for (let obj of pointsArray) {
                totalX += obj.x;
                totalY += obj.y;
            }
            totalX /= pointsArray.length;
            totalY /= pointsArray.length;

            console.log(totalX + " | " + totalY);

            Session.set("midpointX", totalX);
            Session.set("midpointY", totalY);

            redrawPath();
        }
    },
    'click #deleteFileButton ': (event) => {
        console.log("deleteFile button ", this);
        console.log(event.currentTarget.name);
        Images.remove({_id: event.currentTarget.name});
    },
    'click #loadRasterButton ': (event) => {
        //console.log(this);
        // let tempObj = Images.find({_id: event.currentTarget.name}).fetch();
        console.log(event.currentTarget.name);
        raster.remove();
        generateNewRaster(event.currentTarget.name);

        redrawPath();
        //raster.remove();
        //generateNewRaster(this.url);
    },
    'change .imageUploader': (event, template) => {
        console.log("Uploading new item");
        FS.Utility.eachFile(event, function (file) {
            Images.insert(file, function (err, fileObj) {
                // Inserted new doc with ID fileObj._id, and kicked off the data upload using HTTP
            });
        });
    }
});

var getMidpointX = function () {
    return Session.get('midpointX');
};

var getMidpointY = function () {
    return Session.get('midpointY');
};

Tracker.autorun(function () {
    var midpointX = Session.get('midpointX');
    var midpointY = Session.get('midpointY');
});

var generateNewRaster = function (src) {
    let rasterSrc = src;
    //raster.remove();
    raster = new paper.Raster({
        source: rasterSrc
    });
    raster.onLoad = function () {
        let width = paper.view.size.width;
        let height = paper.view.size.height;
        console.log("screen dimensions: " + width + " " + height);
        let midPoint = [width / 2, height / 2];
        let paddingLeft = width / 40;
        let scale = (height / this.height) * (90 / 100);
        console.log("scale: " + scale);
        this.scale(scale);
        console.log("img dimensions: " + this.height + " " + this.width);
        this.position = [midPoint[0] + paddingLeft - (width - this.width * scale) / 2, midPoint[1]];
        console.log("img position: " + this.position);
    };
};

var redrawPath = function() {
    pointsArray = [];
    drawnPath.remove();
    drawnPath = new paper.Path();
    drawnPath.strokeColor = 'black';
    drawnPath.strokeWidth = 4;
};

var frisbeeJson = function() {

}