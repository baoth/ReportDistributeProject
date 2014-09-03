var PicContentList = {
    showtxt: true,
    showfilter: true,
    load: function (datas) {
        this.clear();
        this.append(datas);
    },
    append: function (datas) {
        if (datas == undefined || datas == null) return;

        if (datas.length == 0) return;
        var content = $('.contentList .cfix')[0];
        if (content == undefined || content == null) return;

        for (var i = 0; i < datas.length; i++) {
            var d = datas[i];

            if (d.filter == undefined) {
                d.filter = "";
            }
            var marksect = "             <i class='site-icons'></i>";
            if (d.mark != undefined && d.mark != 0) {
                var marksect = "             <i class='site-icons site-icons-marked'></i>";
            }
            var filtersect = "     <div class='filter'>" +
                                  "         <span></span>" +
                                  "         <i>" + d.filter + "</i>" +
                                  "     </div>";
            var txtsect = " <div class='show-txt'>" +
                                  "     <div class='info'>" +
                                  "         <p class='tit tit-p'>" +
                                  "             <a>" + d.title + "</a>" +
                                  "         </p>" +
                                  "     </div>" +
                                  " </div>";
            if (this.showfilter != true) filtersect = "";
            if (this.showtxt != true) txtsect = "";
            var lisect = "<li class='clear' id='" + d.id + "'>";

            if (i % 3 == 0) {
                lisect = "<li class='clear clear-left' id='" + d.id + "'>";
            }
            else if (i % 3 == 1) {
                lisect = "<li class='clear clear-middle' id='" + d.id + "'>";
            }
            else {
                lisect = "<li class='clear clear-right' id='" + d.id + "'>";
            }
            $(content).append(lisect +
                                " <div class='show-pic'>" +
                                "     <a class='pic' target='' title='' href='#'>" +
                                "         <img src='" + d.imgurl + "' title='' />" +
                                "         <p class='site-piclist_icons'>" +
                                                marksect +
                                "         </p>" +
                                "     </a>" +
                                    filtersect +
                                " </div>" +
                                    txtsect +
                                "</li>");

        }
    },
    clear: function () {
        $(".contentList li.clear").remove();
    }
};
var FlowToolBar = {
    datas: "",
    show: function () {
        var ele = $(".layer-pop-content");
        if (ele == undefined || ele == null) return;
        ele.css("display", "block");
    },
    hide: function () {
        var ele = $(".layer-pop-content");
        if (ele == undefined || ele == null) return;
        ele.css("display", "none");
    },
    bindclick: function (func) {
        $(".layer-pop-content .content-box li").bind('click', function () {
            func(FlowToolBar.datas, $(this).attr('id'));
        });
        $(".layer-pop-content .content-box li").tap(function () {
            func(FlowToolBar.datas, $(this).attr('id'));
        });
    }
};
