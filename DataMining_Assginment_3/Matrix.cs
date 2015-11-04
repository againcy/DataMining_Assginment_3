using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLApp;

namespace DataMining_Assginment_3
{
    class Matrix
    {
        #region 矩阵元素
        public int nRows
        {
            get
            {
                return numRows;
            }
        }
        private int numRows;//行号
        public int nColumns
        {
            get
            {
                return numColumns;
            }
        }
        private int numColumns;//列号
    
        public double[,] Cell
        {
            set
            {
                cell = value;
            }
            get
            {
                return cell;
            }
        }
        private double[,] cell;//元素

        /// <summary>
        /// 是否为对角阵 0:否 1:是 -1未判断
        /// </summary>
        public int diagonal;
        #endregion

        /// <summary>
        /// 定义一个n*m的矩阵
        /// </summary>
        /// <param name="n">行数</param>
        /// <param name="m">列数</param>
        public Matrix(int n,int m)
        {
            Initialize(n, m);
        }

        public void Initialize(int nRow,int nColumn)
        {
            numRows = nRow;
            numColumns = nColumn;
            cell = new double[numRows, numColumns];
            //elements = new double[numRows * numColumns];
            cell.Initialize();
            diagonal = -1;
        }

        /// <summary>
        /// 重载运算符 *
        /// </summary>
        /// <param name="m1">矩阵m1</param>
        /// <param name="m2">矩阵m2</param>
        /// <returns>m1*m2</returns>
        public static Matrix operator * (Matrix m1, Matrix m2)
        {
            return m2.MultiplyByMatrix(m1);
        }
        /// <summary>
        /// 左乘一个矩阵m
        /// </summary>
        /// <param name="m"></param>
        /// <returns>结果矩阵</returns>
        public Matrix MultiplyByMatrix(Matrix m)
        {
            if (m.nColumns != this.nRows) return null;
            Matrix result = new Matrix(m.nRows, this.nColumns);
            if (m.isDiagonal() || this.isDiagonal() == true)
            {
                //乘对角阵
                if (m.isDiagonal() == true)
                {
                    for (int i = 0; i < result.nRows; i++)
                    {
                        for (int j = 0; j < result.nColumns; j++)
                        {
                            result.Cell[i, j] = m.Cell[i, i] * this.Cell[i, j];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < result.nRows; i++)
                    {
                        for (int j = 0; j < result.nColumns; j++)
                        {
                            result.Cell[i, j] = m.Cell[i, j] * this.Cell[j, j];
                        }
                    }
                }

            }
            else
            {
                //一般矩阵乘法
                Parallel.For(0, result.nRows, i =>
                {
                    for (int j = 0; j < result.nColumns; j++)
                    {
                        for (int k = 0; k < this.nRows; k++)
                        {
                            result.Cell[i, j] += m.Cell[i, k] * this.Cell[k, j];
                        }
                    }
                });
                
                //result.SyncElements();
            }
            return result;
        }

        /// <summary>
        /// 重载运算符 +
        /// </summary>
        /// <param name="m1">矩阵m1</param>
        /// <param name="m2">矩阵m2</param>
        /// <returns>m1+m2</returns>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            return m1.AddMatrix(m2);
        }
        /// <summary>
        /// 矩阵加法
        /// </summary>
        /// <param name="m"></param>
        /// <returns>结果矩阵</returns>
        public Matrix AddMatrix(Matrix m)
        {
            if (m.nColumns != this.nColumns || m.nRows != this.nRows) return null;
            Matrix result = new Matrix(m.nRows, m.nColumns);
            for (int i = 0; i < result.nRows; i++)
            {
                for (int j = 0; j < result.nColumns; j++)
                {
                    result.Cell[i, j] = this.Cell[i, j] + m.Cell[i, j];
                }
            }
            //result.SyncElements();
            return result;
        }

        /// <summary>
        /// 重载运算符 -
        /// </summary>
        /// <param name="m1">矩阵m1</param>
        /// <param name="m2">矩阵m2</param>
        /// <returns>m1+m2</returns>
        public static Matrix operator - (Matrix m1, Matrix m2)
        {
            return m1.MinusMatrix(m2);
        }
        /// <summary>
        /// 矩阵减法，this-m
        /// </summary>
        /// <param name="m"></param>
        /// <returns>结果矩阵this-m</returns>
        public Matrix MinusMatrix(Matrix m)
        {
            if (m.nColumns != this.nColumns || m.nRows != this.nRows) return null;
            Matrix result = new Matrix(m.nRows, m.nColumns);
            for (int i = 0; i < result.nRows; i++)
            {
                for (int j = 0; j < result.nColumns; j++)
                {
                    result.Cell[i, j] = this.Cell[i, j] - m.Cell[i, j];
                }
            }
           // result.SyncElements();
            return result;
        }

        /// <summary>
        /// 给一行赋值
        /// </summary>
        /// <param name="rowID">行号</param>
        /// <param name="v"></param>
        public void SetRow(int rowID, double[] v)
        {
            if (v.Count() != this.nColumns) return;
            for (int j = 0; j < this.nColumns; j++) this.Cell[rowID, j] = v[j];
        }

        /// <summary>
        /// 获取一行的值
        /// </summary>
        /// <param name="rowID">行号</param>
        /// <returns></returns>
        public double[] GetRow(int rowID)
        {
            if (rowID >= this.nRows) return null;
            double[] result = new double[this.nColumns];
            for (int j = 0; j < this.nColumns; j++) result[j] = Cell[rowID, j];
            return result;
        }

        /// <summary>
        /// 给一列赋值
        /// </summary>
        /// <param name="columnID">列号</param>
        /// <param name="v"></param>
        public void SetColumn(int columnID, double[]v)
        {
            if (v.Count() != this.nRows) return;
            for (int i = 0; i < this.nRows; i++) this.Cell[i, columnID] = v[i];
        }

        /// <summary>
        /// 获取一列的值
        /// </summary>
        /// <param name="columnID">列号</param>
        /// <returns></returns>
        public double[] GetColumn(int columnID)
        {
            if (columnID >= this.nColumns) return null;
            double[] result = new double[this.nRows];
            for (int i = 0; i < this.nRows; i++) result[i] = Cell[i, columnID];
            return result;
        }

        /// <summary>
        /// 获取对角线上的值
        /// </summary>
        /// <returns>null则非N阶方阵</returns>
        public double[] GetDiagonal()
        {
            if (nRows != nColumns) return null;
            double[] result = new double[nRows];
            for (int i = 0; i < nRows; i++) result[i] = cell[i, i];
            return result;
        }

        /// <summary>
        /// 获取矩阵的转置
        /// </summary>
        /// <returns></returns>
        public Matrix GetTranspose()
        {
            Matrix result = new Matrix(numColumns, numRows);
            for (int i = 0; i < this.numRows; i++) result.SetColumn(i, this.GetRow(i));
            return result;
        }

        /// <summary>
        /// 判断是否为对角阵
        /// </summary>
        /// <returns></returns>
        public bool isDiagonal()
        {
            if (diagonal == -1)
            {
                diagonal = 0;
                if (nColumns != nRows) return false;
                for(int i=0;i<nRows;i++)
                {
                    for (int j = 0; j < nColumns; j++) if (i != j && cell[i, j] != 0) return false;
                }
                diagonal = 1;
                return true;
            }
            else return (diagonal==1);
        }

        /// <summary>
        /// 输出矩阵
        /// </summary>
        /// <param name="format">每个元素的输出格式</param>
        public void Output(string format)
        {
            for (int i = 0; i < this.nRows; i++)
            {
                for (int j = 0; j < this.nColumns; j++) Console.Write(cell[i, j].ToString(format) + " ");
                Console.WriteLine();
            }
        }

        /// <summary>
        /// 使用matlab库运算矩阵乘法
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public Matrix MultiplyByMatrix_MATLAB(Matrix m)
        {
            MLAppClass matlab = new MLAppClass();
            double[,] pia = new double[m.nRows, m.nColumns];
            double[,] pib = new double[this.nRows, this.nColumns];
            matlab.PutFullMatrix("A", "base", m.Cell, pia);
            matlab.PutFullMatrix("B", "base", this.Cell, pib);
            matlab.Execute("C = A*B;");
            Matrix result =new Matrix(m.nRows, this.nColumns);
            System.Array pir = new double[m.nRows,this.nColumns];
            System.Array prr = new double[m.nRows, this.nColumns];
            matlab.GetFullMatrix("C", "base",ref prr,ref pir);
            for(int i =0;i<prr.GetLength(0);i++)
            {
                for (int j = 0; j < prr.GetLength(1); j++) result.Cell[i, j] = (double)(prr.GetValue(i, j));
            }
            return result;
        }

        /// <summary>
        /// 求实对称矩阵特征值与特征向量的雅可比法(来自http://download.csdn.net/detail/jian_5030624/1082723)
        /// </summary>
        /// <param name="dblEigenValue">一维数组，长度为矩阵的阶数，返回时存放特征值</param>
        /// <param name="mtxEigenVector">返回时存放特征向量矩阵，其中第i列为与数组dblEigenValue中第i个特征值对应的特征向量</param>
        /// <param name="nMaxIt">迭代次数</param>
        /// <param name="eps">计算精度</param>
        /// <returns>bool型，求解是否成功</returns>
        public bool ComputeEvJacobi(double[] dblEigenValue, Matrix mtxEigenVector, int nMaxIt, double eps)
        {
            int i, j, p = 0, q = 0, l;
            //int u, w, t, s;
            double fm, cn, sn, omega, x, y, d;

            //SyncElements();
            mtxEigenVector.Initialize(this.numColumns, this.numColumns);
            l = 1;
            for (i = 0; i <= numColumns - 1; i++)
            {
                
                mtxEigenVector.Cell[i, i] = 1.0;
                for (j = 0; j <= numColumns - 1; j++)
                    if (i != j)
                        mtxEigenVector.Cell[i, j] = 0.0;
                        
            }

            while (true)
            {
                fm = 0.0;
                for (i = 1; i <= numColumns - 1; i++)
                {
                    for (j = 0; j <= i - 1; j++)
                    {
                        
                        d = Math.Abs(cell[i, j]);
                        if ((i != j) && (d > fm))
                        {
                            fm = d;
                            p = i;
                            q = j;
                        }
                    }
                }

                if (fm < eps || l>nMaxIt)
                {
                    //this.SyncCell();
                    for (i = 0; i < numColumns; ++i)
                        dblEigenValue[i] = cell[i, i];
                    //mtxEigenVector.SyncCell();
                     
                    return true;
                }

                /*if (l > nMaxIt)
                    return false;*/

                l = l + 1;
                //u = p * numColumns + q;
               // w = p * numColumns + p;
                //t = q * numColumns + p;
                //s = q * numColumns + q;
                x = -cell[p,q];
                y = (cell[q,q] - cell[p,p]) / 2.0;
                omega = x / Math.Sqrt(x * x + y * y);

                if (y < 0.0)
                    omega = -omega;

                sn = 1.0 + Math.Sqrt(1.0 - omega * omega);
                sn = omega / Math.Sqrt(2.0 * sn);
                cn = Math.Sqrt(1.0 - sn * sn);
                fm = cell[p, p];
                cell[p, p] = fm * cn * cn + cell[q, q] * sn * sn + cell[p, q] * omega;
                cell[q, q] = fm * sn * sn + cell[q, q] * cn * cn - cell[p, q] * omega;
                cell[p, q] = 0.0;
                cell[q, p] = 0.0;
                for (j = 0; j <= numColumns - 1; j++)
                {
                    if ((j != p) && (j != q))
                    {
                       // u = p * numColumns + j; w = q * numColumns + j;
                        fm = cell[p,j];
                        cell[p, j] = fm * cn + cell[q, j] * sn;
                        cell[q, j] = -fm * sn + cell[q, j] * cn;
                    }
                }

                for (i = 0; i <= numColumns - 1; i++)
                {
                    if ((i != p) && (i != q))
                    {
                        //u = i * numColumns + p;
                       // w = i * numColumns + q;
                        fm = cell[i, p];
                        cell[i, p] = fm * cn + cell[i, q] * sn;
                        cell[i, q] = -fm * sn + cell[i, q] * cn;
                    }
                }

                for (i = 0; i <= numColumns - 1; i++)
                {
                   // u = i * numColumns + p;
                    //w = i * numColumns + q;
                    fm = mtxEigenVector.cell[i, p];
                    mtxEigenVector.cell[i, p] = fm * cn + mtxEigenVector.cell[i, q] * sn;
                    mtxEigenVector.cell[i, q] = -fm * sn + mtxEigenVector.cell[i, q] * cn;
                }
            }
        }

        /// <summary>
        /// 求矩阵的-1/2次幂，矩阵必须为非负对角阵D，求D^(-1/2)
        /// </summary>
        /// <returns>D^(-1/2)</returns>
        public Matrix SqrtInverse()
        {
            if (nRows != nColumns) return null;
            Matrix result = new Matrix(nRows, nRows);
            try
            {
                for (int i = 0; i < nRows; i++) result.Cell[i, i] = 1.0 / Math.Sqrt(this.Cell[i, i]);
            }
            catch
            { }
            result.diagonal = 1;
            return result;
        }

        /// <summary>
        /// 向量x和y的欧式距离，若两向量维度不同则返回-1
        /// </summary>
        /// <param name="x">向量x</param>
        /// <param name="y">向量y</param>
        /// <returns>x和y的欧氏距离的平方或-1</returns>
        public static double Dist(double[] x, double[] y)
        {
            if (x.Count() != y.Count()) return -1;
            else
            {
                double sum = 0;
                for (int i = 0; i < x.Count(); i++)
                {
                    sum += Math.Pow((x[i] - y[i]), 2);
                }
                return sum;
            }
        }
    }
}
