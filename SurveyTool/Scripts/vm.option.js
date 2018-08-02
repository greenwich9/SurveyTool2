var OptionModel = function (data) {
	var self = this;

	self.id = ko.observable(0);
	self.text = ko.observable().extend({ required: true });
	self.isActive = ko.observable(true);

	self.isValid = function () {
		return self.text.isValid();
	};

	self.enable = function () {
		self.isActive(true);
	};

	self.disable = function () {
		self.isActive(false);
	};


}