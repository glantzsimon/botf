using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;

namespace K9.WebApplication.Services.Stripe
{
    public class StripeRepository : IRepository<Charge>
    {
        private readonly IStripeService _stripeService;

        public StripeRepository(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        public int GetCount(string whereClause = "")
        {
            return GetCharges().Where(whereClause).Count();
        }

        public string GetName(string tableName, int id)
        {
            throw new NotImplementedException();
        }

        public List<Charge> GetQuery(string sql)
        {
            return GetCharges().Where(sql).ToList();
        }

        public List<Charge> List()
        {
            return GetCharges().ToList();
        }

        public List<ListItem> ItemList()
        {
            throw new NotImplementedException();
        }

        public List<TModel> CustomQuery<TModel>(string sql) where TModel : class
        {
            throw new NotImplementedException();
        }

        public void Create(Charge item)
        {
            throw new NotImplementedException();
        }

        public void CreateBatch(List<Charge> items)
        {
            throw new NotImplementedException();
        }

        public void Update(Charge item)
        {
            throw new NotImplementedException();
        }

        public void UpdateBatch(List<Charge> items)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteBatch(List<int> items)
        {
            throw new NotImplementedException();
        }

        public void Delete(Charge item)
        {
            throw new NotImplementedException();
        }

        public void DeleteBatch(List<Charge> items)
        {
            throw new NotImplementedException();
        }

        public bool Exists(int id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string query)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Expression<Func<Charge, bool>> expression)
        {
            return GetCharges().Where(expression).Any();
        }

        public List<Charge> Find(string name)
        {
            return GetCharges().Where(d => d.Name == name).ToList();
        }

        public IQueryable<Charge> Find(Expression<Func<Charge, bool>> expression)
        {
            return GetCharges().Where(expression);
        }

        public Charge Find(int id)
        {
            throw new NotImplementedException();
        }

        public List<Charge> GetBy<T2>(int id) where T2 : class, IObjectBase
        {
            throw new NotImplementedException();
        }

        public List<Charge> GetAllBy<T2, T3>(int id) where T2 : class, IObjectBase where T3 : class, IObjectBase
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private IQueryable<Charge> GetCharges()
        {
            return _stripeService.GetCharges().Select(
                c => new Charge
                {
                    Currency = c.Currency,
                    Customer = c.Customer.Description,
                    CustomerEmail = c.Customer.Email,
                    DonationDescription = c.Description,
                    DonatedOn = c.Created
                }).AsQueryable();
        }
    }
}