Images = new FS.Collection("images", {
    stores: [new FS.Store.FileSystem("images", {path: "~/uploads"})]
});
Images.allow({
    'insert': function () {
        // add custom authentication code here
        return true;
    },
    'update': function (userId, party) {
        return true;
    },
    'remove': function (userId, party) {
        return true;
    },
    'download': function(userId, fileObj) {
        return true;
    }
});

FrisbeePoints = new Mongo.Collection("frisbeePoints")