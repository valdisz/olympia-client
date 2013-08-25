/// <reference path="crafty.d.ts"/>

module World {
    export var Facts = {
        tileSize: 16
    };
}

module Scenes {
    import facts = World.Facts

    export interface IScene {
        load(): void;
        unload(): void;
    }

    export function register(name: string, scene: IScene) {
        Crafty.scene(name, scene.load, scene.unload);
    }

    export class Loading implements IScene {
        load() {
            Crafty.sprite(facts.tileSize, 'assets/tiles.png', {
                grass: [2, 1],
            });

            Crafty.sprite(facts.tileSize, 'assets/dessert.png', {
                dessert: [0, 2],
            });

            // when assets are loaded, switch to Map view
            Crafty.scene('Map');
        }

        unload() {
        }
    }

    export class Map implements IScene {
        load() {
            // draw random map 10x10
            for (var x = 0; x < 10; x++) {
                for (var y = 0; y < 10; y++) {
                    var tile = Math.random() >= 0.5 ? 'Plain' : 'Dessert';
                    Crafty.e(tile).at(x, y);
                }
            }
        }

        unload() { }
    }
}

module Components {
    import facts = World.Facts

    function tileFactory(tile: string) {
        return {
            init: function() {
                this.requires('2D, Canvas, Grid, ' + tile);
            }
        }
    }

    export var Grid =  {
        init: function() {
            this.attr({
                w: facts.tileSize,
                h: facts.tileSize
            });
        },

        at: function(x: number, y: number) {
            if (x == undefined && y == undefined) {
                return {
                    x: this.x / facts.tileSize,
                    y: this.y / facts.tileSize
                };
            }
            else {
                this.attr({
                    x: x * facts.tileSize,
                    y: y * facts.tileSize
                });

                return this;
            }
        }
    }

    export var Tiles = {
        Plain: tileFactory('grass'),
        Dessert: tileFactory('dessert')
    }
}

module Game {
    import components = Components
    import scenes = Scenes

    export function start() {
        Crafty.init(640, 480);
        Crafty.background('rgb(87, 109, 20)');

        // register components
        Crafty.c('Grid', components.Grid);
        var allTiles = components.Tiles;
        for (var p in components.Tiles) if (allTiles.hasOwnProperty(p)) {
            Crafty.c(p, allTiles[p]);
        }

        // register scenes
        scenes.register('Loading', new scenes.Loading());
        scenes.register('Map', new scenes.Map());

        // run scene
        Crafty.scene('Loading');
    }
}