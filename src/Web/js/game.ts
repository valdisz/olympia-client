/// <reference path="typings/jquery/jquery.d.ts"/>
/// <reference path="typings/crafty.d.ts"/>

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

            // loading map from server
            $.get('api/test', null, function (data) {
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
        load() {
            for (var i in map.provinces) {
                var province = map.provinces[i];

                var t = Crafty.e('Tile');
                t.Tile(province);
                t.at(province.X, TranslateCoords(province.Y));
            }
        }

        unload() { }
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
        init: function () {
            this.requires('2D, Canvas, Grid, Mouse');
            this.bind('Click', function () {
                alert(this.province.Y + this.province.X);
            });
        },

        Tile: function (province: World.IProvince) {
            this.requires(TileMap(province.Terrain));
            this.province = province;
        },


    };
}

module Game {
    import components = Components
    import scenes = Scenes

    export function start() {
        Crafty.init(6400, 4800);
        Crafty.background('rgb(200, 200, 200)');

        // register components
        Crafty.c('Grid', components.Grid);
        Crafty.c('Tile', components.Tile);

        // register scenes
        scenes.register('Loading', new scenes.Loading());
        scenes.register('Map', new scenes.Map());

        // run scene
        Crafty.scene('Loading');
    }
}