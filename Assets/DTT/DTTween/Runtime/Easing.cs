using UnityEngine;

namespace DTT.Tweening
{
    /// <summary>
    /// Easings that can be used to easily select functions from <see cref="TweenDefinitions"/>.
    /// </summary>
    public enum Easing
    {
        [InspectorName("Ease In Sine")]
        EASE_IN_SINE = 0,
        [InspectorName("Ease Out Sine")]
        EASE_OUT_SINE = 1,
        [InspectorName("Ease In Out Sine")]
        EASE_IN_OUT_SINE = 2,
        
        [InspectorName("Ease In Cubic")]
        EASE_IN_CUBIC = 3,
        [InspectorName("Ease Out Cubic")]
        EASE_OUT_CUBIC = 4,
        [InspectorName("Ease In Out Cubic")]
        EASE_IN_OUT_CUBIC = 5,
        
        [InspectorName("Ease In Quint")]
        EASE_IN_QUINT = 6,
        [InspectorName("Ease Out Quint")]
        EASE_OUT_QUINT = 7,
        [InspectorName("Ease In Out Quint")]
        EASE_IN_OUT_QUINT = 8,
        
        [InspectorName("Ease In Circ")]
        EASE_IN_CIRC = 9,
        [InspectorName("Ease Out Circ")]
        EASE_OUT_CIRC = 10,
        [InspectorName("Ease In Out Circ")]
        EASE_IN_OUT_CIRC = 11,
        
        [InspectorName("Ease In Elastic")]
        EASE_IN_ELASTIC = 12,
        [InspectorName("Ease Out Elastic")]
        EASE_OUT_ELASTIC = 13,
        [InspectorName("Ease In Out Elastic")]
        EASE_IN_OUT_ELASTIC = 14,
        
        [InspectorName("Ease In Quad")]
        EASE_IN_QUAD = 15,
        [InspectorName("Ease Out Quad")]
        EASE_OUT_QUAD = 16,
        [InspectorName("Ease In Out Quad")]
        EASE_IN_OUT_QUAD = 17,
        
        [InspectorName("Ease In Quart")]
        EASE_IN_QUART = 18,
        [InspectorName("Ease Out Quart")]
        EASE_OUT_QUART = 19,
        [InspectorName("Ease In Out Quart")]
        EASE_IN_OUT_QUART = 20,
        
        [InspectorName("Ease In Expo")]
        EASE_IN_EXPO = 21,
        [InspectorName("Ease Out Expo")]
        EASE_OUT_EXPO = 22,
        [InspectorName("Ease In Out Expo")]
        EASE_IN_OUT_EXPO = 23,
        
        [InspectorName("Ease In Back")]
        EASE_IN_BACK = 24,
        [InspectorName("Ease Out Back")]
        EASE_OUT_BACK = 25,
        [InspectorName("Ease In Out Back")]
        EASE_IN_OUT_BACK = 26,
        
        [InspectorName("Ease In Bounce")]
        EASE_IN_BOUNCE = 27,
        [InspectorName("Ease Out Bounce")]
        EASE_OUT_BOUNCE = 28,
        [InspectorName("Ease In Out Bounce")]
        EASE_IN_OUT_BOUNCE = 29
    }
}