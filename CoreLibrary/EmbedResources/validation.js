
var as = {
    validateInput: {
        required: function (input) {
            
            var type = input.type;
            switch (type) {
                case 'text':
                case 'textarea':
                case 'select-one': {
                    return (input.value != null && input.value != "") ? true : false;
                } break;
                case 'select-multiple': {
                    var inputElement = input.getElementsByTagName('option');
                    for (var i = 0; i < inputElement.length; i++) {
                        if (inputElement[i].selected) {
                            return true;
                        }
                    }
                    return false;
                } break;
                case 'file': {
                    return (input.files.length > 0) ? true : false;
                } break;
                default: {
                    var span = input.parentElement;
                    var inputElement = span.getElementsByTagName('input');
                    for (var i = 0; i < inputElement.length; i++) {
                        if (inputElement[i].checked) {
                            return true;
                        }
                    }
                    return false;
                }
            }
        },
        regExp: function (input, opt) {
            return new RegExp('^' + opt.pattern + '$').test(input.value);
        },
        email: function (input) {
            var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            return filter.test(input.value);
        }
    }
};
function validateModel(divElement) {
    var isValid = true;
    var earliestErrorInput = null;
    var lstSpan = divElement.querySelectorAll('span.model-input');
    for (var i = 0; i < lstSpan.length; i++) {
        var rules = lstSpan[i].getAttribute('data-validation');
        if (rules != null) {
            rules = JSON.parse(rules);
            var input = lstSpan[i].querySelector('input');
            var msgTag = input.parentElement.querySelector('p.warning-validation');
            if (msgTag == null) {
                msgTag = document.createElement('p');
                msgTag.className = 'warning-validation';
                msgTag.style.color = 'red';
                msgTag.style.fontWeight = '600';
                input.parentElement.appendChild(msgTag);
            }
            for (var j = 0; j < rules.length; j++) {
                var name = rules[j].rule;
                var warning = rules[j].msg;
                if (as.validateInput[name](input, rules[j].options)) {
                    msgTag.style.display = 'none';
                }
                else {
                    if (!earliestErrorInput) earliestErrorInput = input;
                    msgTag.style.display = 'block';
                    msgTag.innerHTML = warning;
                    
                    isValid = false;
                    break;
                }
            }

        }
    }
    if (!isValid && toggleLoadingPanel) {
        earliestErrorInput.parentElement.parentElement.scrollIntoView();
        toggleLoadingPanel();
    }
    return isValid;
}