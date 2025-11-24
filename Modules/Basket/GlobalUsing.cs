global using System.Reflection;

global using Basket.Application.Events;
global using Basket.Application.Features.Cart.CreateCart;
global using Basket.Application.Features.Coupon.ExpireCoupon;
global using Basket.Application.Features.Coupon.GetCouponById;
global using Basket.Application.Features.Coupon.GetCouponByUserId;
global using Basket.Domain.Contracts;
global using Basket.Domain.Models;
global using Basket.Domain.ValueObjects;
global using Basket.Persistence;
global using Basket.Presentation.Requests;

global using Db.Errors;

global using LanguageExt;
global using LanguageExt.Common;
global using LanguageExt.Traits.Domain;

global using MassTransit;

global using MediatR;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Diagnostics;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

global using Serilog;

global using Shared.Application.Abstractions;
global using Shared.Application.Contracts.Product.Queries;
global using Shared.Domain.Abstractions;
global using Shared.Domain.Errors;
global using Shared.Domain.ValueObjects;
global using Shared.Infrastructure.Authentication;
global using Shared.Infrastructure.Clock;
global using Shared.Infrastructure.Errors;
global using Shared.Persistence.Db.Monad;
global using Shared.Persistence.Interceptors;
global using Shared.Presentation.Extensions;

global using static LanguageExt.Prelude;
global using static Shared.Persistence.Db.Monad.Db;

global using Permission = Shared.Infrastructure.Enums.Permission;
global using Unit = LanguageExt.Unit;
