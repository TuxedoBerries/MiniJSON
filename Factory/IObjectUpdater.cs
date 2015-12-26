using System;

namespace TuxedoBerries.MiniJSON.Factories
{
	public interface IObjectUpdater
	{
		/// <summary>
		/// Updates a model with the given type and data.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		void UpdateModel(Type type, object model, object data);
	}
}

