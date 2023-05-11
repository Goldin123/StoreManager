jQuery.fn.tabletoxml = function (options) {
    var settings = $.extend({
        rootnode: "Root",
        childnode: "Child",
        filename: ""
    }, options);
    var clean_head = function (text) {
        text = $.trim(text.replace(/</g, '').replace(/>/g, '').replace(/ /g, ''));
        return text;
    };
    var clean_text = function (text) {
        text = $.trim(text.replace(/</g, '').replace(/>/g, ''));
        return text;
    };
    $(this).each(function () {
        var table = $(this);
        var caption = settings.filename;
        var title = [];
        var loop = 0;
        var head = 0;
        var xml = "<" + settings.rootnode + ">";
        $(this).find('tr').each(function () {
            if (head == 1) {
                xml += "<" + settings.childnode + ">";
                $(this).find('td').each(function () {
                    xml += "<" + title[loop] + ">" + clean_text($(this).text()) + "</" + title[loop] + ">";
                    loop++;
                });
                xml += "</" + settings.childnode + ">";
                loop = 0;
            }
            $(this).find('th').each(function () {
                head = 1;
                var text = clean_head($(this).text());
                title.push(text);
            });
        });
        xml += "</" + settings.rootnode + ">";
        var uri = 'data:text/xml;charset=utf-8,' + encodeURIComponent(xml);
        var download_link = document.createElement('a');
        download_link.href = uri;
        var ts = new Date().getTime();
        if (caption == "") {
            download_link.download = ts + ".xml";
        } else {
            download_link.download = caption + "-" + ts + ".xml";
        }
        document.body.appendChild(download_link);
        download_link.click();
        document.body.removeChild(download_link);
    });
};
