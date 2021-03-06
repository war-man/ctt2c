﻿
var nameAlias = {
    cdp: "Cấp địa phương",
    vptc: "Văn phòng tổng cục",
    cbtddn: "Cán bộ trực đường dây nóng",
    btnmt: "Bộ tài nguyên môi trường",
    ldtcmt: "Lãnh đạo tổng cục môi trường",
    bnk: "Bộ ngành khác"
}
$(document).ready(function () {
    var jwtToken = getCookie("ACCESS-TOKEN");
    if (GroupId !== 'sysadmin') {
        $(".sysadmin").hide();
    } else {
        $(".sysadmin").show();
        $("#HomeNews").show();
    }

    if (GroupId === 'ttdl') {
        $("#btn-phan-cong").hide();
        $("#btn-bao-cao").hide();
        $(".tonghopbaocao").show();
        $(".ttdl").show();
    }

    if (GroupId === 'ldtcmt') {
        $(".newdata").hide();
        $("#btn-phan-cong").show();
        $("#btn-xem-bao-cao").show();
        $(".tonghopbaocao").show();
    }

    if (GroupId === 'dvct') {
        $("#HomeNews").hide();
        $("#AnhVideo").hide();
        $("#Vanban").hide();
        $('.sysconfig').hide();
        $(".ttdl").hide();
    }

    if (GroupId === 'cbtddn') {
        $("#HomeNews").show();
        $("#AnhVideo").show();
        $("#Vanban").show();
        $('.sysconfig').hide();
        $("#News").hide();
       // $(".ttdl").hide();
    }

    function templateDate(date) {
        if (date) {
            return new Date(date).toLocaleString()
        } else {
            return "";
        }

    }

    $('#btn-phan-cong-new').on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if (ngNews.lstNewsId.length > 0) {
            for (var i = 0; i < currentData.length; i++) {
                for (var j = 0; j < ngNews.lstNewsId.length; j++) {
                    var id = ngNews.lstNewsId[j];
                    if (id === currentData[i].id) {
                        if (currentData[i].is_status !== 1) {
                            messagerWarn('Thông báo', 'Vui lòng chọn tin đã duyệt.');
                            return;
                        }
                    }
                }
            }
            $("#exampleModalNew").modal('show');
        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    var url2 = `${appConfig.apiHostUrl}/${NEWS_API.GET_NOTIFI}`;
    url2 = url2 + "?username=" + username;
    callAjax(
        url2,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            $("#countthongbao").html(success.length);
            success.forEach(function (item) {
                var html = '<a href="#" class="dropdown-item"><i class="fa fa-envelope mr-2" ></i> ' + item.Notification + ' <span class="float-right text-muted text-sm" >' + templateDate(item.OnCreated)+' </span></a>';
                html += '<div class="dropdown-divider"></div>';
                html = $("<div />").html(html).html();
                $("#thongbaodata").append(html);
            });         
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )

    $('#btn-bao-cao-new').on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if (ngNews.lstNewsId.length > 0) {
            var url = `${appConfig.apiHostUrl}/${NEWS_API.GET_REPORT}`;
            url = url + "?id=" + parseInt(ngNews.lstNewsId[0]);
            callAjax(
                url,
                null,
                AjaxConst.GetRequest,
                function (xhr) {
                    $(this).addClass('disabled').attr('disabled', true);
                    xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                },
                function (success) {
                    try {
                        $("#commentNews").val(success.data);
                        $("#IdReport").val(success.Id);
                    } catch (e) {
                        $("#IdReport").val("0");
                    }
                    $("#exampleModalNew2").modal('show');
                },
                function (xhr, status, error) {

                    if (xhr.status === 400) {
                        var err = eval("(" + xhr.responseText + ")");
                        err.forEach(function (item) {
                            messagerError(item.Code, item.Description);
                        });
                    } else {
                        messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                    }
                },
                function (complete) {
                    $(this).removeClass('disabled').removeAttr('disabled');
                }
            )
           
        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    $('#clickPhanCong').on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if ($("#phancongcho").val() !== "") {
            var data = {
                username: $("#phancongcho").val(),
                ids: $("#exampleModalNew .NewsId").val(),
                note: $("#exampleModalNew .ghichubaocao").val(),
                type: 3
            }

            kendo.confirm("Xác nhận chuyển?")
                .done(function () {
                    callAjax(
                        `${appConfig.apiHostUrl}/${NEWS_API.SAVE_PUBLISH_NEW_USERNAME}`,
                        data,
                        AjaxConst.PostRequest,
                        function (xhr) {
                            $(this).addClass('disabled').attr('disabled', true);
                            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                        },
                        function (success) {
                            if (!success.did_error) {
                                messagerSuccess('Thông báo', success.model);
                            }
                            if (grid) {
                                grid.clearSelection();
                                grid.dataSource.read();
                            }
                        },
                        function (xhr, status, error) {
                            if (xhr.status === 400) {
                                var err = eval("(" + xhr.responseText + ")");
                                err.forEach(function (item) {
                                    messagerError(item.Code, item.Description);
                                });
                            } else {
                                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                            }
                        },
                        function (complete) {
                            $(this).removeClass('disabled').removeAttr('disabled');
                        }
                    )
                })
        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    $('#clickChuyenConvan').on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if ($(".js-data-example-ajax-chuyencongvan").val().join(",") !== "") {
            var data = {
                username: $(".js-data-example-ajax-chuyencongvan").val().join(","),
                ids: $("#exampleModalNew6 .NewsId").val(),
                note: $("#exampleModalNew6 .ghichubaocao").val(),
                type: 4
            }

            kendo.confirm("Xác nhận chuyển công văn?")
                .done(function () {
                    callAjax(
                        `${appConfig.apiHostUrl}/${NEWS_API.SAVE_PUBLISH_NEW_USERNAME}`,
                        data,
                        AjaxConst.PostRequest,
                        function (xhr) {
                            $(this).addClass('disabled').attr('disabled', true);
                            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                        },
                        function (success) {
                            if (!success.did_error) {
                                messagerSuccess('Thông báo', success.model);
                            }
                            if (grid) {
                                grid.clearSelection();
                                grid.dataSource.read();
                            }
                        },
                        function (xhr, status, error) {
                            if (xhr.status === 400) {
                                var err = eval("(" + xhr.responseText + ")");
                                err.forEach(function (item) {
                                    messagerError(item.Code, item.Description);
                                });
                            } else {
                                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                            }
                        },
                        function (complete) {
                            $(this).removeClass('disabled').removeAttr('disabled');
                        }
                    )
                })
        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    $("#clickBaocao").on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if (ngNews.lstNewsId.length > 0) {
            var idReport = $("#IdReport").val();
            var data = {
                username: "",
                ids: ngNews.lstNewsId,
                Id: parseInt(idReport),
                NewsId: parseInt(ngNews.lstNewsId[0]),
                data: $("#commentNews").val(),
                UserName: username
            }
            
            kendo.confirm("Xác nhận báo cáo ?")
                .done(function () {
                    callAjax(
                        `${appConfig.apiHostUrl}/${NEWS_API.SAVE_PUBLISH_NEW_REPORT}`,
                        data,
                        AjaxConst.PostRequest,
                        function (xhr) {
                            $(this).addClass('disabled').attr('disabled', true);
                            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                        },
                        function (success) {
                            if (!success.did_error) {
                                messagerSuccess('Thông báo', success.model);
                            }
                            if (grid) {
                                grid.clearSelection();
                                grid.dataSource.read();
                            }
                        },
                        function (xhr, status, error) {
                            
                            if (xhr.status === 400) {
                                var err = eval("(" + xhr.responseText + ")");
                                err.forEach(function (item) {
                                    messagerError(item.Code, item.Description);
                                });
                            } else {
                                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                            }
                        },
                        function (complete) {
                            $(this).removeClass('disabled').removeAttr('disabled');
                        }
                    )
                })
        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    $('#btn-xem-bao-cao-new').on('click', function (e) {
        var grid = $('#dataGrid').data('kendoGrid');
        if (ngNews.lstNewsId.length > 0) {
            var url = `${appConfig.apiHostUrl}/${NEWS_API.GET_REPORT}`;
            url = url + "?id=" + parseInt(ngNews.lstNewsId[0]);
            callAjax(
                url,
                null,
                AjaxConst.GetRequest,
                function (xhr) {
                    $(this).addClass('disabled').attr('disabled', true);
                    xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                },
                function (success) {
                    try {
                        $("#commentNews3").val(success.data);
                        $("#IdReport3").val(success.Id);
                        var name = nameAlias[success.UserName];
                        $("#nameBaocao").text(name);
                        $("#exampleModalNew3").modal('show');
                    } catch (e) {
                        messagerError("Alert", "Thông tin chưa có báo cáo!");
                    }
                },
                function (xhr, status, error) {

                    if (xhr.status === 400) {
                        var err = eval("(" + xhr.responseText + ")");
                        err.forEach(function (item) {
                            messagerError(item.Code, item.Description);
                        });
                    } else {
                        messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                    }
                },
                function (complete) {
                    $(this).removeClass('disabled').removeAttr('disabled');
                }
            )

        } else {
            messagerWarn('Thông báo', 'Vui lòng chọn tin.');
        }
    });

    ////select 2
    //var urlselect2 = `${appConfig.apiHostUrl}` + '/api/Users/getUsersForType?type=dvct';
    //$('.js-data-example-ajax').select2({
    //    minimumResultsForSearch: -1,
    //    width: '100%',
    //    ajax: {
    //        url: urlselect2,
    //        dataType: 'json',
    //        beforeSend: function (xhr) {
    //            $(this).addClass('disabled').attr('disabled', true);
    //            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
    //        },
    //        processResults: function (data, params) {
    //            var dataReturn = [];
    //            for (var i = 0; i < data.length; i++) {
    //                var id = data[i].UserName;
    //                var text = data[i].FullName;
    //                var obj = {
    //                    id: id,
    //                    text: text
    //                }
    //                dataReturn.push(obj);
    //            }
                
    //            return {
    //                results: dataReturn
    //            };
    //        },
    //        // Additional AJAX parameters go here; see the end of this chapter for the full code of this example
    //    }
    //});

    var urlselect3 = `${appConfig.apiHostUrl}` + '/api/Users/getUsersForType?type=dvct_dp';
    $('.js-data-example-ajax-chuyencongvan').select2({
        minimumResultsForSearch: -1,
        width: '100%',
        ajax: {
            url: urlselect3,
            dataType: 'json',
            beforeSend: function (xhr) {
                $(this).addClass('disabled').attr('disabled', true);
                xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
            },
            processResults: function (data, params) {
                var dataReturn = [];
                for (var i = 0; i < data.length; i++) {
                    var id = data[i].UserName;
                    var text = data[i].FullName;
                    var obj = {
                        id: id,
                        text: text
                    }
                    dataReturn.push(obj);
                }

                return {
                    results: dataReturn
                };
            },
            // Additional AJAX parameters go here; see the end of this chapter for the full code of this example
        }
    });
});

function createSelect2(dataSeelcted) {
    var urlselect3 = `${appConfig.apiHostUrl}` + '/api/Users/getUsersForType?type=dvct';
    callAjax(
        urlselect3,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (data) {
            if (data) {
                var dataReturn = [];
                for (var i = 0; i < data.length; i++) {
                    var id = data[i].UserName;
                    var text = data[i].FullName;
                    var obj = {
                        id: id,
                        text: text
                    }
                    dataReturn.push(obj);
                }

                $('.js-data-example-ajax').select2({
                    minimumResultsForSearch: -1,
                    width: '100%',
                    data: dataReturn
                });

                var data2 = [];
                for (var j = 0; j < dataSeelcted.length; j++) {
                    data2.push(dataSeelcted[j].UserName);
                }

                $('.js-data-example-ajax').val(data2).trigger('change');
            } else {
                messagerError("Error", "Lỗi hệ thống xin thử lại!");
            }
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )
}

function phancong(news_id) {
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetInfoNewLog?news_id=' + news_id + '&group=ttdl',
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (data) {
            if (data) {
                var success = data.info;
                createSelect2(data.phancong);
                $("#exampleModalNew .ghichubaocao").val("");
                $("#exampleModalNew .NewsId").val(news_id);
                $("#exampleModalNew .NewsLogId").val(success.Id);
                $("#exampleModalNew .ghichubaocao").val(success.Note);
                $("#exampleModalNew").modal('show');

                $('#fileupload3').addClass('fileupload-processing');
                $("#filebaocao3 tbody").empty();
                $.ajax({
                    // Uncomment the following to send cross-domain cookies:
                    //xhrFields: {withCredentials: true},
                    url: `${appConfig.apiHostUrl}` + '/api/NewsLog/upload/' + $("#exampleModalNew .NewsLogId").val(),
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + getCookie("ACCESS-TOKEN"));
                        xhr.setRequestHeader('IdReprot', parseInt($("#exampleModalNew .NewsLogId").val()));
                    },
                    dataType: 'json',
                    context: $('#fileupload3')[0]
                }).always(function () {
                    $(this).removeClass('fileupload-processing');
                }).done(function (result) {
                    $(this).fileupload('option', 'done')
                        .call(this, $.Event('done'), { result: result });
                });

            } else {
                messagerError("Error", "Lỗi hệ thống xin thử lại!");
            }
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )
}

function chuyencongvan(news_id) {
    $("#exampleModalNew6 .NewsId").val(news_id);
    $("#exampleModalNew6").modal('show');
}

function nhapketquaxuly() {
    var grid = $('#dataGrid').data('kendoGrid');
    if ($("#exampleModalNew_nhapketqua .IdReport").val() !== "") {
        var idReport = $("#exampleModalNew_nhapketqua .IdReport").val();
        var data = {
            Id: parseInt(idReport),
            Data: editor2.getData(),
            typeStatus: 5
        }

        kendo.confirm("Xác nhận báo cáo ?")
            .done(function () {
                callAjax(
                    `${appConfig.apiHostUrl}` + '/api/NewsLog/PostReport',
                    data,
                    AjaxConst.PostRequest,
                    function (xhr) {
                        $(this).addClass('disabled').attr('disabled', true);
                        xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                    },
                    function (success) {
                        
                        if (!success.did_error) {
                            messagerSuccess('Thông báo', 'Gửi báo cáo thành công!');
                        }
                        if (grid) {
                            grid.clearSelection();
                            grid.dataSource.read();
                        }
                    },
                    function (xhr, status, error) {
                        
                        if (xhr.status === 400) {
                            var err = eval("(" + xhr.responseText + ")");
                            err.forEach(function (item) {
                                messagerError(item.Code, item.Description);
                            });
                        } else {
                            messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                        }
                    },
                    function (complete) {
                        $(this).removeClass('disabled').removeAttr('disabled');
                    }
                )
            })
    } else {
        messagerWarn('Thông báo', 'Vui lòng chọn tin.');
    }
}

function submitTraLai() {
    var grid = $('#dataGrid').data('kendoGrid');
    if ($("#exampleModalNew_nhapketquatralai .IdReport").val() !== "") {
        var idReport = $("#exampleModalNew_nhapketquatralai .IdReport").val();
        var data = {
            Id: parseInt(idReport),
            Data: editorTralai.getData(),
            typeStatus: 7
        };

        kendo.confirm("Xác nhận chuyển trả lại ?")
            .done(function () {
                callAjax(
                    `${appConfig.apiHostUrl}` + '/api/NewsLog/PostReport',
                    data,
                    AjaxConst.PostRequest,
                    function (xhr) {
                        $(this).addClass('disabled').attr('disabled', true);
                        xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                    },
                    function (success) {

                        if (!success.did_error) {
                            messagerSuccess('Thông báo', 'Chuyển trả lại thành công!');
                        }
                        if (grid) {
                            grid.clearSelection();
                            grid.dataSource.read();
                        }
                        $("#exampleModalNew_nhapketquatralai").modal("hide");
                    },
                    function (xhr, status, error) {

                        if (xhr.status === 400) {
                            var err = eval("(" + xhr.responseText + ")");
                            err.forEach(function (item) {
                                messagerError(item.Code, item.Description);
                            });
                        } else {
                            messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                        }
                    },
                    function (complete) {
                        $(this).removeClass('disabled').removeAttr('disabled');
                    }
                )
            })
    } else {
        messagerWarn('Thông báo', 'Vui lòng chọn tin.');
    }
}

function nhapketcongkhai() {
    var grid = $('#dataGrid').data('kendoGrid');
    if ($("#exampleModalNew_congkhai .IdReport").val() !== "") {
        var idReport = $("#exampleModalNew_congkhai .IdReport").val();
        var data = {
            Id: parseInt(idReport),
            Data: editorCongkhai.getData()
        };

        kendo.confirm("Xác nhận trả lời ?")
            .done(function () {
                callAjax(
                    `${appConfig.apiHostUrl}` + '/api/NewsLog/PostReport',
                    data,
                    AjaxConst.PostRequest,
                    function (xhr) {
                        $(this).addClass('disabled').attr('disabled', true);
                        xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                    },
                    function (success) {
                        var ids = [];
                        ids.push($("#exampleModalNew_congkhai .NewsId").val());
                        if (!success.did_error) {
                            messagerSuccess('Thông báo', 'Trả lời góp ý thành công!');
                            //
                            callAjax(
                                `${appConfig.apiHostUrl}/${NEWS_API.SAVE_PROCESS_NEW}`,
                                ids,
                                AjaxConst.PostRequest,
                                function (xhr) {
                                    $(this).addClass('disabled').attr('disabled', true);
                                    xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                                },
                                function (success) {
                                    if (grid) {
                                        grid.clearSelection();
                                        grid.dataSource.read();
                                    }
                                },
                                function (xhr, status, error) {
                                    if (xhr.status === 400) {
                                        var err = eval("(" + xhr.responseText + ")");
                                        err.forEach(function (item) {
                                            messagerError(item.Code, item.Description);
                                        });
                                    } else {
                                        messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                                    }
                                },
                                function (complete) {
                                    $(this).removeClass('disabled').removeAttr('disabled');
                                }
                            );
                        }
                    },
                    function (xhr, status, error) {

                        if (xhr.status === 400) {
                            var err = eval("(" + xhr.responseText + ")");
                            err.forEach(function (item) {
                                messagerError(item.Code, item.Description);
                            });
                        } else {
                            messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                        }
                    },
                    function (complete) {
                        $(this).removeClass('disabled').removeAttr('disabled');
                    }
                )
            })
    } else {
        messagerWarn('Thông báo', 'Vui lòng chọn tin.');
    }
}

function nhapketquagopy() {
    var grid = $('#dataGrid').data('kendoGrid');
    if ($("#exampleModalNew_gopychidao .IdReport").val() !== "") {
        var idReport = $("#exampleModalNew_gopychidao .IdReport").val();
        var data = {
            Id: parseInt(idReport),
            Data: editorGopy.getData()
        }

        kendo.confirm("Xác nhận gửi ?")
            .done(function () {
                callAjax(
                    `${appConfig.apiHostUrl}` + '/api/NewsLog/PostReport',
                    data,
                    AjaxConst.PostRequest,
                    function (xhr) {
                        $(this).addClass('disabled').attr('disabled', true);
                        xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                    },
                    function (success) {

                        if (!success.did_error) {
                            messagerSuccess('Thông báo', 'Nhập kết quả thành công!');
                        }
                        if (grid) {
                            grid.clearSelection();
                            grid.dataSource.read();
                        }
                    },
                    function (xhr, status, error) {

                        if (xhr.status === 400) {
                            var err = eval("(" + xhr.responseText + ")");
                            err.forEach(function (item) {
                                messagerError(item.Code, item.Description);
                            });
                        } else {
                            messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                        }
                    },
                    function (complete) {
                        $(this).removeClass('disabled').removeAttr('disabled');
                    }
                )
            })
    } else {
        messagerWarn('Thông báo', 'Vui lòng chọn tin.');
    }
}

function nhapketquatralai(news_id) {
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetNewsLogByNewsIdNameFrom?NewsId=' + news_id + "&UserName=" + username,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            try {

                $("#exampleModalNew_nhapketquatralai .commentNews").val(success[0].Data);
                if (success[0].Data) {
                    editorTralai.setData(success[0].Data);
                }

                $("#exampleModalNew_nhapketquatralai .IdReport").val(success[0].Id);
            } catch (e) {
                $("#IdReport").val("0");
            }
            $("#exampleModalNew_nhapketquatralai").modal('show');
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    );

    var url = `${appConfig.apiHostUrl}` + '/api/News/xemchitiet?Id=' + news_id;
    callAjax(
        url,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            if (!success.did_error) {
                var info = success.info;
                var element = $("#thongtinbody_nhapketquatralai");
                element.find(".tieude").html(info.Name);
                element.find(".UserFullName").html(info.UserFullName);
                element.find(".UserEmail").html(info.UserEmail);

                element.find(".noidung").val(info.Content);

                if (info.fileUpload) {
                    var fileuploadSbt = info.fileUpload.split(",");
                    var htmlAno = "";
                    if (fileuploadSbt.length > 0) {
                        for (var k = 0; k < fileuploadSbt.length; k++) {
                            var fileItemAno = fileuploadSbt[k];
                            htmlAno += '<a target="_blank" href="' + fileItemAno + '">' + fileItemAno + '</a></br>';
                        }
                        $("#thongtinbody_nhapketquatralai .item-from-file").html(htmlAno);
                    }
                }
            }  
        }
    );
}

function nhapketqua(news_id) {
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetNewsLogByNewsIdNameFrom?NewsId=' + news_id + "&UserName=" + username,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            try {
                $("#exampleModalNew_nhapketqua .commentNews").val(success[0].Data);
                if (success[0].Data) {
                    editor2.setData(success[0].Data);
                }
                
                $("#exampleModalNew_nhapketqua .IdReport").val(success[0].Id);
            } catch (e) {
                $("#IdReport").val("0");
            }
            $("#exampleModalNew_nhapketqua").modal('show');
            $('#fileupload').addClass('fileupload-processing');
            $("#filebaocao tbody").empty();
            $.ajax({
                // Uncomment the following to send cross-domain cookies:
                //xhrFields: {withCredentials: true},
                url: `${appConfig.apiHostUrl}` + '/api/NewsLog/upload/' + $("#exampleModalNew_nhapketqua .IdReport").val(),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Authorization', 'Bearer ' + getCookie("ACCESS-TOKEN"));
                    xhr.setRequestHeader('IdReprot', parseInt($("#exampleModalNew_nhapketqua .IdReport").val()));
                },
                dataType: 'json',
                context: $('#fileupload')[0]
            }).always(function () {
                $(this).removeClass('fileupload-processing');
            }).done(function (result) {
                $(this).fileupload('option', 'done')
                    .call(this, $.Event('done'), { result: result });
            });
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )

    var url = `${appConfig.apiHostUrl}` + '/api/News/xemchitiet?Id=' + news_id;
    callAjax(
        url,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            if (!success.did_error) {
                var info = success.info;
                var element = $("#thongtinbody_nhapketqua");
                element.find(".tieude").html(info.Name);
                element.find(".UserFullName").html(info.UserFullName);
                element.find(".UserEmail").html(info.UserEmail);

                element.find(".noidung").val(info.Content);

                if (info.fileUpload) {
                    var fileuploadSbt = info.fileUpload.split(",");
                    var htmlAno = "";
                    if (fileuploadSbt.length > 0) {
                        for (var k = 0; k < fileuploadSbt.length; k++) {
                            var fileItemAno = fileuploadSbt[k];
                            htmlAno += '<a target="_blank" href="' + fileItemAno + '">' + fileItemAno + '</a></br>';
                        }
                        $("#thongtinbody_nhapketqua .item-from-file").html(htmlAno);
                    }
                }
            }
        }
    );
}

function gopychidao(news_id) {
    editorGopy.setData("");
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetNewsLogByNewsIdNameFrom?NewsId=' + news_id + "&UserName=" + username,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            try {
                $("#exampleModalNew_gopychidao .commentNews").val(success[0].Data);
                if (success[0].Data) {
                    editorGopy.setData(success[0].Data);
                }

                $("#exampleModalNew_gopychidao .IdReport").val(success[0].Id);
            } catch (e) {
                $("#IdReport").val("0");
            }
            $("#exampleModalNew_gopychidao").modal('show');
            $('#fileupload4').addClass('fileupload-processing');
            $("#filebaocao tbody").empty();
            $.ajax({
                // Uncomment the following to send cross-domain cookies:
                //xhrFields: {withCredentials: true},
                url: `${appConfig.apiHostUrl}` + '/api/NewsLog/upload/' + $("#exampleModalNew_gopychidao .IdReport").val(),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Authorization', 'Bearer ' + getCookie("ACCESS-TOKEN"));
                    xhr.setRequestHeader('IdReprot', parseInt($("#exampleModalNew_gopychidao .IdReport").val()));
                },
                dataType: 'json',
                context: $('#fileupload4')[0]
            }).always(function () {
                $(this).removeClass('fileupload-processing');
            }).done(function (result) {
                $(this).fileupload('option', 'done')
                    .call(this, $.Event('done'), { result: result });
            });
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )
}

function xemchitiet(news_id) {
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetReport?NewsId=' + news_id,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            $("#thongtinbody").html("");
            success.forEach(function (value) {
                var html = "";
                if (!value.Data) {
                    value.Data = "";
                }
                html = '<label for="recipient-name" class="col-form-label">' + value.FullUserName + '</label><p>' + value.Data + '</p>';
                for (var i = 0; i < value.files.length; i++) {
                    html += '<a href="' + value.files[i].url +'" target="_blank">' + value.files[i].name + '</a><br>';
                }
                    
                $("#thongtinbody").append(html);

            });

            $("#exampleModalNew_xemchitiet").modal("show");
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )
}


function congkhai2(news_id) {
    var grid = $('#dataGrid').data('kendoGrid');
    var ids = [];
    ids.push(news_id);
    if (news_id > 0) {
        kendo.confirm("Xác nhận đăng tin này?")
            .done(function () {
                callAjax(
                    `${appConfig.apiHostUrl}/${NEWS_API.SAVE_PROCESS_NEW}`,
                    ids,
                    AjaxConst.PostRequest,
                    function (xhr) {
                        $(this).addClass('disabled').attr('disabled', true);
                        xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
                    },
                    function (success) {
                        if (!success.did_error) {
                            messagerSuccess('Thông báo', success.model);
                        }
                        if (grid) {
                            grid.clearSelection();
                            grid.dataSource.read();
                        }
                    },
                    function (xhr, status, error) {
                        if (xhr.status === 400) {
                            var err = eval("(" + xhr.responseText + ")");
                            err.forEach(function (item) {
                                messagerError(item.Code, item.Description);
                            });
                        } else {
                            messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
                        }
                    },
                    function (complete) {
                        $(this).removeClass('disabled').removeAttr('disabled');
                    }
                )
            })
    } else {
        messagerWarn('Thông báo', 'Vui lòng chọn tin.');
    }
}

function congkhai(news_id) {
    editorCongkhai.setData("");
    $("#exampleModalNew_congkhai .IdReport").val("0");
    $("#exampleModalNew_congkhai .NewsId").val(news_id);
    callAjax(
        `${appConfig.apiHostUrl}` + '/api/NewsLog/GetNewsLogByNewsIdNameFrom?NewsId=' + news_id + "&UserName=is_ano",
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            try {
                $("#exampleModalNew_congkhai .commentNews").val(success[0].Data);
                if (success[0].Data) {
                    editorCongkhai.setData(success[0].Data);
                }

                $("#exampleModalNew_congkhai .IdReport").val(success[0].Id);
            } catch (e) {
                $("#IdReport").val("0");
            }
            $("#exampleModalNew_congkhai").modal('show');
            $('#fileupload5').addClass('fileupload-processing');
            $("#filecongkhai tbody").empty();
            $.ajax({
                // Uncomment the following to send cross-domain cookies:
                //xhrFields: {withCredentials: true},
                url: `${appConfig.apiHostUrl}` + '/api/NewsLog/upload/' + $("#exampleModalNew_congkhai .IdReport").val(),
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('Authorization', 'Bearer ' + getCookie("ACCESS-TOKEN"));
                    xhr.setRequestHeader('IdReprot', parseInt($("#exampleModalNew_congkhai .IdReport").val()));
                },
                dataType: 'json',
                context: $('#fileupload5')[0]
            }).always(function () {
                $(this).removeClass('fileupload-processing');
            }).done(function (result) {
                $(this).fileupload('option', 'done')
                    .call(this, $.Event('done'), { result: result });
            });
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    );

    var element = $("#exampleModalNew_congkhai");
    var editor = element.find(".noidung").data("kendoEditor");
    var editor2 = element.find(".noidung-ttdl").data("kendoEditor");
    var editor3 = element.find(".noidung-ldtcmt").data("kendoEditor");
    $(".item-from-file-ttdl").html("");
    element.find(".item-from-file").html("");
    element.find(".tieude").val("");
    editor.value("");
    editor.value("");

    var url = `${appConfig.apiHostUrl}` + '/api/News/xemchitiet?Id=' + news_id;
    callAjax(
        url,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            if (!success.did_error) {
                var info = success.info;
                var ldtcmt = success.ldtcmt;
                var ttdl = success.ttdl;

                element.find(".tieude").html(info.Name);
                element.find(".UserFullName").html(info.UserFullName);
                element.find(".UserEmail").html(info.UserEmail);

                editor.value(info.Content);

                if (info.fileUpload) {
                    var fileuploadSbt = info.fileUpload.split(",");
                    var htmlAno = "";
                    if (fileuploadSbt.length > 0) {
                        for (var k = 0; k < fileuploadSbt.length; k++) {
                            var fileItemAno = fileuploadSbt[k];
                            htmlAno += '<a target="_blank" href="' + fileItemAno + '">' + fileItemAno + '</a></br>';
                        }
                        $(".item-from-file").html(htmlAno);
                    }
                }

                //ttdl

                editor2.value(ttdl.newsLog.Note);

                var htmlTtdl = "";
                if (ttdl.lstFiles.length > 0) {
                    for (var i = 0; i < ttdl.lstFiles.length; i++) {
                        var fileItem = ttdl.lstFiles[i];
                        htmlTtdl += '<a target="_blank" href="' + fileItem.url + '">' + fileItem.name + '</a></br>';
                    }
                    $(".item-from-file-ttdl").html(htmlTtdl);
                }

                //ldtcmt

                editor3.value(ldtcmt.newsLog.Data);
                var htmlldtcmt = "";
                if (ldtcmt.lstFiles.length > 0) {
                    for (var j = 0; j < ldtcmt.lstFiles.length; j++) {
                        var fileItem2 = ttdl.lstFiles[j];
                        htmlldtcmt += '<a target="_blank" href="' + fileItem2.url + '">' + fileItem2.name + '</a></br>';
                    }
                    $(".item-from-file-ldtcmt").html(htmlldtcmt);
                }
            }
            if (grid) {
                grid.clearSelection();
                grid.dataSource.read();
            }
        },
        function (xhr, status, error) {
            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
            if (grid) {
                grid.clearSelection();
                grid.dataSource.read();
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    );
}


function baocaoketqua(news_id) {
    var url = `${appConfig.apiHostUrl}/${NEWS_API.GET_REPORT}`;
    url = url + "?id=" + news_id;
    callAjax(
        url,
        null,
        AjaxConst.GetRequest,
        function (xhr) {
            $(this).addClass('disabled').attr('disabled', true);
            xhr.setRequestHeader('Authorization', `Bearer ${jwtToken}`);
        },
        function (success) {
            try {
                $("#commentNews").val(success.data);
                $("#IdReport").val(success.Id);
            } catch (e) {
                $("#IdReport").val("0");
            }
            $("#exampleModalNew2").modal('show');
        },
        function (xhr, status, error) {

            if (xhr.status === 400) {
                var err = eval("(" + xhr.responseText + ")");
                err.forEach(function (item) {
                    messagerError(item.Code, item.Description);
                });
            } else {
                messagerError(MESSAGES.ERR_CONNECTION.key, MESSAGES.ERR_CONNECTION.value);
            }
        },
        function (complete) {
            $(this).removeClass('disabled').removeAttr('disabled');
        }
    )
}