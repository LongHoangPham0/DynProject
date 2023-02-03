function doOnLoad() {
    window.toggleLoadingPanel(false);
    applyLoadingPanel();
}

applyLoadingPanel = function () {
    var forms = document.getElementsByTagName('form');
    for (var i = 0; i < forms.length; i++) {
        var onsubmit = forms[i].getAttribute('onsubmit');
        forms[i].addEventListener('submit', function () {
            toggleLoadingPanel(true);
        });
    }
    var as = document.getElementsByTagName('a');
    for (var i = 0; i < as.length; i++) {
        if (as[i].onclick == null && as[i].href != '#' && as[i].target != '_blank') {
            as[i].addEventListener('click', function () {
                toggleLoadingPanel(true);
            });
        }
    }
};
toggleLoadingPanel = function (visible) {

    let panel = window.toggleLoadingPanel.panel;
    if (!panel) {
        //search for existing html element
        panel = document.getElementsByClassName('loading-panel')[0];
        if (panel) {
            panel.spin = document.getElementsByClassName('loading-spin')[0];
            panel.shownCnt = 1;
        }
        else {
            panel = document.createElement('div');
            panel.spin = document.createElement('div');
            //<style>@keyframes spin360 { 100% { -webkit-transform: rotate(360deg); transform:rotate(360deg); } }</style>
            const dynamicStyles = document.createElement('style');
            document.head.appendChild(dynamicStyles);
            dynamicStyles.sheet.insertRule('@keyframes spin360 { 100% { -webkit-transform: rotate(360deg); transform:rotate(360deg); } }', 0);
            panel.shownCnt = 0;
        }
        //if (!panel.shownCnt) panel.shownCnt = 0;
        panel.className = 'loading-panel';


        window.toggleLoadingPanel.panel = panel;

        const style = panel.style;
        style.zIndex = '9999';
        style.opacity = 0.5;
        style.position = 'fixed';
        style.width = '100%';
        style.height = '100%';
        style.backgroundColor = '#CCC';

        const spin = panel.spin;
        spin.innerHTML = '֎';
        spin.className = 'loading-spin';
        spin.style.animation = 'spin360 4s linear infinite';
        spin.style.fontSize = '5em';
        spin.style.position = 'fixed';
        spin.style.left = 'calc(50% - 25px)';
        spin.style.top = '30%';
        spin.style.width = '50px';
        spin.style.height = '50px';
        spin.style.lineHeight = '53px';
        spin.style.fontFamily = 'Courier New';
        spin.style.textAlign = 'center';

        document.body.insertBefore(panel, document.body.firstChild);
        document.body.insertBefore(spin, document.body.firstChild);

    }
    panel.shownCnt += visible ? 0 : -1;
    if (panel.shownCnt === 0) {
        panel.spin.style.display = panel.style.display = visible ? 'inline-block' : 'none';
        if (visible) {
            panel.orgOverflow = document.body.style.overflow || 'initial';
            document.body.style.overflow = 'hidden';
        }
        else {
            if (panel.orgOverflow) {
                document.body.style.overflow = panel.orgOverflow;
                panel.orgOverflow = null;
            }
        }
    }
    panel.shownCnt += visible ? 1 : 0;
}
window.initModal = function () {
    if (window.popupModal != null) return;
    var m = document.createElement('alert');
    m.header = document.createElement('header');
    m.appendChild(m.header);
    m.content = document.createElement('content');
    m.appendChild(m.content);
    m.ok = document.createElement('button');
    m.appendChild(m.ok);
    m.ok.owner = m;
    m.ok.onclick = function () {
        this.owner.hide();
        if (this.owner.onOkClick != null) this.owner.onOkClick(this.owner);
    };
    m.cancel = document.createElement('button');
    m.appendChild(m.cancel);
    m.cancel.owner = m;
    m.cancel.onclick = function () {
        this.owner.hide();
        if (this.owner.onCancelClick != null) this.owner.onCancelClick(this.owner);
    };
    m.show = function (message, title, ok, cancel, onOk, onCancel) {
        this.content.innerHTML = message;
        this.header.innerHTML = title || 'Information';
        if (ok == null) {
            this.ok.style.display = 'none';
        }
        else {
            this.ok.innerHTML = ok;
            this.ok.style.display = '';
        }
        if (cancel == null) {
            this.cancel.style.display = 'none';
        }
        else {
            this.cancel.innerHTML = cancel;
            this.cancel.style.display = '';
        }
        this.bgModal.style.display = 'block';
        this.cotainer.style.display = 'block';

        this.onOkClick = onOk;
        this.onCancelClick = onCancel;
    };
    m.hide = function () {
        this.bgModal.style.display = 'none';
        this.cotainer.style.display = 'none';
    };

    m.bgModal = document.createElement('div')
    document.body.appendChild(m.bgModal);
    m.bgModal.className = 'alert-background';
    m.bgModal.style.display = 'none';
    m.bgModal.style.position = 'fixed';
    m.bgModal.style.width = '100%';
    m.bgModal.style.height = '100%';
    m.bgModal.style.top = '0px';
    m.bgModal.style.left = '0px';
    m.bgModal.style.opacity = 0.5;
    m.bgModal.style.backgroundColor = '#AAA';


    m.cotainer = document.createElement('div');
    document.body.appendChild(m.cotainer);
    m.cotainer.className = 'alert-cotainer';
    m.cotainer.style.display = 'none';
    m.cotainer.style.position = 'fixed';
    m.cotainer.style.width = '100%';
    m.cotainer.style.height = '100%';
    m.cotainer.style.top = '0px';
    m.cotainer.style.left = '0px';

    m.cotainer.appendChild(m);

    window.popupModal = m;
};
HTMLButtonElement.prototype.applyConfirm = function (message, title, okTxt, cancelTxt) {

    if (this.showConfirm == null) {
        this.confirmArgs = {
            title: title || 'Confirmation',
            message: message || 'Are you sure?',
            okTxt: okTxt || 'OK',
            cancelTxt: cancelTxt || 'Cancel',
            onOK: function (alert) {
                alert.sender.isConfirmed = true;
                alert.sender.click();
            },
            onCancel: function (alert) {
                alert.sender.isConfirmed = false;
            }
        };
        this.showConfirm = function () {
            this.isConfirmed = false;
            window.initModal();
            window.popupModal.sender = this;
            window.popupModal.show(this.confirmArgs.message, this.confirmArgs.title, this.confirmArgs.okTxt, this.confirmArgs.cancelTxt, this.confirmArgs.onOK, this.confirmArgs.onCancel);

        };

        this.orgOnclick = this.onclick;
        this.onclick = function (evt) {
            if (this.isConfirmed) {
                this.isConfirmed = false;
                if (this.orgOnclick) this.orgOnclick(evt);
                return true;
            }
            else {
                this.showConfirm();
                evt.stopPropagation();
                return false;
            }
        };

    }

};
let observer = new MutationObserver(function (mutations) {
    for (let mutation of mutations) {
        
        for (let node of mutation.addedNodes) {
            searchForButton(node);
        }
    }
});
const searchForButton = function(node){
    if (node.nodeName == 'BUTTON') {
        if (node.attributes['confirm-message']) {
            node.applyConfirm(node.attributes['confirm-message'].value)
        }
        else if (node.className.indexOf('btn-danger') >= 0) {
            node.applyConfirm('Please think twice before doing this. Are you sure?')

        }
        return;
    }
    else if (node.childNodes) {
        for (let subNode of node.childNodes) searchForButton(subNode);
    }
}
observer.observe(document, {
    childList: true,
    subtree: true
});