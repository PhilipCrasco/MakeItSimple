using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Common.Pagination;
using MakeItSimple.WebApi.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static MakeItSimple.WebApi.Features.Users.GetUserAsync;

namespace FeedbackService.Api.Tests.Users
{
    public class GetUserAsyncTest
    {
        private readonly IMediator _mediator;
        private readonly GetUserAsync _getUserAsync;

        public GetUserAsyncTest()
        {
            var mediatorMock = new Mock<IMediator>();
            _mediator = mediatorMock.Object;
            _getUserAsync = new GetUserAsync(_mediator);

        }






    }
}
