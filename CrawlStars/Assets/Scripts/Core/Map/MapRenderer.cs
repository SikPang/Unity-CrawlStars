using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Utility;

namespace Core.Map {
    public class MapRenderer : MonoBehaviour {
        [SerializeField] private Transform mapRoot;

        private List<Tile> loadedTiles = new List<Tile>();

        public const float TileScale = 1.2f;

        public void Generate(MapLoader.MapType mapType) {
            var mapData = MapLoader.LoadMapFile(mapType);
            if (mapData == null) return;

            Vector2 startPos = new Vector2(
                -TileScale * (mapData.width - 1) * 0.5f,
                TileScale * (mapData.height - 1) * 0.5f
            );
            for (int y = 0; y < mapData.height; ++y) {
                for (int x = 0; x < mapData.width; ++x) {
                    var obj = ObjectPooling.Instance.Get<Tile>("Tile", mapRoot);
                    if (obj == null) return;

                    var tileType = mapData.map[y][x] == 1 ? Tile.TileType.Wall : Tile.TileType.Ground;
                    obj.Initialize(tileType, TileScale);
                    obj.transform.localPosition = startPos + new Vector2(x, -y) * TileScale;
                    loadedTiles.Add(obj);
                }
            }
        }

        public void Clear() {
            foreach (var tile in loadedTiles) {
                ObjectPooling.Instance.TryAbandon("Tile", tile.gameObject);
            }
            loadedTiles.Clear();
        }
    }
}