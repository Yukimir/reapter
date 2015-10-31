using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace FDJ
{
    public partial class Form1 : Form
    {
        string[] breaklist = new string[5] { "0", "0", "0", "0", "0" };
        clsMCI cm = new clsMCI();
        string a = "0", b = "0";
        public Form1()
        {
            InitializeComponent();
            
        }
        private void addlist(string newtime)
        {
            for (int i = 0; i < 4; i++)
            {
                breaklist[i] = breaklist[i + 1];
            }
            breaklist[4] = newtime;
            button3.Text = (breaklist[0] == "0") ? "无" : formatLongToTimeStr(long.Parse(breaklist[0]));
            button4.Text = (breaklist[1] == "0") ? "无" : formatLongToTimeStr(long.Parse(breaklist[1]));
            button5.Text = (breaklist[2] == "0") ? "无" : formatLongToTimeStr(long.Parse(breaklist[2]));
            button6.Text = (breaklist[3] == "0") ? "无" : formatLongToTimeStr(long.Parse(breaklist[3]));
            button7.Text = (breaklist[4] == "0") ? "无" : formatLongToTimeStr(long.Parse(breaklist[4]));
        }
        public static String formatLongToTimeStr(long l)
        {
            String str = "aaa";
            int hour = 0;
            int minute = 0;
            int second = 0;
            int ms = 0;
            second = (int)l / 1000;
            ms = (int)l - second * 1000;
            if (second > 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute > 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }
            return string.Format("{0}:{1}:{2}",hour.ToString(), minute.ToString(), second.ToString());
        }
        public class clsMCI
        {

            public clsMCI()
            {

                //

                // TODO: 在此处添加构造函数逻辑

                //

            }



            //定义API函数使用的字符串变量 

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]

            private string Name = "";

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]

            private string durLength = "";

            [MarshalAs(UnmanagedType.LPTStr, SizeConst = 128)]

            private string TemStr = "";

            int ilong;

            //定义播放状态枚举变量

            public enum State
            {

                mPlaying = 1,

                mPuase = 2,

                mStop = 3

            };

            //结构变量

            public struct structMCI
            {

                public bool bMut;

                public int iDur;

                public int iPos;

                public int iVol;

                public int iBal;

                public string iName;

                public State state;

            };



            public structMCI mc = new structMCI();
            public int id;


            //取得播放文件属性

            public string FileName
            {

                get
                {

                    return mc.iName;

                }

                set
                {

                    //ASCIIEncoding asc = new ASCIIEncoding(); 

                    try
                    {
                        id++;

                        //if(mc.state == State.mPlaying) this.StopT();

                        Name = "";

                        TemStr = "";

                        TemStr = TemStr.PadLeft(127, Convert.ToChar(" "));

                        Name = Name.PadLeft(260, Convert.ToChar(" "));

                        mc.iName = value;

                        ilong = APIClass.GetShortPathName(mc.iName, Name, Name.Length);

                        Name = GetCurrPath(Name);

                        //Name = "open " + Convert.ToChar(34) + Name + Convert.ToChar(34) + " alias " + id.ToString() + "";

                        Name = "open " + Name + " alias " + id.ToString();

                        ilong = APIClass.mciSendString("close all", TemStr, TemStr.Length, 0);

                        ilong = APIClass.mciSendString(Name, TemStr, TemStr.Length, 0);

                        if (ilong != 0) {

                            StringBuilder errorText = new StringBuilder();
                            APIClass.mciGetErrorString(ilong, errorText, 50);

                            MessageBox.Show(errorText.ToString());
                        }

                        ilong = APIClass.mciSendString("set " + id.ToString() + " time format milliseconds", TemStr, TemStr.Length, 0);

                        mc.state = State.mStop;

                    }

                    catch
                    {

                    }

                }

            }

            //播放

            public void play()
            {

                TemStr = "";

                TemStr = TemStr.PadLeft(127, Convert.ToChar(" "));

                APIClass.mciSendString("play " + id.ToString(), TemStr, TemStr.Length, 0);

                mc.state = State.mPlaying;

            }

            //停止

            public void StopT()
            {

                TemStr = "";

                TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));

                ilong = APIClass.mciSendString("close " + id.ToString(), TemStr, 128, 0);

                ilong = APIClass.mciSendString("close all", TemStr, 128, 0);

                mc.state = State.mStop;

            }



            public void Puase()
            {

                TemStr = "";

                TemStr = TemStr.PadLeft(128, Convert.ToChar(" "));

                ilong = APIClass.mciSendString("pause " + id.ToString(), TemStr, TemStr.Length, 0);

                mc.state = State.mPuase;

            }

            private string GetCurrPath(string name)
            {

                if (name.Length < 1) return "";

                name = name.Trim();

                name = name.Substring(0, name.Length - 1);

                return name;

            }

            //总时间

            public string Duration
            {

                get
                {

                    durLength = "";

                    durLength = durLength.PadLeft(128, Convert.ToChar(" "));

                    APIClass.mciSendString("status " + id.ToString() + " length", durLength, durLength.Length, 0);

                    durLength = durLength.Trim();

                    if (durLength == "") return "null";

                    return durLength;

                }

            }

            public int seek(int position)
            {
                APIClass.mciSendString(string.Format("seek " + id.ToString() + " to {0}",position.ToString()),"",0,0);
                this.play();
                return 1;
            }
            public int seekwithoutplay(int position)
            {
                APIClass.mciSendString(string.Format("seek " + id.ToString() + " to {0}", position.ToString()), "", 0, 0);
                return 1;
            }

            //当前时间

            public string CurrentPosition
            {

                get
                {

                    durLength = "";

                    durLength = durLength.PadLeft(128, Convert.ToChar(" "));

                    APIClass.mciSendString("status " + id.ToString() + " position", durLength, durLength.Length, 0);

                    //mc.iPos = (int)(Convert.ToDouble(durLength) / 1000f);

                    return durLength.Trim();

                }

            }

        }



        public class APIClass
        {

            [DllImport("kernel32.dll", CharSet = CharSet.Auto)]

            public static extern int GetShortPathName(

            string lpszLongPath,

            string shortFile,

            int cchBuffer

            );



            [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Auto)]

            public static extern int mciSendString(

            string lpstrCommand,

            string lpstrReturnString,

            int uReturnLength,

            int hwndCallback

            );
            [DllImport("winmm.dll")]
            public static extern bool mciGetErrorString(Int32 errorCode, StringBuilder errorText, Int32 errorTextSize);

        }


        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            timer1.Enabled = false;
            label1.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            cm.FileName = label1.Text;
            totalTimeLabel.Text = formatLongToTimeStr(long.Parse(cm.Duration));
            trackBar1.Maximum = int.Parse(cm.Duration);
            timer1.Enabled = true;
            for (int i = 0; i < 5; i++) addlist("0");
            button1.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
            button7.Enabled = true;
            button9.Enabled = true;
            button10.Enabled = true;
        }

        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            cm.play();
            button2.Enabled = true;
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            cm.Puase();
            button1.Enabled = true;
            button2.Enabled = false;
            addlist(cm.CurrentPosition);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            cm.seek(int.Parse(breaklist[0]));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cm.seek(int.Parse(breaklist[1]));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            cm.seek(int.Parse(breaklist[2]));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            cm.seek(int.Parse(breaklist[3]));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            cm.seek(int.Parse(breaklist[4]));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++) addlist("0");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            trackBar1.Value = int.Parse(cm.CurrentPosition);
            currentTimeLabel.Text = formatLongToTimeStr(long.Parse(cm.CurrentPosition));
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (cm.mc.state == clsMCI.State.mPlaying)
            {
                cm.seek(trackBar1.Value);
            }
            else
            {
                cm.seekwithoutplay(trackBar1.Value);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            cm.FileName = "C:\\Users\\Yukimir\\Music\\1.mp3";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            cm.FileName = "C:\\Users\\Yukimir\\Music\\2.mp3";
        }

        private void button9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                a = cm.CurrentPosition;
                button9.Text = formatLongToTimeStr(long.Parse(a));
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            cm.seek(int.Parse(a));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            cm.seek(int.Parse(b));
            if (button1.Enabled == true)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }
        }

        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                b = cm.CurrentPosition;
                button10.Text = formatLongToTimeStr(long.Parse(b));
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MessageBox.Show("暂停时当前进度会在填充进下方的进度槽\n"+
                            "（当超过五个时，最旧的一个会被删除）\n"+
                            "点击进度按钮可以从相应的进度开始播放。\n"+
                            "最下方的A/B按键是自定义记录点，右键点击按键可以记录当前进度\n"+
                            "左键点击跳至记录的进度");
        }

    }
}
