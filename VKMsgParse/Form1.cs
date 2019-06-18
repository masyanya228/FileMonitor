using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tree;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using Google.Cloud.Vision.V1;
using Image = Google.Cloud.Vision.V1.Image;
using Google.Apis.Auth.OAuth2;

namespace VKMsgParse
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }
        Tree.Tree mainCl;
        string file = "";
        private YandexPredictor yandexPredictor;

        private void button2_Click(object sender, EventArgs e)
        {
            Do(textBox1.Text);
        }
        /// <summary>
        /// Old for Vlada
        /// </summary>
        void OldDo()
        {
            StreamReader r = new StreamReader(textBox1.Text + "\\Влада.html");
            file = r.ReadToEnd();
            r.Close();
            file = file.ToLower();
            file = file.Replace("&quot", "").Replace("&br", "").Replace(",", "").Replace(".", "").Replace(";", "").Replace("?", "").Replace("!", "").Replace("(", "").Replace(")", "");
            mainCl = Tree.Tree.BuildTree(file);
            List<Tree.Tree> cl = Tree.Tree.FindAllByName(mainCl, "class=\"msg_item\"", Tree.Tree.SearchType.byWord);
            /*for (int i = 0; i < cl.Count; i++)
            {
                int g = Tree.Tree.GetLength(cl[i]);
                if (g != 14)
                {
                    cl.RemoveAt(i);
                    i--;
                }
            }*/
            int col = 0;
            for (int i = 0; i < cl.Count; i++)
            {
                Tree.Tree fcl = Tree.Tree.FindByName(cl[i], "msg_body");
                Tree.Tree fcln = Tree.Tree.FindByName(cl[i], "from");
                if (fcl != null)
                {
                    Msg m = new Msg();
                    m.msg = Tree.Tree.ConcateStrings(fcl.GetStrings());
                    m.name = fcln.GetStrings()[0];
                    string[] words = m.msg.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    col += words.Length;
                    Msg.msgs.Add(m);
                    for (int g = 0; g < words.Length; g++)
                    {
                        Rang h = Rang.GetRang(words[g]);
                        if (h != null)
                        {
                            h.col++;
                        }
                        else
                        {
                            Rang f = new Rang();
                            f.col = 1;
                            f.word = words[g];
                            Rang.rangs.Add(f);
                        }
                    }
                }
                else
                {
                    cl.RemoveAt(i);
                    i--;
                }
            }
            Rang[] rngs = Rang.GetRangs();
            string sum = cl.Count + "\r\n" + rngs.Length + "\r\n";
            int col1 = 0, col2 = 0;
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                if (Msg.msgs[i].name == "марсель хабибуллин")
                {
                    col1 += Msg.msgs[i].msg.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                }
                else
                {
                    col2 += Msg.msgs[i].msg.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                }
            }
            sum += "Марсель " + col1 + "\r\n" + "Владулечка " + col2 + "\r\n";
            for (int i = 0; i < rngs.Length; i++)
            {
                if (rngs[i].col >= 5)
                    sum += rngs[i].word + " " + rngs[i].col + "\r\n";
            }
            ///textBox2.Text = sum;
            Rang rn = Rang.GetRang("главное");
        }

        void Do(string directory)
        {
            Dictionary<string, int> urls = new Dictionary<string, int>();
            if (!Directory.Exists(directory))
                return;
            string[] files = Directory.GetFiles(directory);
            for (int i = 0; i < files.Length; i++)
            {
                try
                {
                    if (files[i].Length > 200)
                    {
                        //string name=files[i].Substring(files[i].IndexOf)
                        //Захир Рахимов(145782194),Руслан Бурков(157572649),Кирилл Кукушкин(174486179),Ангелина Муратова(193083024),Маша Котова(233992959),Макс Мирный(258802397),Владимир Даурский(380886042),Алина Немец(4793 (2)
                    }
                    StreamReader r = new StreamReader(files[i]);
                    string str = r.ReadToEnd();
                    r.Close();
                    str = str.Replace("</div></body>", "").Replace("</html>", "").Replace("	", "");
                    string[] s = str.Split(new string[] { "div id=\""/*"class=\"msg_item\""*/ }, StringSplitOptions.None);
                    //1,2,3,4,5 last - id
                    for (int g = 1; g < s.Length; g++)
                    {
                        Msg m = new Msg();
                        //string[] s2 = s[g].Split(new string[] { "><", }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> s2 = new List<string>();
                        s2.AddRange(s[g].Split(new string[] { "<div class=\"upic\">", "<img src=", "></div>", "</div>", "<div class=\"from\">", "<b>", "</b>", "<a href=", "</a>", "<div class=\"msg_body\">", "<div id=", "<div class=\"attacments\">", "<div class=\"attacment\">", "<a target=\"_blank\" href=", "<div class=\"att_ico att_photo" }, StringSplitOptions.RemoveEmptyEntries));
                        for (int h = 0; h < s2.Count; h++)
                        {
                            s2[h] = s2[h].Trim(new char[] { '\"', ' ' });
                            if (s2[h].Length == 0)
                            {
                                s2.RemoveAt(h);
                                h--;
                                continue;
                            }
                        }

                        for (int h = 0; h < s2.Count; h++)
                        {
                            if (s2[h].IndexOf("class=\"emoji_css\"") >= 0)
                            {
                                string[] s3 = s2[h].Split(new string[] { " ", }, StringSplitOptions.RemoveEmptyEntries);
                                for (int j = 0; j < s3.Length; j++)
                                {
                                    //http://vk.com/images/blank.gif\" emoji=\"D83DDE06\" alt=\"😆\" class=\"emoji_css\" style=\"background-position: 0px -51px;\" />"))
                                    if (s3[j].StartsWith("alt"))
                                    {
                                        s2[h] = s3[j].Substring(4).Trim(new char[] { '\"' });
                                    }
                                }
                            }
                            else if (s2[h].IndexOf("<img class=\"emoji\"") >= 0)
                            {
                                string[] s3 = s2[h].Split(new string[] { " ", }, StringSplitOptions.RemoveEmptyEntries);
                                string endtxt = "";
                                for (int j = 0; j < s3.Length; j++)
                                {
                                    //<img class=\"emoji\" alt=\"☺\" src=\"http://vk.com/images/emoji/263A.png\" 
                                    if (s3[j].StartsWith("alt"))
                                    {
                                        endtxt += s3[j].Substring(4).Trim(new char[] { '\"' });
                                    }
                                    else if (s3[j].IndexOf(">") >= 0)
                                    {
                                        endtxt += s3[j].Substring(s3[j].IndexOf(">") + 1);
                                    }
                                }
                                s2[h] = endtxt;
                            }
                            if (h == 0)
                            {
                                int npos = s2[h].IndexOf("\"");
                                if (npos == -1)
                                    npos = s2[h].Length;
                                m.msgid = s2[h].Substring(3, npos - 3);
                            }
                            else if (h == 1)
                            {
                                if (s2[h] == "")
                                    s2 = s2;
                                int npos = s2[h].IndexOf("\"");
                                if (npos == -1)
                                    npos = s2[h].Length;
                                m.avaurl = s2[h].Substring(0, npos);
                            }
                            else if (h == 2)
                            {
                                if (s2[h] == "")
                                    s2 = s2;
                                m.name = s2[h];
                            }
                            else if (h == 3)
                            {
                                if (s2[h] == "")
                                    s2 = s2;
                                int npos = s2[h].IndexOf("\"");
                                if (npos == -1)
                                    npos = s2[h].Length;
                                m.senderurl = s2[h].Substring(0, npos);
                            }
                            else if (h == 4)
                            {
                                int npos = Math.Max(0, s2[h].IndexOf(">") + 1);
                                s2[h] = s2[h].Substring(npos);
                                if (s2[h] == "")
                                    s2 = s2;
                                if (!DateTime.TryParse(s2[h], out m.date))
                                    s2 = s2;
                            }
                            else if (h == 5)
                            {
                                if (s2[h] == "")
                                    s2 = s2;
                                if (s2[h] == "<div class=\"att_head\"> <div class=\"att_ico att_fwd")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_geo")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_video")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_audio")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_doc")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_sticker")
                                {

                                }
                                else if (s2[h].StartsWith("alt"))
                                {
                                    m.msg += s2[h].Substring(4).Trim(new char[] { '\"' });
                                }
                                else
                                {
                                    m.msg += s2[h];
                                }
                            }
                            else if (h == s2.Count - 1)
                            {
                                if (s2[h] != "<")
                                    s2 = s2;

                            }
                            else if (h > 5)
                            {
                                if (s2[h].IndexOf("http") >= 0)
                                {
                                    int stpos = s2[h].IndexOf("http");
                                    int npos = s2[h].IndexOf("\"");
                                    string url = "";
                                    if (npos > -1)
                                    {
                                        url = s2[h].Substring(stpos, npos - stpos);
                                    }
                                    else
                                    {
                                        url = s2[h].Substring(stpos);
                                    }
                                    m.attachesUrl.Add(url);
                                }
                                continue;
                                //fwd
                                /*if (s2[h] == "")
                                    s2 = s2;
                                if (s2[h].StartsWith("alt"))
                                    m.msg += s2[h].Substring(4).Trim(new char[] { '\"' });
                                else if (s2[h] == "<div class=\"att_ico att_geo")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_video")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_audio")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_doc")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_ico att_sticker")
                                {

                                }
                                else if (s2[h] == "<div class=\"att_head\"> <div class=\"att_ico att_fwd")
                                {

                                }
                                else if (s2[h] == "<div class=\"fwd\"><div class=\"msg_item\">" | s2[h] == "<div class=\"msg_item\">")
                                {
                                    for (int nc = 1; h < s2.Count - 1 & nc <= 5; nc++)
                                    {
                                        h++;
                                        int npos = s2[h].IndexOf("\"");
                                        if (s2[h] == "Материалы:")
                                        {
                                            m.msg += "\r\n" + s2[h];
                                        }
                                        else if (s2[h] == "Пересланные сообщения:")
                                        {
                                            m.msg += "\r\n" + s2[h];
                                        }
                                        else if (s2[h] == "<div class=\"att_head\"> <div class=\"att_ico att_fwd")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"fwd\"><div class=\"msg_item\">")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"msg_item\">")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"att_ico att_geo")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"att_ico att_video")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"att_ico att_audio")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"att_ico att_doc")
                                        {

                                        }
                                        else if (s2[h] == "<div class=\"att_ico att_sticker")
                                        {

                                        }
                                        else if (s2[h].IndexOf("<img class=\"") >= 0)
                                        {

                                        }
                                        else
                                        {
                                            if (npos == -1)
                                            {
                                                npos = s2[h].Length;
                                                if (s2[h].IndexOf("http") >= 0)
                                                {
                                                    m.attachesUrl.Add(s2[h].Substring(0, npos));
                                                }
                                                else
                                                {
                                                    m.msg += s2[h];
                                                }
                                            }
                                            else
                                                m.attachesUrl.Add(s2[h].Substring(0, npos));
                                        }
                                    }
                                }
                                else
                                {
                                    int npos = s2[h].IndexOf("\"");
                                    if (s2[h] == "Материалы:")
                                    {
                                        m.msg += "\r\n" + s2[h];
                                    }
                                    else if (s2[h] == "Пересланные сообщения:")
                                    {
                                        m.msg += "\r\n" + s2[h];
                                    }
                                    else
                                    {
                                        if (npos == -1)
                                        {
                                            npos = s2[h].Length;
                                            if (s2[h].IndexOf("http") >= 0)
                                            {
                                                m.attachesUrl.Add(s2[h].Substring(0, npos));
                                            }
                                            else
                                            {
                                                m.msg += s2[h];
                                            }
                                        }
                                        else
                                            m.attachesUrl.Add(s2[h].Substring(0, npos));
                                    }
                                }*/
                            }
                        }
                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(m.msg, myWriter);
                        m.msg = new StringBuilder(myWriter.ToString()).ToString();
                        if (s2.Count != 7)
                            s2 = s2;
                        if (s2.Count > 5)
                            Msg.msgs.Add(m);
                        else
                        {

                        }
                    }
                    //END PARSE
                }
                catch (Exception ex)
                {

                }
            }

            Msg.msgs = Msg.msgs.OrderBy(x => x.date).ToList();

            var listmsgswithatach = new List<Msg>();
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                if (Msg.msgs[i].attachesUrl.Count > 0)
                {
                    listmsgswithatach.Add(Msg.msgs[i]);
                    for (int n = 0; n < Msg.msgs[i].attachesUrl.Count; n++)
                    {
                        if (Msg.msgs[i].attachesUrl[n] == "https://sun1-15.userapi.com/c831409/v831409158/106e0e/MjNgTKKs2KM.jpg")
                        {
                            var fddgh = Msg.msgs[i];
                        }
                        if (urls.ContainsKey(Msg.msgs[i].attachesUrl[n]))
                        {
                            urls[Msg.msgs[i].attachesUrl[n]]++;
                        }
                        else
                        {
                            urls.Add(Msg.msgs[i].attachesUrl[n], 1);
                        }
                    }
                }
            }
            listBox1.Items.AddRange(urls.Keys.ToArray());


            int lastid = int.Parse(Msg.msgs[0].msgid);
            var listmisstakes = new List<Msg>();
            int sumremoved = 0;
            List<int> remlist = new List<int>();
            for (int i = 1; i < Msg.msgs.Count; i++)
            {
                int id = int.Parse(Msg.msgs[i].msgid);
                if (lastid + 1 != id & Msg.msgs[i - 1].name == Msg.msgs[i].name & Msg.msgs[i - 1].name == "Алина Немец")
                {
                    listmisstakes.Add(Msg.msgs[i - 1]);
                    listmisstakes.Add(Msg.msgs[i]);
                    sumremoved += (id - lastid - 1);
                    remlist.Add(id - lastid - 1);
                }
                lastid = id;
            }
        }
        class Msg
        {
            public static List<Msg> msgs = new List<Msg>();
            public string avaurl;
            public string name;
            public string senderurl;
            public DateTime date;
            public string msg;
            public List<string> attachesUrl = new List<string>();
            public string msgid;
        }
        class Rang
        {
            public static List<Rang> rangs = new List<Rang>();
            public int col;
            public string word;
            public static Rang GetRang(string msg)
            {
                for (int i = 0; i < rangs.Count; i++)
                {
                    if (rangs[i].word == msg)
                        return rangs[i];
                }
                return null;
            }

            public static Rang[] GetRangs()
            {
                List<Rang> ot = rangs;
                int c = 1;
                while (c > 0)
                {
                    c = 0;
                    for (int i = 1; i < ot.Count; i++)
                    {
                        if (ot[i - 1].col >= ot[i].col)
                        {

                        }
                        else
                        {
                            c++;
                            Rang fr = ot[i - 1];
                            ot[i - 1] = ot[i];
                            ot[i] = fr;
                        }
                    }
                }
                return ot.ToArray();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ApiTest("C:\\Users\\Регина\\Desktop\\Дип\\Авторизация.PNG");
            speech = new Speech();
            sightengine = new Sightengine("&api_user=1662965073&api_secret=dtndtQ3d79k7mQxEKu6M");
            yandexPredictor = new YandexPredictor("pdct.1.1.20190615T105757Z.634d42853c41a01a.ed3a4154c7ced1884a81d656dd61a70e81b0dec3");

            //webBrowser1.Navigate("https://psv4.userapi.com/c852632//u146667314/audiomsg/d11/eafe6fa850.ogg");
            //OldDo();
            Button but = new Button();
            LinkLabel link = new LinkLabel();
            //ReadAll();
            int sttime = Environment.TickCount;
            Do(textBox1.Text);
            int col = Msg.msgs.Count;
            sttime = Environment.TickCount - sttime;

            sttime = Environment.TickCount;
            string[] urls = GetAllPhotos();
            sttime = Environment.TickCount - sttime;

            sttime = Environment.TickCount;
            SortAllByDate();
            sttime = Environment.TickCount - sttime;

            sttime = Environment.TickCount;
            int[] res = GetGraph();
            sttime = Environment.TickCount - sttime;

            sttime = Environment.TickCount;
            sttime = Environment.TickCount;
            res = GetGraph();
            sttime = Environment.TickCount - sttime;
            /*StreamWriter w = new StreamWriter(Environment.CurrentDirectory + "\\Cash.txt");
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                w.WriteLine(Msg.msgs[i].name + "\r\n&" + Msg.msgs[i].senderurl + "\r\n&" + Msg.msgs[i].date + "\r\n&" + Msg.msgs[i].avaurl + "\r\n&" + Msg.msgs[i].msg + "\r\n&" + Msg.msgs[i].msgid);
                for (int j = 0; j < Msg.msgs[i].attachesUrl.Count; j++)
                {
                    w.Write(Msg.msgs[i].attachesUrl[j] + "\r\n&");
                }
                w.WriteLine();
                w.WriteLine();
            }
            w.Close();*/
            sttime = Environment.TickCount - sttime;
            /*for (int i = 0; i < Msg.msgs.Count; i++)
            {
                link = new LinkLabel();
                link.Text = Msg.msgs[i].name;
                link.LinkClicked += linkLabel1_LinkClicked;
                link.Tag = Msg.msgs[i].senderurl;
                tableLayoutPanel1.Controls.Add(link, 0, i);
                but = new Button();
                but.Text = Msg.msgs[i].date.ToShortDateString();
                tableLayoutPanel1.Controls.Add(but, 1, i);
                but = new Button();
                but.Text = Msg.msgs[i].msg;
                tableLayoutPanel1.Controls.Add(but, 2, i);
                but = new Button();
                but.Text = Msg.msgs[i].senderurl;
                tableLayoutPanel1.Controls.Add(but, 3, i);
                but = new Button();
                but.Text = Msg.msgs[i].msgid;
                tableLayoutPanel1.Controls.Add(but, 4, i);
                break;
            }*/
        }

        private static void ApiTest(string filePath)
        {
            var image = Image.FromFile(filePath);
            var client = ImageAnnotatorClient.Create();
            var response = client.DetectImageProperties(image);
            string header = "Red\tGreen\tBlue\tAlpha\n";
            foreach (var color in response.DominantColors.Colors)
            {
                Console.Write(header);
                header = "";
                Console.WriteLine("{0}\t{0}\t{0}\t{0}",
                    color.Color.Red, color.Color.Green, color.Color.Blue,
                    color.Color.Alpha);
            }
        }
        /*public object AuthImplicit(string projectId)
        {
            // If you don't specify credentials when constructing the client, the
            // client library will look for credentials in the environment.
            var credential = GoogleCredential.GetApplicationDefault();
            var storage = StorageClient.Create(credential);
            // Make an authenticated API request.
            var buckets = storage.ListBuckets(projectId);
            foreach (var bucket in buckets)
            {
                Console.WriteLine(bucket.Name);
            }
            return null;
        }*/

        Dictionary<string, UInt64> GetMsgCol()
        {
            Dictionary<string, UInt64> ot = new Dictionary<string, ulong>();
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                string et = Msg.msgs[i].name;
                ulong col = 1;
                while (i + (int)col < Msg.msgs.Count & Msg.msgs[i + (int)col].name == et)
                {
                    col++;
                }
                bool f = false;
                foreach (string s in ot.Keys)
                {
                    if (s == Msg.msgs[i].name)
                    {
                        f = true;
                        ot[s] += col;
                        break;
                    }
                }
                if (!f)
                {
                    ot.Add(Msg.msgs[i].name, 1);
                }
            }
            return ot;
        }
        int[] GetGraph()
        {
            List<int> ot = new List<int>();
            if (Msg.msgs.Count == 0)
                return ot.ToArray();
            DateTime start = Msg.msgs[0].date;
            DateTime end = Msg.msgs[Msg.msgs.Count - 1].date;
            DateTime po = start;

            for (int i = 0; i < Msg.msgs.Count;)
            {
                ot.Add(0);
                bool b = false;
                while (Msg.msgs[i].date.Subtract(po).TotalHours < 24)
                {
                    b = true;
                    if (Msg.msgs[i].name == "Марсель Хабибуллин")
                    {
                        ot[ot.Count - 1] += Msg.msgs[i].msg.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                    i++;
                    if (i >= Msg.msgs.Count)
                        break;
                }
                if (!b)
                    i++;
                po = po.AddDays(1);
            }
            string toexcel = "";
            for (int i = 0; i < ot.Count; i++)
            {
                toexcel += ot[i] + "\r\n";
            }
            Clipboard.SetText(toexcel);
            return ot.ToArray();
        }
        void ReadAll()
        {
            StreamReader r = new StreamReader(Environment.CurrentDirectory + "\\Cash.txt");
            string[] str = r.ReadToEnd().Split(new string[] { "\r\n\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < str.Length; i++)
            {
                string[] str2 = str[i].Split(new string[] { "\r\n&" }, StringSplitOptions.RemoveEmptyEntries);
                Msg m = new Msg();
                if (str2.Length > 5)
                {
                    m.name = str2[0];
                    m.senderurl = str2[1];
                    DateTime.TryParse(str2[2], out m.date);
                    m.avaurl = str2[3];
                    m.msg = str2[4];
                    m.msgid = str2[5];
                    for (int n = 5 + 1; n < str2.Length; n++)
                    {
                        m.attachesUrl.Add(str2[n]);
                    }
                    Msg.msgs.Add(m);
                }
                /*r.ReadLine(Msg.msgs[i].name + "\r\n" + Msg.msgs[i].senderurl + "\r\n" + Msg.msgs[i].date + "\r\n" + Msg.msgs[i].avaurl + "\r\n" + Msg.msgs[i].msg + "\r\n" + Msg.msgs[i].msgid);
                for (int j = 0; j < Msg.msgs[i].attachesUrl.Count; j++)
                {
                    r.ReadLine(Msg.msgs[i].attachesUrl[j]);
                }
                r.ReadLine();*/
            }
        }
        void SortAllByDate()
        {
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                bool tr = false;
                for (int j = i - 1; j >= 0; j--)
                {
                    //if (Msg.msgs[i].senderurl == Msg.msgs[j].senderurl & "http://vk.com/id146667314" != Msg.msgs[i].senderurl)
                    //continue;
                    string fstr = Msg.msgs[i].date.ToLongTimeString() + " " + Msg.msgs[j].date.ToLongTimeString();
                    int res = Msg.msgs[i].date.CompareTo(Msg.msgs[j].date);
                    if (res == 1)
                    {
                        if (j + 1 != i)
                        {
                            //Msg fm = Msg.msgs[j];
                            Msg.msgs.Insert(j + 1, Msg.msgs[i]);
                            Msg.msgs.RemoveAt(i + 1);
                        }
                        tr = true;
                        break;
                    }
                    else
                    {

                    }
                }
                if (!tr)
                {
                    Msg.msgs.Insert(0, Msg.msgs[i]);
                    Msg.msgs.RemoveAt(i + 1);
                }
            }
        }
        string[] GetAllPhotos()
        {
            List<string> ot = new List<string>();
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                for (int j = 0; j < Msg.msgs[i].attachesUrl.Count; j++)
                {
                    string clearurl = Msg.msgs[i].attachesUrl[j];
                    int npos = clearurl.IndexOf("?");
                    if (npos >= 0)
                    {
                        clearurl = clearurl.Substring(0, npos);
                    }
                    if (clearurl == "<div class=")
                        clearurl = clearurl;
                    if (!ot.Contains(clearurl))
                    {
                        ot.Add(clearurl);
                    }
                }
            }
            return ot.ToArray();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start((sender as LinkLabel).Tag as string);
            /*sttime = Environment.TickCount;
            StreamWriter w = new StreamWriter(Environment.CurrentDirectory + "\\Cash.txt");
            for (int i = 0; i < Msg.msgs.Count; i++)
            {
                w.WriteLine(Msg.msgs[i].name + "\r\n" + Msg.msgs[i].senderurl + "\r\n" + Msg.msgs[i].date + "\r\n" + Msg.msgs[i].avaurl + "\r\n" + Msg.msgs[i].msg + "\r\n" + Msg.msgs[i].msgid);
                for (int j = 0; j < Msg.msgs[i].attachesUrl.Count; j++)
                {
                    w.WriteLine(Msg.msgs[i].attachesUrl[j]);
                }
                w.WriteLine();
            }
            w.Close();
            sttime = Environment.TickCount - sttime;*/
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                webBrowser1.Navigate(listBox1.SelectedItem.ToString());
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.IndexOf('\t') >= 0)
            {
                int pos = textBox2.Text.IndexOf('\t');
                textBox2.Text = textBox2.Text.Replace("\t", "");
                textBox2.SelectionStart = pos;
                textBox2.SelectionLength = 0;
            }
            //string text = textBox2.Text;
            if (yandexPredictor.Predict(textBox2.Text, out prediction, out pos))
                //yandexPredictor.SetPrediction(ref text, prediction, pos);
                label1.Text = prediction;
        }
        int pos = 0;
        string prediction = "";
        private Speech speech;
        private Sightengine sightengine;

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            /*if (e.KeyCode == Keys.ShiftKey)
            {
                textBox2.Text = label1.Text;
                textBox2.SelectionStart = textBox2.TextLength;
            }*/
            if (e.KeyCode == Keys.Tab)
            {
                string text = textBox2.Text;
                yandexPredictor.SetPrediction(ref text, prediction, pos);
                textBox2.Text = text;
                textBox2.SelectionStart = textBox2.TextLength;
                textBox2.SelectionLength = 0;
            }
            else if (e.KeyCode == Keys.Space & pos == 0)
            {
                string text = textBox2.Text;
                yandexPredictor.SetPrediction(ref text, prediction, pos);
                textBox2.Text = text;
                textBox2.SelectionStart = textBox2.TextLength;
                textBox2.SelectionLength = 0;
            }
        }

        private void TextBox2_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var result = sightengine.Analisys(textBox3.Text, Sightengine.Model.nudity | Sightengine.Model.wad | Sightengine.Model.scam | Sightengine.Model.properties | Sightengine.Model.faces | Sightengine.Model.face_attributes | Sightengine.Model.celebrities);
            if (result == null)
            {
                speech.Speak("Ошибка");
            }
        }
    }
}