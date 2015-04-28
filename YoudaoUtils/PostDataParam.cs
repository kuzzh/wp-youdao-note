
namespace YoudaoNoteUtils
{
    public class PostDataParam
    {
        public PostDataParam(string name, object value, PostDataParamType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public PostDataParam(string name, object value, string filePath, PostDataParamType type)
        {
            Name = name;
            Value = value;
            FilePath = filePath;
            Type = type;
        }

        public readonly string Name;
        public readonly string FilePath;
        public readonly object Value;
        public readonly PostDataParamType Type;
    }
}
