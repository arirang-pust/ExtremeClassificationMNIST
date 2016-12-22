using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mine.Data.Readers;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading.Tasks;

namespace Mine.Engines.Classifiers
{
    public class GLMClassifierSelectAlgorithm
    {
        MNISTReader reader;
        public List<LogisticClassifier<byte>> classifiers;
        public List<KeyValuePair<byte[], byte>> trainData;
        public List<KeyValuePair<byte[], byte>> valData;
        List<byte> classes = new List<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        double[] lambdas = new double[] {0.001, 1, 1000};
        double[] taus = new double[] { 0.01, 0.1, 1, 10, 100, 1000};

        public GLMClassifierSelectAlgorithm()
        {
            reader = new MNISTReader();
            classifiers = new List<LogisticClassifier<byte>>();
        }

        /// <summary>
        /// Creates classifiers according to lambda and tau, norm type.
        /// </summary>
        /// <param name="trDpNum"></param>
        /// <param name="valDpNum"></param>
        public void Initialize(int trDpNum, int valDpNum)
        {
            LogisticClassifier<byte> newClassifier;

            classifiers.Clear();

            trainData = reader.GetTrainingSamplesOfDP(trDpNum);
            valData = reader.GetValidationSamplesOfDP(valDpNum);

            foreach (double lambda in lambdas)
            {
                foreach (double tau in taus)
                {
                    for (byte type = 0; type < 3; type++)
                    {
                        newClassifier = new LogisticClassifier<byte>(trainData, valData, classes, lambda, tau, type);
                        classifiers.Add(newClassifier);
                    }
                }
            }
        }

        public void InitializeOpt(int trDpNum, int valDpNum)
        {
            LogisticClassifier<byte> newClassifier;

            trainData = reader.GetTrainingSamplesOfDP(trDpNum);
            valData = reader.GetValidationSamplesOfDP(valDpNum);

            newClassifier = new LogisticClassifier<byte>(trainData, valData, classes, 1, 0.1, 1);
            classifiers.Add(newClassifier);
            newClassifier = new LogisticClassifier<byte>(trainData, valData, classes, 1, 1000, 2);
            classifiers.Add(newClassifier);
            newClassifier = new LogisticClassifier<byte>(trainData, valData, classes, 1, 1, 0);
            classifiers.Add(newClassifier);
        }

        /// <summary>
        /// Iterates all classifiers one round.
        /// </summary>
        public void ExecuteOneIteration()
        {
            Parallel.ForEach(classifiers, classifier =>
            {
                classifier.ExecuteOneIteration();
            });
        }

        /// <summary>
        /// Writes detailed data to files
        /// </summary>
        public void WriteAllData()
        {
            Parallel.ForEach(classifiers, classifier =>
            {
                classifier.WriteData();
            });
        }
    }
}
