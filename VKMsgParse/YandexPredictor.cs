using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKMsgParse
{
    public class YandexPredictor
    {
        string key;
        //pdct.1.1.20190615T105757Z.634d42853c41a01a.ed3a4154c7ced1884a81d656dd61a70e81b0dec3
        public YandexPredictor(string key)
        {
            this.key = key;
            string pre = Prepare("привет");
            pre = Prepare("привет ");
            pre = Prepare("привет как");
            pre = Prepare("привет как ");
            pre = Prepare("привет     как ");
            pre = Prepare("привет как дела");
            pre = Prepare("привет как дела ");
        }
        public bool Predict(string text, out string prediction, out int posDif)
        {
            if (text.Length > 1000)
                text = text.Substring(text.Length - 1000);
            text = Prepare(text);
            var search = Cache.GetRes(text, typeof(YandexPredictor), new TimeSpan(0, 60, 0));
            if (search != null & false)
            {
                var res = search as Dictionary<string, object>;
                prediction = res["text"] as string;
                posDif = int.Parse(res["pos"].ToString());
                return true;
            }
            else
            {
                string res = null;
                string predict = "";
                int pos = 0;
                try
                {
                    res = GET("key=" + key + "&q=" + text + "&lang=ru");
                    string com = res;
                    string[] args = com.Replace("\"", "").Split(new string[] { ",", "{", "[", "}", "]" }, StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length >= 4)
                    {
                        var par = TrueSplit(args[0], ':');
                        if (par[0] == "endOfWord")
                            bool.TryParse(par[1], out bool endOfWord);
                        par = TrueSplit(args[1], ':');
                        if (par[0] == "pos")
                            int.TryParse(par[1], out pos);
                        if (args[2] == "text:")
                            predict = args[3];
                        prediction = predict;
                        posDif = pos;
                        Dictionary<string, object> results = new Dictionary<string, object>();
                        results.Add("text", prediction);
                        results.Add("pos", posDif);
                        Cache.SetRes(text, typeof(YandexPredictor), results);
                        return true;
                    }
                    else
                    {
                        prediction = null;
                        posDif = 0;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    prediction = null;
                    posDif = 0;
                    return false;
                }
            }
        }
        private string Prepare(string text)
        {
            text = CleanUpDoubles(text, ' ');
            int pos = text.Length;
            int target = 3;
            for (int i = 0; i < target; i++)
            {
                if (text.LastIndexOf(' ', pos - 1) >= 0)
                {
                    pos = text.LastIndexOf(' ', pos - 1);
                    if (pos == text.Length-1 & i == 0)
                        target++;
                }
                else
                {
                    pos = 0;
                    break;
                }
            }
            return text.Substring(pos);
        }
        private string CleanUpDoubles(string text, char v)
        {
            while (text != text.Replace(new string(v, 2), v.ToString()))
                text = text.Replace(new string(v, 2), v.ToString());
            return text;
        }
        public void SetPrediction(ref string text, string prediction, int pos)
        {
            if (pos >= 0)
                text += new string(' ', pos) + prediction;
            else
            {
                text = text.Remove(text.Length + pos) + prediction;
            }
        }
        private string[] TrueSplit(string line, char separator = ':')
        {
            if (line == null)
                return new string[] { "", "" };
            int pos = line.IndexOf(separator);
            if (pos >= 0)
                return new string[] { line.Remove(pos), line.Substring(pos + 1) };
            else
                return new string[] { line, line };
        }
        private string GET(string Data)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://predictor.yandex.net/api/v1/predict.json/complete?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
    }
}