﻿var PageAction = function (id) {
    var url = '/Account/Model';
    function AddParams(name, val) {
        var char = url.indexOf('?') >= 0 ? "&" : "?"
        return url + char + name + "=" + val;
    };
    function GetSelectedRowId() {
        var selectids = [];
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
                alert('请选择一行删除.');
            }
            alert('删除：' + id);

        },
        Edit: function () {
            alert('编辑：' + id);

            url = AddParams('id', id);
            window.parent.ControlIFrame(url);
        },
        Add: function () {
            alert("add");
            window.parent.ControlIFrame(url);
        },
        SetAuth: function () {
            alert('授权：' + id);
            url = AddParams('id', id);
            window.parent.ControlIFrame(url);
        },
        ControlBtn: function () {
            var l = GetSelectedRowId().length;
            if (l > 1) {
                $('#b-edit,#b-auth,#b-del').addClass('disabled');
                $('#b-del').removeClass('disabled');
            } else if (l == 1) {
                $('#b-edit,#b-auth,#b-del').removeClass('disabled');
            } else {
                $('#b-edit,#b-auth,#b-del').addClass('disabled');
            }
        }

    }
};
var onDataGridCheck = function (e) {
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
    var table = $('#example').dataTable({
        "order": [[1, 'asc']],
        "processing": true,
        "bServerSide": true,
        "scrollCollapse": true,

        "sAjaxSource": "/Account/GridDatas",
        "sPaginationType": "full_numbers",
        "oLanguage": {
            "searchDefalutText": "订单号",
            "allFilter": 'OrderNumber', //过滤那些列
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
            $(".settings-button").each(function () {
                $(this).toolbar({
                    content: '#user-options',
                    position: 'bottom'
                }).on('toolbarItemClick', function (e, item, val) {
                    var action = $(item).attr('action');
                    PageAction(val)[action]();
                });
            });
        },
        "aoColumns": [
                        { data: null, defaultContent: '', bSortable: false, sWidth: 5,
                            "mRender": function (val, isShow, row) {
                                return '<input style="margin-left: 7px;" type="checkbox" id="' + val + '" onClick="onDataGridCheck(this)"/>'
                            }
                        },
                        { "data": "OrderNumber", "sName": 'OrderNumber', "sTitle": "订单号" },
                        { "data": "Name", "sName": 'Name', "sTitle": "公司名称", "sWidth": 150 },
                        { "data": "LoginKey", "sName": 'LoginKey', "sTitle": "管理账户" },
                        { "data": "StopedDisplay", "sName": 'Stoped', "sTitle": "是否停用", "bSortable": false },
                        { "data": "CreateDateDisplay", "sName": 'CreateDate', "sTitle": "创建日期" },
                        { "data": "Id", "sTitle": "操作", "bSortable": false,
                            "mRender": function (val, isShow, row) {
                                return '<div class="settings-button" v="' + val + '"><img src="../../Content/Img/icon-cog-small.png" /></div>';
                            }
                        }
                    ]
    });
    $('#b-add,#b-edit,#b-del,#b-auth').on('click', function () {
        var action = $(this).attr('action'), val = '';
        if (action != 'add') {
            val = "";
        }
        PageAction(val)[action]();
    });
    $('#checkall').bind('click', function () {
        var check = $(this).attr("checked") == undefined ? false : true;
        $("#example tbody tr input[type='checkbox']").each(function () {
            //alert(typeof(this));
            $(this).attr("checked", check);
            onDataGridCheck(this);
        });
    });
    $("body").click(function (e, t) {
        if (selectself) selectself.hide();
    });
});