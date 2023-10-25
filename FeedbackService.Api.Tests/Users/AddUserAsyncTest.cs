using MakeItSimple.WebApi.Features.ErrorException.UserException;
using MakeItSimple.WebApi.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Api.Tests.Users
{
   
    public class AddUserAsyncTest
    {
        private readonly AddUserAsync _controller;
        private readonly Mock<IMediator> _mediatorMock;

        public AddUserAsyncTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AddUserAsync(_mediatorMock.Object);
        }

        [Fact]
        public async Task AddUser_ReturnsOkResult()
        {
            // Arrange
            var command = CreateValidCommand();

            // Act
            var result = await _controller.AddUser(command);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_UsernameAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var command = CreateValidCommand();

            // Simulate a situation where the username already exists
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserAsync.AddNewUserCommand>(), default))
                .ThrowsAsync(new UserAlreadyExistException("johndoe"));

            // Act
            var result = await _controller.AddUser(command);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_DepartmentNotFound_ReturnsConflict()
        {
            // Arrange
            var command = CreateValidCommand();

            // Simulate a situation where the department is not found
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserAsync.AddNewUserCommand>(), default))
                .ThrowsAsync(new DepartmentNotFoundException());

            // Act
            var result = await _controller.AddUser(command);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        [Fact]
        public async Task AddUser_PasswordInvalid_ReturnsConflict()
        {
            // Arrange
            var command = CreateValidCommand();

            // Simulate a situation where the password is invalid
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddUserAsync.AddNewUserCommand>(), default))
                .ThrowsAsync(new PasswordIsInvalidException());

            // Act
            var result = await _controller.AddUser(command);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
        }

        // Helper method to create a valid command for testing
        private AddUserAsync.AddNewUserCommand CreateValidCommand()
        {
            return new AddUserAsync.AddNewUserCommand
            {
                firstname = "John",
                lastname = "Doe",
                username = "johndoe",
                password = "password",
                confirm_password = "passwords",
                added_by = null,
                user_role_id = 2,
                department_id = 3,
            };
        }

    }
}
