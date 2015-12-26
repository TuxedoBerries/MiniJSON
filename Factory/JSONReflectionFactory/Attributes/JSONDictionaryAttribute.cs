using System;
using System.Reflection;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	[AttributeUsage(AttributeTargets.Property)]
	public class JSONDictionaryAttribute : JSONBaseAttribute
	{
		public JSONDictionaryAttribute (string name)
		{
			_specs = new JSONFieldSpecs (name, JSONMemberType.Dictionary);
		}

		public JSONDictionaryAttribute (string name, string shortName)
		{
			_specs = new JSONFieldSpecs (name, shortName, JSONMemberType.Dictionary);
		}

		public static JSONDictionaryAttribute GetJSONSpecs(PropertyInfo info)
		{
			var objects = info.GetCustomAttributes (typeof(JSONDictionaryAttribute), true);
			if (objects == null || objects.Length <= 0)
				return null;

			return objects[0] as JSONDictionaryAttribute;
		}
	}
}

