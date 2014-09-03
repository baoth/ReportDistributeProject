var FlowToolBar = {
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
        $(".layer-pop-content .content-box li").tap(function () {
            func($(this).attr('id'));
        });
        $(".layer-pop-content .content-box li").bind('click',function () {
            func($(this).attr('id'));
        });
    },
    hideitem:function(id){
        $(".layer-pop-content li").removeClass("box-item-hide");
        $("#" + id).addClass("box-item-hide");
    }

};


var PicContentList = {
    selectedId: "",
    idAliasName: "",
    filterAliasName: "",
    titleAliasName: "",
    imgurlAliasName: "",
    markAliasName: "",
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

            if (d[this.filterAliasName] == undefined) d[this.filterAliasName] = "";
            if (d[this.idAliasName] == undefined) d[this.idAliasName] = "";
            if (d[this.titleAliasName] == undefined) d[this.titleAliasName] = "";
            if (d[this.imgurlAliasName] == undefined) d[this.imgurlAliasName] = "";
            if (d[this.markAliasName] == undefined) d[this.markAliasName] = "";


            var filterval = d[this.filterAliasName], idval = d[this.idAliasName],
                titleval = d[this.titleAliasName], imgval = d[this.imgurlAliasName],
                markval = d[this.markAliasName];

            var marksect = "             <i class='site-icons'></i>";
            if (markval != undefined && markval != 0) {
                var marksect = "             <i class='site-icons site-icons-marked'></i>";
            }
            var filtersect = "     <div class='filter'>" +
                                  "         <span></span>" +
                                  "         <i>" + filterval + "</i>" +
                                  "     </div>";
            var txtsect = " <div class='show-txt'>" +
                                  "     <div class='info'>" +
                                  "         <p class='tit tit-p'>" +
                                  "             <a>" + titleval + "</a>" +
                                  "         </p>" +
                                  "     </div>" +
                                  " </div>";
            if (this.showfilter != true) filtersect = "";
            if (this.showtxt != true) txtsect = "";
            var lisect = "<li class='clear' id='" + idval + "'>";

            if (i % 3 == 0) {
                lisect = "<li class='clear clear-left' id='" + idval + "'>";
            }
            else if (i % 3 == 1) {
                lisect = "<li class='clear clear-middle' id='" + idval + "'>";
            }
            else {
                lisect = "<li class='clear clear-right' id='" + idval + "'>";
            }
            $(content).append(lisect +
                                " <div class='show-pic'>" +
                                "     <a class='pic' target='' title='' href='#'>" +
                                "         <img src='" + imgval + "' title='' />" +
                                "         <p class='site-piclist_icons'>" +
                                                marksect +
                                "         </p>" +
                                "     </a>" +
                                    filtersect +
                                " </div>" +
                                    txtsect +
                                "</li>");

        }

        //绑定事件
        $(".cfix li").bind('dblclick', function () {

            PicContentList.selectedId = $(this).attr("id");
            PicContentList.dblclick();
            var mid = PicContentList.selectedId;
            var a = PicContentList.marked(mid, 0);
            if (a == true) {
                FlowToolBar.hideitem("bind");
            }
            else {
                FlowToolBar.hideitem("unbind");
            }
            FlowToolBar.show();
        });
        $(".cfix li").doubleTap(function () {

            PicContentList.selectedId = $(this).attr("id");
            PicContentList.dblclick();
            var a = PicContentList.marked(PicContentList.selectedId, 0);
            if (a == true) {
                FlowToolBar.hideitem("bind");
            }
            else {
                FlowToolBar.hideitem("unbind");
            }
            FlowToolBar.show();
        });
        $(".content-box-item").tap(function () {
            PicContentList.selectedId = $(this).attr("id");
            PicContentList.click();
        });
    },
    clear: function () {
        $(".contentList li.clear").remove();
    },
    mark: function (id, mark) {
        if (id == undefined) return;
        var sid = "#" + id + " .site-icons";
        if (mark == undefined || mark == 0) {
            $(sid).addClass("site-icons-marked");
        }
    },
    unmark: function (id, mark) {
        if (id == undefined) return;
        var sid = "#" + id + " .site-icons";
        if (mark == undefined || mark == 0) {
            $(sid).removeClass("site-icons-marked");
        }
    },
    marked: function (id, mark) {
        if (id == undefined) return false;
        var sid = "#" + id + " .site-icons";
        if (mark == undefined || mark == 0) {
            return $(sid).hasClass("site-icons-marked");
        }
    },
    render: function (id, data) {
        if (id == undefined || data == undefined) return;
        $("#" + id + " .filter i").val(data.NickName);
        $("#" + id + " .show-pic img").attr('src', data.HeadImgUrl);
    },
    remove: function (id) {
        if (id == undefined) return;
        $("#" + id).remove();

        var i = 0;
        $(".contentList .cfix li").each(function () {
            $(this).removeClass("clear-left");
            $(this).removeClass("clear-middle");
            $(this).removeClass("clear-right");
            if (i % 3 == 0) {
                $(this).addClass("clear-left");
            }
            else if (i % 3 == 1) {
                $(this).addClass("clear-middle");
            }
            else {
                $(this).addClass("clear-right");
            }
            i = i + 1;
        });
    },
    click: function () { },
    dblclick: function () { },

    bind: function (id) {
        if (id == undefined) return;
        $.ajax({
            type: "POST",
            url: "../FReportUserAttention/BindUpdate",
            data: { 'OpenId': id, 'Binded': 1 },
            dataType: "json",
            success: function (data) {
                FlowToolBar.hide();
                if (data.result == 1) {
                    alert(data.msg);
                }
                else {
                    PicContentList.mark(id, 0);
                }
            },
            error: function (e) {
                FlowToolBar.hide();
                alert('操作错误!')
            }
        });
    },

    unbind: function (id) {
        if (id == undefined) return;
        $.ajax({
            type: "POST",
            url: "../FReportUserAttention/BindUpdate",
            data: { 'OpenId': id, 'Binded': 0 },
            dataType: "json",
            success: function (data) {
                FlowToolBar.hide();
                if (data.result == 1) {
                    alert(data.msg);
                }
                else {
                    PicContentList.unmark(id, 0);
                }
            },
            error: function (e) {
                FlowToolBar.hide();
                alert('操作错误!')
            }
        });
    },

    refresh: function (id) {
        if (id == undefined) return;
        $.ajax({
            type: "POST",
            url: "../FReportUserAttention/GetRowByFilter",
            data: { 'OpenId': id },
            dataType: "json",
            success: function (data) {
                FlowToolBar.hide();
                if (data.result == 1) {

                }
                else {
                    PicContentList.render(id, data);
                }
            },
            error: function (e) {
                FlowToolBar.hide();
                alert('操作错误!')
            }
        });
    },

    removeen: function (id) {
        if (id == undefined) return;
        $.ajax({
            type: "POST",
            url: "../FReportUserAttention/Delete",
            data: "id=" + id + ",0,''",
            dataType: "json",
            success: function (data) {
                FlowToolBar.hide();
                if (data.result == 1) {
                    alert(data.msg);
                }
                else {
                    PicContentList.remove(id);
                }
            },
            error: function (e) {
                FlowToolBar.hide();
                alert('操作错误!')
            }
        });
    }
};

