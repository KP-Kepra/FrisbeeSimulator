Meteor.methods({
    'deleteFile': function (_id) {
        var upload = Images.findOne(_id);
        if (upload == null) {
            throw new Meteor.Error(404, 'Upload not found'); // maybe some other code
        }
        Images.remove(_id);
    },
    'frisbeePoint.insert' : (tick, x, y, z)=> {
        FrisbeePoints.insert({type:"dataPoint", tick:tick, x:x, y:y, z:z})
    },
    'wipeFrisbeePoints' : () => {
        FrisbeePoints.remove({"type":"dataPoint"})
    },
    'frisbeePoint.getOne' : (tick) => {
        return FrisbeePoints.find({"tick":tick}).fetch()[0]
    }
});