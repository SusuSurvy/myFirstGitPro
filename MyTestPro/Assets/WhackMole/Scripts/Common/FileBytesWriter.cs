using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Lobby3D.Common
{
    public class FileBytesWriter
    {
        private const byte FALSE_BYTE = 0;
        private const byte TRUE_BYTE = 1;

        private FileStream m_stream;

        public FileBytesWriter(FileStream stream)
        {
            m_stream = stream;
        }

        private void Write(byte[] buffer, int size)
        {
            m_stream.Write(buffer, 0, size);
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
            byte[] res = new byte[1];
            res[0] = bData;
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

        public void WriteRawData(byte[] buf)
        {
            if (buf != null)
            {
                Write(buf, buf.Length);
            }
        }

        public void WriteData(string data)
        {
            byte[] strContents = System.Text.Encoding.UTF8.GetBytes(data);
            short len = (short)strContents.Length;
            WriteData(len);
            Write(strContents, strContents.Length);
        }

        public void WriteData(char data)
        {
            byte[] res = BitConverter.GetBytes(data);
            Write(res, res.Length);
        }

    }
}
