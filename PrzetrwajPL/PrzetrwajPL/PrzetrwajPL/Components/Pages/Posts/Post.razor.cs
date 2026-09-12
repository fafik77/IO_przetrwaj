using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Przetrwaj.CommonLibrary.Consts;
using Przetrwaj.CommonLibrary.Models;
using Przetrwaj.CommonLibrary.Models.Posts;
using Przetrwaj.CommonLibrary.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrzetrwajPL.Components.Pages.Posts;

public partial class Post
{
	[Parameter]
	public string Id { get; set; }

	[CascadingParameter]
	private Task<AuthenticationState> AuthStateTask { get; set; }

	private const string PostsApiPath = "/Posts";
	private const string VotePositiveApiName = "vote-positive";
	private const string VoteNegativeApiName = "vote-negative";
	private const string CommentApiName = "comment";

	private string PostApiEndpoint => $"{PostsApiPath}/{Id}";
	private string UpdateCommentApiEndpoint => $"{PostsApiPath}/comment";

	private PostCompleteDataDto? post;
	private bool isLoading = true;
	private bool isVoting = false;
	private string? currentUserId;
	private bool isAuthorOfPost = false;

	private bool showCommentForm = false;
	private bool isSendingComment = false;
	private string newCommentContent = "";
	private string? fullScreenImageUrl;

	[Inject]
	private NavigationManager Nav { get; set; } = default!;

	private bool isEditingPost = false;
	private bool isSavingPost = false;
	private UpdatePostCommand editPostCommand = new();
	private string? postValidationErrorMessage;

	private bool HasVoted => post?.MyVote?.IsUpvoteOrNull != null;
	private bool IsUpvoted => post?.MyVote?.IsUpvoteOrNull == true;
	private bool IsDownvoted => post?.MyVote?.IsUpvoteOrNull == false;

	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthStateTask;
		var userPrincipal = authState.User;
		currentUserId = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
		await LoadData();
	}

	private async Task LoadData()
	{
		isLoading = true;
		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			post = await client.GetFromJsonAsync<PostCompleteDataDto>(PostApiEndpoint);

			if (post != null && post.Author != null && !string.IsNullOrEmpty(currentUserId))
			{
				isAuthorOfPost = post.Author.Id == currentUserId;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Błąd: {ex.Message}");
		}
		finally
		{
			isLoading = false;
		}
	}

	private async Task VotePositive()
	{
		if (isVoting || HasVoted) return;
		isVoting = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.PostAsync($"{PostApiEndpoint}/{VotePositiveApiName}", null);

			if (response.IsSuccessStatusCode)
			{
				await LoadData();
			}
			else
			{
				Console.WriteLine("Błąd serwera przy głosowaniu." + response.StatusCode);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Błąd głosowania: " + ex.Message);
		}
		finally
		{
			isVoting = false;
		}
	}

	private async Task VoteNegative()
	{
		if (isVoting || HasVoted) return;
		isVoting = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.PostAsync($"{PostApiEndpoint}/{VoteNegativeApiName}", null);

			if (response.IsSuccessStatusCode)
			{
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Błąd głosowania: " + ex.Message);
		}
		finally
		{
			isVoting = false;
		}
	}

	private void ToggleCommentForm() => showCommentForm = !showCommentForm;

	private async Task SubmitComment()
	{
		if (string.IsNullOrWhiteSpace(newCommentContent)) return;
		isSendingComment = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var commentNew = new AddCommentCommand { Comment = newCommentContent };

			var response = await client.PostAsJsonAsync($"{PostApiEndpoint}/{CommentApiName}", commentNew);

			if (response.IsSuccessStatusCode)
			{
				newCommentContent = "";
				showCommentForm = false;
				await LoadData();
			}
			else
			{
				Console.WriteLine("Błąd serwera przy dodawaniu komentarza." + response.StatusCode);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Błąd dodawania komentarza: " + ex.Message);
		}
		finally
		{
			isSendingComment = false;
		}
	}

	private string FormatCommentTime(DateTimeOffset dateCreated)
	{
		var diff = DateTimeOffset.Now - dateCreated;
		if (diff.TotalHours < 23)
		{
			int hours = (int)diff.TotalHours;
			int minutes = diff.Minutes;
			if (hours > 0)
			{
				return $"{hours}h {minutes}min temu";
			}
			else
			{
				return $"{minutes}min temu";
			}
		}
		else
		{
			return dateCreated.ToString("HH:mm dd.MM.yyyy");
		}
	}

	private void ShowFullScreenImage(string url)
	{
		fullScreenImageUrl = url;
	}

	private void CloseFullScreenImage()
	{
		fullScreenImageUrl = null;
	}

	private void StartEditComment(CommentDto comment)
	{
		comment.InEditComment = comment.Comment;
		comment.IsEditing = true;
	}

	private void CancelEditComment(CommentDto comment)
	{
		comment.InEditComment = null;
		comment.IsEditing = false;
	}

	private async Task UpdateComment(CommentDto comment)
	{
		if (string.IsNullOrWhiteSpace(comment.InEditComment)) return;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var updateCmd = new AddCommentCommand { Comment = comment.InEditComment };

			var response = await client.PutAsJsonAsync($"{UpdateCommentApiEndpoint}/{comment.CommentId}", updateCmd);

			if (response.IsSuccessStatusCode)
			{
				comment.Comment = comment.InEditComment;
				comment.InEditComment = null;
				comment.IsEditing = false;
			}
			else
			{
				Console.WriteLine("Błąd serwera przy aktualizacji komentarza: " + response.StatusCode);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("Błąd aktualizacji komentarza: " + ex.Message);
		}
	}

	private void StartEditPost()
	{
		if (post == null) return;
		editPostCommand = new UpdatePostCommand
		{
			Title = post.Title,
			Description = post.Description,
			CustomCategory = post.Category?.IsCustom == true ? post.Category.Name : null
		};
		postValidationErrorMessage = null;
		isEditingPost = true;
	}

	private void CancelEditPost()
	{
		isEditingPost = false;
		postValidationErrorMessage = null;
		editPostCommand = new();
	}

	private async Task UpdatePost(UpdatePostCommand comm)
	{
		var validationContext = new ValidationContext(comm);
		var validationResults = new List<ValidationResult>();

		if (!Validator.TryValidateObject(comm, validationContext, validationResults, validateAllProperties: true))
		{
			postValidationErrorMessage = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
			return;
		}

		postValidationErrorMessage = null;
		isSavingPost = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.PatchAsJsonAsync(PostApiEndpoint, comm);

			if (response.IsSuccessStatusCode)
			{
				isEditingPost = false;
				await LoadData();
			}
			else
			{
				postValidationErrorMessage = $"Błąd serwera przy aktualizacji posta: {response.StatusCode}";
			}
		}
		catch (Exception ex)
		{
			postValidationErrorMessage = $"Błąd aktualizacji posta: {ex.Message}";
		}
		finally
		{
			isSavingPost = false;
		}
	}

	private async Task DeletePost()
	{
		if (post == null) return;

		string confirmMessage = $"Usunąć post {post.Title}?\nPost zostanie przeniesiony do archiwum i nie będzie już dostępny.\nJeśli potrzebujesz usunąć zdjęcie skontatkuj się z administratorem.";
		bool confirmed = await JS.InvokeAsync<bool>("confirm", confirmMessage);
		if (!confirmed) return;

		isSavingPost = true;

		try
		{
			var client = ClientFactory.CreateClient(Consts.PrzetrwajApiClientName);
			var response = await client.DeleteAsync(PostApiEndpoint);

			if (response.IsSuccessStatusCode)
			{
				Nav.NavigateTo("/list");
			}
			else
			{
				postValidationErrorMessage = $"Błąd serwera przy usuwaniu posta: {response.StatusCode}";
			}
		}
		catch (Exception ex)
		{
			postValidationErrorMessage = $"Błąd usuwania posta: {ex.Message}";
		}
		finally
		{
			isSavingPost = false;
		}
	}
}
