// Write your Javascript code.
//Script for paging
//currentPage, func, totalPage, sort, filter, nowsort


$(function () {

    $('.table-footable').footable();
    $('[data-toggle="tooltip"]').tooltip();


   $("input:text").focus(function () { $(this).select(); });

});

    //event handler untuk show/close modal dialog
//$("#modal-action-customer").on('loaded.bs.modal', function (e) {

$('body').on('loaded.bs.modal', '#modal-container', function (e) {

    //$('.table-footable').footable();
    $.validator.unobtrusive.parse("form");
    //alert('open');
});

$('body').on('hidden.bs.modal', '#modal-container', function (e) {
    $('.modal-content').html('');
    $(this).removeData('bs.modal');
});
//IVY : alert!!!! tadinya event handler untuk modal dikaitkan ke modal-container namun sekarang dikaitkan ke body. ini untuk mengatasi
//partial view modal yang diletakkan dalam view component maupun diletakkan dalam index page.
//Perhatikan juga pada event loaded.bs.modal, tadinya ada $('.table-footable').footable() namun sekarang dicomment karena menyebabkan index page stock watclist/screening
//terscroll kembali ke atas saat menampilkan chart. Ini tidak baik sehingga kita buang, konsekuensinya bila di modal form membutuhkan fungsi footable maka tidak akan jalan


//$("#modal-container").on('loaded.bs.modal', function (e) {
 

//    //$('.table-footable').footable();
//    $.validator.unobtrusive.parse("form");
//})
//        .on('hidden.bs.modal', function (e) {

//            $('.modal-content').html('');
//            //$('#chart').html('');
//            $(this).removeData('bs.modal');
//        });


function DisplayError(err)
{
    if(err.status == '401')
        displayModal('Error', 'Your session is ended. Please re-login', '/Common/ShowModal/');
    else
        displayModal('Error', err.statusText, '/Common/ShowModal/');
}

function addToWatchlist(stockID) {
    //alert('masuk dari global');
    $.ajax({
        url: '/StockWatchList/AddToStock/',
        data: JSON.stringify(stockID),
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        cache: false,
        success: function (result) {
            //$('#List').unmask("waiting ...");
            //load returned data inside contentFrame DIV

            if (result) {
                displayModal('Error', result, '/Common/ShowModal/');
            }

            else {
                //refreshStockWatchList();
                //$('.table-responsive').responsiveTable('update');
                //ga jadi lah
                //displayModal('Information', Id.toUpperCase() + ' has been added to your watchlist', '/Common/ShowModal/');
            }
            //$('#spanValidation').text(result);
            //$('#txtStock').val('');

        },
        error: function (error) {
            alert("sitejs addToWatchlist " + error);
            console.log(error);
        }
    });
}


function displayModal(header, message, url) {

    $.ajax({
        url: url,
        data: { header: header, body: message },
        type: 'POST',
        //cache: false,
        success: function (result) {
            
            $('#modal-content').html(result);
            $('.modal-content').html(result);
            $('#modal-container').modal('show');

            

            //$('#modal-action-customer').modal('show');

            //IVY : Ini ditutup dulu karena menyebabkan table tidak rapi saat tidak ada plan dan discan
            //$('.table-footable').footable();


            //$('#modal-action').modal('show');

            //$('.table-responsive').responsiveTable('update');
        }
    });

};

function paging(currentPage, gotoPage, totalPage, sort, filter, nowsort, url, divName) {
        //var currentPage = @Model.currentPage;
        //var totalPage = @Model.totalPage;
        //var sort = '@Model.sorting';
        //var filter = '@Model.filter';
        //var url = '@Model.url';
    //var divName = '#@Model.divName';
        //alert(currentPage + ' ' + gotoPage + ' ' + totalPage + ' ' + sort + ' ' + filter + ' ' + nowsort + ' ' + url + ' ' + divName);
   

        if (gotoPage == currentPage && nowsort == '')
        {
            return 0;
        };

        var pageIndex=0;
        if (gotoPage == "F")
        {
            pageIndex = 1;
        }
        else if (gotoPage == "L")
        {
            pageIndex = totalPage;
        }
        else if (gotoPage == "P")
        {
            if (currentPage > 1) {
                pageIndex = currentPage - 1;
            }
            else
                return;
        }
        else if (gotoPage == "N")
        {
            if(currentPage < totalPage)
            {
                pageIndex = currentPage + 1;
            }
            else
                return;
        }
        else
        {
            pageIndex = gotoPage;
        }

        $('#mask').show();
        //$('#mask').addClass('ajax');
        $.ajax({
            //url: '/StockWatchList/Index/',
            url: url,
            data: { filter: filter, page: pageIndex, sort: sort, nowsort: nowsort },
            type: 'POST',
            //cache: false,
            success: function (result) {
                //$('#List').unmask("waiting ...");
                //load returned data inside contentFrame DIV
  
                $('#' + divName).html(result);
                //$('.table-responsive').responsiveTable('update');
                $('.table').footable();
                $('#mask').hide();
                //$('#mask').removeClass('ajax');
            },
            error: function (fail) {
                console.log(fail);
                alert("Fail paging " + fail);
                //$('#mask').removeClass('ajax');
                $('#mask').hide();
            }
        });

};

//------------function untuk charting d3


