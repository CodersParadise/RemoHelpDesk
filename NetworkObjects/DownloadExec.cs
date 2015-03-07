namespace NetworkObjects
{
    using System;

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
