namespace VrtNuDownloader.Core.Service.Logging
{
    public interface ILoggingService
    {
        void WriteLog(MessageType type, string message);
    }

    public enum MessageType
    {
        Info,
        Error,
        Warn
    }
}
