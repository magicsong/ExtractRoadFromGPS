using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Windows.Forms;

namespace PBSLocationProblemSolver
{
    class Program
    {
        class SimpleCombination
        {
            private int count;
            private string wordClass;

            public int Count { get => count; set => count = value; }
            public string WordClass { get => wordClass; set => wordClass = value; }

            public SimpleCombination(string wordClass, int count)
            {
                WordClass = wordClass;
                Count = count;
            }
        }
        static Dictionary<string, SimpleCombination> myDictionary = new Dictionary<string, SimpleCombination>();
        [STAThread]
        static void Main()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt;*.csv";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName/*,Encoding.GetEncoding("GB2312")*/))
                {
                    Console.WriteLine("正在密集的处理任务");
                    while (!sr.EndOfStream)
                    {
                        string sentence = sr.ReadLine();
                        Task t = new Task((word) =>
                        {
                            DownloadPageAsync(word.ToString());
                        }, sentence);
                        t.Start();
                    }
                }
                //Console.WriteLine("数据处理结束");
                //Console.WriteLine("开始保存数据");
                //StreamWriter sw = new StreamWriter("wordAnalysis.txt");
                //foreach (var item in myDictionary)
                //{
                //    sw.WriteLine(string.Format("{0}\t{1}\t{2}", item.Key, item.Value.Count, item.Value.WordClass));
                //}
                //Console.WriteLine("Done");
            }
            Console.ReadLine();
        }
        static string APIKEY = "r1x1H6U6S8BmqfbwvsezaOQpsaOxiDRe3EkGSQoS";
        static async void DownloadPageAsync(string sentence)
        {
            // ... Target page.http://api.ltp-cloud.com/analysis/?api_key=r1x1H6U6S8BmqfbwvsezaOQpsaOxiDRe3EkGSQoS&text=我是中国人。&pattern=sdp&format=plain
            string url = string.Format("http://api.ltp-cloud.com/analysis/?api_key={0}&text={1}&pattern=pos&format=plain", APIKEY, "我是中国人。");
            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())         
            {
                var result = await client.GetStringAsync(url);
                // ... Display the result.
                if (result!= null)
                {
                    Console.WriteLine(result);
                    //Analysis
                    string[] patterns = result.Split();
                    foreach (var item in patterns)
                    {
                        int index = item.LastIndexOf('_');
                        string word = item.Substring(0, index);
                        string wordclass = item.Substring(index + 1);
                        if (myDictionary.ContainsKey(word))
                        {
                            myDictionary[word].Count += 1;
                        }
                        else
                        {
                            myDictionary.Add(word, new SimpleCombination(wordclass, 1));
                        }
                    }
                }
            }
        }
    }
}
