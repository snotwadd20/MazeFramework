using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Useless.MazeMaker
{
    public class NodeData
    {
        public UPoint index = null;
        public int links = Maze.UNVISITED; //-1 means hasn't been checked for links yet
        public NodeData(UPoint _index) { index = _index; }

    }//MapTileData

    public class Maze
    {
        //********************************
        //CONSTANTS AND STATICS
        //********************************
        //For maze generation
        public const int IN_FRONTIERS = -2;
        public const int UNVISITED = -1;
        public const int VISITED = 0; //Must be 0
        public const int UP = 0x1;
        public const int RIGHT = 0x2;
        public const int DOWN = 0x4;
        public const int LEFT = 0x8;

        //public int seed = -1;
        //public RandomSeed r = null;
        public NodeData[,] nodes = null;

        public UPoint startPos = null;

        public int Width = 5;
        public int Height = 5;

        public RandomSeed r = null;
        
        public Maze(int w, int h)
        {
            Width = w;
            Height = h;
        }//Constructor

        //Call this to construct the maze
        public void Make(int seed)
        {
            r = new RandomSeed(seed);
            nodes = new NodeData[Width, Height];

            for (int x = 0; x < nodes.GetLength(0); x++)
                for (int y = 0; y < nodes.GetLength(1); y++)
                    nodes[x, y] = new NodeData(new UPoint(x, y));

            DijkstraMaze();
        }

        private void DijkstraMaze()
        {
            List<UPoint> frontiers = new List<UPoint>();
            //Choose a starting point for maze gen
            UPoint startingNode = new UPoint(r.getIntInRange(0, nodes.GetLength(0) - 1), r.getIntInRange(0, nodes.GetLength(1) - 1));
            nodes[startingNode.ix, startingNode.iy].links = VISITED; //Visited

            startPos = startingNode;

            addUnvisitedToList(startingNode, ref frontiers);
            foreach (UPoint frontier in frontiers)
            {
                nodes[frontier.ix, frontier.iy].links = IN_FRONTIERS;
            }//foreach

            while (frontiers.Count > 0)
            {
                //Choose a frontier at random   
                UPoint frontier = randomPointFromList(ref frontiers);
                nodes[frontier.ix, frontier.iy].links = VISITED; //Visited

                //Choose a neighbor (that is not a frontier) at random
                List<UPoint> nonFrontiersTemp = new List<UPoint>();

                //Find the non-frontier neighbors of this frontier
                addAllVisitedToList(frontier, ref nonFrontiersTemp);

                //Get one at random
                UPoint nonFrontier = nonFrontiersTemp[r.getIntInRange(0, nonFrontiersTemp.Count - 1)];

                //Connect it to the frontier
                linkNeighbors(frontier, nonFrontier);

                //Add all unvisited neighbors of this frontier to the frontiers list
                List<UPoint> unvisitedNeighbors = new List<UPoint>();
                addUnvisitedToList(frontier, ref unvisitedNeighbors);

                for (int i = 0; i < unvisitedNeighbors.Count; i++)
                {
                    frontiers.Add(unvisitedNeighbors[i]);
                    nodes[unvisitedNeighbors[i].ix, unvisitedNeighbors[i].iy].links = IN_FRONTIERS;
                }//for
            }//while
        }//generateMaze

        //-------------------------------
        //Check if a coordinate point is within bounds
        public bool inBounds(UPoint loc, bool dataBounds = false)
        {
            return inBounds(loc.ix, loc.iy, dataBounds);
        }//inBounds
        public bool inBounds(int x, int y, bool dataBounds = false)
        {
            if (dataBounds)
            {
                return (x >= 0 && x < nodes.GetLength(0) &&
                        y >= 0 && y < nodes.GetLength(1));
            }//if


            return (x >= 0 && x < Width &&
                    y >= 0 && y < Height);
        }//inBounds

       

        //********************************
        // FUNCTIONS DEALING WITH LINKS
        //********************************
        //-------------------------------
        // Add all surrounding unvisited link cells to the supplied list
        public int addUnvisitedToList(UPoint center, ref List<UPoint> list)
        {
            int frontiersAdded = 0;

            for (int i = 1; i <= 8; i *= 2)
            {
                UPoint pt = findNeighbor(center, i);
                if (inBounds(pt, true) && nodes[pt.ix, pt.iy].links == UNVISITED) // UP
                {
                    list.Add(pt);
                    frontiersAdded++;
                }//if
            }//for

            return frontiersAdded;
        }//addUnvisitedToList

        // Add all surrounding visited link cells to the supplied list
        public int addAllVisitedToList(UPoint center, ref List<UPoint> list)
        {
            int nonFrontiersAdded = 0;

            for (int i = 1; i <= 8; i *= 2)
            {
                UPoint pt = findNeighbor(center, i);
                if (inBounds(pt, true) && nodes[pt.ix, pt.iy].links >= VISITED) // UP
                {
                    list.Add(pt);
                    nonFrontiersAdded++;
                }//if
            }//for

            return nonFrontiersAdded;
        }//addFrontiers

        //-------------------------------
        //Find a "neighbor" cell in a direction and return coordinates
        public UPoint findNeighbor(UPoint loc, int dir)
        {
            return findNeighbor(loc.ix, loc.iy, dir);
        }//findNeighbor
        public UPoint findNeighbor(int x, int y, int dir)
        {
            switch (dir)
            {
                case UP:
                    y++;
                    break;
                case DOWN:
                    y--;
                    break;
                case LEFT:
                    x--;
                    break;
                case RIGHT:
                    x++;
                    break;
                default:
                    Debug.LogError("MapMaker.cs: THIS IS NOT A DIRECTION: " + dir + " (" + x + "," + y + ")");
                    return new UPoint(-1, -1);
            }//switch

            return new UPoint(x, y);
        }//findNeighbor

        //-------------------------------
        ///Check bitmap to see if a room has a link in that direction
        public bool isLinkedInDir(UPoint loc, int dir)
        {
            return isLinkedInDir(loc.ix, loc.iy, dir);
        }//isLinkedInDir
        public bool isLinkedInDir(int x, int y, int dir)
        {
            return (nodes[x, y].links & dir) > 0;
        }//isLinkedInDir

        //-------------------------------
        //Links both "doors" between adjacent "neighbor" cells
        public void linkNeighbors(UPoint cellA, UPoint cellB)
        {
            //Figure out what direction cellB is from cellA
            //Does not check if cells are actually adjacent
            if (Mathf.Abs(cellA.ix - cellB.ix) != 1 &&
               Mathf.Abs(cellA.iy - cellB.iy) != 1)
            {
                Debug.LogError("MapMaker.cs: CELLS ARE NOT ADJACENT! : (" + cellA.ix + "," + cellA.iy + " -> " + cellB.ix + "," + cellB.iy + ")");
                return;
            }//if

            if (cellB.iy > cellA.iy)
            {
                //Cell B is above Cell A
                nodes[cellA.ix, cellA.iy].links |= UP;
                nodes[cellB.ix, cellB.iy].links |= DOWN;
            }//if
            else if (cellB.iy < cellA.iy)
            {
                //Cell B is below Cell A
                nodes[cellA.ix, cellA.iy].links |= DOWN;
                nodes[cellB.ix, cellB.iy].links |= UP;
            }//else if
            else if (cellB.ix > cellA.ix)
            {
                //Cell B is to the right of Cell A
                nodes[cellA.ix, cellA.iy].links |= RIGHT;
                nodes[cellB.ix, cellB.iy].links |= LEFT;
            }//else if
            else if (cellB.ix < cellA.ix)
            {
                //Cell B is to the left of Cell A
                nodes[cellA.ix, cellA.iy].links |= LEFT;
                nodes[cellB.ix, cellB.iy].links |= RIGHT;
            }//else if
        }//linkNeighbors

        //-------------------------------
        public void unlinkAllNeighbors(UPoint cell)
        {
            //Cell B is above Cell A
            nodes[cell.ix, cell.iy].links &= ~UP;
            nodes[cell.ix, cell.iy].links &= ~DOWN;
            nodes[cell.ix, cell.iy].links &= ~RIGHT;
            nodes[cell.ix, cell.iy].links &= ~LEFT;


            if (cell.y - 1 >= 0)
                nodes[cell.ix, cell.iy - 1].links &= ~UP;

            if (cell.y + 1 < nodes.GetLength(1))
                nodes[cell.ix, cell.iy + 1].links &= ~DOWN;

            if (cell.x - 1 >= 0)
                nodes[cell.ix - 1, cell.iy].links &= ~RIGHT;

            if (cell.x + 1 < nodes.GetLength(0))
                nodes[cell.ix + 1, cell.iy].links &= ~LEFT;
        }//unlinkAllNeighbors

        //-------------------------------
        public void linkAllNeighbors(ref UPoint cell)
        {
            //Cell B is above Cell A
            nodes[cell.ix, cell.iy].links |= UP;
            nodes[cell.ix, cell.iy].links |= DOWN;
            nodes[cell.ix, cell.iy].links |= RIGHT;
            nodes[cell.ix, cell.iy].links |= LEFT;


            if (cell.y - 1 >= 0)
                nodes[cell.ix, cell.iy - 1].links |= UP;

            if (cell.y + 1 < nodes.GetLength(1))
                nodes[cell.ix, cell.iy + 1].links |= DOWN;

            if (cell.x - 1 >= 0)
                nodes[cell.ix - 1, cell.iy].links |= RIGHT;

            if (cell.x + 1 < nodes.GetLength(0))
                nodes[cell.ix + 1, cell.iy].links |= LEFT;
        }//unlinkAllNeighbors

        //-------------------------------
        //Return the number of direction links encoded into a bitmask
        public int numLinks(int bitmask)
        {
            int linksCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if ((bitmask & (1 << i)) != 0)
                {
                    linksCount++;
                }//if
            }//for
            return linksCount;
        }//numLinks

        //-------------------------------
        //Given a direction bitmask, return the bitmask for the opposite wall
        public int oppositeDir(int dir)
        {
            switch (dir)
            {
                case UP:
                    return DOWN;
                case DOWN:
                    return UP;
                case LEFT:
                    return RIGHT;
                case RIGHT:
                    return LEFT;
                default:
                    Debug.LogError("MapMaker.cs: THIS IS NOT A DIRECTION: " + dir);
                    break;
            }//switch
            return -1;
        }//wal

        //-------------------------------
        //Given a list, pops a random point and returns it
        public UPoint randomPointFromList(ref List<UPoint> list)
        {
            if (list.Count <= 0)
            {
                return null;
            }//if

            int index = r.getIntInRange(0, list.Count - 1);

            UPoint popped = list[index];
            list.RemoveAt(index);
            return popped;

        }//randomPointFromList

        //-------------------------------
        //Given a bitmask of directions, choose one and return it. Also update the bitmask
        public int randomDirFromAvailable(ref int availableDirs)
        {
            int adTemp = availableDirs;
            int numDirs = numLinks(adTemp);

            if (numDirs == 0)
            {
                return -1;
            }//if

            int[] dirList = new int[numDirs];

            int nextSpot = 0;
            if ((availableDirs & UP) != 0)
            {
                dirList[nextSpot] = UP;
                nextSpot++;
            }//if
            if ((availableDirs & DOWN) != 0)
            {
                dirList[nextSpot] = DOWN;
                nextSpot++;
            }//if
            if ((availableDirs & LEFT) != 0)
            {
                dirList[nextSpot] = LEFT;
                nextSpot++;
            }//if
            if ((availableDirs & RIGHT) != 0)
            {
                dirList[nextSpot] = RIGHT;
                nextSpot++;
            }//if
            int theDir = dirList[r.getIntInRange(0, numDirs - 1)];
            availableDirs &= ~theDir;
            return theDir;
        }//randomDirFromAvailable

        public override string ToString()
        {
            if (nodes == null)
                return "";

            string str = "Map links - " + r.getSeed() + " : " + nodes.GetLength(0) + "x" + nodes.GetLength(1);
            str += "\n      ";

            for (int x = 0; x < nodes.GetLength(0); x++)
                str += x.ToString("D2") + ", ";

            str += "\n      ----------------------------------------------------------------------------\n";

            for (int y = nodes.GetLength(1) - 1; y >= 0; y--)
            {
                str += y.ToString("D2") + "| ";
                for (int x = 0; x < nodes.GetLength(0); x++)
                    str += nodes[x, y].links.ToString("D2") + ", ";

                str += "\n";
            }//for
            
            return str;
        }//ToString

       
        public bool isFlagged(int bitmask, int flag)
        {
            return (bitmask & flag) != 0;
        }//isFlagged

        public void Sparsify(int numCellsVisited)
        {
            List<UPoint> culDeSacs = null;
            while (numCellsVisited > 0)
            {
                culDeSacs = FindAllDeadEnds();

                while (culDeSacs.Count > 0)
                {
                    unlinkAllNeighbors(randomPointFromList(ref culDeSacs));
                    numCellsVisited--;

                    if (numCellsVisited <= 0)
                        break;
                    //Debug.Log(cell);
                }//foreach

                if (culDeSacs.Count <= 0)
                {
                    break;
                }//if
            }//while
        }//Sparsify

        public List<UPoint> FindAllDeadEnds()
        {
            List<UPoint> culDeSacs = new List<UPoint>();
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                for (int x = 0; x < nodes.GetLength(0); x++)
                {
                    if (numLinks(nodes[x, y].links) == 1)
                    {
                        culDeSacs.Add(new UPoint(x, y));
                    }//if
                }//for
            }//for
            return culDeSacs;
        }//FindAllDeadEnds

        public void Loopify(float amount)
        {
            //GO OVER THE MAP AND RANDOMLY LINK A RANDOM NUMBER OF TILES TO NEIGHBORS
            for (int i = 0; i < Mathf.RoundToInt(nodes.GetLength(0) * amount * 2); i++)
            {
                UPoint cell = new UPoint(r.getIntInRange(0, nodes.GetLength(0) - 1), r.getIntInRange(0, nodes.GetLength(1) - 1));

                //int newLinks = tiles[cell.ix, cell.iy].links;

                for (int dir = 1; dir <= LEFT; dir *= 2)
                {
                    if (r.getRandom() <= amount && inBounds(findNeighbor(cell, dir), true))
                    {
                        linkNeighbors( cell, findNeighbor(cell, dir));
                    }//if
                }//while
            }//for
        }//Loopify

    }//Maze
}//namespace
