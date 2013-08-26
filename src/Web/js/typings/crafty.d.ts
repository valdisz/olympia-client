declare module Crafty {

    function scene(sceneName: string, init: Function, uninit?: Function): void;
    function scene(sceneName: string): void;

    function sprite(tile: number, url: string, map: any): void;

    function c(name: string, component: Object): void;
    function e(components: string): any;

    function init(width: number, height: number): void;
    function background(color: string): void;
}