
namespace AudioVisualizationLibrary.Enums
{
    public enum Directions
    {
        None,
        Left,
        Right,
    }
    public enum SignalShiftDirections
    {
        None,
        Left,
        Right,
        Random,
        RandomModular,
    }
    public enum VisualizerTypes : int
    {
        Bar,
        BarDouble,

        BarScalable,
        BarScalableDouble,

        /// <summary>
        /// Dairesel spectrum çizer
        /// </summary>
        RadialScalable,

        /// <summary>
        /// Dairesel spectrum çizer
        /// </summary>
        RadialDoubleScalable,

        /// <summary>
        /// Dairesel basit spectrum çizer
        /// </summary>
        Radial,

        /// <summary>
        /// Dairesel basit spectrum çizer
        /// </summary>
        RadialDouble,

        /// <summary>
        /// Dairesel dalga spectrum çizer
        /// </summary>
        RadialWave,

        /// <summary>
        /// Dairesel dalga spectrum çizer
        /// </summary>
        RadialWaveDouble,

        /// <summary>
        /// Dalga spectrum çizer
        /// </summary>
        Wave,

        /// <summary>
        /// Çokgen spectrum çizer
        /// </summary>
        Polygon,
        PolygonWave,
    }
    public enum TestModes
    {
        None,
        NormalVisualizers,
        RadialVisualizers,
        PolygonVisualizers,
        SignalProperties
    }
    public enum BackgroundTypes
    {
        Image,
        Video,
        Color
    }
    public enum MultiColorModes
    {
        None,
        Gradient,
        MultiColor,
    }
    public enum MultiSineModes
    {
        Absolute,
        Percentage,
        Random,
        MultiSineAbsolute,
        MultiSinePercentage,
        MultiSineRandom,
    }
    public enum SignalOutputMode
    {
        /// <summary>
        /// Normal mod
        /// </summary>
        Normal,

        /// <summary>
        /// Ortalama mod
        /// </summary>
        Average,

        /// <summary>
        /// Sabit sinyal modu
        /// </summary>
        Absolute,

        /// <summary>
        /// Modüler şekilde ilk sinyali normal ve normalin yarısı şeklinde işler
        /// </summary>
        AbsoluteHalf,

        /// <summary>
        /// Modüler şekilde sinyali normal ve normalin yarısı şeklinde işler
        /// </summary>
        ModularHalf,

        /// <summary>
        /// Modüler aritmetik mod
        /// </summary>
        Modular,

        InvertedAverage,
        
        InvertedAbsoluteHalf,

        InvertedModular,

        InvertedModularHalf,

        Random,

        /// <summary>
        /// Yalnızca maksimum veri içeren sektör gösterilir
        /// </summary>
        Max,

        /// <summary>
        /// Yalnızca minimum veri içeren sektör gösterilir
        /// </summary>
        Min,

        /// <summary>
        /// Yalnızca maksimum ve minimum veri içeren sektörler gösterilir.
        /// </summary>
        MaxAndMin,
    }
}
