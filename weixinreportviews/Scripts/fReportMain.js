var PageAction = function (id) {
    var urlaccount = '/FReportMain/Model';
    function AddParams(name, val, url) {
        var char = url.indexOf('?') >= 0 ? "&" : "?"
        return url + char + name + "=" + val;
    };
    function GetSingleSelectedRowData() {
        if (id != undefined && id != "") {
            var name = $('#example tbody tr td div.pressed').attr('n')
            return { id: id, name: name };
        }

        var selectedDom = $('#example tbody tr.selected td div');
        return { id: selectedDom.attr('v'), name: selectedDom.attr('n') };
    }
    function GetSelectedRowId() {

        var selectids = [];

        if (id != undefined && id != "") {
            selectids.push(id);
            return selectids;
        }
        var selectedDom = $('#example tbody tr.selected');
        $.each(selectedDom, function () {
            var id = $(this).find('td div').attr('v');
            selectids.push(id);
        });
        return selectids;
    }
    return {
        Delete: function () {

            var ids = GetSelectedRowId();
            if (ids.length == 0) {
                return;
            }
            layer.confirm('确定删除吗？', function () {
                layer.load('删除中..', 0);
                $.ajax({
                    type: "post",
                    url: "/FReportMain/Delete",
                    data: { "id": ids.join(',') },
                    dataType: "json",
                    success: function (data) {
                        if (data.result == 1) {
                            layer.alert(data.msg, 3);
                        }
                        else {
                            layer.closeAll();
                            $("#example").dataTable().fnDraw();
                        }
                    }
                });
            });


        },
        Edit: function () {

            var ids = GetSelectedRowId();
            if (ids.length != 1) {
                return;
            }
            url = AddParams('id', ids[0], urlaccount);
            window.parent.ControlIFrame(url);
        },
        Add: function () {
            window.parent.ControlIFrame(urlaccount);
        },
        CreateHtml: function () {
            debugger
            var o = GetSingleSelectedRowData();
            if (o.name&&o.name.length>32) {
                window.open('http://' + window.location.host + "/" + o.name.replace(/\/\//g, '/'));
            } else {
                layer.alert("预览地址不存在!");
            }
        },
        ControlBtn: function () {
            var l = GetSelectedRowId().length;
            if (l > 1) {
                $('#b-edit,#b-auth').addClass('disabled');
                $('#b-del').removeClass('disabled');
            } else if (l == 1) {
                $('#b-edit,#b-auth,#b-del').removeClass('disabled');
            } else {
                $('#b-edit,#b-auth,#b-del').addClass('disabled');
            }
        }

    }
};

window.onDataGridCheck = function (e) {
    var checked = $(e).attr("checked") == 'checked'
    if (checked) {
        $(e).parent().parent().addClass('selected');
    } else {
        $(e).parent().parent().removeClass('selected');
    }
    /*控制按钮可用不可用*/
    PageAction().ControlBtn();

};
$(document).ready(function () {
    /*
    重绘列表
    $('#example').dataTable().fnDraw();
    */
    /*绑定grid的check的方法*/
    var BindGridChecked = function () {
        $('#b-edit,#b-auth,#b-del').addClass('disabled');
        $('#checkall').bind('click', function () {
            var check = $(this).attr("checked") == undefined ? false : true;
            $("#example tbody tr input[type='checkbox']").each(function () {
                //alert(typeof(this));
                $(this).attr("checked", check);
                onDataGridCheck(this);
            });
        });
    };
    var table = $('#example').dataTable({
        "order": [[1, 'asc']],
        "processing": false,
        "bServerSide": true,
        "scrollCollapse": true,

        "sAjaxSource": "/FReportMain/GridDatas",
        "sPaginationType": "full_numbers",
        "oLanguage": {
            "searchDefalutText": "订单号",
            "allFilter": 'Title,ReportKey', //过滤那些列
            "sLengthMenu": "每页显示 _MENU_ 条记录",
            "sZeroRecords": "抱歉， 没有找到",
            "sInfo": "从 _START_ 到 _END_ /共 _TOTAL_ 条数据",
            "sInfoEmpty": "没有数据",
            "sInfoFiltered": "(从 _MAX_ 条数据中检索)",
            "sZeroRecords": "没有检索到数据",
            "sSearch": "",
            "oPaginate": {
                "sFirst": "首页",
                "sPrevious": "前一页",
                "sNext": "后一页",
                "sLast": "尾页"
            }

        },
        columnDefs: [{
            targets: [1],
            orderData: [1, 2]

        }],
        "fnDrawCallback": function (oSettings) {
            $('#checkall').attr("checked",false);
            BindGridChecked();
          
            $(".settings-button").each(function () {
                $(this).toolbar({
                    content: '#user-options',
                    position: 'bottom'
                }).on('toolbarItemClick', function (e, item, val) {
                    var action = $(item).attr('action');
                    PageAction(val)[action]();
                });
            });
            if (window.layerFilter) {
                layer.close(window.layerFilter);
            }
        },
        "aoColumns": [
            { data: null, defaultContent: '', bSortable: false, sWidth: 2,
                "mRender": function (val, isShow, row) {
                    return '<input style="margin-left: 15px;" type="checkbox" id="' + val + '" onClick="onDataGridCheck(this)"/>'
                }
            },  
            { "data": "Title", "sName": 'Title', "sTitle": "标题", "sWidth": 250 },
            { "data": "ReportKey", "sName": 'ReportKey', "sTitle": "关键字", "sWidth": 150 },
            { "data": "CreateDateDisplay", "sName": 'CreateDate', "sTitle": "创建日期", "sWidth": 70 },
            { "data": "StopedDisplay", "sName": 'Stoped', "sTitle": "停用", "bSortable": false, "sWidth": 70 },
            { "data": "Id", "sTitle": "操作", "bSortable": false, "sWidth": 70,
                "mRender": function (val, isShow, row) {
                    return '<div class="settings-button" v="' + val + '" n="' + row['Url'] + '"><img src="../../Content/Img/icon-cog-small.png" /></div>';
                }
            }
        ]
    });
    $('#b-add,#b-edit,#b-del,#b-auth').on('click', function () {
        if ($(this).hasClass('disabled')) return;
        var action = $(this).attr('action'), val = '';
        if (action != 'add') {
            val = "";
        }
        PageAction(val)[action]();
    });
    $("body").click(function (e, t) {
        if (selectself) selectself.hide();
    });
    BindGridChecked();
});