/**
 * Created by kristiansuhartono on 9/1/2017.
 */

FlowRouter.route("/", {
    action: () => {
        BlazeLayout.render("mainLayout", {mainContent: "home"})
    }
})
