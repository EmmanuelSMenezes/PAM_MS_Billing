using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Newtonsoft.Json;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
  public class DeliveryOptionsRepository : IDeliveryOptionsRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public DeliveryOptionsRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

        public DeliveryOptionsResponse CreateDelivery(CreateDeliveryRequest createDeliveryRequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var sqlinsertproduct = @$"INSERT INTO logistics.actuation_area_delivery_option(
                                                name, 
                                                created_by) 
                                            VALUES(
                                               '{createDeliveryRequest.Name}', 
                                               '{createDeliveryRequest.Created_by}') RETURNING *;";

                    var inserted = connection.Query<DeliveryOptionsResponse>(sqlinsertproduct).ToList();

                    if (inserted.Count == 0)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileInsertFreightOnDB");
                    }

                    transaction.Commit();
                    connection.Close();


                    return inserted.FirstOrDefault();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ListDeliveryOptionsResponse GetDeliveryOptions(Filter filter)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    var sql = @$"SELECT * FROM logistics.actuation_area_delivery_option";

                    var getall = connection.Query<DeliveryOptions>(sql).ToList();

                    int totalRows = getall.Count();
                    float totalPages = (float)totalRows / (float)filter.itensPerPage;

                    totalPages = (float)Math.Ceiling(totalPages);

                    getall = getall.Skip((int)((filter.page - 1) * filter.itensPerPage)).Take((int)filter.itensPerPage).ToList();

                    return new ListDeliveryOptionsResponse() { Delivery_options = getall, Pagination = new Pagination() { totalPages = (int)totalPages, totalRows = totalRows } };

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DeliveryOptionsResponse ActiveDelivery(UpdateDeliveryRequest updateDeliveryRequest)
        {

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var sql = @$"UPDATE logistics.actuation_area_delivery_option SET	                               
                                            active = {updateDeliveryRequest.Active},
                                            updated_by = '{updateDeliveryRequest.Updated_by}',
                                            updated_at = CURRENT_TIMESTAMP
                                            WHERE delivery_option_id = '{updateDeliveryRequest.Delivery_options_id}' 
                                            RETURNING *;";

                    var updatedProduct = connection.Query<DeliveryOptionsResponse>(sql).ToList();

                    if (updatedProduct.Count == 0)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileUpdatedtDeliveryOnDB");
                    }

                    transaction.Commit();
                    connection.Close();


                    return updatedProduct.FirstOrDefault();

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
