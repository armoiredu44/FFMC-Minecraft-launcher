using System;
using System.ComponentModel;

public class UIManager : HttpUtility, INotifyPropertyChanged
{
    private string downloadSpeed;

    public string DownloadSpeedTextBlock
    {
        get { return downloadSpeed; }
        set
        {
            if (downloadSpeed != value)
            {
                downloadSpeed = value;
            }
        }
    }

}
