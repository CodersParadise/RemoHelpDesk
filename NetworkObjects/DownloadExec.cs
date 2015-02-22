using System;

namespace NetworkObjects
{
    [Serializable]
    public class DownloadExec
    {
        public DownloadExec(String pURL)
        {
            this.url = pURL;

        }

        public String url { get; set; }

    }
}
