using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VKMsgParse
{
    public class Sightengine
    {
        string key;
        //&url=https://nowere.net/b/arch/94742/src/1374775499739.jpg
        //models=nudity,wad,offensive,faces,face-attributes
        //&api_user=1662965073&api_secret=dtndtQ3d79k7mQxEKu6M
        public Sightengine(string key)
        {
            this.key = key;
        }
        public Dictionary<string, string> Analisys(string text, Model models)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            var search = Cache.GetRes(text, typeof(Sightengine), new TimeSpan(24, 0, 0));
            if (search != null)
            {
                return search as Dictionary<string, string>;
            }
            else
            {
                string res = GET(CreateParams(models) + key + "&url=" + text);
                string com = res;
                try
                {
                    dynamic stuff = JsonConvert.DeserializeObject(com);
                    string status = stuff["status"];
                    results.Add("status", status);

                    //string request = stuff["request"];

                    string weapon = stuff["weapon"];
                    results.Add("weapon", weapon);

                    string alcohol = stuff["alcohol"];
                    results.Add("alcohol", alcohol);

                    string drugs = stuff["drugs"];
                    results.Add("drugs", drugs);

                    string scam = stuff["scam"]["prob"];
                    results.Add("scam", scam);

                    string nudity_raw = stuff["nudity"]["raw"];
                    results.Add("nudity_raw", nudity_raw);

                    string nudity_safe = stuff["nudity"]["safe"];
                    results.Add("nudity_safe", nudity_safe);

                    string nudity_partial = stuff["nudity"]["partial"];
                    results.Add("nudity_partial", nudity_partial);

                    //string faces = stuff["faces"];


                    string sharpness = stuff["sharpness"];
                    results.Add("sharpness", sharpness);

                    string brightness = stuff["brightness"];
                    results.Add("brightness", brightness);

                    string contrast = stuff["contrast"];
                    results.Add("contrast", contrast);

                    string color = stuff["colors"]["dominant"]["r"] + "," + stuff["colors"]["dominant"]["g"] + "," + stuff["colors"]["dominant"]["b"];
                    results.Add("color", color);

                    string[] colors = new string[stuff["colors"]["other"].Count];
                    for (int i = 0; i < stuff["colors"]["other"].Count; i++)
                    {
                        colors[i] = stuff["colors"]["other"][i]["r"] + "," + stuff["colors"]["other"][i]["g"] + "," + stuff["colors"]["other"][i]["b"];
                    }

                    string texthas_artificial = stuff["text"]["has_artificial"];
                    results.Add("texthas_artificial", texthas_artificial);

                    string texthas_natural = stuff["text"]["has_natural"];
                    results.Add("texthas_natural", texthas_natural);

                    string offensiveprob = stuff["offensive"]["prob"];
                    results.Add("offensiveprob", offensiveprob);
                    if (stuff["offensive"].ContainsKey("boxes"))
                    {
                        List<Dictionary<string, string>> offensiveboxes = new List<Dictionary<string, string>>();
                        for (int i = 0; i < stuff["offensive"]["boxes"].Count; i++)
                        {
                            var newdic = new Dictionary<string, string>();
                            offensiveboxes.Add(newdic);
                            newdic.Add("x1", stuff["offensive"]["boxes"][i]["x1"].ToString());
                            newdic.Add("y1", stuff["offensive"]["boxes"][i]["y1"].ToString());
                            newdic.Add("x2", stuff["offensive"]["boxes"][i]["x2"].ToString());
                            newdic.Add("y2", stuff["offensive"]["boxes"][i]["y2"].ToString());
                            newdic.Add("label", stuff["offensive"]["boxes"][i]["label"].ToString());
                            newdic.Add("prob", stuff["offensive"]["boxes"][i]["prob"].ToString());
                        }
                    }
                    string media = stuff["media"]["uri"];
                    results.Add("media", media);
                    return results;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        public string CreateParams(Model models)
        {
            string res = "models=";
            foreach (Model model in Enum.GetValues(typeof(Model)))
            {
                res += (models & model) == model ? model.ToString() + "," : "";
            }
            return res.Trim(',').Replace("_", "-");
        }
        public enum Model
        {
            nudity = 1,
            wad = 2,
            properties = 4,
            celebrities = 8,
            offensive = 16,
            faces = 32,
            scam = 64,
            face_attributes = 128,
            text = 256
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
            System.Net.WebRequest req = System.Net.WebRequest.Create("https://api.sightengine.com/1.0/check.json?" + Data);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.Stream stream = resp.GetResponseStream();
            System.IO.StreamReader sr = new System.IO.StreamReader(stream);
            string Out = sr.ReadToEnd();
            sr.Close();
            return Out;
        }
    }
}
