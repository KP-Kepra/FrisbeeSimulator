import { Meteor } from 'meteor/meteor';

Meteor.startup(() => {
  // code to run on server at startup
    //console.log(JSON.parse(Assets.getText('frisbeeValues.json')))
    Meteor.call('wipeFrisbeePoints');
    let jsonContents = JSON.parse(Assets.getText('frisbeeValues.json'));
    console.log("Processing JSON Points");
    // for(let i = 0; i < jsonContents.length; i++) {
    //     Meteor.wrapAsync(Meteor.call('frisbeePoint.insert', jsonContents[i].Tick, jsonContents[i].Points.X, jsonContents[i].Points.Y,
    //     jsonContents.Points.Z));
    // }
    let arr = [];
    for(let i = 0; i < jsonContents.length; i++) {
        arr.push({tick:jsonContents[i].Tick, x: jsonContents[i].Points.X,
            y:jsonContents[i].Points.Y, z:jsonContents[i].Points.Z})
    }
    console.log(arr.length);
    console.log(arr);
    for(let i = 0; i < jsonContents.length; i++) {
        Meteor.setTimeout(function() {
            let _tick = jsonContents[i].Tick;
            let _X = jsonContents[i].Points.X;
            let _Y = jsonContents[i].Points.Y;
            let _Z = jsonContents[i].Points.Z;
            if(_X && _Y && _Z) {
                Meteor.call('frisbeePoint.insert', _tick, _X, _Y, _Z);
            }
        }, 500);
    }
 //   console.log(jsonContents[0].Points.Z);
    // for (let elem in jsonContents) {
    //     arr.push(elem);
    //     console.log(elem);
    // }
    // console.log("Split");
    // console.log(arr);
    // console.log(arr.length);
});
