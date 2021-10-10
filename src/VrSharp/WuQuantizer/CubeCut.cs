namespace PuyoTools.Core.Textures.Quantizers.Wu
{
    internal struct CubeCut
    {
        public readonly byte? Position;
        public readonly float Value;

        public CubeCut(byte? cutPoint, float result)
        {
            Position = cutPoint;
            Value = result;
        }
    }
}