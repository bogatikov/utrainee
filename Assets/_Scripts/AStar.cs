using System.Linq;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
class AStar
{
    public static List<(int, int)> FindPath(int[,] field, (int, int) start, (int, int) goal)
    {
        var closedSet = new Collection<PathNode>();
        var openSet = new Collection<PathNode>();

        PathNode startNode = new PathNode()
        {
            Position = start,
            CameFrom = null,
            PathLengthFromStart = 0,
            HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal)
        };
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet.OrderBy(node =>
                node.EstimateFullPathLength).First();

            if (currentNode.Position == goal)
                return GetPathForNode(currentNode);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (var neighbourNode in GetNeighbours(currentNode, goal, field))
            {
                if (closedSet.Count(node => node.Position == neighbourNode.Position) > 0)
                    continue;

                var openNode = openSet.FirstOrDefault(node => node.Position == neighbourNode.Position);

                if (openNode == null)
                    openSet.Add(neighbourNode);
                else
                    if (openNode.PathLengthFromStart > neighbourNode.PathLengthFromStart)
                {
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = neighbourNode.PathLengthFromStart;
                }
            }

        }

        return null;
    }
    private static Collection<PathNode> GetNeighbours(PathNode pathNode, (int, int) goal, int[,] field)
    {
        var result = new Collection<PathNode>();

        // Соседними точками являются соседние по стороне клетки.
        (int, int)[] neighbourPoints = new (int, int)[4];
        neighbourPoints[0] = (pathNode.Position.Item1 + 1, pathNode.Position.Item2);
        neighbourPoints[1] = (pathNode.Position.Item1 - 1, pathNode.Position.Item2);
        neighbourPoints[2] = (pathNode.Position.Item1, pathNode.Position.Item2 + 1);
        neighbourPoints[3] = (pathNode.Position.Item1, pathNode.Position.Item2 - 1);

        foreach (var point in neighbourPoints)
        {
            // Проверяем, что не вышли за границы карты.
            if (point.Item1 < 0 || point.Item1 >= field.GetLength(0))
                continue;
            if (point.Item2 < 0 || point.Item2 >= field.GetLength(1))
                continue;
            // Проверяем, что по клетке можно ходить.
            if (field[point.Item1, point.Item2] != 1)
                continue;
            // Заполняем данные для точки маршрута.
            var neighbourNode = new PathNode()
            {
                Position = point,
                CameFrom = pathNode,
                PathLengthFromStart = pathNode.PathLengthFromStart +
                GetDistanceBetweenNeighbours(),
                HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
            };
            result.Add(neighbourNode);
        }
        return result;
    }
    private static int GetDistanceBetweenNeighbours()
    {
        return 1;
    }
    private static List<(int, int)> GetPathForNode(PathNode pathNode)
    {
        var result = new List<(int, int)>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.Position);
            currentNode = currentNode.CameFrom;
        }
        result.Reverse();
        return result;
    }
    public static int GetHeuristicPathLength((int, int) from, (int, int) to)
    {
        return Math.Abs(from.Item1 - to.Item1) + Math.Abs(from.Item2 - to.Item2);
    }
}

class PathNode
{
    public (int, int) Position { get; set; }

    public int PathLengthFromStart { get; set; }
    public PathNode CameFrom { get; set; }

    public int HeuristicEstimatePathLength { get; set; }
    public int EstimateFullPathLength
    {
        get
        {
            return this.PathLengthFromStart + this.HeuristicEstimatePathLength;
        }
    }

}