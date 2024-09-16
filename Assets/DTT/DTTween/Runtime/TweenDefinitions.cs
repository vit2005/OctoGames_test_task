using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace DTT.Tweening
{
    public static class TweenDefinitions
    {
        public delegate float TweenFunction(float x);

        public static readonly ReadOnlyDictionary<Easing, TweenFunction> TweenMap =
            new ReadOnlyDictionary<Easing, TweenFunction>(new Dictionary<Easing, TweenFunction>()
            {
                { Easing.EASE_IN_SINE, EaseInSine },
                { Easing.EASE_OUT_SINE, EaseOutSine },
                { Easing.EASE_IN_OUT_SINE, EaseInOutSine },
                
                { Easing.EASE_IN_CUBIC, EaseInCubic },
                { Easing.EASE_OUT_CUBIC, EaseOutCubic },
                { Easing.EASE_IN_OUT_CUBIC, EaseInOutCubic },
                
                { Easing.EASE_IN_QUINT, EaseInQuint },
                { Easing.EASE_OUT_QUINT, EaseOutQuint },
                { Easing.EASE_IN_OUT_QUINT, EaseInOutQuint },
                
                { Easing.EASE_IN_CIRC, EaseInCirc },
                { Easing.EASE_OUT_CIRC, EaseOutCirc },
                { Easing.EASE_IN_OUT_CIRC, EaseInOutCirc },
                
                { Easing.EASE_IN_ELASTIC, EaseInElastic },
                { Easing.EASE_OUT_ELASTIC, EaseOutElastic },
                { Easing.EASE_IN_OUT_ELASTIC, EaseInOutElastic },
                
                { Easing.EASE_IN_QUAD, EaseInQuad },
                { Easing.EASE_OUT_QUAD, EaseOutQuad },
                { Easing.EASE_IN_OUT_QUAD, EaseInOutQuad },
                
                { Easing.EASE_IN_QUART, EaseInQuart },
                { Easing.EASE_OUT_QUART, EaseOutQuart },
                { Easing.EASE_IN_OUT_QUART, EaseInOutQuart },
                
                { Easing.EASE_IN_EXPO, EaseInExpo },
                { Easing.EASE_OUT_EXPO, EaseOutExpo },
                { Easing.EASE_IN_OUT_EXPO, EaseInOutExpo },
                
                { Easing.EASE_IN_BACK, EaseInBack },
                { Easing.EASE_OUT_BACK, EaseOutBack },
                { Easing.EASE_IN_OUT_BACK, EaseInOutBack },
                
                { Easing.EASE_IN_BOUNCE, EaseInBounce },
                { Easing.EASE_OUT_BOUNCE, EaseOutBounce },
                { Easing.EASE_IN_OUT_BOUNCE, EaseInOutBounce },
            });

        public static float EaseInSine(float x) => 1 - Mathf.Cos((x * Mathf.PI) / 2);

        public static float EaseOutSine(float x) => Mathf.Sin((x * Mathf.PI) / 2);

        public static float EaseInOutSine(float x) => -(Mathf.Cos(Mathf.PI * x) - 1) / 2;

        public static float EaseInCubic(float x) => x * x * x;

        public static float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

        public static float EaseInOutCubic(float x) => x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;

        public static float EaseInQuint(float x) => x * x * x * x * x;

        public static float EaseOutQuint(float x) => 1 - Mathf.Pow(1 - x, 5);

        public static float EaseInOutQuint(float x) => x < 0.5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;

        public static float EaseInCirc(float x) => 1 - Mathf.Sqrt(1 - Mathf.Pow(x, 2));

        public static float EaseOutCirc(float x) => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));

        public static float EaseInOutCirc(float x) =>
            x < 0.5
                ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
                : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;

        public static float EaseInElastic(float x) 
        {
            const float C4 = (2 * Mathf.PI) / 3;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * C4);
        }
        
        public static float EaseOutElastic(float x) 
        {
            const float C4 = (2 * Mathf.PI) / 3;

            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * C4) + 1;
        }
        
        public static float EaseInOutElastic(float x) 
        {
            const float c5 = (2 * Mathf.PI) / 4.5f;

            return x == 0
                ? 0 : x == 1
                    ? 1
                    : x < 0.5
                        ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
                        : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
        }
        
        public static float EaseInQuad(float x) => x * x;

        public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

        public static float EaseInOutQuad(float x) => x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;

        public static float EaseInQuart(float x) => x * x * x * x;

        public static float EaseOutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);

        public static float EaseInOutQuart(float x) => x < 0.5 ? 8 * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 4) / 2;

        public static float EaseInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);

        public static float EaseOutExpo(float x) => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);

        public static float EaseInOutExpo(float x) 
        {
            return x == 0
                ? 0
                : x == 1
                    ? 1
                    : x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2
                        : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
        }
        
        public static float EaseInBack(float x) {
            const float C1 = 1.70158f;
            const float C3 = C1 + 1f;

            return C3 * x * x * x - C1 * x * x;
        }
        
        public static float EaseOutBack(float x) {
            const float C1 = 1.70158f;
            const float C3 = C1 + 1f;

            return 1 + C3 * Mathf.Pow(x - 1, 3) + C1 * Mathf.Pow(x - 1, 2);
        }
        
        public static float EaseInOutBack(float x) {
            const float C1 = 1.70158f;
            const float C2 = C1 * 1.525f;

            return x < 0.5
                ? (Mathf.Pow(2 * x, 2) * ((C2 + 1) * 2 * x - C2)) / 2
                : (Mathf.Pow(2 * x - 2, 2) * ((C2 + 1) * (x * 2 - 2) + C2) + 2) / 2;
        }
        
        public static float EaseInBounce(float x) => 1 - EaseOutBounce(1 - x);

        public static float EaseOutBounce(float x) {
            const float N1 = 7.5625f;
            const float D1 = 2.75f;

            if (x < 1 / D1)
                return N1 * x * x;
            if (x < 2 / D1)
                return N1 * (x -= 1.5f / D1) * x + 0.75f;
            if (x < 2.5 / D1)
                return N1 * (x -= 2.25f / D1) * x + 0.9375f;
            
            return N1 * (x -= 2.625f / D1) * x + 0.984375f;
        }
        
        public static float EaseInOutBounce(float x) =>
            x < 0.5
                ? (1 - EaseOutBounce(1 - 2 * x)) / 2
                : (1 + EaseOutBounce(2 * x - 1)) / 2;
    }
}