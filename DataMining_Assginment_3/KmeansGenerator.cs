using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining_Assginment_3
{
    class KmeansGenerator 
    {
        //public delegate void PrintLogFunction(string str);
        public Dataset Data
        {
            get
            {
                return dataset;
            }
        }
        private Dataset dataset;

        private DataType[] presentative;//聚类的代表
        private double convergence;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据集</param>
        /// <param name="conv">收敛条件</param>
        public KmeansGenerator(Dataset data, double conv)
        {
            dataset = data;
            dataset.NormalizeLabel();
            convergence = conv;
        }

        /// <summary>
        /// 初始化代表集合
        /// </summary>
        /// <param name="K"></param>
        private void InitializePresentative(int K)
        {
            Random rand = new Random();
            DataType[] dataArray = dataset.Data.ToArray();
            List<int> check = new List<int>();
            for (int i = 0; i < K; i++)
            {
                int tmp = rand.Next() % dataset.Data.Count();
                while (check.Contains(tmp) == true)
                {
                    tmp = rand.Next() % dataset.Data.Count();
                }
                check.Add(tmp);
                presentative[i] = dataArray[tmp];
                presentative[i].label = i;
            }
        }

        /// <summary>
        /// 执行kmeans聚类方法，生成K个类，重复执行repeat次选结果最好的
        /// </summary>
        /// <param name="K">聚类个数</param>
        /// <param name="repeat">重复次数</param>
        /// <returns>聚类结果的purity和gini指标</returns>
        public ValidationPair Generate(int K, int repeat, PrintLogFunction PrintLog)
        {
            ValidationPair result = new ValidationPair();
            for (int r = 0; r < repeat; r++)
            {
                PrintLog("正在执行第" + r.ToString() + "次kmeans聚类...");
                #region 执行一次聚类

                presentative = new DataType[K];
                InitializePresentative(K);
                double sumDist = 100;
                bool isOver = false;
                while (sumDist > convergence)
                {
                    //对当前的代表进行聚类
                    isOver = true;
                    foreach (var data in dataset.Data)
                    {
                        double minDist = double.MaxValue;
                        int lableNow = 0;
                        for (int i = 0; i < K; i++)
                        {
                            double d = Matrix.Dist(data.features, presentative[i].features);
                            if (d < minDist)
                            {
                                minDist = d;
                                lableNow = i;
                            }
                        }
                        if (lableNow != data.label) isOver = false;
                        data.label = lableNow;
                    }
                    if (isOver == true) break;//若当前迭代未改变类，则结束
                                              //生成新的代表
                    DataType[] newPresentative = new DataType[K];
                    for (int i = 0; i < K; i++)
                    {
                        int clusterCount = 0;
                        newPresentative[i] = new DataType(presentative[0].cntFeatures);
                        //新的代表取聚类中所有向量的平均值
                        foreach (var data in dataset.Data)
                        {
                            if (data.label != i) continue;
                            clusterCount++;
                            newPresentative[i].Add(data);
                        }
                        for (int j = 0; j < newPresentative[i].cntFeatures; j++) newPresentative[i].features[j] /= clusterCount;
                    }
                    //计算两次代表之间的平均距离
                    sumDist = 0;
                    for (int i = 0; i < K; i++)
                    {
                        sumDist += Matrix.Dist(presentative[i].features, newPresentative[i].features);
                    }
                    presentative = newPresentative;
                    sumDist /= K;
                }
                #endregion

                #region 生成评价指标

                ValidationPair v = ClusterValidater.GetValidation(dataset, K);
                if (v.purity>=result.purity)
                {
                    if (v.purity > result.purity) result = v;
                    else if (v.gini < result.gini) result = v;
                }
                #endregion
            }
            return result;
        }
    }
}
