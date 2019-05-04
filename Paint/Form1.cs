using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading;


namespace lab2
{
    public partial class Form1 : Form
    {
        bool draw = false;
        bool pen = false;
        bool rectangle = false;
        bool elipse = false;
        bool resize = false;
        Button buttonColor = null;
        private int _x;
        private int _y;
        private List<(GraphicsPath path,Pen pen)> paths = new List<(GraphicsPath path, Pen pen)>();
        private List<(Rectangle rect, Pen pen)> rectangles = new List<(Rectangle rect, Pen pen)>();
        private List<(Rectangle rect, Pen pen)> elipses = new List<(Rectangle rect, Pen pen)>();
        private GraphicsPath _gPath = new GraphicsPath();
        Pen graphPen = new Pen(Color.Black, 1);
        bool isTmpRect = false;
        Rectangle tmpRect = new Rectangle();
        Point p1, p2;
        public Form1()
        {
            InitializeComponent();
            Bcolor.BackColor = Color.Black;
            pictureBox1.Image = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height,pictureBox1.CreateGraphics());
            for (int i = 1; i < 4; i++)
            {
                toolStripComboBox1.Items.Add(i);
            }
            toolStripComboBox1.Text = "1";
            LoadColors();
        }

        private void LoadColors()
        {
            foreach (KnownColor cc in Enum.GetValues(typeof(KnownColor)))
            {
                Color color = Color.FromKnownColor(cc);
                Button button = new Button
                {                   
                    FlatStyle = FlatStyle.Flat,                  
                    BackColor = color,
                    Width = 25,
                    Height = 25,
                    Name = cc.ToString()
                };
                button.FlatAppearance.BorderSize = 0;
                button.Click += ButtonColorClick;
                flowLayoutPanel1.Controls.Add(button);

            }
            
        }


        private void ButtonColorClick(object sender, EventArgs e)
        {
            if (buttonColor != null)
            {
                buttonColor.Image = new Bitmap(25, 25);
                using (var g = Graphics.FromImage(buttonColor.Image))
                {
                    g.Clear(buttonColor.BackColor);
                }
            }  
            Button b = (Button)sender;
            buttonColor = b;
            graphPen = new Pen(b.BackColor,graphPen.Width);
            Bcolor.BackColor = b.BackColor;
            
            b.Image = new Bitmap(25, 25);
            using (var g = Graphics.FromImage(b.Image))
            {
                buttonColor = b;
                Color c = Color.FromArgb(255 ^ b.BackColor.R, 255 ^ b.BackColor.G, 255 ^ b.BackColor.B);
                Pen p = new Pen(c, 2)
                {
                    DashStyle = DashStyle.Dash
                };
                g.Clear(b.BackColor);
                g.DrawRectangle(p, 1, 1, 21, 21);
            }

            b.Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            toolStripButton4.Checked = false;
            rectangle = false;
            toolStripButton5.Checked = false;
            elipse = false;
            Toggle(ref pen, toolStripButton1);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isTmpRect = false;
                resize = true;
                pictureBox1.Refresh();
                draw = false;
                
            }
            if (e.Button == MouseButtons.Left)
            {
                if (pen)
                {
                    paths.Add((_gPath, graphPen));
                    _x = e.X;
                    _y = e.Y;
                    p1 = e.Location;
                    draw = true;
                }

                if (elipse)
                {
                    _x = e.X;
                    _y = e.Y;
                    draw = true;

                }

                if (rectangle)
                {
                    _x = e.X;
                    _y = e.Y;
                    draw = true;

                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                draw = false;
                if (isTmpRect)
                {
                    if (rectangle)
                    {
                        rectangles.Add((tmpRect, graphPen));
                    }
                    if (elipse)
                    {
                        elipses.Add((tmpRect, graphPen));
                    }
                    isTmpRect = false;
                }
                _gPath = new GraphicsPath();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pen && draw)
            {
                _gPath.AddLine(new Point(_x, _y), new Point(e.X, e.Y));
                _x = e.X;
                _y = e.Y;
                p2 = e.Location;
                pictureBox1.Refresh();
                p1 = p2;
            }
            if( elipse && draw)
            {
                isTmpRect = true;
                tmpRect = new Rectangle(
                        Math.Min(_x, e.X),
                        Math.Min(_y, e.Y),
                        Math.Abs(e.X - _x),
                        Math.Abs(e.Y - _y));
                pictureBox1.Refresh();
            }

            if (rectangle && draw)
            {
                isTmpRect = true;
                tmpRect = new Rectangle(
                        Math.Min(_x, e.X),
                        Math.Min(_y, e.Y),
                        Math.Abs(e.X - _x),
                        Math.Abs(e.Y - _y));
                pictureBox1.Refresh();
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            resize = true;
            pictureBox1.Refresh();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ClearPictureData();
            pictureBox1.Refresh();
        }

        private void ClearPictureData()
        {
            paths.Clear();
            elipses.Clear();
            rectangles.Clear();
            pictureBox1.Image = new Bitmap(pictureBox1.ClientSize.Width, pictureBox1.ClientSize.Height, pictureBox1.CreateGraphics());
        }

        private void BLoad_Click(object sender, EventArgs e)
        {
            //open file
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg|bmp files (*.bmp)|*.bmp";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                ClearPictureData();
                Image img= Image.FromFile(fileDialog.FileName);
                pictureBox1.Image = img;
                this.Width = Math.Min(img.Width + Width - pictureBox1.Width,Screen.PrimaryScreen.WorkingArea.Width);
                this.Height = Math.Min(img.Height + Height - pictureBox1.Height, Screen.PrimaryScreen.WorkingArea.Width);
                CenterToScreen();
            }
        }

        private void BSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "png files (*.png)|*.png|jpeg files (*.jpeg)|*.jpeg|bmp files (*.bmp)|*.bmp";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                Graphics g = Graphics.FromImage(pictureBox1.Image);
                foreach (var path in paths)
                {
                    g.DrawPath(path.pen, path.path);
                }

                foreach (var rect in rectangles)
                {
                    g.DrawRectangle(rect.pen, rect.rect);
                }

                foreach (var elipse in elipses)
                {
                    g.DrawEllipse(elipse.pen, elipse.rect);
                }

                pictureBox1.Image.Save(saveFileDialog.FileName);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            graphPen = new Pen(graphPen.Color, toolStripComboBox1.SelectedIndex + 1);
            pictureBox1.Focus();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            toolStripButton1.Checked = false;
            pen = false;
            toolStripButton5.Checked = false;
            rectangle = false;
            Toggle(ref elipse,toolStripButton4);
        }

        private void Toggle(ref bool item,ToolStripButton button)
        {
            if (button.Checked == true)
            {
                item = true;
            }
            else
            {
                item = false;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
           
            toolStripButton4.Checked = false;
            elipse = false;
            toolStripButton1.Checked = false;
            pen = false;
            Toggle(ref rectangle, toolStripButton5);

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pl-PL");
            // The following line provides localization for the application's user interface.  
            Thread.CurrentThread.CurrentUICulture = culture;
            // The following line provides localization for data formats.  
            Thread.CurrentThread.CurrentCulture = culture;

            ////save stuff
            //Size _size = Size;
            //Color _Bcolor = Bcolor.BackColor;
            //string _thickness = toolStripComboBox1.Text;
            //Image image = pictureBox1.Image;
            ////clear
            //this.Controls.Clear();

            ////normal init
            //InitializeComponent();
            //this.Size = _size;
            //Bcolor.BackColor = _Bcolor;
            //pictureBox1.Image = image;
            //for (int i = 1; i < 4; i++)
            //{
            //    toolStripComboBox1.Items.Add(i);
            //}
            //toolStripComboBox1.Text = _thickness;
            //LoadColors();

            //if (buttonColor != null)
            //{
            //    foreach (Button item in flowLayoutPanel1.Controls)
            //    {
            //        if (item.BackColor == buttonColor.BackColor)
            //        {
            //            buttonColor = item;
            //            item.PerformClick();
            //        }
            //    }

            //}

            //if (pen)
            //    toolStripButton1.Checked = true;
            //if (elipse)
            //    toolStripButton4.Checked = true;
            //if (rectangle)
            //    toolStripButton5.Checked = true;

            ////fun

            //resize = true;
            //pictureBox1.Refresh();
            
            ApplyResourceToControl(new ComponentResourceManager(typeof(Form1)),this);

          //  resize = true;
            pictureBox1.Refresh();

            // ComponentResourceManager resources = new ComponentResourceManager(this.GetType());
            // ApplyResourceToControl(resources, this, culture);
        }

        private static void ApplyResourceToControl(ComponentResourceManager res, object control)
        {
            
            if(control is ToolStrip s)
            {
                foreach (ToolStripItem item in s.Items)
                {
                    res.ApplyResources(item, item.Name);
                    ApplyResourceToControl(res, item);
                }


            }
            if(control is Control contr)
            {
                foreach (Control c in contr.Controls)
                {
                    res.ApplyResources(c, c.Name);
                    ApplyResourceToControl(res, c);
                }
            }

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("");
            // The following line provides localization for the application's user interface.  
            Thread.CurrentThread.CurrentUICulture = culture;
            // The following line provides localization for data formats.  
            Thread.CurrentThread.CurrentCulture = culture;

            ////save stuff
            //Size _size = Size;
            //Color _Bcolor = Bcolor.BackColor;
            //string _thickness = toolStripComboBox1.Text;
            //Image image = pictureBox1.Image;

            ////clear
            //this.Controls.Clear();


            ////normal init
            //InitializeComponent();
            //this.Size = _size;

            //Bcolor.BackColor = _Bcolor;
            //pictureBox1.Image = image;
            //for (int i = 1; i < 4; i++)
            //{
            //    toolStripComboBox1.Items.Add(i);
            //}
            //toolStripComboBox1.Text = _thickness;
            //LoadColors();
            //if (buttonColor != null)
            //{
            //    foreach (Button item in flowLayoutPanel1.Controls)
            //    {
            //        if (item.BackColor == buttonColor.BackColor)
            //        {
            //            buttonColor = item;
            //            item.PerformClick();
            //        }
            //    }

            //}

            //if (pen)
            //    toolStripButton1.Checked = true;
            //if (elipse)
            //    toolStripButton4.Checked = true;
            //if (rectangle)
            //    toolStripButton5.Checked = true;

            //fun
            ApplyResourceToControl(new ComponentResourceManager(typeof(Form1)), this);

          //  resize = true;
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
           if ((draw && (pen||elipse||rectangle))||resize)
           {
                Graphics g = e.Graphics;
                foreach (var path in paths)
                {
                    g.DrawPath(path.pen, path.path);
                }
                foreach (var rect in rectangles)
                {
                    g.DrawRectangle(rect.pen, rect.rect);
                }
                foreach (var elipse in elipses)
                {
                    g.DrawEllipse(elipse.pen, elipse.rect);
                }

                if (isTmpRect)
                {
                    if (rectangle)
                    {
                        g.DrawRectangle(graphPen, tmpRect);
                    }
                    if (elipse)
                    {
                        g.DrawEllipse(graphPen, tmpRect);
                    }

                }
                resize = false;
           }           
        }
    }
}
