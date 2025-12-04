global using System.ComponentModel.DataAnnotations.Schema;
global using System.Reflection;

global using LanguageExt;
global using LanguageExt.Traits.Domain;
global using LanguageExt.UnsafeValueAccess;
//global using Product = Product.Domain.Models.Product;
global using MassTransit;

global using MediatR;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
//global using MediatR;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Product.Application.Events;
global using Product.Application.Features.DeleteImages;
global using Product.Application.Features.DeleteProduct;
global using Product.Application.Features.GetAllBrands;
global using Product.Application.Features.GetAllCategories;
global using Product.Application.Features.GetAllColors;
global using Product.Application.Features.GetAllSizes;
global using Product.Domain.Contracts;
global using Product.Domain.ValueObjects;
global using Product.Persistence;
global using Product.Presentation.Requests;

global using Serilog;

global using Shared.Application.Abstractions;
global using Shared.Application.Contracts.Product.Queries;
global using Shared.Application.Contracts.Product.Results;
global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.Validations;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Clock;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Extensions;
global using Shared.Persistence.Interceptors;
global using Shared.Presentation;
global using Shared.Presentation.Extensions;

global using SixLabors.ImageSharp;

global using static LanguageExt.Prelude;
global using static Shared.Application.Contracts.Product.Constants;
global using static Shared.Persistence.Db.Monad.Db;

global using Color = Product.Domain.ValueObjects.Color;
global using Error = LanguageExt.Common.Error;
global using Size = Product.Domain.ValueObjects.Size;
global using Unit = LanguageExt.Unit;


