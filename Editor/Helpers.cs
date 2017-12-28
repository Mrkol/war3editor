using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using OpenTK;

namespace Editor
{
    static class Helpers
    {
    	private static string TabStr(int depth)
    	{
    		return new string(' ', depth*2);
    	}

		public static string ToString<T>(this T o, int depth = 0)
		{
			if (depth > 100 || object.Equals(o, default(T))) return "";


			Type type = Nullable.GetUnderlyingType(o.GetType()) ?? o.GetType();
			dynamic obj = Convert.ChangeType(o, type);

			string result = "";

			if (ToStringAsIs.Contains(type)) 
			{
				result = obj.ToString();
			}
			else if (type.IsArray)
			{
				result += '[';
				if (obj.Length > 0)
				{
					result += '\n';
					foreach (object el in obj)
					{
						if (el == null) continue;
						string str = el.ToString(depth + 1);
						result += TabStr(depth + 1) + str + '\n';
					}
					result += TabStr(depth);
				}
				result += ']';
			}
			else
			{
				result += type.ToString() + '\n';

				FieldInfo[] fields = type.GetFields();

				if (fields.Length > 0)
				{
		            result += TabStr(depth) + '{' + '\n';
					foreach (FieldInfo field in fields)
					{
						object val = field.GetValue(obj);
						if (val == null) continue;

						result += string.Format("{0}  {1} = {2}\n",
							TabStr(depth),
							field.Name,
							val.ToString(depth + 1));
					}
		            result += TabStr(depth) + '}';
				}
			}
            

            return result;
		}

		public static string ToString(this Vector3 vec)
		{
			return string.Format("({0}, {1}, {2})", vec.X, vec.Y, vec.Z);
		}

		public static string ToString(this IEnumerable list, int depth = 0)
		{
			string tabstr = new string(' ', depth*2);

			string result = tabstr + "[\n";

			foreach (object o in list)
			{
				result += o.ToString(depth + 1) + "\n";
			}

			result += tabstr + "]\n";

			return result;
		}

		public static Type[] ToStringAsIs = 
			{typeof(byte), typeof(sbyte),
			 typeof(short), typeof(ushort),
			 typeof(int), typeof(uint),
			 typeof(long), typeof(ulong),
			 typeof(float), typeof(double),
			 typeof(string), typeof(bool),
			 typeof(Vector2), typeof(Vector3), typeof(Vector4),
			 typeof(Vector2d), typeof(Vector3d), typeof(Vector4d),
			 typeof(Vector2h), typeof(Vector3h), typeof(Vector4h)};
    }
}