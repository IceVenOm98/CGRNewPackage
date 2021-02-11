namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Web;
    using System.Web.SessionState;
    using Newtonsoft.Json;
	using Terrasoft.Configuration.SearchDuplicatesService;
	using Terrasoft.Core;
	using Terrasoft.Core.Factories;
	using Terrasoft.Web.Common;

	#region Class: DeduplicationService

	///<summary>Represent class for deduplication service.</summary>
	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
#if NETSTANDARD2_0
	public class DeduplicationService : BaseService
#else
	public class DeduplicationService : BaseService, IReadOnlySessionState
#endif
	{

		/// <summary>
		/// Web-method provide result of search for similar records.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <param name="request">Request data.</param>
		/// <returns>List of GUIDs of double records was found.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public List<Guid> FindDuplicatesOnSave(string schemaName, SingleRequest request)
		{
			DeduplicationProcessing deduplicationProcessing =
				ClassFactory.Get<DeduplicationProcessing>(new ConstructorArgument("userConnection", UserConnection));
			List<Guid> result = deduplicationProcessing.FindDuplicates(schemaName, request);
			return result;
		}

		/// <summary>
		/// Web-method provide excluding records from deduplication process.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <param name="notDuplicateList">The list of records that will be excluded from deduplication process.</param>
		/// <param name="request">Request data.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void SetDuplicatesOnSave(string schemaName, List<string> notDuplicateList, SingleRequest request)
		{
			if (notDuplicateList.Any())
			{
				notDuplicateList.Add(request.Id.ToString());
				AddToIgnoreList(schemaName, notDuplicateList);
			}
		}

		/// <summary>
		/// Web-method provide result of deduplication.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <param name="columns">Requested columns.</param>
		/// <param name="offset">Start position for groups.</param>
		/// <returns>Escaped JSON string contains group of duplicates.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "GetDeduplicationResults", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public string GetDeduplicationResults(string schemaName, string[] columns, int offset)
		{
			DeduplicationProcessing deduplicationProcessing =
				ClassFactory.Get<DeduplicationProcessing>(new ConstructorArgument("userConnection", UserConnection));
			DuplicatesGroupResponse objectResult =
				deduplicationProcessing.GetDeduplicationResults(schemaName, columns, offset);
			string result = JsonConvert.SerializeObject(objectResult);
			return result;
		}

		/// <summary>
		/// Web-method invokes duplicates search.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <returns>Response of search request.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "FindEntityDuplicatesAsync", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public DuplicatesSearchResponse FindEntityDuplicatesAsync(string schemaName)
		{
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationSearch deduplicationSearch = ClassFactory.Get<DeduplicationSearch>(args);
			DuplicatesSearchResponse response = deduplicationSearch.FindEntityDuplicatesAsync(schemaName);
			return response;
		}

		/// <summary>
		/// Web-method invokes duplicates merge.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <param name="groupId">Identifier of the group of search results.</param>
		/// <param name="deduplicateRecordIds">Unique identifiers.</param>
		/// <param name="mergeConfig">JSON serialized merge config.</param>
		/// <returns>Response of search request.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "MergeEntityDuplicatesAsync",
			BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		public DuplicatesMergeResponse MergeEntityDuplicatesAsync(string schemaName, int groupId, List<Guid> deduplicateRecordIds,
				string mergeConfig)
		{
			Dictionary<string, string> config = null;
			if (!string.IsNullOrEmpty(mergeConfig))
			{
				config = JsonConvert.DeserializeObject<Dictionary<string, string>>(mergeConfig);
			}
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationProcessing deduplicationProcessing = ClassFactory.Get<DeduplicationProcessing>(args);
			DuplicatesMergeResponse response =
				deduplicationProcessing.MergeEntityDuplicatesAsync(schemaName, groupId, deduplicateRecordIds, config);
			return response;
		}

		/// <summary>
		/// Web-method moves records group to ignore list.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <param name="deduplicateRecordIds">Unique identifiers.</param>
		/// <returns>Response of search request.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "AddToIgnoreList",
			BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json,
			ResponseFormat = WebMessageFormat.Json)]
		public void AddToIgnoreList(string schemaName, List<string> deduplicateRecordIds)
		{
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationProcessing deduplicationProcessing = ClassFactory.Get<DeduplicationProcessing>(args);
			deduplicationProcessing.AddToIgnoreList(schemaName, deduplicateRecordIds);
		}

		/// <summary>
		/// Web-method returns info about last search session execution.
		/// </summary>
		/// <param name="schemaName">Schema name.</param>
		/// <returns>Response of search request.</returns>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "GetSearchInfo", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public DuplicatesSearchResponse GetSearchInfo(string schemaName)
		{
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationSearch deduplicationSearch = ClassFactory.Get<DeduplicationSearch>(args);
			DuplicatesSearchResponse response = deduplicationSearch.GetInfo(schemaName);
			return response;
		}

		/// <summary>
		/// Web-method schedule duplicates search.
		/// </summary>
		/// <param name="cronExpression">Cron expression.</param>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "ScheduleSearch", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void ScheduleSearch(string cronExpression)
		{
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationSearch deduplicationSearch = ClassFactory.Get<DeduplicationSearch>(args);
			deduplicationSearch.FindEntityDuplicatesBySchedule(cronExpression);
		}

		/// <summary>
		/// Web-method removes search schedule.
		/// </summary>
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "RemoveScheduledSearch", BodyStyle = WebMessageBodyStyle.Wrapped,
			RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
		public void RemoveScheduledSearch()
		{
			var args = new ConstructorArgument[] {
				new ConstructorArgument("userConnection", UserConnection)
			};
			DeduplicationSearch deduplicationSearch = ClassFactory.Get<DeduplicationSearch>(args);
			deduplicationSearch.RemoveSchedule();
		}

	}

	#endregion

}
