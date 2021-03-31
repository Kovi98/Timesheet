function($) {
    function createError(message) {
        var parent = $('#error-alert');
        var div = $('<div/>', {
            class: 'alert alert-danger alert-dismissible fade show',
            "role": 'alert'
        });
        var button = $('<button></button>', {
            class: 'close',
            "type": 'button',
            "data-dismiss": 'alert',
            "aria-label": 'Close'
        });
        parent.append(div.append(message).append(button.append($('<span aria-hidden="true">&times;</span>'))))
    }
    function createWarning(message) {
        var parent = $('#warning-alert');
        var div = $('<div/>', {
            class: 'alert alert-warning alert-dismissible fade show',
            "role": 'alert'
        });
        var button = $('<button></button>', {
            class: 'close',
            "type": 'button',
            "data-dismiss": 'alert',
            "aria-label": 'Close'
        });
        parent.append(div.append(message).append(button.append($('<span aria-hidden="true">&times;</span>'))))
    }
    function createSuccess(message) {
        var parent = $('#success-alert');
        var div = $('<div/>', {
            class: 'alert alert-success alert-dismissible fade show',
            "role": 'alert'
        });
        var button = $('<button></button>', {
            class: 'close',
            "type": 'button',
            "data-dismiss": 'alert',
            "aria-label": 'Close'
        });
        parent.append(div.append(message).append(button.append($('<span aria-hidden="true">&times;</span>'))))
    }
}