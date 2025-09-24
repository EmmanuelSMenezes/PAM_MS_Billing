using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
    public class CardRepository : ICardRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        public CardRepository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public CardResponse CreateCardRepository(CardTokenResponse card, string requestcard, string responsecard)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"INSERT INTO consumer.card
                                (consumer_id, number, validity, created_by, name, card_id_pagseguro, brand, request, response)
                                 VALUES('{card.Consumer_id}','{card.First_digits}**{card.Last_digits}','{card.Exp_month}/{card.Exp_year}'
                                  ,'{card.Created_by}','{card.Holder.Name}','{card.Id}','{card.Brand}','{requestcard}','{responsecard}') RETURNING *;";

                    var response = connection.Query<CardResponse>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception )
            {
                throw;
            }
        }

        public CardResponse DeleteCardRepository(Guid card_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"DELETE FROM consumer.card WHERE card_id='{card_id}' RETURNING *;";

                    var response = connection.Query<CardResponse>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<CardResponse> GetCardRepository(Guid consumer_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM consumer.card WHERE consumer_id = '{consumer_id}';";
                    var response = connection.Query<CardResponse>(sql).ToList();

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while get consumer by consumer id");
                throw;
            }
        }

        public Consumer GetConsumerByConsumerIdRepository(Guid consumer_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM consumer.consumer WHERE consumer_id = '{consumer_id}';";
                    var response = connection.Query<Consumer>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while get consumer by consumer id");
                throw;
            }
        }

        public CardResponse UpdateCardRepository(CardTokenResponse card, string requestcard, string responsecard)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"UPDATE consumer.card
                                 SET number='{card.First_digits}**{card.Last_digits}'
                                   , name = '{card.Holder.Name}'
                                   , validity = '{card.Exp_month}/{card.Exp_year}'
                                   , updated_by = '{card.Created_by}'
                                   , updated_at = CURRENT_TIMESTAMP
                                   , card_id_pagseguro = '{card.Id}'
                                   , brand = '{card.Brand}'
                                   , request = '{requestcard}'
                                   , response = '{responsecard}'
                                 WHERE card_id = '{card.Card_id}' RETURNING *";

                    var response = connection.Query<CardResponse>(sql).FirstOrDefault();

                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
