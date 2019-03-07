using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
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
	public class ContentController : ControllerBase
	{
		private readonly ContentManagementService _contentManagementService;
		private readonly IAuthorizationPolicy _authPolicy;

		public ContentController(ContentManagementService contentManagementService, IAuthorizationPolicy authPolicy)
		{
			_contentManagementService = contentManagementService;
			_authPolicy = authPolicy;
		}

		[HttpGet, Route("content/{siteId}/{contentId}/versions")]
		public async Task<IActionResult> GetVersions(string siteId, string contentId)
		{
			if (!_authPolicy.AllowManageContent(siteId, contentId, User))
			{
				return new StatusCodeResult((int)(User.Identity.IsAuthenticated ? HttpStatusCode.Forbidden : HttpStatusCode.Unauthorized));
			}

			try
			{
				var versions = await _contentManagementService.GetVersionsAsync(siteId, contentId);
				return Ok(versions);
			}
			catch (Exception)
			{
				return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet, Route("content/{siteId}/{contentId}")]
		public async Task<IActionResult> GetPublishedContent(string siteId, string contentId)
		{
			string publishedVersionId = await _contentManagementService.GetCurrentlyPublishedVersionIdAsync(siteId, contentId, DateTimeOffset.Now);

			if (!_authPolicy.AllowGetCurrentPublishedContent(siteId, contentId, User))
			{
				return new StatusCodeResult( User.Identity.IsAuthenticated ? (int)HttpStatusCode.Forbidden : (int)HttpStatusCode.Unauthorized);
			}

			// //TODO:
			// var ifNoneMatchHeader = Request.Headers.IfNoneMatch.SingleOrDefault();
			// if (ifNoneMatchHeader != null
			// 	&& "\"" + publishedVersionId + "\"" == ifNoneMatchHeader.Tag)
			// {
			// 	var notModifiedMessage = new HttpResponseMessage(HttpStatusCode.NotModified);
			// 	return notModifiedMessage;
			// }

			if (publishedVersionId == null)
			{
				return new NotFoundResult();
			}

			var stream = new MemoryStream();
			var contentType = await _contentManagementService.DownloadContentToStreamAsync(siteId, contentId, publishedVersionId, stream);
			stream.Seek(0, SeekOrigin.Begin);

			return new FileStreamResult(stream, contentType){ EntityTag = new EntityTagHeaderValue($@"""{ publishedVersionId }""")};
		}

		[HttpGet, Route("content/{siteId}/{contentId}/{versionId}")]
		public async Task<IActionResult> GetContent(string siteId, string contentId, string versionId)
		{
			if (!_authPolicy.AllowManageContent(siteId, contentId, User))
			{
				return new StatusCodeResult( User.Identity.IsAuthenticated ? (int)HttpStatusCode.Forbidden : (int)HttpStatusCode.Unauthorized);
			}

			var stream = new MemoryStream();
			var contentType = await _contentManagementService.DownloadContentToStreamAsync(siteId, contentId, versionId, stream);
			stream.Seek(0, SeekOrigin.Begin);

			if (contentType == null)
			{
				return new NotFoundResult();
			}

			return new FileStreamResult(stream, contentType);
		}

		[HttpPost, Route("content/{siteId}/{contentId}/{versionId}/status")]
		public async Task<IActionResult> PostStatusChange(string siteId, string contentId, string versionId, [FromBody]StatusChangeModel statusModel)
		{
			if (!_authPolicy.AllowChangeContentStatus(siteId, contentId, User))
			{
				return new StatusCodeResult((int)(User.Identity.IsAuthenticated ? HttpStatusCode.Forbidden : HttpStatusCode.Unauthorized));
			}

			var now = DateTimeOffset.Now;
			await _contentManagementService.ChangeStatusForContentAsync(siteId, contentId, versionId, statusModel.Status, statusModel.Responsible, now, statusModel.StartDate);

			return await GetVersions(siteId, contentId);
		}

		//[HttpPut, Route("content/{site}/{contentId}")]
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


		//[HttpDelete, Route("content/{siteId}/{contentId}")]
		//public IHttpActionResult Delete(string siteId, string contentId)
		//{
		//	_contentVersioningService.DeleteContent(siteId, contentId);
		//	return StatusCode(HttpStatusCode.NoContent);
		//}
	}
}
