using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataMining_Assginment_3
{
    public delegate void PrintLogFunction(string str);
    
    class Program
    {
        static string fileName;
        static Dataset dataset;
        static void Input()
        {
            Console.WriteLine("输入读入文件的文件名：");
            fileName = Console.ReadLine();
            dataset = new Dataset();
            StreamReader sr = new StreamReader(fileName);
            string line;
            while ((line=sr.ReadLine())!=null)
            {
                var tmp = line.Split(',');
                DataType newdata = new DataType(tmp.Length - 1);
                newdata.features = tmp.Take(tmp.Length - 1).Select<string, double>(x => Convert.ToDouble(x)).ToArray();
                newdata.cntFeatures = tmp.Length - 1;
                newdata.label_grountTruth = Convert.ToInt32(tmp[tmp.Length - 1]);
                dataset.AddData(newdata);
            }
            sr.Close();
        }

        static void TestKmeans(int cnt, double conv)
        {
            StreamWriter sw = new StreamWriter(@".\"+fileName+@"\result_kmeans.txt");
            ValidationPair result;
            PrintLog("开始执行kmeans...");
            KmeansGenerator kmeans = new KmeansGenerator(dataset, conv);
            result = kmeans.Generate(cnt, 10, PrintLog);
            sw.WriteLine("purity=" + result.purity.ToString());
            sw.WriteLine("gini=" + result.gini.ToString());
            PrintLog("kmeans结束...");
            sw.Close();
        }
        static void TestNMF(int cnt, double conv)
        {
            StreamWriter sw = new StreamWriter(@".\" + fileName + @"\result_NMF.txt");
            ValidationPair result;
            PrintLog("开始执行NMF...");
            NMFGenerator NMF = new NMFGenerator(dataset, conv);
            result = NMF.Generate(cnt, 10, PrintLog);
            sw.WriteLine("purity=" + result.purity.ToString());
            sw.WriteLine("gini=" + result.gini.ToString());
            PrintLog("NMF结束...");
            sw.Close();
        }
        static void TestSpectral(int cnt,double conv)
        {
            SpectralClusteringGenerator spectral = new SpectralClusteringGenerator(dataset, conv, PrintLog, @".\" + fileName +@"\neighbours.txt");
            StreamWriter sw = new StreamWriter(@".\" + fileName + @"\result_Spectral.txt");
            for (int N = 3; N <= 9; N = N + 3)
            {
                ValidationPair result;
                PrintLog("开始执行Spectral, n="+N.ToString()+" ...");
                result = spectral.Generate(cnt, N, PrintLog);
                sw.WriteLine("N=" + N.ToString());
                sw.WriteLine("  purity=" + result.purity.ToString());
                sw.WriteLine("  gini=" + result.gini.ToString());
            }
            sw.Close();
            PrintLog("Spectral结束...");
        }
        
        /// <summary>
        /// 打印日志的函数
        /// </summary>
        /// <param name="str">日志内容</param>
        static void PrintLog(string str)
        {
            StreamWriter sw = new StreamWriter(@".\" + fileName + @"\result_log.txt",true);
            Console.WriteLine(System.DateTime.Now.ToLongTimeString() + "  " + str);
            sw.WriteLine(System.DateTime.Now.ToLongTimeString() +"  "+ str);
            sw.Close();
        }

        static void TestCluster(double conv)
        {
            Console.WriteLine("输入目标类数量：");
            int cnt = 1;
            int.TryParse(Console.ReadLine(), out cnt);
            fileName = fileName.Substring(0, fileName.IndexOf(".txt"));
            if (File.Exists(@".\" + fileName + @"\result_log.txt") == true) File.Delete(@".\" + fileName + @"\result_log.txt");
            TestKmeans(cnt, conv);
            TestNMF(cnt, conv);
            TestSpectral(cnt, conv);
        }


        static void Main(string[] args)
        {
            Input();
            TestCluster(0.001);
            Console.WriteLine("输入回车结束...");
            Console.ReadLine();
        }
    }
}
