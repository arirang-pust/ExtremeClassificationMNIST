using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNumerics.LinearAlgebra;
using System.IO;

namespace Mine.Engines.Classifiers
{
    public abstract class GLMExtremeClassifier<T>
    {
        #region fields
        protected List<KeyValuePair<byte[], T>> data;
        protected List<KeyValuePair<byte[], T>> valData;
        List<T> classes; 

        protected double lipsz;
        protected string type;        
        protected Matrix w;
        protected double tau = 10000;
        protected double lambda = 10000;
        protected byte normType = 0;
        Matrix sigmaHat;
        Matrix sigmaInv;
        
        protected int iter = 0;
        protected int maxIter = 100000;
        double loss = 0;

        protected double minLoss = double.MaxValue;
        protected double minError = double.MaxValue;
        protected Matrix argminW;
        protected Matrix errorMinW;

        List<double> trErrs = new List<double>();
        List<double> valErrs = new List<double>();
        List<double> trLosses = new List<double>();
        List<double> valLosses = new List<double>();

        List<List<double>> digitPer = new List<List<double>>();

        #endregion
        
        #region properties
        /// <summary>
        /// Gets and sets lipschitz constant
        /// </summary>
        public double Lipsz
        {
            get { return lipsz; }
            set { lipsz = value; }
        }

        /// <summary>
        /// Gets and sets type of the link function
        /// </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        
        /// <summary>
        /// Gets and sets training set.
        /// </summary>
        public List<KeyValuePair<byte[], T>> Data
        {
            get { return data; }
            set 
            { 
                data = value;
                CalSigmaHat();
            }
        }

        /// <summary>
        /// Gets and sets validation data.
        /// </summary>
        public List<KeyValuePair<byte[], T>> ValData
        {
            get { return valData; }
            set { valData = value; }
        }

        /// <summary>
        /// Gets and sets class labels.
        /// </summary>
        public List<T> Classes
        {
            get { return classes; }
            set { classes = value; }
        }

        /// <summary>
        /// Gets sigma hat.
        /// </summary>
        public Matrix SigmaHat
        {
            get { return sigmaHat; }
        }

        /// <summary>
        /// Gets W.
        /// </summary>
        public Matrix W
        {
            get { return w; }
        }

        /// <summary>
        /// Gets and sets trace norm regularization factor
        /// </summary>
        public double Tau
        {
            get { return tau; }
            set { tau = value; }
        }

        /// <summary>
        /// Gets and sets inversion matrix regularization factor
        /// </summary>
        public double Lambda
        {
            get { return lambda; }
            set { lambda = value; }
        }

        /// <summary>
        /// Gets and sets norm type. if 0, trace norm, if 1, L1 norm.
        /// </summary>
        public byte NormType
        {
            get { return normType; }
            set { normType = value; }
        }

        /// <summary>
        /// argminW = argmin(loss + regularization)
        /// </summary>
        public Matrix ArgminW
        {
            get { return argminW; }
        }

        /// <summary>
        /// Gets validation-error min matrix.
        /// </summary>
        public Matrix ErrorMinW
        {
            get { return errorMinW; }
        }

        /// <summary>
        /// Gets current iteration round.
        /// </summary>
        public int Iter
        {
            get { return iter; }
        }

        /// <summary>
        /// Gets and sets maximum iteration number.
        /// </summary>
        public int MaxIter
        {
            get { return maxIter; }
            set { maxIter = value; }
        }

        /// <summary>
        /// Gets current loss.
        /// </summary>
        public double Loss
        {
            get { return loss; }
        }

        /// <summary>
        /// Gets minimum loss.
        /// </summary>
        public double MinLoss
        {
            get { return minLoss; }
        }

        /// <summary>
        /// Gets and sets training set errors.
        /// </summary>
        public List<double> TrErrs
        {
            get { return trErrs; }
            set { trErrs = value; }
        }

        /// <summary>
        /// Gets and sets validation set errors.
        /// </summary>
        public List<double> ValErrs
        {
            get { return valErrs; }
            set { valErrs = value; }
        }

        /// <summary>
        /// Gets and sets training set losses.
        /// </summary>
        public List<double> TrLosses
        {
            get { return trLosses; }
            set { trLosses = value; }
        }

        /// <summary>
        /// Gets and sets validation set losses.
        /// </summary>
        public List<double> ValLosses
        {
            get { return valLosses; }
            set { valLosses = value; }
        }

        /// <summary>
        /// Gets error rate per digit.
        /// </summary>
        public List<List<double>> DigitPer
        {
            get { return digitPer; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public GLMExtremeClassifier()
        {
        }

        /// <summary>
        /// A constructor with training data
        /// </summary>
        /// <param name="data">Training data</param>
        public GLMExtremeClassifier(List<KeyValuePair<byte[], T>> data, List<KeyValuePair<byte[], T>> valData, List<T> classes, double lam = 10000, double ta = 10, byte norm = 0)
        {            
            if (data == null || valData == null || classes == null)
            {
                throw new ArgumentNullException();
            }

            Data = data;
            ValData = valData;
            Classes = classes;
            w = new Matrix(classes.Count, data[0].Key.Length + 1);
            lambda = lam;
            tau = ta;
            normType = norm;

            for (int i = 0; i < classes.Count; i++)
            {
                digitPer.Add(new List<double>());
            }

            trErrs.Add(GetErrorRate(0));
            valErrs.Add(GetErrorRate(1));
            trLosses.Add(CalculateReguledConvexLoss(0));
            valLosses.Add(CalculateReguledConvexLoss(1));
        }
        #endregion

        #region abstract methods
        /// <summary>
        /// Link function. It uses w as the coef.
        /// </summary>
        /// <returns></returns>
        public abstract Matrix G(Matrix x);

        /// <summary>
        /// Link function. It uses wm as the coef.
        /// </summary>
        /// <returns></returns>
        public abstract Matrix G(Matrix wm, Matrix x);

        /// <summary>
        /// Convex function.
        /// </summary>
        /// <returns></returns>
        public abstract double Fi(Matrix x);
        #endregion

        #region methods
        /// <summary>
        /// Resets this classifier
        /// </summary>
        public void ResetClassifier()
        {
            w = new Matrix(classes.Count, data[0].Key.Length + 1);
            iter = 0;
            minLoss = double.MaxValue;

            trErrs.Clear();
            valErrs.Clear();
            trLosses.Clear();
            valLosses.Clear();

            trErrs.Add(GetErrorRate(0));
            valErrs.Add(GetErrorRate(1));
            trLosses.Add(CalculateReguledConvexLoss(0));
            valLosses.Add(CalculateReguledConvexLoss(1));
        }

        /// <summary>
        /// Calculates sigma hat.
        /// </summary>
        private void CalSigmaHat()
        {
            Matrix x = new Matrix(data[0].Key.Length + 1, 1);
            Matrix identity = new Matrix(data[0].Key.Length + 1);

            sigmaHat = new Matrix(data[0].Key.Length + 1);
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Key.Length; j++)
                {
                    x[j, 0] = data[i].Key[j];
                }
                x[data[i].Key.Length, 0] = 1;

                sigmaHat = sigmaHat + (1.0 / data.Count) * (x * x.Transpose());
            }

            for(int i = 0; i < identity.RowCount; i++)
            {
                identity[i, i] = 1;
            }

            sigmaHat = sigmaHat + lambda * identity;

            sigmaInv = sigmaHat.Inverse();
        }

        /// <summary>
        /// Calculates convex loss l(W; (xi, yi)) = Fi(Wx)- yTWx
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private double CalculateConvexLossAt(int i, int type)
        {
            Matrix x = new Matrix(data[i].Key.Length + 1, 1);
            Matrix y = new Matrix(classes.Count, 1);
            if (type == 0)
            {
                for (int j = 0; j < data[i].Key.Length; j++)
                {
                    x[j, 0] = data[i].Key[j];
                }
                x[data[i].Key.Length, 0] = 1;
                y[classes.IndexOf(data[i].Value), 0] = 1;
            }
            else
            {
                for (int j = 0; j < valData[i].Key.Length; j++)
                {
                    x[j, 0] = valData[i].Key[j];
                }
                x[valData[i].Key.Length, 0] = 1;
                y[classes.IndexOf(valData[i].Value), 0] = 1;
            }
            
            return Fi(x) - (y.Transpose() * W * x)[0, 0];
        }

        /// <summary>
        /// Calculates convex loss for all data.
        /// </summary>
        /// <returns></returns>
        private double CalculateConvexLoss(int type)
        {
            double lnWt = 0;

            if (type == 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    lnWt += CalculateConvexLossAt(i, type) / data.Count;
                }
            }
            else
            {
                for (int i = 0; i < valData.Count; i++)
                {
                    lnWt += CalculateConvexLossAt(i, type) / valData.Count;
                }
            }

            return lnWt;
        }

        /// <summary>
        /// Calculates regularized convex loss with trace norm.
        /// </summary>
        /// <returns></returns>
        public double CalculateReguledConvexLoss(int type)
        {
            if (normType == 0)
            {
                loss = CalculateConvexLoss(type) + tau * (w * w.Transpose()).Trace;
            }
            else if(normType == 1)
            {
                loss = CalculateConvexLoss(type) + tau * (w * w.Transpose()).Norm1();
            }
            else if (normType == 2)
            {
                loss = CalculateConvexLoss(type) + tau * (w * w.Transpose()).FrobeniusNorm();
            }

            if (loss < minLoss && type == 1)
            {
                minLoss = loss;
                argminW = w.Clone();
            }

            return loss;
        }

        /// <summary>
        /// Iterates one round and upgrades W and loss.
        /// </summary>
        public void ExecuteOneIteration()
        {
            Matrix estErr = new Matrix(classes.Count, data[0].Key.Length + 1);
            Matrix yHat;
            Matrix x = new Matrix(data[0].Key.Length + 1, 1);
            Matrix y;
            Matrix dw = new Matrix(classes.Count, data[0].Key.Length + 1);

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Key.Length; j++)
                {
                    x[j, 0] = data[i].Key[j];
                }
                x[data[i].Key.Length, 0] = 1;

                y = new Matrix(classes.Count, 1);
                y[classes.IndexOf(data[i].Value), 0] = 1;

                yHat = G(x);
                estErr = estErr + (1.0 / data.Count) * ((yHat - y) * x.Transpose());
            }

            if (normType == 0)
            {
                for (int i = 0; i < Math.Min(dw.RowCount, dw.ColumnCount); i++)
                {
                    dw[i, i] = 1;
                }
            }
            else if (normType == 1)
            {
                for (int i = 0; i < dw.RowCount; i++)
                {
                    for (int j = 0; j < dw.ColumnCount; j++)
                    {
                        dw[i, j] = Math.Sign(w[i, j]);
                    }
                }
            }
            else if (normType == 2)
            {
                dw = 2 * w;
            }

            estErr = estErr + tau * dw;

            if (normType == 0 || normType == 1)
            {
                w = w - (0.5 / Lipsz) * (sigmaInv * estErr.Transpose()).Transpose();
            }
            else if (normType == 2)
            {
                w = w - (0.2 / Lipsz) * (sigmaInv * estErr.Transpose()).Transpose();
            }

            iter++;

            trErrs.Add(GetErrorRate(0));
            valErrs.Add(GetErrorRate(1));
            trLosses.Add(CalculateReguledConvexLoss(0));
            valLosses.Add(CalculateReguledConvexLoss(1));
        }

        /// <summary>
        /// Gets error rate for training or validation data set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private double GetErrorRate(int type)
        {
            double per;
            int[] digitCnt = new int[10];
            int cnt = 0;
            T res;
            List<KeyValuePair<byte[], T>> set;

            if (type == 0)
            {
                set = data;
            }
            else
            {
                set = valData;
            }

            for (int i = 0; i < set.Count; i++)
            {
                res = TestWithW(set[i].Key);

                if (classes.IndexOf(res) != classes.IndexOf(set[i].Value))
                {
                    cnt++;
                    if (type != 0)
                    {
                        digitCnt[classes.IndexOf(set[i].Value)]++;
                    }
                }
            }

            per = cnt * 100.0 / set.Count;

            if (type != 0 && per < minError)
            {
                minError = per;
                errorMinW = w.Clone();                
            }

            if (type != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    digitPer[i].Add(digitCnt[i] * 100.0 / cnt);
                }
            }

            return per;
        }

        /// <summary>
        /// Gets error rate for data set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        private double GetErrorRate(List<KeyValuePair<byte[], T>> set)
        {
            double per;
            int cnt = 0;
            T res;

            for (int i = 0; i < set.Count; i++)
            {
                res = TestWithW(set[i].Key);

                if (classes.IndexOf(res) != classes.IndexOf(set[i].Value))
                {
                    cnt++;
                }
            }

            per = cnt * 100.0 / set.Count;

            return per;
        }

        /// <summary>
        /// Returns the result of test with errorMinW.
        /// </summary>
        /// <param name="data">Input for test</param>
        /// <returns></returns>
        public T TestVal(byte[] data)
        {
            Matrix x = new Matrix(data.Length + 1, 1);
            Matrix y;
            double max = double.MinValue;
            int index = -1;

            for (int i = 0; i < data.Length; i++)
            {
                x[i, 0] = data[i];
            }
            x[data.Length, 0] = 1;

            y = G(errorMinW, x);

            for (int i = 0; i < y.RowCount; i++)
            {
                if (y[i, 0] > max)
                {
                    max = y[i, 0];
                    index = i;
                }
            }

            return classes[index];
        }

        /// <summary>
        /// Returns the result of test with argmin.
        /// </summary>
        /// <param name="data">Input for test</param>
        /// <returns></returns>
        public T Test(byte[] data)
        {
            Matrix x = new Matrix(data.Length + 1, 1);
            Matrix y;
            double max = double.MinValue;
            int index = -1;

            for (int i = 0; i < data.Length; i++)
            {
                x[i, 0] = data[i];
            }
            x[data.Length, 0] = 1;

            y = G(argminW, x);

            for (int i = 0; i < y.RowCount; i++)
            {
                if (y[i, 0] > max)
                {
                    max = y[i, 0];
                    index = i;
                }
            }

            return classes[index];
        }

        /// <summary>
        /// Returns the result of test with w.
        /// </summary>
        /// <param name="data">Input for test</param>
        /// <returns></returns>
        public T TestWithW(byte[] data)
        {
            Matrix x = new Matrix(data.Length + 1, 1);
            Matrix y;
            double max = double.MinValue;
            int index = -1;

            for (int i = 0; i < data.Length; i++)
            {
                x[i, 0] = data[i];
            }
            x[data.Length, 0] = 1;

            y = G(x);

            for (int i = 0; i < y.RowCount; i++)
            {
                if (y[i, 0] > max)
                {
                    max = y[i, 0];
                    index = i;
                }
            }

            return classes[index];
        }

        /// <summary>
        /// Writes this classifier to file.
        /// </summary>
        /// <remarks>
        /// Signature(MINEGLM\0), type, lipsz, lambda, tau, norm, iter, minLoss, minError, wrow, wcol, argminW, errorMinW
        /// </remarks>
        /// <param name="fileName"></param>
        public void WriteToFile(string fileName)
        {
            FileStream fs;
            BinaryWriter writer;

            try
            {
                using (fs = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    writer = new BinaryWriter(fs);

                    // signature
                    writer.Write('M');
                    writer.Write('I');
                    writer.Write('N');
                    writer.Write('E');
                    writer.Write('G');
                    writer.Write('L');
                    writer.Write('M');
                    writer.Write('\0');

                    // type
                    writer.Write(type);

                    // lipsz
                    writer.Write(lipsz);

                    // lambda
                    writer.Write(lambda);

                    // tau
                    writer.Write(tau);

                    // norm
                    writer.Write(normType);

                    // iter
                    writer.Write(iter);

                    // minLoss
                    writer.Write(minLoss);

                    // minError
                    writer.Write(minError);

                    // wRow
                    writer.Write(argminW.RowCount);

                    // wCol
                    writer.Write(argminW.ColumnCount);

                    // argminW
                    for (int i = 0; i < argminW.RowCount; i++)
                    {
                        for (int j = 0; j < argminW.ColumnCount; j++)
                        {
                            writer.Write(argminW[i, j]);
                        }
                    }

                    // errorMinW
                    for (int i = 0; i < errorMinW.RowCount; i++)
                    {
                        for (int j = 0; j < errorMinW.ColumnCount; j++)
                        {
                            writer.Write(errorMinW[i, j]);
                        }
                    }

                    writer.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }            
        }

        public void WriteData()
        {
            string fileName = @"result train=" + data.Count.ToString() + " val=" + valData.Count.ToString() + "/" + "lambda=" + lambda.ToString() + " tau=" + tau.ToString() + " norm=";
            List<string> lines = new List<string>();

            if (normType == 0)
            {
                fileName += "Trace/";
            }
            else if (normType == 1)
            {
                fileName += "L1/";
            }
            else if (normType == 2)
            {
                fileName += "L2/";
            }

            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }

            // train errors            
            lines.Clear();
            for (int i = 0; i < trErrs.Count; i++)
            {
                lines.Add(trErrs[i].ToString());
            }

            File.WriteAllLines(fileName + "Training Errors.txt", lines);

            // val Errors
            lines.Clear();
            for (int i = 0; i < valErrs.Count; i++)
            {
                lines.Add(valErrs[i].ToString());
            }

            File.WriteAllLines(fileName + "Validation Errors.txt", lines);

            // train losses
            lines.Clear();
            for (int i = 0; i < trLosses.Count; i++)
            {
                lines.Add(trLosses[i].ToString());
            }

            File.WriteAllLines(fileName + "Train Losses.txt", lines);

            // validation losses
            lines.Clear();
            for (int i = 0; i < valLosses.Count; i++)
            {
                lines.Add(valLosses[i].ToString());
            }

            File.WriteAllLines(fileName + "Validation Losses.txt", lines);

            // digits
            for (int j = 0; j < 10; j++)
            {
                lines.Clear();
                for (int i = 0; i < digitPer[j].Count; i++)
                {
                    lines.Add(digitPer[j][i].ToString());
                }

                File.WriteAllLines(fileName + "Error in " + j.ToString() + ".txt", lines);
            }
        }
        #endregion

        #region override methods
        public override string ToString()
        {
            string str = "";

            str += type + " Classifier with ";

            if (normType == 0)
            {
                str += "Trace";
            }
            else if(normType == 1)
            {
                str += "L1";
            }
            else if(normType == 2)
            {
                str += "L2";
            }

            str += " regularization";
            str += " : Lambda = " + lambda.ToString() + " Tau = " + tau.ToString();

            return str;
        }
        #endregion
    }
}
