using System.Reflection;
using System.Threading.Tasks;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace SoundFader.utils
{
    internal static class ManagerHelper
    {
        private static readonly string BLANK_IMAGE = @"data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg""/>";
        private static readonly FieldInfo FldProxy = typeof(ConnectionManager).GetField("_proxy", BindingFlags.Instance | BindingFlags.NonPublic);

        public static async Task SetImageFromDataUriAsync(this ConnectionManager manager, string context, string dataUrl)
        {
            IStreamDeckProxy proxy = (IStreamDeckProxy)FldProxy.GetValue(manager);

            SetImageArgs args = new()
            {
                context = context,
                payload = new SetImageArgs.Payload
                {
                    TargetType = SetTitleArgs.TargetType.HardwareAndSoftware,
                    image = dataUrl
                }
            };
            await proxy.SendStreamDeckEvent(args);
        }

        public static async Task SetImageToBlankAsync(this ConnectionManager connectionManager, string context)
        {
            await connectionManager.SetImageFromDataUriAsync(context, BLANK_IMAGE);
        }
    }
}
