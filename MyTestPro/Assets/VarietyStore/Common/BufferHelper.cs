using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Common
{
    public sealed class BufferHelper
    {
        private const byte FALSE_BYTE = 0;
        private const byte TRUE_BYTE = 1;
        private const int DEFAULT_BUFFER_SIZE = 1024;

        private byte[] m_buffer;
        public byte[] ByteBuffer
        {
            get { return this.m_buffer; }
        }

        private int m_iRead;
        internal int ReadIndex
        {
            get { return this.m_iRead; }
        }

        private int m_iWrite;
        public int WriteIndex
        {
            get { return this.m_iWrite; }
        }

        private int m_iCapacity;
        internal int Capacity
        {
            get { return this.m_iCapacity; }
        }

        private int m_iSize;

        public BufferHelper()
        {
            this.m_iCapacity = DEFAULT_BUFFER_SIZE;
            byte[] capBuffer = new byte[DEFAULT_BUFFER_SIZE];
            InitBuffer(capBuffer, 0, 0, 0);
        }

        public BufferHelper(int bufferSize)
        {
            this.m_iCapacity = 0;
            byte[] capBuffer = null;
            if (bufferSize > 0)
            {
                m_iCapacity = bufferSize;
                capBuffer = new byte[bufferSize];
            }

            InitBuffer(capBuffer, 0, 0, 0);
        }

        public BufferHelper(byte[] buffer)
        {
            this.m_iCapacity = 0;
            if (buffer != null)
            {
                this.m_iCapacity = buffer.Length;
            }

            InitBuffer(buffer, m_iCapacity, m_iCapacity, 0);
        }

        #region WriteData

        private void Write(byte[] buffer, int size)
        {
            if (this.m_iWrite + size > this.m_iCapacity)
            {
                this.m_iCapacity = m_iWrite + size * 2;
                byte[] tempBuffer = new byte[m_iCapacity];
                Array.Copy(this.m_buffer, 0, tempBuffer, 0, this.m_iWrite);
                this.m_buffer = tempBuffer;
            }

            Array.Copy(buffer, 0, this.m_buffer, this.m_iWrite, size);
            WriteSize(size);
        }

        private void WriteSize(int size)
        {
            this.m_iWrite += size;
            this.m_iSize += size;

            SizeChanged();
        }

        public void WriteData(int data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(uint data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(float data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(short data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(ushort data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(long data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(ulong data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(byte data)
        {
            byte[] res = new byte[1];
            res[0] = data;
            Write(res, res.Length);
        }

        public void WriteData(double data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteData(bool data)
        {
            byte bData = data ? TRUE_BYTE : FALSE_BYTE;
            byte[] res = new byte[1] { bData };
            Write(res, res.Length);
        }

        public void WriteData(byte[] res)
        {
            if (res == null)
            {
                WriteData(0);
            }
            else
            {
                WriteData(res.Length);
                Write(res, res.Length);
            }
        }

        public void WriteData(string data)
        {
            byte[] strContents = System.Text.Encoding.UTF8.GetBytes(data);
            WriteData(strContents.Length);
            Write(strContents, strContents.Length);
        }

        public void WriteData(char data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

        public void WriteBuffer(byte[] data, int size)
        {
            Write(data, size);
        }
        #endregion

        #region Read
        public byte[] Read(int size, int offset = 0)
        {
            int currIndex = ReadSize(size + offset);
            byte[] tmpBuffer = null;
            if (currIndex > -1)
            {
                tmpBuffer = new byte[size];

                Array.Copy(this.m_buffer, currIndex + offset, tmpBuffer, 0, size);
            }

            return tmpBuffer;
        }

        private int ReadSize(int size)
        {
            int retIndex = this.m_iRead;
            if (this.m_iRead + size > this.m_iWrite)
            {
                Debug.LogError("Buffer is not have enough content.");
                return -1;
            }
            this.m_iRead += size;
            this.m_iSize -= size;
            SizeChanged();
            return retIndex;
        }

        private void SizeChanged()
        {
        }

        public int ReadInt32()
        {
            int currIndex = ReadSize(sizeof(int));
            int ret = BitConverter.ToInt32(m_buffer, currIndex);
            return ret;
        }

        public uint ReadUInt32()
        {
            int currIndex = ReadSize(sizeof(uint));
            return BitConverter.ToUInt32(m_buffer, currIndex);
        }

        public short ReadInt16()
        {
            int currIndex = ReadSize(sizeof(short));
            return BitConverter.ToInt16(m_buffer, currIndex);
        }

        public ushort ReadUInt16()
        {
            int currIndex = ReadSize(sizeof(ushort));
            return BitConverter.ToUInt16(m_buffer, currIndex);
        }

        public long ReadInt64()
        {
            int currIndex = ReadSize(sizeof(long));
            return BitConverter.ToInt64(m_buffer, currIndex);
        }

        public ulong ReadUInt64()
        {
            int currIndex = ReadSize(sizeof(ulong));
            return BitConverter.ToUInt64(m_buffer, currIndex);
        }

        public bool ReadBool()
        {
            int currIndex = ReadSize(sizeof(byte));
            return m_buffer[currIndex] == TRUE_BYTE;
        }

        public float ReadFloat()
        {
            int currIndex = ReadSize(sizeof(float));
            return BitConverter.ToSingle(m_buffer, currIndex);
        }

        public double ReadDouble()
        {
            int currIndex = ReadSize(sizeof(double));
            return BitConverter.ToDouble(m_buffer, currIndex);
        }

        public byte ReadByte()
        {
            int currIndex = ReadSize(sizeof(byte));
            return m_buffer[currIndex];
        }

        public char ReadChar()
        {
            int currIndex = ReadSize(sizeof(char));
            return BitConverter.ToChar(m_buffer, currIndex);
        }

        public byte[] ReadBytes()
        {
            int byteLength = ReadInt32();
            byte[] byteContents = null;
            if (byteLength > 0)
            {
                byteContents = Read(byteLength);
            }
            return byteContents;
        }

        public string ReadStr()
        {
            int strLength = ReadInt32();
            int currIndex = ReadSize(strLength);

            return System.Text.Encoding.UTF8.GetString(m_buffer, currIndex, strLength);
        }

        public List<int> ReadInt32List()
        {
            int num = ReadInt32();
            var data = new List<int>(num);
            for(int i = 0; i < num; ++i)
            {
                var item = ReadInt32();
                data.Add(item);
            }

            return data;
        }

        public List<float> ReadFloatList()
        {
            int num = ReadInt32();
            var data = new List<float>(num);
            for(int i = 0; i < num; ++i)
            {
                var item = ReadFloat();
                data.Add(item);
            }

            return data;
        }

        public List<string> ReadStrList()
        {
            int num = ReadInt32();
            var data = new List<string>(num);
            for(int i = 0; i < num; ++ i)
            {
                var item = ReadStr();
                data.Add(item);
            }

            return data;
        }

        public List<T> ReadEnumList<T>(T count)
        {
            List<T> data = new List<T>();
            int num = ReadInt32();
            for(int i = 0; i < num; ++i)
            {
                var item = ReadInt32();
                data.Add((T)Convert.ChangeType(item, typeof(int)));
            }

            return data;
        }

        #endregion

        #region Other Func
        private void InitBuffer(byte[] buffer, int size, int writeIndex, int readIndex)
        {
            this.m_buffer = buffer;
            this.m_iSize = size;
            this.m_iRead = readIndex;
            this.m_iWrite = writeIndex;
        }

        internal int GetSendBuffeLength()
        {
            return m_iSize;
        }

        internal int GetSurplusCapacity()
        {
            return m_iCapacity - m_iWrite;
        }

        internal void FormationBuffer()
        {
            if (this.m_iSize > 0)
            {
                Array.Copy(this.m_buffer, this.m_iRead, this.m_buffer, 0, this.m_iSize);
            }
            this.m_iRead = 0;
            this.m_iWrite = this.m_iSize;
            if (this.m_iCapacity > 0)
            {
                Array.Clear(this.m_buffer, this.m_iWrite, this.m_iCapacity - this.m_iWrite);
            }
        }

        internal void ClearBuffer()
        {
            if (this.m_iCapacity > 0)
            {
                Array.Clear(this.m_buffer, 0, m_iCapacity);
            }
            this.m_iSize = 0;
            this.m_iRead = 0;
            this.m_iWrite = 0;

            SizeChanged();
        }

        internal byte[] GetCopyOfContent()
        {
            byte[] tmpBuffer = null;
            if (this.m_iSize > 0)
            {
                tmpBuffer = new byte[this.m_iSize];
                System.Array.Copy(this.m_buffer, m_iRead, tmpBuffer, 0, m_iSize);
            }

            return tmpBuffer;
        }

        internal int GetSendBuffer(out byte[] sendBuffer)
        {
            sendBuffer = null;
            if (m_iSize > 0)
            {
                sendBuffer = m_buffer;
            }
            return m_iSize;
        }

        public void WriteRawData(byte[] buf)
        {
            if (buf != null)
            {
                Write(buf, buf.Length);
            }
        }

        public byte[] GetSendBuffer()
        {
            byte[] res = Read(this.m_iSize);
            return res;
        }
        #endregion
    }

}