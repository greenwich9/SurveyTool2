var QuestionModel = function(data) {
    var self = this;

    self.id = ko.observable(0);
    self.title = ko.observable().extend({ required: true });
    self.type = ko.observable().extend({ required: true });
    self.body = ko.observable().extend({ required: true });
    self.isActive = ko.observable(true);
    self.isRequired = ko.observable("Yes").extend({ required: true });

    self.modal = $('#add-option');
    self.options = ko.observableArray([]);
    self.current = ko.observable();

    self.activeText = ko.computed(function() {
        return self.isActive() ? "true" : "false";
    }, self);

    self.isValid = function() {
        return self.title.isValid() && self.type.isValid() && self.body.isValid();
    };

    self.enable = function() {
        self.isActive(true);
    };

    self.disable = function() {
        self.isActive(false);
    };

    self.containOption = function () {
        //return "false";
        return self.type() === "RadioBox" && self.isActive;
    };


    self.hasOptions = ko.computed(function () {
        return self.options().length > 0;
    }, self);

    self.optionCount = ko.computed(function () {
        return self.options().length;
    }, self);

    // Functions

    self.newOption = function () {
        self.current(new OptionModel());
        self.modal.modal();
    };

    self.editOption = function (item) {
        self.current(item);
        self.modal.modal();
    };

    self.saveOption = function (item) {
        var index;
        if (item.isValid()) {
            index = self.options.indexOf(item);
            if (index >= 0) {
                self.options.splice(index, 1);
                self.options.splice(index, 0, item);
            } else {
                self.options.push(item);
            }

            self.modal.modal('hide');
        }
        else {
            alert('Error: All fields are required!');
        }
    };

    self.moveUp = function (item) {
        var currIndex = self.options.indexOf(item),
            prevIndex = currIndex - 1;

        if (currIndex > 0) {
            self.options.splice(currIndex, 1);
            self.options.splice(prevIndex, 0, item);
        }
    };

    self.moveDown = function (item) {
        var currIndex = self.options.indexOf(item),
            nextIndex = currIndex + 1,
            lastIndex = self.options().length - 1;

        if (currIndex < lastIndex) {
            self.options.splice(currIndex, 1);
            self.options.splice(nextIndex, 0, item);
        }
    };

    // Callbacks

    self.afterAdd = function (elem) {
        var el = $(elem);
        if (elem.nodeType === 1) {
            el.before("<div/>");
            el.prev()
                .width(el.innerWidth())
                .height(el.innerHeight())
                .css({
                    "position": "absolute",
                    "background-color": "#ffff99",
                    "opacity": "0.5"
                })
                .fadeOut(500);
        }
    };

    // Initialize

    if (data != null) {
        for (var i = 0; i < data.Options.length; i++) {
            var q = new OptionModel();
            q.id(data.Options[i].Id);
            q.text(data.Options[i].Text);
            
            q.isActive(data.Options[i].IsActive);
            self.options.push(q);
        }
    }

    return self;

};