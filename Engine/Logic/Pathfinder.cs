using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Logic
{

  public class Pathfinder
  {
    /// <summary>
    /// Characters that we can walk on
    /// </summary>
    private string _walkableCharacters = "0";

    public List<Point> Find(char[,] map, Point start, Point end)
    {
      ValidatePositions(map, start, end);

      // nodes that have already been analyzed and have a path from the start to them
      var closedSet = new List<Point>();

      // nodes that have been identified as a neighbor of an analyzed node, but have 
      // yet to be fully analyzed
      var openSet = new List<Point> { start };

      // a dictionary identifying the optimal origin point to each node. this is used 
      // to back-track from the end to find the optimal path
      var cameFrom = new Dictionary<Point, Point>();

      // a dictionary indicating how far each analyzed node is from the start
      var currentDistance = new Dictionary<Point, int>();

      // a dictionary indicating how far it is expected to reach the end, if the path 
      // travels through the specified node. 
      var predictedDistance = new Dictionary<Point, float>();

      // initialize the start node as having a distance of 0, and an estmated distance 
      // of y-distance + x-distance, which is the optimal path in a square grid that 
      // doesn't allow for diagonal movement
      currentDistance.Add(start, 0);
      predictedDistance.Add(
          start, Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y)
      );

      // if there are any unanalyzed nodes, process them
      while (openSet.Count > 0)
      {
        // get the node with the lowest estimated cost to finish
        var current = (
            from p in openSet orderby predictedDistance[p] ascending select p
        ).First();

        // if it is the finish, return the path
        if (current.X == end.X && current.Y == end.Y)
        {
          // generate the found path
          return ReconstructPath(cameFrom, end);
        }

        // move current node from open to closed
        openSet.Remove(current);
        closedSet.Add(current);

        // process each valid node around the current node
        var neighbours = GetNeighbourNodes(map, current);

        foreach (var neighbor in neighbours)
        {
          var tempCurrentDistance = currentDistance[current] + 1;

          // if we already know a faster way to this neighbor, use that route and 
          // ignore this one
          if (closedSet.Contains(neighbor)
              && tempCurrentDistance >= currentDistance[neighbor])
          {
            continue;
          }

          // if we don't know a route to this neighbor, or if this is faster, 
          // store this route
          if (!closedSet.Contains(neighbor)
              || tempCurrentDistance < currentDistance[neighbor])
          {
            if (cameFrom.Keys.Contains(neighbor))
            {
              cameFrom[neighbor] = current;
            }
            else
            {
              cameFrom.Add(neighbor, current);
            }

            currentDistance[neighbor] = tempCurrentDistance;
            predictedDistance[neighbor] =
                currentDistance[neighbor]
                + Math.Abs(neighbor.X - end.X)
                + Math.Abs(neighbor.Y - end.Y);

            // if this is a new node, add it to processing
            if (!openSet.Contains(neighbor))
            {
              openSet.Add(neighbor);
            }
          }
        }
      }

      // unable to figure out a path, abort.
      throw new Exception(
          string.Format(
              "unable to find a path between ({0},{1}) and ({2},{3})",
              start.X, start.Y,
              end.X, end.Y
          )
      );
    }

    /// <summary>
    /// Check if the positions are on the map, and aren't on an 'unwalkable' character
    /// </summary>
    /// <param name="map">What we need to navigate through</param>
    /// <param name="start">Where we start</param>
    /// <param name="end">Where we end</param>
    private void ValidatePositions(char[,] map, Point start, Point end)
    {
      if (start.X < 0)
        throw new Exception("Start.X isn't on map: " + start.X);
      if (start.X >= map.GetLength(1))
        throw new Exception("Start.Y isn't on map: " + start.Y);
      if (end.X < 0)
        throw new Exception("End.X isn't on map: " + end.X);
      if (end.X >= map.GetLength(0))
        throw new Exception("End.Y isn't on map: " + end.Y);

      var startCharacter = map[start.Y, start.X];
      var endCharacter = map[end.Y, end.X];

      if (!_walkableCharacters.Contains(startCharacter))
        throw new Exception(string.Format("Start position ({0}) is on 'unwalkable' character: {1}", start.ToString(), startCharacter));

      if (!_walkableCharacters.Contains(endCharacter))
        throw new Exception(string.Format("Start position ({0}) is on 'unwalkable' character: {1}", end.ToString(), endCharacter));
    }

    /// <summary>
    /// Return a list of accessible nodes neighboring a specified node
    /// </summary>
    /// <param name="node">The center node to be analyzed.</param>
    /// <returns>A list of nodes neighboring the node that are accessible.</returns>
    private IEnumerable<Point> GetNeighbourNodes(char[,] map, Point node)
    {
      var nodes = new List<Point>();

      // up
      if (node.Y > 0 && _walkableCharacters.Contains(map[node.Y - 1, node.X]))
      {
        nodes.Add(new Point(node.X, node.Y - 1));
      }

      // right
      if (node.X < map.GetLength(1) - 1 && _walkableCharacters.Contains(map[node.Y, node.X + 1]))
      {
        nodes.Add(new Point(node.X + 1, node.Y));
      }

      // down
      if (node.Y < map.GetLength(0) - 1 && _walkableCharacters.Contains(map[node.Y + 1, node.X]))
      {
        nodes.Add(new Point(node.X, node.Y + 1));
      }

      // left
      if (node.X > 0 && _walkableCharacters.Contains(map[node.Y, node.X - 1]))
      {
        nodes.Add(new Point(node.X - 1, node.Y));
      }

      return nodes;
    }

    /// <summary>
    /// Process a list of valid paths generated by the Pathfind function and return 
    /// a coherent path to current.
    /// </summary>
    /// <param name="cameFrom">A list of nodes and the origin to that node.</param>
    /// <param name="current">The destination node being sought out.</param>
    /// <returns>The shortest path from the start to the destination node.</returns>
    private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
      if (!cameFrom.Keys.Contains(current))
        return new List<Point> { current };

      var path = ReconstructPath(cameFrom, cameFrom[current]);
      path.Add(current);

      return path;
    }
  }
}