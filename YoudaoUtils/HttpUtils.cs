

namespace YoudaoNoteUtils
{
    using SocketEx;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;

    public static class HttpUtils
    {
        /// <summary>
        /// 向指定地址发送一个 GET 请求。
        /// </summary>
        /// <param name="url">要请求的地址。</param>
        /// <returns>请求返回的结果。</returns>
        public static RequestResult<string> SendGetRequest(string url)
        {
            var requestResult = new RequestResult<string>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        requestResult.OperateResult = OperateResultEnum.Success;
                        requestResult.Code = httpWebResponse.StatusCode;
                        requestResult.Result = streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                using (var errorResponseStream = e.Response.GetResponseStream())
                {
                    using (var errorStreamReader = new StreamReader(errorResponseStream, Encoding.UTF8))
                    {
                        requestResult.OperateResult = OperateResultEnum.Fail;
            
                        var httpWebResponse = e.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                            requestResult.Code = httpWebResponse.StatusCode;
                        requestResult.ErrorMsg = errorStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e1)
            {
                requestResult.OperateResult = OperateResultEnum.Fail;
                requestResult.ErrorMsg = e1.ToString();
            }

            return requestResult;
        }

        public static RequestResult<Resource> SendGetRequestWithReturnStream(string url)
        {
            var result = new RequestResult<Resource>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                
                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (null == httpWebResponse)
                    {
                        var resp = GetResponseWithTcpClient(httpWebRequest.RequestUri.PathAndQuery);
                        result.OperateResult = OperateResultEnum.Success;
                        result.Code = resp.StatusCode;
                        result.Result = new Resource(url, resp.ResponseByte);
                    }
                    else
                    {
                        using (var responseStream = httpWebResponse.GetResponseStream())
                        {
                            var buffer = new byte[1024];
                            int bytesRead;
                            var ms = new MemoryStream();

                            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                ms.Write(buffer, 0, bytesRead);
                            }

                            ms.Position = 0;
                            var tempBuffer = new byte[ms.Length];
                            ms.Read(tempBuffer, 0, tempBuffer.Length);
                            ms.Close();

                            result.OperateResult = OperateResultEnum.Success;
                            result.Code = httpWebResponse.StatusCode;
                            var res = new Resource(url, tempBuffer);
                            result.Result = res;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (var errorResponseStream = e.Response.GetResponseStream())
                {
                    using (var errorStreamReader = new StreamReader(errorResponseStream, Encoding.UTF8))
                    {
                        result.OperateResult = OperateResultEnum.Fail;
                        var httpWebResponse = e.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                            result.Code = (httpWebResponse.StatusCode);
                        result.ErrorMsg = errorStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e1)
            {
                result.OperateResult = OperateResultEnum.Fail;
                result.ErrorMsg = e1.ToString();
            }

            return result;
        }

        /// <summary>
        /// 向指定地址发送一个 POST 请求。
        /// </summary>
        /// <param name="url">要请求的地址。</param>
        /// <param name="postData">请求发送的数据，默认为空字符串。</param>
        /// <returns>请求返回的结果。</returns>
        public static RequestResult<string> SendPostRequest(string url, PostData postData = null)
        {
            var result = new RequestResult<string>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                var byteRequest = new byte[0];
                if (null != postData)
                {
                    byteRequest = Encoding.UTF8.GetBytes(postData.GetPostData());
                }

                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = byteRequest.Length;

                var stream = httpWebRequest.GetRequestStream();
                stream.Write(byteRequest, 0, byteRequest.Length);
                stream.Close();

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    if (null == httpWebResponse)
                    {
                        result.OperateResult = OperateResultEnum.Fail;
                        result.ErrorMsg = "httpWebResponse 为空";
                        return result;
                    }
                    using (var responseStream = httpWebResponse.GetResponseStream())
                    {
                        using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            result.OperateResult = OperateResultEnum.Success;
                            result.Code = httpWebResponse.StatusCode;
                            result.Result = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException e)
            {
                using (var errorResponseStream = e.Response.GetResponseStream())
                {
                    using (var errorStreamReader = new StreamReader(errorResponseStream, Encoding.UTF8))
                    {
                        result.OperateResult = OperateResultEnum.Fail;
                        var httpWebResponse = e.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                            result.Code = httpWebResponse.StatusCode;
                        result.ErrorMsg = errorStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e1)
            {
                result.OperateResult = OperateResultEnum.Fail;
                result.ErrorMsg = e1.ToString();
            }

            return result;
        }

        public static RequestResult<string> SendPostRequestWithMultipart(string url, string accessToken, PostData postData)
        {
            var result = new RequestResult<string>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                var boundary = String.Format("----------{0:N}", Guid.NewGuid());

                httpWebRequest.ContentType = "multipart/form-data;boundary=" + boundary;
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers["Authorization"] = "OAuth oauth_token=\"" + accessToken + "\"";
                var byteRequest = postData.GetPostDataWithMultipart(boundary);
                httpWebRequest.ContentLength = byteRequest.Length;

                httpWebRequest.AllowReadStreamBuffering = true;

                var stream = httpWebRequest.GetRequestStream();
                stream.Write(byteRequest, 0, byteRequest.Length);
                stream.Close();

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var responseStream = httpWebResponse.GetResponseStream())
                {
                    using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result.OperateResult = OperateResultEnum.Success;
                        result.Code = httpWebResponse.StatusCode;
                        result.Result = streamReader.ReadToEnd();
                    }
                }
            }
            catch (WebException e)
            {
                using (var errorResponseStream = e.Response.GetResponseStream())
                {
                    using (var errorStreamReader = new StreamReader(errorResponseStream, Encoding.UTF8))
                    {
                        result.OperateResult = OperateResultEnum.Fail;
                        var httpWebResponse = e.Response as HttpWebResponse;
                        if (httpWebResponse != null)
                            result.Code = httpWebResponse.StatusCode;
                        result.ErrorMsg = errorStreamReader.ReadToEnd();
                    }
                }
            }
            catch (Exception e1)
            {
                result.OperateResult = OperateResultEnum.Fail;
                result.ErrorMsg = e1.ToString();
            }

            return result;
        }

        private static TcpClientResponse GetResponseWithTcpClient(string pathAndQuery)
        {
            var connection = CreateConnection();
            var stream = connection.GetStream();

            var writer = new StreamWriter(stream);
            var request = "GET " + pathAndQuery + " HTTP/1.1\r\nHost: " + ServerAddress
                + "\r\nConnection: close\r\n\r\n";

            writer.WriteLine(request);
            writer.Flush();

            return stream.GetTcpClientResponse();
        }

        private static SecureTcpClient CreateConnection()
        {
            var connection = new SecureTcpClient(ServerAddress, ServerPort);

            return connection;
        }
        const string ServerAddress = "note.youdao.com";
        const int ServerPort = 443;
    }
}
