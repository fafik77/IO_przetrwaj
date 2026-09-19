using MediatR;
using Microsoft.AspNetCore.Mvc;
using Przetrwaj.Application.Queries.Version;
using Przetrwaj.Domain.Models.Dtos;
using Przetrwaj.Presentation.Controllers;

namespace Przetrwaj.Tests.Unit.Version;

public class GetAppVersionQueryTests
{
	[Fact]
	public async Task Handle_ReturnsValidAppVersionDto()
	{
		// Arrange
		var handler = new GetAppVersionQueryHandler();
		var query = new GetAppVersionQuery();

		// Act
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.NotNull(result);
		Assert.False(string.IsNullOrWhiteSpace(result.Version));
		Assert.NotNull(result.Environment);
	}

	[Fact]
	public async Task VersionController_ReturnsOkWithVersionDto()
	{
		// Arrange
		var expectedDto = new AppVersionDto
		{
			Version = "1.0.0",
			InformationalVersion = "1.0.0+testcommit",
			CommitHash = "testcommit",
			BuildDate = DateTime.UtcNow,
			Environment = "Testing"
		};

		var mediator = new MockMediator(expectedDto);
		var controller = new VersionController(mediator);

		// Act
		var actionResult = await controller.GetVersion(CancellationToken.None);

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(actionResult);
		var model = Assert.IsType<AppVersionDto>(okResult.Value);
		Assert.Equal("1.0.0", model.Version);
		Assert.Equal("testcommit", model.CommitHash);
		Assert.Equal("Testing", model.Environment);
	}

	private class MockMediator : IMediator
	{
		private readonly AppVersionDto _dto;

		public MockMediator(AppVersionDto dto)
		{
			_dto = dto;
		}

		public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
		{
			if (typeof(TResponse) == typeof(AppVersionDto))
			{
				return Task.FromResult((TResponse)(object)_dto);
			}

			throw new NotImplementedException();
		}

		public Task<object?> Send(object request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest => throw new NotImplementedException();
		public Task Publish(object notification, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification => throw new NotImplementedException();
		public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
		public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}
}
