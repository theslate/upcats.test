using ImageMagick;

namespace UpCataas
{
    public interface IImageTransformer
    {
        IMagickImage FlipUpsideDown(IMagickImage image);
    }
}