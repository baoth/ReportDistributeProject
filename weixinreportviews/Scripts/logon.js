var createLoginForm = function (e) {
    var t = jQuery, n = function () {
        var e = !1;
        inputs = m.getVal();
        switch (!1) {
            case !!inputs.account:
                s.trigger("Warning", [f, "你还没有输入帐号！"]);
                break;
            case !!inputs.password:
                s.trigger("Warning", [l, "你还没有输入密码！"]);
                break;
            case !!inputs.verify || !!c.data("isHide"):
                s.trigger("Warning", [h, "你还没有输入验证码！"]), r();
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
        o.removeClass("dn"), s.text(n).hide().fadeIn();
    }), p.bind({
        load: function () {
            v([f, l]), c.show().data("isHide", 0), h.focus().select();
        },
        error: function () { }
    }), s.bind("Response", function (e, t, n) {
        o.removeClass("dn"), s.html(n).hide().fadeIn(), v([f, l], "N"), c.data("isHide") || r();
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
            t.post("/cgi-bin/login?lang=zh_CN", {
                username: e.account,
                pwd: t.md5(e.password.substr(0, 16)),
                imgcode: c.data("isHide") ? "" : e.verify,
                f: "json"
            }, function (t) {
                var n = t.base_resp.ret + "", i;
                u.hasClass("checkbox_checked") ? WXM.Helpers.setCookie("remember_acct", e.account, 30) : WXM.Helpers.setCookie("remember_acct", "EXPIRED", -1);
                switch (n) {
                    case "-1":
                        i = "系统错误，请稍候再试。";
                        break;
                    case "-2":
                        i = "帐号或密码错误。";
                        break;
                    case "-23":
                        i = "您输入的帐号或者密码不正确，请重新输入。";
                        break;
                    case "-21":
                        i = "不存在该帐户。";
                        break;
                    case "-7":
                        i = "您目前处于访问受限状态。";
                        break;
                    case "-8":
                        i = "请输入图中的验证码", r();
                        return;
                    case "-27":
                        i = "您输入的验证码不正确，请重新输入", r();
                        break;
                    case "-26":
                        i = "该公众会议号已经过期，无法再登录使用。";
                        break;
                    case "0":
                        i = "成功登录，正在跳转...", location.href = t.redirect_url;
                        return;
                    case "-25":
                        i = '海外帐号请在公众平台海外版登录,<a href="http://admin.wechat.com/">点击登录</a>';
                        break;
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


(function (e) {
    e._parseJSON = e.parseJSON, e.parseJSON = function (t) {
        return e._parseJSON(t.replace(/(^|[\r\n])\t*/g, "").replace(/\t/g, "\\t").replace(/\\x3c/g, "<").replace(/\\x27/g, "'"));
    };
})(jQuery), function (e, t) {
    if (window.WXM) return;
    var n = window.WXM = {
        Version: "2012-06-25",
        Author: "zemzheng@tencent.com;raphealguo@tencent.com",
        DATA: {},
        Templates: {},
        Event: function () {
            var t = {}, n = {}, r = function (e) {
                var t, r = 0;
                return n[e] || (n[e] = [], r = 1), r;
            }, i = function (e, t) {
                var r = n[e], i = 0;
                return r && typeof t == "function" && (r.push(t), i = 1), i;
            }, s = function (r, i) {
                var s, o, u, a, f;
                u = 0;
                if (s = n[r]) while (o = s[u++]) setTimeout(function (e, t, n) {
                    return function () {
                        n(e, t);
                    };
                } (r, e.extend({}, i), o), 0);
                t[r] = arguments;
            }, o = function (e) {
                return t[e];
            };
            return {
                notify: s,
                bind: i,
                register: r,
                getHistory: o
            };
        } (),
        THelper: {
            format: function (e) {
                var n, r = e, i, s, o = arguments.length;
                if (o < 2) return e;
                n = 0;
                while (++n < o) r = r.replace(/%s/, "{#" + n + "#}");
                r.replace("%s", ""), n = 1;
                while ((i = arguments[n]) !== t) s = new RegExp("{#" + n + "#}", "g"), r = r.replace(s, i), n++;
                return r;
            },
            getImgDataUrl: function (e, t, r) {
                return n.ROOT.cgi + "getimgdata?msgid=" + e + "&mode=" + (t || "small") + (r ? "&source=" + r : "") + (n.DATA.token && n.DATA.token.length ? "&token=" + n.DATA.token : "&token=");
            },
            getVoiceDataUrl: function (e, t) {
                return n.ROOT.cgi + "getvoicedata?msgid=" + e + (t ? "&source=" + t : "") + (n.DATA.token && n.DATA.token.length ? "&token=" + n.DATA.token : "&token=");
            }
        },
        Handles: {},
        ROOT: {
            imgs: "/htmledition/images/",
            cgi: "/cgi-bin/",
            css: "/htmledition/style/",
            js: "/zh_CN/htmledition/js/"
        },
        Helpers: {
            flashTimer: null,
            flashStr: "",
            flashTitle: function (e) {
                var e = e || n.Helpers.flashStr;
                clearTimeout(n.Helpers.flashTimer), document.title = n.Helpers.flashStr = e.substring(1, e.length) + e.substring(0, 1), n.Helpers.flashTimer = setTimeout(n.Helpers.flashTitle, 500);
            },
            regFilter: function (e) {
                return e.replace(/([\^\.\[\$\(\)\|\*\+\?\{\\])/ig, "\\$1");
            },
            setCookie: function (e, t, n) {
                var n = n || 30, r = new Date;
                r.setTime(r.getTime() + n * 24 * 60 * 60 * 1e3), document.cookie = e + "=" + escape(t) + ";expires=" + r.toGMTString();
            },
            getCookie: function (e) {
                var t = new RegExp(["(^|;|\\s+)", n.Helpers.regFilter(e), "=([^;]*);?"].join(""));
                if (t.test(document.cookie)) try {
                    return decodeURIComponent(RegExp.$2);
                } catch (r) {
                    return RegExp.$2;
                }
            },
            getUin: function () {
                var e = n.Helpers.getCookie("uin");
                return e && e.substr(1, e.length) || 0;
            },
            getSkey: function () {
                return n.Helpers.getCookie("skey");
            },
            getPageNav: function (e, t, n) {
                return {
                    PageIdx: e,
                    PageSize: t,
                    PageCount: n
                };
            },
            parseParams: function (e) {
                e == t && (e = window.location.search);
                var n = e.replace(/^\?/, "").split("&"), r = 0, i, s = {}, o, u, a;
                while ((i = n[r++]) !== t) o = i.match(/^([^=&]*)=(.*)$/), o && (u = decodeURIComponent(o[1]), a = decodeURIComponent(o[2]), s[u] = a);
                return s;
            },
            setNavByTitle: function (e) {
                return n.Helpers.markSelectedInArr({
                    tKey: "title",
                    tVal: e,
                    list: n.DATA.nav
                });
            },
            setNavById: function (e) {
                return n.Helpers.markSelectedInArr({
                    tKey: "_id",
                    tVal: e,
                    list: n.DATA.nav
                });
            },
            markSelectedInArr: function (e) {
                var t = e.sKey || "selected", n = e.tVal, r = e.tKey, i = e.sVal || "selected", s = 0, o, u = e.list;
                if (r && n && u) while (o = u[s++]) if (n == o[r]) {
                    o[t] = i;
                    return;
                }
            },
            createPage: function () {
                var t = n.DATA, r = n.Templates, i, s, o = n.Handles, u = n.Helpers.tmpl;
                for (i in r) {
                    if (!r.hasOwnProperty(i)) continue;
                    o[i] ? o[i](i) : (s = r[i](t), e(i).html(s));
                }
            },
            createDomByHtml: function (t) {
                return e("<div></div>").html(t).children();
            },
            extendPageMsg: function (e, t) {
                var n = e.replace(/(\?|&)(pagesize|pageidx)=\d*/g, ""), r = "&pagesize=" + t.PageSize;
                return t.nextPage = t.PageIdx + 1 < t.PageCount ? n + r + "&pageidx=" + (t.PageIdx + 1) : "#", t.passPage = t.PageIdx > 0 ? n + r + "&pageidx=" + (t.PageIdx - 1) : "#", t;
            },
            htmlEncode: function (e) {
                return e.replace(/&/g, "&amp;").replace(/ /g, "&nbsp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/\n/g, "<br />").replace(/"/g, "&quot;");
            },
            htmlDecode: function (e) {
                return e.replace(/&#39;/g, "'").replace(/<br\s*(\/)?\s*>/g, "\n").replace(/&nbsp;/g, " ").replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&quot;/g, '"').replace(/&amp;/g, "&");
            },
            time: {
                timeDelta: function (e) {
                    var t = new Date;
                    return t.setHours(0), t.setMinutes(0), t.setSeconds(0), t.setMilliseconds(0), Math.round((new Date(e.getFullYear(), e.getMonth(), e.getDate()) - t) / 1e3 / 24 / 3600);
                },
                toLocaleDayString: function (e) {
                    var t = e.getDay();
                    switch (t) {
                        case 1:
                            return "星期一";
                        case 2:
                            return "星期二";
                        case 3:
                            return "星期三";
                        case 4:
                            return "星期四";
                        case 5:
                            return "星期五";
                        case 6:
                            return "星期六";
                        case 0:
                            return "星期日";
                        default:
                            return "";
                    }
                },
                newDate: function (e) {
                    var t = e.split("-");
                    return new Date(t[0], parseInt(t[1], 10) - 1, t[2]);
                },
                tranDate2LocaleDateString: function (e) {
                    var t = n.Helpers.time, r = new Date, i = t.timeDelta(e);
                    return i == 0 ? n.formatDate(new Date(e), "hh:mm") : i == -1 ? "昨天" : i >= -6 ? t.toLocaleDayString(e) : e.getFullYear() == (new Date).getFullYear() ? "" + (e.getMonth() + 1) + "月" + e.getDate() + "日" : e.toLocaleDateString();
                }
            },
            replaceQQSmile: function (e) {
                return e.replace(/http:\/\/cache\.soso\.com\/img\/img\/e1([^"]*)\.gif/g, function (e, t) {
                    return "https://res.mail.qq.com/zh_CN/images/mo/DEFAULT2/" + (t.charAt(0) == "0" ? t.substr(1, t.length) : t) + ".gif";
                });
            }
        },
        Log: {
            error: function (e, r) {
                r == t && (r = "0"), n.Helpers.ajax("/misc/jslog", {
                    level: "error",
                    id: r,
                    content: e
                });
            },
            debug: function (e, r) {
                r == t && (r = "0"), n.Helpers.ajax("/misc/jslog", {
                    level: "debug",
                    id: r,
                    content: e
                });
            }
        },
        Plugins: {},
        PageController: {},
        FormFactory: function () {
            var e = {}, t = function (t, n) {
                e[t] = n;
            }, n = function (t) {
                return e[t];
            };
            return {
                register: t,
                getFormHandler: n
            };
        } (),
        main: function () {
            var t = n.Helpers.tmpl;
            n.Event.notify("WXM:init"), n.Templates = {
                "#header": t("#tCommonHeader"),
                "#main": t("#tCommonMain"),
                "#footer": t("#tCommonFooter")
            }, n.Helpers.createPage(), e("#notifyIcon").length && W.ajax("/cgi-bin/sysnotify?lang=zh_CN&f=json&begin=0&count=5", {}, function (t) {
                t.BaseResp.Ret == 0 && t.Count && e("#notifyIcon").removeClass("none");
            }), n.Event.notify("WXM:init:finish");
        }
    };
    n.Event.register("WXM:init"), n.Event.register("WXM:init:finish");
} (jQuery), function (e, t) {
    if (!e || !e.Version) return;
    var n = {};
    e.Helpers.tmpl = function r(e, i) {
        if (!e) return "";
        var s = /\/\*TEMPLATES\*\//.test(e) ? new Function("obj", "var p=[],print=function(){p.push.apply(p,arguments);};with(obj){p.push('" + function (e) {
            e = e.replace(/[\r\t\n]/g, " ").split("<#WXM#").join("	");
            while (/((^|#WXM#>)[^\t]*)'/g.test(e)) e = e.replace(/((^|#WXM#>)[^\t]*)'/g, "$1\r");
            return e.replace(/\t=(.*?)#WXM#>/g, "',$1,'").split("	").join("');").split("#WXM#>").join("p.push('").split("\r").join("\\'");
        } (e) + "');}return p.join('');") : n[e] = n[e] || r(t(e).html());
        return i && typeof s == "function" ? s(i) : s;
    };
} (window.WXM, jQuery), function (e, t) {
    if (!e || !e.Version) return;
    var n, r, i = e.Helpers.tmpl, s = {
        success: "suc",
        failure: "err"
    }, o;
    e.Event.bind("WXM:init", function () {
        n = e.Helpers.createDomByHtml(i("#tTips", {})), r = t(".tipContent", n), o = r.attr("class"), n.appendTo("body");
    }), e.PageController.tips = {
        show: function (e, t, i) {
            i === undefined && (i = 2e3);
            if (e && e.length) {
                var u = t && t.type && s[t.type] || s.success;
                n.hide(), r.attr("class", o + " " + u).html(e), n.fadeIn(), setTimeout(function () {
                    n.fadeOut();
                }, i);
            }
        }
    };
} (window.WXM, jQuery), function (e, t) {
    if (!e || !e.Version) return;
    var n = e.Helpers.tmpl, r, i, s, o, u, a, f, l, c, h, p, d, v = t(window), m = e.PageController, g, y, b, w, E, S, x = m.dialog = {
        show: function (e) {
            x.flush();
            var e = e || {}, t;
            undefined !== e.title && u.html(e.title), t = {
                width: "auto"
            }, o.css(t), a.css(t), (t = e.content) || (t = r({})), a.html(t), undefined !== e.operation && f.html(e.operation), "function" == typeof (t = e.onok) ? (g = t, l.show()) : l.hide(), e.onenter && C(), "function" == typeof (t = e.oncancle) && (y = t), "function" == typeof (t = e.onStep) && (w = t), "function" == typeof (t = e.onBack) && (E = t), "function" == typeof (t = e.onTopClose) && (S = t), e.hasClose && h.show(), e.noCancle && c.hide(), e.hiddenBtnArea && f.hide(), e.initFinal && e.initFinal(), i.fadeIn(), b = 1, e.width ? o.width(e.width) : (a.width(a.children().outerWidth()), o.width(a.outerWidth())), e.height && a.height(e.height), N();
        },
        show_loading: function (e) {
            var e = e || {}, t;
            $loading_dialog_box.fadeIn();
        },
        hide: function () {
            i.fadeOut();
        },
        hide_loading: function () {
            $loading_dialog_box.fadeOut();
        },
        get$Inside: function (e) {
            return t(e, a);
        },
        flush: function () {
            h.css("display", "none"), p.css("display", "none"), d.css("display", "none"), a.attr("class", "content"), o.attr("style", "width:auto;"), a.attr("style", "width:auto;");
        }
    }, T, N = function () {
        if (b) {
            var e = (v.height() - o.height()) / 2;
            e = e < 0 ? 0 : e, o.css({
                top: e
            });
        }
    };
    v.bind({
        resize: function () {
            if (!b) return;
            clearTimeout(T), T = setTimeout(N, 50);
        }
    }), e.Event.bind("WXM:init", function () {
        r = n("#tLoading"), i = e.Helpers.createDomByHtml(n("#tDialog", {})).appendTo("body"), $loading_dialog_box = e.Helpers.createDomByHtml(n("#tLoadingDialog", {})).appendTo("body"), s = t(".background", i), o = t(".dialog", i), u = t(".title", o), a = t(".content", o), f = t(".operation", o), l = t("#dialogOK", f), c = t("#dialogCancle", f), h = t("#dialogClose", f), p = t("#dialogStep", f), d = t("#dialogBack", f), $dialog_topCloseBtn = t("#dialogTopClose", o), l.click(function () {
            (!g || !g()) && x.hide();
        }), c.click(function () {
            y && y(a), x.hide();
        }), h.click(function () {
            x.hide();
        }), p.click(function () {
            w();
        }), d.click(function () {
            E();
        }), $dialog_topCloseBtn.click(function () {
            S && S(), x.hide();
        });
    });
    var C = function () {
        $dialog_input = t("#dialogContent input[type=text]"), $dialog_input = t($dialog_input[$dialog_input.length - 1]), $dialog_input.keypress(function (e) {
            if (e.keyCode == 13) return l.click(), !1;
        });
    };
} (window.WXM, jQuery), function (e, t) {
    var n = e.Helpers.tmpl, r = e.Helpers.extendPageMsg, i = e.PageController, s = i.dialog, o = e.ROOT.cgi + "filemanagepage?JSON=1&t=wxm_fileselect&type=", u, a = function (e) {
        var e = e || {}, i = e.type, f = "function" == typeof e.onselect && e.onselect, l = "function" == typeof e.oninit && e.oninit, c = e.url;
        s.show({
            title: "选择素材",
            width: 300,
            oncancle: function () {
                u && u.abort();
            }
        }), u = t.ajax({
            url: c || o + i,
            cache: !1,
            dataType: "text",
            scriptCharset: "utf8",
            success: function (e, c, h) {
                if (!e) return;
                var e = t.parseJSON(e);
                r(o + i, e.PageMsg), u = null, s.show({
                    title: "选择素材",
                    content: n("#tmpl-common-mediaselect", e),
                    onok: function () {
                        if ("function" == typeof f) {
                            var e = s.get$Inside("input:checked").closest(".column");
                            f({
                                Id: e.data("id") * 1,
                                FileName: e.data("filename"),
                                Type: e.data("type") * 1,
                                Size: e.data("size"),
                                UpdateTime: e.data("updatetime") * 1,
                                ExtLength: e.data("extlength"),
                                Source: e.data("source")
                            });
                        }
                    }
                }), s.get$Inside(".pageNavigator a").click(function (e) {
                    return a({
                        url: t(e.target).attr("href"),
                        type: i,
                        onselect: f
                    }), !1;
                }), l && l();
            }
        });
    };
    i.mediaSelector = function (e, t, n) {
        a({
            type: e,
            onselect: t,
            oninit: n
        });
    };
} (WXM, jQuery), function (e, t) {
    var n = function (e) {
        return e.charCodeAt();
    }, r = function (e) {
        return e.toString(16).length / 2;
    }, i = e.Helpers.countStrByte = function (e) {
        var t = 0, i, s = 0;
        while (i = e.charAt(t++)) s += r(n(i));
        return s;
    };
} (WXM), function (e, t, n) {
    window.W == n && (window.W = {}), e.Helpers.ajax = W.ajax = function (n, r, i, s, o, u, a) {
        a = a == 0 ? a : !0, u = u || "json", r = r || {}, e.DATA.token && e.DATA.token.length && (r.token = e.DATA.token), r.ajax = 1, t.ajax({
            type: "POST",
            url: n,
            async: a,
            data: r,
            scriptCharset: "utf8",
            success: function (n) {
                typeof n == "string" && (n = t.parseJSON(n));
                if (n && n.ret == "-20000") {
                    e.Event.notify("WXM:timeout"), W.err("登录超时，请刷新页面重新登录！");
                    return;
                }
                i && i(n, r);
            },
            error: function (e) {
                s && s(e);
            },
            complete: o || function () { }
        });
    }, W.t = e.Helpers.tmpl, W.d = e.PageController.dialog, W.suc = function (t) {
        e.PageController.tips.show(t, {
            type: "success"
        });
    }, W.err = function (t, n) {
        e.PageController.tips.show(t, {
            type: "failure"
        }, n);
    };
} (WXM, jQuery), function (e, t, n) {
    e.valid = {
        isURL: function (e) {
            var t = new RegExp("[a-zA-Z0-9\\.\\-]+\\.([a-zA-Z]{2,4})(:\\d+)?(/[a-zA-Z0-9\\.\\-~!@#$%^&amp;*+?:_/=<>]*)?", "gi");
            return t.test(e) ? !0 : !1;
        },
        isMail: function (e) {
            var t = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
            return t.test(e);
        },
        isPostCode: function (e) {
            return /^[0-9]{6,6}$/.test(e);
        },
        isLicenseNumber: function (e) {
            return /^[\s\S]{1,100}$/.test(e);
        },
        isPassport: function (e) {
            var t = new RegExp(/^[0-9A-Z]{8,30}$/);
            return t.test(e) ? !0 : !1;
        },
        isMobile: function (e) {
            var t = /^1\d{10}$/;
            return t.test(e) ? !0 : !1;
        },
        isGlobalMobile: function (e) {
            return /^\d{6,15}$/.test(e);
        },
        isPersonName: function (e) {
            return /^[\s\w\u4e00-\u9fa5|\u00b7]{1,20}$/g.test(e);
        },
        isCompanyName: function (e) {
            return /^[\s\S]{1,200}$/.test(e);
        },
        isOrgCode: function (e) {
            return /^[\d|A-Z]{8,8}-[\d|A-Z]{1,1}$/.test(e);
        },
        isIdCard: function (e) {
            function n(e) {
                var t = 0;
                e[17].toLowerCase() == "x" && (e[17] = 10);
                for (var n = 0; n < 17; n++) t += o[n] * e[n];
                return valCodePosition = t % 11, e[17] == u[valCodePosition] ? !0 : !1;
            }
            function r(e) {
                var t = e.substring(6, 10), n = e.substring(10, 12), r = e.substring(12, 14), i = new Date(t, parseFloat(n) - 1, parseFloat(r));
                return (new Date).getFullYear() - parseInt(t) < 18 ? !1 : i.getFullYear() != parseFloat(t) || i.getMonth() != parseFloat(n) - 1 || i.getDate() != parseFloat(r) ? !1 : !0;
            }
            function i(e) {
                var t = e.substring(6, 8), n = e.substring(8, 10), r = e.substring(10, 12), i = new Date(t, parseFloat(n) - 1, parseFloat(r));
                return i.getYear() != parseFloat(t) || i.getMonth() != parseFloat(n) - 1 || i.getDate() != parseFloat(r) ? !1 : !0;
            }
            function s(e) {
                e = t.trim(e.replace(/ /g, ""));
                if (e.length == 15) return !1;
                if (e.length == 18) {
                    var i = e.split("");
                    return r(e) && n(i) ? !0 : !1;
                }
                return !1;
            }
            var o = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1], u = [1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2];
            return s(e);
        }
    }, e.formatDate = function (e, t) {
        var n = e.getFullYear(), r = "0" + (e.getMonth() + 1);
        r = r.substring(r.length - 2);
        var i = "0" + e.getDate();
        i = i.substring(i.length - 2);
        var s = "0" + e.getHours();
        s = s.substring(s.length - 2);
        var o = "0" + e.getMinutes();
        return o = o.substring(o.length - 2), t.replace("yyyy", n).replace("MM", r).replace("dd", i).replace("hh", s).replace("mm", o);
    }, e.Plugins = e.Plugins || {}, e.Plugins.pageJump = function (e, n, r) {
        var i = t(e), s = i.data("baseurl"), o = i.parent().find(".pageIdxInput"), u = t.trim(o.val()) * 1 - 1;
        u = u > 0 ? u : 0, u = r ? r <= u ? r - 1 : u : 0;
        if (!n || n.type == "click") window.location = s + "&pageidx=" + u; else {
            var a = 0;
            window.event ? a = window.event.keyCode : n.which && (a = n.which), a == 13 && (window.location = s + "&pageidx=" + u);
        }
    };
} (WXM, jQuery), WXM.Event.bind("WXM:init:finish", function () {
    function e(e, n) {
        e.focus(function () {
            this.value === n && (this.value = ""), t(this).removeClass("placeholder");
        }).blur(function () {
            this.value === "" && (this.value = n, t(this).addClass("placeholder"));
        }), e.val() === "" && e.addClass("placeholder"), e.val() || e.val(n);
    }
    if (!("placeholder" in document.createElement("input"))) {
        var t = jQuery;
        t.each(['input[type="text"]', "textarea[placeholder]"], function (n, r) {
            var i = t(r);
            for (var s = 0; s < i.length; ++s) {
                var o = i.eq(s), u = o.attr("placeholder");
                u && e(o, u);
            }
        });
    }
});