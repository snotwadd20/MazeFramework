using UnityEngine;
using System.Collections;

namespace Useless.MazeMaker
{
    public class DisplayMaze : MonoBehaviour
    {
        public Maze maze = null;
        public int _seed = -1;

        public int Width = 5;
        public int Height = 5;

        public bool _DebugDraw = true;
        public bool _DebugPrint = false;

        public void Start()
        {
            maze = new Maze(Width, Height);

            if (_seed == -1)
                _seed = System.Math.Abs((int)System.DateTime.Now.Ticks);

            maze.Make(_seed);

            if (_DebugPrint)
                print(maze.ToString());
        }//Init

        public void Update()
        {
            if (_DebugDraw)
                DrawDebugLines(transform.position);
        }//Update

        public void DrawDebugLines(UPoint offset)
        {
            UPoint _temp1 = new UPoint(0, 0);
            UPoint _temp2 = new UPoint(0, 0);

            for (int y = maze.tiles.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < maze.tiles.GetLength(0); x++)
                {
                    if (maze.isFlagged(maze.tiles[x, y].links, Maze.UP))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x, y + 1));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.tiles[x, y].links, Maze.RIGHT))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x + 1, y));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.tiles[x, y].links, Maze.DOWN))
                    {
                        _temp1 = offset + (new UPoint(x, y));
                        _temp2 = offset + (new UPoint(x, y - 1));
                        Debug.DrawLine(_temp1, _temp2);
                    }//if

                    if (maze.isFlagged(maze.tiles[x, y].links, Maze.LEFT))
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
