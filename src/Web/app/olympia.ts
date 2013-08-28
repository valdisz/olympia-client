/// <reference path="../js/typings/crafty.d.ts"/>
/// <reference path="../js/typings/jquery/jquery.d.ts"/>

declare var angular;

module Olympia {
    import map = World.map;

    angular.module('Olympia', [])
        .controller('main', function ($scope, $http) {
            $scope.upload = function () {
                $('#fileUploadForm input').click();
            };

            var loadWorld = function () {
                $http.get('api/world').success(function (data) {
                    $scope.provinces = data.Provinces;
                    $scope.nobles = data.Nobles;

                    map.provinces = data.Provinces;

                    Crafty.scene('Map');
                });
            };

            $scope.uploadReport = function () {
                $http({
                    method: 'POST',
                    url: "api/upload",
                    headers: { 'Content-Type': false },
                    transformRequest: function (data) {
                        var formData = new FormData();
                        formData.append("file", data.file);
                        return formData;
                    },
                    data: { file: $scope.reportFile }
                })
                .success(function (data, status, headers, config) {
                    loadWorld();
                })
                .error(function (data, status, headers, config) {
                    alert("failed!");
                });
            };

            $scope.select = function (noble) {
                $scope.selected = noble;
            };

            $scope.addOrder = function (com) {
                if (!$scope.selected) {
                    return;
                }

                if (!$scope.selected.orders) {
                    $scope.selected.orders = [];
                }

                $scope.selected.orders.push({ com: com.name, time: com.time });
            };

            $scope.factionOrders = [
                { name: 'Admit', time: 0 },
                { name: 'Default', time: 0 },
                { name: 'Defend', time: 0 },
                { name: 'Hostile', time: 0 }
            ];

            $scope.commands = [
                { name: 'Accept', time: 0 },
                { name: 'Attack', time: 1 },
                { name: 'Banner', time: 0 },
                { name: 'Behind', time: 0 },
                { name: 'Board', time: 0 },
                { name: 'Breed', time: 0 },
                { name: 'Bribe', time: 7 },
                { name: 'Build', time: 'depends' },
                { name: 'Buy', time: 0 },
                { name: 'Claim', time: 0 },
                { name: 'Collect', time: 'depends' },
                { name: 'Decree', time: 0 },
                { name: 'Die', time: 0 },
                { name: 'Drop', time: 0 },
                { name: 'Execute', time: 0 },
                { name: 'Explore', time: 7 },
                { name: 'Fee', time: 0 },
                { name: 'Ferry', time: 0 },
                { name: 'Flag', time: 0 },
                { name: 'Fly', time: 'depends' },
                { name: 'Forget', time: 0 },
                { name: 'Form', time: 7 },
                { name: 'Garrison', time: 1 },
                { name: 'Give', time: 0 },
                { name: 'Guard', time: 0 },
                { name: 'Honor', time: 1 },
                { name: 'Improve', time: 'depends' },
                { name: 'Make', time: 'depends' },
                { name: 'Message', time: 1 },
                { name: 'Move', time: 'depends' },
                { name: 'Name', time: 0 },
                { name: 'Neutral', time: 0 },
                { name: 'Oath', time: 1 },
                { name: 'Pillage', time: 7 },
                { name: 'Pledge', time: 0 },
                { name: 'Promote', time: 0 },
                { name: 'Quest', time: 7 },
                { name: 'Raze', time: 'depends' },
                { name: 'Repair', time: 'depends' },
                { name: 'Research', time: 7 },
                { name: 'Sail', time: 'depends' },
                { name: 'Seek', time: 7 },
                { name: 'Sell', time: 0 },
                { name: 'Stack', time: 0 },
                { name: 'Stop', time: 0 },
                { name: 'Study', time: 7 },
                { name: 'Surrender', time: 0 },
                { name: 'Terrorize', time: 7 },
                { name: 'Train', time: 'depends' },
                { name: 'Ungarrison', time: 1 },
                { name: 'Unload', time: 0 },
                { name: 'Unstack', time: 0 },
                { name: 'Use', time: 'depends' },
                { name: 'Wait', time: 'depends' }
            ];

            // run
            var h = $('#beforeMap').height();
            Game.start($('#beforeMap div').width(), $(window).height() - 70 - h);

            Crafty.scene('Loading');

            loadWorld();
        })

        .directive('fileInput', function ($parse) {
            return {
                restrict: 'EA',
                template: '<input type="file" />',
                replace: true,
                link: function (scope, element, attrs) {

                    var modelGet = $parse(attrs.fileInput);
                    var modelSet = modelGet.assign;
                    var onChange = $parse(attrs.onChange);

                    var updateModel = function () {
                        scope.$apply(function () {
                            modelSet(scope, element[0].files[0]);
                            onChange(scope);
                        });
                    };

                    element.bind('change', updateModel);
                }
            };
        })

        .directive('craftyCanvas', function () {
        return {
            restrict: 'E',
            replace: true,
            template: '<div id="cr-stage"></div>',
            link: function (scope, element, attrs) {
                window.onresize = function () {
                    var h = $('#beforeMap').height();
                    Crafty.viewport.init(element.parent().width(), $(window).height() - 70 - h);
                };
            }
        };
    });
}
