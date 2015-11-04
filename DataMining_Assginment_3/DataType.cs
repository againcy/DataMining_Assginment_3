namespace DataMining_Assginment_3
{
    class DataType
    {
        /// <summary>
        /// 特征
        /// </summary>
        public double[] features;
        /// <summary>
        /// 先验类标
        /// </summary>
        public int label_grountTruth;
        /// <summary>
        /// 根据聚类算法得出的类标
        /// </summary>
        public int label;

        /// <summary>
        /// 特征的数量
        /// </summary>
        public int cntFeatures;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="n">特征数量</param>
        public DataType(int n)
        {
            features = new double[n];
            for (int i = 0; i < n; i++) features[i] = 0;
            cntFeatures = n;
        }

        /// <summary>
        /// 将特征的向量相加
        /// </summary>
        /// <param name="d"></param>
        public void Add(DataType d)
        {
            if (d.cntFeatures != cntFeatures) return;
            for(int i =0;i<cntFeatures;i++)
            {
                features[i] += d.features[i];
            }
        }
    }
}
