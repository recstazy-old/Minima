﻿using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Minima.Navigation
{
    /// <summary>
    /// Dijkstra algorithm implementation
    /// </summary>
    public class Dijkstra : IPathFindAlgorithm
    {
        #region Fields

        private DijkstraNode[] nodes;
        private NavPoint[] points;
        private Dictionary<NavPoint, int> pointsIndexes = new Dictionary<NavPoint, int>();

        #endregion

        /// <summary>
        /// Initialize the point field to search in
        /// </summary>
        public Dijkstra(NavPoint[] points)
        {
            this.points = points;
            nodes = CreateNodes(points);
        }

        /// <summary>
        /// Calculate navigation path from origin to target
        /// </summary>
        public NavPath FindPath(NavPoint origin, NavPoint target)
        {
            var startNode = GetNodeByPoint(origin);
            SetWeight(startNode, 0f, new NavPoint());

            var dijkstraPath = GetPath(target);
            var path = new NavPath();
            path.InitializeEmpty();

            path.Add(dijkstraPath.Select(node => node.Point).ToArray());

            return path;
        }

        /// <summary>
        /// Main Dijktra's algorithm
        /// </summary>
        private DijkstraNode[] GetPath(NavPoint target)
        {
            var exclusions = new DijkstraNode[0];
            DijkstraNode current;

            while(true)
            {
                current = GetMinWeightNode(exclusions);

                if (current.Point == target)
                {
                    break;
                }

                var connections = current.Point.ConnectedPoints;

                foreach (var c in connections)
                {
                    float distance = Vector2.Distance(current.Point.Position, c.Position);
                    float connectionWeight = distance + current.Weight;
                    var connectedNode = GetNodeByPoint(c);

                    if (connectedNode.IsValid)
                    {
                        if (connectionWeight < connectedNode.Weight)
                        {
                            SetWeight(connectedNode, connectionWeight, current.Point);
                        }
                    }
                }

                exclusions = exclusions.ConcatOne(current);
            }

            return ConstructPath(current);
        }

        private DijkstraNode[] ConstructPath(DijkstraNode lastNode)
        {
            var node = lastNode;
            var path = new DijkstraNode[0];

            while (node.IsValid && node.Weight > 0f)
            {
                path = path.ConcatOne(node);
                node = GetNodeByPoint(node.Source);
            }

            path = path.Reverse().ToArray();

            return path;
        }

        private DijkstraNode GetMinWeightNode(params DijkstraNode[] except)
        {
            return nodes
                .Except(except)
                .Aggregate((node, next) => node.Weight < next.Weight ? node : next);
        }

        private DijkstraNode[] CreateNodes(NavPoint[] points)
        {
            DijkstraNode[] nodes = new DijkstraNode[0];
            
            foreach (var p in points)
            {
                nodes = nodes.ConcatOne(new DijkstraNode(p, Mathf.Infinity));
            }

            return nodes;
        }

        private DijkstraNode GetNodeByPoint(NavPoint point)
        {
            var index = IndexOf(point);

            if (index < 0)
            {
                return new DijkstraNode();
            }

            var node = nodes[index];
            return node;
        }

        private void SetWeight(DijkstraNode node, float weight, NavPoint source)
        {
            nodes[IndexOf(node)].SetWeightAndSource(weight, source);
        }

        private int IndexOf(DijkstraNode node)
        {
            return IndexOf(node.Point);
        }

        private int IndexOf(NavPoint point)
        {
            if (!pointsIndexes.ContainsKey(point))
            {
                var index = Array.IndexOf(points, point);
                pointsIndexes.Add(point, index);
                return index;
            }
            else
            {
                return pointsIndexes[point];
            }
        }
    }
}
