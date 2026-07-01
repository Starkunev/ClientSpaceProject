//using System;
//using System.Net.Sockets;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using System.Windows;
//using WpfApp1.Models;

//namespace WpfApp1
//{
//    public class ChatClient : IDisposable
//    {
//        private TcpClient _client;
//        private NetworkStream _stream;
//        private bool _disposed = false;
//        public event Action<Packet> OnPacketReceived;
//        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNamingPolicy = null,
//            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
//        };

//        public async Task<bool> ConnectAsync(string serverIp, int port)
//        {
//            try
//            {
//                _client = new TcpClient();
//                await _client.ConnectAsync(serverIp, port);
//                _stream = _client.GetStream();
//                _client.ReceiveTimeout = 10000;
//                _client.SendTimeout = 10000;
//                return true;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public async Task<AuthResponse> RegisterAsync(string name, string login, string password)
//        {
//            try
//            {
//                var request = new RegisterRequest
//                {
//                    Name = name,
//                    Login = login,
//                    Password = password
//                };

//                string requesJson = JsonSerializer.Serialize(request, _jsonOptions);
//                JsonDocument doc = JsonDocument.Parse(requesJson);
//                JsonElement dataElement = doc.RootElement.Clone();

//                var packet = new Packet
//                {
//                    Type = PacketType.Registration,
//                    Data = dataElement
//                };

//                await SendPacketAsync(packet);
//                return await ReceiveResponseAsync();
//            }
//            catch (Exception ex)
//            {
//                return new AuthResponse
//                {
//                    Success = false,
//                    Message = $"Ошибка: {ex.Message}"
//                };
//            }
//        }

//        public async Task<AuthResponse> LoginAsync(string login, string password)
//        {
//            try
//            {
//                var request = new LoginRequest
//                {
//                    Login = login,
//                    Password = password
//                };

//                string requesJson = JsonSerializer.Serialize(request, _jsonOptions);
//                JsonDocument doc = JsonDocument.Parse(requesJson);
//                JsonElement dataElement = doc.RootElement.Clone();

//                var packet = new Packet
//                {
//                    Type = PacketType.Login,
//                    Data = dataElement
//                };

//                await SendPacketAsync(packet);
//                return await ReceiveResponseAsync();
//            }
//            catch (Exception ex)
//            {
//                return new AuthResponse
//                {
//                    Success = false,
//                    Message = $"Ошибка: {ex.Message}"
//                };
//            }
//        }

//        private async Task SendPacketAsync(Packet packet)
//        {
//            if (_stream == null || !_client.Connected)
//                throw new InvalidOperationException("Нет подключения к серверу");

//            string json = JsonSerializer.Serialize(packet, _jsonOptions);
//            byte[] data = Encoding.UTF8.GetBytes(json);

//            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
//            await _stream.WriteAsync(lengthBytes, 0, 4);
//            await _stream.WriteAsync(data, 0, data.Length);
//            await _stream.FlushAsync();
//        }

//        private async Task<AuthResponse> ReceiveResponseAsync()
//        {
//            if (_stream == null || !_client.Connected)
//                throw new InvalidOperationException("Нет подключения к серверу");

//            try
//            {
//                byte[] lengthBuffer = new byte[4];
//                int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

//                if (bytesRead == 0)
//                {
//                    return new AuthResponse
//                    {
//                        Success = false,
//                        Message = "Сервер закрыл соединение"
//                    };
//                }

//                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

//                if (dataLength <= 0 || dataLength > 1048576)
//                {
//                    return new AuthResponse
//                    {
//                        Success = false,
//                        Message = "Некорректная длина данных"
//                    };
//                }

//                byte[] dataBuffer = new byte[dataLength];
//                int totalRead = 0;

//                while (totalRead < dataLength)
//                {
//                    int currentRead = await _stream.ReadAsync(dataBuffer, totalRead, dataLength - totalRead);
//                    if (currentRead == 0) break;
//                    totalRead += currentRead;
//                }

//                if (totalRead == 0)
//                {
//                    return new AuthResponse
//                    {
//                        Success = false,
//                        Message = "Не получены данные"
//                    };
//                }

//                string responseJson = Encoding.UTF8.GetString(dataBuffer, 0, totalRead);

//                JsonDocument doc = JsonDocument.Parse(responseJson);
//                JsonElement root = doc.RootElement;

//                AuthResponse response = new AuthResponse();

//                if (root.TryGetProperty("Data", out JsonElement data))
//                {
//                    if (data.TryGetProperty("BaseResponse", out JsonElement baseResponseElem))
//                    {
//                        if (baseResponseElem.TryGetProperty("Success", out JsonElement success))
//                            response.Success = success.GetBoolean();
//                        if (baseResponseElem.TryGetProperty("Message", out JsonElement message))
//                            response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
//                        if (data.TryGetProperty("ClientResponse", out JsonElement clientResponseElem))
//                        {
//                            if (clientResponseElem.TryGetProperty("Id", out JsonElement id))
//                                response.Id = Guid.Parse(id.GetString());
//                            if (clientResponseElem.TryGetProperty("Name", out JsonElement name))
//                                response.Name = name.GetString() ?? "";
//                            if (clientResponseElem.TryGetProperty("IsOnline", out JsonElement online))
//                                response.IsOnline = online.GetBoolean();
//                        }
//                    }
//                    else
//                    {
//                        if (data.TryGetProperty("Success", out JsonElement success))
//                            response.Success = success.GetBoolean();
//                        if (data.TryGetProperty("Message", out JsonElement message))
//                            response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
//                    }
//                }
//                else
//                {
//                    if (root.TryGetProperty("Success", out JsonElement success))
//                        response.Success = success.GetBoolean();
//                    if (root.TryGetProperty("Message", out JsonElement message))
//                        response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
//                    if (root.TryGetProperty("Id", out JsonElement id))
//                        response.Id = Guid.Parse(id.GetString());
//                    if (root.TryGetProperty("Name", out JsonElement name))
//                        response.Name = name.GetString() ?? "";
//                }

//                response.Message = response.Message ?? (response.Success ? "Успешно" : "Ошибка");
//                return response;
//            }
//            catch (Exception ex)
//            {
//                return new AuthResponse
//                {
//                    Success = false,
//                    Message = $"Ошибка: {ex.Message}"
//                };
//            }
//        }

//        private async Task<BaseResponse> ReceiveBaseResponseAsync()
//        {
//            if (_stream == null || !_client.Connected)
//                throw new InvalidOperationException("Нет подключения к серверу");

//            try
//            {
//                byte[] lengthBuffer = new byte[4];
//                int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

//                if (bytesRead == 0)
//                {
//                    return new BaseResponse
//                    {
//                        Success = false,
//                        Message = "Сервер закрыл соединение"
//                    };
//                }

//                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

//                if (dataLength <= 0 || dataLength > 1048576)
//                {
//                    return new BaseResponse
//                    {
//                        Success = false,
//                        Message = "Некорректная длина данных"
//                    };
//                }

//                byte[] dataBuffer = new byte[dataLength];
//                int totalRead = 0;

//                while (totalRead < dataLength)
//                {
//                    int currentRead = await _stream.ReadAsync(dataBuffer, totalRead, dataLength - totalRead);
//                    if (currentRead == 0) break;
//                    totalRead += currentRead;
//                }

//                if (totalRead == 0)
//                {
//                    return new BaseResponse
//                    {
//                        Success = false,
//                        Message = "Не получены данные"
//                    };
//                }

//                string responseJson = Encoding.UTF8.GetString(dataBuffer, 0, totalRead);

//                // Парсим ответ
//                JsonDocument doc = JsonDocument.Parse(responseJson);
//                JsonElement root = doc.RootElement;

//                var response = new BaseResponse();

//                if (root.TryGetProperty("Data", out JsonElement data))
//                {
//                    if (data.TryGetProperty("Success", out JsonElement success))
//                        response.Success = success.GetBoolean();
//                    if (data.TryGetProperty("Message", out JsonElement message))
//                        response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
//                }
//                else if (root.TryGetProperty("Success", out JsonElement success))
//                {
//                    response.Success = success.GetBoolean();
//                    if (root.TryGetProperty("Message", out JsonElement message))
//                        response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
//                }

//                return response;
//            }
//            catch (Exception ex)
//            {
//                return new BaseResponse
//                {
//                    Success = false,
//                    Message = $"Ошибка: {ex.Message}"
//                };
//            }
//        }
//        //public async Task<BaseResponse> ChangePasswordAsync(string newPassword) {
//        //    try
//        //    {
//        //        var request = new ChangePasswordRequest
//        //        {
//        //            Password = newPassword
//        //        };

//        //        string requestJson = JsonSerializer.Serialize(request, _jsonOptions);
//        //        JsonDocument doc = JsonDocument.Parse(requestJson);
//        //        JsonElement dataElement = doc.RootElement.Clone();

//        //        var packet = new Packet
//        //        {
//        //            Type = PacketType.ChangeClientPassword,
//        //            Data = dataElement
//        //        };
//        //        await SendPacketAsync(packet);
//        //        return await ReceiveBaseResponseAsync();
//        //    }
//        //    catch (Exception ex) {
//        //        return new BaseResponse
//        //        {
//        //            Success = false,
//        //            Message = $"Ошибка: {ex.Message}"
//        //        };
//        //    }

//        //}


//        public async Task<BaseResponse> ChangePasswordAsync(string newPassword)
//        {
//            try
//            {
//                var request = new ChangePasswordRequest
//                {
//                    Password = newPassword
//                };

//                var packet = new Packet
//                {
//                    Type = PacketType.ChangeClientPassword,
//                    Data = JsonSerializer.SerializeToElement(request, _jsonOptions)
//                };

//                System.Diagnostics.Debug.WriteLine($"=== Отправка ChangePassword запроса ===");
//                System.Diagnostics.Debug.WriteLine($"Data: {packet.Data.GetRawText()}");

//                await SendPacketAsync(packet);

//                // Используем TaskCompletionSource для ожидания ответа
//                var tcs = new TaskCompletionSource<BaseResponse>();

//                // Временный обработчик для ожидания ответа
//                void Handler(Packet p)
//                {
//                    if (p.Type == PacketType.ClientPasswordChanged)
//                    {
//                        try
//                        {
//                            var response = p.Data.Deserialize<BaseResponse>();
//                            tcs.TrySetResult(response ?? new BaseResponse { Success = false, Message = "Пустой ответ" });
//                        }
//                        catch (Exception ex)
//                        {
//                            tcs.TrySetResult(new BaseResponse { Success = false, Message = $"Ошибка парсинга: {ex.Message}" });
//                        }
//                    }
//                }

//                // Подписываемся на событие (используем this, а не _chatClient)
//                this.OnPacketReceived += Handler;

//                try
//                {
//                    // Ждем ответ с таймаутом
//                    var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(5000));
//                    if (completedTask != tcs.Task)
//                    {
//                        return new BaseResponse
//                        {
//                            Success = false,
//                            Message = "Таймаут ожидания ответа от сервера"
//                        };
//                    }

//                    return await tcs.Task;
//                }
//                finally
//                {
//                    this.OnPacketReceived -= Handler;
//                }
//            }
//            catch (Exception ex)
//            {
//                return new BaseResponse
//                {
//                    Success = false,
//                    Message = $"Ошибка: {ex.Message}"
//                };
//            }
//        }

//        //-------------------------- Методы для отправки(удаления) и получения сообщений -----------------------------------------

//        public async Task SendMessageAsync(Guid ClientId, string text)
//        {
//            var request = new MessageRequest
//            {
//                FromClientId = ClientId,
//                Text = text
//            };

//            string json = JsonSerializer.Serialize(request, _jsonOptions);
//            JsonDocument doc = JsonDocument.Parse(json);

//            Packet packet = new Packet
//            {
//                Type = PacketType.SendMessage,
//                Data = doc.RootElement.Clone()
//            };

//            await SendPacketAsync(packet);
//        }

//        public async Task GetAllMessages()
//        {
//            var packet = new Packet
//            {
//                Type = PacketType.GetAllMessages,
//                Data = JsonSerializer.SerializeToElement(new { })
//            };
//            await SendPacketAsync(packet);
//        }

//        public async Task DeleteMessageAsync(Guid messageId)
//        {
//            var packet = new Packet
//            {
//                Type = PacketType.DeleteMessage,
//                Data = JsonSerializer.SerializeToElement(messageId, _jsonOptions)
//            };

//            await SendPacketAsync(packet);
//        }
//        public async Task<Packet> ReceivePacketAsync()
//        {
//            if (_stream == null || !_client.Connected)
//                return null;

//            byte[] lengthBuffer = new byte[4];

//            int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

//            if (bytesRead == 0)
//                return new Packet();

//            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);



//            byte[] dataBuffer = new byte[dataLength];

//            int totalRead = 0;

//            while (totalRead < dataLength)
//            {
//                int currentRead = await _stream.ReadAsync(
//                    dataBuffer,
//                    totalRead,
//                    dataLength - totalRead);

//                if (currentRead == 0)
//                    return new Packet();

//                totalRead += currentRead;
//            }

//            string json = Encoding.UTF8.GetString(dataBuffer);

//            if (string.IsNullOrWhiteSpace(json))
//                return new Packet();

//            try
//            {
//                var packet = JsonSerializer.Deserialize<Packet>(json, _jsonOptions);

//                return packet ?? new Packet();
//            }
//            catch (Exception ex)
//            {
//                System.Diagnostics.Debug.WriteLine($"Ошибка десериализации: {ex.Message}");
//                return new Packet();
//            }
//        }

//        public async Task GetAllClientsAsync()
//        {

//            JsonDocument doc = JsonDocument.Parse("{}");

//            var packet = new Packet
//            {
//                Type = PacketType.GetAllClients,
//                Data = doc.RootElement.Clone()
//            };

//            await SendPacketAsync(packet);
//        }



//        public async Task UpdateClientAsync(UpdateClientRequest request)
//        {

//            System.Diagnostics.Debug.WriteLine($"=== ChatClient.UpdateClientAsync ===");
//            System.Diagnostics.Debug.WriteLine($"Name: '{request.Name}'");
//            System.Diagnostics.Debug.WriteLine($"Login: '{request.Login}'");
//            System.Diagnostics.Debug.WriteLine($"Password: '{request.PasswordHash}'");
//            System.Diagnostics.Debug.WriteLine($"Password length: {request.PasswordHash?.Length ?? 0}");
//            System.Diagnostics.Debug.WriteLine($"Password null: {request.PasswordHash == null}");
//            System.Diagnostics.Debug.WriteLine($"Password empty: {string.IsNullOrEmpty(request.PasswordHash)}");
//            System.Diagnostics.Debug.WriteLine($"Avatar: {(request.Avatar == null ? "null" : $"{request.Avatar.Length} bytes")}");


//            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
//            System.Diagnostics.Debug.WriteLine($"Request JSON: {requestJson}");

//            var packet = new Packet
//            {
//                Type = PacketType.UpdateClient,
//                Data = JsonSerializer.SerializeToElement(request, _jsonOptions)
//            };


//            System.Diagnostics.Debug.WriteLine($"Data JSON: {packet.Data.GetRawText()}");


//            var fullJson = JsonSerializer.Serialize(packet, _jsonOptions);
//            System.Diagnostics.Debug.WriteLine($"Full packet JSON (первые 500 символов):");
//            System.Diagnostics.Debug.WriteLine(fullJson.Substring(0, Math.Min(500, fullJson.Length)));

//            await SendPacketAsync(packet);
//        }

//        public bool IsConnected()
//        {
//            if (_client == null)
//                return false;

//            if (!_client.Connected)
//                return false;

//            if (_client.Client.Poll(0, SelectMode.SelectRead))
//            {
//                byte[] buff = new byte[1];
//                if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
//                    return false;
//            }

//            return true;
//        }

//        public void Disconnect()
//        {
//            try
//            {
//                _stream?.Close();
//                _client?.Close();
//            }
//            catch
//            {

//            }
//        }

//        public void Dispose()
//        {
//            if (!_disposed)
//            {
//                Disconnect();
//                _stream?.Dispose();
//                _client?.Dispose();
//                _disposed = true;
//            }
//        }
//    }
//}


using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1
{
    public class ChatClient : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _disposed = false;
        public event Action<Packet> OnPacketReceived;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public async Task<bool> ConnectAsync(string serverIp, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(serverIp, port);
                _stream = _client.GetStream();
                _client.ReceiveTimeout = 10000;
                _client.SendTimeout = 10000;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AuthResponse> RegisterAsync(string name, string login, string password)
        {
            try
            {
                var request = new RegisterRequest
                {
                    Name = name,
                    Login = login,
                    Password = password
                };

                string requesJson = JsonSerializer.Serialize(request, _jsonOptions);
                JsonDocument doc = JsonDocument.Parse(requesJson);
                JsonElement dataElement = doc.RootElement.Clone();

                var packet = new Packet
                {
                    Type = PacketType.Registration,
                    Data = dataElement
                };

                await SendPacketAsync(packet);
                return await ReceiveResponseAsync();
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(string login, string password)
        {
            try
            {
                var request = new LoginRequest
                {
                    Login = login,
                    Password = password
                };

                string requesJson = JsonSerializer.Serialize(request, _jsonOptions);
                JsonDocument doc = JsonDocument.Parse(requesJson);
                JsonElement dataElement = doc.RootElement.Clone();

                var packet = new Packet
                {
                    Type = PacketType.Login,
                    Data = dataElement
                };

                await SendPacketAsync(packet);
                return await ReceiveResponseAsync();
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        private async Task SendPacketAsync(Packet packet)
        {
            if (_stream == null || !_client.Connected)
                throw new InvalidOperationException("Нет подключения к серверу");

            string json = JsonSerializer.Serialize(packet, _jsonOptions);
            byte[] data = Encoding.UTF8.GetBytes(json);

            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            await _stream.WriteAsync(lengthBytes, 0, 4);
            await _stream.WriteAsync(data, 0, data.Length);
            await _stream.FlushAsync();
        }

        private async Task<AuthResponse> ReceiveResponseAsync()
        {
            if (_stream == null || !_client.Connected)
                throw new InvalidOperationException("Нет подключения к серверу");

            try
            {
                byte[] lengthBuffer = new byte[4];
                int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

                if (bytesRead == 0)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Сервер закрыл соединение"
                    };
                }

                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                if (dataLength <= 0 || dataLength > 1048576)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Некорректная длина данных"
                    };
                }

                byte[] dataBuffer = new byte[dataLength];
                int totalRead = 0;

                while (totalRead < dataLength)
                {
                    int currentRead = await _stream.ReadAsync(dataBuffer, totalRead, dataLength - totalRead);
                    if (currentRead == 0) break;
                    totalRead += currentRead;
                }

                if (totalRead == 0)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Не получены данные"
                    };
                }

                string responseJson = Encoding.UTF8.GetString(dataBuffer, 0, totalRead);

                JsonDocument doc = JsonDocument.Parse(responseJson);
                JsonElement root = doc.RootElement;

                AuthResponse response = new AuthResponse();

                if (root.TryGetProperty("Data", out JsonElement data))
                {
                    if (data.TryGetProperty("BaseResponse", out JsonElement baseResponseElem))
                    {
                        if (baseResponseElem.TryGetProperty("Success", out JsonElement success))
                            response.Success = success.GetBoolean();
                        if (baseResponseElem.TryGetProperty("Message", out JsonElement message))
                            response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
                        if (data.TryGetProperty("ClientResponse", out JsonElement clientResponseElem))
                        {
                            if (clientResponseElem.TryGetProperty("Id", out JsonElement id))
                                response.Id = Guid.Parse(id.GetString());
                            if (clientResponseElem.TryGetProperty("Name", out JsonElement name))
                                response.Name = name.GetString() ?? "";
                            if (clientResponseElem.TryGetProperty("IsOnline", out JsonElement online))
                                response.IsOnline = online.GetBoolean();
                        }
                    }
                    else
                    {
                        if (data.TryGetProperty("Success", out JsonElement success))
                            response.Success = success.GetBoolean();
                        if (data.TryGetProperty("Message", out JsonElement message))
                            response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
                    }
                }
                else
                {
                    if (root.TryGetProperty("Success", out JsonElement success))
                        response.Success = success.GetBoolean();
                    if (root.TryGetProperty("Message", out JsonElement message))
                        response.Message = message.GetString() ?? (response.Success ? "Успешно" : "Ошибка");
                    if (root.TryGetProperty("Id", out JsonElement id))
                        response.Id = Guid.Parse(id.GetString());
                    if (root.TryGetProperty("Name", out JsonElement name))
                        response.Name = name.GetString() ?? "";
                }

                response.Message = response.Message ?? (response.Success ? "Успешно" : "Ошибка");
                return response;
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        public async Task<BaseResponse> ChangePasswordAsync(string newPassword)
        {
            try
            {
                var request = new ChangePasswordRequest
                {
                    Password = newPassword
                };

                var packet = new Packet
                {
                    Type = PacketType.ChangeClientPassword,
                    Data = JsonSerializer.SerializeToElement(request, _jsonOptions)
                };

                System.Diagnostics.Debug.WriteLine($"=== Отправка ChangePassword запроса ===");
                System.Diagnostics.Debug.WriteLine($"Data: {packet.Data.GetRawText()}");

                await SendPacketAsync(packet);

                // Используем TaskCompletionSource для ожидания ответа
                var tcs = new TaskCompletionSource<BaseResponse>();

                // Временный обработчик для ожидания ответа
                void Handler(Packet p)
                {
                    if (p.Type == PacketType.ClientPasswordChanged)
                    {
                        try
                        {
                            var response = p.Data.Deserialize<BaseResponse>();
                            tcs.TrySetResult(response ?? new BaseResponse { Success = false, Message = "Пустой ответ" });
                            System.Diagnostics.Debug.WriteLine("=== Обработчик сработал! ===");
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetResult(new BaseResponse { Success = false, Message = $"Ошибка парсинга: {ex.Message}" });
                            System.Diagnostics.Debug.WriteLine($"=== Ошибка в обработчике: {ex.Message} ===");
                        }
                    }
                }

                // Подписываемся на событие
                this.OnPacketReceived += Handler;

                try
                {
                    System.Diagnostics.Debug.WriteLine("=== Ожидание ответа от сервера ===");

                    // Ждем ответ с таймаутом
                    var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(10000));
                    if (completedTask != tcs.Task)
                    {
                        System.Diagnostics.Debug.WriteLine("=== Таймаут! ===");
                        return new BaseResponse
                        {
                            Success = false,
                            Message = "Таймаут ожидания ответа от сервера"
                        };
                    }

                    var result = await tcs.Task;
                    System.Diagnostics.Debug.WriteLine($"=== Ответ получен: Success={result.Success}, Message={result.Message} ===");
                    return result;
                }
                finally
                {
                    this.OnPacketReceived -= Handler;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"=== Ошибка в ChangePasswordAsync: {ex.Message} ===");
                return new BaseResponse
                {
                    Success = false,
                    Message = $"Ошибка: {ex.Message}"
                };
            }
        }

        //-------------------------- Методы для отправки(удаления) и получения сообщений -----------------------------------------

        public async Task SendMessageAsync(Guid ClientId, string text)
        {
            var request = new MessageRequest
            {
                FromClientId = ClientId,
                Text = text
            };

            string json = JsonSerializer.Serialize(request, _jsonOptions);
            JsonDocument doc = JsonDocument.Parse(json);

            Packet packet = new Packet
            {
                Type = PacketType.SendMessage,
                Data = doc.RootElement.Clone()
            };

            await SendPacketAsync(packet);
        }

        public async Task GetAllMessages()
        {
            var packet = new Packet
            {
                Type = PacketType.GetAllMessages,
                Data = JsonSerializer.SerializeToElement(new { })
            };
            await SendPacketAsync(packet);
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            var packet = new Packet
            {
                Type = PacketType.DeleteMessage,
                Data = JsonSerializer.SerializeToElement(messageId, _jsonOptions)
            };

            await SendPacketAsync(packet);
        }

        public async Task<Packet> ReceivePacketAsync()
        {
            if (_stream == null || !_client.Connected)
                return null;

            try
            {
                byte[] lengthBuffer = new byte[4];
                int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

                if (bytesRead == 0)
                    return new Packet();

                int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

                if (dataLength <= 0 || dataLength > 1048576)
                {
                    System.Diagnostics.Debug.WriteLine($"Некорректная длина данных: {dataLength}");
                    return new Packet();
                }

                byte[] dataBuffer = new byte[dataLength];
                int totalRead = 0;

                while (totalRead < dataLength)
                {
                    int currentRead = await _stream.ReadAsync(
                        dataBuffer,
                        totalRead,
                        dataLength - totalRead);

                    if (currentRead == 0)
                        return new Packet();

                    totalRead += currentRead;
                }

                string json = Encoding.UTF8.GetString(dataBuffer);

                if (string.IsNullOrWhiteSpace(json))
                    return new Packet();

                try
                {
                    var packet = JsonSerializer.Deserialize<Packet>(json, _jsonOptions);
                    return packet ?? new Packet();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка десериализации: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"JSON: {json}");
                    return new Packet();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка в ReceivePacketAsync: {ex.Message}");
                return new Packet();
            }
        }

        public async Task GetAllClientsAsync()
        {
            JsonDocument doc = JsonDocument.Parse("{}");

            var packet = new Packet
            {
                Type = PacketType.GetAllClients,
                Data = doc.RootElement.Clone()
            };

            await SendPacketAsync(packet);
        }

        public void RaisePacketReceived(Packet packet)
        {
            OnPacketReceived?.Invoke(packet);
        }

        public async Task UpdateClientAsync(UpdateClientRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"=== ChatClient.UpdateClientAsync ===");
            System.Diagnostics.Debug.WriteLine($"Name: '{request.Name}'");
            System.Diagnostics.Debug.WriteLine($"Login: '{request.Login}'");
            System.Diagnostics.Debug.WriteLine($"Password: '{request.PasswordHash}'");
            System.Diagnostics.Debug.WriteLine($"Password length: {request.PasswordHash?.Length ?? 0}");
            System.Diagnostics.Debug.WriteLine($"Password null: {request.PasswordHash == null}");
            System.Diagnostics.Debug.WriteLine($"Password empty: {string.IsNullOrEmpty(request.PasswordHash)}");
            System.Diagnostics.Debug.WriteLine($"Avatar: {(request.Avatar == null ? "null" : $"{request.Avatar.Length} bytes")}");

            var requestJson = JsonSerializer.Serialize(request, _jsonOptions);
            System.Diagnostics.Debug.WriteLine($"Request JSON: {requestJson}");

            var packet = new Packet
            {
                Type = PacketType.UpdateClient,
                Data = JsonSerializer.SerializeToElement(request, _jsonOptions)
            };

            System.Diagnostics.Debug.WriteLine($"Data JSON: {packet.Data.GetRawText()}");

            var fullJson = JsonSerializer.Serialize(packet, _jsonOptions);
            System.Diagnostics.Debug.WriteLine($"Full packet JSON (первые 500 символов):");
            System.Diagnostics.Debug.WriteLine(fullJson.Substring(0, Math.Min(500, fullJson.Length)));

            await SendPacketAsync(packet);
        }

        public bool IsConnected()
        {
            if (_client == null)
                return false;

            if (!_client.Connected)
                return false;

            if (_client.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] buff = new byte[1];
                if (_client.Client.Receive(buff, SocketFlags.Peek) == 0)
                    return false;
            }

            return true;
        }

        public void Disconnect()
        {
            try
            {
                _stream?.Close();
                _client?.Close();
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _stream?.Dispose();
                _client?.Dispose();
                _disposed = true;
            }
        }
    }
}