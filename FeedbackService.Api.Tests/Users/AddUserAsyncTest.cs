using FakeItEasy;
using MakeItSimple.WebApi.Common;
using MakeItSimple.WebApi.Domain.Users;
using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Features.Users;
using MakeItSimple.WebApi.Services;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Api.Tests.Users
{

    public class AddUserAsyncTest
    {
        private readonly AddUserAsync _addUserAsync;
        private readonly IMediator _mediator;

        public AddUserAsyncTest()
        {
            // Arrange
            var mediatorMock = new Mock<IMediator>();
            _mediator = mediatorMock.Object;
            _addUserAsync = new AddUserAsync(_mediator);
        }

        private class TestAddUserCommand : AddUserAsync.AddNewUserCommand
        {
            public TestAddUserCommand()
            {
                firstname = "John";
                lastname = "Doe";
                username = "joedoe";
                password = "password123";
                confirm_password = "password1231";
                added_by = 123;
                user_role_id = 1;
                department_id = 1;
            }
        }

        [Fact]
        public async Task AddUser_ValidInput_ReturnsOkResult()
        {
            // Arrange
            var command = new TestAddUserCommand();

            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(i => i.FindFirst("id")).Returns(new Claim("id", "123"));
            var userPrincipal = new ClaimsPrincipal(identityMock.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrincipal
                }
            };

            _addUserAsync.ControllerContext = controllerContext;

            // Act
            var result = await _addUserAsync.AddUser(command);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var okResult = (OkObjectResult)result;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);

            var response = okResult.Value as CommandOrQueryResult<object>;
            Assert.NotNull(identityMock.Object);
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Single(response.Messages);
            Assert.Equal("User added Successfully", response.Messages[0]);
        }


        [Fact]
        public async Task AddUser_UsernameAlreadyExists_ReturnsConflictResult()
        {
            // Arrange
         
            var command = new TestAddUserCommand(); // Create an instance of the TestAddUserCommand class
  
            var exception = new UserAlreadyExistException($"Username '{command.username}' already exists");
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(command, default)).Throws(exception);
            var addUserAsyncWithMock = new AddUserAsync(mediatorMock.Object);

            // Act
            var result = await addUserAsyncWithMock.AddUser(command);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
            var conflictResult = (ConflictObjectResult)result;
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            Assert.Equal("joedoe" , command.username);
            var response = conflictResult.Value as CommandOrQueryResult<object>;
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Single(response.Messages);
            //Assert.Equal($"Username '{command.username}' already exists", response.Messages[0]);
        }


        [Fact]
        public async Task AddUser_PasswordIsInvalidException()
        {

            //Arrange 

            var command = new TestAddUserCommand();
            var exception = new PasswordIsInvalidException();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(command, default)).Throws(exception);
            var addUserAsyncWithMock = new AddUserAsync(mediatorMock.Object);

            //Act
            var result = await addUserAsyncWithMock.AddUser(command);

            //Assert
            Assert.IsType<ConflictObjectResult>(result);
            var conflictResult = (ConflictObjectResult)result;
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            var response = conflictResult.Value as CommandOrQueryResult<object>;
            Assert.NotEqual(command.password , command.confirm_password);
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Single(response.Messages);

        }


        [Fact]
        public async Task AddUser_UserRoleNotFoundException()

        {
            //Arrange

            var command = new TestAddUserCommand();
            var exception = new UserRoleNotFoundException();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(command, default)).Throws(exception);
            var addUserAsyncWithMock = new AddUserAsync(mediatorMock.Object);

            //Act
            var result = await addUserAsyncWithMock.AddUser(command);

            //Assert
            Assert.IsType<ConflictObjectResult>(result);
            var conflictResult = (ConflictObjectResult)result;
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            var response = conflictResult.Value as CommandOrQueryResult<object>;
            Assert.NotEqual(1, command.user_role_id);
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Single(response.Messages);

        }


        [Fact]
        public async Task AddUser_DeparmentNotFoundException()
        {
            //Arrange

            var command = new TestAddUserCommand();
            var exception = new DepartmentNotFoundException();
            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Send(command, default)).Throws(exception);
            var addUserAsyncWithMock = new AddUserAsync(mediatorMock.Object);

            //Act
            var result = await addUserAsyncWithMock.AddUser(command);

            //Assert
            Assert.IsType<ConflictObjectResult>(result);
            var conflictResult = (ConflictObjectResult)result;
            Assert.Equal(StatusCodes.Status409Conflict, conflictResult.StatusCode);
            var response = conflictResult.Value as CommandOrQueryResult<object>;
            Assert.NotEqual(1, command.department_id);
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Single(response.Messages);

        }







    }
}
