using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    class Field
    {
        private Cell[,] field;
        private readonly Form form;
        private readonly Size fieldSize;
        private const int globalHeightStart = 15;

        public Field(Form form)
        {
            this.form = form;

            int height = form.Size.Height / Cell.cellSize.Height - globalHeightStart;
            int width = form.Size.Width / Cell.cellSize.Width - 2;

            fieldSize = new Size(width, height);
            field = new Cell[height, width];
        }

        public void AddCells(Block block)
        {
            for (int i = 0; i < block.BlockLength; i++)
            {
                Point cellCoords = block.GetCellCoordsByIndex(i);

                try
                {
                    field[cellCoords.Y - globalHeightStart, cellCoords.X] = block.GetCellByIndex(i);
                }
                catch (Exception)
                {

                }
            }
        }

        public Point[] GetHighestCoords()
        {
            Point[] highestCoords = new Point[fieldSize.Width];
            int index = 0;

            for (int x = 0; x < fieldSize.Width; x++)
            {
                highestCoords[x] = new Point(x, fieldSize.Height + globalHeightStart - 1);
                for (int y = 0; y < fieldSize.Height; y++)
                {
                    if (field[y, x] != null)
                    {
                        highestCoords[x] = new Point(x, y + globalHeightStart);
                        index++;
                        break;
                    }
                }
            }

            return highestCoords;
        }
    }

    class Block
    {
        private readonly Form form;
        private readonly Point[] blockCoords;
        private Cell[] blockBody;
        private Color cellColor;

        public int BlockLength
        {
            get { return blockCoords.Length; }
        }

        public Cell GetCellByIndex(int index)
        {
            return blockBody[index];
        }

        public Point GetCellCoordsByIndex(int index)
        {
            return blockBody[index].CellCoords;
        }

        public Block(BlockPattern blockPattern, Form form)
        {
            this.form = form;
            cellColor = blockPattern.CellColor;
            blockCoords = blockPattern.GetCellsCoords();

            blockBody = new Cell[blockPattern.BlockSize];
            InitializeBlock();
        }

        public bool TryMoveDown(Point[] highestCoords)
        {
            if (!CanMoveDown(highestCoords))
            {
                return false;
            }

            foreach (var cell in blockBody)
            {
                cell.MoveCellDown();
            }

            return true;
        }

        public bool TryMove(int direction)
        {
            if (!CanMove(direction))
            {
                return false;
            }

            foreach (var cell in blockBody)
            {
                cell.MoveCell(direction);
            }

            return true;
        }

        public bool CanMoveDown(Point[] highestCoords)
        {
            foreach (var cell in blockBody)
            {
                int index = cell.CellCoords.X;

                if (!cell.CanMoveDown(highestCoords[index]))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanMove(int direction)
        {
            foreach (var cell in blockBody)
            {
                int minPoint = 0;
                int maxPoint = form.Size.Width - 3 * Cell.cellSize.Width;

                if (!cell.CanMove(direction, minPoint, maxPoint))
                {
                    return false;
                }
            }

            return true;
        }

        private void InitializeBlock()
        {
            for (int i = 0; i < blockBody.Length; i++)
            {
                var cellCoords = blockCoords[i];
                blockBody[i] = new Cell(cellCoords, cellColor);
            }

            ShowBlock();
        }

        private void ShowBlock()
        {
            for (int i = 0; i < blockBody.Length; i++)
            {
                blockBody[i].ShowCell(form);
            }
        }
    }

    class Cell
    {
        public static readonly Size cellSize = new Size(30, 30);
        private readonly Button cellBody;

        public Point CellCoords
        {
            get { return new Point(cellBody.Location.X / cellSize.Width, cellBody.Location.Y / cellSize.Height); }
        }

        public void MoveCellDown()
        {
            int newDirectionY = cellBody.Location.Y + cellSize.Height;

            cellBody.Location = new Point(cellBody.Location.X, newDirectionY);
        }

        public void MoveCell(int direction)
        {
            int newDirectionX = cellBody.Location.X + direction * cellSize.Width;

            cellBody.Location = new Point(newDirectionX, cellBody.Location.Y);
        }

        public bool CanMoveDown(Point highestLocation)
        {
            int newLocationY = cellBody.Location.Y / cellSize.Height + 1;

            return highestLocation.Y != newLocationY;
        }

        public bool CanMove(int direction, int minPoint, int maxPoint)
        {
            int newLocationX = cellBody.Location.X + cellSize.Width * direction;

            return newLocationX >= minPoint && newLocationX <= maxPoint;
        }

        public void ShowCell(Form form)
        {
            form.Controls.Add(cellBody);
        }

        public Cell(Point cellCoords, Color cellColor)
        {
            cellBody = new Button();

            cellBody.Size = cellSize;
            cellBody.BackColor = cellColor;
            cellBody.FlatStyle = FlatStyle.Flat;
            cellBody.Enabled = false;
            cellBody.FlatAppearance.BorderSize = 1;

            int x = cellCoords.X * cellSize.Width;
            int y = cellCoords.Y * cellSize.Height;
            cellBody.Location = new Point(x, y);
        }
    }

    class BlockVariants
    {
        BlockPattern[] blockVariants;

        public BlockVariants()
        {
            blockVariants = new BlockPattern[7];

            Point[] iShape = { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3) };
            blockVariants[0] = new BlockPattern(iShape, Color.Blue);

            Point[] jShape = { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1) };
            blockVariants[1] = new BlockPattern(jShape, Color.Violet);

            Point[] lShape = { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(2, 0) };
            blockVariants[2] = new BlockPattern(lShape, Color.Orange);

            Point[] oShape = { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) };
            blockVariants[3] = new BlockPattern(oShape, Color.Yellow);

            Point[] sShape = { new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(2, 0) };
            blockVariants[4] = new BlockPattern(sShape, Color.Green);

            Point[] tShape = { new Point(1, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1) };
            blockVariants[5] = new BlockPattern(tShape, Color.Pink);

            Point[] zShape = { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1) };
            blockVariants[6] = new BlockPattern(zShape, Color.Red);
        }

        public BlockPattern GetRandomBlockVariant()
        {
            int index = new Random().Next(0, blockVariants.Length);
            return blockVariants[index];
        }
    }

    class BlockPattern
    {
        Point[] cellsCoords;
        Color cellColor;

        public int BlockSize
        {
            get { return cellsCoords.Length; }
        }

        public Point[] GetCellsCoords()
        {
            return (Point[])cellsCoords.Clone();
        }

        public Color CellColor
        {
            get { return cellColor; }
        }

        public BlockPattern(Point[] cellsCoords, Color cellColor)
        {
            this.cellsCoords = cellsCoords;
            this.cellColor = cellColor;
        }
    }
}
