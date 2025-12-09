using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Dtos;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Przetrwaj.Presentation.Controllers;

/// <summary>
/// This is the Public General User endpoint.
/// Do not confuse this one with 'AccountController',
/// as this one does not return any sensitive data period.
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserController : Controller
{



	[HttpGet("{id}")]
	[SwaggerOperation("Get publiclu visible General data of user by id")]
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetById(string id)
	{
		throw new NotImplementedException();
	}


	[HttpGet("{id}/Posts")]
	[SwaggerOperation("Get all posts made by user id")]
	[ProducesResponseType(typeof(IEnumerable<PostDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllPosts(string id)
	{
		throw new NotImplementedException();
	}

	[HttpGet("{id}/Comments")]
	[SwaggerOperation("Get all comments made by user id")]
	[ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetAllComments(string id)
	{
		throw new NotImplementedException();
	}
}
