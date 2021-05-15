using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaintV2
{
    public partial class Form1 : Form
    {
        Pen pen;
        SolidBrush brush;
        Graphics graphics;
        Bitmap bitmap;
        List<Image> images;
        bool moving;
        Point point;
        int state;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pictureBox1.Width,pictureBox1.Height);
            graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            images = new List<Image>();
            pen = new Pen(Color.Black, Convert.ToInt32(maskedTextBox1.Text));
            pen.StartCap = pen.EndCap = LineCap.Round;
            brush = new SolidBrush(Color.Black);
            moving = false;
            state = 0;
            checkBoxBrush.Appearance = Appearance.Button;
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            point = e.Location;
            addCtrlz();
            if (maskedTextShapeWidth.Text != "" && maskedTextShapeHeight.Text != "")
            {
                int width = Convert.ToInt32(maskedTextShapeWidth.Text);
                int height = Convert.ToInt32(maskedTextShapeHeight.Text);
                Rectangle rectangle = new Rectangle(point.X - width/2, point.Y - height/2, width, height);

                int startAngle = Convert.ToInt32(maskedTextStartAngle.Text);
                int endAngle = Convert.ToInt32(maskedTextEndAngle.Text);

                if (width > 0 && height > 0)
                {
                    switch (state)
                    {
                        case 1:
                            if (checkBoxBrush.Checked)
                                graphics.FillRectangle(brush, rectangle);
                            graphics.DrawRectangle(pen, rectangle);
                            break;
                        case 2:
                            Point[] Points =
                            {
                                new Point(point.X, point.Y - height/2),
                                new Point(point.X - width/2, point.Y + height/2),
                                new Point(point.X + width/2, point.Y + height/2)
                            };
                            if (checkBoxBrush.Checked)
                                graphics.FillPolygon(brush, Points);
                            graphics.DrawPolygon(pen, Points);
                            break;
                        case 3:
                            if (checkBoxBrush.Checked)
                                graphics.FillEllipse(brush, rectangle);
                            graphics.DrawEllipse(pen, rectangle);
                            break;
                        case 4:
                            if (checkBoxBrush.Checked)
                                graphics.FillPie(brush, rectangle, startAngle, endAngle);
                            graphics.DrawPie(pen, rectangle, startAngle, endAngle);
                            break;
                        default:
                            break;
                    }
                }
                pictureBox1.Image = bitmap;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (moving && state == 0)
            {
                pictureBox1.Image = bitmap;
                graphics.DrawLine(pen, point, e.Location);
                point = e.Location;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text != "")
                pen.Width = Convert.ToInt32(maskedTextBox1.Text);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.Z))
            {
                if (images.Count > 0)
                {
                    bitmap = (Bitmap)images.Last().Clone();
                    pictureBox1.Image = images.Last();
                    bitmap = new Bitmap(bitmap, pictureBox1.Size);
                    graphics = Graphics.FromImage(bitmap);
                    images.RemoveAt(images.Count - 1);
                }    
                    
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            bitmap = new Bitmap(bitmap, pictureBox1.Size);
            graphics = Graphics.FromImage(bitmap);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switchShape(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            switchShape(1);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            switchShape(2);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            switchShape(3);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            switchShape(4);
        }
        private void switchShape(int id)
        { 
            state = id;
            List<Button> buttons = new List<Button>() {
            button1,
            button2,
            button3,
            button4,
            button5
            };
            buttons.ForEach(b => b.BackColor = Color.White);
            buttons[id].BackColor = Color.IndianRed;
        }
        private void buttoPen_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                pen.Color = colorDlg.Color;
                checkBox1.BackColor = colorDlg.Color;
            }
        }
        private void buttonBrush_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                brush.Color = colorDlg.Color;
                checkBox2.BackColor = colorDlg.Color;
            }
        }
        private void checkBoxBrush_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxBrush.BackColor = checkBoxBrush.Checked ? Color.IndianRed : Color.White;
        }
        private void addCtrlz()
        {
            if (pictureBox1.Image == null)
            {
                return;
            }
            Bitmap btm = (Bitmap)pictureBox1.Image.Clone();
            images.Add(btm);
            if (images.Count > 50)
                images.RemoveAt(0);
        }

        private void открытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                addCtrlz();
                pictureBox1.Image = (Bitmap)Image.FromFile(openFileDialog1.FileName);
                bitmap = (Bitmap)pictureBox1.Image;
            }
        }
        private void сохранитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            addCtrlz();
            graphics.Clear(pictureBox1.BackColor);
            pictureBox1.Image = bitmap;
        }
    }
}
