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

	var pClass = ctx.CurrentItem["Class"];
	var gender = ctx.CurrentItem["Gender"];
	var age = ctx.CurrentItem["Age"];

	return "<span style='color :#f00'>" + pClass + " - " + gender + " - " + age + "</span>";
}
