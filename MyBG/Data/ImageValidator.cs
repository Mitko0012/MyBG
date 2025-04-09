using System.IO;
using System.Drawing;

namespace MyBG.Data;

public static class ImageValidator
{
    public static bool IsImage(MemoryStream memoryStream)
    {
        bool isImage;
        if(memoryStream.CanSeek)
            memoryStream.Seek(0, SeekOrigin.Begin);
        try
        {
            using(Image img = Image.FromStream(memoryStream, false, true))
            {
                isImage = true;
            }
        }
        catch
        {
            isImage = false;
        }
        if(memoryStream.CanSeek)
            memoryStream.Seek(0, SeekOrigin.Begin);
        return isImage;
    }
}