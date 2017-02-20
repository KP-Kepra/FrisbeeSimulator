Meteor.methods({
    'deleteFile': function (_id) {
        var upload = Images.findOne(_id);
        if (upload == null) {
            throw new Meteor.Error(404, 'Upload not found'); // maybe some other code
        }
        Images.remove(_id);
    }
});