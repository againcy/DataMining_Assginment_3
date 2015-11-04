using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMining_Assginment_3
{
    class Dataset
    {
        /// <summary>
        /// 数据
        /// </summary>
        public LinkedList<DataType> Data
        {
            get
            {
                return data;
            }
        }
        private LinkedList<DataType> data;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Dataset()
        {
            data = new LinkedList<DataType>();
        }

        public int CntFeatures;

        /// <summary>
        /// 添加一组数据
        /// </summary>
        /// <param name="newData"></param>
        public void AddData(DataType newData)
        {
            data.AddLast(newData);
            CntFeatures = newData.cntFeatures;
        }

        /// <summary>
        /// 将类标归一化为0,1,2...
        /// </summary>
        public void NormalizeLabel()
        {
            List<int> label = new List<int>();
            foreach(var d in data)
            {
                if (label.Contains(d.label_grountTruth) == false) label.Add(d.label_grountTruth);
            }
            int[] newLabel = label.ToArray();
            foreach(var d in data)
            {
                for (int i = 0; i < newLabel.Count(); i++)
                {
                    if (d.label_grountTruth == newLabel[i]) d.label_grountTruth = i;
                }
            }
        }
        
    }
}
