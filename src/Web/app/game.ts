/// <reference path="../js/typings/jquery/jquery.d.ts"/>
/// <reference path="../js/typings/crafty.d.ts"/>

module World {
    export interface IProvince {
        Terrain: string;
        X: number;
        Y: string;
    }

    export class Map {
        provinces: IProvince[];
    }

    export var spriteSize = 32;
    export var tileSize = spriteSize;

    export var map = new Map();
}

module Scenes {
    import map = World.map;

    export interface IScene {
        load(): void;
        unload(): void;
    }

    export function register(name: string, scene: IScene) {
        Crafty.scene(name, scene.load, scene.unload);
    }

    export class Loading implements IScene {
        load() {
            Crafty.sprite(World.spriteSize, 'assets/tiles.png', {
                tl_grass: [1, 2],
                tl_ocean: [1, 20],
                tl_forest: [6, 59]
            });

            Crafty.sprite(World.spriteSize, 'assets/desert.png', {
                tl_desert: [1, 2],
            });

            Crafty.sprite(World.spriteSize, 'assets/shallows.png', {
                tl_shallows: [0, 0],
            });

            Crafty.sprite(World.spriteSize, 'assets/swamp.png', {
                tl_swamp: [0, 0],
            });

            Crafty.sprite(World.spriteSize, 'assets/mountains.png', {
                tl_mountain: [0, 0],
            });

            Crafty.sprite(World.spriteSize, 'assets/unknown.png', {
                tl_unknown: [0, 0],
            });

            Crafty.sprite(World.spriteSize, 'assets/unknown.png', {
                tl_unknown: [0, 0],
            });

            Crafty.sprite(World.spriteSize, 'assets/select.png', {
                tl_highlight: [0, 0],
                tl_select: [1, 0],
            });

            // loading map from server
            $.get('api/world', null, function (data) {
                map.provinces = data;
                Crafty.scene('Map');
            });
        }

        unload() {
        }
    }

    var row = 'abcdfghjkmnpqrstvwxz';
    var rowLookup = {};
    for (var i = 0; i < row.length; i++) {
        rowLookup[row[i]] = i;
    }
    function TranslateCoords (dim: string) {
        var c1 = rowLookup[dim.charAt(0)];
        var c2 = rowLookup[dim.charAt(1)];
    
        return c1 * row.length + c2;
    };

    export class Map implements IScene {
        sel: any;
        selCallback: Function;

        load() {
            var lastProvince;
            for (var i in map.provinces) {
                var province = map.provinces[i];

                lastProvince = Crafty.e('Tile')
                    .forProvince(province)
                    .at(province.X, TranslateCoords(province.Y));
            }
            Crafty.viewport.follow(lastProvince, 0, 0);
            
            this.selCallback = function (province) {
                this.sel = this.sel || Crafty.e('2D, Canvas, Grid, tl_select');
                this.sel.at(province.X, TranslateCoords(province.Y));
            };
            Crafty.bind('SelectProvince', this.selCallback);
        }

        unload() {
            Crafty.unbind('SelectProvince', this.selCallback);

            if (this.sel) {
                this.sel.destroy();
                this.sel = null;
            }
        }
    }
}

module Components {

    export var Grid = {
        init: function () {
            this.attr({
                w: World.tileSize * 3,
                h: World.tileSize * 3
            });
        },

        at: function (x: number, y: number) {
            if (x == undefined && y == undefined) {
                return {
                    x: this.x / World.tileSize,
                    y: this.y / World.tileSize
                };
            }
            else {
                this.attr({
                    x: x * World.tileSize,
                    y: y * World.tileSize
                });

                return this;
            }
        }
    };

    var _tileMap = {
        'Plain': 'tl_grass',
        'Desert': 'tl_desert',
        'Forest': 'tl_forest',
        'Ocean': 'tl_shallows',
        'Swamp': 'tl_swamp',
        'Mountain': 'tl_mountain'
    };

    export function TileMap (terrain: string) {
        return _tileMap[terrain] || 'tl_unknown';
    }

    export var Tile = {
        forProvince: function (province: World.IProvince) {
            this.requires(TileMap(province.Terrain));
            this.province = province;

            return this;
        },

        init: function () {
            this.requires('2D, Canvas, Grid, Mouse');

            this.bind('Click', function (e) {
                Crafty.trigger('SelectProvince', this.province);
            });
        }
    };
}

module Game {
    import components = Components
    import scenes = Scenes

    export function start(w, h) {
        Crafty.init(w, h);
        Crafty.canvas.init();
        Crafty.background('rgb(200, 200, 200)');

        // register components
        Crafty.c('Grid', components.Grid);
        Crafty.c('Tile', components.Tile);

        // register scenes
        scenes.register('Loading', new scenes.Loading());
        scenes.register('Map', new scenes.Map());

        // configure stage to be panable
        Crafty.addEvent(this, Crafty.stage.elem, "mousedown", function (e) {
            if (e.button !== 2) return;
            var base = { x: e.clientX, y: e.clientY };

            function scroll(e) {
                var dx = base.x - e.clientX,
                    dy = base.y - e.clientY;
                base = { x: e.clientX, y: e.clientY };
                Crafty.viewport.x -= dx;
                Crafty.viewport.y -= dy;
            };

            Crafty.addEvent(this, Crafty.stage.elem, "mousemove", scroll);
            Crafty.addEvent(this, Crafty.stage.elem, "mouseup", function () {
                Crafty.removeEvent(this, Crafty.stage.elem, "mousemove", scroll);
            });
        });

        // run scene
        Crafty.scene('Loading');
    }
}