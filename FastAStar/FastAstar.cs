using System;
using System.Collections.Generic;

namespace FastAStar
{

    /**
	 * A*搜索 128x128 4方向
	 * @author clifford.cheny http://www.cnblogs.com/flash3d/
	 */

    public class Node
    {
        public int block;
        public int version;
        public List<Node> links;
        public int linksLength;
        public Node parent;
        public int nowCost;
        public int mayCost;
        public int dist;
        public int x;
        public int y;
        public Node next;
        public Node pre;
    }


    public class Astar
    {
        public const int MAP_SIZE = 128;
        public const int MAP_SIZE_1 = 127;
        //public const int BLOCK_NUM = 1000;
        public const int D1 = 7;


        public List<Node> map;
        public int nodeVersion = 0;
        //public int mapVersion = 0;
        public bool isPrune = true;

        Random random = new Random();
        //产生地图
        public void InitMap(byte[,] data)
        {
            //mapVersion++;

            Node n;
            List<Node> links = new List<Node>();
            int rows = data.GetUpperBound(0) + 1;
            int cols = data.GetUpperBound(1) + 1;

            map = new List<Node>(MAP_SIZE * MAP_SIZE);
            for (int i = 0; i < MAP_SIZE * MAP_SIZE; i++)
            {
                n = new Node();
                int x = i & MAP_SIZE_1;
                int y = i >> D1;
                n.x = x;
                n.y = y;
                n.links = new List<Node>(4);
                map.Add(n);
            }

            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    map[(i << D1) | j].block = data[i,j];
                }
            }

            for (int i = 0; i < MAP_SIZE * MAP_SIZE; i++)
            {
                n = map[i];
                if (n.block == 1) continue;
                links = n.links;
                int x = n.x;
                int y = n.y;
                if ((((i & MAP_SIZE) >> D1) ^ (i & 1)) != 0)
                {
                    if (y > 0 && map[((y - 1) << D1) | x].block != 1)
                    {
                        links.Add(map[((y - 1) << D1) | x]);
                    }
                    if (y < MAP_SIZE_1 && map[((y + 1) << D1) | x].block != 1)
                    {
                        links.Add(map[((y + 1) << D1) | x]);
                    }
                    if (x < MAP_SIZE_1 && map[(y << D1) | (x + 1)].block != 1)
                    {
                        links.Add(map[(y << D1) | (x + 1)]);
                    }
                    if (x > 0 && map[(y << D1) | (x - 1)].block != 1)
                    {
                        links.Add(map[(y << D1) | (x - 1)]);
                    }
                }
                else
                {
                    if (x < MAP_SIZE_1 && map[(y << D1) | (x + 1)].block != 1)
                    {
                        links.Add(map[(y << D1) | (x + 1)]);
                    }
                    if (x > 0 && map[(y << D1) | (x - 1)].block != 1)
                    {
                        links.Add(map[(y << D1) | (x - 1)]);
                    }
                    if (y > 0 && map[((y - 1) << D1) | x].block != 1)
                    {
                        links.Add(map[((y - 1) << D1) | x]);
                    }
                    if (y < MAP_SIZE_1 && map[((y + 1) << D1) | x].block != 1)
                    {
                        links.Add(map[((y + 1) << D1) | x]);
                    }
                }
                n.linksLength = links.Count;
            }
        }

        //搜索
        public List<Node> search(int startX, int startY, int endX, int endY)
        {
             var startNode = map[(startY << D1) | startX];
            var endNode = map[(endY << D1) | endX];
            if (startNode.block == 1 || endNode.block == 1)
            {
                return null;
            }
            int i;
            int l;
            int f;
            Node t;
            int openBase = Math.Abs(endX - startX) + Math.Abs(endY - startY);
            List<Node> open = new List<Node>(2);
            Node current;
            Node test;
            List<Node> links;
            open.Add(startNode);
            startNode.pre = startNode.next = null;
            startNode.version = ++nodeVersion;
            startNode.nowCost = 0;
            while (true)
            {
                current = open[0];
                open[0] = current.next;
                if (null != open[0]) open[0].pre = null;

                if (current == endNode)
                {
                    if (isPrune)
                        return prunePath(startNode, current);
                    else
                        return buildPath(startNode, current);
                }
                links = current.links;
                l = current.linksLength;
                for (i = 0; i < l; i++)
                {
                    test = links[i];
                    f = current.nowCost + 1;
                    if (test.version != nodeVersion)
                    {
                        test.version = nodeVersion;
                        test.parent = current;
                        test.nowCost = f;
                        test.dist = Math.Abs(endX - test.x) + Math.Abs(endY - test.y);
                        f += test.dist;
                        test.mayCost = f;
                        f = (f - openBase) >> 1;
                        test.pre = null;
                        test.next = open[f];
                        if (open[f] != null) open[f].pre = test;
                        open[f] = test;
                    }
                    else if (test.nowCost > f)
                    {
                        if (test.pre != null) test.pre.next = test.next;
                        if (test.next != null) test.next.pre = test.pre;
                        if (open[1] == test) open[1] = test.next;
                        test.parent = current;
                        test.nowCost = f;
                        test.mayCost = f + test.dist;
                        test.pre = null;
                        test.next = open[0];
                        if (open[0] != null) open[0].pre = test;
                        open[0] = test;
                    }
                }
                if (null == open[0])
                {
                    if (null == open[1]) break;
                    t = open[0];
                    open[0] = open[1];
                    open[1] = t;
                    openBase += 2;
                }
            }
            return null;
        }

        private List<Node> prunePath(Node startNode, Node endNode)
        {

            List<Node> path = new List<Node>();
            path.Add(endNode);
            if (endNode == startNode) return path;
            var current = endNode.parent;
            var dx = current.x - endNode.x;
            var dy = current.y - endNode.y;
            int cx;
            int cy;
            Node t;
            Node t2;

            while (true)
            {
                if (current == startNode)
                {
                    path.Add(current);
                    return path;
                }
                t = current.parent;
                cx = current.x;
                cy = current.y;
                if (t != startNode)
                {
                    t2 = t.parent;
                    if (Math.Abs(t2.x - cx) == 1
                        && Math.Abs(t2.y - cy) == 1
                        && map[(cy << D1) | t2.x].block != 1
                        && map[(t2.y << D1) | cx].block != 1)
                    {
                        t = t2;
                    }
                }
                if (t.x - cx == dx && t.y - cy == dy)
                {
                    current = t;
                }
                else
                {
                    dx = t.x - cx;
                    dy = t.y - cy;
                    path.Add(current);
                    current = t;
                }
            }
        }

        private List<Node> buildPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            path.Add(endNode);
            while (endNode != startNode)
            {
                endNode = endNode.parent;
                path.Add(endNode);
            }
            return path;
        }
    }
}