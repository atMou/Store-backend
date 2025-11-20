
global using System.ComponentModel.DataAnnotations.Schema;
global using System.IdentityModel.Tokens.Jwt;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;

global using Identity.Application.EventHandlers;
global using Identity.Application.Features.Login;
global using Identity.Application.Features.Logout;
global using Identity.Application.Features.Refresh;
global using Identity.Application.Features.Register;
global using Identity.Domain.Contracts;
global using Identity.Domain.IRepositories;
global using Identity.Domain.Models;
global using Identity.Domain.ValueObjects;
global using Identity.Infrastructure.OptionsSetup;
global using Identity.Persistence;
global using Identity.Persistence.Repositories;
global using Identity.Presentation.Extensions;
global using Identity.Presentation.Requests;
global using Identity.ValueObjects;

global using LanguageExt;
global using LanguageExt.Common;
global using LanguageExt.Sys.Traits;
global using LanguageExt.Traits.Domain;

global using MassTransit;

global using MediatR;

global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;

global using SendGrid.Helpers.Mail;

global using Serilog;

global using Shared.Application.Abstractions;
global using Shared.Application.Contracts.Queries;
global using Shared.Domain.Abstractions;
global using Shared.Domain.Contracts.User;
global using Shared.Domain.Errors;
global using Shared.Domain.Validations;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Clock;
global using Shared.Infrastructure.Email;
global using Shared.Infrastructure.Errors;
global using Shared.Infrastructure.Images;
global using Shared.Messaging.Events;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Interceptors;
global using Shared.Presentation.Extensions;

global using static LanguageExt.Prelude;

global using fileIO = LanguageExt.Sys.Live.Implementations.FileIO;
global using Unit = LanguageExt.Unit;
