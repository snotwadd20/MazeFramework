using UnityEngine;
using System.Collections;

namespace Useless.MazeMaker
{
    public class DisplayMaze : MonoBehaviour
    {
        public enum MazeDisplayType { Bricks, Tiles, OutlinerDebug, ConsoleDebug } //Tiles and bricks TBD
        public MazeDisplayType displayType = MazeDisplayType.OutlinerDebug;
        public Maze maze = null;
        public int _seed = -1;

        public int sparsenessPasses = 0;
        public float loopinessFactor = 0;

        public int Width = 5;
        public int Height = 5;

        //public bool _DebugDraw = true;
        //public bool _DebugPrint = false;

        public void Start()
        {
            maze = new Maze(Width, Height);

            if (_seed == -1)
                _seed = System.Math.Abs((int)System.DateTime.Now.Ticks);

            maze.Make(_seed);
            maze.Sparsify(sparsenessPasses);
            maze.Loopify(loopinessFactor);

            switch (displayType)
            {
                case MazeDisplayType.OutlinerDebug:
                    break;
                case MazeDisplayType.Bricks:
                    CreateBricks();
                    break;
                case MazeDisplayType.Tiles:
                    break;
                case MazeDisplayType.ConsoleDebug:
                    print(maze.ToString());
                    break;
            }//switch
        }//Init

        private Texturizer t = null;
        public void CreateBricks()
        {
            GameObject _brickPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //_brickPrefab.transform.localScale = transform.localScale;
            Destroy(_brickPrefab.GetComponent<BoxCollider>());

            if (t == null)
                t = new Texturizer(Color.white);

            int scaleX = 2;
            int scaleY = 2;

            int xMin = 1 - scaleX;
            int xMax = scaleX - 1;

            int yMin = 1 - scaleY;
            int yMax = scaleY - 1;

            int xOffset = scaleX + (scaleX - 1);
            int yOffset = scaleY + (scaleY - 1);
            //PlaceAllDoors(scaleX, scaleY);
            for (int y = 0; y < maze.nodes.GetLength(1); y++)
            {
                for (int x = 0; x < maze.nodes.GetLength(0); x++)
                {
                    UPoint startingOffset = new Vector3(x * xOffset, y * yOffset);
                    
                    for (int yOff = yMin; yOff <= yMax; yOff++)
                    {
                        for (int xOff = xMin; xOff <= xMax; xOff++)
                        {
                            int links = maze.nodes[x, y].links;
                            int newLink = 15;

                            if ((yOff == yMin && !maze.isFlagged(links, Maze.DOWN)) ||
                               (yOff == yMax && !maze.isFlagged(links, Maze.UP)) ||
                               (xOff == xMax && !maze.isFlagged(links, Maze.RIGHT)) ||
                               (xOff == xMin && !maze.isFlagged(links, Maze.LEFT)))
                            {
                                newLink = 0;
                            }//if
                            else if (maze.numLinks(links) < 4 && (
                                    (yOff == yMin && xOff == xMin) ||
                                    (yOff == yMax && xOff == xMin) ||
                                    (yOff == yMin && xOff == xMax) ||
                                    (yOff == yMax && xOff == xMax)))
                            {
                                newLink = 0;
                            }
                            else if (yOff == 0 && xOff == 0 && links == 0)
                            {
                                newLink = 0;
                            }//else if

                            UPoint placeAt = startingOffset + new Vector3(xOff, yOff);

                            //UPoint tilesIndex = startingOffset + new UPoint(scaleX - 1, scaleY - 1) + new UPoint(xOff, yOff);

                            t.setupTile(newLink);


                            GameObject tile = t.makeObject();
                            //tile.Initialize(tilesIndex.ix, tilesIndex.iy);
                            //tile.links = newLink;
                            tile.transform.position = placeAt;
                            tile.transform.position += new Vector3(0, 0, 10);

                            tile.transform.parent = transform;
                            tile.name = "Tile [" + x + "," + y + "]";

                            //if (x == 0 && y == 0 && maze.numLinks(links) == 1)
                            //{
                                //tile.tag = "ChestTile";
                            //}//if

                            //tiles[tilesIndex.ix, tilesIndex.iy] = tile;
                        }//for
                    }//for
                }//for
            }//for

            _brickPrefab.SetActive(false);
        }//CreateBricks

        public void Update()
        {
            switch(displayType)
            {
                case MazeDisplayType.OutlinerDebug:
                    DrawDebugLines(transform.position);
                    break;
                case MazeDisplayType.Bricks:
                    break;
                case MazeDisplayType.Tiles:
                    break;
                case MazeDisplayType.ConsoleDebug:
                    break;
            }//switch
        }//Update

        public void DrawDebugLines(UPoint offset)
        {
            UPoint _temp1 = new UPoint(0, 0);
            UPoint _temp2 = new UPoint(0, 0);

            for (int y = maze.nodes.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < maze.nodes.GetLength(0); x++)
                {
                    if (maze.isFlagged(maze.nodes[x, y].links, Maze.UP))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x, y + 1));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.nodes[x, y].links, Maze.RIGHT))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x + 1, y));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.nodes[x, y].links, Maze.DOWN))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x, y - 1));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.nodes[x, y].links, Maze.LEFT))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x - 1, y));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if
                }//for
            }//for
        }//DrawDebugLines
    }//DisplayMaze
}//namespace
