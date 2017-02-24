/**
 * Created by kristiansuhartono on 9/1/2017.
 */

Meteor.publish("images", () => {
    console.log("Publishing image data");
    return Images.find();
});

Meteor.publish("frisbeePoints.all", () => {
    console.log("Publishing points");
    return FrisbeePoints.find();
});

