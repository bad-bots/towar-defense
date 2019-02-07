using System;
using System.Globalization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class RangedFloat
{
    public float minValue;
    public float maxValue;

    public RangedFloat(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }
}

public class MinMaxRangeAttribute : PropertyAttribute
{
    public readonly float min;
    public readonly float max;

    public MinMaxRangeAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RangedFloat))]
[CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
public class RangedFloatPropertyDrawer : PropertyDrawer
{
    const int labelWidth = 50;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
    {
        var limits = attribute != null ? attribute as MinMaxRangeAttribute : new MinMaxRangeAttribute(0, 1);

        EditorGUI.BeginProperty(pos, GUIContent.none, property);
        SerializedProperty minProp = property.FindPropertyRelative("minValue");
        SerializedProperty maxProp = property.FindPropertyRelative("maxValue");
        float minValue = minProp.floatValue;
        float maxValue = maxProp.floatValue;

        pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

        // EditorGUI.LabelField(
        //   new Rect(pos.x, pos.y, labelWidth, pos.height),
        //   new GUIContent(minValue.ToString("N2"))
        // );
        minValue = DrawRoundedFloatField(new Rect(pos.x, pos.y, labelWidth, pos.height), minValue);
        EditorGUI.MinMaxSlider(
          new Rect(pos.x + labelWidth, pos.y, pos.width - labelWidth * 2, pos.height),
          ref minValue, ref maxValue, limits.min, limits.max);

        maxValue = DrawRoundedFloatField(
          new Rect(pos.x + pos.width - labelWidth, pos.y, labelWidth, pos.height),
          maxValue);

        minProp.floatValue = minValue;
        maxProp.floatValue = maxValue;

        EditorGUI.EndProperty();
    }

    private float DrawRoundedFloatField(Rect pos, float value)
    {
        string floatString = value.ToString("F2");
        string input = EditorGUI.TextField(pos, floatString);
        if (input != floatString)
        {
            float inputFloat;
            bool valid = float.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out inputFloat);
            if (valid)
                value = inputFloat;
        }

        return value;
    }
}
#endif
