using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mine.Data.Readers
{
    public class MNISTReader
    {
        #region static fields
        public static string _trSetLbPath = "train-labels.idx1-ubyte";
        public static string _trSetImgPath = "train-images.idx3-ubyte";
        public static string _tsSetLbPath = "t10k-labels.idx1-ubyte";
        public static string _tsSetImgPath = "t10k-images.idx3-ubyte";
        #endregion

        #region fields
        string trSetLbPath;
        string trSetImgPath;
        string tsSetLbPath;
        string tsSetImgPath;
        int trSetNum;
        int tsSetNum;
        int nRow;
        int nCol;
        #endregion

        #region properties
        /// <summary>
        ///  Training set label file path
        /// </summary>
        public string TrSetLbPath
        {
            get { return trSetLbPath; }
            set { trSetLbPath = value; }
        }

        /// <summary>
        /// Training set image file path
        /// </summary>
        public string TrSetImgPath
        {
            get { return trSetImgPath; }
            set { trSetImgPath = value; }
        }

        /// <summary>
        /// Test set label file path
        /// </summary>
        public string TsSetLbPath
        {
            get { return tsSetLbPath; }
            set { tsSetLbPath = value; }
        }

        /// <summary>
        /// Test set image file path
        /// </summary>
        public string TsSetImgPath
        {
            get { return tsSetImgPath; }
            set { tsSetImgPath = value; }
        }

        /// <summary>
        /// Number of training samples 
        /// </summary>
        public int TrSetNum
        {
            get { return trSetNum; }
            set { trSetNum = value; }
        }

        /// <summary>
        /// Number of test samples 
        /// </summary>
        public int TsSetNum
        {
            get { return tsSetNum; }
            set { tsSetNum = value; }
        }

        /// <summary>
        /// Number of rows in a digit image
        /// </summary>
        public int NRow
        {
            get { return nRow; }
            set { nRow = value; }
        }

        /// <summary>
        /// Number of columns in a digit image
        /// </summary>
        public int NCol
        {
            get { return nCol; }
            set { nCol = value; }
        }
        #endregion

        #region constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public MNISTReader()
        {
            trSetImgPath = MNISTReader._trSetImgPath;
            tsSetImgPath = MNISTReader._tsSetImgPath;
            trSetLbPath = MNISTReader._trSetLbPath;
            tsSetLbPath = MNISTReader._tsSetLbPath;

            if (!ValidatePath())
            {
                throw new System.ArgumentException();
            }
        }

        /// <summary>
        /// Specific constructor
        /// </summary>
        public MNISTReader(string dir)
        {
            trSetImgPath = dir + "\\" + MNISTReader._trSetImgPath;
            tsSetImgPath = dir + "\\" + MNISTReader._tsSetImgPath;
            trSetLbPath = dir + "\\" + MNISTReader._trSetLbPath;
            tsSetLbPath = dir + "\\" + MNISTReader._tsSetLbPath;

            if (!ValidatePath())
            {
                throw new System.ArgumentException();
            }
        }
        #endregion

        #region methods
        /// <summary>
        /// Validates file path and gets sample numbers and image size.
        /// </summary>
        /// <returns></returns>
        private bool ValidatePath()
        {
            FileStream fs;
            BinaryReader reader;
            int magic;

            try
            {
                fs = new FileStream(trSetLbPath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                magic = reader.ReadInt32();
                magic = magic / 0x1000000 + ((magic % 0x1000000) / 0x10000) * 0x100 + ((magic % 0x10000) / 0x100) * 0x10000 + (magic % 0x100) * 0x1000000; // big endian to little endian
                trSetNum = reader.ReadInt32();
                trSetNum = trSetNum / 0x1000000 + ((trSetNum % 0x1000000) / 0x10000) * 0x100 + ((trSetNum % 0x10000) / 0x100) * 0x10000 + (trSetNum % 0x100) * 0x1000000; // big endian to little endian
                reader.Close();
                fs.Close();

                if (magic != 0x0801)
                {
                    return false;
                }

                fs = new FileStream(tsSetLbPath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                magic = reader.ReadInt32();
                magic = magic / 0x1000000 + ((magic % 0x1000000) / 0x10000) * 0x100 + ((magic % 0x10000) / 0x100) * 0x10000 + (magic % 0x100) * 0x1000000; // big endian to little endian
                tsSetNum = reader.ReadInt32();
                tsSetNum = tsSetNum / 0x1000000 + ((tsSetNum % 0x1000000) / 0x10000) * 0x100 + ((tsSetNum % 0x10000) / 0x100) * 0x10000 + (tsSetNum % 0x100) * 0x1000000; // big endian to little endian
                reader.Close();
                fs.Close();

                if (magic != 0x0801)
                {
                    return false;
                }

                fs = new FileStream(trSetImgPath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                magic = reader.ReadInt32();
                magic = magic / 0x1000000 + ((magic % 0x1000000) / 0x10000) * 0x100 + ((magic % 0x10000) / 0x100) * 0x10000 + (magic % 0x100) * 0x1000000; // big endian to little endian
                reader.ReadInt32();
                nRow = reader.ReadInt32();
                nRow = nRow / 0x1000000 + ((nRow % 0x1000000) / 0x10000) * 0x100 + ((nRow % 0x10000) / 0x100) * 0x10000 + (nRow % 0x100) * 0x1000000; // big endian to little endian
                nCol = reader.ReadInt32();
                nCol = nCol / 0x1000000 + ((nCol % 0x1000000) / 0x10000) * 0x100 + ((nCol % 0x10000) / 0x100) * 0x10000 + (nCol % 0x100) * 0x1000000; // big endian to little endian
                reader.Close();
                fs.Close();
                
                if (magic != 0x0803)
                {
                    return false;
                }

                fs = new FileStream(tsSetImgPath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                magic = reader.ReadInt32();
                magic = magic / 0x1000000 + ((magic % 0x1000000) / 0x10000) * 0x100 + ((magic % 0x10000) / 0x100) * 0x10000 + (magic % 0x100) * 0x1000000; // big endian to little endian
                reader.Close();
                fs.Close();

                if (magic != 0x0803)
                {
                    return false;
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the pixel value of (x, y) pixel of the ith training image.
        /// </summary>
        /// <param name="i">image no</param>
        /// <param name="x">col</param>
        /// <param name="y">row</param>
        /// <returns></returns>
        public byte GetTraingImagePixelAt(int i, int x, int y)
        {
            FileStream fs;
            BinaryReader reader;
            byte pixel;

            if(x >= nCol || y >= nRow || i >= trSetNum || x < 0 || y < 0 || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(trSetImgPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(nRow * nCol * i + 0x10 + y * nRow + x, SeekOrigin.Begin);
                    pixel = reader.ReadByte();
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return pixel;
        }

        /// <summary>
        /// Returns the ith training image as a byte array.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte[] GetTrainingImageDataAt(int i)
        {
            FileStream fs;
            BinaryReader reader;
            byte[] pixels;

            if (i >= trSetNum || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(trSetImgPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(nRow * nCol * i + 0x10, SeekOrigin.Begin);
                    pixels = reader.ReadBytes(nRow * nCol);
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return pixels;
        }

        /// <summary>
        /// Returns the ith label of training data
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte GetTrainingLabelAt(int i)
        {
            FileStream fs;
            BinaryReader reader;
            byte lb;

            if (i >= trSetNum || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(trSetLbPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(i + 0x08, SeekOrigin.Begin);
                    lb = reader.ReadByte();
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return lb;
        }

        /// <summary>
        /// Returns the ith training sample as key-value pair of pixels and label.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public KeyValuePair<byte[], byte> GetTrainingSampleAt(int i)
        {
            return new KeyValuePair<byte[], byte>(GetTrainingImageDataAt(i), GetTrainingLabelAt(i));
        }

        /// <summary>
        /// Returns all the training samples in the range as a list of key-value pairs.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllTrainingSamples(int s, int l)
        {
            if (s < 0 || s >= trSetNum || s + l > trSetNum)
            {
                throw new ArgumentOutOfRangeException();
            }

            List<KeyValuePair<byte[], byte>> allList = new List<KeyValuePair<byte[], byte>>();

            for (int i = s; i < s + l; i++)
            {
                allList.Add(GetTrainingSampleAt(i));
            }

            return allList;
        }

        /// <summary>
        /// Returns all the training samples as a list of key-value pairs.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllTrainingSamples()
        {
            List<KeyValuePair<byte[], byte>> allList = new List<KeyValuePair<byte[],byte>>();

            for (int i = 0; i < trSetNum; i++)
            {
                allList.Add(GetTrainingSampleAt(i));
            }

            return allList;
        }
        
        /// <summary>
        /// Returns training set with num data points for each digit.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetTrainingSamplesOfDP(int num)
        {
            List<KeyValuePair<byte[], byte>> list = new List<KeyValuePair<byte[], byte>>();
            KeyValuePair<byte[], byte> sample;
            int[] digitCnt = new int[10];
            List<int> picked = new List<int>();
            int ran;
            Random rand = new Random(DateTime.Now.Millisecond);

            while (picked.Count < trSetNum && list.Count < 10 * num)
            {
                ran = rand.Next(trSetNum);
                sample = GetTrainingSampleAt(ran);
                if (digitCnt[sample.Value] < num)
                {
                    list.Add(sample);
                    digitCnt[sample.Value]++;
                }
                picked.Add(ran);
            }

            return list;
        }

        /// <summary>
        /// Returns validation set with num data points for each digit.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetValidationSamplesOfDP(int num)
        {
            List<KeyValuePair<byte[], byte>> list = new List<KeyValuePair<byte[], byte>>();
            KeyValuePair<byte[], byte> sample;
            int[] digitCnt = new int[10];
            List<int> picked = new List<int>();
            int ran;
            Random rand = new Random(DateTime.Now.Millisecond);

            while (picked.Count < tsSetNum && list.Count < 10 * num)
            {
                ran = rand.Next(tsSetNum);
                sample = GetTestSampleAt(ran);
                if (digitCnt[sample.Value] < num)
                {
                    list.Add(sample);
                    digitCnt[sample.Value]++;
                }
                picked.Add(ran);
            }

            return list;
        }

        /// <summary>
        /// Returns the pixel value of (x, y) pixel of ith test image.
        /// </summary>
        /// <param name="i">image no</param>
        /// <param name="x">col</param>
        /// <param name="y">row</param>
        /// <returns></returns>
        public byte GetTestImagePixelAt(int i, int x, int y)
        {
            FileStream fs;
            BinaryReader reader;
            byte pixel;

            if (x >= nCol || y >= nRow || i >= tsSetNum || x < 0 || y < 0 || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(tsSetImgPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(nRow * nCol * i + 0x10 + y * nRow + x, SeekOrigin.Begin);
                    pixel = reader.ReadByte();
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return pixel;
        }

        /// <summary>
        /// Returns the ith test image as a byte array.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte[] GetTestImageDataAt(int i)
        {
            FileStream fs;
            BinaryReader reader;
            byte[] pixels;

            if (i >= tsSetNum || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(tsSetImgPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(nRow * nCol * i + 0x10, SeekOrigin.Begin);
                    pixels = reader.ReadBytes(nRow * nCol);
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return pixels;
        }

        /// <summary>
        /// Returns the ith label of test data
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte GetTestLabelAt(int i)
        {
            FileStream fs;
            BinaryReader reader;
            byte lb;

            if (i >= tsSetNum || i < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            try
            {
                using (fs = File.Open(tsSetLbPath, FileMode.Open, FileAccess.Read))
                {
                    reader = new BinaryReader(fs, Encoding.BigEndianUnicode);
                    reader.BaseStream.Seek(i + 0x08, SeekOrigin.Begin);
                    lb = reader.ReadByte();
                    reader.Close();
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return lb;
        }

        /// <summary>
        /// Returns the ith training sample as key-value pair of pixels and label.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public KeyValuePair<byte[], byte> GetTestSampleAt(int i)
        {
            return new KeyValuePair<byte[], byte>(GetTestImageDataAt(i), GetTestLabelAt(i));
        }

        /// <summary>
        /// Returns all the test samples in the range as a list of key-value pairs.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllTestSamples(int s, int l)
        {
            if (s < 0 || s >= tsSetNum || s + l > tsSetNum)
            {
                throw new ArgumentOutOfRangeException();
            }

            List<KeyValuePair<byte[], byte>> allList = new List<KeyValuePair<byte[], byte>>();

            for (int i = s; i < s + l; i++)
            {
                allList.Add(GetTestSampleAt(i));
            }

            return allList;
        }

        /// <summary>
        /// Returns all the test samples as a list of key-value pairs.
        /// </summary>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllTestSamples()
        {
            List<KeyValuePair<byte[], byte>> allList = new List<KeyValuePair<byte[], byte>>();

            for (int i = 0; i < tsSetNum; i++)
            {
                allList.Add(GetTestSampleAt(i));
            }

            return allList;
        }

        /// <summary>
        /// Returns the pixel value of the ith image of the "type" data set.
        /// </summary>
        /// <param name="type">Training or Test</param>
        /// <param name="i">image no</param>
        /// <param name="x">col</param>
        /// <param name="y">row</param>
        /// <returns></returns>
        public byte GetImagePixelAt(string type, int i, int x, int y)
        {
            if (type == "Training")
            {
                return GetTraingImagePixelAt(i, x, y);
            }
            else if (type == "Test")
            {
                return GetTestImagePixelAt(i, x, y);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the ith "type" image as a byte array.
        /// </summary>
        /// <param name="type">Training or Test</param>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte[] GetImageDataAt(string type, int i)
        {
            if (type == "Training")
            {
                return GetTrainingImageDataAt(i);
            }
            else if (type == "Test")
            {
                return GetTestImageDataAt(i);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns the ith label of "type" data
        /// </summary>
        /// <param name="type">Training or Test</param>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte GetLabelAt(string type, int i)
        {
            if (type == "Training")
            {
                return GetTrainingLabelAt(i);
            }
            else if (type == "Test")
            {
                return GetTestLabelAt(i);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        /// <summary>
        /// Returns all the "type" samples in the range as a list of key-value pairs.
        /// </summary>
        /// <param name="type">Training or Test</param>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllSamples(string type, int s, int l)
        {
            if (type == "Training")
            {
                return GetAllTrainingSamples(s, l);
            }
            else if (type == "Test")
            {
                return GetAllTestSamples(s, l);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns all the "type" samples as a list of key-value pairs.
        /// </summary>
        /// <param name="type">Training or Test</param>
        /// <returns></returns>
        public List<KeyValuePair<byte[], byte>> GetAllSamples(string type)
        {
            if (type == "Training")
            {
                return GetAllTrainingSamples();
            }
            else if (type == "Test")
            {
                return GetAllTestSamples();
            }
            else
            {
                throw new ArgumentException();
            }
        }
        #endregion
    }
}
