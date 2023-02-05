namespace Roots
{
    public class ResourceUtils
    {
        public static ResourceTypes GetResourceType(string ext)
        {
            switch (ext)
            {
                case ".unity": return ResourceTypes.Scene;
                default: return ResourceTypes.Prefab;
            }
        }
    }
}
