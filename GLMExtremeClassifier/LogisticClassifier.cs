using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNumerics.LinearAlgebra;
using System.IO;

namespace Mine.Engines.Classifiers
{
    public class LogisticClassifier<T> : GLMExtremeClassifier<T>
    {
        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public LogisticClassifier()
        {
            lipsz = 1;
            type = "Logistic";
        }

        /// <summary>
        /// A constructor with training data
        /// </summary>
        /// <param name="data">Training data</param>
        public LogisticClassifier(List<KeyValuePair<byte[], T>> data, List<KeyValuePair<byte[], T>> valData, List<T> classes, double lam = 10000, double ta = 10, byte norm = 0)
            : base(data, valData, classes, lam, ta, norm)
        {
            lipsz = 1;
            type = "Logistic";
        }

        /// <summary>
        /// A constructor from a file
        /// </summary>
        /// <remarks>
        /// Signature(MINEGLM\0), type, lipsz, lambda, tau, norm, iter, minLoss, minError, wrow, wcol, argminW, errorMinW
        /// </remarks>
        /// <param name="fileName">File with the argminW</param>
        public LogisticClassifier(string fileName)
        {
            FileStream fs;
            BinaryReader reader;
            byte[] signature;
            int wRow, wCol;

            try
            {
                using (fs = File.Open(fileName, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs);

                    // signature
                    signature = reader.ReadBytes(8);
                    if( signature[0] != 'M' || 
                        signature[1] != 'I' ||
                        signature[2] != 'N' ||
                        signature[3] != 'E' ||
                        signature[4] != 'G' ||
                        signature[5] != 'L' ||
                        signature[6] != 'M' ||
                        signature[7] != '\0')
                    {
                        reader.Close();
                        fs.Close();

                        throw new FileLoadException();
                    }

                    // type
                    type = reader.ReadString();

                    if (type != "Logistic")
                    {
                        reader.Close();
                        fs.Close();

                        throw new FileLoadException();
                    }

                    // lipsz
                    lipsz = reader.ReadDouble();

                    if (lipsz != 1)
                    {
                        reader.Close();
                        fs.Close();

                        throw new FileLoadException();
                    }

                    // lambda
                    lambda = reader.ReadDouble();

                    // tau
                    tau = reader.ReadDouble();

                    // norm
                    normType = reader.ReadByte();

                    // iter
                    iter = reader.ReadInt32();

                    // minLoss
                    minLoss = reader.ReadDouble();

                    // minError
                    minError = reader.ReadDouble();

                    // wRow
                    wRow = reader.ReadInt32();

                    // wCol
                    wCol = reader.ReadInt32();

                    // argminW
                    argminW = new Matrix(wRow, wCol);
                    for (int i = 0; i < argminW.RowCount; i++)
                    {
                        for (int j = 0; j < argminW.ColumnCount; j++)
                        {
                            argminW[i, j] = reader.ReadDouble();
                        }
                    }

                    // errorMinW
                    errorMinW = new Matrix(wRow, wCol);
                    for (int i = 0; i < errorMinW.RowCount; i++)
                    {
                        for (int j = 0; j < errorMinW.ColumnCount; j++)
                        {
                            errorMinW[i, j] = reader.ReadDouble();
                        }
                    }

                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            w = new Matrix(argminW.RowCount, argminW.ColumnCount);
        }
        #endregion
        
        #region override methods
        /// <summary>
        /// link function , it returns y hat from wt * x.
        /// </summary>
        public override Matrix G(Matrix x)
        {
            if (x.ColumnCount != 1 || w.ColumnCount != x.RowCount)
            {
                throw new ArgumentException();
            }

            Matrix yHat = new Matrix(w.RowCount, x.ColumnCount);
            Matrix wtx = w * x;
            double sigma = 0;
            double max = 0;

            for (int i = 0; i < yHat.RowCount; i++)
            {
                sigma += Math.Exp(wtx[i, 0]);
            }

            if(double.IsInfinity(sigma))
            {
                max = wtx.GetColumnArray(0).Max();
                for (int i = 0; i < yHat.RowCount; i++)
                {
                    if (max == wtx[i, 0])
                    {
                        yHat[i, 0] = 1;
                    }
                }
            }
            else
            {
                for (int i = 0; i < yHat.RowCount; i++)
                {
                    yHat[i, 0] = Math.Exp(wtx[i, 0]) / sigma;
                }
            }                      

            return yHat;
        }

        /// <summary>
        /// link function , it returns y hat from wmt * x.
        /// </summary>
        public override Matrix G(Matrix wm, Matrix x)
        {
            if (x.ColumnCount != 1 || wm.ColumnCount != x.RowCount)
            {
                throw new ArgumentException();
            }

            Matrix yHat = new Matrix(wm.RowCount, x.ColumnCount);
            Matrix wmtx = wm * x;
            double sigma = 0;

            for (int i = 0; i < yHat.RowCount; i++)
            {
                sigma += Math.Exp(wmtx[i, 0]);
            }

            for (int i = 0; i < yHat.RowCount; i++)
            {
                yHat[i, 0] = Math.Exp(wmtx[i, 0]) / sigma;
            }

            return yHat;
        }

        /// <summary>
        /// convex function , it returns convex value.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public override double Fi(Matrix x)
        {
            if (x.ColumnCount != 1 || w.ColumnCount != x.RowCount)
            {
                throw new ArgumentException();
            }

            Matrix wtx = w * x;
            double sigma = 0;

            for (int i = 0; i < wtx.RowCount; i++)
            {
                sigma += Math.Exp(wtx[i, 0]);
            }

            return Math.Log(sigma);
        }
        #endregion
    }
}
