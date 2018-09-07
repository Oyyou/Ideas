using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Logic
{
  public enum PathStatus
  {
    Valid,
    Invalid,
  }

  public class PathfinderResult
  {
    public PathStatus Status { get; set; }

    public List<Point> Path { get; set; }

    public List<string> Errors { get; set; }
  }

  public static class Pathfinder
  {
    /// <summary>
    /// Characters that we can walk on
    /// </summary>
    private static string _walkableCharacters = "0";

    public static PathfinderResult Find(char[,] map_, Point start, Point end)
    {
      var map = map_;
      map[start.Y, start.X] = '0';

      // We can't start the search from the original point, so we need to find which of the below 4 is the best route
      var leftStart = new Point(start.X - 1, start.Y);
      var rightStart = new Point(start.X + 1, start.Y);
      var upStart = new Point(start.X, start.Y - 1);
      var downStart = new Point(start.X, start.Y + 1);

      var startPoints = new List<Point>()
      {
        start
        //leftStart,
        //rightStart,
        //upStart,
        //downStart,
      };

      var errors = new List<string>();

      for (int i = 0; i < startPoints.Count; i++)
      {
        var result = ValidatePositions(map, startPoints[i], end);

        if (result.Status == PathStatus.Invalid)
        {
          errors.AddRange(result.Errors);
          startPoints.RemoveAt(i);
          i--;
        }
      }

      if (startPoints.Count == 0)
      {
        return new PathfinderResult()
        {
          Status = PathStatus.Invalid,
          Path = new List<Point>(),
          Errors = errors,
        };
      }

      var endResults = new List<PathfinderResult>();

      foreach (var startPoint in startPoints)
      {
        //var straightLineStatus = GetStraightLineStatus(map, startPoint, end);

        //if (straightLineStatus.Status == PathStatus.Valid)
        //{
        //  return straightLineStatus;
        //}

        // nodes that have already been analyzed and have a path from the start to them
        var closedSet = new List<Point>();

        // nodes that have been identified as a neighbor of an analyzed node, but have 
        // yet to be fully analyzed
        var openSet = new List<Point> { startPoint };

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
        currentDistance.Add(startPoint, 0);
        predictedDistance.Add(
            startPoint, Math.Abs(startPoint.X - end.X) + Math.Abs(startPoint.Y - end.Y)
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
            endResults.Add(new PathfinderResult()
            {
              Status = PathStatus.Valid,
              Path = ReconstructPath(cameFrom, end),
            });
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

        var endResult = new PathfinderResult()
        {
          Status = PathStatus.Invalid,
          Path = new List<Point>(),
          Errors = new List<string>(),
        };

        // unable to figure out a path, abort.
        endResult.Errors.Add(
            string.Format(
                "unable to find a path between ({0},{1}) and ({2},{3})",
                startPoint.X, startPoint.Y,
                end.X, end.Y
            )
        );

        endResults.Add(endResult);
      }

      var validResults = endResults.Where(c => c.Status == PathStatus.Valid);
      var invalidResults = endResults.Where(c => c.Status == PathStatus.Invalid);

      if (validResults.Count() > 0)
      {
        var newPath = validResults.OrderBy(c => c.Path.Count).FirstOrDefault().Path;
        newPath.RemoveAt(0);

        return new PathfinderResult()
        {
          Status = PathStatus.Valid,
          Path = newPath,
        };
      }
      else
      {
        return new PathfinderResult()
        {
          Status = PathStatus.Invalid,
          Path = new List<Point>(),
          //Errors = new List<string>(invalidResults.SelectMany(c => c.Errors)),
        };
      }
    }

    private static PathfinderResult GetStraightLineStatus(char[,] map, Point start, Point end)
    {
      var isStraightX = start.X == end.X;
      var isStraightY = start.Y == end.Y;

      if (isStraightX)
      {
        var points = new List<Point>();
        bool isValid = true;
        for (int y = Math.Min(start.Y, end.Y); y < Math.Max(start.Y, end.Y); y++)
        {
          isValid = false;

          var point = map[y, start.X];

          if (_walkableCharacters.Contains(point))
          {
            points.Add(new Point(start.X, y));
            isValid = true;
          }
          else
          {
            break;
          }
        }

        if (isValid)
        {
          return new PathfinderResult()
          {
            Status = PathStatus.Valid,
            Path = start.Y > end.Y ? points.OrderByDescending(c => c.Y).ToList() : points,
          };
        }
      }

      if (isStraightY)
      {
        var points = new List<Point>();
        bool isValid = true;
        for (int x = Math.Min(start.X, end.X); x < Math.Max(start.X, end.X); x++)
        {
          isValid = false;

          var point = map[start.Y, x];

          if (_walkableCharacters.Contains(point))
          {
            points.Add(new Point(x, start.Y));
            isValid = true;
          }
          else
          {
            break;
          }
        }

        if (isValid)
        {
          return new PathfinderResult()
          {
            Status = PathStatus.Valid,
            Path = start.X > end.X ? points.OrderByDescending(c => c.X).ToList() : points,
          };
        }
      }

      return new PathfinderResult()
      {
        Status = PathStatus.Invalid,
        Path = new List<Point>(),
        Errors = new List<string>(),
      };
    }

    /// <summary>
    /// Check if the positions are on the map, and aren't on an 'unwalkable' character
    /// </summary>
    /// <param name="map">What we need to navigate through</param>
    /// <param name="start">Where we start</param>
    /// <param name="end">Where we end</param>
    private static PathfinderResult ValidatePositions(char[,] map, Point start, Point end)
    {
      var result = new PathfinderResult()
      {
        Errors = new List<string>(),
        Path = new List<Point>(),
        Status = PathStatus.Valid,
      };

      result.Errors = new List<string>();

      if (start.X < 0)
        result.Errors.Add("Start.X isn't on map: " + start.X);
      if (start.X >= map.GetLength(1))
        result.Errors.Add("Start.Y isn't on map: " + start.Y);
      if (end.X < 0)
        result.Errors.Add("End.X isn't on map: " + end.X);
      if (end.X >= map.GetLength(0))
        result.Errors.Add("End.Y isn't on map: " + end.Y);

      try
      {
        var startCharacter = map[start.Y, start.X];

        if (!_walkableCharacters.Contains(startCharacter))
          result.Errors.Add(string.Format("Start position ({0}) is on 'unwalkable' character: {1}", start.ToString(), startCharacter));
      }
      catch (IndexOutOfRangeException e)
      {
        result.Errors.Add(e.ToString());
      }

      try
      {
        var endCharacter = map[end.Y, end.X];

        if (!_walkableCharacters.Contains(endCharacter))
          result.Errors.Add(string.Format("Start position ({0}) is on 'unwalkable' character: {1}", end.ToString(), endCharacter));
      }
      catch (IndexOutOfRangeException e)
      {
        result.Errors.Add(e.ToString());
      }

      if (result.Errors.Count > 0)
        result.Status = PathStatus.Invalid;

      return result;
    }

    /// <summary>
    /// Return a list of accessible nodes neighboring a specified node
    /// </summary>
    /// <param name="node">The center node to be analyzed.</param>
    /// <returns>A list of nodes neighboring the node that are accessible.</returns>
    private static IEnumerable<Point> GetNeighbourNodes(char[,] map, Point node)
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
    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
      if (!cameFrom.Keys.Contains(current))
        return new List<Point> { current };

      var path = ReconstructPath(cameFrom, cameFrom[current]);
      path.Add(current);

      return path;
    }
  }
}