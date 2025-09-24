using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Domain.Model.Orders;
using Newtonsoft.Json;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        public PaymentRepository(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public PaymentOptionsResponse CreatePayment(CreatePaymentRequest createPaymentRequest)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var sqlinsertproduct = @$"INSERT INTO billing.payment_options(
                                                description, 
                                                created_by) 
                                            VALUES(
                                               '{createPaymentRequest.Description}', 
                                               '{createPaymentRequest.Created_by}') RETURNING *;";

                    var inserted = connection.Query<PaymentOptionsResponse>(sqlinsertproduct).ToList();

                    var sqlpaymentlocal = @$"INSERT INTO billing.payment_options_local
                                            (payment_local_id, created_by, payment_options_id)
                                            VALUES('34496a35-a666-4b79-aa83-fb5b88afea73','{createPaymentRequest.Created_by}',
                                            '{inserted.First().Payment_options_id}') RETURNING *";

                    var paymentlocal = connection.Query<dynamic>(sqlpaymentlocal).ToList();

                    if (!inserted.Any() || !paymentlocal.Any())
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileInsertPaymentOnDB");
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

        public ListPaymentOptionsResponse GetPaymentOptions(Filter filter)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    var sql = @$"SELECT po.*,pl.payment_local_id, pl.payment_local_name
                                    FROM billing.payment_options po
                                    JOIN billing.payment_options_local pol 
                                    ON pol.payment_options_id = po.payment_options_id
                                    JOIN billing.payment_local pl  
                                    ON pl.payment_local_id = pol.payment_local_id";

                    var getall = connection.Query<PaymentOptions>(sql).ToList();

                    int totalRows = getall.Count();
                    float totalPages = (float)totalRows / (float)filter.itensPerPage;

                    totalPages = (float)Math.Ceiling(totalPages);

                    getall = getall.Skip((int)((filter.page - 1) * filter.itensPerPage)).Take((int)filter.itensPerPage).ToList();

                    return new ListPaymentOptionsResponse() { Payments = getall, Pagination = new Pagination() { totalPages = (int)totalPages, totalRows = totalRows } };

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PaymentOptionsResponse ActivePayment(UpdatePaymentRequest updatePaymentRequest)
        {

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var sql = @$"UPDATE billing.payment_options SET	                               
                                            active = {updatePaymentRequest.Active},
                                            updated_by = '{updatePaymentRequest.Updated_by}',
                                            updated_at = CURRENT_TIMESTAMP
                                            WHERE payment_options_id = '{updatePaymentRequest.Payment_options_id}' 
                                            RETURNING *;";

                    var updatedProduct = connection.Query<PaymentOptionsResponse>(sql).ToList();

                    if (updatedProduct.Count == 0)
                    {
                        transaction.Dispose();
                        connection.Close();
                        throw new Exception("errorWhileUpdatedtPaymentOnDB");
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

        public List<OrderPix> GetOrdersPix()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT ph.response->>'id' as id, ph.order_id, o.created_by  
                                 FROM billing.payment_history ph 
                                 JOIN orders.orders o on o.order_id = ph.order_id 
                                 WHERE ph.status = 5 and NOT EXISTS (
                                 SELECT 1
                                 FROM billing.payment_history ph2
                                 WHERE ph2.order_id = ph.order_id
                                 AND ph2.status = 1)";

                    var response = connection.Query<OrderPix>(sql).ToList();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool CreatePaymentHistory(Guid order_id, string jsonpayment, int satus)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"INSERT INTO billing.payment_history
                                (order_id, response, status)
                                VALUES('{order_id}', '{jsonpayment}', {satus}) RETURNING *";

                    var response = connection.Query<dynamic>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }


        }

        public ValueMinimun GetPagSeguroValueMin(Guid partner_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {

                    var sql = @$"SELECT *
                                    FROM billing.pagseguro_value_minimum
                                    WHERE partner_id = '{partner_id}'";

                    var get = connection.Query<ValueMinimun>(sql).FirstOrDefault();


                    return get;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public bool CreateAccessTokenHistory(Partner partner, string jsonaccesstoken)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"INSERT INTO billing.access_token_history
                                (response, partner_id, created_by)
                                VALUES('{jsonaccesstoken}', '{partner.Partner_id}', '{partner.User_id}') RETURNING *";

                    var response = connection.Query<dynamic>(sql).FirstOrDefault();
                    if (response == null)
                    {
                        throw new Exception("");
                    }
                    return true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Partner GetPartnerByUserId(Guid user_id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"SELECT * FROM partner.partner WHERE user_id = '{user_id}'";

                    var response = connection.Query<Partner>(sql).FirstOrDefault();
                    
                    return response;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public BankPartner CreateBank(string account_id, Partner partner)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    var sql = @$"INSERT INTO partner.bank_details
                                    (partner_id, created_by, account_id)
                                 VALUES('{partner.Partner_id}', '{partner.User_id}', '{account_id}') RETURNING *";

                    var response = connection.Query<BankPartner>(sql).First();

                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[Repository - CreateBank]: Exception when creating bank details!");
                throw;
            }
        }
    }
}
