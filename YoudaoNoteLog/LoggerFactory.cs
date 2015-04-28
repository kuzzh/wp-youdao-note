

namespace YoudaoNoteLog
{

    public static class LoggerFactory
    {
        private static readonly ILog Logger = new Log();

        public static ILog GetLogger()
        {
            return Logger;
        }
    }
}
