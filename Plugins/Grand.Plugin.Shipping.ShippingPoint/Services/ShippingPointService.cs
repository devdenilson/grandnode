﻿using System;
using System.Linq;
using Grand.Core;
using Grand.Core.Caching;
using Grand.Core.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Grand.Plugin.Shipping.ShippingPoint.Domain;
using System.Threading.Tasks;

namespace Grand.Plugin.Shipping.ShippingPoint.Services
{
    public class ShippingPointService : IShippingPointService
    {
        #region Constants

        private const string PICKUP_POINT_ALL_KEY = "Grand.ShippingPoint.all-{0}-{1}";
        private const string PICKUP_POINT_PATTERN_KEY = "Grand.ShippingPoint.";

        #endregion

        #region Fields

        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Domain.ShippingPoints> _shippingPointRepository;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="ShippingPointRepository">Store pickup point repository</param>
        public ShippingPointService(ICacheManager cacheManager,
            IRepository<Domain.ShippingPoints> ShippingPointRepository)
        {
            this._cacheManager = cacheManager;
            this._shippingPointRepository = ShippingPointRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all pickup points
        /// </summary>
        /// <param name="storeId">The store identifier; pass "" to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Pickup points</returns>
        public virtual async Task<IPagedList<ShippingPoints>> GetAllStoreShippingPoint(string storeId = "", int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from gp in _shippingPointRepository.Table
                        where (gp.StoreId == storeId || string.IsNullOrEmpty(gp.StoreId)) || storeId == ""
                        select gp;

            var records = await query.ToListAsync();

            //paging
            return await Task.FromResult(new PagedList<Domain.ShippingPoints>(records, pageIndex, pageSize));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pickupPointId"></param>
        /// <returns></returns>
        public virtual Task<ShippingPoints> GetStoreShippingPointByPointName(string pointName)
        {
            return (from shippingOoint in _shippingPointRepository.Table
                         where shippingOoint.ShippingPointName == pointName
                         select shippingOoint).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a pickup point
        /// </summary>
        /// <param name="pickupPointId">Pickup point identifier</param>
        /// <returns>Pickup point</returns>
        public virtual Task<ShippingPoints> GetStoreShippingPointById(string pickupPointId)
        {
            return _shippingPointRepository.GetByIdAsync(pickupPointId);
        }

        /// <summary>
        /// Inserts a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task InsertStoreShippingPoint(Domain.ShippingPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException("pickupPoint");

            await _shippingPointRepository.InsertAsync(pickupPoint);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Updates the pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task UpdateStoreShippingPoint(Domain.ShippingPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException("pickupPoint");

            await _shippingPointRepository.UpdateAsync(pickupPoint);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }

        /// <summary>
        /// Deletes a pickup point
        /// </summary>
        /// <param name="pickupPoint">Pickup point</param>
        public virtual async Task DeleteStoreShippingPoint(Domain.ShippingPoints pickupPoint)
        {
            if (pickupPoint == null)
                throw new ArgumentNullException("pickupPoint");

            await _shippingPointRepository.DeleteAsync(pickupPoint);
            _cacheManager.RemoveByPattern(PICKUP_POINT_PATTERN_KEY);
        }
        #endregion
    }
}
