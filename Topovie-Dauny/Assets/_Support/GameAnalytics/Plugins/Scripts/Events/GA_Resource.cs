using System.Collections.Generic;
using _Support.GameAnalytics.Plugins.Scripts.Wrapper;

namespace _Support.GameAnalytics.Plugins.Scripts.Events
{
	public static class GA_Resource
	{
		#region public methods

		public static void NewEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId, IDictionary<string, object> fields, bool mergeFields)
		{
			GA_Wrapper.AddResourceEvent(flowType, currency, amount, itemType, itemId, fields, mergeFields);
		}

		#endregion
	}
}