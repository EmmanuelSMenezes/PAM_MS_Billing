using System;
using System.Collections.Generic;
using Domain.Model;

namespace Infrastructure.Repository
{
    public interface ICardRepository
    {
        CardResponse CreateCardRepository(CardTokenResponse createCardRequest, string request, string response);
        Consumer GetConsumerByConsumerIdRepository(Guid consumer_id);
        CardResponse UpdateCardRepository(CardTokenResponse updateCardRequest, string request, string response);
        List<CardResponse> GetCardRepository(Guid consumer_id);
        CardResponse DeleteCardRepository(Guid card_id);
    }
}
