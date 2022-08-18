using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System;

public static class Utils
{
    public static T SetFlag<T>(this T en, T flag) where T : struct, IConvertible
    {
        int value = en.ToInt32(CultureInfo.InvariantCulture);
        int newFlag = flag.ToInt32(CultureInfo.InvariantCulture);

        return (T)(object)(value | newFlag);
    }

    public static T UnsetFlag<T>(this T en, T flag) where T : struct, IConvertible
    {
        int value = en.ToInt32(CultureInfo.InvariantCulture);
        int newFlag = flag.ToInt32(CultureInfo.InvariantCulture);

        return (T)(object)(value & ~newFlag);
    }


    public static void SetFlag<T>(this T existingCollection, T flag)
    {
        int value = existingCollection.(CultureInfo.InvariantCulture);
    }


    public static bool HasFlag(this Enum variable, Enum value)
    {
        // check if from the same type.
        if (variable.GetType() != value.GetType())
        {
            throw new ArgumentException("The checked flag is not from the same type as the checked variable.");
        }
 
        ulong num = Convert.ToUInt64(value);
        ulong num2 = Convert.ToUInt64(variable);
 
        return (num2 & num) == num;
    }
}
