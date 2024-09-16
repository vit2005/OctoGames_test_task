using System;

namespace Naninovel.Spreadsheet
{
    public class ProgressChangedArgs : EventArgs
    {
        public readonly string Info;
        public readonly float Progress;

        public ProgressChangedArgs (string info, float progress)
        {
            Info = info;
            Progress = progress;
        }
    }
}
