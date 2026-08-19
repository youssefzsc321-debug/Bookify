var chart = null;


$(document).ready(function () {

    var dateRangeElement = $('[data-kt-daterangepicker="true"]');

    dateRangeElement.on('apply.daterangepicker', function (ev, picker) {

        var startDate = picker.startDate.format('YYYY-MM-DD');
        var endDate = picker.endDate.format('YYYY-MM-DD');

        drawRentalsChart(startDate, endDate);
    });


    drawRentalsChart();

    drawSubscribersChart();

});



function drawRentalsChart(startDate = null, endDate = null) {

    var element = document.getElementById('RentalsPerDay');

    if (!element) {
        console.error("RentalsPerDay element not found.");
        return;
    }


    // Destroy old chart
    if (chart) {

        chart.destroy();
        chart = null;

    }


    var height = parseInt(KTUtil.css(element, 'height'));

    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-info');
    var lightColor = KTUtil.getCssVariableValue('--kt-info-light');


    var url = '/Dashboard/GetRentalPerDay';


    if (startDate && endDate) {

        url += `?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}`;

    }


    console.log("Request URL:", url);


    $.get({

        url: url,

        success: function (data) {

            console.log("New Chart Data:", data);


            var values = data.map(function (item) {

                return Number(item.value);

            });


            var categories = data.map(function (item) {

                return item.label;

            });


            var maxValue = Math.max(...values, 1);


            var options = {

                series: [{
                    name: 'Books',
                    data: values
                }],


                chart: {

                    fontFamily: 'inherit',

                    type: 'area',

                    height: height,

                    toolbar: {
                        show: false
                    }

                },


                legend: {
                    show: false
                },


                dataLabels: {
                    enabled: false
                },


                fill: {

                    type: 'solid',

                    opacity: 1

                },


                stroke: {

                    curve: 'smooth',

                    show: true,

                    width: 3,

                    colors: [baseColor]

                },


                xaxis: {

                    categories: categories,

                    axisBorder: {
                        show: false
                    },

                    axisTicks: {
                        show: false
                    },

                    labels: {

                        style: {

                            colors: labelColor,

                            fontSize: '12px'

                        }

                    },


                    crosshairs: {

                        position: 'front',

                        stroke: {

                            color: baseColor,

                            width: 1,

                            dashArray: 3

                        }

                    }

                },


                yaxis: {

                    min: 0,

                    tickAmount: maxValue,

                    labels: {

                        style: {

                            colors: labelColor,

                            fontSize: '12px'

                        }

                    }

                },


                states: {

                    normal: {

                        filter: {

                            type: 'none',

                            value: 0

                        }

                    },


                    hover: {

                        filter: {

                            type: 'none',

                            value: 0

                        }

                    },


                    active: {

                        allowMultipleDataPointsSelection: false,

                        filter: {

                            type: 'none',

                            value: 0

                        }

                    }

                },


                tooltip: {

                    style: {

                        fontSize: '12px'

                    }

                },


                colors: [lightColor],


                grid: {

                    borderColor: borderColor,

                    strokeDashArray: 4,

                    yaxis: {

                        lines: {

                            show: true

                        }

                    }

                },


                markers: {

                    strokeColor: baseColor,

                    strokeWidth: 3

                }

            };


            chart = new ApexCharts(element, options);


            chart.render()

                .then(function () {

                    console.log("Rentals chart rendered successfully.");

                })

                .catch(function (error) {

                    console.error("ApexCharts error:", error);

                });

        },


        error: function (xhr) {

            console.error("GetRentalPerDay Error");

            console.error(xhr.status);

            console.error(xhr.responseText);

        }

    });

}




function drawSubscribersChart() {

    var canvas = document.getElementById('SubscribersPerCity');

    if (!canvas) {

        console.error("SubscribersPerCity element not found.");

        return;

    }


    $.get({

        url: '/Dashboard/GetSubscribersPerCity',

        success: function (data) {

            console.log("Subscribers Data:", data);


            var labels = data.map(function (item) {

                return item.cityName;

            });


            var values = data.map(function (item) {

                return Number(item.count);

            });


            var primaryColor =
                KTUtil.getCssVariableValue('--kt-primary');

            var dangerColor =
                KTUtil.getCssVariableValue('--kt-danger');

            var successColor =
                KTUtil.getCssVariableValue('--kt-success');

            var warningColor =
                KTUtil.getCssVariableValue('--kt-warning');

            var infoColor =
                KTUtil.getCssVariableValue('--kt-info');


            var chartData = {

                labels: labels,

                datasets: [{

                    data: values,

                    backgroundColor: [

                        infoColor,

                        successColor,

                        warningColor,

                        primaryColor,

                        dangerColor,

                        '#5F91B6',

                        '#D3F6FC',

                        '#C8B0D2'

                    ],

                    borderRadius: 8

                }]

            };


            var config = {

                type: 'doughnut',

                data: chartData,

                options: {

                    responsive: true,

                    plugins: {

                        legend: {

                            position: 'bottom'

                        },

                        title: {

                            display: false

                        }

                    }

                }

            };


            new Chart(canvas, config);

        },


        error: function (xhr) {

            console.error("GetSubscribersPerCity Error");

            console.error(xhr.status);

            console.error(xhr.responseText);

        }

    });

}