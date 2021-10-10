using System.Drawing;

namespace PuyoTools.Core.Textures.Quantizers.Wu
{
    public interface IWuQuantizer
    {
        Image QuantizeImage(Bitmap image, int alphaThreshold, int alphaFader);
    }
}