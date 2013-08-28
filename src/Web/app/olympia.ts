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
                    alert("success!");

                    $http.get('api/world').success(function (data) {
                        map.provinces = data;
                        Crafty.scene('Map');
                    });
                })
                .error(function (data, status, headers, config) {
                    alert("failed!");
                });
            };
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
                window.addEventListener('load', function() {
                    Game.start(element.parent().width(), $(window).height() - 50);
                });

                window.onresize = function () {
                    Crafty.viewport.init(element.parent().width(), $(window).height() - 50);
                };
            }
        };
    });
}