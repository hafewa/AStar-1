using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{

    public class Grid
    {
        public static float straightCost = 1.0f;//直线
        public static float diagCost = 1.4142135623730951f;//斜线
        public static bool allowDiagMove = false;
        public static int findWalkableNodeMaxDist = 10;

        private Node _startNode;
        private Node _endNode;
        private Node[][] _nodes;
        private int _numCols;
        private int _numRows;
        private int _maxCol;
        private int _maxRow;

        /// <summary>
        /// 起始点
        /// </summary>
        public Node startNode
        {
            get { return _startNode; }
        }

        /// <summary>
        /// 结束点
        /// </summary>
        public Node endNode
        {
            get { return _endNode; }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int numCols
        {
            get { return _numCols; }
        }

        /// <summary>
        /// 行数
        /// </summary>
        public int numRows
        {
            get { return _numRows; }
        }

        /// <summary>
        /// 最大列数
        /// </summary>
        public int maxCol
        {
            get { return _maxCol; }
        }

        /// <summary>
        /// 最大行数
        /// </summary>
        public int maxRow
        {
            get { return _maxRow; }
        }

        /// <summary>
        /// 初始化网格
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public void InitGrid(int rows, int cols)
        {
            _nodes = new Node[cols][];
            for (int i = 0; i < cols; i++)
            {
                _nodes[i] = new Node[rows];
                for (int j = 0; j < rows; j++)
                    _nodes[i][j] = new Node(i, j, true);
            }
            _numRows = rows;
            _numCols = cols;
            _maxRow = _numRows - 1;
            _maxCol = _numCols - 1;

        }

        /// <summary>
        /// 初始化网格数据
        /// </summary>
        /// <param name="gridData"></param>
        public void InitGridData(byte[,] gridData)
        {
            for (int i = 0; i < _numCols; i++)
                for (int j = 0; j < _numRows; j++)
                    _nodes[i][j].walkable = gridData[j,i] != (byte)NodeType.UNWALKABLE;
        }

        /// <summary>
        /// 寻找最近可行走的节点
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <returns></returns>
        public Node FindWalkableNode(int startX, int startY, int endX, int endY)
        {
            int distX = Math.Abs(endX - startX);
            int distY = Math.Abs(endY - startY);
            Node node = null;
            Node testNode = null;
            int dist = 1;
            int sx;
            int sy;
            int ex;
            int ey;
            while (dist <= findWalkableNodeMaxDist)
            {
                sx = endX - dist;
                sy = endY - dist;
                ex = endX + dist;
                ey = endY + dist;
                for (int i = sx; i <= ex; i++)
                {
                    for (int j = sy; j <= ey; j++)
                    {
                        // 超出测试点范围
                        if (i < 0 || j < 0 || i >= _numCols || j >= _numRows || i - sx > distX || j - sy > distY)
                            continue;
                        testNode = GetNode(i, j);
                        // 当测试点不为空（在整个地图范围内）并且node为空（首次测试）或testNode点到起始点的距离小于node点到起始点的距离
                        if (testNode != null && testNode.walkable &&
                            (node == null || GetSquare(i, j, startX, startY) < GetSquare(node.x, node.y, startX, startY)))
                        {
                            node = testNode;
                        }
                    }
                }
                if (node != null)
                    return node;

                dist++;
            }
            return node;
        }

        /// <summary>
        /// 是否存在节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool HasNode(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _numCols && y < _numRows;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Node GetNode(int x, int y)
        {
            return _nodes[x][y];
        }

        /// <summary>
        /// 设置起始节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetStartNode(int x, int y)
        {
            _startNode = _nodes[x][y];
        }

        /// <summary>
        /// 设置结束节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetEndNode(int x, int y)
        {
            _endNode = _nodes[x][y];
        }

        /// <summary>
        /// 获取节点是否可行走
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool GetWalkable(int x, int y)
        {
            return _nodes[x][y].walkable;
        }

        /// <summary>
        /// 设置节点是否可行走
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="walkable"></param>
        public void SetWalkable(int x, int y, bool walkable)
        {
            _nodes[x][y].walkable = walkable;
        }

        public static float GetSquare(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            return dx * dx + dy * dy;
        }

    }
}
