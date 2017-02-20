/**
 * Created by kristiansuhartono on 9/1/2017.
 */

Meteor.publish("images", () => {
    console.log("Publishing image data");
    return Images.find();
})