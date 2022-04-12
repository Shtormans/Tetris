using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        BlockVariants blockVariants;
        Block block;
        Field field;
        Point[] highestCoords;

        public Form1()
        {
            InitializeComponent();

            blockVariants = new BlockVariants();
            field = new Field(this);

            CreateNewBlock();
        }

        private void CreateNewBlock()
        {
            highestCoords = field.GetHighestCoords();

            BlockPattern blockPattern = blockVariants.GetRandomBlockVariant();
            block = new Block(blockPattern, this);

            timer1.Interval = 500;
            timer1.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int direction = 0;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    direction = -1; 
                    break;
                case Keys.Right:
                    direction = 1;
                    break;
                case Keys.Down:
                    timer1.Interval = 50;
                    break;
                default:
                    break;
            }

            block.TryMove(direction);
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (!block.TryMoveDown(highestCoords))
            {
                field.AddCells(block);
                CreateNewBlock();
            }
        }
    }
}
