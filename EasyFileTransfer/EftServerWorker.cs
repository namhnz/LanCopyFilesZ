using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyFileTransfer.Model;

namespace EasyFileTransfer
{
    public class EftServer
    {
        public string SaveTo;
        public int Port;
        TcpListener obj_server;

        // private readonly BackgroundWorker _worker;

        #region Custom code

        // Nguon: https://stackoverflow.com/a/85188/7182661
        public event EventHandler<DataReceivingArgs> DataStartReceiving;

        public void OnDataStartReceiving(string receivingFileName)
        {
            EventHandler<DataReceivingArgs> handler = DataStartReceiving;
            if (null != handler) handler(this, new DataReceivingArgs() { ReceivingFileName = receivingFileName });
        }

        public event EventHandler<DataReceivingArgs> DataFinishReceiving;

        public void OnDataFinishReceiving(string receivingFileName)
        {
            EventHandler<DataReceivingArgs> handler = DataFinishReceiving;
            if (null != handler) handler(this, new DataReceivingArgs() { ReceivingFileName = receivingFileName });
        }

        #endregion

        public EftServer(string SaveTo, int Port)
        {
            this.SaveTo = SaveTo;
            this.Port = Port;
            obj_server = new TcpListener(IPAddress.Any, Port);
        }

        /// <summary>
        /// Start EftServer and listening on port
        /// </summary>
        public async Task StartServer()
        {
            obj_server.Start();

            while (true)
            {
                TcpClient tc = await obj_server.AcceptTcpClientAsync().ConfigureAwait(false);
                SocketHandler obj_handler = new SocketHandler(tc, SaveTo, true);

                Task.Run(() => { obj_handler.ProcessSocketRequest(OnDataStartReceiving, OnDataFinishReceiving); });
            }
        }
    }

    class SocketHandler
    {
        // NetworkStream ns;
        string SaveTo;
        private readonly bool _ownsClient;

        private TcpClient _client;

        public SocketHandler(TcpClient client, string saveTo, bool ownsClient)
        {
            this.SaveTo = saveTo;
            _ownsClient = ownsClient;
            // ns = client.GetStream();

            _client = client;
        }

        public void ProcessSocketRequest(Action<string> onDataStartReceiving, Action<string> onDataFinishReceiving)
        {
            try
            {
                using (var stream = _client.GetStream())
                {
                    string receivingFileName = "";

                    FileStream fs = null;
                    long current_file_pointer = 0;
                    Boolean loop_break = false;

                    while (true)
                    {
                        if (stream.ReadByte() == 2)
                        {
                            byte[] cmd_buffer = new byte[3];
                            stream.Read(cmd_buffer, 0, cmd_buffer.Length);
                            byte[] recv_data = ReadStream(stream);
                            switch (Convert.ToInt32(Encoding.UTF8.GetString(cmd_buffer)))
                            {
                                case 101:
                                    //download++;
                                    break;
                                case 125:
                                {
                                    // Custom code
                                    receivingFileName = Encoding.UTF8.GetString(recv_data);
                                    onDataStartReceiving(receivingFileName);

                                    fs = new FileStream(@"" + SaveTo + receivingFileName, FileMode.CreateNew);
                                    byte[] data_to_send = CreateDataPacket(stream, Encoding.UTF8.GetBytes("126"),
                                        Encoding.UTF8.GetBytes(Convert.ToString(current_file_pointer)));
                                    stream.Write(data_to_send, 0, data_to_send.Length);
                                    stream.Flush();
                                }
                                    break;
                                case 127:
                                {
                                    fs.Seek(current_file_pointer, SeekOrigin.Begin);
                                    fs.Write(recv_data, 0, recv_data.Length);
                                    current_file_pointer = fs.Position;
                                    byte[] data_to_send = CreateDataPacket(stream, Encoding.UTF8.GetBytes("126"),
                                        Encoding.UTF8.GetBytes(Convert.ToString(current_file_pointer)));
                                    stream.Write(data_to_send, 0, data_to_send.Length);
                                    stream.Flush();
                                }
                                    break;
                                case 128:
                                {
                                    fs.Close();
                                    loop_break = true;
                                }
                                    break;
                                default:
                                    break;
                            }
                        }

                        if (loop_break == true)
                        {
                            stream.Close();
                            break;
                        }
                    }

                    onDataFinishReceiving(receivingFileName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                if (_ownsClient && _client != null)
                {
                    (_client as IDisposable).Dispose();
                    _client = null;
                }
            }
        }

        public byte[] ReadStream(NetworkStream ns)
        {
            byte[] data_buff = null;

            int b = 0;
            string buff_Length = "";

            while ((b = ns.ReadByte()) != 4)
            {
                buff_Length += (char)b;
            }

            int data_Length = Convert.ToInt32(buff_Length);
            data_buff = new byte[data_Length];
            int byte_Read = 0;
            int byte_Offset = 0;
            while (byte_Offset < data_Length)
            {
                byte_Read = ns.Read(data_buff, byte_Offset, data_Length - byte_Offset);
                byte_Offset += byte_Read;
            }

            return data_buff;
        }

        private byte[] CreateDataPacket(NetworkStream ns, byte[] cmd, byte[] data)
        {
            byte[] initialize = new byte[1];
            initialize[0] = 2;
            byte[] separator = new byte[1];
            separator[0] = 4;
            byte[] dataLength = Encoding.UTF8.GetBytes(Convert.ToString(data.Length));
            MemoryStream ms = new MemoryStream();
            ms.Write(initialize, 0, initialize.Length);
            ms.Write(cmd, 0, cmd.Length);
            ms.Write(dataLength, 0, dataLength.Length);
            ms.Write(separator, 0, separator.Length);
            ms.Write(data, 0, data.Length);

            return ms.ToArray();
        }
    }
}