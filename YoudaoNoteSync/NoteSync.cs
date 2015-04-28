

namespace YoudaoNoteSync
{
    /// <summary>
    /// 待同步的笔记。
    /// </summary>
    public class NoteSync
    {
        public string NotePath { get; set; }
        public string NotebookName { get; set; }
        public string NotebookPath { get; set; }


        public NoteSync(string notePath, string notebookName, string notebookPath)
        {
            NotePath = notePath;
            NotebookName = notebookName;
            NotebookPath = notebookPath;
        }
    }
}
