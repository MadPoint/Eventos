(function () {
	var url = ""; //Azure ML Web Service URL
	var apiKey = ""; //Azure ML API Key for your experiment web service

	var proxyUrl = ""; //Url to the azurewebsite where you host your proxy webapi. Sample: 'https://mywebsite.azurewebsites.net/api/proxy'

	var requests = [];

    // Create object that have the context information about the field that we want to change it's output render 
    var survivedFiledContext = {};
    survivedFiledContext.Templates = {};
    survivedFiledContext.Templates.Fields = {
        // Apply the new rendering for Survived field on List View
        "Survived": { "View": survivedFiledTemplate }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(survivedFiledContext);

	// This function provides the rendering logic for list view
	function survivedFiledTemplate(ctx) {
	
		var id = ctx.CurrentItem["ID"];
		var pClass = ctx.CurrentItem["Passenger_x0020_Class"];
		var sex = ctx.CurrentItem["Sex"];
		var age = ctx.CurrentItem["Age"];
	
		var request = function(){
			jQuery.ajax({
				type: "POST",
				url: proxyUrl,
				dataType: "json",
				success: function (json) {
					var result = JSON.parse(json);
					var values = result.Results.output1.value.Values[0];
					var scoredLabels = values[0];
					var scoredProbabilities = values[1];
					
					var span = jQuery("#survived" + id);
					span.html(scoredLabels == 1 ? "OK" : "KO");
					span.css('color', scoredLabels == 1 ? 'green' : 'red');
				},
				data: {
					"ApiKey": apiKey,
					"Url": url,
					"Data": JSON.stringify(getData(pClass, sex, age))
				} 
			}).fail(function() {
				console.log("Error getting Azure ML results");
			});
		};
	
		if (window.jQuery) {  
    		request();
		} else {
			requests.push(request);
		}
	
		return "<span id='survived" + id + "'></span>";
	}
	
	function getData(pClass, sex, age) {
		var data = {
			"Inputs": {
				"input1": {
					"ColumnNames": [
					  "PassengerId",
					  "Survived",
					  "Pclass",
					  "Name",
					  "Sex",
					  "Age",
					  "SibSp",
					  "Parch",
					  "Ticket",
					  "Fare",
					  "Cabin",
					  "Embarked"
					],
					"Values": [
					  [
						"",
						"",
						pClass,
						"",
						sex,
						age,
						"",
						"",
						"",
						"",
						"",
						""
					  ]
					]
				}
			},
			"GlobalParameters": {}
		};

		return data;
	}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Those lines are to add jQuery to the page, you can remove them if you inject this script to your site (using a CustomAction), you use a ScriptWebpart or you add it to the masterpage
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

var head = document.getElementsByTagName('head')[0];    
function getScript(url, success) {
	var script = document.createElement('script');
	script.src = url;
	var done = false;
	// Attach handlers for all browsers
	script.onload = function() {
		done = true;
		success();
		script.onload = script.onreadystatechange = null;
		head.removeChild(script);
	};

    script.onreadystatechange = function() {
		if (!done && (!this.readyState
			|| this.readyState == 'loaded'
			|| this.readyState == 'complete')) {
			done = true;
			success();
			script.onload = script.onreadystatechange = null;
			head.removeChild(script);
		}
	};
	head.appendChild(script);
}

getScript('https://code.jquery.com/jquery-1.12.3.min.js', function() {
	requests.forEach(function(request) {
		request();
	}, this);
});

})();