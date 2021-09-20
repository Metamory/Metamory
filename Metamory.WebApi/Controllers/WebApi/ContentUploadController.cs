using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Metamory.Api;
using Metamory.WebApi.Models.WebApi.Content;
using Metamory.WebApi.Policies;
using Metamory.WebApi.Utils;
using Newtonsoft.Json.Linq;


namespace Metamory.WebApi.Controllers.WebApi
{
	//[RoutePrefix("api/content")]
	[StopwatchFilter]
	[EnableCors("CorsPolicy")]
	public class ContentUploadController : ControllerBase
	{
		private readonly ContentManagementService _contentManagementService;
		private readonly IAuthorizationPolicy _authPolicy;


		public ContentUploadController(ContentManagementService contentManagementService, IAuthorizationPolicy authPolicy)
		{
			_contentManagementService = contentManagementService;
			_authPolicy = authPolicy;
		}


		[HttpPost, Route("content/{siteId}/{contentId}")]
		public async Task<IActionResult> Post(string siteId, string contentId, HttpRequestMessage requestMessage)
		{
			if (!_authPolicy.AllowManageContent(siteId, contentId, User))
			{
				return new StatusCodeResult((int)(User.Identity.IsAuthenticated ? HttpStatusCode.Forbidden : HttpStatusCode.Unauthorized));
			}

			PostContentModel model;
			// if (this.Request.Content.IsMimeMultipartContent())
			// {
			// 	model = await GetPostContentModelFromMultiPartAsync(siteId, contentId, requestMessage);
			// }
			// else if (requestMessage.Content.IsFormData())
			if (this.Request.HasFormContentType)
			{
				model = await GetPostContentModelFromFormAsync(siteId, contentId);
			}
			else
			{
				model = await GetPostContentModelFromAjaxAsync(siteId, contentId);
			}

			if (model.ContentStream != null && model.ContentType != null)
			{
				var contentMetadata = await _contentManagementService.StoreAsync(siteId, contentId, DateTimeOffset.Now,
					model.ContentStream, model.ContentType, model.PreviousVersionId, model.Author, model.Label);
				return Ok(contentMetadata);
			}

			return new StatusCodeResult((int)HttpStatusCode.BadRequest);
		}

		private async Task<PostContentModel> GetPostContentModelFromAjaxAsync(string siteId, string contentId)
		{
			using (var sr = new StreamReader(this.Request.Body))
			{
				string jsonBodyString = await sr.ReadToEndAsync();
				var jsonBody = JObject.Parse(jsonBodyString);

				Func<string, string> GetValue = key =>
				{
					var val = jsonBody[key];
					return val != null ? val.ToString() : null;
				};

				var model = new PostContentModel()
				{
					Author = GetValue("author"),
					Label = GetValue("label"),
					PreviousVersionId = GetValue("previousVersionId"),
					ContentType = GetValue("contentType"),
					ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(GetValue("content")))
				};

				return model;
			};
		}

		private async Task<PostContentModel> GetPostContentModelFromFormAsync(string siteId, string contentId)
		{
			var formValues = await this.Request.ReadFormAsync();
			Func<string, string> GetValue = key => formValues[key];

			var model = new PostContentModel()
			{
				Author = GetValue("author"),
				Label = GetValue("label"),
				PreviousVersionId = GetValue("previousVersionId"),
				ContentType = GetValue("contentType"),
				ContentStream = new MemoryStream(Encoding.UTF8.GetBytes(GetValue("content")))
			};

			return model;
		}


		// private async Task<PostContentModel> GetPostContentModelFromMultiPartAsync(string siteId, string contentId, HttpRequestMessage requestMessage)
		// {
		// 	var provider = await requestMessage.Content.ReadAsMultipartAsync(new MultipartMemoryStreamProvider());

		// 	var model = new PostContentModel();
		// 	foreach (var content in provider.Contents)
		// 	{
		// 		if (content.Headers.ContentDisposition.Name == "\"author\"")
		// 		{
		// 			model.Author = await content.ReadAsStringAsync();
		// 		}

		// 		if (content.Headers.ContentDisposition.Name == "\"label\"")
		// 		{
		// 			model.Label = await content.ReadAsStringAsync();
		// 		}

		// 		if (content.Headers.ContentDisposition.Name == "\"previousVersionId\"")
		// 		{
		// 			model.PreviousVersionId = await content.ReadAsStringAsync();
		// 		}

		// 		if (content.Headers.ContentDisposition.Name == "\"content\"")
		// 		{
		// 			model.ContentStream = await content.ReadAsStreamAsync();
		// 			model.ContentType = content.Headers.ContentType.MediaType;
		// 		}
		// 	}

		// 	return model;
		// }
	}
}
