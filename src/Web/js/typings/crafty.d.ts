declare module Crafty {

    export interface ICanvas {
        init(): void;
    }
    export var canvas: ICanvas;

    export interface IViewport {
        x: number;
        y: number;
        init(width, height): void;

        follow(target: Object, offsetx: number, offsety: number): void;
    }
    export var viewport: IViewport;

    export interface IStage {
        elem: any;
    }
    export var stage: IStage;

    function scene(sceneName: string, init: Function, uninit?: Function): void;

    function scene(sceneName: string): void;

    function sprite(tile: number, url: string, map: any): void;

    function c(name: string, component: Object): void;

    function e(components: string): any;

    function init(width: number, height: number): void;

    function background(color: string): void;

    function addEvent(ctx: Object, obj: HTMLElement, event: string, callback: Function): void;

    function removeEvent(ctx: Object, obj: HTMLElement, event: string, callback: Function): void;

    function bind(eventName: string, callback: Function): void;

    function unbind(eventName: string, callback: any): void;

    function trigger(eventName: string, data: any): void;
}