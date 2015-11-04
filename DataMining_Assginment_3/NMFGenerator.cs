using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining_Assginment_3
{
    class NMFGenerator
    {
       // public delegate void PrintLogFunction(string str);
        public Dataset Data
        {
            get
            {
                return dataset;
            }
        }
        private Dataset dataset;

        private double convergence;
        public DataType[] ArrData
        {
            get
            {
                return arrData;
            }
        }
        private DataType[] arrData;
        private Matrix X, U, V;//X=UV^T

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">数据集</param>
        /// <param name="conv">收敛条件</param>
        public NMFGenerator(Dataset data, double conv)
        {
            dataset = data;
            dataset.NormalizeLabel();
            convergence = conv;
            X = new Matrix(dataset.CntFeatures, dataset.Data.Count());

            arrData = dataset.Data.ToArray();
            for(int column=0;column<dataset.Data.Count();column++)
            {
                X.SetColumn(column, arrData[column].features);
            }
        }

        public void InitializeUV(int K)
        {
            Random rand = new Random();
            for(int i=0;i<U.nRows;i++)
            {
                for(int j=0;j<U.nColumns; j++)
                {
                    U.Cell[i, j] = (double)(rand.Next() % 100) / 100.0;
                }
                    
            }
            for (int i = 0; i < V.nRows; i++)
            {
                for (int j = 0; j < V.nColumns; j++)
                {
                    V.Cell[i, j] = (double)(rand.Next() % 100)/100.0;
                }

            }
        }

        /// <summary>
        /// 基于NMF聚类，生成K个类，重复执行repeat次选结果最好的
        /// </summary>
        /// <param name="K">聚类个数</param>
        /// <param name="repeat">重复次数</param>
        /// <returns>聚类结果的purity和gini指标</returns>
        public ValidationPair Generate(int K, int repeat, PrintLogFunction PrintLog)
        {
            ValidationPair result = new ValidationPair();
            for (int r = 0; r < repeat; r++)
            {
                PrintLog("正在执行第" + r.ToString() + "次NMF聚类...");
                #region 执行一次聚类

                U = new Matrix(dataset.CntFeatures, K);
                V = new Matrix(dataset.Data.Count(), K);
                InitializeUV(K);
                Matrix newU = new Matrix(dataset.CntFeatures, K);
                Matrix newV = new Matrix(dataset.Data.Count(), K);
                bool isOver = false;
                int cntIterate = 0;
                while (isOver == false && cntIterate<1000)
                {
                    //迭代直到V不再变化
                    Matrix Ua = X * V;
                    Matrix Ub = U * V.GetTranspose() * V;
                    Matrix Va = X.GetTranspose() * U;
                    Matrix Vb = V * U.GetTranspose() * U;
                    for (int i = 0; i < newU.nRows; i++)
                    {
                        for (int j = 0; j < newU.nColumns; j++)
                        {
                            newU.Cell[i, j] = Ua.Cell[i, j] / Ub.Cell[i, j];
                        }
                    }
                    isOver = true;
                    for (int i = 0; i < newV.nRows; i++)
                    {
                        for (int j = 0; j < newV.nColumns; j++)
                        {
                            newV.Cell[i, j] = Va.Cell[i, j] / Vb.Cell[i, j];
                            if (newV.Cell[i, j] != V.Cell[i, j]) isOver = false;
                        }
                    }
                    V = newV;
                    U = newU;
                    cntIterate++;
                }
                //将类标写入data
                for (int i = 0; i < V.nRows; i++)
                {
                    var row = V.GetRow(i);
                    double max = 0;
                    int best = 0;
                    for (int j = 0; j < K; j++)
                    {
                        if (row[j] > max)
                        {
                            max = row[j];
                            best = j;
                        }
                    }
                    arrData[i].label = best;
                }
                #endregion

                #region 生成评价指标

                ValidationPair v = ClusterValidater.GetValidation(dataset, K);
                if (v.purity >= result.purity)
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
