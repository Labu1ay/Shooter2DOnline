import { Room, Client, Delayed } from "colyseus";
import { Schema, type, MapSchema, ArraySchema } from "@colyseus/schema";

export class Vector2Float extends Schema {
    @type("uint32") id = 0;
    @type("number") cX = Math.floor(Math.random() * 20) - 10;
    @type("number") cZ = Math.floor(Math.random() * 14) - 7;
}

export class Player extends Schema {
    @type("number") pX = Math.floor(Math.random() * 20) - 10;
    @type("number") pY = 0;
    @type("number") pZ = Math.floor(Math.random() * 14) - 7;
    @type("number") vX = 0;
    @type("number") vY = 0;
    @type("number") vZ = 0;
    @type("number") rY = 0;
    @type("int8") maxHP = 0;
    @type("int8") currentHP = 0;
    @type("uint16") score = 0;

}

export class State extends Schema {
    @type({ map: Player }) players = new MapSchema<Player>();
    @type([Vector2Float]) coins = new ArraySchema<Vector2Float>();
    coinLastId = 0;

    createCoins(){
        const coin = new Vector2Float();
        coin.id = this.coinLastId++;
        this.coins.push(coin); 
    }

    collectCoin(player: Player, data: any){
        const coin = this.coins.find((value) => value.id === data.id);
        if(coin === undefined) return;

        coin.cX = Math.floor(Math.random() * 20) - 10;
        coin.cZ = Math.floor(Math.random() * 14) - 7;

        player.score++;
        
    }

    createPlayer(sessionId: string, data: any) {
        const player = new Player();
        player.maxHP = data.hp;
        player.currentHP = data.hp;
        this.players.set(sessionId, player);
    }

    removePlayer(sessionId: string) {
        this.players.delete(sessionId);
    }

    movePlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);
        player.pX = data.pX;
        player.pY = data.pY;
        player.pZ = data.pZ;
        player.vX = data.vX;
        player.vY = data.vY;
        player.vZ = data.vZ;
        player.rY = data.rY;        
    }
}

export class StateHandlerRoom extends Room<State> {
    maxClients = 2;
    startCoinCount = 3;
    gameIsStarted: boolean = false;
    awaitStart: Delayed;

    onCreate () {
        this.setState(new State());

        this.onMessage("collect", (client, data) => {
            const player = this.state.players.get(client.sessionId);
            this.state.collectCoin(player, data);
        });

        this.onMessage("move", (client, data) => {
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("shoot", (client, data) =>{
            this.broadcast("Shoot", data, { except: client });
        });

        this.onMessage("damage", (client, data) => {
            const clientID = data.id;
            const player = this.state.players.get(clientID);

            let hp = player.currentHP - data.value;
            if(hp >= 0) {
                player.currentHP = hp;
                return;
            }
        });

        this.onMessage("leave", (client, data) =>{
            this.broadcast("Leave", data, { except: client });
        });

    }



    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, data: any) {
        this.state.createPlayer(client.sessionId, data);

        if(this.clients.length < 2) return;

        this.broadcast("GetReady");
        this.awaitStart = this.clock.setTimeout(() => {
            try {
                this.broadcast("Start");

                for (let i = 0; i < this.startCoinCount; i++) {
                    this.state.createCoins();
                }

                this.gameIsStarted = true;
            } catch (error) {
                this.broadcast("CancelStart");
            }
        }, 1000);
    }

    onLeave (client) {
        if(this.gameIsStarted === false && this.awaitStart !== undefined && this.awaitStart.active){
            this.broadcast("CancelStart");
            this.awaitStart.clear(); 
        }

        this.state.removePlayer(client.sessionId);
    }

    onDispose () {

    }

}
