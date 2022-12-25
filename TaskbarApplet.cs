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
using log4net;
using Jint;

namespace GKHelper
{
    public partial class TaskbarApplet : UserControl
    {
        public ILog log;

        public List<Lesson> lessons = new List<Lesson>();
        public List<String> scripts = new List<String>();

        public int tick = 0;

        public Engine jse;

        public TaskbarApplet()
        {
            InitializeComponent();

            log = log4net.LogManager.GetLogger("Band");
            log.Info("Logger started successfully");

            //Try to get current date and load given file
            log.Info("Loading Date Configuration File");
            DateTime now = DateTime.Now;
            DateTime date = DateTime.Today;
            log.Debug(now + " " + date);

            try
            {
                string name = "C:/GKB/Config_" + now.DayOfWeek.ToString() + ".txt";
                log.Debug(name);
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
                log.Fatal("Cannot load timetable", e);
                Environment.Exit(1);
            }

            //Try loading scripts
            log.Info("Loading scripts");
            foreach(string s in Directory.EnumerateFiles("C:/GKB"))
            {
                string name = Path.GetFileName(s);
                if (name.EndsWith(".js"))
                {
                    log.Info("Loading:" + name);
                    using(StreamReader sr=new StreamReader(s))
                    {
                        scripts.Add(sr.ReadToEnd());
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Left--;
            if (label2.Left + label2.Width < 0)
            {
                label2.Left = this.Width;
                TickNews();
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
                    log.Debug(line);
                    string[] res = line.Split(' ');
                    Lesson l = new Lesson(AppendTime(date, res[0]), AppendTime(date, res[1]), res[2]);
                    log.Info("New lesson:" + l);
                    lessons.Add(l);
                }
                log.Info(lessons.Count + " lessons read!");
            }
        }

        private void TaskbarApplet_Load(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender,EventArgs e)
        {
            DateTime now = DateTime.Now;

            label2.Visible = true;
            label3.Visible = false;
            for (int i = 0; i < lessons.Count; i++)
            {
                Lesson l = lessons[i];

                if (l.begin <= now && l.end >= now) //in this class
                {
                    label2.Visible = false;
                    label3.Visible = true;
                    break;
                }
                else if (l.begin > now)
                {
                    break;
                }
            }
    }

        public void TickNews()
        {
            tick++;
            DateTime now = DateTime.Now;
            bool found = false;

            for (int i = 0; i < lessons.Count; i++)
            {
                Lesson l = lessons[i];

                if (l.begin <= now && l.end >= now) //in this class
                {
                    label3.Text = label2.Text = "还有" + (int)l.end.Subtract(now).TotalMinutes + "分";
                    label1.Text = l.subject;

                    found = true;
                    break;
                }
                else if (l.begin > now)
                {
                    string x = "下一节：" + (int)l.begin.Subtract(now).TotalMinutes + "分后 " + l.subject;
                    if (i != lessons.Count - 1)
                    {
                        x += "。然后是";
                        for (int j = i + 1; j < lessons.Count; j++)
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
            
            if(tick%2==1 && scripts.Count>0) //override with script return
            {
                try { 
                    var rng = scripts[new Random().Next(0, scripts.Count-1)];
                    jse = new Engine().SetValue("log", new Action<object>(log.Debug));
                    string str=(string)jse.Execute(rng).Evaluate("main()").ToObject();
                    label2.Text = str;
                    log.Debug("Loaded script content:" + str);
                }
                catch(Exception ex)
                {
                    log.Error("Failed to load script content:", ex);
                }
            }
        }
    }
}
