using System.Diagnostics.CodeAnalysis;

namespace FSAWebSystem.Models.ViewModels
{
    public class BucketPlant
    {

        public string BannerName { get; set;}
        public string PlantCode { get; set;}
    }

    public class BucketPlantEqualityComparer : IEqualityComparer<BucketPlant>
    {
        public bool Equals(BucketPlant? x, BucketPlant? y)
        {
            return x.BannerName == y.BannerName;
        }

        public int GetHashCode([DisallowNull] BucketPlant obj)
        {
            unchecked
            {
                if (obj == null)
                    return 0;
                int hashCode = obj.BannerName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.BannerName.GetHashCode();
                return hashCode;
            }
        }
    }
}
