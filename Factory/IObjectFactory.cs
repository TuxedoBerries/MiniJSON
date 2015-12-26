using System;

namespace TuxedoBerries.MiniJSON.Factories
{
	public interface IObjectFactory
	{
		/// <summary>
		/// Creates a model based on given type and filled with the given data.
		/// </summary>
		/// <returns>The model.</returns>
		/// <param name="type">Type.</param>
		/// <param name="data">Data.</param>
		object CreateModel (Type type, object data);
	}
}

