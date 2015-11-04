using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataMining_Assginment_3
{
    
    class SpectralClusteringGenerator
    {
        
        public Dataset Data
        {
            get
            {
                return dataset;
            }
        }
        private Dataset dataset;
        //private KD_Tree kdTree;//用于查询一个点的最近邻n个点

        private double convergence;
        public DataType[] ArrData
        {
            get
            {
                return arrData;
            }
        }
        private DataType[] arrData;
        private string addrNeighbours;

        private Dictionary<int, int[]> nearestNeighbours;

        //private Matrix W;//边权矩阵
        //private Matrix D;//边权和矩阵（对角矩阵）
        //private Matrix L;//Laplacian矩阵

        
        private MLApp.MLAppClass matlab;//用于和matlab交互用

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据集</param>
        /// <param name="conv">kmeans时的收敛值</param>
        /// <param name="addr">邻接表的地址</param>
        public SpectralClusteringGenerator(Dataset data, double conv, PrintLogFunction PrintLog, string addr)
        {
            dataset = data;
            arrData = data.Data.ToArray();
            convergence = conv;
            PrintLog("初始化MATLAB组件...");
            matlab = new MLApp.MLAppClass();
            matlab.Visible = 0;
            PrintLog("初始化结束...");
            if (addr == null /*|| File.Exists(addr)==false*/)
            {
                PrintLog("初始化Spectral邻居点图, 正在寻找每个点相邻最近的数个点...");
                FindNeighbours(9, PrintLog);
                PrintLog("初始化结束...");
            }
            else
            {
                addrNeighbours = addr;
            }
        }

        /// <summary>
        /// 预处理，计算每个点最近的n个邻居，用于算法中W矩阵的计算
        /// <param name="neighbourCnt">每个点记录最近的neighbourCnt个点</param>
        /// </summary>
        private void FindNeighbours(int neighbourCnt, PrintLogFunction PrintLog)
        {
            int n = arrData.Count();
            nearestNeighbours = new Dictionary<int, int[]>();
            //寻找相邻点
            for (int i = 0; i < n; i++)
            {
                Dictionary<int, double> dist = new Dictionary<int, double>();
                for (int j = 0; j < n; j++)
                {
                    if (j == i) dist.Add(j, double.MaxValue);//一个点不能与自己相邻
                    else dist.Add(j, Matrix.Dist(arrData[i].features, arrData[j].features));
                }
                var items = from pair in dist
                            orderby pair.Value ascending
                            select pair;
                var arr = items.ToArray();
                nearestNeighbours.Add(i, new int[neighbourCnt]);
                for (int k = 0; k < neighbourCnt; k++)
                {
                    nearestNeighbours[i][k] = arr[k].Key;
                }
            }
            //将近邻矩阵打印出来保存
            StreamWriter sw = new StreamWriter(@"neighbours_tmp.txt");
            for(int i=0;i< n;i++)
            {
                for (int k = 0; k < neighbourCnt; k++) sw.Write(nearestNeighbours[i][k].ToString() + ",");
                sw.WriteLine();
            }
            sw.Close();
        }

        private void GetNeighbours(int neighbourCnt)
        {
            StreamReader sr = new StreamReader(addrNeighbours);
            string line;
            int id = 0;
            nearestNeighbours = new Dictionary<int, int[]>();
            while ((line=sr.ReadLine())!=null)
            {
                string[] neighbours = line.Split(',');
                nearestNeighbours.Add(id, new int[neighbourCnt]);
                for (int i = 0; i < neighbourCnt; i++)
                {
                    nearestNeighbours[id][i] = Convert.ToInt32(neighbours[i]);
                }
                id++;
            }
            sr.Close();
        }

        /// <summary>
        /// 执行谱聚类算法，生成K个类，每个点选最近的neighbourCnt个邻居
        /// </summary>
        /// <param name="K">类个数</param>
        /// <param name="neighbourCnt">邻居个数</param>
        /// <returns>聚类结果的purity和gini指标</returns>
        public ValidationPair Generate(int K, int neighbourCnt,PrintLogFunction PrintLog)
        {
            GetNeighbours(neighbourCnt);
            
            int n = arrData.Count();
            matlab.Execute("n = " + n.ToString() + ";");
            Array W = new double[n, n];
            //计算W矩阵
            for (int i=0;i< n;i++)
            {
                for(int j=0;j<neighbourCnt;j++)
                {
                    int a = i;
                    int b = nearestNeighbours[i][j];
                    W.SetValue(1, a, b);
                    W.SetValue(1, b, a);
                }
            }
            Array piW = new double[n, n]; 
            matlab.PutFullMatrix("W", "base", W, piW);
            
            //计算D矩阵
            matlab.Execute("s = sum(W);");//对W的每一行求和
            matlab.Execute("D = full(sparse(1:n, 1:n, s));");//将s赋值给D的对角线
            //计算E矩阵,E的K个最大特征值等于归一化L后L的K个最小特征值
            matlab.Execute("L = D - W;");
            matlab.Execute("E = D^(-1/2)*W*D^(-1/2);");
            matlab.Execute("k=" + K.ToString() + ";");
            PrintLog("正在计算特征值特征向量...");
            matlab.Execute("[Q, V] = eigs(E, k);");
            PrintLog("特征值特征向量计算完毕");
            //将特征向量填充给新的数据集
            Array pr = new double[n, K];
            Array pi = new double[n, K];
            matlab.GetFullMatrix("Q", "base", ref pr, ref pi);
            Dataset newData = new Dataset();
            for (int i = 0; i < n; i++)
            {
                DataType d = new DataType(K);
                for (int j = 0; j < K; j++) d.features[j] = (double)pr.GetValue(i, j);
                d.label_grountTruth = arrData[i].label_grountTruth;
                newData.AddData(d);
            }
            //对新的数据进行kmeans聚类
            KmeansGenerator kmeans = new KmeansGenerator(newData, convergence);
            return kmeans.Generate(K, 10, PrintLog);
        }

    }
}
