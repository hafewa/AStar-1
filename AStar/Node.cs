using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    public enum NodeType
    {
        WALKABLE = 0,
        UNWALKABLE = 1,
        TRANSPARENT = 2
    }

    public class Node 
    {
        public int x;
        public int y;
        public float f;
        public float g;
        public float h;
        public bool walkable;
        public bool walkableOriginal;
        public Node parent;
        public int version;

        public Node(int x, int y, bool walkable)
        {
            this.x = x;
            this.y = y;
            this.walkable = walkableOriginal = walkable;
        }

        public void ResetWalkable()
        {
            walkable = walkableOriginal;
        }
    }
}
