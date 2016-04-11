# MazeFramework 

This is a framework for Unity (5.3+) that makes mazes. It also displays them in a limited fashion (debug draw to scene view and debug print to console). 

- !! Turn Gizmos on to see what it displays when you open the MapGen scene !!

I keep having to rewrite this code, so I'm just putting it up here so I don't have to do it anymore.

Two files do most of the heavy lifting:

- DisplayMaze.cs - Creates a maze based on inspector settings and displays it using Debug Scene displays (turn on Gizmos if you want to see it in Game mode, too)

- Maze.cs - This is a non-monobehavior class that makes 2-dimensional mazes (using Dijkstra's algorithm ATM -- may add other methods later). It stores the mazes as a 2-dimensional array of links (integer bitfields) that store whether a given maze node is linked in a direction.  (Up = 1. Right = 2. Down = 4. Left = 8), and any combinations are done by bitwise or-ing (|) them together.

There are a few other files:

- RandomSeed.cs - This is my seeded random number generator that I use in a lot of projects.

- UPoint.cs - This is a Vector2, basically, only it allows you to output the X and Y coords rounded to integer values (ix, and iy). This is useful when dealing with multi-dimensional arrays, so I don't have to cast it every time.