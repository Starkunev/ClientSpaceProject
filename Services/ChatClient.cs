
//using System;
//using System.Net.Sockets;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;
//using WpfApp1.Models;
//namespace WpfApp1 
//{
//    //public class Packet
//    //{
//    //    public PacketType Type { get; set; }
//    //    public JsonElement Data { get; set; }
//    //}

//    //public class RegisterRequest
//    //{
//    //    public string Name { get; set; } = string.Empty;
//    //    public string Login { get; set; } = string.Empty;
//    //    public string Password { get; set; } = string.Empty;
//    //}


//    //public class BaseResponse
//    //{
//    //    public bool Success { get; set; }
//    //    public string Message { get; set; } = string.Empty;
//    //}

//    //public class LoginRequest
//    //{
//    //    public string Login { get; set; }
//    //    public string Password { get; set; }
//    //}

//    //public class AuthResponse
//    //{
//    //    public bool Success { get; set; }
//    //    public string Message { get; set; }
//    //    public int Id { get; set; }
//    //    public string Name { get; set; }
//    //    public bool IsOnline { get; set; }
//    //}

//    //public enum PacketType
//    //{
//    //    Registration,
//    //    Login,
//    //    SendMessage,
//    //    GetAllMessages
//    //}

//    public class ChatClient : IDisposable
//    {
//        private TcpClient _client;
//        private NetworkStream _stream;
//        private bool _disposed = false;
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

//                if (dataLength <= 0 || dataLength > 65536)
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
//                                response.Id = id.GetInt32();
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
//                        response.Id = id.GetInt32();
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




//        //-------------------------- -------Методы для отправки и получения сообщений -----------------------------------------

//        public async Task SendMessageAsync(int ClientId, string text) {
//            var request = new MessageRequest
//            {
//                FromClientId = ClientId,
//                Text = text
//            };

//            string json = JsonSerializer.Serialize(request,_jsonOptions);

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

//                Type = PacketType.GetAllMessages


//            };
//            await SendPacketAsync(packet);
//        }



//        public async Task UpdateClientAsync(UpdateClientRequest request)
//        {
//            string json = JsonSerializer.Serialize(request, _jsonOptions);
//            JsonDocument doc = JsonDocument.Parse(json);

//            Packet packet = new Packet
//            {
//                Type = PacketType.UpdateClient,
//                Data = doc.RootElement.Clone()
//            };

//            await SendPacketAsync(packet);

//            // Ждем ответа
//            var response = await ReceivePacketAsync();
//            // Обработка ответа...
//        }









//        public async Task<Packet> ReceivePacketAsync()
//        {
//            if (_stream == null || !_client.Connected)
//                return null;

//            byte[] lengthBuffer = new byte[4];

//            int bytesRead =
//                await _stream.ReadAsync(lengthBuffer, 0, 4);

//            if (bytesRead == 0)
//                return new Packet();

//            int dataLength =
//                BitConverter.ToInt32(lengthBuffer, 0);

//            byte[] dataBuffer = new byte[dataLength];

//            int totalRead = 0;

//            while (totalRead < dataLength)
//            {
//                int currentRead =
//                    await _stream.ReadAsync(
//                        dataBuffer,
//                        totalRead,
//                        dataLength - totalRead);

//                if (currentRead == 0)
//                    return null;

//                totalRead += currentRead;
//            }

//            string json =
//                Encoding.UTF8.GetString(dataBuffer);

//            return JsonSerializer.Deserialize<Packet>(
//                json,
//                _jsonOptions);
//        }
//        public async Task GetAllClientsAsync()
//        {
//            var packet = new Packet
//            {
//                Type = PacketType.GetAllClients
//            };
//            await SendPacketAsync(packet);
//        }

//        public async Task UpdateClientAsync(UpdateClientRequest request)
//        {
//            string json = JsonSerializer.Serialize(request, _jsonOptions);
//            JsonDocument doc = JsonDocument.Parse(json);

//            Packet packet = new Packet
//            {
//                Type = PacketType.UpdateClient,
//                Data = doc.RootElement.Clone()
//            };

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

                if (dataLength <= 0 || dataLength > 65536)
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

        //-------------------------- Методы для отправки и получения сообщений -----------------------------------------

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
                Type = PacketType.GetAllMessages
            };
            await SendPacketAsync(packet);
        }


        public async Task<Packet> ReceivePacketAsync()
        {
            if (_stream == null || !_client.Connected)
                return null;

            byte[] lengthBuffer = new byte[4];

            int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

            if (bytesRead == 0)
                return new Packet();

            int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

            //if (dataLength <= 0)
            //    return null;

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
                return new Packet();
            }
        }
        //public async Task<Packet> ReceivePacketAsync()
        //{
        //    if (_stream == null || !_client.Connected)
        //        return null;

        //    byte[] lengthBuffer = new byte[4];

        //    int bytesRead = await _stream.ReadAsync(lengthBuffer, 0, 4);

        //    if (bytesRead == 0)
        //        return new Packet();

        //    int dataLength = BitConverter.ToInt32(lengthBuffer, 0);

        //    byte[] dataBuffer = new byte[dataLength];

        //    int totalRead = 0;

        //    while (totalRead < dataLength)
        //    {
        //        int currentRead = await _stream.ReadAsync(dataBuffer, totalRead, dataLength - totalRead);

        //        if (currentRead == 0)
        //            return null;

        //        totalRead += currentRead;
        //    }

        //    string json = Encoding.UTF8.GetString(dataBuffer);

        //    return JsonSerializer.Deserialize<Packet>(json, _jsonOptions);
        //}

        public async Task GetAllClientsAsync()
        {
            // Создаем пустой JsonElement
            JsonDocument doc = JsonDocument.Parse("{}");

            var packet = new Packet
            {
                Type = PacketType.GetAllClients,
                Data = doc.RootElement.Clone()
            };

            await SendPacketAsync(packet);
        }

        // ЕДИНСТВЕННЫЙ МЕТОД UpdateClientAsync - УДАЛИТЕ ДУБЛИКАТ
        public async Task UpdateClientAsync(UpdateClientRequest request)
        {
            string json = JsonSerializer.Serialize(request, _jsonOptions);
            JsonDocument doc = JsonDocument.Parse(json);

            Packet packet = new Packet
            {
                Type = PacketType.UpdateClient,
                Data = doc.RootElement.Clone()
            };

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
                // Игнорируем ошибки при отключении
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