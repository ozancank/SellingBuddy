﻿using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BasketService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly IIdentityService _identityService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BasketController> _logger;

    public BasketController(IBasketRepository repository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
    {
        _repository = repository;
        _identityService = identityService;
        _eventBus = eventBus;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Basket Service is Up and Running");
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
    {
        var basket = await _repository.GetBasketAsync(id);
        return Ok(basket ?? new CustomerBasket(id));
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket value)
    {
        return Ok(await _repository.UpdateBasketAsync(value));
    }

    [HttpPost]
    [Route("additem")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
    {
        var userId = _identityService.GetUserName();

        var basket = await _repository.GetBasketAsync(userId);

        if (basket == null)
        {
            basket = new CustomerBasket(userId);
        }

        basket.Items.Add(basketItem);

        await _repository.UpdateBasketAsync(basket);

        return Ok();
    }

    [HttpPost]
    [Route("checkout")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
    {
        var userId = basketCheckout.Buyer;

        var basket = await _repository.GetBasketAsync(userId);

        if (basket == null)
        {
            return BadRequest();
        }

        var userName = _identityService.GetUserName();

        var eventMessage = new OrderCreatedIntegrationEvent(userId, userName, basketCheckout.City, basketCheckout.Street, basketCheckout.State, basketCheckout.Country,
            basketCheckout.ZipCode, basketCheckout.CardNumber, basketCheckout.CardHolderName, basketCheckout.CardExpiration, basketCheckout.CardSecurityNumber,
            basketCheckout.CardTypeId, basketCheckout.Buyer, basket);

        try
        {
            //listen itself to clean the basket
            //it islistened by OrderApi to start to
            _eventBus.Publish(eventMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Publishing integration Event: {IntegrationEventId} from {BasketService.App}", eventMessage.Id);
            throw;
        }

        return Accepted();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
    public async Task DeleteBasketByIdAsync(string id)
    {
        await _repository.DeleteBasketAsync(id);
    }
}