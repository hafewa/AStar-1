using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitGrids();
            ClientSize = new Size(ColNum * GridSize, RowNum * GridSize);
            BackColor = Color.White;
            DoubleBuffered = true;
        }

        const int GridSize = 20;
        const int RowNum = 40;
        const int ColNum = 60;
        enum GridType
        {
            Blank,
            Block,
            Start,
            End
        }
        class DrawGrid
        {
            public GridType type;
            public int i, j;
            public Rectangle rect;
        }

        DrawGrid[,] AllGrids = new DrawGrid[RowNum, ColNum];
        Point[] ThePath = null;
        AStar astar = new AStar();

        void InitGrids()
        {
            for (int r = 0; r < RowNum; r++)
            {
                for (int c = 0; c < ColNum; c++)
                {
                    AllGrids[r, c] = new DrawGrid
                    {
                        type = GridType.Blank,
                        i = r,
                        j = c,
                        rect = new Rectangle(c * GridSize, r * GridSize, GridSize, GridSize)
                    };
                }
            }

            astar.grid = new Grid();
            astar.grid.InitGrid(RowNum, ColNum);

        }

        Pen gridpen = new Pen(Color.Brown, 2);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            //网格
            for (int i = 0; i < RowNum; i++)
            {
                for (int j = 0; j < ColNum; j++)
                {
                    var g = AllGrids[i, j];
                    e.Graphics.DrawRectangle(Pens.Gray, g.rect);
                    if (g.type == GridType.Block)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, g.rect);
                    }
                    else if (g.type == GridType.Start)
                    {
                        e.Graphics.FillRectangle(Brushes.Red, g.rect);
                    }
                    else if (g.type == GridType.End)
                    {
                        e.Graphics.FillRectangle(Brushes.Blue, g.rect);
                    }
                }
            }

            //路径
            if (ThePath != null)
            {
                e.Graphics.DrawLines(gridpen, ThePath);
            }
        }

        DrawGrid startGrid, endGrid;
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            int c = e.X / GridSize;
            int r = e.Y / GridSize;
            var grid = AllGrids[r, c];
            if (e.Button == MouseButtons.Left)
            {
                if (grid.type == GridType.Blank)
                {
                    grid.type = GridType.Block;

                    if (startDown)
                    {
                        if (startGrid != null)
                        {
                            startGrid.type = GridType.Blank;
                        }
                        grid.type = GridType.Start;
                        startGrid = grid;
                    }
                    else if (endDown)
                    {
                        if (endGrid != null)
                        {
                            endGrid.type = GridType.Blank;
                        }
                        grid.type = GridType.End;
                        endGrid = grid;
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (grid.type == GridType.Block)
                {
                    grid.type = GridType.Blank;
                }

            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int c = e.X / GridSize;
            int r = e.Y / GridSize;
            if (c < 0) c = 0;
            if (c > ColNum - 1) c = ColNum - 1;
            if (r < 0) r = 0;
            if (r > RowNum - 1) r = RowNum - 1;
            var grid = AllGrids[r, c];
            if (e.Button == MouseButtons.Left)
            {
                if (grid.type == GridType.Blank)
                {
                    grid.type = GridType.Block;
                }
            }
            else if(e.Button == MouseButtons.Right)
            {
                if (grid.type == GridType.Block)
                {
                    grid.type = GridType.Blank;
                }
            }
            Invalidate();
        }


        bool startDown = false;
        bool endDown = false;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.S)
            {
                startDown = true;
                endDown = false;
            }
            if (e.KeyCode == Keys.E)
            {
                endDown = true;
                startDown = false;
            }
            if(e.KeyCode == Keys.Space)
            {
                RunAStar();
            }
            if(e.KeyCode == Keys.Escape)
            {
                ThePath = null;
                foreach(var g in AllGrids)
                {
                    g.type = GridType.Blank;
                }
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.S)
            {
                startDown = false;
            }
            if (e.KeyCode == Keys.E)
            {
                endDown = false;
            }
        }

       void RunAStar()
        {
            byte[,] blocks = new byte[RowNum,ColNum];
            for(int i = 0; i < RowNum; i++)
            {
                for(int j = 0; j < ColNum; j++)
                {
                    switch (AllGrids[i, j].type)
                    {
                        case GridType.Block:
                            blocks[i, j] = (int)NodeType.UNWALKABLE;
                            break;
                    }
                }
            }

            astar.grid.InitGridData(blocks);
            var path = astar.Find(startGrid.j, startGrid.i, endGrid.j, endGrid.i);
            if (path != null)
            {
                ThePath = path.Select(n => new Point(n.x*GridSize+GridSize/2, n.y*GridSize + GridSize / 2)).ToArray();
            }
            Invalidate();
        }
    }
}
