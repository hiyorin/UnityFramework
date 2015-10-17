using UnityEngine;

public class FilterFileExtensionAttribute : PropertyAttribute
{
    public string fileExtension = string.Empty;

    public FilterFileExtensionAttribute (string fileExtension)
    {
        this.fileExtension = fileExtension;
    }
}
