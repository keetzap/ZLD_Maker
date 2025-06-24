using UnityEngine;
using Color = UnityEngine.Color;

namespace Keetzap.Feedback
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class FeedbackEffectAttribute : System.Attribute
    {
        private readonly string _path;
        private readonly string _name;
        private readonly Color _color;
        private readonly bool _displayColorFullHeader;

        public FeedbackEffectAttribute(string path, float rColor = 1.0f, float gColor = 1.0f, float bColor = 1.0f, bool displayColorFullHeader = false)
        {
            _path = path;
            _name = path.Split('/')[^1];

            if (rColor > 1.0f || gColor > 1.0f || bColor > 1.0f)
            {
                Debug.LogError("Wrong Feedback effect color parameter. It only goes from 0.0f to 1.0f");
            }

            _color = new Color(rColor, gColor, bColor);
            _displayColorFullHeader = displayColorFullHeader;
        }

        public static string GetFeedbackDefaultName(System.Type type)
        {
            foreach (var obj in type.GetCustomAttributes(false))
            {
                if (obj is FeedbackEffectAttribute feedbackEffectAttribute)
                {
                    return feedbackEffectAttribute._name;
                }
            }

            return type.Name;
        }

        public static string GetFeedbackDefaultPath(System.Type type)
        {
            foreach (var obj in type.GetCustomAttributes(false))
            {
                if (obj is FeedbackEffectAttribute feedbackEffectAttribute)
                {
                    return feedbackEffectAttribute._path;
                }
            }

            return string.Empty;
        }

        public static Color GetFeedbackColor(System.Type type)
        {
            foreach (var obj in type.GetCustomAttributes(false))
            {
                if (obj is FeedbackEffectAttribute feedbackEffectAttribute)
                {
                    return feedbackEffectAttribute._color;
                }
            }

            return Color.white;
        }
        
        public static bool GetDisplayColorFullHeader(System.Type type)
        {
            foreach (var obj in type.GetCustomAttributes(false))
            {
                if (obj is FeedbackEffectAttribute feedbackEffectAttribute)
                {
                    return feedbackEffectAttribute._displayColorFullHeader;
                }
            }

            return false;
        }
    }
}