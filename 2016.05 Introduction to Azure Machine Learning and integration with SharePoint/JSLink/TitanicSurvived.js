(function () {

	var survivedFiledContext = {};
	survivedFiledContext.Templates = {};
	survivedFiledContext.Templates.Fields = {
		// Apply the new rendering for Survived field on List View
		"Survived": { "View": survivedFiledTemplate }
	};

	SPClientTemplates.TemplateManager.RegisterTemplateOverrides(survivedFiledContext);

})();

// This function provides the rendering logic for list view
function survivedFiledTemplate(ctx) {

	var id = ctx.CurrentItem["ID"];
	var pClass = ctx.CurrentItem["Passenger_x0020_Class"];
	var sex = ctx.CurrentItem["Sex"];
	var age = ctx.CurrentItem["Age"];

	return "<span style='color :#f00'>" + pClass + " - " + sex + " - " + age + "</span>";
}
