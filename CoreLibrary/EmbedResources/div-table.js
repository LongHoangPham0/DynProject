var divTable = {
    init: function () {
        var tables = document.querySelectorAll('div.l-table');
        for (var i = 0; i < tables.length; i++) {
            tables[i].header = tables[i].querySelector('div.l-head');
            tables[i].body = tables[i].querySelector('div.l-body');
            //var scrollSize = this.getScrollBarSize();
            var contentWidth = tables[i].header.firstElementChild.clientWidth;

            
            if (tables[i].header.clientWidth > contentWidth) {
                tables[i].header.style.maxWidth = contentWidth + 'px';
                tables[i].body.style.maxWidth = contentWidth + 'px';
                tables[i].body.style.overflowX = 'hidden';
            }
            
            tables[i].body.table = tables[i];
            tables[i].body.onscroll = this.triggerScrollEvent;
            var noItem = tables[i].querySelector('div.l-no-item');
            if (noItem != null) {
                noItem.style.width = contentWidth + 'px';
            }

        }

    },
    triggerScrollEvent: function () {

        window.status = this.scrollTop;
        //this.table.header.style.overflowY = 'scroll';
        this.table.header.scrollLeft = this.scrollLeft;
        if (this.table.header.scrollLeft != this.scrollLeft) {
            this.scrollLeft = this.table.header.scrollLeft;
        }
    },
    scrollBarSize: 0,
    getScrollBarSize: function () {
        if (this.scrollBarSize == 0) {
            var scrollDiv = document.createElement("div");
            var styleObj = scrollDiv.style;
            styleObj.width = '100px';
            styleObj.height = '100px';
            styleObj.overflow = 'scroll';
            styleObj.position = 'absolute';
            styleObj.top = '-900px';
            document.body.appendChild(scrollDiv);
            // Get the scrollbar width
            this.scrollBarSize = scrollDiv.offsetWidth - scrollDiv.clientWidth;
            // Delete the DIV 
            document.body.removeChild(scrollDiv);
        }
        return this.scrollBarSize;
    }
};
window.addEventListener('load', function () { divTable.init(); });