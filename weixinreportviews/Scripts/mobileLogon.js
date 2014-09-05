var createLoginForm = function (e) {
    var t = Zepto, n = function () {
        var e = !1;
        inputs = m.getVal();
        switch (!1) {
            case !!inputs.account:
                s.trigger("Warning", [f, "你还没有输入帐号！"]);
                break;
            case !!inputs.password:
                s.trigger("Warning", [l, "你还没有输入密码！"]);
                break;
            default:
                e = !0;
        }
        return e;
    }, r = function () {
        h.val(""), p.attr("src", "/cgi-bin/verifycode?username=" + m.getVal().account + "&r=" + +(new Date));
    }, i = e.selector, s = t(i.error), o = t(i.errorArea), u = t(i.rememberAcct), a = t(i.changeImgLink), f = t(i.account).keydown(function (e) {
        e.keyCode == 13 && l.focus().select();
    }), l = t(i.password).keydown(function (e) {
        e.keyCode == 13 && d.click();
    }), c = t(i.verifyArea).data("isHide", 1).hide(), h = t(i.verify).keydown(function (e) {
        e.keyCode == 13 && (l.val() ? d.click() : l.focus().select());
    }), p = t(i.verifyImg).click(r), d = t(i.loginBtn), v = function (e, t) {
        return;
        var n, r, i;
    };
    s.bind("Warning", function (e, t, n) {
        o.removeClass("dn"), s.text(n).hide()/*hanyx.fadeIn();*/
    }), p.bind({
        load: function () {
            v([f, l]), c.show().data("isHide", 0), h.focus().select();
        },
        error: function () { }
    }), s.bind("Response", function (e, t, n) {
        o.removeClass("dn"), s.html(n).hide()/*.fadeIn()*/, v([f, l], "N"), c.data("isHide") || r();
        switch (t) {
            case "-3":
                l.focus().select();
                break;
            case "-6":
                h.focus().select();
                break;
            default:
                f.focus().select();
        }
        t != "-32" && l.val("");
    });
    var m = {
        showVerifyImg: r,
        submit: function () {
            if (!n()) return;
            var e = m.getVal();
            t.post("/Logon/Login", {
                loginkey: e.account,
                password: e.password
            }, function (t) {
                
                var n = t.error + "", i;
                switch (n) {

                    case "2":
                        i = "帐号停用。";
                        break;
                    case "3":
                        i = "密码错误。";
                        break;
                    case "1":
                        i = "帐户不存在。";
                        break;
                    case "0":
                        i = "成功登录，正在跳转...", location.href = '/FReportHome/Index';
                        return;
                    case "4":
                        i = "成功登录，正在跳转...", location.href = '/Home/Index';
                        return;
                    default:
                        i = "未知的返回。";
                        return;
                }
                s.trigger("Response", [n, i]);
            }, "json");
        },
        getVal: function () {
            return {
                account: t.trim(f.val()),
                password: t.trim(l.val()),
                verify: t.trim(h.val())
            };
        },
        setVal: function (e, n) {
            return t(i).val(n).length;
        }
    };
    return a.click(function () {
        
        m.showVerifyImg();
    }), d.click(m.submit), f.focus(), m;
};

