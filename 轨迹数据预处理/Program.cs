using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace 轨迹数据预处理
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt";
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("var gpsdata=[");
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        int split = line.IndexOf('\t');
                        int num = int.Parse(line.Substring(0, split));
                        sb.Append("{'geo':[");
                        string[] data = line.Substring(split + 1).Split(new char[] { ',', '|' });
                        for (int i = 0; i < data.Length; i += 3)
                        {
                            //添加时间信息
                            sb.Append("[" + data[i + 1] + "," + data[i + 2] +","+data[i].Substring(0,7)+"]");
                            if (i != data.Length - 3)
                            {
                                sb.Append(',');
                            }
                        }
                        sb.Append("],'count':1},");
                    }
                    sb.Remove(sb.Length - 1, 1);//移除最后一个逗号
                    sb.Append("];");
                    StreamWriter sw = new StreamWriter("gpstrajectories.js");
                    sw.Write(sb.ToString());
                    sw.Close();
                }
            }
        }
    }
}
