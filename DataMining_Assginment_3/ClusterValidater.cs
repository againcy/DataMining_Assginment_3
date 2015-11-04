using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataMining_Assginment_3
{
    class ClusterValidater
    {
        

        private static DataType[] dataset;
        private static int nClusters;
        
        /// <summary>
        /// 获取聚类结果的评价指标
        /// </summary>
        /// <param name="data">聚类结果</param>
        /// <param name="K">类的个数</param>
        /// <returns>结果的purity和gini</returns>
        public static ValidationPair GetValidation(Dataset data, int K)
        {
            dataset = data.Data.ToArray();
            nClusters = K;
            ValidationPair result = new ValidationPair();
            result.gini = GetGini();
            result.purity = GetPurity();
            return result;
        }

        /// <summary>
        /// 获取Purity
        /// </summary>
        /// <returns></returns>
        private static double GetPurity()
        {
            int[,] map = new int[nClusters,nClusters];//map[i,j]表示原类标i的数据被分类到了类标j上
            map.Initialize();
            foreach (var data in dataset)
            {
                for (int i = 0; i < nClusters; i++)
                {
                    if (data.label_grountTruth == i) map[i, data.label]++;
                }
            }
            int[] P = new int[nClusters];//P[j]表示算法生成的类j中多数是原类标i的数据
            for (int j = 0; j < nClusters; j++)
            {
                int max = 0;
                for(int i =0;i< nClusters;i++)
                {
                    if (map[i, j] > max) max = map[i, j];
                }
                P[j] = max;
            }
            return (double)P.Sum() / (double)dataset.Count();
        }

        /// <summary>
        /// 获取Gini
        /// </summary>
        /// <returns></returns>
        private static double GetGini()
        {
            int[,] map = new int[nClusters, nClusters];//map[i,j]表示原类标i的数据被分类到了类标j上
            map.Initialize();
            foreach (var data in dataset)
            {
                for (int i = 0; i < nClusters; i++)
                {
                    if (data.label_grountTruth == i) map[i, data.label]++;
                }
            }
            int[] M = new int[nClusters];
            M.Initialize();
            for (int j = 0; j < nClusters; j++)
            {
                for (int i = 0; i < nClusters; i++) M[j] += map[i, j];
            }
            double[] Gini = new double[nClusters];
            for(int j=0;j<nClusters;j++)
            {
                Gini[j] = 1;
                for(int i=0;i<nClusters;i++)
                {
                    Gini[j] -= Math.Pow((double)map[i, j] / (double)M[j], 2);
                }
            }
            double GiniAvr = 0;
            for (int j = 0; j < nClusters; j++) GiniAvr += Gini[j] * M[j];
            GiniAvr /= M.Sum();
            return GiniAvr;
        }

        public static void OutputResult(Dataset dataset)
        {
            StreamWriter sw = new StreamWriter("output.txt");
            sw.WriteLine("生成类,原始类");
            foreach(var data in dataset.Data)
            {
                sw.WriteLine(data.label.ToString() + "," + data.label_grountTruth.ToString());
            }
            sw.Close();
        }
    }

    class ValidationPair
    {
        public double purity;
        public double gini;
        public ValidationPair()
        {
            purity = 0;
            gini = 1;
        }
    }
}
