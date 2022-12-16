using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic;

namespace GKHelper
{
    public partial class TaskbarApplet : UserControl
    {
        public List<Lesson> lessons = new List<Lesson>();
        bool marquee = true;

        public TaskbarApplet()
        {
            InitializeComponent();


            //Try to get current date and load given file
            Console.WriteLine("Loading Date Configuration File");
            DateTime now = DateTime.Now;
            DateTime date = DateTime.Today;
            Console.WriteLine(now + " " + date);

            try
            {
                string name = "C:/GKB/Config_" + now.DayOfWeek.ToString() + ".txt";
                Console.WriteLine(name);
                if (!File.Exists(name))
                {
                    string user = Interaction.InputBox("Cannot find configuration file:" + name + "\nPlease specify another:", "No such file", "");
                    name = "C:/GKB/Config_" + user + ".txt";
                }

                BandLoadTimetable(name);
                TickNews();
            }
            catch (Exception e)
            {
                MessageBox.Show(e + "\n" + e.StackTrace, "Fatal Error!!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine(e + "\n" + e.StackTrace);
                Environment.Exit(1);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (marquee)
            {
                label2.Left--;
                if (label2.Left + label2.Width < 0)
                {
                    label2.Left = this.Width;
                }
            }
        }
        public DateTime AppendTime(DateTime date, string str)
        {
            string[] info = str.Split(':');
            return date.AddHours(Double.Parse(info[0])).AddMinutes(Double.Parse(info[1]));
        }
        public void BandLoadTimetable(string name)
        {
            DateTime date = DateTime.Today;
            using (StreamReader sr = new StreamReader(name))
            {
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();
                sr.ReadLine();

                string line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line == "")
                    {
                        continue;
                    }
                    Console.WriteLine(line);
                    string[] res = line.Split(' ');
                    Lesson l = new Lesson(AppendTime(date, res[0]), AppendTime(date, res[1]), res[2]);
                    Console.WriteLine("New lesson:" + l);
                    lessons.Add(l);
                }
                Console.WriteLine(lessons.Count + " lessons read!");
            }
        }

        private void TaskbarApplet_Load(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender,EventArgs e)
        {
            TickNews();
        }
        public void TickNews()
        {
            marquee = true;
            DateTime now = DateTime.Now;
            bool found = false;

            for (int i = 0; i < lessons.Count; i++)
            {
                Lesson l = lessons[i];

                if (l.begin <= now && l.end >= now) //in this class
                {
                    label2.Text = "还有" + (int)l.end.Subtract(now).TotalMinutes + "分";
                    label2.Left = 100;
                    label1.Text = l.subject;
                    marquee = false;

                    found = true;
                    break;
                }
                else if (l.begin > now)
                {
                    string x="下一节："+ (int)l.begin.Subtract(now).TotalMinutes + "分后 "+l.subject;
                    if (i != lessons.Count - 1)
                    {
                        x += "。然后是";
                        for (int j = i+1; j < lessons.Count; j++)
                        {
                            x += lessons[j].subject;
                            if (j != lessons.Count - 1)
                            {
                                x += "、";
                            }
                            else
                            {
                                x += "。";
                            }
                        }
                    }
                    label2.Text = x;
                    label1.Text = l.subject;

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                label2.Text = "今日课程已全部结束！";
                label1.Text = "回家";
            }
        }
    }
}
