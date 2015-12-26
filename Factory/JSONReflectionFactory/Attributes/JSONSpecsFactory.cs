using System;
using System.Reflection;
using System.Collections.Generic;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	public class JSONSpecsFactory
	{
		private Dictionary<Type, List<JSONFieldSpecs>> _specsDictionary;

		public JSONSpecsFactory ()
		{
			_specsDictionary = new Dictionary<Type, List<JSONFieldSpecs>> ();
		}

		/// <summary>
		/// Gets the specs for a JSON representation.
		/// </summary>
		/// <returns>The specs.</returns>
		/// <param name="type">Type.</param>
		public IEnumerator<JSONFieldSpecs> GetSpecs(Type type)
		{
			if (!_specsDictionary.ContainsKey (type)) {
				_specsDictionary.Add (type, GetFullSpecs (type));
			}
			return _specsDictionary [type].GetEnumerator();
		}

		/// <summary>
		/// Gets the full JSON specs.
		/// </summary>
		private List<JSONFieldSpecs> GetFullSpecs(Type type)
		{
			List<JSONFieldSpecs> list = new List<JSONFieldSpecs> ();
			var properties = type.GetProperties ();
			foreach (var prop in properties) {
				JSONBaseAttribute specs;
				// Check Fields
				specs = JSONFieldAttribute.GetJSONSpecs (prop);
				if (specs != null) {
					AddSpecs (list, prop, specs);
					continue;
				}

				// Check Object
				specs = JSONObjectAttribute.GetJSONSpecs (prop);
				if (specs != null) {
					AddSpecs (list, prop, specs);
					continue;
				}

				// Check Array
				specs = JSONArrayAttribute.GetJSONSpecs (prop);
				if (specs != null) {
					AddSpecs (list, prop, specs);
					continue;
				}

				// Check List
				specs = JSONListAttribute.GetJSONSpecs (prop);
				if (specs != null) {
					AddSpecs (list, prop, specs);
					continue;
				}

				// Check Dictionary
				specs = JSONDictionaryAttribute.GetJSONSpecs (prop);
				if (specs != null) {
					AddSpecs (list, prop, specs);
					continue;
				}
			}
			return list;
		}

		/// <summary>
		/// Adds the specs.
		/// </summary>
		/// <param name="list">List.</param>
		/// <param name="prop">Property.</param>
		/// <param name="attribute">Attribute.</param>
		private void AddSpecs(List<JSONFieldSpecs> list, PropertyInfo prop, JSONBaseAttribute attribute)
		{
			if (!prop.CanWrite) {
				UnityEngine.Debug.LogWarningFormat ("Property [{0}] does not have write access", prop.Name);
				return;
			}

			var specsData = attribute.Specs;
			specsData.Property = prop;
			list.Add (specsData);
		}
	}
}

