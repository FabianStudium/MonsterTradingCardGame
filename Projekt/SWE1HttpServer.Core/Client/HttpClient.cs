﻿using SWE1HttpServer.Core.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SWE1HttpServer.Core.Client
{
    class HttpClient : IClient
    {
        private readonly TcpClient connection;
        public HttpClient(TcpClient connection)
        {
            this.connection = connection;
        }

        public RequestContext ReceiveRequest()
        {
            var reader = new StreamReader(connection.GetStream());

            string line;
            string path = null;
            string version = null;
            var method = HttpMethod.Get;
            bool isFirstLine = true;
            var header = new Dictionary<string, string>();
            var contentLength = 0;
            string payload = null;

            try
            {
                while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                {
                    if (isFirstLine)
                    {
                        // read HTTP method, resource path, and HTTP version
                        var info = line.Split(' ');

                        method = MethodUtilities.GetMethod(info[0]);
                        path = info[1];
                        version = info[2];

                        isFirstLine = false;
                    }
                    else
                    {
                        // read HTTP header entries
                        var info = line.Split(":", 2);
                        header.Add(info[0].Trim(), info[1].Trim());
                        if (info[0] == "Content-Length")
                        {
                            contentLength = int.Parse(info[1]);
                        }
                    }
                }           
            }
            catch (IOException)
            {
                return null;
            }

            if (path == null || version == null)
            {
                return null;
            }

            // read HTTP body and check the payload
            if (contentLength > 0 && header.ContainsKey("Content-Type"))
            {
                var data = new StringBuilder(200);
                char[] buffer = new char[1024];
                int totalBytesRead = 0;
                while (totalBytesRead < contentLength)
                {
                    try
                    {
                        var bytesRead = reader.Read(buffer, 0, 1024);
                        totalBytesRead += bytesRead;
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        data.Append(buffer, 0, bytesRead);
                    }
                    catch (IOException)
                    {
                        return null;
                    }
                }
                payload = data.ToString();

                // TODO: maybe check the content type for the payload
            }

            return new RequestContext()
            {
                Method = method,
                ResourcePath = path,
                HttpVersion = version,
                Header = header,
                Payload = payload
            };
        }

        public void SendResponse(Response.Response response)
        {
            var writer = new StreamWriter(connection.GetStream()) { AutoFlush = true };
            writer.Write($"HTTP/1.1 {(int)response.StatusCode} {response.StatusCode}\r\n");
            
            if (!string.IsNullOrEmpty(response.Payload))
            {
                var payload = Encoding.UTF8.GetBytes(response.Payload);
                writer.Write($"Content-Length: {payload.Length}\r\n");
                writer.Write("\r\n");
                writer.Write(Encoding.UTF8.GetString(payload));
                writer.Close();
            }
            else 
            {
                writer.Write("\r\n");
                writer.Close();
            }
        }
    }
}
