using System;

namespace Lobby3D.Common
{
    public class BytesReader
    {
        private byte[] m_buffer;
        private int m_readIndex;

        public BytesReader(byte[] buffer)
        {
            m_buffer = buffer;
            m_readIndex = 0;
        }

        public byte[] Read(int size)
        {
            int currIndex = MoveNext(size);
            byte[] tmpBuffer = null;
            if (currIndex > -1)
            {
                tmpBuffer = new byte[size];

                Array.Copy(m_buffer, currIndex, tmpBuffer, 0, size);
            }

            return tmpBuffer;
        }
        private int MoveNext(int size)
        {
            int retIndex = m_readIndex;
            if (m_readIndex + size > m_buffer.Length)
            {
                throw new System.Exception("BytesReader overflow.");
            }
            m_readIndex += size;

            return retIndex;
        }

        public int ReadInt32()
        {
            int currIndex = MoveNext(sizeof(int));
            return BitConverter.ToInt32(m_buffer, currIndex);
        }

        public uint ReadUInt32()
        {
            int currIndex = MoveNext(sizeof(uint));
            return BitConverter.ToUInt32(m_buffer, currIndex);
        }

        public short ReadInt16()
        {
            int currIndex = MoveNext(sizeof(short));
            return BitConverter.ToInt16(m_buffer, currIndex);
        }

        public ushort ReadUInt16()
        {
            int currIndex = MoveNext(sizeof(ushort));
            return BitConverter.ToUInt16(m_buffer, currIndex);
        }

        public long ReadInt64()
        {
            int currIndex = MoveNext(sizeof(long));
            return BitConverter.ToInt64(m_buffer, currIndex);
        }

        public ulong ReadUInt64()
        {
            int currIndex = MoveNext(sizeof(ulong));
            return BitConverter.ToUInt64(m_buffer, currIndex);
        }

        public bool ReadBool()
        {
            int currIndex = MoveNext(sizeof(byte));
            return m_buffer[currIndex] == 1;
        }

        public float ReadFloat()
        {
            int currIndex = MoveNext(sizeof(float));
            return BitConverter.ToSingle(m_buffer, currIndex);
        }

        public double ReadDouble()
        {
            int currIndex = MoveNext(sizeof(double));
            return BitConverter.ToDouble(m_buffer, currIndex);
        }

        public byte ReadByte()
        {
            int currIndex = MoveNext(sizeof(byte));
            return m_buffer[currIndex];
        }

        public char ReadChar()
        {
            int currIndex = MoveNext(sizeof(char));
            return BitConverter.ToChar(m_buffer, currIndex);
        }

        public byte[] ReadBytes()
        {
            int byteLength = ReadInt32();
            byte[] byteContents = null;
            if(byteLength > 0)
            {
                byteContents = Read(byteLength);
            }
            return byteContents;
        }

        public string ReadStr()
        {
            int strLength = ReadInt16();
            int currIndex = MoveNext(strLength);

            return System.Text.Encoding.UTF8.GetString(m_buffer, currIndex, strLength); ;
        }
    }
}