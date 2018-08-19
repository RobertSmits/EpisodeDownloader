namespace VrtNuDownloader.Service.Interface
{
    public interface ILogService
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
