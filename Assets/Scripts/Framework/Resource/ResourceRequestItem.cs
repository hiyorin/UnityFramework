namespace Framework.Resource
{
    public enum ResourceType
    {
        Asset,
        AssetBundle,
        Texture,
    }

    public class ResourceRequestItem : BaseResourceRequest
    {
        public readonly string url;
        public readonly ResourceType type;

        public ResourceRequestItem (string url, ResourceType type)
        {
            this.url = url;
            this.type = type;
        }
    }
}