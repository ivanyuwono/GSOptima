//if (document.readyState === 'complete') {
//    alert(' The page is fully loaded');
//}

//$(document).ready(function () {

function precisionRound(number, precision) {
    var factor = Math.pow(10, precision);
    return Math.round(number * factor) / factor;
}
function toDate(dateStr) {
    //var parts = dateStr.toString().split("/");
    var yyyy = dateStr.substring(0, 4);
    var mm = dateStr.substring(5, 7);
    var dd = dateStr.substring(8, 10);
    //var date = new Date(parts[2], parts[0] - 1, parts[1]);

    var date = new Date(yyyy, mm - 1, dd);

    //return new Date(parts[2], parts[1] - 1, parts[0]);
    return date;
};




function AddRemoveWatchlist(callback) {
    //callback adalah sebuah fungsi yang akan dijalankan saat ajax call berhasil dipanggil contoh callback adalah fungsi refresh watchlist
    //teknik ini untuk membuat 2 ajax call menjadi sequential karena pada dasarnya ajax call ada pararel thread
    var action = "";
    var value = $('#btnAddToStock, #btnAddToStockxs').attr("value");

    //alert('function ' + value);

    if (value == "R")
        action = "R"
    else if (value == "A")
        action = "A";
    else
        alert('invalid value');


    var Id = $('#hiddenStockID').val();
    if (Id == '')
        return;

    var obj = { "StockID": Id, "Action": action };


    $.ajax({
        url: '/StockWatchList/AddToStock/',
        data: JSON.stringify(obj),
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
                if (action == "A") {
                    $('#btnAddToStock, #btnAddToStockxs').attr("value", "R");
                    $('#divAddToStock, #divAddToStockxs').text("Remove");
                    //$('#btnAddToStock').text("Remove");
                }
                else {
                    $('#btnAddToStock, #btnAddToStockxs').attr("value", "A");
                    $('#divAddToStock, #divAddToStockxs').text("Add");
                    //$('#btnAddToStock').text("Add");
                }


                if (callback)
                    callback();
                //refreshStockWatchList();
            }
        },
        error: function (error) {
            DisplayError(error);
            alert('chartjs AddRemoveWatchlist ' + error);

        }
    });

}
    //$('#txtStockId').val('AALI');
    //$('#btn1Y').prop('disabled', false);
    //$('#btn6M').prop('disabled', false);
    //$('#btn3M').prop('disabled', true);
    //$('#btn1M').prop('disabled', false);



    //$('#btnAddToStock, #btnAddToStockxs').on('click', function () {
    //    alert('add from chart');

    //    var action = "";
    //    var value = $('#btnAddToStock, #btnAddToStockxs').attr("value");
 
    //    if (value == "R")
    //        action = "R"
    //    else if (value == "A")
    //        action = "A";
    //    else
    //        alert('invalid value');

        
    //    var Id = $('#hiddenStockID').val();
    //    if (Id == '')
    //        return;

    //    var obj = { "StockID": Id, "Action": action };  

    //    //alert(value);

    //    $.ajax({
    //        url: '/StockWatchList/AddToStock/',
    //        data: JSON.stringify(obj),
    //        type: 'POST',
    //        dataType: 'json',
    //        contentType: 'application/json',
    //        cache: false,
    //        success: function (result) {
    //            //$('#List').unmask("waiting ...");
    //            //load returned data inside contentFrame DIV
 
    //            if (result) {
    //                displayModal('Error', result, '/Common/ShowModal/');
    //            }
    //            else
    //            {
    //                if (action == "A") {
    //                    $('#btnAddToStock, #btnAddToStockxs').attr("value", "R");
    //                    $('#divAddToStock, #divAddToStockxs').text("Remove");
    //                    //$('#btnAddToStock').text("Remove");
    //                }
    //                else {
    //                    $('#btnAddToStock, #btnAddToStockxs').attr("value", "A");
    //                    $('#divAddToStock, #divAddToStockxs').text("Add");
    //                    //$('#btnAddToStock').text("Add");
    //                }
    //                //refreshStockWatchList();
    //            }
    //        },
    //        error: function (error) {
    //            DisplayError(err);
    //            //alert(error);

    //        }
    //    });

    //});


$(document).ready(function () {


    d3.select(window).on('resize', resize);
    $('#optData').on('change', function () {
        if (txtStockId.value != '')
            OneMonth($('#txtStockId').val(), optData.value);
    });

    $('#btnChart').on('click', function () {
        if (txtStockId.value != '')
            OneMonth($('#txtStockId').val(), 6);
    });

    $("#txtStockId").keypress(function (event) {

        if (event.which == 13) {
            if (txtStockId.value != '')
                OneMonth($('#txtStockId').val(), 6);
        }

    });


    //$('#divTime').on('click', '#btn1M', function () {
    $('#btn1M, #btn1Mxs').on('click', function () {
        //if ($('#txtStockId').length > 0)  //exist
        //{
        //    if ($('#txtStockId').val() != '') {
        //        OneMonth($('#txtStockId').val(), 1);
        //    }
        //}
        //else if ($('#hiddenStockID').val() != '') {
        //    OneMonth($('#hiddenStockID').val(), 1);
        //}
        OneMonth($('#hiddenStockID').val(), 1);
        //$('#btn1M, #btn1Mxs').addClass('active');
        //$('#btn3M, #btn3Mxs').removeClass('active');
        //$('#btn6M, #btn6Mxs').removeClass('active');
        //$('#btn1Y, #btn1Yxs').removeClass('active');

        $('#btn1M, #btn1Mxs').prop('disabled', true);
        $('#btn6M, #btn6Mxs').prop('disabled', false);
        $('#btn3M, #btn3Mxs').prop('disabled', false);
        $('#btn1Y, #btn1Yxs').prop('disabled', false);

    });
    //$('#divTime').on('click', '#btn3M', function () {
    $('#btn3M, #btn3Mxs').on('click', function () {
        //if ($('#txtStockId').length > 0)  //exist
        //{
        //    if ($('#txtStockId').val() != '') {
        //        OneMonth($('#txtStockId').val(), 3);
        //    }
        //}
        //else if ($('#hiddenStockID').val() != '') {
        //    OneMonth($('#hiddenStockID').val(), 3);
        //}
        OneMonth($('#hiddenStockID').val(), 3);
        $('#btn3M, #btn3Mxs').prop('disabled', true);
        $('#btn1M, #btn1Mxs').prop('disabled', false);
        $('#btn6M, #btn6Mxs').prop('disabled', false);
        $('#btn1Y, #btn1Yxs').prop('disabled', false);
    });
    $('#btn6M, #btn6Mxs').on('click', function () {

        //if ($('#txtStockId').length > 0)  //exist
        //{
        //    if ($('#txtStockId').val() != '') {
        //        OneMonth($('#txtStockId').val(), 6);
        //    }
        //}
        //else if ($('#hiddenStockID').val() != '') {
        //    OneMonth($('#hiddenStockID').val(), 6);
        //}

        OneMonth($('#hiddenStockID').val(), 6);
        $('#btn6M, #btn6Mxs').prop('disabled', true);
        $('#btn1M, #btn1Mxs').prop('disabled', false);
        $('#btn3M, #btn3Mxs').prop('disabled', false);
        $('#btn1Y, #btn1Yxs').prop('disabled', false);
    });
    $('#btn1Y, #btn1Yxs').on('click', function () {
        //if ($('#txtStockId').length > 0)  //exist
        //{
        //    if ($('#txtStockId').val() != '') {
        //        OneMonth($('#txtStockId').val(), 12);
        //    }
        //}
        //else if ($('#hiddenStockID').val() != '')
        //{
        //    OneMonth($('#hiddenStockID').val(), 12);
        //}

        OneMonth($('#hiddenStockID').val(), 12);
        $('#btn1Y, #btn1Yxs').prop('disabled', true);
        $('#btn1M, #btn1Mxs').prop('disabled', false);
        $('#btn3M, #btn3Mxs').prop('disabled', false);
        $('#btn6M, #btn6Mxs').prop('disabled', false);
    });
   
    //*pastikan berisi saham awal yang mau diload
    var save_Stock = 'AALI';

    function OneMonth(stockID, num) {
        save_Stock = stockID;
        LoadCSV(stockID, num * 30);
    }
    function LoadCSV(stockId, num) {


        var month = num / 30;

        if (month == 1) {
            $('#btn1Y, #btn1Yxs').prop('disabled', false);
            $('#btn6M, #btn6Mxs').prop('disabled', false);
            $('#btn3M, #btn3Mxs').prop('disabled', false);
            $('#btn1M, #btn1Mxs').prop('disabled', true);
        }
        else if (month == 3) {
            $('#btn1Y, #btn1Yxs').prop('disabled', false);
            $('#btn6M, #btn6Mxs').prop('disabled', false);
            $('#btn3M, #btn3Mxs').prop('disabled', true);
            $('#btn1M, #btn1Mxs').prop('disabled', false);
        }
        else if (month == 6) {
            $('#btn1Y, #btn1Yxs').prop('disabled', false);
            $('#btn6M, #btn6Mxs').prop('disabled', true);
            $('#btn3M, #btn3Mxs').prop('disabled', false);
            $('#btn1M, #btn1Mxs').prop('disabled', false);
        }
        else if (month == 12) {
            $('#btn1Y, #btn1Yxs').prop('disabled', true);
            $('#btn6M, #btn6Mxs').prop('disabled', false);
            $('#btn3M, #btn3Mxs').prop('disabled', false);
            $('#btn1M, #btn1Mxs').prop('disabled', false);
        }

        $('#txtStockId').val(stockId);
        


        var obj = { "StockId": stockId, "NumberOfDays": num };
        var data = JSON.stringify(obj);
        //alert("LoadCSV " + stockId + ' ' + obj);
        if (stockId == '') {
            displayModal('Error', 'Stock is not valid', '/Common/ShowModal/');
            return;
        }
        $('#mask').show();


        $.ajax({
            //url: '/StockChart/GetPrices/',
            //data: JSON.stringify(obj),
            url: '/StockChart/GetPrices/',
            data: JSON.stringify(obj),
            type: 'POST',
            dataType: 'json',   //balikan data dari server
            contentType: 'application/json',

            cache: false,
            success: function (output) {

  
                if (output) {
 
                    var result = output.prices;
   
                    var name = output.stockName;
                    if (name == '')
                        alert('not valid');

                    $('#hiddenStockID').val(stockId); //bila berhasil maka stockID ditampung pada hidden textbox
                    var isOnWatchList = output.isOnWatchList;
                    if (isOnWatchList == "R")
                    {   //ada di watchlist
                        
                        var value = $('#btnAddToStock, #btnAddToStockxs').attr("value");
                        //alert('ada '+ value);
                        //$('#btnAddToStock, #btnAddToStockxs').prop("value", "R");
                        $('#btnAddToStockxs').attr("value", "R");
                        $('#btnAddToStock').attr("value", "R");
                        //$('#btnAddToStock, #btnAddToStockxs').prop("value", "R");
                        $('#divAddToStock, #divAddToStockxs').text('Remove');
                    }
                    else
                    {
                        
                        var value = $('#btnAddToStock, #btnAddToStockxs').attr("value");
                        //alert('tdk ada'+value);
                        //$('#btnAddToStock, #btnAddToStockxs').prop("value", "A");
                        $('#btnAddToStockxs').attr("value", "A");
                        $('#btnAddToStock').attr("value", "A");
                        $('#divAddToStock, #divAddToStockxs').text('Add');
                    }
                    //if valid stock

                    //$.ajax({
                    //    url: '/StockWatchList/InquiryWatchlist/',
                    //    data: JSON.stringify(stockId),
                    //    type: 'POST',
                    //    dataType: 'json',
                    //    contentType: 'application/json',
                    //    cache: false,
                    //    success: function (result) {

        
                    //        if (result == true)
                    //        {   //ada di watchlist
       
                    //            var value = $('#btnAddToStock').attr("value");
                    //            $('#btnAddToStock').prop("value", "R");
                    //            $('#divAddToStock').text('Remove');
                    //         }
                    //        else
                    //        {
                    //            var value = $('#btnAddToStock').attr("value");
                    //            $('#btnAddToStock').prop("value", "A");
                    //            $('#divAddToStock').text('Add');
                    //        }
                    //    },
                    //    error: function (error) {
                    //        alert(error);
                    //    }
                    //});


                    result.forEach(function (d, i) {                    // NEW
                        d.index = i;
                    });




                    var maxdate = d3.max(result, function (d) {
                        return toDate(d.date);
                    });
        


                    var x = [];
                    if (num > 0) {
                        x = result.filter(function (d) {
                            var limit = maxdate.getTime() - num * 8.64e7;

                            if (toDate(d["date"]).getTime() >= limit) {
                                return d;
                            }

                        });
                    }
                    else {

                        x = result;
                    }

                    //display price properties 
                    var sName = output.stockID + ' - ' + output.stockName;
                    $('#stockName, #stockNamexs').text(sName);
                    $('#lastOpen, #lastOpenxs').text(output.lastOpen.toLocaleString('id-ID'));
                    $('#lastHigh, #lastHighxs').text(output.lastHigh.toLocaleString('id-ID'));
                    $('#lastLow, #lastLowxs').text(output.lastLow.toLocaleString('id-ID'));
                    $('#lastClose, #lastClosexs').text(output.lastClose.toLocaleString('id-ID'));
            
            
                    var str = '-';
                    if (!output.lastSupport) {
                        str = '-';
                    }
                    else   {
                        str = output.lastSupport.toLocaleString('id-ID');
                    }
                 
                    $('#support, #supportxs').text(str);

                    if (!output.lastResistance) {
                        str = '-';
                    }
                    else {
                        str = output.lastResistance.toLocaleString('id-ID');
                    }

                    $('#resistance, #resistancexs').text(str);

                    if(output.percentToSupport==0)
                    {
                        str = '0';
                        $('#rangeToSupport, #rangeToSupportxs').text(precisionRound(str, 3) + '%');
                    }
                    else if (!output.percentToSupport) {
                        str = '-';
                        $('#rangeToSupport, #rangeToSupportxs').text(str);
                    }
                    else {
                        str = output.percentToSupport;
                        $('#rangeToSupport, #rangeToSupportxs').text(precisionRound(str, 3) + '%');
                    }
                    
                    if (output.percentToResistance == 0)
                    {
                        str = '0';
                        $('#rangeToResist, #rangeToResistxs').text(precisionRound(str, 3) + '%');
                    }
                    else if (!output.percentToResistance) {
                        str = '-';
                        $('#rangeToResist, #rangeToResistxs').text(str);
                    }
                    else {
                        str = output.percentToResistance;
                        $('#rangeToResist, #rangeToResistxs').text(precisionRound(str, 3) + '%');
                    }
                    


                    if (!output.risk) {
                        str = 'N/A';
                        $('#risk, #riskxs').text(str);
                    }
                    else {
                        str = output.risk;
                        $('#risk, #riskxs').text(precisionRound(str, 3) + '%');
                    }
                    
                    

                    temp_data = x;
                    display(x, name);
                    //$('#placeholder').html(output);

                }
                else {
                    //displayModal('Information', Id + ' has been added to your watchlist', '/StockWatchList/ShowModal/');
                    //alert("Stock ID not valid");
                    displayModal('Error', 'Stock is not valid', '/Common/ShowModal/');
                }

                $('#mask').hide();
            },
            error: function (err) {
                console.log("load csv");
                console.log(err);
                //alert("error " + err);
                DisplayError(err);
                $('#mask').hide();
            }
        });


    }
    //------------------------------------------------------------------------------//
    //*pindah disini
        

    var current_data_num = 0;

    var temp_data = [];
    var prev_zoom, prev_start, prev_end, prev_x, prev_y;

    var margin = { top: 30, right: 45, bottom: 30, left: 45 }
        
        

    //var w = parseInt(d3.select('#container').style('width'), 10);
    
    //var w = document.getElementById('chart').offsetWidth;
    var w = 0;
    var viewport_width = $(window).width();

    if ($('.modal-body').length > 0) {
        //bila modal-body ada artinya chart dibuka sebagai pop up, maka tidak bisa ambil lebar dari modal-content karena kadang-kadang isinya 0
        //w = $('#chart').width();

        //cara lain kita tentukan secara manual lebarnya sesuai dengan css bootstrap, bila css bootstrap berubah maka code dibawah ini juga harus diubah
        if (viewport_width >= 768 && viewport_width < 992) {
            w = 526;
        }
        else if (viewport_width >= 992) {
            w = 826;
        }
        else if (viewport_width < 768) {
            w = viewport_width - 128;
        }

    }
    else {
        //bila modal-body tidak ada artinya dibuka dari menu chart maka kita bisa ambil lebar dari div chart
        w = $('.chart').width();


    }
    

    if (w <= 0 || w==margin.left+margin.right) {
        console.log(w + ' ' + $('.chart').width() + ' ' + $('.modal-dialog').width() + ' ' + $(window).width());
        alert(w + ' ' + $('.chart').width() + ' ' + $('.modal-dialog').width() + ' ' + $(window).width());
    }
    else
    {
        temp_width = 0;
    }
    w = w - margin.left - margin.right;
    var buffer = 10;

    //variabel dibawah ini untuk mengubah tinggi dari masing2 indikator teknikal
    //bila ada perubahan, sesuaikan juga dalam chart.css.
    //tinggi dari chart dalam chart.css minimal adalah sama dengan total tinggi dari masing2 indikator teknikal
    var h = 400 - margin.top - margin.bottom;
    var recWidth = 5;
    var hVol = 100, htrendColor = 20, htrendBar = 101, hGSLine = 95;


    // Create svg element
    var clipDiv = d3.select('#chart').classed('chart', true).append('div')
        .attr('id', 'series-clip')
        .style('position', 'absolute')
        .style('overflow', 'hidden')
        .style('top', margin.top + 'px')
        .style('left', margin.left + 'px');

    var volDiv = d3.select('#chart').classed('chart', true).append('div')
        .attr('id', 'volume-clip')
        .style('position', 'absolute')
        .style('overflow', 'hidden')
        .style('top', (margin.top + (h - hVol) + 'px'))
        .style('left', margin.left + 'px');

    var srLabelDiv = d3.select('#chart').classed('chart', true).append('div')
            .attr('id', 'srlabel-clip')
            .style('position', 'absolute')
            .style('overflow', 'hidden')
            .style('top', (margin.top + 'px'))
            .style('left', (margin.left + w - buffer) + 'px');


    var seriesDiv = clipDiv.append('div')
        .attr('id', 'series-container');

    var seriesVolDiv = volDiv.append('div')
        .attr('id', 'volume-container');

    var seriesSRLabelDiv = srLabelDiv.append('div')
        .attr('id', 'srlabel-container');

    var trendColorDiv = d3.select('#chart').classed('chart', true).append('div')
        .attr('id', 'trendcolor-clip')
        .style('position', 'absolute')
        .style('overflow', 'hidden')
        .style('top', (margin.top + h + 'px'))
        .style('left', margin.left + 'px');

    var trendBarDiv = d3.select('#chart').classed('chart', true).append('div')
        .attr('id', 'trendbarclip')
        .style('position', 'absolute')
        .style('overflow', 'hidden')
        .style('top', (margin.top + (h + htrendColor) + 'px'))
        .style('left', margin.left + 'px');

    var GSLineDiv = d3.select('#chart').classed('chart', true).append('div')
        .attr('id', 'gslineclip')
        .style('position', 'absolute')
        .style('overflow', 'hidden')
        .style('top', (margin.top + (h + htrendColor + htrendBar) + 'px'))
        .style('left', margin.left + 'px');


    //Create svg for whole
    var svg = d3.select('#chart').append('svg')
        .style('position', 'absolute')
        .attr('width', w + margin.left + margin.right)
        .attr('height', h + margin.top + margin.bottom + htrendColor + htrendBar + hGSLine);

    var seriesSvg = seriesDiv.append('svg')
        .attr('width', w - buffer)   //IVY
        .attr('height', h);

    if (w - buffer == 0 || w == 0) {

        console.log('init2 w= ' + w + ' buffer= ' + buffer);
        alert('init2 w= ' + w + ' buffer= ' + buffer + ' ' + $('#chart').length );
    }

    var seriesVolSvg = seriesVolDiv.append('svg')
        .attr('width', w - buffer)   //IVY
        .attr('height', hVol);

    var trendColorSvg = trendColorDiv.append('svg')
        .attr('width', w - buffer)   //IVY
        .attr('height', htrendColor);

    var trendBarSvg = trendBarDiv.append('svg')
        .attr('width', w - buffer)   //IVY
        .attr('height', htrendBar);

    var GSLineSvg = GSLineDiv.append('svg')
        .attr('width', w - buffer)   //IVY
        .attr('height', hGSLine);

    var seriesSRLabelSvg = seriesSRLabelDiv.append('svg')
            .attr('width', buffer)   //IVY
            .attr('height', h);



    var zoom = d3.zoom()
    .scaleExtent([1, 4])
    .on("zoom", zoom2);

    //var drag = d3.drag().on("drag", dragged);


    //Create candle chart  container
    var g = svg.append('g')
        .attr('transform', 'translate(' + margin.left + ',' + margin.top + ')');

    //var test = svg.append('g').	attr('transform', 'translate(' + margin.left + ',' + (margin.top + hVol) + ')');

    // Create plot area
    var plotArea = g.append('g');
    plotArea.append('clipPath')
        .attr('id', 'plotAreaClip')
        .append('rect').attr('width', w - buffer)  //IVY
        .attr('height', h);


    plotArea.attr('clip-path', 'url(#plotAreaClip)');

    var p = plotArea.append('rect')
        .attr('class', 'zoom-pane')
        .attr('width', w - buffer)  //IVY
        .attr('height', h).on("mousemove", handleMouseMove);

    p.call(zoom);


    var xScale, yScale, yVolScale, yGSLineScale, yBigWaveScale;



    var line = d3.line()
    .x(function (d) {
        return d.x;
    })
    .y(function (d) {
        return d.y;
    });



    var bars, path, rect, xAX, yAX, xAxis, yAxis, yVolAX, yVolAX, volbars;
    xAX = g.append('g')
    .attr('class', 'axis x')
    .attr('transform', 'translate(' + 0 + ',' + (h + htrendColor + htrendBar + hGSLine) + ')');

    yAX = g.append('g').classed("axis y", true).attr('transform', 'translate(' + w + ',0)');
    yVolAX = g.append('g').classed("axis y", true).attr('transform', 'translate(' + '0' + ',' + (h - hVol) + ')');


    trendBarSvg.append("line").classed("trendbarline", true);
    trendBarSvg.append("line").classed("trendbarbottomline", true);
    trendBarSvg.append("text").classed("label1", true);

    GSLineSvg.append("line").classed("gslinecenter", true);
    GSLineSvg.append("text").classed("label1", true);
    GSLineSvg.append("text").classed("label2", true);

    seriesSvg.append("text").classed("label", true);
    seriesSvg.append("text").classed("label2", true);
    seriesSvg.append("text").classed("label3", true);


    seriesSRLabelSvg.append("text").classed("labelS", true);
    seriesSRLabelSvg.append("text").classed("labelR", true);

    //var save_Stock = '';

    var obj = JSON.parse(bolla);



    var result = obj.prices;
    var num = obj.numberOfDays;
    var name = obj.stockName;
    result.forEach(function (d, i) {                    // NEW
        d.index = i;
    });

    var maxdate = d3.max(result, function (d) {
        return toDate(d.date);
    });

    //var maxdate = toDate(result[result.length - 1]);

    var x = [];
    if (num > 0) {
        x = result.filter(function (d) {
            var limit = maxdate.getTime() - num * 8.64e7;

            if (toDate(d["date"]).getTime() >= limit) {
                return d;
            }

        });
    }
    else {

        x = result;
    }

    temp_data = x;
    display(x, name);

    //document.addEventListener('DOMContentLoaded', function () {
    ////$(document).ready(function () {
    //document.addEventListener('DOMContentLoaded',
    //  function () {
    //      alert('hello!');
    //  }, false);
    //document.getElementById("chart").onload = function () { myFunction() };


    //document.getElementById("chart").addEventListener("DOMContentLoaded", myFunction);




    
 

    //function GetPrices(stockId, numberOfDays) {

    //    $.ajax({
    //        url: '/StockChart/Index/',
    //        //data: { filter: $('#filter').val(), page: '', sort:'', nowsort:''},
    //        data: { stockId: stockId, numberOfDays: numberOfDays },
    //        type: 'POST',
    //        //cache: false,
    //        success: function (result) {
    //            //$('#List').unmask("waiting ...");
    //            //load returned data inside contentFrame DIV
    //            alert("success");
    //            $('#chart').html(result);
    //            //$('#mask').hide();
    //        }
    //    });
    //}

    //*semula disini


       
    //var result = @Html.Raw(Json.Encode(Model.prices));


    //*disinijuga


    //*initial load
    //LoadCSV('AALI', 30);
    //$('#txtStockId').val('AALI');
       

    function refreshYScale(datum) {

        var min = d3.min(datum, function (d) {
            return d.low;
        });


        var max = d3.max(datum, function (d) {
            return d.high;
        });
        var maxVol = d3.max(datum, function (d) {
            return d.volume;
        });

        var minGS = d3.min(datum, function (d) {
            return d.gsLine;
        });
        var maxGS = d3.max(datum, function (d) {
            return d.gsLine;
        });

        var minBigWave = d3.min(datum, function (d) {
            if (d.frequency == 0 || d.frequency == null) return 0
            else
                return d.volume / (d.frequency * d.frequency * d.frequency);
        });
        var maxBigWave = d3.max(datum, function (d) {
            if (d.frequency == 0 || d.frequency == null) return 0
            else
                return d.volume / (d.frequency * d.frequency * d.frequency);
        });

        yScale = d3.scaleLinear()
                .domain([min * 0.90, max * 1.10]).range([h, 0]);

        yVolScale = d3.scaleLinear().domain([0, maxVol]).range([hVol, 0]);
        //alert(minGS + " " + maxGS);

        //if(Math.abs(minGS) >= Math.abs(maxGS))
        //{
        //	yGSLineScale = d3.scaleLinear().domain([-minGS , minGS]).range([hGSLine, 0]);
        //}
        //else
        //{
        //	yGSLineScale = d3.scaleLinear().domain([-maxGS , maxGS]).range([hGSLine, 0]);
        //}

        //Sengaja dikurangi 20 untuk margin
        yGSLineScale = d3.scaleLinear().domain([minGS, maxGS]).range([hGSLine  , 10]);

        yBigWaveScale = d3.scaleLinear().domain([0, maxBigWave]).range([hGSLine   , 10]);

        oldMaxVol = maxVol;
        //yVolScale.clamp(true);

    }
    function SearchByIndex(datum, index1, index2) {
        var arr_temp = []
        for (var x = 0; x < datum.length; x++) {
            if (datum[x].index >= index1 && datum[x].index <= index2) {
                arr_temp.push(datum[x]);
            }

            //return datum[x];
        }
        return arr_temp;
    }

    function display(datum, name) {

  
        //*ditutup dulu
        //var mindate = d3.min(datum, function (d) {
        //    return toDate(d.date);
        //});
        //d3.selectAll(".zoom-pane").call(zoom.transform, d3.zoomIdentity);

        //var maxdate = d3.max(datum, function (d) {
        //    return toDate(d.date);
        //});

        refreshYScale(datum);


        var maxindex = d3.max(datum, function (d) {
            return d.index;
        });
        var minindex = d3.min(datum, function (d) {
            return d.index;
        });




        xScale = d3.scaleLinear().clamp(false)
        //.domain(datum.map(function(d) { return d.index; })).range([0, w]);
        .domain([minindex - 0.5, maxindex + 0.5]).range([0, w - buffer]);  //IVY
        //.domain([
        //	new Date(mindate.getTime() ),
        //	new Date(maxdate.getTime() + 8.64e7)
        //]).range([0, w]);

        recWidth = xScale(1) - xScale(0) - 1;

        //alert ('texx ' + mindate + ' ' + maxdate + ' ' + w);
        //alert(d3.timeMonths(mindate.getTime(), maxdate.getTime(), 1));


        var tickarray = [];
        var prevmonth = 0, prevyear = 0;

        for (var i = 0; i <= datum.length - 1; i++) {
            if (toDate(datum[i].date).getMonth() != prevmonth || toDate(datum[i].date).getFullYear() != prevyear) {
                tickarray.push(toDate(datum[i].date));
                prevmonth = toDate(datum[i].date).getMonth();
                prevyear = toDate(datum[i].date).getFullYear();
            }
        }

        xAxis = d3.axisBottom()
        .scale(xScale)
        .tickFormat(function (d, i) {//console.log(d + " " + i);
            if (Math.floor(d) != d) {
                return;
            }

            var formatMonth = d3.timeFormat("%d/%m/%Y")

            //	return datum[i].date;
            for (var x = 0; x < datum.length; x++) {
                if (datum[x].index == d)
                    //return toDate(datum[x].date);
                    return formatMonth(toDate(datum[x].date));
            }
        })

        DetermineTicks();

        //return datum[d].date});

        //.tickValues(tickarray)


        //.tickValues([new Date(2016,7,1), new Date(2016,8,1), new Date(2016,9,1)])
        //.ticks(d3.timeMonth.every(1))
        //.tickFormat(d3.timeFormat("%b %Y"));
        //.ticks(d3.timeMonday.every(1))
        //.tickFormat(d3.timeFormat("%d/%m/%Y"));


        //.tickSize(16, 0);
        //.tickFormat(d3.timeFormat("%b %Y"));

        //.tickFormat(1,"%B");
        //.ticks(d3.timeMonth.every(1));
        yAxis = d3.axisRight().scale(yScale);

        //yVolAxis = d3.axisLeft().scale(yVolScale).tickFormat(d3.formatPrefix(".1", 1e6)).ticks(5);
        yVolAxis = d3.axisLeft().scale(yVolScale).tickFormat(d3.formatPrefix(".0", 1e6)).ticks(5);

        xAX.call(xAxis);
        yAX.call(yAxis);
        yVolAX.call(yVolAxis);

        var r_datum = datum.filter(function (d) { if (d.resistance > 0) return d });
        var lineR = seriesSvg.selectAll(".lineR").data([r_datum]);

        var s_datum = datum.filter(function (d) { if (d.support > 0) return d });
        var lineS = seriesSvg.selectAll(".lineS").data([s_datum]);

        var BB_datum = datum.filter(function (d) { if (d.bbUpper > 0 && d.bbLower > 0) return d });
        var areaBB = seriesSvg.selectAll(".bb").data([BB_datum]);

        //var gs_datum = datum.filter(function (d) { if (isNaN(d.gsLine) == false) return d });
        var gs_datum = datum.filter(function (d) { if (d.gsLine != null) return d });
        var gsline = GSLineSvg.selectAll(".gsline").data([gs_datum]);

        //var sLabel = seriesSRLabelSvg.selectAll(".labelS").data([s_datum]);
        //var rLabel = seriesSRLabelSvg.selectAll(".labelR").data([r_datum]);

        lineR.exit().remove();
        lineS.exit().remove();
        areaBB.exit().remove();
        gsline.exit().remove();

        //sLabel.exit().remove();
        //rLabel.exit().remove();

        var resistanceGraph = lineR.enter().append("g").classed("lineR", true).append("path");
        var supportGraph = lineS.enter().append("g").classed("lineS", true).append("path");
        var bbGraph = areaBB.enter().append("g").classed("bb", true).append("path");
        var GSLineGraph = gsline.enter().append("g").classed("gsline", true).append("path");

        //var sLabelGraph = sLabel.enter().append("text").classed("labelS", true));
        //var rLabelGraph = rLabel.enter().append("text").classed("labelR", true));

        drawResistance(resistanceGraph, xScale, yScale);
        drawResistance(lineR.select("path"), xScale, yScale);

        drawSupport(supportGraph, xScale, yScale);
        drawSupport(lineS.select("path"), xScale, yScale);

        drawSRLabel(datum, xScale, yScale);
        //drawSRLabel(sLabel.select("text"), xScale, yScale);

        drawBB(bbGraph, xScale, yScale);
        drawBB(areaBB.select("path"), xScale, yScale);

        drawGSLine(GSLineGraph, xScale, yGSLineScale);
        drawGSLine(gsline, xScale, yGSLineScale);

        var monyong = seriesSvg.selectAll(".series").data(datum, function (d) { return d.date + d.stockID; });
        hilang = monyong.exit().remove();
        bars = monyong.enter().append("g").classed("series", true);

    
        buildCandle(bars, xScale, yScale);
        refreshCandle(monyong, xScale, yScale);

        //alert(datum.length);
        var last = datum[datum.length - 1];
        var stockname = "XX";
        //var formatMonth = d3.timeFormat("%d %b %Y")

        //Set display label
        //var textLabels = seriesSvg.select(".label")
        // .attr("x", 0)
        // .attr("y", 8).text(name + " - " + formatMonth(toDate(last.date)))
        // ///.text( function (d) { return d.date + " " + d.close})
        // .attr("font-family", "sans-serif")
        // .attr("font-size", "12px")
        // .attr("fill", "black");

        //seriesSvg.select(".label2")
        // .attr("x", 0)
        // .attr("y", 22).text("O:" + last.open + " L:" + last.low + " S:" + last.support)
        // ///.text( function (d) { return d.date + " " + d.close})
        // .attr("font-family", "sans-serif")
        // .attr("font-size", "11px")
        // .attr("fill", "black");

        //seriesSvg.select(".label3")
        // .attr("x", 0)
        // .attr("y", 36).text("H:" + last.high + " C:" + last.close + " R:" + last.resistance)
        // ///.text( function (d) { return d.date + " " + d.close})
        // .attr("font-family", "sans-serif")
        // .attr("font-size", "11px")
        // .attr("fill", "black");

        DisplayStockLabel(name,last);

        var volser = seriesVolSvg.selectAll(".series").data(datum, function (d) { return d.date + d.stockID; });
        volser.exit().remove();
        volbars = volser.enter().append("g").classed("series", true);

        

        buildVolume(volbars, xScale, yVolScale);
        refreshVolume(volser, xScale, yVolScale);


        var trendColor = trendColorSvg.selectAll(".trendcolor").data(datum, function (d) { return d.date + d.stockID; });
        trendColor.exit().remove();
        trendColorBars = trendColor.enter().append("g").classed("trendcolor", true).append("rect");
        buildTrendColor(trendColor.select("rect"), xScale);
        buildTrendColor(trendColorBars, xScale);

        //refreshTrendColor(trendColor, xScale);
        //*ga usah dulu ya
        var bwbar = GSLineSvg.selectAll(".bwbar").data(datum, function (d) { return d.date + d.stockID; });
        bwbar.exit().remove();
        var bwbarEnter = bwbar.enter().append("g").classed("bwbar", true).append("path");
        buildBigWave(bwbar.select("path"), xScale, yBigWaveScale);            //refresh
        buildBigWave(bwbarEnter, xScale, yBigWaveScale);  //enter


        var trendBar = trendBarSvg.selectAll(".trendbar").data(datum, function (d) { return d.date + d.stockID; });
        trendBar.exit().remove();
        var trendBarEnter = trendBar.enter().append("g").classed("trendbar", true);
        trendBarEnter.append("rect").classed("trend3", true);
        trendBarEnter.append("rect").classed("trend6", true);
        trendBarEnter.append("rect").classed("trend12", true);
        trendBarEnter.append("rect").classed("trendlow3", true);
        trendBarEnter.append("rect").classed("trendlow6", true);
        trendBarEnter.append("rect").classed("trendlow12", true);

        //trendBarSvg.append("text").classed("label", true);

        buildTrendBar(trendBarEnter, xScale);
        buildTrendBar(trendBarSvg.selectAll(".trendbar"), xScale);


        d3.selectAll(".zoom-pane").call(zoom.transform, d3.zoomIdentity);

        //var r_datum = datum.filter(function (d){if (d.resistance > 0) return d});


        //monyong2.exit().remove();
    }
    
    var oldMaxVol = 0;
    function zoom2() {
 

        var current_data = seriesSvg.selectAll(".series").data();
        var t = d3.event.transform;

        var tx = t.x, ty = t.y, tk = t.k, xt = t.rescaleX(xScale) , xy = t.rescaleY(yScale);

        if (current_data.length > 1)
            recWidth = xt(1) - xt(0) - 1;

        var lastt = xt.range()[1];


        //if(offsetX <0)
        var z = (tk - 1) * -(w - buffer);

        if (tx < z) {
            //console.log("XXXXX " + ((tk-1) * -w) + " " + tx + " " + w);
            //console.log(((tk-1)*-w-tx)/tk);
            d3.selectAll(".zoom-pane").call(zoom.translateBy, ((tk - 1) * -(w - buffer) - tx) / tk, 0);

            return;
        }
        if (tx > 0) {

            d3.selectAll(".zoom-pane").call(zoom.translateBy, -tx / tk, 0);

            return;
        }


        //var offsetX = -1 * Math.floor((tx / (tk * w)) * temp_data.length);
        //var mainoffsetX = Math.floor((w / (tk * w)) * temp_data.length);
        //var newdata = [];

        //for (var i = offsetX; i < offsetX + mainoffsetX; i++) {
        //    newdata.push(toDate(temp_data[i].date));
        //}

        //var tickarray = [];
        //var prevmonth = 0, prevyear = 0;

        //for (var i = 0; i <= newdata.length - 1; i++) {
        //    if ((newdata[i]).getMonth() != prevmonth || (newdata[i]).getFullYear() != prevyear) {
        //        tickarray.push((newdata[i]));
        //        prevmonth = (newdata[i]).getMonth();
        //        prevyear = (newdata[i]).getFullYear();
        //    }
        //}


        //xAX.call(xAxis.scale(xScale).tickValues(tickarray).tickFormat(d3	.timeFormat("%d/%m/%Y")));



        //var temp_arr = SearchByIndex(current_data, Math.ceil(xt.invert(0)), Math.ceil(xt.invert(w)));
        var temp_arr = SearchByIndex(current_data, Math.round(xt.invert(0)), Math.round(xt.invert(w - buffer)));

        var minprice = d3.min(temp_arr, function (d) {
            return d.low;
        });
        var maxprice = d3.max(temp_arr, function (d) {
            return d.high;
        });
        var maxvolume = d3.max(temp_arr, function (d) {
            return d.volume;
        });

        //var lastsupport = temp_arr[temp_arr.length - 1].support;
        //var lastresist = temp_arr[temp_arr.length - 1].resistance;

        //console.log(temp_arr);

        //console.log(lastsupport + ' ' + lastresist);
       


        var yOldVolScale = d3.scaleLinear();



        if (maxvolume != yVolScale.domain()[1]) {
            yOldVolScale.domain(yVolScale.domain()).range([hVol, 0]);

            oldMaxVol = yVolScale.domain()[1];
        }

        //*hitung new height, gunakan yScale yang lama tetapi dimodifikasi high price dan low pricenya

        yAX.call(yAxis.scale(yScale.domain([minprice * 0.9, maxprice * 1.10])));
        yVolAX.call(yVolAxis.scale(yVolScale.domain([0, maxvolume])));



        //xAX.call(xAxis.scale(xt.rangeRound([0, w * tk])));
        xAX.call(xAxis.scale(xt));

        refreshCandle(seriesSvg.selectAll(".series"), xt, yScale);



        //*berhasil
        refreshVolume(seriesVolSvg.selectAll(".series"), xt, yVolScale);

        drawResistance(seriesSvg.selectAll(".lineR").select("path"), xt, yScale);
        drawSupport(seriesSvg.selectAll(".lineS").select("path"), xt, yScale);
        drawBB(seriesSvg.selectAll(".bb").select("path"), xt, yScale);
        drawGSLine(GSLineSvg.selectAll(".gsline").select("path"), xt, yGSLineScale);


        //seriesVolSvg.selectAll(".series").attr("transform", "translate(" + tx + "," + ty + ")scale(" + "1" + ", " + "1" + ")");

        buildTrendColor(trendColorSvg.selectAll(".trendcolor").select("rect"), xt);
        buildTrendBar(trendBarSvg.selectAll(".trendbar"), xt);
        buildBigWave(GSLineSvg.selectAll(".bwbar").select("path"), xt, yBigWaveScale);
        // xScale = xt;
        drawSRLabel(current_data, xt, yScale);

    }
    function dragged() {

        //console.log("drag " + d3.event.x + " " + d3.event.y);
    }
    
    function handleMouseMove(d, i) {
        

        var coords = d3.mouse(this);
        var newXScale = xAxis.scale();


        var x = Math.round(newXScale.invert(coords[0]));

        // Normally we go from data to pixels, but here we're doing pixels to data
        var data = {
            x: Math.round(newXScale.invert(coords[0])),  // Takes the pixel number to convert to number
            y: Math.round(yScale.invert(coords[1]))

        };
        var current_data = seriesSvg.selectAll(".series").data();
        var temp_arr = SearchByIndex(current_data, Math.round(newXScale.invert(0)), Math.round(newXScale.invert(w)));

        var idx = temp_arr.filter(function (d) { return d.index === x });
        var last = idx[0];

        //console.log(xScale.domain());
        //console.log(x + ' ' + toDate(idx.date));

        //console.log(idx[0].date);

        var index = seriesSvg.select(".label").text().indexOf("- ");
        var name = seriesSvg.select(".label").text().substr(0, index);
        //var formatMonth = d3.timeFormat("%d %b %Y") 
        //seriesSvg.select(".label").text(name + " - " + formatMonth(toDate(last.date)))
        //  .attr("x", 0)
        //  .attr("y", 8)
        //  ///.text( function (d) { return d.date + " " + d.close})
        //  .attr("font-family", "sans-serif")
        //  .attr("font-size", "12px")
        //  .attr("fill", "black");

        //seriesSvg.select(".label2")
        // .attr("x", 0)
        // .attr("y", 22).text("O:" + last.open + " L:" + last.low + " S:" + last.support)
        // ///.text( function (d) { return d.date + " " + d.close})
        // .attr("font-family", "sans-serif")
        // .attr("font-size", "11px")
        // .attr("fill", "black");

        //seriesSvg.select(".label3")
        // .attr("x", 0)
        // .attr("y", 36).text("H:" + last.high + " C:" + last.close + " R:" + last.resistance)
        // ///.text( function (d) { return d.date + " " + d.close})
        // .attr("font-family", "sans-serif")
        // .attr("font-size", "11px")
        // .attr("fill", "black");
        DisplayStockLabel(name,last);

    }
    function DisplayStockLabel(namex, lastx)
    {
        var formatMonth = d3.timeFormat("%d/%m/%Y")

        //console.log(lastx);
        seriesSvg.select(".label").text(namex + " - " + formatMonth(toDate(lastx.date)))
          .attr("x", 0)
          .attr("y", 8)
          ///.text( function (d) { return d.date + " " + d.close})
          .attr("font-family", "sans-serif")
          .attr("font-size", "12px")
          .attr("fill", "black");

        seriesSvg.select(".label2")
         .attr("x", 0)
         .attr("y", 22).text("O:" + lastx.open + " L:" + lastx.low + " S:" + lastx.support)
         ///.text( function (d) { return d.date + " " + d.close})
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "black");

        seriesSvg.select(".label3")
         .attr("x", 0)
         .attr("y", 36).text("H:" + lastx.high + " C:" + lastx.close + " R:" + lastx.resistance)

         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "black");
    }

    function DetermineTicks()
    {
 
        if (w < 250)
            xAxis.ticks(2);
        else if (w < 468)
            xAxis.ticks(3);
        else if (w >= 468 && w <= 768) {
            //alert('2 ' + w)
            xAxis.ticks(5);
        }
        else {
            //alert('3 ' + w)
            //xAxis.ticks(d3.timeWeek);
            xAxis.ticks(10);
        }
    }


    function resize() {
        // update width
   

        var current_data = seriesSvg.selectAll(".series").data();
        if (current_data.length > 1)
            recWidth = xScale(1) - xScale(0) - 1;

        //w = parseInt(d3.select('#container').style('width').toString(), 10);
        w = document.getElementById('chart').offsetWidth;

        w = w - margin.left - margin.right;

        DetermineTicks();

        xScale.range([0, w - buffer]);  //IVY
        svg.attr('width', w + margin.left + margin.right);

        //*Resize all SVG
        seriesSvg.attr('width', w - buffer);
        seriesVolSvg.attr('width', w - buffer);
        trendColorSvg.attr('width', w - buffer);
        trendBarSvg.attr('width', w - buffer);
        GSLineSvg.attr('width', w - buffer);

        if (w - buffer == 0)
            alert(w2 + ' ' + $('.chart').width() + ' ' + $('.modal-dialog').width() + ' ' + $(window).width());

        seriesSRLabelSvg.attr('width', buffer);
        srLabelDiv.style('top', (margin.top + 'px'))
            .style('left', (margin.left + w - buffer) + 'px');
        //var srLabelDiv = d3.select('#chart').classed('chart', true).append('div')
        //    .attr('id', 'srlabel-clip')
        //    .style('position', 'absolute')
        //    .style('overflow', 'hidden')
        //    .style('top', (margin.top + 'px'))
        //    .style('left', (margin.left + w - buffer) + 'px');


        d3.selectAll(".zoom-pane").attr('width', w - buffer);
        d3.selectAll("#plotAreaClip rect").attr('width', w - buffer);
        // do the actual resize...
        yAX.attr('transform', 'translate(' + (w ) + ',0)');


        //----------------------------------------------------------------------------------------------------//
        //IVY 20180607
        //logic dibawah ini untuk menghitung ulang berapa bulan tampilan chart disesuaikan dengan lebar layar
        //logic ini ditutup karena permintaan rio per 5 Juni 2018
        //var next_data_num = 0;
        //if (w <= 600)
        //    next_data_num = 30;
        //else if (w > 600 && w <= 800)
        //    next_data_num = 90;
        //else
        //    next_data_num = 180;
        //----------------------------------------------------------------------------------------------------//
        //if (next_data_num != current_data_num) {
          
        //    LoadCSV(save_Stock, next_data_num);
        //    current_data_num = next_data_num;
        //}

        //else {
        //    refreshCandle(seriesSvg.selectAll(".series"), xScale, yScale);
        //    refreshVolume(seriesVolSvg.selectAll(".series"), xScale, yVolScale);

        //    //*Support resistance
        //    drawResistance(seriesSvg.selectAll(".lineR").select("path"), xScale, yScale);
        //    drawSupport(seriesSvg.selectAll(".lineS").select("path"), xScale, yScale);
        //    drawBB(seriesSvg.selectAll(".bb").select("path"), xScale, yScale);
        //    drawGSLine(GSLineSvg.selectAll(".gsline").select("path"), xScale, yGSLineScale);
        //    //*trend color, trend bar

        //    buildTrendColor(trendColorSvg.selectAll(".trendcolor").select("rect"), xScale);
        //    buildTrendBar(trendBarSvg.selectAll(".trendbar"), xScale);

        //}
        //IVY 20180607

        refreshCandle(seriesSvg.selectAll(".series"), xScale, yScale);
        refreshVolume(seriesVolSvg.selectAll(".series"), xScale, yVolScale);

        //*Support resistance
        drawResistance(seriesSvg.selectAll(".lineR").select("path"), xScale, yScale);
        drawSupport(seriesSvg.selectAll(".lineS").select("path"), xScale, yScale);
        drawBB(seriesSvg.selectAll(".bb").select("path"), xScale, yScale);
        drawGSLine(GSLineSvg.selectAll(".gsline").select("path"), xScale, yGSLineScale);
        //*trend color, trend bar

        buildTrendColor(trendColorSvg.selectAll(".trendcolor").select("rect"), xScale);
        buildTrendBar(trendBarSvg.selectAll(".trendbar"), xScale);
    }

    function buildCandle(selection, xScale, yScale) {
        var current_data = seriesSvg.selectAll(".series").data();

        selection.append("path")
        .attr("d", function (d) {
            //alert(d.index + " " + xScale(d.index) + " " + yScale(d.high));
            return line([
        //{ x:  xScale(d.index) + recWidth/2 , y: yScale(d.high) },
        //{ x:  xScale(d.index)  + recWidth/2 , y: yScale(d.low) }
          { x: xScale(d.index), y: yScale(d.high) },
          { x: xScale(d.index), y: yScale(d.low) }
            ]);
        }).
        attr("class", function (d, i) {

            try {

                var classx = "";
                if (d.open > d.close)
                    classx = "highlow highlowline-red";
                else if (d.open < d.close)
                    classx = "highlow highlowline-green";
                else {  //doji
                    //if(selection.data().length >= i-1)
                    if (i > 0) {

                        //if(d.high >= selection.data()[i-1].high)
                        //if (d.high >= current_data[i - 1].high)
                        if (d.close >= current_data[i - 1].close)
                            classx = "highlow highlowline-green";
                        else
                            classx = "highlow highlowline-red";
                        //console.log(selection.data().length + " " + (i-1) + " " + d.high + " " + d.date);
                    }
                    else
                        classx = "highlow highlowline-green";
                }

                return classx;
            }
            catch (e) { alert(e); }

        })
        .attr("stroke", "black").attr("stroke-width", "0.7")
        //.attr("shape-rendering","crispEdges")
        .attr("vector-effect", "non-scaling-stroke");


        selection.append("path")
        .attr("d", function (d) {

            return line([
        { x: xScale(d.index) - recWidth / 2, y: yScale(d.open) },
        { x: xScale(d.index) - recWidth / 2, y: yScale(d.close) },
        { x: xScale(d.index) + recWidth / 2, y: yScale(d.close) },
        { x: xScale(d.index) + recWidth / 2, y: yScale(d.open) }
            ]);
        })
        .attr("class", function (d, i) {

            try {
                var classx;
                if (d.open > d.close)
                    classx = "bar minus";
                else if (d.open < d.close)
                    classx = "bar plus";
                else {
                    //if(selection.data().length >= i-1)

                    if (i > 0) {

                        //if(d.high >= selection.data()[i-1].high) classx = "bar highlowline-green";
                        if (d.high >= current_data[i - 1].high) classx = "bar highlowline-green";
                        else classx = "bar highlowline-red";
                    }
                    else
                        classx = "bar highlowline-green";

                    //return "bar same";
                }


                return classx;
            }
            catch (e) { throw (e); alert(e); }

        });


    };

    function buildTrendColor(selection, xScale) {


        var width = xScale(1) - xScale(0);
        //var width =xscale(xscale.domain[1]) - xscale(xscale.domain[0]) ;


        selection
        //.append('rect')
        .attr('width', function (d) {
            //var xx = xScale(xScale.domain()[1]) - xScale(d.index);
            //if(xx < 0)
            //	alert (xScale(xScale.domain()[1]) + " " + xScale(d.index) + " " + d.index);
            //return xScale(xScale.domain()[1]) - xScale(d.index);
            return width + 0.7;
        })
        .attr('height', htrendColor)
        .attr('x', function (d) { return xScale(d.index) - (width / 2); })
        .attr('y', 0)
        //.attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {

            if (d.close >= d.mA20 && d.mA20 >= d.mA60)
                //return "Blue";
                return "#0083FF";
            else if (d.mA20 >= d.close && d.close >= d.mA60)
                //return "Green";
                return "#009051";
            else if (d.mA20 >= d.mA60 && d.mA60 >= d.close)
                //return "DarkRed";
                //return "IndianRed";
                return "#FF7E79";
            else if (d.mA60 >= d.mA20 && d.mA20 >= d.close)
                //return "IndianRed";
                //return "DarkRed";
                return "#D11D00";
            else if (d.mA60 >= d.close && d.close >= d.mA20)
                return "White";
            else if (d.close >= d.mA60 && d.mA60 >= d.mA20)
                return "#009051";
            else {
                //console.log(d.close + " " + d.MA20 + " " + d.MA60);
                return "Black";
            }
            //else if(d.close >= d.MA20 && d.MA20 >= d.MA60)
            //	return "Blue";
        });


    }

    function buildTrendBar(selection, xScale) {
        //*WARNING : Kalau cuma ada 1 data bagaimana?

        //var width = xScale(1) - xScale(0);
        //var width =xscale(xscale.domain[1]) - xscale(xscale.domain[0]) ;
        //alert(xScale.domain()[1]);




        selection.select(".trend3")
        //.append('rect')
        //.classed('trend3',true)
        .attr('width', recWidth)    
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 35)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {
            if (d.trendHigh >= 1)
                return "#003300";
            else
                return "White";

        });

        selection.select(".trend6")
        //.append('rect').classed('trend6',true)
        .attr('width', recWidth)
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 20)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {
            if (d.trendHigh >= 2)
                return "Green"
            else
                return "White";
        });

        selection.select(".trend12")
        //.append('rect').classed('trend12',true)
        .attr('width', recWidth)
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 5)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('opacity', function (d) {
            if (d.trendHigh >= 3)
                return "1"
            else
                return "0";
        })
        .attr('fill', function (d) {
            if (d.trendHigh >= 3)
                return "YellowGreen"
            else
                return "White";
        });

        selection.select(".trendlow3")
        //.append('rect').classed('trendlow3',true)
        .attr('width', recWidth)
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 52)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {
            if (d.trendLow >= 1)
                return "DarkRed"
            else
                return "White";
        });
        selection.select(".trendlow6")
        //.append('rect').classed('trendlow6',true)
        .attr('width', recWidth)
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 67)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {
            if (d.trendLow >= 2)
                return "Red"
            else
                return "White";
        });
        selection.select(".trendlow12")
        //.append('rect').classed('trendlow12',true)
        .attr('width', recWidth)
        .attr('height', 15)
        .attr('x', function (d) { return xScale(d.index) - recWidth / 2; })
        .attr('y', 82)
        .attr("vector-effect", "non-scaling-stroke")
        .attr('fill', function (d) {
            if (d.trendLow >= 3) {
                return "LightPink"
            }
            else
                return "White";
        });

        trendBarSvg.select(".trendbarline")
            .attr("vector-effect", "non-scaling-stroke")
                 .attr("x1", 0)
                  //.attr("y1", 31)
            .attr("y1", 51)
                    .attr("x2", w)
                    //.attr("y2", 31)
            .attr("y2", 51)
            .attr("stroke-width", 1)
                 .attr("stroke", "black");

        trendBarSvg.select(".trendbarbottomline")
            .attr("vector-effect", "non-scaling-stroke")
                 .attr("x1", 0)
                  //.attr("y1", 31)
            .attr("y1", htrendBar)
                    .attr("x2", w)
                    //.attr("y2", 31)
            .attr("y2", htrendBar)
            .attr("stroke-width", 1)
                 .attr("stroke", "black");


        trendBarSvg.select(".label1")
         .attr("x", 0)
         .attr("y", 10).text("Trend Optimizer")
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "black");


    }


    function buildVolume(selection, xScale, yScale) {

        var current_data = seriesSvg.selectAll(".series").data();
        selection.append("path")
        .attr("d", function (d) {

            return line([
        { x: xScale(d.index) - recWidth / 2, y: yScale(d.volume) },
        { x: xScale(d.index) - recWidth / 2, y: hVol },
        { x: xScale(d.index) + recWidth / 2, y: hVol },
        { x: xScale(d.index) + recWidth / 2, y: yScale(d.volume) }
            ]);
        })
        .attr("class", function (d, i) {

            try {
                var classx;

                if (d.open > d.close)
                    classx = "bar minus-tr";
                else if (d.open < d.close)
                    classx = "bar plus-tr";
                else {
                    //if(selection.data().length >= i-1)
                    if (i > 0) {
                        //if (d.high >= current_data[i - 1].high) classx = "bar plus-tr";
                        if (d.close >= current_data[i - 1].close)
                            classx = "bar plus-tr";
                            //if(d.high >= selection.data()[i-1].high) classx = "bar plus-tr";
                        else classx = "bar minus-tr";
                    }
                    else classx = "bar plus-tr";
                    //return "bar same";
                }

                return classx;

            }
            catch (e) {
                alert(e);
            }

        });


    };


    function drawResistance(selection, xScale, yScale) {

        var line = d3.line()
            .x(function (d) { return xScale(d.index); })
            .y(function (d) { return yScale(d.resistance); });


        selection.attr("d", line);

    }

    function drawSupport(selection, xScale, yScale) {

        var line = d3.line()
            .x(function (d) { return xScale(d.index); })
            .y(function (d) { return yScale(d.support); });

        selection.attr("d", line);
        


    }

    function drawSRLabel(data, xScale, yScale)
    {

        var temp_arr = SearchByIndex(data, Math.round(xScale.invert(0)), Math.round(xScale.invert(w - buffer)));
        var lastsupport = temp_arr[temp_arr.length - 1].support;
        var lastresist = temp_arr[temp_arr.length - 1].resistance;
        var lastindex = temp_arr[temp_arr.length - 1].index;
        //console.log(lastsupport + ' x' + yScale(lastsupport));


        seriesSRLabelSvg.select(".labelS").text("S")
         .attr("x", 0)
         .attr("y", yScale(lastsupport))
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "red");

        seriesSRLabelSvg.select(".labelR").text("R")
         .attr("x", 0)
         .attr("y", yScale(lastresist))
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "Blue");

    }
    function drawBB(selection, xScale, yScale) {

        var area = d3.area()
            .x(function (d) { return xScale(d.index); })
            .y0(function (d) { return yScale(d.bbLower); })
            .y1(function (d) { return yScale(d.bbUpper); });

        selection.attr("d", area);
    }
    function drawGSLine(selection, xScale, yScale) {


        var lineX = d3.line()
            .x(function (d) {
     
                //if (d.gsLine != null) 
                return xScale(d.index);
            })
            .y(function (d) {
                //if (d.gsLine != null)
                return yScale(d.gsLine);
            });
        
        //var s_datum = datum.filter(function (d) { if (d.support > 0) return d });

        selection.attr("d", lineX).attr("stroke", "black");
   
 
        GSLineSvg.select(".label2")
         .attr("x", 0)
         .attr("y", 24).text("GS Line")
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "black");


        //GSLineSvg.select(".gslinecenter")
        //    .attr("x1", 0)
        //    .attr("x2", w).attr("y1", yScale(0))
        //    .attr("y2", yScale(0))
        //    .attr("stroke-width", 1)
        //    .attr("stroke", "black");
    }

    function buildBigWave(selection, xScale, yScale) {

        GSLineSvg.select(".label1")
         .attr("x", 0)
         .attr("y", 12).text("Big Wave Indicator")
         .attr("font-family", "sans-serif")
         .attr("font-size", "11px")
         .attr("fill", "black");


        var current_data = GSLineSvg.selectAll(".bwbar").data();
        selection
        .attr("d", function (d) {
            var bw = 0;
            
            if (d.frequency == 0 || d.frequency == null)
                bw = 0;
            else
                bw = d.volume / (d.frequency * d.frequency * d.frequency);
            

            if (bw == 0)
            {
                return line([
                { x: xScale(d.index) - recWidth / 2, y: hGSLine },
                { x: xScale(d.index) - recWidth / 2, y: hGSLine },
                { x: xScale(d.index) + recWidth / 2, y: hGSLine },
                { x: xScale(d.index) + recWidth / 2, y: hGSLine }
                ]);

            }
            else
            {

                return line([
                { x: xScale(d.index) - recWidth / 2, y: yScale(bw) },
                { x: xScale(d.index) - recWidth / 2, y: hGSLine },
                { x: xScale(d.index) + recWidth / 2, y: hGSLine },
                { x: xScale(d.index) + recWidth / 2, y: yScale(bw) }
                ]);
            }
            
        })
        .attr("class", function (d, i) {

            try {
                var classx;
                
                var bw;
                if (d.frequency == 0 || d.frequency == null)
                    bw = 0;
                else
                    bw = d.volume / (d.frequency * d.frequency * d.frequency);
                //calc yesterday bigwave
                var lastBW;
                if (i > 0) {
                    if (current_data[i - 1].frequency == 0 || current_data[i - 1].frequency == null)
                        lastBW = 0;
                    else
                        lastBW = current_data[i - 1].volume / (current_data[i - 1].frequency * current_data[i - 1].frequency * current_data[i - 1].frequency);
                }
                else
                    lastBW = 0;


                //console.log(bw + ' ' + lastBW);


                if (bw >= lastBW)
                    classx = "bar plus-tr";
                else
                    classx = "bar minus-tr";

                //logik ini ditutup
                //if (d.open > d.close)
                //    classx = "bar minus-tr";
                //else if (d.open < d.close)
                //    classx = "bar plus-tr";
                //else {
                //    //if(selection.data().length >= i-1)
                //    if (i > 0) {
                //        if (d.high >= current_data[i - 1].high) classx = "bar plus-tr";
                //            //if(d.high >= selection.data()[i-1].high) classx = "bar plus-tr";
                //        else classx = "bar minus-tr";
                //    }
                //    else classx = "bar plus-tr";
                //    //return "bar same";
                //}

                return classx;

            }
            catch (e) {
                alert(e + ' ' + i);
            }

        });
    }
    function refreshCandle(selection, xScale, yScale) {


        selection.selectAll(".bar")
            .attr("d", function (d) {

                
                return line([
            { x: xScale(d.index) - recWidth / 2, y: yScale(d.open) },
            { x: xScale(d.index) - recWidth / 2, y: yScale(d.close) },
            { x: xScale(d.index) + recWidth / 2, y: yScale(d.close) },
            { x: xScale(d.index) + recWidth / 2, y: yScale(d.open) }
                ]);
            });

        selection.selectAll(".highlow")
            .attr("d", function (d) {

                return line([
                { x: xScale(d.index), y: yScale(d.high) },
                { x: xScale(d.index), y: yScale(d.low) }
                ]);
            });
        xAX.call(xAxis.scale(xScale));
        //yAX.call(yAxis.scale(yScale));
    }
    function refreshVolume(selection, xScale, yScale) {

        selection.selectAll(".bar")
            .attr("d", function (d) {

                return line([
            { x: xScale(d.index) - recWidth / 2, y: yScale(d.volume) },
            { x: xScale(d.index) - recWidth / 2, y: hVol },
            { x: xScale(d.index) + recWidth / 2, y: hVol },
            { x: xScale(d.index) + recWidth / 2, y: yScale(d.volume) }
                ]);
            });

        xAX.call(xAxis.scale(xScale));
    }


});