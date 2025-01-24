using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace SoundFader
{
    internal class CustomSdProxy : IStreamDeckProxy, IDisposable
    {
        private readonly ClientWebSocket _Socket = new ClientWebSocket();

        private bool disposedValue = false;

        public WebSocketState State => _Socket.State;

        public Task ConnectAsync(Uri uri, CancellationToken token)
        {
            return _Socket.ConnectAsync(uri, token);
        }

        public Task Register(string registerEvent, string uuid)
        {
            return _Socket.SendAsync(GetPluginRegistrationBytes(registerEvent, uuid), WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
        }

        public Task SendStreamDeckEvent(BaseStreamDeckArgs args)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(args));
            return _Socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, endOfMessage: true, CancellationToken.None);
        }

        public async Task<string> GetMessageAsString(CancellationToken token)
        {
            // IMPROVED: Call ReceiveAsync() repeatedly to receive a long message over 64k
            var segment = new ArraySegment<byte>(new byte[4096]);
            var message = new StringBuilder();
            WebSocketReceiveResult result;
            do
            {
                result = await _Socket.ReceiveAsync(segment, token);
                message.Append(Encoding.UTF8.GetString(segment.Array, 0, result.Count));
            }
            while (!result.EndOfMessage);
            return message.ToString();
        }

        private ArraySegment<byte> GetPluginRegistrationBytes(string registerEvent, string uuid)
        {
            Info.PluginRegistration value = new Info.PluginRegistration
            {
                @event = registerEvent,
                uuid = uuid
            };
            string s = JsonConvert.SerializeObject(value);
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            return new ArraySegment<byte>(bytes);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                _Socket.Dispose();
                disposedValue = true;
            }
        }

        ~CustomSdProxy()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
