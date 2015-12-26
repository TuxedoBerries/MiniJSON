using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using TuxedoBerries.MiniJSON.Serialization;

namespace TuxedoBerries.MiniJSON.Factories.Reflection
{
	public class JSONObjectFactory : IModelFactory, IObjectFactory, IModelUpdater, IObjectUpdater
	{
		private JSONSpecsFactory _specsFactory;

		public JSONObjectFactory ()
		{
			_specsFactory = new JSONSpecsFactory ();
		}

		/// <summary>
		/// Creates a model based on an Object.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="data">Data.</param>
		public M CreateModel <M>(object data) where M : class, new()
		{
			var model = CreateModel (typeof(M), data);
			return model as M;
		}

		/// <summary>
		/// Creates a model based on given type and filled with the given data.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="type">Type.</param>
		/// <param name="data">Data.</param>
		public object CreateModel (Type type, object data)
		{
			if (!CheckDataAsObject (data))
				return null;

			var model = Activator.CreateInstance (type);
			SetModelData (model, data, _specsFactory.GetSpecs(type));
			return model;
		}

		/// <summary>
		/// Updates a model with the given data.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		public void UpdateModel<M>(M model, object data) where M : class
		{
			UpdateModel (typeof(M), model, data);
		}

		/// <summary>
		/// Updates a model with the given type and data.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		public void UpdateModel(Type type, object model, object data)
		{
			if (!CheckDataAsObject (data))
				return;

			SetModelData (model, data, _specsFactory.GetSpecs(type));
		}

		/// <summary>
		/// Checks the data as object.
		/// </summary>
		/// <returns><c>true</c>, if data as object was checked, <c>false</c> otherwise.</returns>
		/// <param name="data">Data.</param>
		private bool CheckDataAsObject(object data)
		{
			if (!JSONHelper.IsJSONObject (data)) {
				UnityEngine.Debug.LogWarning ("Given data is not an Object");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Sets the model data.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		private void SetModelData(object model, object data, IEnumerator<JSONFieldSpecs> typeSpecs)
		{
			var dict = JSONHelper.AsObject(data);
			while (typeSpecs.MoveNext()) {
				var specs = typeSpecs.Current;
				if (dict.ContainsKey (specs.Name)) {
					SetData (model, dict [specs.Name], specs);
					continue;
				}
				if (!specs.ShortNameAvailable)
					continue;

				if (dict.ContainsKey (specs.ShortName)) {
					SetData (model, dict [specs.ShortName], specs);
				}
			}
		}

		/// <summary>
		/// Sets the data to the given model
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		/// <param name="specs">Specs.</param>
		private void SetData(object model, object data, JSONFieldSpecs specs)
		{
			switch (specs.MemberType) {
				case JSONMemberType.Value:
					SetValueData (model, data, specs);
					break;
				case JSONMemberType.Object:
					SetObjectData (model, data, specs);
					break;
				case JSONMemberType.Array:
					SetArrayData (model, data, specs);
					break;
				case JSONMemberType.List:
					SetListData (model, data, specs);
					break;
				case JSONMemberType.Dictionary:
					SetDictionaryData (model, data, specs);
					break;
			}
		}

		private void SetValueData(object model, object data, JSONFieldSpecs specs)
		{
			var convertedData = Convert.ChangeType (data, specs.Property.PropertyType);
			specs.Property.SetValue (model, convertedData, null);
		}

		private void SetObjectData(object model, object data, JSONFieldSpecs specs)
		{
			var nestedModel = CreateModel(specs.Property.PropertyType, data);
			specs.Property.SetValue (model, nestedModel, null);
		}

		private void SetArrayData(object model, object data, JSONFieldSpecs specs)
		{
			if (!JSONHelper.IsJSONArray (data)) {
				UnityEngine.Debug.LogWarningFormat ("Data [{0}] is not an array", specs.Name);
				return;
			}

			var arrayData = JSONHelper.AsArray (data);
			var elementType = specs.Property.PropertyType.GetElementType ();
			var propertyData = Array.CreateInstance (elementType, arrayData.Count);
			for (int i = 0; i < arrayData.Count; ++i) {
				var createdModel = CreateModel(elementType, arrayData[i]);
				propertyData.SetValue (createdModel, i);
			}
			specs.Property.SetValue (model, propertyData, null);
		}

		private void SetListData(object model, object data, JSONFieldSpecs specs)
		{
			if (!JSONHelper.IsJSONArray (data)) {
				UnityEngine.Debug.LogWarningFormat ("Data [{0}] is not an array", specs.Name);
				return;
			}

			var arrayData = JSONHelper.AsArray (data);
			var elementType = specs.Property.PropertyType.GetGenericArguments ()[0];
			var propertyData = Activator.CreateInstance (specs.Property.PropertyType) as IList;

			for (int i = 0; i < arrayData.Count; ++i) {
				var createdModel = CreateModel(elementType, arrayData[i]);
				propertyData.Add (createdModel);
			}
			specs.Property.SetValue (model, propertyData, null);
		}

		private void SetDictionaryData(object model, object data, JSONFieldSpecs specs)
		{
			if (!JSONHelper.IsJSONObject (data)) {
				UnityEngine.Debug.LogWarningFormat ("Data [{0}] is not an objec", specs.Name);
				return;
			}

			var objectData = JSONHelper.AsObject (data);
			var elementType = specs.Property.PropertyType.GetGenericArguments ();
			var propertyData = Activator.CreateInstance (specs.Property.PropertyType) as IDictionary;

			foreach(var element in objectData) {
				var createdModel = CreateModel(elementType[1], element.Value);
				propertyData.Add (element.Key, createdModel);
			}
			specs.Property.SetValue (model, propertyData, null);
		}
	}
}

