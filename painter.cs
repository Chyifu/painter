using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HW1_102378056
{
    public partial class Form1 : Form
    {
        //宣告
        Graphics g;
        float penwidth = 3F;                                                     //初始設定畫筆寬度
        Pen pen = new Pen(Color.Black, 3);                                       //初始設定畫筆顏色
        SolidBrush Bpen = new SolidBrush(Color.Black);                           //初始設定填滿畫筆顏色
        Color color = Color.Black;                                               //初始設定顏色變數顏色
        Bitmap img;                                                              //記錄繪圖
        Bitmap buffer;                                                           //暫時置放繪圖空間
        int choose = 0;                                                          //滑鼠在畫布上時操作選擇 0:直線
        List<Point> points = new List<Point>();                                  //畫筆時記錄滑鼠移動的座標點
        bool isMouseDown = false;


        public Form1()
        {
            InitializeComponent();
            img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(img);
            SolidBrush Epen = new SolidBrush(Color.White);
            g.FillRectangle(Epen, 0, 0, 810, 455);
        }



        /**************************************************************************************/
        /*                      滑鼠在畫布上按下時的動作                                      */
        /**************************************************************************************/
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            Point p = new Point(e.X, e.Y);
            buffer = new Bitmap(img);      
            Graphics g = Graphics.FromImage(buffer);
            points.Clear();
            points.Add(p);
            if (choose == 1){                                                    //畫點時才進行以下操作 (因為有時後滑鼠會一邊移動一邊按下 所以兩邊都要寫點才不會漏掉)
                Color col = color;                                               //把目前選擇的顏色放到畫筆的顏色中
                SolidBrush Bpen = new SolidBrush(col);                           //點要用填滿筆刷畫
                g.FillEllipse(Bpen, p.X, p.Y, penwidth, penwidth);               //依照點下當時的座標畫一個填滿的圓形
            }
            pictureBox1.Image = buffer;
            textBox1.Text = textBox1.Text + "(" + p.X + "," + p.Y + ")" + Environment.NewLine;
        }


        /**************************************************************************************/
        /*                      滑鼠在畫布上移動時的動作                                      */
        /**************************************************************************************/
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);                                        //滑鼠的座標
            buffer = new Bitmap(img);      
            Graphics g = Graphics.FromImage(buffer);

            if (points.Count > 0 && isMouseDown)                                 //當滑鼠被按下(isMouseDown=true)且移動時(points序列有增加座標點)
            {
                Point Start_point = points[0];                                   //取得移動的起點座標
                Point End_point = points[points.Count - 1];                      //取得移動的終點座標
                Color col = color;

                switch (choose)
                {
                    case 0:                                                      //畫直線                 
                        Pen pc = new Pen(col, penwidth); 
                        g.DrawLine(pc, Start_point, p);                          //從起始點畫值線到目前畫到的座標
                        break;
                    case 1:                                                      //畫點
                         SolidBrush Bpen = new SolidBrush(col);
                        g.FillEllipse(Bpen, Start_point.X, Start_point.Y, penwidth, penwidth);
                        break;
                    case 2:                                                      //畫筆
                        pc = new Pen(col, penwidth); 
                        Point p0 = Start_point;
                        foreach (Point p1 in points)
                        {
                            g.DrawLine(pc, p0, p1);
                            p0 = p1;
                        }
                        break;
                    case 3:                                                      //畫圓形
                        pc = new Pen(col, penwidth);
                        g.DrawEllipse(pc, Start_point.X, Start_point.Y, p.X - Start_point.X, p.Y - Start_point.Y);                          
                        break;
                    case 4:                                                      //畫矩形
                        pc = new Pen(col, penwidth);
                        float wid = 0, hei = 0;                                  //因為矩形皆由右上角開始畫到左下角  因應滑鼠滑動方向會有四種情況斜右上(下),斜左上(下) 不同情況起始點終點設定不一樣
                        if (p.X - Start_point.X < 0 && p.Y - Start_point.Y < 0)
                        {
                            wid =Start_point.X-p.X;
                            hei = Start_point.Y - p.Y;
                            g.DrawRectangle(pc, p.X, p.Y, wid, hei);
                        }
                        else if (p.X - Start_point.X > 0 && p.Y - Start_point.Y < 0)
                        { 
                            wid = p.X - Start_point.X;
                            hei = Start_point.Y - p.Y;
                            g.DrawRectangle(pc, Start_point.X, p.Y, wid, hei);
                        }
                        else if (p.X - Start_point.X < 0 &&  p.Y - Start_point.Y > 0)
                        {
                            wid = Start_point.X - p.X;
                            hei =p.Y - Start_point.Y;
                            g.DrawRectangle(pc, p.X, Start_point.Y, wid, hei);
                        }
                        else {
                            wid =p.X- Start_point.X ;
                            hei = p.Y - Start_point.Y;
                            g.DrawRectangle(pc, Start_point.X, Start_point.Y, wid, hei);
                        }
                        break;
                    case 5:                                                      //橡皮擦     
                        SolidBrush Epen = new SolidBrush(Color.White);
                        p0 = Start_point;
                        foreach (Point p1 in points)
                        {
                            g.FillRectangle(Epen, p0.X, p0.Y, 30, 30);
                            p0 = p1;
                        }
                        break;
                }
            }
            points.Add(p);                                                      //將移動的座標點加入座標點集合中
            pictureBox1.Image = buffer;                                         //將暫存的影像放到pictureBox1.image中
            textBox3.Text = "(" + e.X + "," + e.Y + ")";
        }


        /**************************************************************************************/
        /*                      滑鼠在畫布上放開時的動作                                      */
        /**************************************************************************************/
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            img = buffer;             
            pictureBox1.Image = img;        
                textBox2.Text = textBox2.Text + "(" + e.X + "," + e.Y + ")" + Environment.NewLine;
        }

        /**************************************************************************************/
        /*                      工具列圖示被按下時每個圖示的顯示狀態設置(選擇或不選擇)        */
        /*           toolStripButton3_Click 比較特別 功能是清空畫面 並未設置按鈕選取狀態      */
        /*           toolStripButton1,2,4,13,14,15 皆是繪圖功能設定                           */
        /**************************************************************************************/
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            choose = 1;
            toolStripButton1.Checked = true;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = false;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            choose = 0;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = true;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = img;
            textBox1.Text="";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            choose = 2;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = true;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = false;
        }
        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            choose = 3;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = true;
            toolStripButton15.Checked = false;
        }

        private void toolStripButton13_Click(object sender, EventArgs e)
        {
            choose = 4;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton13.Checked = true;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = false;
        }
        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            choose = 5;
            toolStripButton1.Checked = false;
            toolStripButton2.Checked = false;
            toolStripButton3.Checked = false;
            toolStripButton4.Checked = false;
            toolStripButton13.Checked = false;
            toolStripButton14.Checked = false;
            toolStripButton15.Checked = true;
        }


        /**************************************************************************************/
        /*                      顏色圖示被按下時每個圖示的顯示狀態設置(選擇或不選擇)          */
        /*                      toolStripButton5~12  皆是顏色設定                             */
        /**************************************************************************************/

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            color = Color.Black;
            toolStripButton5.Checked = true;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = true;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked =false;
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            color = Color.White;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = true;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            color = Color.Red;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = true;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            color = Color.Blue;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = true;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            color = Color.Green;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = true;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            color = Color.Gray;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = true;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            color = Color.Yellow;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = true;
            toolStripButton12.Checked = false;
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            color = Color.Pink;
            toolStripButton5.Checked = false;
            toolStripButton6.Checked = false;
            toolStripButton7.Checked = false;
            toolStripButton8.Checked = false;
            toolStripButton9.Checked = false;
            toolStripButton10.Checked = false;
            toolStripButton11.Checked = false;
            toolStripButton12.Checked = true;
        }
        /**************************************************************************************/
        /*                                        筆刷粗細設定                                */
        /**************************************************************************************/
        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
        string width=comboBox1.Text;
    
       if (string.Equals(width,"3pt"))
       {
                pen.Width = 3F;
                penwidth = pen.Width;
            }
       else if (string.Equals(width, "5pt"))
            {
                pen.Width = 5F;
                penwidth = pen.Width;
            }
            else {
                pen.Width = 8F;
                penwidth = pen.Width;
            }
        }
        /**************************************************************************************/
        /*                                        儲存檔案設定                                */
        /**************************************************************************************/
        private void 存檔ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
             {
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1:
                        pictureBox1.Image.Save(fs,System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;

                    case 2:
                        pictureBox1.Image.Save(fs,System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        pictureBox1.Image.Save(fs,System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
                fs.Close();
              }
            
        }


    }
}
