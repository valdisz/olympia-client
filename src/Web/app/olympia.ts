/// <reference path="../js/typings/crafty.d.ts"/>
/// <reference path="../js/typings/jquery/jquery.d.ts"/>

declare var angular;

module Olympia {
    angular.module('Olympia', [])
    .controller('main', function() { })
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
                    console.log('resize');
                };
            }
        };
    });
}