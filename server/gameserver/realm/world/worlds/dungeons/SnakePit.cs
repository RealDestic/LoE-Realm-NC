﻿namespace LoESoft.GameServer.realm.world
{
    public class SnakePit : World
    {
        public SnakePit()
        {
            Name = "Snake Pit";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
        }

        protected override void Init() => LoadMap("snakepit", MapType.Wmap);
    }
}