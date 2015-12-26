using System;

namespace TuxedoBerries.MiniJSON.Factories
{
	public interface IModelUpdater
	{
		/// <summary>
		/// Updates a model with the given data.
		/// </summary>
		/// <param name="model">Model.</param>
		/// <param name="data">Data.</param>
		void UpdateModel<M>(M model, object data) where M : class;
	}
}

