using System;
using System.Text;
using System.Reflection;

namespace tw.ccnet.core.util
{
	public class ReflectionUtil
	{
		/// <summary>
		/// Utility class is not intended for instantiation.
		/// </summary>
		private ReflectionUtil() { }

		/// <summary>
		/// Gets a value indicating whether the types, fields and properties of the
		/// specified objects are equal.
		/// </summary>
		/// <param name="o1"></param>
		/// <param name="o2"></param>
		/// <returns></returns>
		public static bool ReflectionEquals(object o1, object o2)
		{
			// verify fields
			return ValidateTypes(o1, o2) && ValidateFields(o1, o2) && ValidateProperties(o1, o2);
		}

		#region Private helper methods

		private static bool ValidateTypes(object o1, object o2)
		{
			return o2 != null && o1.GetType() == o2.GetType();
		}

		private static bool ValidateFields(object o1, object o2)
		{
			foreach (FieldInfo field in o1.GetType().GetFields())
			{
				if (field.FieldType.IsArray)
				{
					return ValidateArrays((object[])field.GetValue(o1), (object[])field.GetValue(o2));
				}
				if (IsNotEqual(field.GetValue(o1), field.GetValue(o2)))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ValidateProperties(object o1, object o2)
		{
			foreach (PropertyInfo property in o1.GetType().GetProperties())
			{
				if (property.PropertyType.IsArray)
				{
					return ValidateArrays((object[])property.GetValue(o1, null), (object[])property.GetValue(o2, null));
				}
				if (IsNotEqual(property.GetValue(o1, null), property.GetValue(o2, null)))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ValidateArrays(object[] o1, object[] o2)
		{
			if (o1 == null && o2 == null) return true;
			if (o1 == null || o2 == null) return false;
			if (o1.Length != o2.Length) return false;

			for (int i = 0; i < o1.Length; i++)
			{
				if (IsNotEqual(o1[i], o2[i])) return false;
			}
			return true;
		}

		private static bool IsNotEqual(Object o1, Object o2) 
		{
			return ! IsEqual(o1, o2);
		}

		private static bool IsEqual(Object o1, Object o2) 
		{
			return (o1 == null) ? o2 == null : o1.Equals(o2);
		}

		#endregion

		/// <summary>
		/// Uses reflection to compile a string representation of an object, by
		/// querying its type, field names/values, and property names/values.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ReflectionToString(object obj)
		{
			Type type = obj.GetType();
			StringBuilder buffer = new StringBuilder(type.Name);
			buffer.Append(": (");

			int count = 0;
			foreach (FieldInfo info in type.GetFields())
			{
				if (count++ > 0) buffer.Append(",");
				buffer.Append(info.Name).Append("=");
				buffer.Append(info.GetValue(obj));
			}

			foreach (PropertyInfo info in type.GetProperties())
			{
				if (count++ > 0) buffer.Append(",");
				buffer.Append(info.Name).Append("=");
				buffer.Append(info.GetValue(obj, null));
			}
			buffer.Append(")");
			return buffer.ToString();
		}
	}
}
