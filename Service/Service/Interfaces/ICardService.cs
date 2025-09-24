using System;
using System.Collections.Generic;
using Domain.Model;

namespace Application.Service
{
    public interface ICardService
    {
        CardResponse CreateCardService(CreateCardRequest createCardRequest, string token);
        CardResponse UpdateCardService(UpdateCardRequest updateCardRequest, string token);
        List<CardResponse> GetCardService(Guid consumer_id);
        CardResponse DeleteCardService(Guid card_id);
    }
}
